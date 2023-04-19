using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.GSSystem.GameSystem;
using DiIiS_NA.LoginServer.Battle;
using Circle = DiIiS_NA.GameServer.Core.Types.Misc.Circle;
using Player = DiIiS_NA.GameServer.GSSystem.PlayerSystem.Player;
using Scene = DiIiS_NA.GameServer.GSSystem.MapSystem.Scene;
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public abstract class Actor : WorldObject
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// ActorSNO.
		/// </summary>
		public SNOHandle ActorSNO { get; private set; }
		
		public ActorSno SNO => (ActorSno)ActorSNO.Id;

		public string Name => ActorSNO.Name;

		/// <summary>
		/// Gets or sets the sno of the actor used to identify the actor to the player
		/// This is usually the same as actorSNO except for actors that have a GBHandle
		/// There are few exceptions though like the Inn_Zombies that have both.
		/// Used by ACDEnterKnown to name the actor.
		/// </summary>
		public ActorSno NameSNO { get; set; }

		public bool Disable = false;

		public bool Spawner = false;

		/// <summary>
		/// The actor type.
		/// </summary>
		public abstract ActorType ActorType { get; }

		public object _payloadLock = new();

		/// <summary>
		/// Current scene for the actor.
		/// </summary>
		public virtual Scene CurrentScene
		{
			get { return World.QuadTree.Query<Scene>(Bounds).FirstOrDefault(); }
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
			get { return new PRTransform { Quaternion = new Quaternion { W = RotationW, Vector3D = RotationAxis }, Vector3D = Position }; }
		}

		/// <summary>
		/// Replaces the actor's rotation with one that rotates along the Z-axis by the specified "facing" angle. 
		/// </summary>
		/// <param name="facingAngle">The angle in radians.</param>
		public void SetFacingRotation(float facingAngle)
		{
			if (!Spawner)
			{
				Quaternion q = Quaternion.FacingRotation(facingAngle);
				RotationW = q.W;
				RotationAxis = q.Vector3D;
			}
		}

		/// <summary>
		/// Tags read from MPQ's for the actor.
		/// </summary>
		public TagMap Tags { get; private set; }

		/// <summary>
		/// Attribute map.
		/// </summary>
		public GameAttributeMap Attributes { get; }

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
		public bool Alive => !Dead;

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
		public DiIiS_NA.Core.MPQ.FileFormats.ActorData ActorData 
			=> (DiIiS_NA.Core.MPQ.FileFormats.ActorData)MPQStorage.Data.Assets[SNOGroup.Actor][(int)SNO].Data;

		/// <summary>
		/// The animation set for actor.
		/// </summary>
		public DiIiS_NA.Core.MPQ.FileFormats.AnimSet AnimationSet
		{
			get
			{
				if (ActorData.AnimSetSNO != -1)
					return (DiIiS_NA.Core.MPQ.FileFormats.AnimSet)MPQStorage.Data.Assets[SNOGroup.AnimSet][ActorData.AnimSetSNO].Data;
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
		// TODO: check if the following is correct: @iamdroppy
		// {
		// 	get => Attributes[GameAttribute.Hidden];
		// 	set => Attributes[GameAttribute.Hidden] = value;
		// }

		public bool AdjustPosition = true;

		public int OriginalLevelArea = -1;
		
		public int? MarkerSetIndex { get; private set; }

		private int _snoTriggeredConversation = -1;

		/// <summary>
		/// Creates a new actor.
		/// </summary>
		/// <param name="world">The world that initially belongs to.</param>
		/// <param name="sno">SNOId of the actor.</param>
		/// <param name="tags">TagMapEntry dictionary read for the actor from MPQ's..</param>
		/// <param name="isMarker">Is Marker</param>		   
		protected Actor(World world, ActorSno sno, TagMap tags, bool isMarker = false)
			: base(world, world.IsPvP ? World.NewActorPvPID : world.Game.NewActorGameId)
		{
			Tags = tags;

			Attributes = new GameAttributeMap(this);

			if (isMarker) return;

			AffixList = new List<Affix>();

			// if (tags != null && tags.ContainsKey(MarkerKeys.OnActorSpawnedScript) && tags[MarkerKeys.OnActorSpawnedScript].Id == 178440)
				// AnimationSet = (AnimSet)MPQStorage.Data.Assets[SNOGroup.AnimSet][11849].Data; //OminNPC_Male (Wounded)
			//else
			//	if (this.ActorData.AnimSetSNO != -1)
			//		this.AnimationSet = (Mooege.Common.MPQ.FileFormats.AnimSet)Mooege.Common.MPQ.MPQStorage.Data.Assets[SNOGroup.AnimSet][this.ActorData.AnimSetSNO].Data;

			ActorSNO = new SNOHandle(SNOGroup.Actor, (int)sno);
			NameSNO = sno;
			//Logger.Info("Loaded actor {0}, id {1}, type {2}", this.ActorSNO.Name, this.DynamicID, this.ActorData.Type);
			//Quality = 0; - removed, 0 is default and you can't change the quality
			HasLoot = true;

			if (ActorData.TagMap.ContainsKey(ActorKeys.TeamID))
			{
				Attributes[GameAttributes.TeamID] = ActorData.TagMap[ActorKeys.TeamID];
				//Logger.Debug("Actor {0} has TeamID {1}", this.ActorSNO.Name, this.Attributes[GameAttribute.TeamID]);
			}
			Spawned = false;
			Size = new Size(1, 1);
			GBHandle = new GBHandle { Type = -1, GBID = -1 }; // Seems to be the default. /komiga
			CollFlags = ActorData.ActorCollisionData.CollFlags.I3;

			ReadTags();
			// Listen for quest progress if the actor has a QuestRange attached to it
			//foreach (var quest in World.Game.QuestManager.Quests)
			if (_questRange != null)
				World.Game.QuestManager.OnQuestProgress += QuestProgress;
			UpdateQuestRangeVisibility();
		}

		/// <summary>
		/// Creates a new actor.
		/// </summary>
		/// <param name="world">The world that initially belongs to.</param>
		/// <param name="sno">SNOId of the actor.</param>
		protected Actor(World world, ActorSno sno)
			: this(world, sno, null)
		{ }

		protected virtual void QuestProgress() // erekose changed from protected to public
		{
			//Logger.Debug(" (quest_onQuestProgress) has been called for actor {0} -> lauching UpdateQuestRangeVisibility", NameSNOId);
			try
			{
				UpdateQuestRangeVisibility();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "quest_OnQuestProgress exception: ");
			}
		}
		private bool _isDestroyed = false;
		
		/// <summary>
		/// Unregister from quest events when object is destroyed 
		/// </summary>
		public override void Destroy()
		{
			if (_isDestroyed) return;
			if (SNO == ActorSno._p6_necro_corpse_flesh)
				if (World != null)
					foreach (var plr in World.Game.Players.Values)
						if (plr.SkillSet.HasPassive(208594) && DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0,100) > 45)
							World.SpawnHealthGlobe(this, plr, Position);

			if (_questRange != null)
				if (World == null)
					Logger.Debug("World is null? {0}", GetType());
				else if (World.Game == null)
					Logger.Debug("Game is null? {0}", GetType());
				else if (World.Game.QuestManager != null)
					//foreach (var quest in World.Game.QuestManager)
					World.Game.QuestManager.OnQuestProgress -= QuestProgress;

			base.Destroy();
		}

		#region enter-world, change-world, teleport helpers

		public virtual void EnterWorld(Vector3D position)
		{
			// var quest = MPQStorage.Data.Assets[SNOGroup.Quest][74128];

			if (World != null)
			{
				int count = World.GetActorsBySNO(SNO).Count;
				if (count > 0)
					NumberInWorld = count;
            }

            if (Spawned)
				return;

			Position = position;
			CheckPointPosition = position;
			CurrentDestination = position;

			if (World != null) // if actor got into a new world.
			{
				World.Enter(this); // let him enter first.
				if ((this is Monster && AdjustPosition) || this is Item)
					if (!World.CheckLocationForFlag(position, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk)) //if actor has spawned in unwalkable zone
						Unstuck();
			}
		}

		public virtual void BeforeChangeWorld() {}

		public virtual void AfterChangeWorld() {}

		public void ChangeWorld(World world, Vector3D position)
		{
			if (World == world)
				return;

			var prevWorld = World;
			//uint prevWorldId = prevWorld.GlobalID;
			BeforeChangeWorld();

			World?.Leave(this); // make him leave it first.

			World = world;
			Position = position;

			if (world.IsPvP)
			{
				//this.GlobalIDOverride = World.NewActorPvPID;
				Attributes[GameAttributes.Team_Override] = 10;
			}
			else
			{
				//this.GlobalIDOverride = 0;
				Attributes[GameAttributes.Team_Override] = -1;
			}

			World?.Enter(this); // let him enter first.

			CheckPointPosition = position;
			if (this is Player)
				world.BroadcastIfRevealed(ACDWorldPositionMessage, this);
			AfterChangeWorld();

			if (this is Player plr)
			{
				Hireling hireling = plr.ActiveHireling;
				if (hireling != null)
				{
					hireling.Brain.DeActivate();
					hireling.ChangeWorld(world, position);
					hireling.Brain.Activate();
					plr.ActiveHireling = hireling;
				}
				Hireling questHireling = plr.QuestHireling;
				if (questHireling != null)
				{
					questHireling.Brain.DeActivate();
					questHireling.ChangeWorld(world, position);
					questHireling.Brain.Activate();
					plr.QuestHireling = questHireling;
				}
				foreach (var fol in plr.Followers.Keys.ToList())
				{
					var minion = prevWorld.GetActorByGlobalId(fol);
					if (minion is Minion m)
					{
						m.Brain.DeActivate();
						plr.Followers.Remove(fol);
						minion.ChangeWorld(world, position);
						plr.Followers.Add(minion.GlobalID, minion.SNO);
						m.Brain.Activate();
					}
				}
				
				//(this as Player).InGameClient.SendMessage(new WorldDeletedMessage() { WorldID = prevWorld.GlobalID });
			}
		}

		public void ChangeWorld(World world, StartingPoint startingPoint)
		{
			if (startingPoint != null)
			{
				RotationAxis = startingPoint.RotationAxis;
				RotationW = startingPoint.RotationW;

				ChangeWorld(world, startingPoint.Position);
			}
		}

		public void Teleport(Vector3D position)
		{
			Position = position;
			if (this is Player player)
			{
				player.BetweenWorlds = true;
				player.InGameClient.SendMessage(new ACDTranslateSyncMessage()
				{
					ActorId = DynamicID(this as Player),
					Position = Position
				});
			}
			else 
			{
				World.BroadcastIfRevealed(plr => new ACDTranslateSyncMessage()
				{
					ActorId = DynamicID(plr),
					Position = Position

				}, this);
			}


			OnTeleport();
			World.BroadcastIfRevealed(ACDWorldPositionMessage, this);
			if (this is Player plr)
			{
				var hireling = plr.ActiveHireling;
				if (hireling != null)
				{
					(hireling as Hireling).Brain.DeActivate();
					hireling.Position = position;
					(hireling as Hireling).Brain.Activate();
				}
				var questHireling = plr.QuestHireling;
				if (questHireling != null)
				{
					questHireling.Brain.DeActivate();
					questHireling.Position = position;
					questHireling.Brain.Activate();
				}
				foreach (var fol in plr.Followers)
				{
					if (World.GetActorByGlobalId(fol.Key) is Minion minion)
					{
						minion.Brain.DeActivate();
						World.GetActorByGlobalId(fol.Key).Position = position;
						minion.Brain.Activate();
					}
				}

				plr.RevealActorsToPlayer();
				plr.ReRevealPlayersToPlayer();
				Attributes[GameAttributes.Looping_Animation_Start_Time] = -1;
				Attributes[GameAttributes.Looping_Animation_End_Time] = -1;
				Attributes.BroadcastChangedIfRevealed();
				//Refresh Inventory
				plr.Inventory.RefreshInventoryToClient();
			}
		}

		#endregion

		#region Movement/Translation

		public void TranslateFacing(Vector3D target, bool immediately = false)
		{
			float facingAngle = Movement.MovementHelpers.GetFacingAngle(this, target);
			SetFacingRotation(facingAngle);

			if (World == null) return;
			if (!Spawner)
				World.BroadcastIfRevealed(plr => new ACDTranslateFacingMessage
				{
					ActorId = DynamicID(plr),
					Angle = facingAngle,
					TurnImmediately = immediately
				}, this);
		}

		public void Unstuck()
		{
			if (World == null) return;
			for (int i = 1; i <= 8; i++)
			{
				int radius = (int)Math.Pow(2, i);
				for (int a = 0; a < 8; a++)
				{
					float angle = (float)(0.125f * a * (Math.PI * 2));
					Vector3D correctPosition = Position + new Vector3D((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius, 0);
					if (World.CheckLocationForFlag(correctPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						Position = correctPosition;
						World.BroadcastIfRevealed(ACDWorldPositionMessage, this);
						return;
					}
				}
			}
		}

		#endregion

		#region Effects

		public void PlayEffectGroup(int effectGroupSNO)
		{
			#if DEBUG
			if (Dicts.DictSNOEffectGroup.ContainsValue(effectGroupSNO))
			{
				var effectGroupKey = Dicts.DictSNOEffectGroup.FirstOrDefault(x => x.Value == effectGroupSNO).Key;
				Logger.MethodTrace($"{effectGroupSNO} on {GetType().Name}. Type: $[green]${effectGroupKey}$[/]$");
			}
			else
			{
				Logger.MethodTrace($"{effectGroupSNO} on {GetType().Name}. Type: $[red]$Unknown$[/]$");
			}
			#endif
			PlayEffect(Effect.PlayEffectGroup, effectGroupSNO);
		}

		public void PlayEffectGroup(int effectGroupSNO, Actor target)
		{
			if (target == null || World == null) return;

			World.BroadcastIfRevealed(plr => new EffectGroupACDToACDMessage
			{
				ActorID = DynamicID(plr),
				TargetID = target.DynamicID(plr),
				EffectSNOId = effectGroupSNO
			}, this);
		}

		public void PlayHitEffect(int hitEffect, Actor hitDealer)
		{
			if (hitDealer.World == null || World == null) return;

			World.BroadcastIfRevealed(plr => new PlayHitEffectMessage
			{
				ActorID = DynamicID(plr),
				HitDealer = hitDealer.IsRevealedToPlayer(plr) ? hitDealer.DynamicID(plr) : DynamicID(plr),
				DamageType = hitEffect,
				CriticalDamage = false
			}, this);
		}

		public void PlayEffect(Effect effect, int? param = null, bool broadcast = true)
		{
			if (World == null) return;

			if (broadcast)
				World.BroadcastIfRevealed(plr => new PlayEffectMessage
				{
					ActorId = DynamicID(plr),
					Effect = effect,
					OptionalParameter = param,
					PlayerId = (this as Player)?.PlayerIndex
				}, this);
			else
			{
				(this as Player)?.InGameClient.SendMessage(new PlayEffectMessage
				{
					ActorId = DynamicID(this as Player),
					Effect = effect,
					OptionalParameter = param,
					PlayerId = (this as Player)?.PlayerIndex
				});
			}
		}

		public void AddRopeEffect(int ropeSNO, Actor target)
		{
			if (target?.World == null || World == null) return;

			World.BroadcastIfRevealed(plr => new RopeEffectMessageACDToACD
			{
				RopeSNO = ropeSNO,
				StartSourceActorId = (int)DynamicID(plr),
				Field2 = 4,
				DestinationActorId = (int)(target.IsRevealedToPlayer(plr) ? target.DynamicID(plr) : DynamicID(plr)),
				Field4 = 5,
				Field5 = true
			}, this);
		}

		public void AddRopeEffect(int ropeSNO, Vector3D target)
		{
			World?.BroadcastIfRevealed(plr => new RopeEffectMessageACDToPlace
			{
				RopeSNO = ropeSNO,
				StartSourceActorId = (int)DynamicID(plr),
				Field2 = 4,
				EndPosition = new WorldPlace { Position = target, WorldID = World.GlobalID },
				Field4 = true
			}, this);
		}

		public void AddComplexEffect(int effectGroupSNO, Actor target)
		{
			if (target == null || target.World == null || World == null) return;

			World.BroadcastIfRevealed(plr => new ComplexEffectAddMessage
			{
				EffectId = World.LastCEId++, //last ids
				Type = 1,
				EffectSNO = effectGroupSNO,
				SourceActorId = (int)DynamicID(plr),
				TargetActorId = (int)(target.IsRevealedToPlayer(plr) ? target.DynamicID(plr) : DynamicID(plr)),
				Param1 = 0,
				Param2 = 0,
				IgroneOwnerAlpha = true
			}, target);
		}

		public void SetIdleAnimation(AnimationSno animationSNO)
		{
			if (this.World == null || animationSNO == AnimationSno._NONE) return;

			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = (int)animationSNO
			}, this);
		}

		public void PlayAnimationAsSpawn(AnimationSno animationSNO)
		{
			if (this is Monster)
			{
				// unused
				//var Anim = (DiIiS_NA.Core.MPQ.FileFormats.Anim)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.Anim][animationSNO].Data;

				World.BroadcastIfRevealed(plr => new PlayAnimationMessage
					{
						ActorID = DynamicID(plr),
						AnimReason = 9,
						UnitAniimStartTime = 0,
						tAnim = new PlayAnimationMessageSpec[]
						{
							new PlayAnimationMessageSpec
							{
								Duration = -2,
								AnimationSNO = (int)animationSNO,
								PermutationIndex = 0x0,
								AnimationTag = 0,
								Speed = 1.0f,
							}
						}
					},
					this);
			}
		}

		public void PlayAnimation(int animationType, AnimationSno animationSNO, float speed = 1.0f, int? ticksToPlay = null, int type2 = 0)
		{
			if (animationSNO == AnimationSno._NONE)
			{
				Logger.Warn($"PlayAnimation: {(int)animationSNO} is not a valid animation");
				return;
			}
			if (this.World == null) return;

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimReason = animationType,
				UnitAniimStartTime = type2,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new()
					{
						Duration = ticksToPlay ?? -2,  // -2 = play animation once through
						AnimationSNO = (int)animationSNO,
						PermutationIndex = 0x0,  // TODO: implement variations?
						AnimationTag = 0,
						Speed = speed,
					}
				}
			}, this);
		}

		public void PlayActionAnimation(AnimationSno animationSNO, float speed = 1.0f, int? ticksToPlay = null)
		{
			PlayAnimation(3, animationSNO, speed, ticksToPlay);
		}

		public void NotifyConversation(int status)
		{
			//0 - turn off, 1 - yellow "!", 2 - yellow "?", 3 - "*", 4 - bubble, 5 - silver "!", 6 - none (spams errors)
			Attributes[GameAttributes.Conversation_Icon, 0] = status + 1;
			//this.Attributes[GameAttribute.MinimapIconOverride] = (status > 0) ? 120356 : -1;

			Attributes.BroadcastChangedIfRevealed();
		}

		public void AddPercentHP(int percentage, bool GuidingLight = false)
		{
			float quantity = percentage * Attributes[GameAttributes.Hitpoints_Max_Total] / 100;
			AddHP(quantity, GuidingLight);
		}

		public virtual void AddHP(float quantity, bool guidingLight = false)
		{
			if (quantity > 0)
			{
				if (Attributes[GameAttributes.Hitpoints_Cur] < Attributes[GameAttributes.Hitpoints_Max_Total])
				{
					Attributes[GameAttributes.Hitpoints_Cur] = Math.Min(
						Attributes[GameAttributes.Hitpoints_Cur] + quantity,
						Attributes[GameAttributes.Hitpoints_Max_Total]);

					Attributes.BroadcastChangedIfRevealed();
				}
			}
			else
			{
				Attributes[GameAttributes.Hitpoints_Cur] = Math.Max(
					Attributes[GameAttributes.Hitpoints_Cur] + quantity,
					0);

				Attributes.BroadcastChangedIfRevealed();
			}

		}
		#endregion

		#region reveal & unreveal handling

		public void UpdateQuestRangeVisibility()
		{
			if (World != null)
				if (!Hidden)
				{
					if (_questRange != null)
						Visible = (World.Game.CurrentAct == ActEnum.OpenWorld && !(this is Monster)) || World.Game.QuestManager.IsInQuestRange(_questRange);
					else
						Visible = true;
				}
				else
				{
					Visible = false;
					foreach (var plr in GetPlayersInRange(100f))
						Unreveal(plr);
				}
			else
				Visible = false;
		}

		public void SetUsable(bool activated)
		{
			Attributes[GameAttributes.Team_Override] = activated ? -1 : 2;
			Attributes[GameAttributes.Untargetable] = !activated;
			Attributes[GameAttributes.NPC_Is_Operatable] = activated;
			Attributes[GameAttributes.Operatable] = activated;
			Attributes[GameAttributes.Operatable_Story_Gizmo] = activated;
			Attributes[GameAttributes.Disabled] = !activated;
			Attributes[GameAttributes.Immunity] = !activated;
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
			return player.RevealedObjects.ContainsKey(GlobalID);
		}

		public ACDEnterKnownMessage ACDEnterKnown(Player plr)
		{
			return new ACDEnterKnownMessage
			{
				ActorID = DynamicID(plr),
				ActorSNOId = (int)SNO,
				Flags = Field2,
				LocationType = HasWorldLocation ? 0 : 1,
				WorldLocation = HasWorldLocation ? WorldLocationMessage() : null,
				InventoryLocation = HasWorldLocation ? null : InventoryLocationMessage(plr),
				GBHandle = GBHandle,
				snoGroup = Field7,
				snoHandle = (int)NameSNO,
				Quality = Quality,
				LookLinkIndex = Field10,
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
				if (Hidden || Dead || !Visible || World == null) return false;
				
                var mysticHiddenWorlds = new[] {
                    WorldSno.trdun_crypt_falsepassage_01,
					WorldSno.trdun_crypt_falsepassage_02,
					WorldSno.trdun_crypt_fields_flooded_memories_level01,
					WorldSno.trdun_crypt_fields_flooded_memories_level02,
					WorldSno.trdun_crypt_skeletonkingcrown_00,
					WorldSno.trdun_crypt_skeletonkingcrown_01,
					WorldSno.trdun_crypt_skeletonkingcrown_02,
				};
				//Leave Miriam in Crypt
				if (SNO == ActorSno._pt_mystic_novendor_nonglobalfollower && mysticHiddenWorlds.Contains(World.SNO)) return false;


				//Destroy Bonewall and Jondar if Exit_S on Second Level of Cathedral
				if (World.SNO == WorldSno.a1trdun_level04 && SNO is ActorSno._trdun_cath_bonewall_a_door or ActorSno._adventurer_d_templarintrounique) 
					return false;

				if (SNO.IsUberWorldActor() && !World.SNO.IsUberWorld()) return false;
				if (SNO.IsAdventureModeActor() && World.Game.CurrentAct != ActEnum.OpenWorld) return false;
				if (SNO == ActorSno._x1_adria_boss_scriptedsequenceonly) return false;

				if (player.RevealedObjects.ContainsKey(GlobalID)) return false; // already revealed

				if (player.World == null) return false;

				if (SNO == ActorSno._zombieskinny_custom_a && World.SNO == WorldSno.trout_town && CurrentScene.SceneSNO.Id == 33348 && Position.X < 2896) return false;


				if (!(this is Item) && player.World.GlobalID != World.GlobalID) return false;

				if (!(this is Item) && GetScenesInRange().Count > 0 && !GetScenesInRange().OrderBy(scene => PowerMath.Distance2D(scene.Position, Position)).First().IsRevealedToPlayer(player)) return false;

				uint objId = player.NewDynamicID(GlobalID, this is Player thisPlayer && (!thisPlayer.IsInPvPWorld || this == player) ? thisPlayer.PlayerIndex : -1);

				player.RevealedObjects.Add(GlobalID, objId);

				var gbIdBank = new int[AffixList.Count];
				int i = 0;
				foreach (var affix in AffixList)
				{
					gbIdBank[i] = affix.AffixGbid;
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
				if (this is Player)
				{
					msg.Flags = this == player ? 0x09 : 0x01;
				}

				player.InGameClient.SendMessage(msg);

				// Collision Flags
				if (this is not Projectile && this is not Item)
				{
					player.InGameClient.SendMessage(new ACDCollFlagsMessage
					{
						ActorID = objId,
						CollFlags = CollFlags
					});
				}

				// Send Attributes
				Attributes.SendMessage(player.InGameClient);

				if (this is Monster)
				{
					Attributes[GameAttributes.Hitpoints_Cur] += 0.001f;
					Attributes.BroadcastChangedIfRevealed();
				}

				// This is always sent even though it doesn't identify the actor. /komiga
				player.InGameClient.SendMessage(new PrefetchMessage
				{
					Name = ActorSNO
				});

				// Reveal actor (creates actor and makes it visible to the player)
				if (this is Player or NPC or Goblin)
					player.InGameClient.SendMessage(new ACDCreateActorMessage(objId));

				TrickleMessage trickle = new TrickleMessage()
				{
					ActorId = DynamicID(player),
					ActorSNO = (int)SNO,
					WorldLocation = new WorldPlace()
					{
						WorldID = World.GlobalID,
						Position = Position
					},
					HealthPercent = 1f,
					
				};

				if (this is Player playerTrickle)
					trickle.PlayerIndex = playerTrickle.PlayerIndex;

				player.InGameClient.SendMessage(trickle);


				// Actor group
				player.InGameClient.SendMessage(new ACDGroupMessage
				{
					ActorID = objId,
					Group1Hash = 0,
					Group2Hash = 0,
				});



				#region Special cases

				switch (World.SNO)
				{
					// set idle animation for zombies in tristram - ZHRAAT
					case WorldSno.trout_town:
					{
						if (Tags != null)
							if (Tags.ContainsKey(MarkerKeys.Group1Hash))
								if (Tags[MarkerKeys.Group1Hash] == -1248096796)
									PlayActionAnimation(AnimationSno.zombie_male_skinny_eating);
						break;
					}
					// set idle animation for workers
					case WorldSno.trout_tristram_inn when SNO == ActorSno._omninpc_tristram_male_a:
						PlayActionAnimation(AnimationSno.omninpc_male_hth_injured);
						break;
					default:
					{
						if (SNO == ActorSno._leah)
							player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Inventory.VisualInventoryMessage()
							{
								ActorID = DynamicID(player),
								EquipmentList = new VisualEquipment()
								{
									Equipment = new VisualItem[]
									{
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = unchecked((int)-2091504072),
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,//0x6C3B0389,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
										new()
										{
											GbId = -1,
											DyeType = 0,
											ItemEffectType = 0,
											EffectLevel = -1,
										},
									}
								}
							});
						break;
					}
				}

				#endregion
				// if (this is NPC || this is InteractiveNPC)
				// {
				// 	//.Contains<TagMap>(AnimationSetKeys.Idle)
				// 	//if (this.AnimationSet.Animations.ContainsKey(AnimationSetKeys.Idle.ID))
				// 	//	this.SetIdleAnimation(this.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Idle]);
				// 	//this.PlayAnimation(0, this.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Idle]);
				// }

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
				if (!player.RevealedObjects.ContainsKey(GlobalID)) return false; // not revealed yet
				if (!(this is Item) && player.World.GlobalID != World.GlobalID) return false;

				//PreloadRemoveACDMessage
				var gbidbank = new int[AffixList.Count];
				int i = 0;
				foreach(var affix in AffixList)
                {
					gbidbank[i] = affix.AffixGbid;
					i++;
                }
				if (this is Player)
					player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.InventoryCreateMessage)
					{
						ActorID = DynamicID(player),
					});
				if (this is Minion)
				{
					uint DynID = 0;
					player.RevealedObjects.TryGetValue(GlobalID, out DynID);
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
				player.InGameClient.SendMessage(new ACDDestroyActorMessage(DynamicID(player)));

				//Logger.Trace("Unreveal actor {0} as {1}", this.GlobalID, this.DynamicID(player));
				player.RevealedObjects.Remove(GlobalID);

				//if (!(this is Item) && this.Dead && this.World.Players.Values.Where(p => this.IsRevealedToPlayer(p)).Count() == 0)
				//this.Destroy();
				return true;
			}
		}

		#endregion

		#region proximity-based query helpers

		#region circurlar region queries

		public List<Player> GetPlayersInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<Player>(radius);
		}

		public List<Item> GetItemsInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<Item>(radius);
		}

		public List<Monster> GetMonstersInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<Monster>(radius);
		}

		public List<Actor> GetActorsInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			if (World == null || Position == null) return new List<Actor>();

			return GetObjectsInRange<Actor>(radius);
		}

		public List<Actor> GetActorsInRange(Vector3D TPosition, float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<Actor>(TPosition, radius);
		}

		public List<T> GetObjectsInRange<T>(Vector3D TPosition, float? radius = null) where T : WorldObject
		{
			var proximityCircle = new Circle(TPosition.X, TPosition.Y, radius ?? DefaultQueryProximityRadius);
			return World.QuadTree.Query<T>(proximityCircle);
		}

		public List<T> GetActorsInRange<T>(float? radius = null) where T : Actor
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<T>(radius);
		}

		public List<Scene> GetScenesInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<Scene>(radius);
		}

		public List<WorldObject> GetObjectsInRange(float? radius = null)
		{
			radius ??= DefaultQueryProximityRadius;
			return GetObjectsInRange<WorldObject>(radius);
		}

		public List<T> GetObjectsInRange<T>(float? radius = null, bool includeHierarchy = false) where T : WorldObject
		{
			if (World == null || Position == null) return new List<T>();
			radius ??= DefaultQueryProximityRadius;
			var proximityCircle = new Circle(Position.X, Position.Y, radius.Value);
			return World.QuadTree.Query<T>(proximityCircle, includeHierarchy);
		}

		#endregion

		#region rectangluar region queries
		public List<Player> GetPlayersInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<Player>(lenght);
		public List<Item> GetItemsInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<Item>(lenght);
		public List<Monster> GetMonstersInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<Monster>(lenght);
		public List<Actor> GetActorsInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<Actor>(lenght);
		public List<T> GetActorsInRegion<T>(int lenght = DefaultQueryProximityLenght) where T : Actor => GetObjectsInRegion<T>(lenght);
		public List<Scene> GetScenesInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<Scene>(lenght);
		public List<WorldObject> GetObjectsInRegion(int lenght = DefaultQueryProximityLenght) => GetObjectsInRegion<WorldObject>(lenght);

		public List<T> GetObjectsInRegion<T>(int lenght = DefaultQueryProximityLenght) where T : WorldObject
		{
			// ReSharper disable PossibleLossOfFraction
			var proximityRectangle = new RectangleF(Position.X - lenght / 2, Position.Y - lenght / 2, lenght, lenght);
			// ReSharper enable PossibleLossOfFraction
			return World.QuadTree.Query<T>(proximityRectangle);
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
			return new ACDWorldPositionMessage { ActorID = DynamicID(plr), WorldLocation = WorldLocationMessage() };
		}

		public virtual ACDInventoryPositionMessage ACDInventoryPositionMessage(Player plr)
		{
			return new ACDInventoryPositionMessage()
			{
				ItemId = DynamicID(plr),
				InventoryLocation = InventoryLocationMessage(plr),
				LocType = 1 // TODO: find out what this is and why it must be 1...is it an enum?
			};
		}

		public virtual WorldLocationMessageData WorldLocationMessage()
		{
			return new WorldLocationMessageData { Scale = Scale, Transform = Transform, WorldID = World.GlobalID };
		}

		#endregion

		#region tag-readers

		/// <summary>
		/// Reads known tags from TagMapEntry and set the proper values.
		/// </summary>
		protected virtual void ReadTags()
		{
			if (Tags == null) return;

			// load scale from actor data and override it with marker tags if one is set
			Scale = ActorData.TagMap.ContainsKey(ActorKeys.Scale) ? ActorData.TagMap[ActorKeys.Scale] : 1;
			Scale = Tags.ContainsKey(MarkerKeys.Scale) ? Tags[MarkerKeys.Scale] : Scale;


			if (Tags.ContainsKey(MarkerKeys.QuestRange))
			{
				int snoQuestRange = Tags[MarkerKeys.QuestRange].Id;
				if (MPQStorage.Data.Assets[SNOGroup.QuestRange].ContainsKey(snoQuestRange))
					_questRange = MPQStorage.Data.Assets[SNOGroup.QuestRange][snoQuestRange].Data as DiIiS_NA.Core.MPQ.FileFormats.QuestRange;
				else Logger.Debug("Actor {0}  GlobalID {1} is tagged with unknown QuestRange {2}", NameSNO, GlobalID, snoQuestRange);
			}

			if (Tags.ContainsKey(MarkerKeys.ConversationList) && WorldGenerator.DefaultConversationLists.ContainsKey((int)SNO))
			{
				int snoConversationList = WorldGenerator.DefaultConversationLists[(int)SNO];//Tags[MarkerKeys.ConversationList].Id;

				Logger.Debug(" (ReadTags) actor {0} GlobalID {2} has a conversation list {1}", NameSNO, snoConversationList, GlobalID);

				if (MPQStorage.Data.Assets[SNOGroup.ConversationList].ContainsKey(snoConversationList))
					ConversationList = MPQStorage.Data.Assets[SNOGroup.ConversationList][snoConversationList].Data as DiIiS_NA.Core.MPQ.FileFormats.ConversationList;
				else
					if (snoConversationList != -1)
					Logger.Warn("Actor {0} - Conversation list {1} not found!", NameSNO, snoConversationList);
			}


			if (Tags.ContainsKey(MarkerKeys.TriggeredConversation))
				_snoTriggeredConversation = Tags[MarkerKeys.TriggeredConversation].Id;
		}

		#endregion

		#region movement

		public void Move(Vector3D point, float facingAngle)
		{
			CurrentDestination = point;
			if (point == Position) return;
			SetFacingRotation(facingAngle);

			// find suitable movement animation
			int aniTag;
			if (AnimationSet == null)
				aniTag = -1;
			else if (AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk) && !(this is Minion) && !(this is Hireling))
				aniTag = AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Walk);
			else if (AnimationSet.TagExists(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run))
				aniTag = AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Run);
			else
				aniTag = -1;
			World?.BroadcastIfRevealed(plr => new ACDTranslateNormalMessage
			{
				ActorId = DynamicID(plr),
				Position = point,
				Angle = facingAngle,
				SnapFacing = false,
				MovementSpeed = WalkSpeed,
				MoveFlags = 0,
				AnimationTag = aniTag
			}, this);
		}

		public void MoveSnapped(Vector3D point, float facingAngle)
		{
			Position = point;
			SetFacingRotation(facingAngle);

			World.BroadcastIfRevealed(plr => new ACDTranslateSnappedMessage
			{
				ActorId = (int)DynamicID(plr),
				Position = point,
				Angle = facingAngle,
				Field3 = false,
				Field4 = 0x900  // TODO: figure out when to use this field
			}, this);
		}

		#endregion

		public override string ToString() => $"[Actor] [Type: {ActorType}] SNOId:{SNO} GlobalId: {GlobalID} Position: {Position} Name: {Name}";
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
