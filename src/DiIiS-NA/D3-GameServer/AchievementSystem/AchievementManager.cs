﻿//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.AchievementSystem
{
	public class AchievementManager : RPCObject
	{
		private static readonly AchievementManager _instance = new AchievementManager();
		public static AchievementManager Instance { get { return _instance; } }
		public static string AchievementFileHash = "2c11c0ec90a821ecc3dac6b81db355b2a2ff9f15e1d4f9512f3a96380c980887";
		public static string AchievementFilename = AchievementFileHash + ".achu";
		public static string AchievementURL = "http://eu.depot.battle.net:1119/" + AchievementFilename;

		public static D3.AchievementsStaticData.AchievementFile Achievements;

		public static void Initialize()
		{
			if (File.Exists(Path.Combine(FileHelpers.AssemblyRoot, AchievementFilename)))
			{
				var br = new BinaryReader(File.Open(Path.Combine(FileHelpers.AssemblyRoot, AchievementFilename), FileMode.Open));
				Achievements = D3.AchievementsStaticData.AchievementFile.ParseFrom(br.ReadBytes((int)br.BaseStream.Length));
				br.Close();
				Logger.Info("Achievements loaded from file.");
			}
			else
			{
				Logger.Info("Achimevement file not founded! Try download...");
				var attempts = 0;
				byte[] data = new byte[] { };
				while (attempts < 5)
				{
					try
					{
						data = new System.Net.WebClient().DownloadData(AchievementURL);
						break;
					}
					catch (System.Net.WebException)
					{
						attempts++;
					}
				}
				try
				{
					Achievements = D3.AchievementsStaticData.AchievementFile.ParseFrom(data);
					if (attempts < 5)
					{
						var br = new BinaryWriter(File.Open(AchievementFilename, FileMode.CreateNew));
						br.Write(data);
						br.Close();

					}
					else
					{
						Logger.Error("Error of Downloading.");
					}
				}
				catch (Google.ProtocolBuffers.InvalidProtocolBufferException)
				{
					Achievements = D3.AchievementsStaticData.AchievementFile.CreateBuilder().Build();
					Logger.Error("File was downloaded, but error of read.");
				}
				catch (IOException)
				{
					Logger.Error("{0} error permission.", AchievementFilename);
				}

			}
		}

		public static int TotalAchievements
		{
			get { return Achievements.AchievementCount; }
		}
		public static int TotalCategories
		{
			get { return Achievements.CategoryCount; }
		}
		public static int TotalCriteria
		{
			get { return Achievements.CriteriaCount; }
		}
		public static IList<D3.AchievementsStaticData.StaticAchievementDefinition> GetAllAchievements
		{
			get { return Achievements.AchievementList; }
		}

		public static bool IsHardcore(ulong achId)
		{
			ulong category_id = Achievements.AchievementList.Single(a => a.Id == achId).CategoryId;
			while (category_id != 0)
			{
				if (category_id == 5505028)
					return true;
				var category_data = Achievements.CategoryList.Single(a => a.Id == category_id);
				if (!category_data.HasParentId)
					break;

				category_id = category_data.ParentId;
			}
			return false;
		}
		public static List<uint> UnserializeBytes(byte[] data)
		{
			return Enumerable.Range(0, data.Length / 4).Select(i => BitConverter.ToUInt32(data, i * 4)).ToList();
		}
		public static byte[] SerializeBytes(List<uint> data)
		{
			return data.SelectMany(BitConverter.GetBytes).ToArray();
		}

		public static D3.AchievementsStaticData.StaticAchievementDefinition GetAchievementById(ulong id)
		{
			D3.AchievementsStaticData.StaticAchievementDefinition Achi = Achievements.AchievementList.Where(ach => ach.Id == id).FirstOrDefault();
			if (Achi != null)
				return Achi;
			else
				return null;
		}

		public static void UpdateSnapshot(BattleClient client, ulong achievementId = 0, ulong criteriaId = 0, uint quantity = 1)
		{
			var snapshot = D3.Achievements.Snapshot.CreateBuilder();

			if (achievementId != 0)
				snapshot.AddAchievementSnapshot(D3.Achievements.AchievementUpdateRecord.CreateBuilder()
					.SetAchievementId(achievementId)//74987243307105)
					.SetCompletion((int)DateTime.Now.ToUnixTime())//1476016727)
					.Build()
				);

			if (criteriaId != 0)
				snapshot.AddCriteriaSnapshot(D3.Achievements.CriteriaUpdateRecord.CreateBuilder()
					.SetCriteriaId32AndFlags8((uint)criteriaId)
					.SetQuantity32(quantity)
					.Build()
				);
		}

		public static ulong GetMainCriteria(ulong achievementId)
		{
			return Achievements.CriteriaList.Where(c =>
				c.ParentAchievementId == achievementId
			).Select(c => c.CriteriaId).FirstOrDefault();
		}

		public static List<D3.AchievementsStaticData.StaticCriteriaDefinition> GetCriterias(ulong achievementId)
		{
			var a = Achievements.CriteriaList.Where(c =>
				c.ParentAchievementId == achievementId
			).Select(c => c).ToList();
			return a;
		}

		public static void GrantAchievement(BattleClient client, ulong achievementId)
		{
			Task.Run(() =>
			{
				lock (client._serviceLock)
				{
					Logger.Trace("GrantAchievement(): id {0}", achievementId);
					if (client.Account.GameAccount.Achievements.Where(a => a.AchievementId == achievementId && a.Completion != -1).Count() > 0) return;
					DBAchievements achievement = null;
					var achs = DBSessions.SessionQueryWhere<DBAchievements>(dbi =>
							dbi.DBGameAccount.Id == client.Account.GameAccount.PersistentID);
					foreach (var ach in achs)
						if (ach.AchievementId == achievementId)
							if (ach.CompleteTime != -1)
								return;
							else
								achievement = ach;

					if (achievement == null)
					{
						achievement = new DBAchievements()
						{
							DBGameAccount = client.Account.GameAccount.DBGameAccount,
							AchievementId = achievementId,
							Criteria = new byte[0],
							IsHardcore = IsHardcore(achievementId),
							CompleteTime = (int)DateTime.Now.ToUnixTime()
						};
						DBSessions.SessionSave(achievement);
					}
					else
					{
						achievement.CompleteTime = (int)DateTime.Now.ToUnixTime();
						DBSessions.SessionUpdate(achievement);
					}

					var record = D3.Achievements.AchievementUpdateRecord.CreateBuilder()
							.SetAchievementId(achievementId)
							.SetCompletion((int)DateTime.Now.ToUnixTime())
							.Build();

					client.Account.GameAccount.Achievements.Add(record);

					UpdateSnapshot(client, achievementId);

					if (IsHardcore(achievementId))
					{
						if (achs.Where(a => a.CompleteTime != -1 && a.IsHardcore == true).Count() >= 30) //31 in total
						{
							var toons = DBSessions.SessionQueryWhere<DBToon>(dbt => dbt.DBGameAccount.Id == client.Account.GameAccount.PersistentID && dbt.isHardcore == true && dbt.Archieved == false);
						}
					}

					if (client.Account.GameAccount.Clan != null)
						client.Account.GameAccount.Clan.AddNews(client.Account.GameAccount, 1, D3.Guild.AchievementNews.CreateBuilder().SetAchievementId(achievementId).Build().ToByteArray());

					var criterias = Achievements.CriteriaList.Where(c =>
						(c.AdvanceEvent.Id == 200 && c.AdvanceEvent.Comparand == achievementId)
					).ToList();
					foreach (var criteria in criterias)
						GrantCriteria(client, criteria.CriteriaId);
				}
			});
		}

		public static void GrantCriteria(BattleClient client, ulong criteriaId)
		{
			Task.Run(() =>
			{
				lock (client._serviceLock)
				{
					Logger.Trace("GrantCriteria(): id {0}", criteriaId);
					D3.AchievementsStaticData.StaticCriteriaDefinition definition = null;

					uint UCriteriaId = (uint)criteriaId;

					var criteria_datas = Achievements.CriteriaList.Where(c => c.CriteriaId == criteriaId).ToList();
					if (criteriaId != 3367569)
					{
						if (criteria_datas.Count == 0)
							return;
						else
							definition = criteria_datas.First();

						var achs = DBSessions.SessionQueryWhere<DBAchievements>(dbi =>
							dbi.DBGameAccount.Id == client.Account.GameAccount.PersistentID &&
							dbi.AchievementId == definition.ParentAchievementId);

						var achievement = new DBAchievements();
						if (achs.Count == 0)
						{
							Logger.Trace("GrantCriteria(): creating new ach data");
							achievement.DBGameAccount = client.Account.GameAccount.DBGameAccount;
							achievement.AchievementId = definition.ParentAchievementId;
							achievement.IsHardcore = IsHardcore(definition.ParentAchievementId);
							achievement.CompleteTime = -1;
							achievement.Quantity = 0;
							List<uint> crits = new List<uint>();
							crits.Add(UCriteriaId);
							achievement.Criteria = SerializeBytes(crits);
							Logger.Trace("GrantCriteria(): {0} - {1} - {2} - {3}", client.Account.GameAccount.PersistentID, definition.ParentAchievementId, achievement.CompleteTime, UCriteriaId);
							DBSessions.SessionSave(achievement);
						}
						else
						{
							achievement = achs.First();
							if (!UnserializeBytes(achievement.Criteria).Contains(UCriteriaId))
							{
								Logger.Trace("GrantCriteria(): editing ach data, id: {0}", achievement.Id);
								List<uint> crits = UnserializeBytes(achievement.Criteria);
								crits.Add(UCriteriaId);
								achievement.Criteria = SerializeBytes(crits);
								DBSessions.SessionUpdate(achievement);
							}
							else return;
						}

						Logger.Trace("GrantCriteria(): ach data updated");

						var record = D3.Achievements.CriteriaUpdateRecord.CreateBuilder()
								.SetCriteriaId32AndFlags8(UCriteriaId)
								.SetQuantity32(1)
								.Build();

						client.Account.GameAccount.AchievementCriteria.Add(record);

						int critCount = UnserializeBytes(achievement.Criteria).Count;
						int neededCritCount = Achievements.CriteriaList.Where(c => c.ParentAchievementId == definition.ParentAchievementId).ToList().Count;

						if (critCount >= neededCritCount)
						{
							GrantAchievement(client, definition.ParentAchievementId);
							return;
						}

						var ach_data = Achievements.AchievementList.Single(a => a.Id == definition.ParentAchievementId);
						if (!ach_data.HasSupersedingAchievementId || client.Account.GameAccount.Achievements.Where(a => a.AchievementId == ach_data.SupersedingAchievementId && a.Completion > 0).Count() > 0)
							UpdateSnapshot(client, 0, criteriaId);
					}
					else
					{
						var achs = DBSessions.SessionQueryWhere<DBAchievements>(dbi =>
							dbi.DBGameAccount.Id == client.Account.GameAccount.PersistentID &&
							dbi.AchievementId == 1);

						var achievement = new DBAchievements();

						if (achs.Count == 0)
						{
							achievement.DBGameAccount = client.Account.GameAccount.DBGameAccount;
							achievement.AchievementId = 1;
							achievement.IsHardcore = false;
							achievement.CompleteTime = -1;
							achievement.Quantity = 0;
							List<uint> crits = new List<uint>();
							crits.Add(UCriteriaId);
							achievement.Criteria = SerializeBytes(crits);

							var record = D3.Achievements.CriteriaUpdateRecord.CreateBuilder()
								.SetCriteriaId32AndFlags8(UCriteriaId)
								.SetQuantity32(1)
								.Build();

							client.Account.GameAccount.AchievementCriteria.Add(record);

						}
						else
						{
							achievement = achs.First();
							
							Logger.Trace("GrantCriteria(): editing ach data, id: {0}", achievement.Id);
							List<uint> crits = UnserializeBytes(achievement.Criteria);
							crits.Add(UCriteriaId);
							achievement.Criteria = SerializeBytes(crits);
							DBSessions.SessionUpdate(achievement);

							var alreadycrits = client.Account.GameAccount.AchievementCriteria.Where(c => c.CriteriaId32AndFlags8 == 3367569).First();
							uint alcount = alreadycrits.CriteriaId32AndFlags8;
							var newrecord = D3.Achievements.CriteriaUpdateRecord.CreateBuilder()
								.SetCriteriaId32AndFlags8(UCriteriaId)
								.SetQuantity32(alcount++)
								.Build();
							int critCount = UnserializeBytes(achievement.Criteria).Count;

							if (critCount >= 5)
							{
								GrantCriteria(client, 74987246353740);
							}
							client.Account.GameAccount.AchievementCriteria.Remove(alreadycrits);
							client.Account.GameAccount.AchievementCriteria.Add(newrecord);
						}
						

						DBSessions.SessionSave(achievement);

					}
				}
			});
		}

		public static void UpdateQuantity(BattleClient client, ulong achievementId, uint additionalQuantity)
		{
			Task.Run(() =>
			{
				lock (client._serviceLock)
				{
					if (additionalQuantity == 0) return;
					Logger.Trace("UpdateQuantity(): id {0}", achievementId);
					if (client.Account.GameAccount.Achievements.Where(a => a.AchievementId == achievementId && a.Completion != -1).Count() > 0) return;

					ulong mainCriteriaId = GetMainCriteria(achievementId);
					var aa = client.Account.GameAccount.AchievementCriteria;
					D3.Achievements.CriteriaUpdateRecord mainCriteria;
					lock (client.Account.GameAccount.AchievementCriteria)
					{
						mainCriteria = client.Account.GameAccount.AchievementCriteria.Where(c => c.CriteriaId32AndFlags8 == (uint)mainCriteriaId).FirstOrDefault();
					}
					if (mainCriteria == null)
						mainCriteria = D3.Achievements.CriteriaUpdateRecord.CreateBuilder()
								.SetCriteriaId32AndFlags8((uint)mainCriteriaId)
								.SetQuantity32(0)
								.Build();

					var criteria_data = Achievements.CriteriaList.Single(c => c.CriteriaId == mainCriteriaId);

					uint newQuantity = Math.Min(mainCriteria.Quantity32 + additionalQuantity, (uint)criteria_data.NecessaryQuantity);

					if (newQuantity >= (uint)criteria_data.NecessaryQuantity)
					{
						GrantAchievement(client, achievementId);
						return;
					}

					var achs = DBSessions.SessionQueryWhere<DBAchievements>(dbi =>
							dbi.DBGameAccount.Id == client.Account.GameAccount.PersistentID &&
							dbi.AchievementId == achievementId);

					var achievement = new DBAchievements();
					if (achs.Count == 0)
					{
						Logger.Trace("UpdateQuantity(): creating new ach data");
						achievement.DBGameAccount = client.Account.GameAccount.DBGameAccount;
						achievement.AchievementId = achievementId;
						achievement.IsHardcore = IsHardcore(achievementId);
						achievement.CompleteTime = -1;
						List<uint> crits = new List<uint>();
						achievement.Criteria = SerializeBytes(crits);
						achievement.Quantity = (int)newQuantity;
						DBSessions.SessionSave(achievement);
					}
					else
					{
						achievement = achs.First();
						Logger.Trace("UpdateQuantity(): editing ach data, id: {0}", achievement.Id);
						achievement.Quantity = (int)newQuantity;
						DBSessions.SessionUpdate(achievement);
					}

					if (client.Account.GameAccount.AchievementCriteria.Contains(mainCriteria))
						client.Account.GameAccount.AchievementCriteria.Remove(mainCriteria);

					client.Account.GameAccount.AchievementCriteria.Add(
						mainCriteria.ToBuilder().SetQuantity32(newQuantity).Build()
					);
					var ach_data = Achievements.AchievementList.Single(a => a.Id == achievementId);
					if (!ach_data.HasSupersedingAchievementId/* || client.Account.GameAccount.Achievements.Where(a => a.AchievementId == ach_data.SupersedingAchievementId && a.Completion > 0).Count() > 0*/)
						UpdateSnapshot(client, 0, (ulong)mainCriteria.CriteriaId32AndFlags8, newQuantity);
				}
			});
		}
		
		public static void UpdateAllCounters(BattleClient client, int type, uint addCounter, int comparand)
		{
			var criterias = Achievements.CriteriaList.Where(c => c.AdvanceEvent.Id == (ulong)type);
			if (comparand != -1)
				criterias = criterias.Where(c => c.AdvanceEvent.ModifierList.First().Comparand == (ulong)comparand);
			foreach (var criteria in criterias)
			{
				UpdateQuantity(client, criteria.ParentAchievementId, addCounter);
			}
		}

		public static void CheckQuestCriteria(BattleClient client, int questId, bool isCoop)
		{
			var criterias = Achievements.CriteriaList.Where(c =>
				(c.AdvanceEvent.Id == 406 && c.AdvanceEvent.Comparand == Convert.ToUInt64(questId))
				&&
				(isCoop ? true : (c.AdvanceEvent.ModifierCount == 0))

			//(c.AdvanceEvent.Id == 410 && (int)(c.AdvanceEvent.Comparand >> 8) == questId) //only for challenges
			).ToList();
			foreach (var criteria in criterias)
				GrantCriteria(client, criteria.CriteriaId);
		}

		public static void CheckLevelCap(BattleClient client)
		{
			var capped_toons = DBSessions.SessionQueryWhere<DBToon>(dbt =>
						dbt.DBGameAccount.Id == client.Account.GameAccount.PersistentID &&
						dbt.Level == 60);
			if (capped_toons.Count() >= 2)
				GrantAchievement(client, 74987243307116);
			if (capped_toons.Count() >= 5)
				GrantAchievement(client, 74987243307118);
			if (capped_toons.Count() >= 10)
				GrantAchievement(client, 74987243307120);

			int different_classes = 0;

			foreach (ToonClass toon_class in (ToonClass[])Enum.GetValues(typeof(ToonClass)))
			{
				var toons = capped_toons.Where(t => t.Class == toon_class);
				if (toons.Count() > 0) different_classes++;
				if (toons.Count() >= 2)
				{
					switch (toon_class)
					{
						case ToonClass.Barbarian:
							GrantAchievement(client, 74987243307044);
							break;
						case ToonClass.Monk:
							GrantAchievement(client, 74987243307542);
							break;
						case ToonClass.DemonHunter:
							GrantAchievement(client, 74987243307057);
							break;
						case ToonClass.WitchDoctor:
							GrantAchievement(client, 74987243307562);
							break;
						case ToonClass.Wizard:
							GrantAchievement(client, 74987243307581);
							break;
					}
				}
			}

			if (different_classes >= 2)
				GrantAchievement(client, 74987243307121);

			if (different_classes >= 5)
				GrantAchievement(client, 74987243307362);
		}

		public static void CheckSalvageItemCriteria(BattleClient client, int itemGbId)
		{
			var criterias = Achievements.CriteriaList.Where(c =>
				(c.AdvanceEvent.Id == 9 && c.AdvanceEvent.Comparand == Convert.ToUInt64(unchecked((ulong)itemGbId)))).ToList();
			foreach (var criteria in criterias)
				GrantCriteria(client, criteria.CriteriaId);
		}

		public static void CheckKillMonsterCriteria(BattleClient client, int actorId, int type, bool isHardcore)
		{
			var actorId64 = Convert.ToUInt64(actorId);
			var type64 = Convert.ToUInt64(type);
			var criterias = Achievements.CriteriaList.Where(c =>
				(c.AdvanceEvent.Id == 105 && c.AdvanceEvent.Comparand == actorId64)).ToList();

			if (!isHardcore)
				criterias = criterias.Where(c => c.AdvanceEvent.ModifierList.Where(m => m.NecessaryCondition == 306).Count() == 0).ToList();

			criterias = criterias.Where(c => c.AdvanceEvent.ModifierList.Single(m => m.NecessaryCondition == 103).Comparand == type64).ToList();

			foreach (var criteria in criterias)
				GrantCriteria(client, criteria.CriteriaId);
		}

		public static void CheckConversationCriteria(BattleClient client, int convId)
		{
			var criterias = Achievements.CriteriaList.Where(c =>
				(c.AdvanceEvent.Id == 604 && c.AdvanceEvent.Comparand == Convert.ToUInt64(convId))
				||
				(c.AdvanceEvent.Id == 601 && c.AdvanceEvent.Comparand == Convert.ToUInt64(convId))
			).ToList();
			foreach (var criteria in criterias)
				GrantCriteria(client, criteria.CriteriaId);
		}

		public static void CheckLevelAreaCriteria(BattleClient client, int laId)
		{
			if (laId != -1)
			{
				var criterias = Achievements.CriteriaList.Where(c =>
					(c.AdvanceEvent.Id == 407 && c.AdvanceEvent.Comparand == Convert.ToUInt64(laId))
				).ToList();
				foreach (var criteria in criterias)
					if (criteria.CriteriaId != 74987243308157) //timed Fast_Time cork
						GrantCriteria(client, criteria.CriteriaId);
			}
		}
	}
	/*
				
     */
}
