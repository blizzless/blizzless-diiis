//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
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
			if (this.SNO == ActorSno._a3dunrmpt_interactives_signal_fire_a_prop) this.Attributes[GameAttribute.MinimapActive] = true;

			if (this.SNO.IsChest() || this.SNO.IsCorpse()) haveDrop = true;

			switch (sno)
			{
				case ActorSno._trout_highlands_chest_bloody: //bloody
					this.Quality = 1;
					break;
				case ActorSno._trout_fields_chest_rare: //rare
				case ActorSno._a2dun_zolt_chest_rare: //Zolt_rare
				case ActorSno._x1_global_chest_startsclean: //event
					this.Quality = 2;
					break;
			}

			if (sno == ActorSno._x1_global_chest_startsclean) rewardChestAvailable = false;
		}

		public override bool Reveal(Player player)
		{
			if (this.SNO == ActorSno._a2dun_aqd_chest_special_facepuzzle_large && this.World.SNO != WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large) return false; //dakab chest
			if (this.SNO == ActorSno._a2dun_aqd_chest_rare_facepuzzlesmall && this.World.SNO == WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large) return false; //not dakab chest

			if (!rewardChestAvailable) return false; //event reward chest

			if (!base.Reveal(player))
				return false;
			if (this.Attributes[GameAttribute.Disabled])
			{
				player.InGameClient.SendMessage(new SetIdleAnimationMessage
				{
					ActorID = this.DynamicID(player),
					AnimationSNO = AnimationSetKeys.Open.ID
				});
			}
			return true;
		}



		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (this.SNO == ActorSno._p4_setdung_totem_cru_thorns)
				return;
			
			if (this.Attributes[GameAttribute.Disabled]) return;

			base.OnTargeted(player, message);

			player.AddAchievementCounter(74987243307152, 1);

			if (ActorData.TagMap.ContainsKey(ActorKeys.Lore))
				Logger.Debug("Lore detected: {0}", ActorData.TagMap[ActorKeys.Lore].Id);

			if (this.SNO == ActorSno._trout_highlands_manor_firewood) //Leor bone
			{
				foreach (var plr in this.GetPlayersInRange(30))
					this.World.SpawnItem(this, plr, -629520052);
			}
			if (this.SNO == ActorSno._trout_newtristram_adria_blackmushroom) //Black Mushroom
			{
				foreach (var plr in this.GetPlayersInRange(30))
					this.World.SpawnItem(this, plr, -1993550104);
			}
			if (this.SNO == ActorSno._caout_oasis_chest_rare_mapvendorcave) //Rainbow Chest
			{
				foreach (var plr in this.GetPlayersInRange(30))
					this.World.SpawnItem(this, plr, 725082635);
			}

			if (haveDrop)
			{
				var dropRates = this.World.Game.IsHardcore ? LootManager.GetSeasonalDropRates((int)this.Quality, Program.MaxLevel) : LootManager.GetDropRates((int)this.Quality, Program.MaxLevel);
				foreach (var rate in dropRates)
					foreach (var plr in this.GetPlayersInRange(30))
					{
						float seed = (float)FastRandom.Instance.NextDouble();
						if (seed < 0.8f)
							this.World.SpawnGold(this, plr);
						if (seed < 0.6f)
							this.World.SpawnGold(this, plr);
						if (seed < 0.5f)
							this.World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.2f)
							this.World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.07f)
							this.World.SpawnRandomGem(this, plr);
						if (seed < 0.04f)
							this.World.SpawnRandomGem(this, plr);
						if (seed < 0.10f)
							this.World.SpawnRandomPotion(this, plr);
						if (seed < (rate * (1f + plr.Attributes[GameAttribute.Magic_Find])))
						{
							var lootQuality = this.World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)this.Quality, this.World.Game.Difficulty) : LootManager.GetLootQuality((int)this.Quality, this.World.Game.Difficulty);
							this.World.SpawnRandomEquip(plr, plr, lootQuality);
						}
						else
							break;
					}
			}

			if (GeneratorsSystem.LoreRegistry.Lore.ContainsKey(this.World.SNO) && GeneratorsSystem.LoreRegistry.Lore[this.World.SNO].chests_lore.ContainsKey(this.SNO))
				foreach (var p in this.GetPlayersInRange(30))
					foreach (int loreId in GeneratorsSystem.LoreRegistry.Lore[this.World.SNO].chests_lore[this.SNO])
						if (!p.HasLore(loreId))
						{
							World.DropItem(this, null, ItemGenerator.CreateLore(p, loreId));
							break;
						}

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
				new PlayAnimationMessageSpec()
				{
					Duration = 50,
					AnimationSNO = (int)AnimationSet.Animations[AnimationSetKeys.Opening.ID],
					PermutationIndex = 0,
					AnimationTag = 0,
					Speed = 1
				}
				}

			}, this);

			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			this.Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
			//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
			this.Attributes[GameAttribute.Chest_Open, 0xFFFFFF] = true;
			Attributes.BroadcastChangedIfRevealed();

			this.Attributes[GameAttribute.Disabled] = true;

			if (this.SNO == ActorSno._trdun_cath_chandelier_trap_switch2)
			{
				var lamp = this.GetActorsInRange(50f).Where(x => x.SNO == ActorSno._trdun_cath_chandelier_trap || x.SNO == ActorSno._trdun_cath_braizer_trap).First();
				if (lamp != null)
					(lamp as CathedralLamp).Die();
			}

			if (this.SNO == ActorSno._a2dun_zolt_centerpiece_a)
			{
				if (this.World.SNO == WorldSno.a2dun_zolt_level01)
					foreach (var plr in this.World.Game.Players.Values)
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
					foreach (var plr in this.World.Game.Players.Values)
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
