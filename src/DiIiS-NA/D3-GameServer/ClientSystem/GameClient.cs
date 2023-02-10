using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.ClientSystem.Base;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Tick;
using DiIiS_NA.LoginServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem
{
	public class GameClient : IClient
	{
		private static readonly Logger Logger = LogManager.CreateLogger("GC");

		public IConnection Connection { get; set; }
		public BattleClient BnetClient { get; set; }

		//private readonly GameBitBuffer _incomingBuffer = new GameBitBuffer(512);
		private readonly GameBitBuffer _outgoingBuffer = new GameBitBuffer(ushort.MaxValue);

		private object _clientStreamLock = new object();

		public Game Game { get; set; }
		public Player Player { get; set; }

		protected bool _tickingEnabled = false;

		public bool TickingEnabled
		{
			get
			{
				return _tickingEnabled;
			}
			set
			{
				_tickingEnabled = value;
				//if (value == true)
				//this.SendTick();
			}
		}

		public object _bufferLock = new object(); // we should be locking on this private object, locking on gameclient (this) may cause deadlocks. detailed information: http://msdn.microsoft.com/fr-fr/magazine/cc188793%28en-us%29.aspx /raist.

		public bool IsLoggingOut;

		public GameClient(IConnection connection)
		{
			TickingEnabled = false;
			Connection = connection;
			_outgoingBuffer.WriteInt(32, 0);
		}

		public virtual void Parse(ConnectionDataEventArgs e)
		{
			//Console.WriteLine(e.Data.Dump());

			//lock (_clientStreamLock)
			//{
			Task.Run(() =>
			{
				lock (_clientStreamLock)
				{
					var cancelToken = new CancellationTokenSource();
					//Task.Delay(5000, cancelToken.Token).ContinueWith((task) => { Logger.Warn("Character {0} caused server CPU overload!", this.Player.Toon.Name); this.Game.Dispose(); }, TaskContinuationOptions.NotOnCanceled);
					try
					{
						GameBitBuffer _incomingBuffer = new GameBitBuffer(512);

						_incomingBuffer.AppendData(e.Data.ToArray());

						while (Connection.IsOpen() && _incomingBuffer.IsPacketAvailable())
						{
							int end = _incomingBuffer.Position;
							end += _incomingBuffer.ReadInt(32) * 8;

							while ((end - _incomingBuffer.Position) >= 9 && Connection.IsOpen())
							{
								var message = _incomingBuffer.ParseMessage();
								//217
								//
								if (message == null) continue;
								try
								{
									Logger.LogIncomingPacket(message); // change ConsoleTarget's level to Level.Dump in program.cs if u want to see messages on console.
									if (message.Id == 96 || message.Id == 369 || message.Id == 269)
										message.Consumer = Consumers.Inventory;
									if (message.Consumer != Consumers.None)
									{
										if (message.Consumer == Consumers.ClientManager) ClientManager.Instance.Consume(this, message); // Client should be greeted by ClientManager and sent initial game-setup messages.
										else Game.Route(this, message);
									}

									else if (message is ISelfHandler) (message as ISelfHandler).Handle(this); // if message is able to handle itself, let it do so.
									else if (message.Id != 217)
										Logger.Warn("{0} - ID:{1} has no consumer or self-handler.", message.GetType().Name, message.Id);

								}
								catch (NotImplementedException)
								{
									Logger.Warn("Unhandled game message: 0x{0:X4} {1}", message.Id, message.GetType().Name);
								}
							}

							_incomingBuffer.Position = end;
						}
						_incomingBuffer.ConsumeData();
						//Thread.Sleep(5);
					}
					catch (Exception ex)
					{
						Logger.WarnException(ex, "Parse() exception: ");
					}
					finally
					{
						cancelToken.Cancel();
					}
				}
			});
			//}
		}

		private int LastReplicatedTick = 0;

		public virtual void SendMessage(GameMessage message)
		{
			//System.Threading.Thread.Sleep(50);
			lock (_outgoingBuffer)
			{
				if (Game.TickCounter > LastReplicatedTick && TickingEnabled && !(message is GameTickMessage) /*&& !(message is EndOfTickMessage)*/ && !Player.BetweenWorlds)
				{
					/*var endMessage = new EndOfTickMessage()
					{
							Field0 = this.Game.TickCounter,
							Field1 = this.LastReplicatedTick
					};
					Logger.LogOutgoingPacket(endMessage);
					_outgoingBuffer.EncodeMessage(endMessage);
					Connection.Send(_outgoingBuffer.GetPacketAndReset());*/

					LastReplicatedTick = Game.TickCounter;
					var tickMessage = new GameTickMessage(Game.TickCounter);
					Logger.LogOutgoingPacket(tickMessage);
					_outgoingBuffer.EncodeMessage(tickMessage);
					Connection.Send(_outgoingBuffer.GetPacketAndReset());
					dataSent = false;
				}
				//if (message is GameTickMessage)
				//message = new GameTickMessage(this.Game.TickCounter); //reassigning new tick value
				Logger.LogOutgoingPacket(message);
				_outgoingBuffer.EncodeMessage(message); // change ConsoleTarget's level to Level.Dump in program.cs if u want to see messages on console.

				if (TickingEnabled)
				{
					var data = _outgoingBuffer.GetPacketAndReset();
					Connection.Send(data);
				}
				dataSent = true;
				//if (flushImmediately) this.SendTick();
			}
		}

		public void SendBytes(byte[] data)
		{
			Connection.Send(data);
		}

		protected bool dataSent = true;

		public bool OpenWorldDefined = false;

		public void SendTick()
		{
			//if (_outgoingBuffer.Length <= 32) return;
			lock (_outgoingBuffer)
			{
				if (!dataSent) return;

				if (TickingEnabled && Game.TickCounter > LastReplicatedTick)
				{
					/*this.SendMessage(new EndOfTickMessage()
					{
						Field0 = this.Game.TickCounter,
						Field1 = this.LastReplicatedTick
					}); // send the tick end.*/
					SendMessage(new GameTickMessage(Game.TickCounter)); // send the tick.
					LastReplicatedTick = Game.TickCounter;
					//this.SendMessage(new GameTickMessage(0)); //before client enters game causes freeze with PvP scoreboard
					/*this.SendMessage(new EndOfTickMessage()
					{
						Field0 = this.Game.TickCounter,
						Field1 = 0
					}); // send the tick end*/
					dataSent = false;
					FlushOutgoingBuffer();
				}
			}
		}

		public virtual void FlushOutgoingBuffer()
		{
			lock (_outgoingBuffer)
			{
				if (_outgoingBuffer.Length <= 32) return;

				var data = _outgoingBuffer.GetPacketAndReset();
				Connection.Send(data);
			}
		}
	}
}
