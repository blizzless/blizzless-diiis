//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol.presence.v1;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.LoginServer.Helpers
{
	public class FieldKeyHelper
	{
		public enum Program : uint
		{
			BNet = 16974,
			D3 = 17459,
			S2 = 21298,
			WoW = 5730135,
		}

		public enum OriginatingClass : uint
		{
			Account = 1,
			GameAccount = 2,
			Hero = 3,
			Party = 4,
			GameSession = 5
		}

		public static FieldKey Create(Program program, OriginatingClass originatingClass, uint field, ulong index)
		{
			return
				FieldKey.CreateBuilder().SetProgram((uint)program).SetGroup((uint)originatingClass).SetField(
					field).SetUniqueId(index).Build();
		}


		private HashSet<FieldKey> _changedFields = new HashSet<FieldKey>();
		private Dictionary<FieldKey, FieldOperation> _FieldValues = new Dictionary<FieldKey, FieldOperation>();

		public void SetFieldValue(FieldKey key, FieldOperation operation)
		{
			if (!_changedFields.Contains(key))
				_changedFields.Add(key);

			_FieldValues[key] = operation;
		}

		//TODO: Use covariance and refactor this
		public void SetPresenceFieldValue(IPresenceField field)
		{
			if (field != null)
			{
				SetFieldValue(field.GetFieldKey(), field.GetFieldOperation());
			}
		}

		//TODO: Use covariance and refactor this
		public void SetIntPresenceFieldValue(IntPresenceField field)
		{
			if (field != null)
			{
				var key = Create(field.Program, field.OriginatingClass, field.FieldNumber, field.Index);
				this.SetFieldValue(key, field.GetFieldOperation());
			}
		}

		//TODO: Use covariance and refactor this
		public void SetStringPresenceFieldValue(StringPresenceField field)
		{
			if (field != null)
			{
				var key = Create(field.Program, field.OriginatingClass, field.FieldNumber, field.Index);
				this.SetFieldValue(key, field.GetFieldOperation());
			}
		}

		public List<FieldOperation> GetChangedFieldList()
		{
			return new List<FieldOperation>(_FieldValues.Values);
		}

		public void ClearChanged()
		{
			this._changedFields.Clear();
			this._FieldValues.Clear();
		}

	}
}
