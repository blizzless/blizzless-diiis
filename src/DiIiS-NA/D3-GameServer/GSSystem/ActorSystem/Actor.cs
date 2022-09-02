//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public abstract class Actor : WorldObject
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// ActorSNO.
		/// </summary>
		public SNOHandle ActorSNO { get; private set; }

		/// <summary>
		/// Gets or sets the sno of the actor used to identify the actor to the player
		/// This is usually the same as actorSNO except for actors that have a GBHandle
		/// There are few exceptions though like the Inn_Zombies that have both.
		/// Used by ACDEnterKnown to name the actor.
		/// </summary>
		public int NameSNOId { get; set; }

		public bool Disable = false;

		public bool Spawner = false;

		/// <summary>
		/// The actor type.
		/// </summary>
		public abstract ActorType ActorType { get; }

		public object _payloadLock = new object();

		/// <summary>
		/// Current scene for the actor.
		/// </summary>
		public virtual Scene CurrentScene
		{
			get { return this.World.QuadTree.Query<Scene>(this.Bounds).FirstOrDefault(); }
		}

		/// <summary>
		/// Returns true if actor is already spawned in the world.
		/// </summary>
		public bool Spawned { get; private set; }
		public int GroupId = 0;
		public int NumberInWorld = 0;
		/// <summary>
		/// Default lenght value for region based queries.
		/// </summary>
		public const int DefaultQueryProximityLenght = 240;

		/// <summary>
		/// Default lenght value for range based queries.
		/// </summary>
		public int DefaultQueryProximityRadius = 100;
		public float LastSecondCasts = 0;

		/// <summary>
		/// PRTransform for the actor.
		/// </summary>
		public virtual PRTransform Transform
		{
			get { return new PRTransform { Quaternion = new Quaternion { W = this.RotationW, Vector3D = this.RotationAxis }, Vector3D = this.Position }; }
		}

		/// <summary>
		/// Replaces the actor's rotation with one that rotates along the Z-axis by the specified "facing" angle. 
		/// </summary>
		/// <param name="facingAngle">The angle in radians.</param>
		public void SetFacingRotation(float facingAngle)
		{
			if (!this.Spawner)
			{
				Quaternion q = Quaternion.FacingRotation(facingAngle);
				this.RotationW = q.W;
				this.RotationAxis = q.Vector3D;
			}
		}

		/// <summary>
		/// Tags read from MPQ's for the actor.
		/// </summary>
		public TagMap Tags { get; private set; }

		/// <summary>
		/// Attribute map.
		/// </summary>
		public GameAttributeMap Attributes { get; set; } //TODO: this needs to be "private set", but without errors on speed modifications

		/// <summary>
		/// Affix list.
		/// </summary>
		public List<Affix> AffixList { get; set; }

		/// <summary>
		/// GBHandle.
		/// </summary>
		public GBHandle GBHandle { get; private set; }

		/// <summary>
		/// Collision flags.
		/// </summary>
		public int CollFlags { get; set; }

		/// <summary>
		/// Check for summoned monsters
		/// </summary>
		public bool HasLoot { get; set; }

		public bool Dead = false;

		/// <summary>
		/// Gets whether the actor is visible by questrange, privately set on quest progress
		/// </summary>
		public bool Visible { get; private set; }

		/// <summary>
		/// The QuestRange specifies the visibility of an actor, depending on quest progress
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.QuestRange _questRange;

		protected DiIiS_NA.Core.MPQ.FileFormats.ConversationList ConversationList;
		public Vector3D CheckPointPosition { get; set; }
		public Vector3D CurrentDestination { get; set; }

		/// <summary>
		/// Returns true if actor has world location.
		/// TODO: I belive this belongs to WorldObject.cs /raist.
		/// </summary>
		public virtual bool HasWorldLocation
		{
			get { return true; }
		}

		/// <summary>
		/// The info set for actor. (erekose)
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.Actor ActorData
		{
			get
			{
				return (DiIiS_NA.Core.MPQ.FileFormats.Actor)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Actor][this.ActorSNO.Id].Data;
			}
		}

		/// <summary>
		/// The animation set for actor.
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.AnimSet AnimationSet
		{
			get
			{
				if (this.ActorData.AnimSetSNO != -1)
					return (DiIiS_NA.Core.MPQ.FileFormats.AnimSet)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.AnimSet][this.ActorData.AnimSetSNO].Data;
				else
					return null;
			}
		}

		public float WalkSpeed = 0.108f;

		public int Field2 = 0x00000000; 
		public int Field7 = -1;   

		
		public virtual int Quality { get; set; }

		public byte Field10 = 0x00; 
		public int? Field11 = null;  

		public int? MarkerSetSNO { get; private set; }

		public bool Hidden = false;

		public bool AdjustPosition = true;

		public int OriginalLevelArea = -1;
		
		public int? MarkerSetIndex { get; private set; }

		private int snoTriggeredConversation = -1;

		/// <summary>
		/// Creates a new actor.
		/// </summary>
		/// <param name="world">The world that initially belongs to.</param>
		/// <param name="snoId">SNOId of the actor.</param>
		/// <param name="tags">TagMapEntry dictionary read for the actor from MPQ's..</param>		   
		protected Actor(World world, int snoId, TagMap tags, bool isMarker = false)
			: base(world, world.IsPvP ? World.NewActorPvPID : world.Game.NewActorGameID)
		{
			this.Tags = tags;

			this.Attributes = new GameAttributeMap(this);

			if (isMarker) return;

			this.AffixList = new List<Affix>();

			//if (tags != null && tags.ContainsKey(MarkerKeys.OnActorSpawnedScript) && tags[MarkerKeys.OnActorSpawnedScript].Id == 178440)
			//	this.AnimationSet = (Mooege.Common.MPQ.FileFormats.AnimSet)Mooege.Common.MPQ.MPQStorage.Data.Assets[SNOGroup.AnimSet][11849].Data; //OminNPC_Male (Wounded)
			//else
			//	if (this.ActorData.AnimSetSNO != -1)
			//		this.AnimationSet = (Mooege.Common.MPQ.FileFormats.AnimSet)Mooege.Common.MPQ.MPQStorage.Data.Assets[SNOGroup.AnimSet][this.ActorData.AnimSetSNO].Data;

			this.ActorSNO = new SNOHandle(SNOGroup.Actor, snoId);
			this.NameSNOId = snoId;
			//Logger.Info("Loaded actor {0}, id {1}, type {2}", this.ActorSNO.Name, this.DynamicID, this.ActorData.Type);
			this.Quality = 0;
			this.HasLoot = true;

			if (ActorData.TagMap.ContainsKey(ActorKeys.TeamID))
			{
				this.Attributes[GameAttribute.TeamID] = ActorData.TagMap[ActorKeys.TeamID];
				//Logger.Debug("Actor {0} has TeamID {1}", this.ActorSNO.Name, this.Attributes[GameAttribute.TeamID]);
			}
			this.Spawned = false;
			this.Size = new Size(1, 1);
			this.GBHandle = new GBHandle { Type = -1, GBID = -1 }; // Seems to be the default. /komiga
			this.CollFlags = this.ActorData.ActorCollisionData.CollFlags.I3;

			this.ReadTags();
			// Listen for quest progress if the actor has a QuestRange attached to it
			//foreach (var quest in World.Game.QuestManager.Quests)
			if (_questRange != null)
				World.Game.QuestManager.OnQuestProgress += new QuestManager.QuestProgressDelegate(quest_OnQuestProgress);
			UpdateQuestRangeVisbility();
		}

		/// <summary>
		/// Creates a new actor.
		/// </summary>
		/// <param name="world">The world that initially belongs to.</param>
		/// <param name="snoId">SNOId of the actor.</param>
		protected Actor(World world, int snoId)
			: this(world, snoId, null)
		{ }

		protected virtual void quest_OnQuestProgress() // erekose changed from protected to public
		{
			//Logger.Debug(" (quest_onQuestProgress) has been called for actor {0} -> lauching UpdateQuestRangeVisibility", NameSNOId);
			try
			{
				UpdateQuestRangeVisbility();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "quest_OnQuestProgress exception: ");
			}
		}

		/// <summary>
		/// Unregister from quest events when object is destroyed 
		/// </summary>
		public override void Destroy()
		{
			if (this.ActorSNO.Id == 454066)
				if (World != null)
					foreach (var plr in World.Game.Players.Values)
						if (plr.SkillSet.HasPassive(208594) && DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0,100) > 45)
							World.SpawnHealthGlobe(this, plr, this.Position);

			if (_questRange != null)
				if (World == null)
					Logger.Debug("World is null? {0}", this.GetType());
				else if (World.Game == null)
					Logger.Debug("Game is null? {0}", this.GetType());
				else if (World.Game.QuestManager != null)
					//foreach (var quest in World.Game.QuestManager)
					World.Game.QuestManager.OnQuestProgress -= quest_OnQuestProgress;

			base.Destroy();
		}

		#region enter-world, change-world, teleport helpers

		public virtual void EnterWorld(Vector3D position)
		{
			var Quest = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Quest][74128];

			if (this.World != null)
				if (this.World.GetActorsBySNO(this.ActorSNO.Id).Count > 0)
				{
					int count = this.World.GetActorsBySNO(this.ActorSNO.Id).Count;
					NumberInWorld = count;
				}
			 if (this.Spawned)
				return;

			this.Position = position;
			this.CheckPointPosition = position;
			this.CurrentDestination = position;

			if (this.World != null) // if actor got into a new world.
			{
				this.World.Enter(this); // let him enter first.
				if ((this is Monster && this.AdjustPosition) || this is Item)
					if (!this.World.CheckLocationForFlag(position, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk)) //if actor has spawned in unwalkable zone
						this.Unstuck();
			}
		}

		public virtual void BeforeChangeWorld()
		{

		}

		public virtual void AfterChangeWorld()
		{

		}


		public void ChangeWorld(World world, Vector3D position)
		{
			if (this.World == world)
				return;

			var prevWorld = this.World;
			//uint prevWorldId = prevWorld.GlobalID;
			BeforeChangeWorld();

			if (this.World != null) // if actor is already in a existing-world
				this.World.Leave(this); // make him leave it first.

			this.World = world;
			this.Position = position;

			if (world.IsPvP)
			{
				//this.GlobalIDOverride = World.NewActorPvPID;
				this.Attributes[GameAttribute.Team_Override] = 10;
			}
			else
			{
				//this.GlobalIDOverride = 0;
				this.Attributes[GameAttribute.Team_Override] = -1;
			}

			if (this.World != null) // if actor got into a new world.
				this.World.Enter(this); // let him enter first.

			this.CheckPointPosition = position;
			if (this is Player)
				world.BroadcastIfRevealed((plr => this.ACDWorldPositionMessage(plr)), this);
			AfterChangeWorld();

			if (this is Player)
			{
				Hireling hireling = (this as Player).ActiveHireling;
				if (hireling != null)
				{
					(hireling as Hireling).Brain.DeActivate();
					hireling.ChangeWorld(world, position);
					(hireling as Hireling).Brain.Activate();
					(this as Player).ActiveHireling = hireling;
				}
				Hireling questhireling = (this as Player).SetQuestHireling;
				if (questhireling != null)
				{
					(questhireling as Hireling).Brain.DeActivate();
					questhireling.ChangeWorld(world, position);
					(questhireling as Hireling).Brain.Activate();
					(this as Player).SetQuestHireling = questhireling;
				}
				foreach (var fol in (this as Player).Followers.Keys.ToList())
				{
					var minion = prevWorld.GetActorByGlobalId(fol);
					if (minion != null)
					{
						(minion as Minion).Brain.DeActivate();
						(this as Player).Followers.Remove(fol);
						minion.ChangeWorld(world, position);
						(this as Player).Followers.Add(minion.GlobalID, minion.ActorSNO.Id);
						(minion as Minion).Brain.Activate();
					}
				}
				
				//(this as Player).InGameClient.SendMessage(new WorldDeletedMessage() { WorldID = prevWorld.GlobalID });
			}
		}

		public void ChangeWorld(World world, StartingPoint startingPoint)
		{
			if (startingPoint != null)
			{
				this.RotationAxis = startingPoint.RotationAxis;
				this.RotationW = startingPoint.RotationW;

				this.ChangeWorld(world, startingPoint.Position);
			}
		}

		public void Teleport(Vector3D position)
		{
			this.Position = position;
			if (this is Player)
			{
				(this as Player).BetweenWorlds = true;
				(this as Player).InGameClient.SendMessage(new ACDTranslateSyncMessage()
				{
					ActorId = this.DynamicID(this as Player),
					Position = this.Position
				});
			}
			else 
			{
				this.World.BroadcastIfRevealed(plr => new ACDTranslateSyncMessage()
				{
					ActorId = this.DynamicID(plr),
					Position = this.Position

				}, this);
			}


			this.OnTeleport();
			this.World.BroadcastIfRevealed(plr => this.ACDWorldPositionMessage(plr), this);
			if (this is Player)
			{
				(this as Player).BetweenWorlds = false;
			}

			if (this is Player)
			{
				var hireling = (this as Player).ActiveHireling;
				if (hireling != null)
				{
					(hireling as Hireling).Brain.DeActivate();
					hireling.Position = position;
					(hireling as Hireling).Brain.Activate();
				}
				var questhireling = (this as Player).SetQuestHireling;
				if (questhireling != null)
				{
					(questhireling as Hireling).Brain.DeActivate();
					questhireling.Position = position;
					(questhireling as Hireling).Brain.Activate();
				}
				foreach (var fol in (this as Player).Followers)
				{
					var minion = this.World.GetActorByGlobalId(fol.Key);
					if (minion != null)
					{
						(minion as Minion).Brain.DeActivate();
						this.World.GetActorByGlobalId(fol.Key).Position = position;
						(minion as Minion).Brain.Activate();
					}
				}

				(this as Player).RevealActorsToPlayer();
				(this as Player).ReRevealPlayersToPlayer();
				this.Attributes[GameAttribute.Looping_Animation_Start_Time] = -1;
				this.Attributes[GameAttribute.Looping_Animation_End_Time] = -1;
				this.Attributes.BroadcastChangedIfRevealed();
				//Refresh Inventory
				(this as Player).Inventory.RefreshInventoryToClient();
			}
		}

		#endregion

		#region Movement/Translation

		public void TranslateFacing(Vector3D target, bool immediately = false)
		{
			float facingAngle = Movement.MovementHelpers.GetFacingAngle(this, target);
			this.SetFacingRotation(facingAngle);

			if (this.World == null) return;
			if (!Spawner)
				this.World.BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
				{
					ActorId = DynamicID(plr),
					Angle = facingAngle,
					TurnImmediately = immediately
				}, this);
		}

		public void Unstuck()
		{
			if (this.World == null) return;
			Vector3D correctPosition = null;
			for (int i = 1; i <= 8; i++)
			{
				int radius = (int)Math.Pow(2, i);
				for (int a = 0; a < 8; a++)
				{
					float angle = (float)((0.125f * a) * (Math.PI * 2));
					correctPosition = this.Position + new Vector3D((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius, 0);
					if (this.World.CheckLocationForFlag(correctPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						this.Position = correctPosition;
						this.World.BroadcastIfRevealed(plr => this.ACDWorldPositionMessage(plr), this);
						return;
					}
				}
			}
		}

		#endregion

		#region Effects

		public void PlayEffectGroup(int effectGroupSNO)
		{
			PlayEffect(Effect.PlayEffectGroup, effectGroupSNO);
		}

		public void PlayEffectGroup(int effectGroupSNO, Actor target)
		{
			if (target == null || this.World == null) return;

			World.BroadcastIfRevealed(plr => new EffectGroupACDToACDMessage
			{
				ActorID = this.DynamicID(plr),
				TargetID = target.DynamicID(plr),
				EffectSNOId = effectGroupSNO
			}, this);
		}

		public void PlayHitEffect(int hitEffect, Actor hitDealer)
		{
			if (hitDealer.World == null || this.World == null) return;

			World.BroadcastIfRevealed(plr => new PlayHitEffectMessage
			{
				ActorID = DynamicID(plr),
				HitDealer = (hitDealer.IsRevealedToPlayer(plr) ? hitDealer.DynamicID(plr) : this.DynamicID(plr)),
				DamageType = hitEffect,
				CriticalDamage = false
			}, this);
		}

		public void PlayEffect(Effect effect, int? param = null, bool broadcast = true)
		{
			if (this.World == null) return;

			if (broadcast)
				this.World.BroadcastIfRevealed(plr => new PlayEffectMessage
				{
					ActorId = this.DynamicID(plr),
					Effect = effect,
					OptionalParameter = param,
					PlayerId = this is Player ? (this as Player).PlayerIndex : null
				}, this);
			else
				if (this is Player)
				(this as Player).InGameClient.SendMessage(new PlayEffectMessage
				{
					ActorId = this.DynamicID(this as Player),
					Effect = effect,
					OptionalParameter = param,
					PlayerId = this is Player ? (this as Player).PlayerIndex : null
				});
		}

		public void AddRopeEffect(int ropeSNO, Actor target)
		{
			if (target == null || target.World == null || this.World == null) return;

			this.World.BroadcastIfRevealed(plr => new RopeEffectMessageACDToACD
			{
				RopeSNO = ropeSNO,
				StartSourceActorId = (int)DynamicID(plr),
				Field2 = 4,
				DestinationActorId = (int)(target.IsRevealedToPlayer(plr) ? target.DynamicID(plr) : this.DynamicID(plr)),
				Field4 = 5,
				Field5 = true
			}, this);
		}

		public void AddRopeEffect(int ropeSNO, Vector3D target)
		{
			if (this.World == null) return;

			this.World.BroadcastIfRevealed(plr => new RopeEffectMessageACDToPlace
			{
				RopeSNO = ropeSNO,
				StartSourceActorId = (int)this.DynamicID(plr),
				Field2 = 4,
				EndPosition = new WorldPlace { Position = target, WorldID = this.World.GlobalID },
				Field4 = true
			}, this);
		}

		public void AddComplexEffect(int effectGroupSNO, Actor target)
		{
			if (target == null || target.World == null || this.World == null) return;

			this.World.BroadcastIfRevealed(plr => new ComplexEffectAddMessage
			{
				EffectId = this.World.LastCEId++, //last ids
				Type = 1,
				EffectSNO = effectGroupSNO,
				SourceActorId = (int)this.DynamicID(plr),
				TargetActorId = (int)(target.IsRevealedToPlayer(plr) ? target.DynamicID(plr) : this.DynamicID(plr)),
				Param1 = 0,
				Param2 = 0,
				IgroneOwnerAlpha = true
			}, target);
		}

		public void SetIdleAnimation(int animationSNO)
		{
			if (this.World == null || animationSNO == -1) return;

			this.World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = animationSNO
			}, this);
		}

		public void PlayAnimationAsSpawn(int animationSNO)
		{
			if (this is Monster)
			{
				var Anim = (DiIiS_NA.Core.MPQ.FileFormats.Anim)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Anim][animationSNO].Data;

				if ((this as Monster).Brain != null)
				{
					//(this as Monster).Brain.DeActivate();
					/*
                    System.Threading.Tasks.Task.Delay(1200).ContinueWith(delegate
					{
						(this as Monster).Brain.Activate();
					});
					//*/
				}

				this.World.BroadcastIfRevealed(plr => new PlayAnimationMessage
				{
					ActorID = this.DynamicID(plr),
					AnimReason = 9,
					UnitAniimStartTime = 0,
					tAnim = new PlayAnimationMessageSpec[]
					{
					new PlayAnimationMessageSpec
					{
						Duration = -2,
						AnimationSNO = animationSNO,
						PermutationIndex = 0x0,
						AnimationTag = 0,
						Speed = 1.0f,
					}
					}
				}, this);
			}
		}

		public void PlayAnimation(int animationType, int animationSNO, float speed = 1.0f, int? ticksToPlay = null, int type2 = 0)
		{
			if (this.World == null || animationSNO == -1) return;

			this.World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimReason = animationType,
				UnitAniimStartTime = type2,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec
					{
						Duration = ticksToPlay.HasValue ? ticksToPlay.Value : -2,  // -2 = play animation once through
						AnimationSNO = animationSNO,
						PermutationIndex = 0x0,  // TODO: implement variations?
						AnimationTag = 0,
						Speed = speed,
					}
				}
			}, this);
		}

		public void PlayActionAnimation(int animationSNO, float speed = 1.0f, int? ticksToPlay = null)
		{
			PlayAnimation(3, animationSNO, speed, ticksToPlay);
		}

		public void NotifyConversation(int status)
		{
			//0 - turn off, 1 - yellow "!", 2 - yellow "?", 3 - "*", 4 - bubble, 5 - silver "!", 6 - none (spams errors)
			this.Attributes[GameAttribute.Conversation_Icon, 0] = status + 1;
			//this.Attributes[GameAttribute.MinimapIconOverride] = (status > 0) ? 120356 : -1;

			Attributes.BroadcastChangedIfRevealed();
		}

		public void AddPercentHP(int percentage, bool GuidingLight = false)
		{
			float quantity = (percentage * this.Attributes[GameAttribute.Hitpoints_Max_Total]) / 100;
			this.AddHP(quantity, GuidingLight);
		}

		public virtual void AddHP(float quantity, bool GuidingLight = false)
		{
			if (quantity > 0)
			{
				if (this.Attributes[GameAttribute.Hitpoints_Cur] < this.Attributes[GameAttribute.Hitpoints_Max_Total])
				{
					this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
						this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
						this.Attributes[GameAttribute.Hitpoints_Max_Total]);

					this.Attributes.BroadcastChangedIfRevealed();
				}
			}
			else
			{
				this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(
					this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
					0);

				this.Attributes.BroadcastChangedIfRevealed();
			}

		}
		#endregion

		#region reveal & unreveal handling

		public void UpdateQuestRangeVisbility()
		{
			if (World != null)
				if (!this.Hidden)
				{
					if (_questRange != null)
						Visible = (this.World.Game.CurrentAct == 3000 && !(this is Monster)) ? true : this.World.Game.QuestManager.IsInQuestRange(_questRange);
					else
						Visible = true;
				}
				else
				{
					Visible = false;
					foreach (var plr in this.GetPlayersInRange(100f))
						Unreveal(plr);
				}
			else
				Visible = false;
		}

		public void SetUsable(bool Activated)
		{
			Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
			Attributes[GameAttribute.Untargetable] = !Activated;
			Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
			Attributes[GameAttribute.Operatable] = Activated;
			Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
			Attributes[GameAttribute.Disabled] = !Activated;
			Attributes[GameAttribute.Immunity] = !Activated;
			Attributes.BroadcastChangedIfRevealed();
		}

		public void SetVisible(bool visibility)
		{
			Visible = visibility;
		}

		/// <summary>
		/// Returns true if the actor is revealed to player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns><see cref="bool"/></returns>
		public bool IsRevealedToPlayer(Player player)
		{
			return player.RevealedObjects.ContainsKey(this.GlobalID);
		}

		public ACDEnterKnownMessage ACDEnterKnown(Player plr)
		{
			return new ACDEnterKnownMessage
			{
				ActorID = this.DynamicID(plr),
				ActorSNOId = this.ActorSNO.Id,
				Flags = this.Field2,
				LocationType = this.HasWorldLocation ? 0 : 1,
				WorldLocation = this.HasWorldLocation ? this.WorldLocationMessage() : null,
				InventoryLocation = this.HasWorldLocation ? null : this.InventoryLocationMessage(plr),
				GBHandle = this.GBHandle,
				snoGroup = this.Field7,
				snoHandle = this.NameSNOId,
				Quality = this.Quality,
				LookLinkIndex = this.Field10,
				snoAmbientOcclusionOverrideTex = null,
				MarkerSetSNO = null,
				MarkerSetIndex = null,
				EnterKnownLookOverrides = null
			};
		}

		/// <summary>
		/// Reveals an actor to a player.
		/// </summary>
		/// <returns>true if the actor was revealed or false if the actor was already revealed.</returns>
		public override bool Reveal(Player player)
		{
			lock (player.RevealedObjects)
			{
				if (this.Hidden || this.Dead || !this.Visible || this.World == null) return false;
				
				//Leave Miriam in Crypt
				if (this.ActorSNO.Id == 175310)
					if (this.World.WorldSNO.Id == 72636 ||
						this.World.WorldSNO.Id == 72637 ||
						this.World.WorldSNO.Id == 102299 ||
						this.World.WorldSNO.Id == 165797 ||
						this.World.WorldSNO.Id == 154587 ||
						this.World.WorldSNO.Id == 60600 ||
						this.World.WorldSNO.Id == 92126
						)
						return false;


				//Destroy Bonewall and Jondar if Exit_S on Second Level of Cathedral
				if (World.WorldSNO.Id == 50582 && this.ActorSNO.Id == 109209) return false;
				if (World.WorldSNO.Id == 50582 && this.ActorSNO.Id == 86624) return false;

				if (this.ActorSNO.Name.Contains("Uber") && !this.World.WorldSNO.Name.Contains("Uber")) return false;
				if (this.ActorSNO.Name.Contains("AdventureMode") && this.World.Game.CurrentAct != 3000) return false;
				if (this.ActorSNO.Name.Contains("ScriptedSequenceOnly")) return false;

				if (player.RevealedObjects.ContainsKey(this.GlobalID)) return false; // already revealed

				if (player.World == null) return false;

				if (this.ActorSNO.Id == 218339)
					if (this.World.WorldSNO.Id == 71150)
						if (this.CurrentScene.SceneSNO.Id == 33348)
							if (this.Position.X < 2896)
								return false;


				if (!(this is Item) && player.World.GlobalID != this.World.GlobalID) return false;

				if (!(this is Item) && this.GetScenesInRange().Count() > 0 && !this.GetScenesInRange().OrderBy(scene => PowerMath.Distance2D(scene.Position, this.Position)).First().IsRevealedToPlayer(player)) return false;

				uint objId = player.NewDynamicID(this.GlobalID, (this is Player && (!(this as Player).IsInPvPWorld || this == player)) ? (int)(this as Player).PlayerIndex : -1);

				player.RevealedObjects.Add(this.GlobalID, objId);

				var gbidbank = new int[this.AffixList.Count];
				int i = 0;
				foreach (var affix in AffixList)
				{
					gbidbank[i] = affix.AffixGbid;
					i++;
				}
				/*
				player.InGameClient.SendMessage(new PreloadACDDataMessage(Opcodes.PreloadAddACDMessage)
				{
					ActorID = this.DynamicID(player),
					SNOActor = this.ActorSNO.Id,
					eWeaponClass = 0,
					gbidMonsterAffixes = new int[0] //gbidbank
				});
				//*/
				var msg = ACDEnterKnown(player);

				// normaly when we send acdenterknown for players own actor it's set to 0x09. But while sending the acdenterknown for another player's actor we should set it to 0x01. /raist
				if ((this is Player) && this != player)
					msg.Flags = 0x01;

				player.InGameClient.SendMessage(msg);

				// Collision Flags
				if (!((this is Projectile) || (this is Item)))
				{
					player.InGameClient.SendMessage(new ACDCollFlagsMessage
					{
						ActorID = objId,
						CollFlags = this.CollFlags
					});
				}

				// Send Attributes
				Attributes.SendMessage(player.InGameClient);

				if (this is Monster)
				{
					this.Attributes[GameAttribute.Hitpoints_Cur] += 0.001f;
					this.Attributes.BroadcastChangedIfRevealed();
				}

				// This is always sent even though it doesn't identify the actor. /komiga
				player.InGameClient.SendMessage(new PrefetchMessage
				{
					Name = this.ActorSNO
				});

				// Reveal actor (creates actor and makes it visible to the player)
				if (this is Player || this is NPC || this is Goblin)
					player.InGameClient.SendMessage(new ACDCreateActorMessage(objId));

				TrickleMessage Trickle = new TrickleMessage()
				{
					ActorId = this.DynamicID(player),
					ActorSNO = this.ActorSNO.Id,
					WorldLocation = new WorldPlace()
					{
						WorldID = this.World.GlobalID,
						Position = this.Position
					},
					HealthPercent = 1f,
					
				};

				if (this is Player)
					Trickle.PlayerIndex = (this as Player).PlayerIndex;

				player.InGameClient.SendMessage(Trickle);


				// Actor group
				player.InGameClient.SendMessage(new ACDGroupMessage
				{
					ActorID = objId,
					Group1Hash = 0,
					Group2Hash = 0,
				});



				#region Особенные случаи
				//Задаём идл для зомбей в тристраме - ЖРАТ
				if (this.World.WorldSNO.Id == 71150)
				{
					if (this.Tags != null)
						if (this.Tags.ContainsKey(MarkerKeys.Group1Hash))
							if (this.Tags[MarkerKeys.Group1Hash] == -1248096796)
								this.PlayActionAnimation(11514);
				}
				//Задаём идл для работяг
				else if (this.World.WorldSNO.Id == 109362 & this.ActorSNO.Id == 84529)
					this.PlayActionAnimation(102329);
				else if (this.ActorSNO.Id == 4580)
					player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Inventory.VisualInventoryMessage()
					{
						ActorID = this.DynamicID(player),
						EquipmentList = new MessageSystem.Message.Fields.VisualEquipment()
						{
							Equipment = new MessageSystem.Message.Fields.VisualItem[]
							{
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = unchecked((int)-2091504072),
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,//0x6C3B0389,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
								new MessageSystem.Message.Fields.VisualItem()
								{
									GbId = -1,
									DyeType = 0,
									ItemEffectType = 0,
									EffectLevel = -1,
								},
							}
						}
					});

				#endregion
				if (this is NPC || this is InteractiveNPC)
				{
					//.Contains<TagMap>(AnimationSetKeys.Idle)
					//if (this.AnimationSet.Animations.ContainsKey(AnimationSetKeys.Idle.ID))
					//	this.SetIdleAnimation(this.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Idle]);
					//this.PlayAnimation(0, this.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Idle]);
				}

				//Logger.Trace("Reveal actor [{2}]{0} as {1}", this.GlobalID, objId, this.ActorSNO.Name);



				return true;
			}
		}

		/// <summary>
		/// Unreveals an actor from a player.
		/// </summary>
		/// <returns>true if the actor was unrevealed or false if the actor wasn't already revealed.</returns>
		public override bool Unreveal(Player player)
		{
			lock (player.RevealedObjects)
			{
				if (!player.RevealedObjects.ContainsKey(this.GlobalID)) return false; // not revealed yet
				if (!(this is Item) && player.World.GlobalID != this.World.GlobalID) return false;

				//PreloadRemoveACDMessage
				var gbidbank = new int[this.AffixList.Count];
				int i = 0;
				foreach(var affix in AffixList)
                {
					gbidbank[i] = affix.AffixGbid;
					i++;
                }
				if (this is Player)
					player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.InventoryCreateMessage)
					{
						ActorID = this.DynamicID(player),
					});
				if (this is Minion)
				{
					uint DynID = 0;
					player.RevealedObjects.TryGetValue(this.GlobalID, out DynID);
					if (DynID != 0)
					{
						player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Pet.PetDetachMessage()
						{
							PetId = DynID,
						});
					}
				}
				/*
				if (this is Monster)
					player.InGameClient.SendMessage(new RemoveRagdollMessage()
					{
						Field0 = this.DynamicID(player),
						Field1 = (this as Monster).Monster.Id,
					});
				//*/
				player.InGameClient.SendMessage(new ACDDestroyActorMessage(this.DynamicID(player)));

				//Logger.Trace("Unreveal actor {0} as {1}", this.GlobalID, this.DynamicID(player));
				player.RevealedObjects.Remove(this.GlobalID);

				//if (!(this is Item) && this.Dead && this.World.Players.Values.Where(p => this.IsRevealedToPlayer(p)).Count() == 0)
				//this.Destroy();
				return true;
			}
		}

		#endregion

		#region proximity-based query helpers

		#region circurlar region queries

		public List<Player> GetPlayersInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<Player>(radius);
		}

		public List<Item> GetItemsInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<Item>(radius);
		}

		public List<Monster> GetMonstersInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<Monster>(radius);
		}

		public List<Actor> GetActorsInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			if (this.World == null || this.Position == null) return new List<Actor>();

			return this.GetObjectsInRange<Actor>(radius);
		}

		public List<Actor> GetActorsInRange(Vector3D TPosition, float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<Actor>(TPosition, radius);
		}

		public List<T> GetObjectsInRange<T>(Vector3D TPosition, float radius) where T : WorldObject
		{
			var proximityCircle = new Circle(TPosition.X, TPosition.Y, radius);
			return this.World.QuadTree.Query<T>(proximityCircle);
		}

		public List<T> GetActorsInRange<T>(float radius = -1) where T : Actor
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<T>(radius);
		}

		public List<Scene> GetScenesInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<Scene>(radius);
		}

		public List<WorldObject> GetObjectsInRange(float radius = -1)
		{
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			return this.GetObjectsInRange<WorldObject>(radius);
		}

		public List<T> GetObjectsInRange<T>(float radius = -1, bool includeHierarchy = false) where T : WorldObject
		{
			if (this.World == null || this.Position == null) return new List<T>();
			if (radius == -1) radius = this.DefaultQueryProximityRadius;
			var proximityCircle = new Circle(this.Position.X, this.Position.Y, radius);
			return this.World.QuadTree.Query<T>(proximityCircle, includeHierarchy);
		}

		#endregion

		#region rectangluar region queries

		public List<Player> GetPlayersInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<Player>(lenght);
		}

		public List<Item> GetItemsInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<Item>(lenght);
		}

		public List<Monster> GetMonstersInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<Monster>(lenght);
		}

		public List<Actor> GetActorsInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<Actor>(lenght);
		}

		public List<T> GetActorsInRegion<T>(int lenght = DefaultQueryProximityLenght) where T : Actor
		{
			return this.GetObjectsInRegion<T>(lenght);
		}

		public List<Scene> GetScenesInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<Scene>(lenght);
		}

		public List<WorldObject> GetObjectsInRegion(int lenght = DefaultQueryProximityLenght)
		{
			return this.GetObjectsInRegion<WorldObject>(lenght);
		}

		public List<T> GetObjectsInRegion<T>(int lenght = DefaultQueryProximityLenght) where T : WorldObject
		{
			var proximityRectangle = new RectangleF(this.Position.X - lenght / 2, this.Position.Y - lenght / 2, lenght, lenght);
			return this.World.QuadTree.Query<T>(proximityRectangle);
		}

		#endregion

		#endregion

		#region events


		public virtual void OnEnter(World world)
		{

		}

		public virtual void OnLeave(World world)
		{

		}

		public void OnActorMove(Actor actor, Vector3D prevPosition)
		{
			// TODO: Unreveal from players that are now outside the actor's range. /komiga
		}

		public virtual void OnTargeted(Player player, TargetMessage message)
		{

		}

		public virtual void OnTeleport()
		{

		}

		/// <summary>
		/// Called when a player moves close to the actor
		/// </summary>
		public virtual void OnPlayerApproaching(Player player)
		{
		}

		#endregion

		#region cooked messages

		public virtual InventoryLocationMessageData InventoryLocationMessage(Player plr)
		{
			// Only used in Item; stubbed here to prevent an overrun in some cases. /komiga
			return new InventoryLocationMessageData { OwnerID = 0, EquipmentSlot = 0, InventoryLocation = new Vector2D() };
		}

		public virtual ACDWorldPositionMessage ACDWorldPositionMessage(Player plr)
		{
			return new ACDWorldPositionMessage { ActorID = this.DynamicID(plr), WorldLocation = this.WorldLocationMessage() };
		}

		public virtual ACDInventoryPositionMessage ACDInventoryPositionMessage(Player plr)
		{
			return new ACDInventoryPositionMessage()
			{
				ItemId = this.DynamicID(plr),
				InventoryLocation = this.InventoryLocationMessage(plr),
				LocType = 1 // TODO: find out what this is and why it must be 1...is it an enum?
			};
		}

		public virtual WorldLocationMessageData WorldLocationMessage()
		{
			return new WorldLocationMessageData { Scale = this.Scale, Transform = this.Transform, WorldID = this.World.GlobalID };
		}

		#endregion

		#region tag-readers

		/// <summary>
		/// Reads known tags from TagMapEntry and set the proper values.
		/// </summary>
		protected virtual void ReadTags()
		{
			if (this.Tags == null) return;

			// load scale from actor data and override it with marker tags if one is set
			this.Scale = ActorData.TagMap.ContainsKey(ActorKeys.Scale) ? ActorData.TagMap[ActorKeys.Scale] : 1;
			this.Scale = Tags.ContainsKey(MarkerKeys.Scale) ? Tags[MarkerKeys.Scale] : this.Scale;


			if (Tags.ContainsKey(MarkerKeys.QuestRange))
			{
				int snoQuestRange = Tags[MarkerKeys.QuestRange].Id;
				if (DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.QuestRange].ContainsKey(snoQuestRange))
					_questRange = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.QuestRange][snoQuestRange].Data as DiIiS_NA.Core.MPQ.FileFormats.QuestRange;
				else Logger.Debug("Actor {0}  GlobalID {1} is tagged with unknown QuestRange {2}", NameSNOId, GlobalID, snoQuestRange);
			}

			if (Tags.ContainsKey(MarkerKeys.ConversationList) && WorldGenerator.DefaultConversationLists.ContainsKey(this.ActorSNO.Id))
			{
				int snoConversationList = WorldGenerator.DefaultConversationLists[this.ActorSNO.Id];//Tags[MarkerKeys.ConversationList].Id;

				Logger.Trace(" (ReadTags) actor {0} GlobalID {2} has a conversation list {1}", NameSNOId, snoConversationList, GlobalID);

				if (DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.ConversationList].ContainsKey(snoConversationList))
					ConversationList = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.ConversationList][snoConversationList].Data as DiIiS_NA.Core.MPQ.FileFormats.ConversationList;
				else
					if (snoConversationList != -1)
					Logger.Warn("Actor {0} - Conversation list {1} not found!", NameSNOId, snoConversationList);
			}


			if (this.Tags.ContainsKey(MarkerKeys.TriggeredConversation))
				snoTriggeredConversation = Tags[MarkerKeys.TriggeredConversation].Id;
		}

		#endregion

		#region movement

		public void Move(Vector3D point, float facingAngle)
		{
			this.CurrentDestination = point;
			if (point == this.Position) return;
			this.SetFacingRotation(facingAngle);

			// find suitable movement animation
			int aniTag;
			if (this.AnimationSet == null)
				aniTag = -1;
			else if (this.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk) && !(this is Minion) && !(this is Hireling))
				aniTag = this.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk);
			else if (this.AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run))
				aniTag = this.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run);
			else
				aniTag = -1;
			if(World != null)
			this.World.BroadcastIfRevealed(plr => new ACDTranslateNormalMessage
			{
				ActorId = this.DynamicID(plr),
				Position = point,
				Angle = facingAngle,
				SnapFacing = false,
				MovementSpeed = this.WalkSpeed,
				MoveFlags = 0,
				AnimationTag = aniTag
			}, this);
		}

		public void MoveSnapped(Vector3D point, float facingAngle)
		{
			this.Position = point;
			this.SetFacingRotation(facingAngle);

			this.World.BroadcastIfRevealed(plr => new ACDTranslateSnappedMessage
			{
				ActorId = (int)this.DynamicID(plr),
				Position = point,
				Angle = facingAngle,
				Field3 = false,
				Field4 = 0x900  // TODO: figure out when to use this field
			}, this);
		}

		#endregion

		public override string ToString()
		{
			return string.Format("[Actor] [Type: {0}] SNOId:{1} GlobalId: {2} Position: {3} Name: {4}", this.ActorType, this.ActorSNO.Id, this.GlobalID, this.Position, this.ActorSNO.Name);
		}
	}

	// This should probably be the same as GBHandleType (probably merge them once all actor classes are created)
	public enum ActorType : int
	{
		Invalid = 0,
		Monster = 1,
		Gizmo = 2,
		ClientEffect = 3,
		ServerProp = 4,
		Environment = 5,
		Critter = 6,
		Player = 7,
		Item = 8,
		AxeSymbol = 9,
		Projectile = 10,
		CustomBrain = 11
	}
}
