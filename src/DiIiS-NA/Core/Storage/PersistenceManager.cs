using System;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace DiIiS_NA.Core.Storage
{
	/// <summary>
	/// This attribute is used to tag properties that are persisted by the persistance manager.
	/// The class is mapped to a table with the same name as the class, and by default, each property is
	/// mapped to a column with the same name as the property (property, not the type of the property...)
	/// unless you override it by setting another name. To save and load arrays, you also have to define how
	/// many elements the array has.
	/// 
	public class PersistentPropertyAttribute : Attribute
	{
		public string Name { get; private set; }
		public int Count { get; private set; }

		public PersistentPropertyAttribute(string name) { Name = name; Count = 1; }
		public PersistentPropertyAttribute() { }
		public PersistentPropertyAttribute(string name, int count) { Name = name; Count = count; }
	}


	/// <summary>
	/// Loading classes from and saving classes to the mpq mirror database
	/// </summary>
	public class PersistenceManager
	{
		private class PersistentProperty
		{
			public static IEnumerable<PersistentProperty> GetPersistentProperties(Type t, string hierarchy, Type parent)
			{
				foreach (var property in t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
				{
					if (property.GetCustomAttributes(typeof(PersistentPropertyAttribute), false).Length > 0)
					{
						yield return new PersistentProperty(property, hierarchy, parent);
					}
				}
			}


			private string hierarchy;
			private Type parent;

			public PropertyInfo Property { get; private set; }
			public string ColumnName { get; private set; }
			public int ArrayCount { get; private set; }
			public bool IsGenericList
			{
				get
				{
					return (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
				}
			}

			public bool IsGenericDictionary
			{
				get
				{
					return (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>));
				}
			}

			public bool IsSimpleType
			{
				get
				{
					return Property.PropertyType.Namespace == "System" || Property.PropertyType.IsEnum;
				}
			}

			public bool IsInlinedType
			{
				get
				{
					return !(IsSimpleType || IsGenericDictionary || IsGenericList);
				}
			}

			public Type Type
			{
				get
				{
					return Property.PropertyType;
				}
			}
			public String RelationTableName
			{
				get
				{
					return (parent == null ? Property.DeclaringType.Name : parent.Name) + "_" + ListType.Name + "_" + ColumnName;
				}
			}

			public Type ListType
			{
				get
				{
					if (IsGenericList) return Type.GetGenericArguments()[0];
					if (IsGenericDictionary) return Type.GetGenericArguments()[1];
					throw new Exception("Not a list");
				}
			}

			public PersistentProperty(PropertyInfo p, string hierarchy, Type parent)
			{
				Property = p;
				this.parent = parent;
				PersistentPropertyAttribute pa = (PersistentPropertyAttribute)p.GetCustomAttributes(typeof(PersistentPropertyAttribute), false)[0];
				ColumnName = hierarchy + (pa.Name == null ? pa.Name : pa.Name);
				ArrayCount = pa.Count;
				this.hierarchy = hierarchy;

				if (Type.IsArray && ArrayCount == -1 && Type != typeof(byte[]))
					throw new Exception(String.Format("Field {0} is a dynamic array. Dynamic Arrays are only supported for bytes. Use a generic list instead.", p.Name));
			}
		}

		private static byte[] StringToByteArray(String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		private static bool tableExists(string name)
		{
			using (var cmd = new SqliteCommand(String.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'", name), DBManager.MPQMirror))
			{
				return (cmd.ExecuteScalar() != null);
			}
		}


		#region Loading

		/// <summary>
		/// Loads a persistent object from the database
		/// </summary>
		/// <param name="type">Type of the persisted object</param>
		/// <param name="id">Id of the persisted object</param>
		/// <returns></returns>
		public static object Load(Type type, string id)
		{
			var instance = Activator.CreateInstance(type);
			LoadPartial(instance, id);
			return instance;
		}

		/// <summary>
		/// Loads data from the database into an already existing object. This is mainly for mpq filetypes that are still loaded
		/// from mpq but have some of their properties 'zeroed out' which are now loaded from the database
		/// </summary>
		/// <param name="o">Object with tagged properties that are to be loaded from database</param>
		/// <param name="id">The id of the corresponding entry in the database</param>
		public static void LoadPartial(object o, string id)
		{
			// load the entry and begin reading
			if (tableExists(o.GetType().Name))
			{
				using (var cmd = new SqliteCommand(String.Format("SELECT * FROM {0} WHERE Id={1}", o.GetType().Name, id), DBManager.MPQMirror))
				{
					var reader = cmd.ExecuteReader();
					if (reader.Read() && reader.HasRows)
					{
						Load(o, null, reader);
					}
				}
			}
		}

		// statement used to query all list entries for a given property
		// TODO first projection, then join... not the other way around
		private static string genericListsql = "SELECT * FROM {0} JOIN {1} ON {0}.{1}Id = {1}.Id WHERE {2}Id = {3}";

		/// <summary>
		/// Loads properties of an object from the passed reader. For inlined properties, the parent reader is passed
		/// </summary>
		/// <param name="o">Object to be filled with data</param>
		/// <param name="parent">Parent type that has the id for inlined classes</param>
		/// <param name="reader">Reader from which to take the data</param>
		/// <param name="embeddedPrefix">Prefix for 'inlined' properties (complex types)</param>
		private static void Load(object o, Type parent, SqliteDataReader reader, string embeddedPrefix = "")
		{
			foreach (var property in PersistentProperty.GetPersistentProperties(o.GetType(), embeddedPrefix, parent))
			{
				//if (!reader.Read()) continue;
				string entryId = reader["Id"].ToString();

				// Load generic lists by finding the mn-mapping table and loading every entry recursivly
				if (property.IsGenericList)
				{
					using (var cmd = new SqliteCommand(String.Format(genericListsql, property.RelationTableName, property.ListType.Name, parent == null ? o.GetType().Name : parent.Name, entryId), DBManager.MPQMirror))
					{
						var itemReader = cmd.ExecuteReader();
						var list = Activator.CreateInstance(property.Type);

						if (itemReader.HasRows)
						{
							while (itemReader.Read())
							{
								var item = Activator.CreateInstance(property.ListType);
								Load(item, null, itemReader);
								(list as IList).Add(item);
							}
						}
						property.Property.SetValue(o, list, null);

					}
					continue;
				}

				// Load generic dictionaires by finding the mn-mapping table and loading every entry recursivly
				if (property.IsGenericDictionary)
				{
					using (var cmd = new SqliteCommand(String.Format(genericListsql, property.RelationTableName, property.ListType.Name, parent == null ? o.GetType().Name : parent.Name, entryId), DBManager.MPQMirror))
					{
						var itemReader = cmd.ExecuteReader();
						var dictionary = Activator.CreateInstance(property.Type);

						if (itemReader.HasRows)
						{
							while (itemReader.Read())
							{
								var item = Activator.CreateInstance(property.ListType);
								Load(item, null, itemReader);
								(dictionary as IDictionary).Add(Convert.ChangeType(itemReader["Key"], property.Type.GetGenericArguments()[0]), item);
							}
						}
						property.Property.SetValue(o, dictionary, null);

					}
					continue;
				}

				// load scalar types
				if (property.Type.Namespace == "System")
				{
					// load array of scalar types. The column name of the i-th array entry is "columnName_i"
					if (property.Type.IsArray)
					{
						if (property.ArrayCount == -1)
						{
							byte[] blob = StringToByteArray(reader[property.ColumnName].ToString().Replace("-", ""));
							property.Property.SetValue(o, blob, null);
						}
						else
						{
							Array vals = (Array)Activator.CreateInstance(property.Type, property.ArrayCount);
							for (int i = 0; i < vals.Length; i++)
							{
								vals.SetValue(Convert.ChangeType(reader[property.ColumnName + "_" + i.ToString()], property.Type.GetElementType()), i);
							}

							property.Property.SetValue(o, vals, null);
						}
					}
					else
					{
						property.Property.SetValue(o, Convert.ChangeType(reader[property.ColumnName], property.Type), null);
					}
					continue;
				}

				// load enums
				if (property.Type.IsEnum)
				{
					property.Property.SetValue(o, Enum.Parse(property.Type, reader[property.ColumnName].ToString(), true), null);
					continue;
				}

				// if its none of the earlier types, its a inlined class. class properties
				if (Convert.ToBoolean(reader[property.ColumnName + "_"]))
				{
					var embedded = Activator.CreateInstance(property.Type);
					Load(embedded, o.GetType(), reader, property.ColumnName + "_");
					property.Property.SetValue(o, embedded, null);
				}
			}
		}

		#endregion

		#region Table creation

		private static void CreateTableForType(Type type, Type parent, Dictionary<string, Type> values, string embeddedPrefix = "")
		{
			// Save all scalar and inline types first, so we have the new entry id for our mn-table later
			foreach (var property in PersistentProperty.GetPersistentProperties(type, embeddedPrefix, parent))
			{
				if (property.IsSimpleType)
				{
					// save array of basic types
					if (property.Type.IsArray && property.ArrayCount != -1)
					{
						for (int i = 0; i < property.ArrayCount; i++)
						{
							values.Add(property.ColumnName + "_" + i, property.Property.PropertyType);
						}
					}
					else
					{
						values.Add(property.ColumnName, property.Type);
					}
					continue;
				}

				if (property.IsGenericList || property.IsGenericDictionary)
				{
					string query = "";

					if (property.IsGenericList)
					{
						query = "CREATE TABLE {0} ({1}Id NUMERIC, {2}Id NUMERIC)";
					}

					if (property.IsGenericDictionary)
					{
						query = "CREATE TABLE {0} ({1}Id NUMERIC, {2}Id NUMERIC, Key TEXT)";
					}

					query = String.Format(query, property.RelationTableName, parent == null ? type.Name : parent.Name, property.ListType.Name);

					using (var cmd = new SqliteCommand(query, DBManager.MPQMirror))
					{
						cmd.ExecuteNonQuery();
					}

					CreateTableForType(property.ListType, null, new Dictionary<string, Type>());
					continue;
				}

				values.Add(property.ColumnName + "_", typeof(string));
				CreateTableForType(property.Type, parent == null ? type : parent, values, property.ColumnName + "_");
			}

			// Only create tables for parent classes
			if (parent == null && !tableExists(type.Name))
			{
				if (type.Namespace == "System")
				{
					values.Add("Value", typeof(string));
				}

				var columnDefinitions = values.Select(v => v.Key + " " + ((v.Value == typeof(String) || v.Value == typeof(byte[])) ? "TEXT" : "NUMERIC")).ToList<string>();
				columnDefinitions.Add("Id INTEGER PRIMARY KEY");
				var tableDefinition = String.Join(",", columnDefinitions.ToArray<string>());

				using (var cmd = new SqliteCommand(String.Format("CREATE TABLE {0} ({1})", type.Name, tableDefinition), DBManager.MPQMirror))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		#endregion

		#region Saving

		/// <summary>
		/// Saves an object in the database. Use this for objects that are read from mpq
		/// </summary>
		/// <param name="o">Object to save</param>
		/// <param name="id">Id of the object</param>
		public static void SavePartial(object o, string id)
		{
			if (!tableExists(o.GetType().Name))
			{
				CreateTableForType(o.GetType(), null, new Dictionary<string, Type>());
			}

			Save(o, id, new Dictionary<string, string>());
		}

		/// <summary>
		/// Saves an asset in the database and creates an entry in the db table of contents. Use
		/// this for assets completly load from the database.
		/// </summary>
		/// <param name="o">Object to save</param>
		/// <param name="name">Descriptive name of the asset</param>
		/// <param name="id">Id to identify the object with or null to create a new id</param>
		public static void Save(object o, string name, string id = null)
		{
			if (!tableExists(o.GetType().Name))
			{
				CreateTableForType(o.GetType(), null, new Dictionary<string, Type>());
			}

			id = Save(o, id, new Dictionary<string, string>());

			using (var cmd = new SqliteCommand(String.Format("INSERT INTO TOC (SNOGroup, SNOId, Name) VALUES ('{0}', '{1}', '{2}')", o.GetType().Name, id, name), DBManager.MPQMirror))
			{
				cmd.ExecuteNonQuery();
			}
		}

		private static string Save(object o, string id, Dictionary<string, string> values, string embeddedPrefix = "")
		{
			id = SaveBasic(o, null, id, values, embeddedPrefix);
			SaveEnumerations(o, null, id, embeddedPrefix);
			return id;
		}

		/// <summary>
		/// Save object with all inlined classes to table
		/// </summary>
		/// <param name="o">Object to save</param>
		/// <param name="parent">Parent type (Type of outmost, not inlined object)</param>
		/// <param name="id">Id to use to save or null to create a new</param>
		/// <param name="values">Dictionary in wich inlined classes write their property values</param>
		/// <param name="embeddedPrefix">Prefix to use in inlined classes so all inlined properties have unique column names</param>
		/// <returns>Id used to save the object</returns>
		private static string SaveBasic(object o, Type parent, string id, Dictionary<string, string> values, string embeddedPrefix = "")
		{
			if (o == null) return "";

			// save scalar types
			if (o.GetType().Namespace != "System" && !o.GetType().IsEnum)
			{

				// Save all scalar and inline types first, so we have the new entry id for our mn-table later
				foreach (var property in PersistentProperty.GetPersistentProperties(o.GetType(), embeddedPrefix, parent))
				{
					if (property.IsSimpleType)
					{
						if (property.Type.IsArray)
						{
							if (property.ArrayCount == -1)
							{
								values.Add(property.ColumnName, "'" + BitConverter.ToString((byte[])property.Property.GetValue(o, null)) + "'");
							}
							else
							{
								for (int i = 0; i < property.ArrayCount; i++)
								{
									values.Add(property.ColumnName + "_" + i, "'" + (property.Property.GetValue(o, null) as Array).GetValue(i).ToString() + "'");
								}
							}
						}
						else
						{
							values.Add(property.ColumnName, "'" + property.Property.GetValue(o, null).ToString() + "'");
						}
					}

					// save complex object as inlined class
					if (property.IsInlinedType)
					{
						values.Add(property.ColumnName + "_", "'" + (property.Property.GetValue(o, null) != null).ToString() + "'");
						SaveBasic(property.Property.GetValue(o, null), parent == null ? o.GetType() : parent, id, values, property.ColumnName + "_");
					}
				}

			}
			else
			{
				values.Add("Value", o.ToString());
			}

			// No parent means this class is not inlined. Add a new entry with all saved values in the class table
			if (parent == null)
			{

				if (id != null)
				{
					values.Add("Id", id);
				}

				string cnames = String.Join(",", (new List<string>(values.Keys).ToArray()));
				string cvalues = String.Join(",", (new List<string>(values.Values).ToArray()));

				using (var cmd = new SqliteCommand(String.Format("INSERT INTO {0} ({1}) VALUES ({2})", o.GetType().Name, cnames, cvalues), DBManager.MPQMirror))
				{
					cmd.ExecuteNonQuery();

					using (var last = new SqliteCommand("SELECT last_insert_rowid()", DBManager.MPQMirror))
					{
						id = last.ExecuteScalar().ToString();
					}
				}
			}

			return id;
		}

		/// <summary>
		/// Saves all generic lists and dictionaries of the object and all inlined classes. The object already has to have an id!
		/// </summary>
		/// <param name="o">Object of which to save lists and dictionaries</param>
		/// <param name="parent">Parent (not inlined) Type</param>
		/// <param name="id">Id of the object</param>
		/// <param name="embeddedPrefix">Prefix used to create unique mn table names for all inlined lists and dictionaries</param>
		private static void SaveEnumerations(object o, Type parent, string id, string embeddedPrefix = "")
		{
			if (o == null) return;

			// Save all scalar and inline types first, so we have the new entry id for our mn-table later
			foreach (var property in PersistentProperty.GetPersistentProperties(o.GetType(), embeddedPrefix, parent))
			{
				if (property.IsInlinedType)
				{
					SaveEnumerations(property.Property.GetValue(o, null), parent == null ? o.GetType() : parent, id, property.ColumnName + "_");
				}

				if (property.IsGenericList)
				{
					IList list = (IList)property.Property.GetValue(o, null);

					foreach (var item in list)
					{
						string newId = Save(item, null, new Dictionary<string, string>(), "");

						using (var cmd = new SqliteCommand(String.Format(
							"INSERT INTO {4} ({0}Id, {1}Id) VALUES ({2}, {3})",
							parent == null ? o.GetType().Name : parent.Name,
							property.ListType.Name,
							id,
							newId,
							property.RelationTableName
							), DBManager.MPQMirror))
						{
							cmd.ExecuteNonQuery();
						}
					}
				}

				if (property.IsGenericDictionary)
				{
					IDictionary dictionary = (IDictionary)property.Property.GetValue(o, null);

					foreach (var item in dictionary.Keys)
					{
						string newId = Save(dictionary[item], null, new Dictionary<string, string>(), "");

						using (var cmd = new SqliteCommand(String.Format(
							"INSERT INTO {4} ({0}Id, {1}Id, Key) VALUES ({2}, {3}, {5})",
							parent == null ? o.GetType().Name : parent.Name,
							property.ListType.Name,
							id,
							newId,
							property.RelationTableName,
							item
							), DBManager.MPQMirror))
						{
							cmd.ExecuteNonQuery();
						}
					}
				}

			}
		}

		#endregion
	}
}
