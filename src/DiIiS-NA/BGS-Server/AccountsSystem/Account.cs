﻿//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using bgs.protocol.presence.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Crypthography;

namespace DiIiS_NA.LoginServer.AccountsSystem
{
	public class Account : PersistentRPCObject
	{
		private DBAccount _dbAccount = null; //may be cached forever, as only MooNetServer changes it
		public DBAccount DBAccount
		{
			get
			{
				return _dbAccount;
			}
			set
			{
				_dbAccount = value;
			}
		}

		//public D3.PartyMessage.ScreenStatus ScreenStatus { get; set; }

		public ByteStringPresenceField<D3.OnlineService.EntityId> LastPlayedGameAccountIdField
		{
			get
			{
				var val = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Account, 2, 0, LastSelectedGameAccount);
				return val;
			}
		}

		public ByteStringPresenceField<D3.OnlineService.EntityId> LastPlayedToonIdField
		{
			get
			{
				ByteStringPresenceField<D3.OnlineService.EntityId> val = null;
				if (GameAccount.CurrentToon != null)
					val = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Account, 1, 0, GameAccount.CurrentToon.D3EntityID);
				else
				{
					var Fake = D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(0);
					val = new ByteStringPresenceField<D3.OnlineService.EntityId>(FieldKeyHelper.Program.D3, FieldKeyHelper.OriginatingClass.Account, 1, 0, Fake.Build());
				}
				return val;
			}
		}

		public StringPresenceField RealIDTagField
		{
			get
			{
				return new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 1, 0, string.Format(""));
			}
		}


		public BoolPresenceField AccountOnlineField
		{
			get
			{
				var val = new BoolPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 2, 0, IsOnline);
				return val;
			}
		}

		public StringPresenceField AccountBattleTagField
		{
			get
			{
				var val = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 4, 0, BattleTagName + "#" + HashCode.ToString("D4"));
				return val;
			}
		}

		public StringPresenceField BroadcastMessageField
		{
			get
			{
				var val = new StringPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 2, 0, BroadcastMessage);
				return val;
			}
		}

		public EntityIdPresenceFieldList GameAccountListField
		{
			get
			{
				var val = new EntityIdPresenceFieldList(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 3, 0);
				val.Value.Add(GameAccount.BnetEntityId);
				return val;
			}
		}

		public ulong LastOnline = 1;

		public IntPresenceField LastOnlineField
		{
			get
			{
				var val = new IntPresenceField(FieldKeyHelper.Program.BNet, FieldKeyHelper.OriginatingClass.Account, 6, 0, 0);
				val.Value = (long)LastOnline;
				return val;
			}

			set
			{
				LastOnline = (ulong)value.Value;
				DBAccount.LastOnline = (ulong)value.Value;
				DBSessions.SessionUpdate(DBAccount);
			}

		}


		public bool IsOnline
		{
			get
			{
				//check if gameAccount is online
				return GameAccount.IsOnline;
			}
			set
			{
				GameAccount.IsOnline = value;
			}
		}

		public List<ulong> FriendsIds = new List<ulong>();

		public List<ulong> IgnoreIds = new List<ulong>();

		public string Email
		{
			get
			{
				return DBAccount.Email;
			}
			private set
			{
			}
		} 
		public string SaltedTicket
		{
			get { return DBAccount.SaltedTicket; }
			internal set
			{
				DBAccount.SaltedTicket = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		} 
		public byte[] Salt
		{
			get { return DBAccount.Salt.ToArray(); }
			internal set
			{
				DBAccount.Salt = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		}  // s- User's salt.
		public byte[] FullSalt
		{
			get { return DBAccount.Salt.ToArray(); }
		}  // s- User's salt.

		public byte[] PasswordVerifier
		{
			get { return DBAccount.PasswordVerifier; }
			internal set
			{
				DBAccount.PasswordVerifier = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		} // v - password verifier.

		public int HashCode
		{
			get
			{
				return DBAccount.HashCode;
			}
			private set
			{
				DBAccount.HashCode = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		}

		public string BattleTagName
		{
			get
			{
				bool staff = (DBAccount.UserLevel > UserLevels.Tester);
				//(controller as HandlerController).Client.Account.GameAccount.ProgramField.Value
				if(GameAccount.ProgramField.Value == "APP")
					return string.Format("{0}", DBAccount.BattleTagName);
				else if (GameAccount.ProgramField.Value == "D3")
					return string.Format("{0}", DBAccount.BattleTagName);
					//return string.Format(staff ? " {{icon:bnet}} {{c_legendary}}{0}{{/c}}" : ("{0}"), this.DBAccount.BattleTagName);
				else
					return string.Format("{0}", DBAccount.BattleTagName);
				//return (staff ? " {icon:bnet} " : (premium ? " {icon:gold} " : "")) + dbAcc.BattleTagName;
			}  //{c_blue}{/c}
			private set
			{
				DBAccount.BattleTagName = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		}

		public string BattleTag
		{
			get
			{
				return BattleTagName + "#" + HashCode.ToString("D4");
			}
			set
			{
				if (!value.Contains('#'))
					throw new Exception("BattleTag must contain '#'");

				var split = value.Split('#');
				DBAccount.BattleTagName = split[0];
				DBAccount.HashCode = Convert.ToInt32(split[1]);
				DBSessions.SessionUpdate(DBAccount);
			}
		}

		public UserLevels UserLevel
		{
			get { return DBAccount.UserLevel; }
			internal set
			{
				DBAccount.UserLevel = value;
				DBSessions.SessionUpdate(DBAccount);
			}
		} // user level for account.

		public long MuteTime = 0;
		public int GiftsSent = 0;

		private GameAccount _currentGameAccount;
		public ulong CurrentGameAccountId = 0;
		public GameAccount GameAccount
		{
			get
			{
				if (CurrentGameAccountId == 0) return null;
				if (_currentGameAccount == null)
					_currentGameAccount = GameAccountManager.GetAccountByPersistentID(CurrentGameAccountId);

				return _currentGameAccount;
			}
			set
			{
				_currentGameAccount = value;
				CurrentGameAccountId = value.PersistentID;
			}
		}

		public static readonly D3.OnlineService.EntityId AccountHasNoToons =
			D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(0).Build();

		public D3.OnlineService.EntityId LastSelectedGameAccount
		{
			get
			{
				return GameAccount.D3GameAccountId;
			}
		}

		public string BroadcastMessage = "";

		public Account(DBAccount dbAccount)
			: base(dbAccount.Id)
		{
			DBAccount = dbAccount;
			var account_relations = DBSessions.SessionQueryWhere<DBAccountLists>(dbl => dbl.ListOwner.Id == PersistentID);
			FriendsIds = new HashSet<ulong>(account_relations.Where(dbl => dbl.Type == "FRIEND").Select(a => a.ListTarget.Id)).ToList();
			IgnoreIds = new HashSet<ulong>(account_relations.Where(dbl => dbl.Type == "IGNORE").Select(a => a.ListTarget.Id)).ToList();
			LastOnline = dbAccount.LastOnline;
			SetFields();
		}


		private void SetFields()
		{
			BnetEntityId = bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(PersistentID).Build();
		}

		public void Update(IList<FieldOperation> operations)
		{
			List<FieldOperation> operationsToUpdate = new List<FieldOperation>();
			foreach (var operation in operations)
			{
				switch (operation.Operation)
				{
					case FieldOperation.Types.OperationType.SET:
						var op_build = DoSet(operation.Field);
						if (op_build.HasValue)
						{
							var new_op = operation.ToBuilder();
							new_op.SetField(op_build);
							operationsToUpdate.Add(new_op.Build());
						}
						break;
					case FieldOperation.Types.OperationType.CLEAR:
						DoClear(operation.Field);
						break;
					default:
						Logger.Warn("No operation type.");
						break;
				}
			}
			if (operationsToUpdate.Count > 0)
				UpdateSubscribers(Subscribers, operationsToUpdate);
		}

		private Field.Builder DoSet(Field field)
		{
			FieldOperation.Builder operation = FieldOperation.CreateBuilder();

			Field.Builder returnField = Field.CreateBuilder().SetKey(field.Key);
			if (GameAccount.LoggedInClient == null) return returnField;

			switch ((FieldKeyHelper.Program)field.Key.Program)
			{
				case FieldKeyHelper.Program.D3:
					returnField.SetValue((field.Value));
					Logger.Trace("{0} set Unknown D3:{1}:{2} to {3}", this, field.Key.Group, field.Key.Field, field.Value.IntValue);
					break;
				case FieldKeyHelper.Program.BNet:
					returnField.SetValue((field.Value));
					if (field.Key.Group == 1 && field.Key.Field == 2) // Account's broadcast message
					{
						Logger.Trace("{0} set broadcast message to {1}.", this, field.Value.StringValue);
						BroadcastMessage = field.Value.StringValue;
					}
					else if (field.Key.Group == 1 && field.Key.Field == 7) // Account's AFK status
					{
						Logger.Trace("{0} set AFK to {1}.", this, field.Value.IntValue);
					}
					else if (field.Key.Group == 1 && field.Key.Field == 11) // Account is busy (bool)
					{
						Logger.Trace("{0} set AwayStatus to {1}.", this, field.Value.BoolValue);
						
					}
					else if (field.Key.Group == 1 && field.Key.Field == 12) // Account is busy (bool)
					{
						Logger.Trace("{0} set AwayStatus to {1}.", this, field.Value.BoolValue);
					}
					else
					{
						Logger.Warn("Account Unknown set-key: {0}, {1}, {2}", field.Key.Program, field.Key.Group, field.Key.Field);
					}
					break;
			}
			//We only update subscribers on fields that actually change values.
			return returnField;
		}

		private void DoClear(Field field)
		{
			/*switch ((FieldKeyHelper.Program)field.Key.Program)
			{
				case FieldKeyHelper.Program.D3:
					Logger.Warn("Account: Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group,
								field.Key.Field);
					break;
				case FieldKeyHelper.Program.BNet:
					Logger.Warn("Account: Unknown clear-field: {0}, {1}, {2}", field.Key.Program, field.Key.Group,
								field.Key.Field);
					break;
			}*/
		}

		public Field QueryField(FieldKey queryKey)
		{
			var field = Field.CreateBuilder().SetKey(queryKey);

			switch ((FieldKeyHelper.Program)queryKey.Program)
			{
				case FieldKeyHelper.Program.D3:
					if (queryKey.Group == 1 && queryKey.Field == 1) // Account's last selected toon.
					{
						if (IsOnline) // check if the account is online actually.
							field.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(GameAccount.LastPlayedHeroId.ToByteString()).Build());
					}
					else if (queryKey.Group == 1 && queryKey.Field == 2) // Account's last selected Game Account
					{
						if (IsOnline) // check if the account is online actually.
							field.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(LastSelectedGameAccount.ToByteString()).Build());
					}
					else
					{
						Logger.Warn("Account Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
					}
					break;
				case FieldKeyHelper.Program.BNet:
					if (queryKey.Group == 1 && queryKey.Field == 4) // Account's battleTag
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(BattleTag).Build());
					}
					else if (queryKey.Group == 1 && queryKey.Field == 2) // Account's broadcast message
					{
						field.SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(BroadcastMessage).Build());
					}
					else
					{
						Logger.Warn("Account Unknown query-key: {0}, {1}, {2}", queryKey.Program, queryKey.Group, queryKey.Field);
					}
					break;
			}


			return field.HasValue ? field.Build() : null;
		}

		#region Notifications

		public override void NotifyUpdate()
		{
			var operations = ChangedFields.GetChangedFieldList();
			ChangedFields.ClearChanged();
			UpdateSubscribers(Subscribers, operations);
		}

		//account class generated
		//D3, Account,1,0 -> D3.OnlineService.EntityId: Last Played Hero
		//D3, Account,2,0 -> LastSelectedGameAccount
		//Bnet, Account,1,0 -> RealId Name
		//Bnet, Account,3,index -> GameAccount EntityIds
		//Bnet, Account,4,0 -> BattleTag

		public override List<FieldOperation> GetSubscriptionNotifications()
		{
			//TODO: Create delegate-move this out	
			/*this.GameAccountListField.Value.Clear();
			foreach (var pair in this.GameAccounts)
			{
				this.GameAccountListField.Value.Add(pair.BnetEntityId);
			}*/


			var operationList = new List<FieldOperation>();
			//if (this.LastSelectedHero != AccountHasNoToons)
			//operationList.Add(this.LastPlayedHeroIdField.GetFieldOperation());
			if (LastSelectedGameAccount != AccountHasNoToons)
			{
				operationList.Add(LastPlayedToonIdField.GetFieldOperation());
				operationList.Add(LastPlayedGameAccountIdField.GetFieldOperation());
			}
			operationList.Add(RealIDTagField.GetFieldOperation());
			operationList.Add(AccountOnlineField.GetFieldOperation());
			operationList.AddRange(GameAccountListField.GetFieldOperationList());
			operationList.Add(AccountBattleTagField.GetFieldOperation());
			operationList.Add(BroadcastMessageField.GetFieldOperation());
			operationList.Add(LastOnlineField.GetFieldOperation());

			return operationList;
		}



		#endregion
		public bool VerifyPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
				return false;

			if (password.Length < 8 || password.Length > 16)
				return false;

			var calculatedVerifier = SRP6a.CalculatePasswordVerifierForAccount(Email, password, FullSalt);
			return calculatedVerifier.SequenceEqual(PasswordVerifier);
		}

		public override string ToString()
		{
			return String.Format("{{ Account: {0} [lowId: {1}] }}", Email, BnetEntityId.Low);
		}

		/// <summary>
		/// User-levels.
		/// </summary>
		public enum UserLevels : byte
		{
			User,
			Tester,
			GM,
			Admin,
			Owner
		}
	}
}
