//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem
{
	public abstract class GameMessage
	{
		protected static readonly Logger Logger = LogManager.CreateLogger();

		private static readonly Dictionary<Opcodes, Type> MessageTypes = new Dictionary<Opcodes, Type>();
		private static readonly Dictionary<Opcodes, Consumers> MessageConsumers = new Dictionary<Opcodes, Consumers>();

		static GameMessage()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(GameMessage))) continue;

				var attributes = (MessageAttribute[])type.GetCustomAttributes(typeof(MessageAttribute), true);
				if (attributes.Length == 0) continue;
				foreach (MessageAttribute attribute in attributes)
				{
					foreach (var opcode in attribute.Opcodes)
					{
						if (MessageTypes.ContainsKey(opcode))
						{
							Logger.Fatal("Duplicate opcode detected: {0}", opcode.ToString());
						}
						else
						{
							MessageTypes.Add(opcode, type);
							MessageConsumers.Add(opcode, attribute.Consumer);
						}
					}
				}
			}
		}

		public static T Allocate<T>(Opcodes opcode) where T : GameMessage
		{
			if (!MessageTypes.ContainsKey(opcode))
			{
				Logger.Debug("Unimplemented message: " + opcode.ToString());
				return null;
			}

			var ctorWithParameterExists = MessageTypes[opcode].GetConstructor(new[] { typeof(Opcodes) }) != null;
			var msg = (T)Activator.CreateInstance(MessageTypes[opcode], ctorWithParameterExists ? new object[] { opcode } : null);

			msg.Id = (int)opcode;
			msg.Consumer = MessageConsumers[opcode];
			return msg;
		}

		public static GameMessage ParseMessage(GameBitBuffer buffer)
		{
			int id = buffer.ReadInt(10);
			var msg = Allocate<GameMessage>((Opcodes)id);
			if (msg == null) return null;

			msg.Id = id;
			msg.Parse(buffer);
			return msg;
		}

		protected GameMessage() 
		{
			if (this is Message.Definitions.Encounter.BossEncounterMessage)
				(this as Message.Definitions.Encounter.BossEncounterMessage).Id = 295;
			//UIElementMessage
		}

		protected GameMessage(int id)
		{
			Id = id;
		}

		protected GameMessage(Opcodes opcode)
		{
			Id = (int)opcode;
		}

		public int Id { get; set; }
		public Consumers Consumer { get; set; }

		public abstract void Parse(GameBitBuffer buffer);
		public abstract void Encode(GameBitBuffer buffer);
		public abstract void AsText(StringBuilder b, int pad);

		public string AsText()
		{
			var builder = new StringBuilder();
			builder.AppendLine("GameMessage(0x" + Id.ToString("X4") + ") - Opcode - " + Id);
			AsText(builder, 0);
			return builder.ToString();
		}
	}
}
