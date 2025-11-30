using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class LootContainer : Gizmo
	{
		private bool haveDrop = false;
		public bool rewardChestAvailable = true;

		public LootContainer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			if (SNO == ActorSno._a3dunrmpt_interactives_signal_fire_a_prop) Attributes[GameAttributes.MinimapActive] = true;

			if (SNO.IsChest() || SNO.IsCorpse()) haveDrop = true;

			switch (sno)
			{
				case ActorSno._trout_highlands_chest_bloody: //bloody
					Quality = 1;
					break;
				case ActorSno._trout_fields_chest_rare: //rare
				case ActorSno._a2dun_zolt_chest_rare: //Zolt_rare
				case ActorSno._x1_global_chest_startsclean: //event
					Quality = 2;
					break;
			}

			if (sno == ActorSno._x1_global_chest_startsclean) rewardChestAvailable = false;
		}

		public override bool Reveal(Player player)
		{
			if (SNO == ActorSno._a2dun_aqd_chest_special_facepuzzle_large && World.SNO != WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large) return false; //dakab chest
			if (SNO == ActorSno._a2dun_aqd_chest_rare_facepuzzlesmall && World.SNO == WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large) return false; //not dakab chest

			if (!rewardChestAvailable) return false; //event reward chest

			if (!base.Reveal(player))
				return false;
			if (Attributes[GameAttributes.Disabled])
			{
				player.InGameClient.SendMessage(new SetIdleAnimationMessage
				{
					ActorID = DynamicID(player),
					AnimationSNO = AnimationSetKeys.Open.ID
				});
			}
			return true;
		}



		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (SNO == ActorSno._p4_setdung_totem_cru_thorns)
				return;
			
			if (Attributes[GameAttributes.Disabled]) return;

			base.OnTargeted(player, message);

			player.AddAchievementCounter(74987243307152, 1);

			if (ActorData.TagMap.ContainsKey(ActorKeys.Lore))
				Logger.Debug("Lore detected: {0}", ActorData.TagMap[ActorKeys.Lore].Id);

			if (SNO == ActorSno._trout_highlands_manor_firewood) //Leor bone
			{
				foreach (var plr in GetPlayersInRange(30))
					World.SpawnItem(this, plr, -629520052);
			}
			if (SNO == ActorSno._trout_newtristram_adria_blackmushroom) //Black Mushroom
			{
				foreach (var plr in GetPlayersInRange(30))
					World.SpawnItem(this, plr, -1993550104);
			}
			if (SNO == ActorSno._caout_oasis_chest_rare_mapvendorcave) //Rainbow Chest
			{
				foreach (var plr in GetPlayersInRange(30))
					World.SpawnItem(this, plr, 725082635);
			}

			if (haveDrop)
			{
				var dropRates = World.Game.IsHardcore ? LootManager.GetSeasonalDropRates((int)Quality, Program.MAX_LEVEL) : LootManager.GetDropRates((int)Quality, Program.MAX_LEVEL);
				foreach (var rate in dropRates)
					foreach (var plr in GetPlayersInRange(30))
					{
						float seed = (float)FastRandom.Instance.NextDouble();
						if (seed < 0.8f)
							World.SpawnGold(this, plr);
						if (seed < 0.6f)
							World.SpawnGold(this, plr);
						if (seed < 0.5f)
							World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.2f)
							World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.07f)
							World.SpawnRandomGem(this, plr);
						if (seed < 0.04f)
							World.SpawnRandomGem(this, plr);
						if (seed < 0.10f)
							World.SpawnRandomPotion(this, plr);
						if (seed < (rate * (1f + plr.Attributes[GameAttributes.Magic_Find])))
						{
							var lootQuality = World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)Quality, World.Game.Difficulty) : LootManager.GetLootQuality((int)Quality, World.Game.Difficulty);
							World.SpawnRandomEquip(plr, plr, lootQuality);
						}
						else
							break;
					}
			}

			if (GeneratorsSystem.LoreRegistry.Lore.ContainsKey(World.SNO) && GeneratorsSystem.LoreRegistry.Lore[World.SNO].chests_lore.ContainsKey(SNO))
				foreach (var p in GetPlayersInRange(30))
					foreach (int loreId in GeneratorsSystem.LoreRegistry.Lore[World.SNO].chests_lore[SNO])
						if (!p.HasLore(loreId))
						{
							World.DropItem(this, null, ItemGenerator.CreateLore(p, loreId));
							break;
						}

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
				new PlayAnimationMessageSpec()
				{
					Duration = 50,
					AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
					PermutationIndex = 0,
					AnimationTag = 0,
					Speed = 1
				}
				}

			}, this);

			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			Attributes[GameAttributes.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			Attributes[GameAttributes.Chest_Open, 0xFFFFFF] = true;
			Attributes.BroadcastChangedIfRevealed();

			Attributes[GameAttributes.Disabled] = true;

			if (SNO == ActorSno._trdun_cath_chandelier_trap_switch2)
			{
				var lamp = GetActorsInRange(50f).Where(x => x.SNO == ActorSno._trdun_cath_chandelier_trap || x.SNO == ActorSno._trdun_cath_braizer_trap).First();
				if (lamp != null)
					(lamp as CathedralLamp).Die();
			}

			if (SNO == ActorSno._a2dun_zolt_centerpiece_a)
			{
				if (World.SNO == WorldSno.a2dun_zolt_level01)
					foreach (var plr in World.Game.Players.Values)
						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 57337,
							snoLevelArea = -1,
							StepID = 35,
							TaskIndex = 0,
							Counter = 1,
							Checked = 1,
						});
				else
					foreach (var plr in World.Game.Players.Values)
						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 57337,
							snoLevelArea = -1,
							StepID = 35,
							TaskIndex = 1,
							Counter = 1,
							Checked = 1,
						});
			}
		}
	}
}
