//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection
{
	[Message(Opcodes.PingMessage)]
	public class PingMessage : GameMessage, ISelfHandler
	{
		public int PingCounter;

		public void Handle(GameClient client)
		{
			client.SendMessage(new PongMessage()
			{
				Counter = PingCounter
			});
		}

		public override void Parse(GameBitBuffer buffer)
		{
			PingCounter = buffer.ReadInt(32);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, PingCounter);
		}

		public override void AsText(StringBuilder b, int pad)
		{

		}
	}

	[Message(Opcodes.PongMessage)]
	public class PongMessage : GameMessage
	{
		public int Counter;

		public PongMessage() : base(Opcodes.PongMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			Counter = buffer.ReadInt(32);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, Counter);
		}

		public override void AsText(StringBuilder b, int pad)
		{

		}
	}

	[Message(Opcodes.SpingMessage)]
	public class SPingMessage : GameMessage, ISelfHandler
	{
		public void Handle(GameClient client)
		{
			client.SendMessage(new SpongMessage()
			{
				
			});
		}

		public override void Parse(GameBitBuffer buffer)
		{

		}

		public override void Encode(GameBitBuffer buffer)
		{

		}

		public override void AsText(StringBuilder b, int pad)
		{

		}
	}

	[Message(Opcodes.SpongMessage)]
	public class SpongMessage : GameMessage
	{

		public SpongMessage() : base(Opcodes.PongMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{

		}

		public override void Encode(GameBitBuffer buffer)
		{

		}

		public override void AsText(StringBuilder b, int pad)
		{

		}
	}
}
