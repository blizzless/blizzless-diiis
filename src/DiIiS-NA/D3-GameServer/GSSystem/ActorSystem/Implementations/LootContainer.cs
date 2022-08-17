//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
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

		public LootContainer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			if (this.ActorSNO.Id == 200872) this.Attributes[GameAttribute.MinimapActive] = true;

			if (this.ActorSNO.Name.ToLower().Contains("chest") || this.ActorSNO.Name.ToLower().Contains("corpse")) haveDrop = true;

			switch (snoId)
			{
				case 79319: //bloody
					this.Quality = 1;
					break;
				case 62860: //rare
				case 101500: //Zolt_rare
				case 363725: //event
					this.Quality = 2;
					break;
			}

			if (snoId == 363725) rewardChestAvailable = false;
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (this.ActorSNO.Id == 190524 && this.World.WorldSNO.Id != 158593) return false; //dakab chest
			if (this.ActorSNO.Id == 190708 && this.World.WorldSNO.Id == 158593) return false; //not dakab chest

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



		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			if (this.ActorSNO.Id == 450254)
			{
				
			}
			else
			{ 
				if (this.Attributes[GameAttribute.Disabled]) return;

				base.OnTargeted(player, message);

				player.AddAchievementCounter(74987243307152, 1);

				if (ActorData.TagMap.ContainsKey(ActorKeys.Lore))
					Logger.Debug("Lore detected: {0}", ActorData.TagMap[ActorKeys.Lore].Id);

				if (this.ActorSNO.Id == 213905) //Leor bone
				{
					foreach (var plr in this.GetPlayersInRange(30))
						this.World.SpawnItem(this, plr, -629520052);
				}
				if (this.ActorSNO.Id == 172948) //Black Mushroom
				{
					foreach (var plr in this.GetPlayersInRange(30))
						this.World.SpawnItem(this, plr, -1993550104);
				}
				if (this.ActorSNO.Id == 207706) //Rainbow Chest
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

				if (GeneratorsSystem.LoreRegistry.Lore.ContainsKey(this.World.WorldSNO.Id) && GeneratorsSystem.LoreRegistry.Lore[this.World.WorldSNO.Id].chests_lore.ContainsKey(this.ActorSNO.Id))
					foreach (var p in this.GetPlayersInRange(30))
						foreach (int loreId in GeneratorsSystem.LoreRegistry.Lore[this.World.WorldSNO.Id].chests_lore[this.ActorSNO.Id])
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
						AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
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

				if (this.ActorSNO.Id == 5747)
				{
					var lamp = this.GetActorsInRange(50f).Where(x => x.ActorSNO.Id == 5744 || x.ActorSNO.Id == 89503).First();
					if (lamp != null)
						(lamp as CathedralLamp).Die();
				}

				if (this.ActorSNO.Id == 2975)
				{
					if (this.World.WorldSNO.Id == 50610)
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
}
