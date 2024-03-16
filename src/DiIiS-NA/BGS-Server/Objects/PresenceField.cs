using DiIiS_NA.LoginServer.Helpers;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiIiS_NA.LoginServer.Objects
{
	public class EntityIdPresenceFieldList
	{
		public List<bgs.protocol.EntityId> Value = new();

		protected FieldKeyHelper.Program _program;
		protected FieldKeyHelper.OriginatingClass _originatingClass;
		protected uint _fieldNumber;
		protected uint _index;

		public EntityIdPresenceFieldList(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index)
		{
			_fieldNumber = FieldNumber;
			_index = Index;
			_program = Program;
			_originatingClass = OriginatingClass;
		}

		public List<bgs.protocol.presence.v1.FieldOperation> GetFieldOperationList()
		{
			var operationList = new List<bgs.protocol.presence.v1.FieldOperation>();

			foreach (var id in Value)
			{
				var Key = FieldKeyHelper.Create(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, _fieldNumber, id.High);
				var Field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(Key).SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(id).Build()).Build();
				operationList.Add(bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(Field).Build());
			}
			return operationList;
		}
	}

	public class BoolPresenceField : PresenceField<bool>, IPresenceField
	{
		public BoolPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, bool defaultValue = default(bool))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetBoolValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class EntityIdPresenceField : PresenceField<bgs.protocol.EntityId>, IPresenceField
	{
		public EntityIdPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, bgs.protocol.EntityId defaultValue = default(bgs.protocol.EntityId))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetEntityIdValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class UintPresenceField : PresenceField<ulong>, IPresenceField
	{
		public UintPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, ulong defaultValue = default(ulong))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetUintValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetUintValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class IntPresenceField : PresenceField<long>, IPresenceField
	{
		public IntPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, long defaultValue = default(long))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class FourCCPresenceField : PresenceField<String>, IPresenceField
	{
		public FourCCPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, string defaultValue = default(string))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetFourccValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetFourccValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class StringPresenceField : PresenceField<String>, IPresenceField
	{
		public StringPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, string defaultValue = default(string))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(Value).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(Value).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public class ByteStringPresenceField<T> : PresenceField<T>, IPresenceField where T : IMessageLite<T> //Used IMessageLite to get ToByteString(), might need refactoring later
	{
		public ByteStringPresenceField(FieldKeyHelper.Program Program, FieldKeyHelper.OriginatingClass OriginatingClass, uint FieldNumber, uint Index, T defaultValue = default(T))
			: base(Program, OriginatingClass, FieldNumber, Index, defaultValue)
		{
		}

		public bgs.protocol.presence.v1.Field GetField()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder().SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(Value.ToByteString()).Build()).Build();
			return field;
		}

		public bgs.protocol.presence.v1.FieldOperation GetFieldOperation()
		{
			var fieldKey = FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
			var field = bgs.protocol.presence.v1.Field.CreateBuilder();
			if (Value == null)
				field.SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(ByteString.Empty).Build()).Build();
			else
				field.SetKey(fieldKey).SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(Value.ToByteString()).Build()).Build();
			return bgs.protocol.presence.v1.FieldOperation.CreateBuilder().SetField(field).Build();
		}
	}

	public abstract class PresenceField<T>
	{
		public T Value;

		public FieldKeyHelper.Program Program { get; private set; }
		public FieldKeyHelper.OriginatingClass OriginatingClass { get; private set; }
		public uint FieldNumber { get; private set; }
		public uint Index { get; private set; }

		public PresenceField(FieldKeyHelper.Program program, FieldKeyHelper.OriginatingClass originatingClass, uint fieldNumber, uint index, T defaultValue)
		{
			Value = defaultValue;
			FieldNumber = fieldNumber;
			Index = index;
			Program = program;
			OriginatingClass = originatingClass;
		}

		public bgs.protocol.presence.v1.FieldKey GetFieldKey()
		{
			return FieldKeyHelper.Create(Program, OriginatingClass, FieldNumber, Index);
		}
	}

	public interface IPresenceField
	{
		bgs.protocol.presence.v1.Field GetField();
		bgs.protocol.presence.v1.FieldOperation GetFieldOperation();
		bgs.protocol.presence.v1.FieldKey GetFieldKey();
	}
}
