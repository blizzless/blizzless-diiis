//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.resources.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using Google.ProtocolBuffers;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x3d, serviceName: "bnet.protocol.resources.Resources")]
    public class ResourceService : bgs.protocol.resources.v1.ResourcesService, IServerService
    {
		private static readonly Logger Logger = LogManager.CreateLogger();
		private static byte[] PFTY_HASH = new byte[] { (byte)0xCF, (byte)0x61, (byte)0xE0, (byte)0x81, (byte)0x09, (byte)0x19, (byte)0xC6, (byte)0xA6, (byte)0xF9, (byte)0xC1, (byte)0xCB, (byte)0x24, (byte)0xB3, (byte)0xC6, (byte)0x9D, (byte)0x03, (byte)0xB0, (byte)0x37, (byte)0x08, (byte)0xEC, (byte)0x16, (byte)0xD9, (byte)0x44, (byte)0x51, (byte)0xC5, (byte)0x1F, (byte)0x90, (byte)0x38, (byte)0xE9, (byte)0x09, (byte)0xA7, (byte)0x5A };
		private static byte[] NEW_PFTY_HASH = new byte[] { 0x06, 0xCD, 0x1B, 0x9A, 0x6E, 0xC5, 0x80, 0xE4, 0xCF, 0xF7, 0xB0, 0x42, 0xA0, 0x53, 0x19, 0x07, 0x59, 0xC3, 0xA1, 0x45, 0x4B, 0xC7, 0x9D, 0xBB, 0x6D, 0x3E, 0xFF, 0x2C, 0xB4, 0x16, 0x8B, 0x61 };
		public override void GetContentHandle(IRpcController controller, ContentHandleRequest request, Action<ContentHandle> done)
        {

			Logger.Trace("GetContentHandle(): ProgramId: 0x{0:X8} StreamId: 0x{1:X8}", request.Program, request.Stream);
			if (request.Program == (uint)FieldKeyHelper.Program.BNet)
			{
				var builder = ContentHandle.CreateBuilder()
					.SetRegion(21843)
					.SetUsage(0x70667479) //pfty - ProfanityFilter
					.SetHash(ByteString.CopyFrom(NEW_PFTY_HASH));

				done(builder.Build());
			}
			else if (request.Program == (uint)FieldKeyHelper.Program.D3)
			{
				var builder = ContentHandle.CreateBuilder()
					.SetRegion(0x5553)
					.SetUsage(0x643373)
					.SetProtoUrl("https://prod.depot.battle.net/${hash}.${usage}");
					;
					
				switch (request.Stream)
				{
					case 0x61637473: //acts - Available Acts
						builder.SetHash(ByteString.CopyFrom("bd9e8fc323fe1dbc1ef2e0e95e46355953040488621933d0685feba5e1163a25".ToByteArray()));
						break;
					case 0x71756573: //ques - Available Quests
						builder.SetHash(ByteString.CopyFrom("9303df8f917e2db14ec20724c04ea5d2af4e4cb6c72606b67a262178b7e18104".ToByteArray()));
						break;
					case 0x72707273: //rprs - RichPresence
						builder.SetHash(ByteString.CopyFrom("8F9D8409EA441140E5676823BA867EC56CB2D3D4DF6FC187BA46CBD2855EF799".ToByteArray()));
						break;
					case 0x61706674: //apft - ProfanityFilter
						builder.SetHash(ByteString.CopyFrom("de1862793fdbabb6eb1edec6ad1c95dd99e2fd3fc6ca730ab95091d694318a24".ToByteArray()));
						break;
					default:
						Logger.Warn("Unknown StreamId: 0x{0:X8}", request.Stream);
						builder.SetHash(ByteString.Empty);
						(controller as HandlerController).Status = 4;
						break;
				}
				done(builder.Build());
			}
		}
    }
}
