//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Attribute;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
	public class GameAttributeMap
	{
		private static Logger Logger = LogManager.CreateLogger();

		public struct KeyId
		{
			public int Id;
			public int? Key;

			public override bool Equals(object obj)
			{
				if (obj is KeyId)
				{
					var other = (KeyId)obj;
					if (Key.HasValue != other.Key.HasValue)
						return false;
					if (Key.HasValue && Key.Value != other.Key.Value)
						return false;
					return Id == other.Id;
				}
				return false;
			}

			public override int GetHashCode()
			{
				if (Key.HasValue)
					return Id | (Key.Value << 12);
				return Id;
			}
		}

		public HashSet<KeyId> _changedAttributes = new HashSet<KeyId>();
		public Dictionary<KeyId, GameAttributeValue> _attributeValues = new Dictionary<KeyId, GameAttributeValue>();
		private WorldObject _parent; 

		public GameAttributeMap(WorldObject parent)
		{
			_parent = parent;
		}

		public bool Replicateable(GameAttribute attribute)
		{
			if (_parent is Player || _parent is Hireling)
				return attribute.Flags.HasFlag(ReplicationFlags.PlayerReplicated) || attribute.Flags.HasFlag(ReplicationFlags.PlayerReplicated2);
			if (_parent is Item)
				return attribute.Flags.HasFlag(ReplicationFlags.ItemReplicated);
			if (_parent is Living)
				return attribute.Flags.HasFlag(ReplicationFlags.LivingReplicated);
			if (_parent is Gizmo)
				return attribute.Flags.HasFlag(ReplicationFlags.GizmoReplicated);
			return (attribute.Flags != 0);
		}

		public string Serialize()
		{
			string serialized = "";
			foreach (var pair in _attributeValues)
			{

				var gameAttribute = GameAttribute.Attributes[pair.Key.Id];//GameAttribute.GetById(pair.Key.Id);

				if (serialized.Length > 0)
					serialized += ";";

				var values = RawGetAttributeValue(gameAttribute, pair.Key.Key);

				var ValueF = Convert.ToString(values.ValueF);
				float testFloat = 0.0f;

				if (!float.TryParse(ValueF, out testFloat))
				{
					ValueF = "0.0";
					Logger.Error("Could not save ValueF to DB, saving 0 instead of {0}", pair.Value.ValueF);
				}

				serialized += string.Format("{0},{1}:{2}|{3}", pair.Key.Id, pair.Key.Key, values.Value, ValueF);
			}
			return serialized;//.ZipCompress();
		}

		public void FillBySerialized(string serializedGameAttributeMapCompressed)
		{
			var serializedGameAttributeMap = serializedGameAttributeMapCompressed;//.UnZipCompress();
			_attributeValues.Clear();
			if (serializedGameAttributeMapCompressed == "")
			{
				return;
			}
			var pairs = serializedGameAttributeMap.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var pair in pairs)
			{
				try
				{

					var pairParts = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

					if (pairParts.Length != 2)
					{
						Logger.Error("GA Deserializated error, skipping Bad Pair.");
						continue;
					}
					var values = pairParts[1].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					var valueI = int.Parse(values[0].Trim());
					var valueF = 0.0f;
					if (!float.TryParse(values[1].Trim(), out valueF))
					{
						Logger.Debug("Error Parsing ValueF");
					}

					var keyData = pairParts[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					var attributeId = int.Parse(keyData[0].Trim());
					var gameAttribute = GameAttribute.Attributes[attributeId];// .GetById(attributeId);

					if (gameAttribute.ScriptFunc != null && !gameAttribute.ScriptedAndSettable)
						continue;
					int? attributeKey = null;
					if (keyData.Length > 1)
					{
						attributeKey = int.Parse(keyData[1].Trim());
					}


					var val = RawGetAttributeValue(gameAttribute, attributeKey);
					val.ValueF = valueF;
					val.Value = valueI;
					RawSetAttributeValue(gameAttribute, attributeKey, val);
				}
				catch (Exception exception)
				{
					Logger.ErrorException(exception, "Error setting GA Value \"{0}\"", pair);
				}
			}

		}
		#region message broadcasting

		public void SendMessage(GameClient client)
		{
			lock (attrLock)
			{
				var list = GetMessageList(client);
				foreach (var msg in list)
					client.SendMessage(msg);
				_changedAttributes.Clear();
			}
		}

		public void SendMessage(IEnumerable<GameClient> clients)
		{
			lock (attrLock)
			{
				foreach (var client in clients)
				{
					var list = GetMessageList(client);
					foreach (var msg in list)
					{
						client.SendMessage(msg);
					}
				}
				_changedAttributes.Clear();
			}
		}

		private object attrLock = new object();
		/// <summary>
		/// Send only the changed attributes. How nice is that?
		/// You should generaly use Broadcast if possible
		/// </summary>
		/// <param name="client">the client we send it to</param>
		public void SendChangedMessage(GameClient client)
		{
			lock (attrLock)
			{
				var list = GetChangedMessageList(client);
				foreach (var msg in list)
					client.SendMessage(msg);
				_changedAttributes.Clear();
			}
		}

		public void SendChangedMessage(IEnumerable<GameClient> clients)
		{
			lock (attrLock)
			{
				if (_changedAttributes.Count == 0)
					return;
				foreach (var client in clients)
				{
					var list = GetChangedMessageList(client);
					foreach (var msg in list)
					{
						client.SendMessage(msg);
					}
				}
				_changedAttributes.Clear();
			}
		}

		/// <summary>
		/// Broadcasts attribs to players that the parent actor has been revealed to.
		/// </summary>
		public void BroadcastIfRevealed()
		{
			if (_parent.World != null)
				SendMessage(_parent.World.Players.Values
					.Where(@player => @player.RevealedObjects.ContainsKey(_parent.GlobalID))
					.Select(@player => @player.InGameClient));
		}

		/// <summary>
		/// Broadcasts changed attribs to players that the parent actor has been revealed to.
		/// </summary>
		public void BroadcastChangedIfRevealed()
		{
			if (_parent.World != null)
				SendChangedMessage(_parent.World.Players.Values
					.Where(@player => @player.RevealedObjects.ContainsKey(_parent.GlobalID))
					.Select(@player => @player.InGameClient));
		}

		#endregion

		public void ClearChanged()
		{
			lock (attrLock)
			{
				_changedAttributes.Clear();
			}
		}

		private List<GameMessage> GetMessageList(GameClient client)
		{
			lock (attrLock)
				return GetMessageListFromEnumerator(new HashSet<KeyId>(_attributeValues.Keys), client);
		}

		private List<GameMessage> GetChangedMessageList(GameClient client)
		{
			lock (attrLock)
				return GetMessageListFromEnumerator(new HashSet<KeyId>(_changedAttributes), client);
		}

		private List<GameMessage> GetMessageListFromEnumerator(HashSet<KeyId> attributes, GameClient client)
		{
			var messageList = new List<GameMessage>();

			if (client.Player == null) return messageList;
			if (!client.Player.RevealedObjects.ContainsKey(_parent.GlobalID)) return messageList;
			//103,:0|0;
			//375,:0|0"
			//378,:5|7E-45;
			//396,:1|1E-45;
			//401,:0|0;
			//400,:2007581535|6.86613E+33;
			//* 
			foreach (var attr in attributes.Where(a => GameAttribute.Attributes[a.Id].Id == 408).ToList())
				if (_parent is Item)
					attributes.Remove(attr);
			
			var ids = attributes.GetEnumerator();

			bool level = attributes.Contains(new KeyId() { Id = GameAttribute.Level.Id });
			int count = attributes.Count;

			if (count == 0)
				return messageList;

			try
			{
				if (count == 1)
				{
					AttributeSetValueMessage msg = new AttributeSetValueMessage();
					if (!ids.MoveNext())
						throw new Exception("Expected value in enumerator.");

					var keyid = ids.Current;
					var value = _attributeValues[keyid];

					int id = keyid.Id;
					msg.ActorID = _parent.DynamicID(client.Player);
					msg.Attribute = new NetAttributeKeyValue();

					msg.Attribute.KeyParam = keyid.Key;
					// FIXME: need to rework NetAttributeKeyValue, and maybe rename GameAttribute to NetAttribute?
					msg.Attribute.Attribute = GameAttribute.Attributes[id]; // FIXME
					if (msg.Attribute.Attribute.IsInteger)
						msg.Attribute.Int = value.Value;
					else
						msg.Attribute.Float = value.ValueF;

					messageList.Add(msg);
				}
				else
				{
					// FIXME: probably need to rework AttributesSetValues as well a bit
					if (count >= 10)
					{
						for (; count >= 10; count -= 10)
						{
							AttributesSetValuesMessage msg = new AttributesSetValuesMessage();
							msg.ActorID = _parent.DynamicID(client.Player);
							msg.atKeyVals = new NetAttributeKeyValue[10];
							for (int i = 0; i < 10; i++)
								msg.atKeyVals[i] = new NetAttributeKeyValue();
							for (int i = 0; i < 10; i++)
							{
								KeyId keyid;
								if (!ids.MoveNext())
								{
									if (level)
									{
										keyid = new KeyId { Id = GameAttribute.Level.Id };
										level = false;
									}
									else
									{
										throw new Exception("Expected values in enumerator.");
									}
								}
								else
								{
									keyid = ids.Current;
								}

								var kv = msg.atKeyVals[i];
								if (level && keyid.Id == GameAttribute.Level.Id)
								{
									i--;
									continue;
								}

								var value = _attributeValues[keyid];
								int id = keyid.Id;

								
								
								kv.KeyParam = keyid.Key;
								kv.Attribute = GameAttribute.Attributes[id];
								if (kv.Attribute.IsInteger)
									kv.Int = value.Value;
								else
									kv.Float = value.ValueF;
							}
							messageList.Add(msg);
						}
					}

					if (count > 0)
					{
						AttributesSetValuesMessage msg = new AttributesSetValuesMessage();
						msg.ActorID = _parent.DynamicID(client.Player);
						msg.atKeyVals = new NetAttributeKeyValue[count];
						for (int i = 0; i < count; i++)
						{
							KeyId keyid;
							if (!ids.MoveNext())
							{
								if (level)
								{
									keyid = new KeyId { Id = GameAttribute.Level.Id };
									level = false;
								}
								else
								{
									throw new Exception("Expected values in enumerator.");
								}
							}
							else
							{
								keyid = ids.Current;
							}
							var kv = new NetAttributeKeyValue();
							msg.atKeyVals[i] = kv;

							if (level && keyid.Id == GameAttribute.Level.Id)
							{
								i--;
								continue;
							}


							var value = _attributeValues[keyid];
							var id = keyid.Id;

							kv.KeyParam = keyid.Key;
							kv.Attribute = GameAttribute.Attributes[id];
							if (kv.Attribute.IsInteger)
								kv.Int = value.Value;
							else
								kv.Float = value.ValueF;
						}
						messageList.Add(msg);
					}

				}
			}
			catch (Exception ex)
			{
				Logger.WarnException(ex, "GetMessageListFromEnumerator() exception caught:");
			}
			return messageList;
		}


		private GameAttributeValue GetAttributeValue(GameAttribute attribute, int? key)
		{
			if (attribute.ScriptFunc != null)
				return attribute.ScriptFunc(this, key);
			else
				return RawGetAttributeValue(attribute, key);
		}

		private GameAttributeValue RawGetAttributeValue(GameAttribute attribute, int? key)
		{
			KeyId keyid;
			keyid.Id = attribute.Id;
			keyid.Key = key;

			GameAttributeValue gaValue;
			if (_attributeValues.TryGetValue(keyid, out gaValue))
				return gaValue;
			return attribute._DefaultValue;
		}

		private void SetAttributeValue(GameAttribute attribute, int? key, GameAttributeValue value)
		{
			// error if scripted attribute and is not settable
			if (attribute.ScriptFunc != null && !attribute.ScriptedAndSettable)
			{
				var frame = new System.Diagnostics.StackFrame(2, true);
				Logger.Error("illegal value assignment for GameAttribute.{0} attempted at {1}:{2}",
					attribute.Name, frame.GetFileName(), frame.GetFileLineNumber());
			}

			if (attribute.EncodingType == GameAttributeEncoding.IntMinMax)
			{
				if (value.Value < attribute.Min.Value || value.Value > attribute.Max.Value)
					throw new ArgumentOutOfRangeException("GameAttribute." + attribute.Name.Replace(' ', '_'), "Min: " + attribute.Min.Value + " Max: " + attribute.Max.Value + " Tried to set: " + value.Value);
			}
			else if (attribute.EncodingType == GameAttributeEncoding.Float16)
			{
				if (value.ValueF < GameAttribute.Float16Min || value.ValueF > GameAttribute.Float16Max)
					throw new ArgumentOutOfRangeException("GameAttribute." + attribute.Name.Replace(' ', '_'), "Min: " + GameAttribute.Float16Min + " Max " + GameAttribute.Float16Max + " Tried to set: " + value.ValueF);
			}

			RawSetAttributeValue(attribute, key, value);
		}

		private void RawSetAttributeValue(GameAttribute attribute, int? key, GameAttributeValue value)
		{
			KeyId keyid;
			keyid.Id = attribute.Id;
			keyid.Key = key;

			lock (attrLock)
			{
				_attributeValues[keyid] = value;

				if (!_changedAttributes.Contains(keyid))
					_changedAttributes.Add(keyid);
			}

			// mark dependant attributes as changed
			if (attribute.Dependents != null)
			{
				foreach (var dependent in attribute.Dependents)
				{
					int? usekey;

					if (dependent.IsManualDependency)
						usekey = dependent.Key;
					else
						usekey = dependent.UsesExplicitKey ? null : key;

					if (dependent.IsManualDependency || dependent.UsesExplicitKey == false || dependent.Key == key)
					{
						// TODO: always update dependent values for now, but eventually make this lazy
						RawSetAttributeValue(dependent.Attribute, usekey, dependent.Attribute.ScriptFunc(this, usekey));
					}
				}
			}
		}

		public int this[GameAttributeI attribute]
		{
			get { return GetAttributeValue(attribute, null).Value; }
			set { SetAttributeValue(attribute, null, new GameAttributeValue(value)); }
		}

		public int this[GameAttributeI attribute, int? key]
		{
			get { return GetAttributeValue(attribute, key).Value; }
			set { SetAttributeValue(attribute, key, new GameAttributeValue(value)); }
		}

		public float this[GameAttributeF attribute]
		{
			get { return GetAttributeValue(attribute, null).ValueF; }
			set { SetAttributeValue(attribute, null, new GameAttributeValue(value)); }
		}

		public float this[GameAttributeF attribute, int? key]
		{
			get { return GetAttributeValue(attribute, key).ValueF; }
			set { SetAttributeValue(attribute, key, new GameAttributeValue(value)); }
		}

		public bool this[GameAttributeB attribute]
		{
			get { return GetAttributeValue(attribute, null).Value != 0; }
			set { SetAttributeValue(attribute, null, new GameAttributeValue(value ? 1 : 0)); }
		}

		public bool this[GameAttributeB attribute, int? key]
		{
			get { return GetAttributeValue(attribute, key).Value != 0; }
			set { SetAttributeValue(attribute, key, new GameAttributeValue(value ? 1 : 0)); }
		}

		#region Raw attribute accessors
		// NOTE: these are public, but only exist to be used by GameAttribute scripts.
		// They provide raw attribute access of values, no scripts will be triggered when used.
		public int _RawGetAttribute(GameAttributeI attribute, int? key)
		{
			return RawGetAttributeValue(attribute, key).Value;
		}

		public float _RawGetAttribute(GameAttributeF attribute, int? key)
		{
			return RawGetAttributeValue(attribute, key).ValueF;
		}

		public bool _RawGetAttribute(GameAttributeB attribute, int? key)
		{
			return RawGetAttributeValue(attribute, key).Value != 0;
		}
		#endregion


		public IEnumerable<int> ActiveIds
		{
			get { return _attributeValues.Select(k => k.Key.Id); }
		}
		public int?[] AttributeKeys(GameAttribute ga)
		{
			return _attributeValues.Where(av => av.Key.Id == ga.Id).Select(av => av.Key.Key).ToArray();
		}

		public void LogAll()
		{
			foreach (var pair in _attributeValues)
			{
				Logger.Debug("attribute {0}, {1} => {2}", GameAttribute.Attributes[pair.Key.Id].Name, pair.Key.Key, (GameAttribute.Attributes[pair.Key.Id] is GameAttributeF ? pair.Value.ValueF : pair.Value.Value));
			}
		}

		public bool Contains(GameAttribute attr)
		{
			foreach (var pair in _attributeValues)
			{
				if (pair.Key.Id == attr.Id)
				{
					return true;
				}
			}
			return false;
		}
	}
}
