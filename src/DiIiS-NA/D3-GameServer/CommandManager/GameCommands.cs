//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
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
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.CommandManager
{
    [CommandGroup("invulnerable", "Makes you invulnerable")]
    public class InvulnerableCommand : CommandGroup
    {
        [DefaultCommand]
        public string Invulnerable(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient?.InGameClient?.Player is not { } player)
                return "You can not invoke this command from console.";

            if (player.Attributes.FixedMap.Contains(FixedAttribute.Invulnerable))
            {
                player.Attributes.FixedMap.Remove(FixedAttribute.Invulnerable);
                player.Attributes[GameAttribute.Invulnerable] = false;
                player.Attributes.BroadcastChangedIfRevealed();
                return "You are no longer invulnerable.";
            }

            player.Attributes.FixedMap.Add(FixedAttribute.Invulnerable,
                attributes => { attributes[GameAttribute.Invulnerable] = true; });
            player.Attributes.BroadcastChangedIfRevealed();
            return "You are now invulnerable.";
        }
    }

    [CommandGroup("spawn", "Spawns a mob.\nUsage: spawn [actorSNO] [amount]")]
    public class SpawnCommand : CommandGroup
    {
        [DefaultCommand]
        public string Spawn(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var actorSNO = 6652; /* zombie */
            var amount = 1;

            /*
            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out amount))
                    amount = 1;

                if (amount > 100) amount = 100;

                if (@params.Count() > 1)
                    if (!Int32.TryParse(@params[1], out actorSNO))
                        actorSNO = 6652;
            }
            */
            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out actorSNO))
                    actorSNO = 6652;


                if (@params.Count() > 1)
                    if (!Int32.TryParse(@params[1], out amount))
                        amount = 1;
                if (amount > 100) amount = 100;

            }

            for (int i = 0; i < amount; i++)
            {
                var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Z);

                var monster = player.World.SpawnMonster((ActorSno)actorSNO, position);

            }
            return $"Spawned {amount} mobs with ActorSNO: {actorSNO}";
        }

    }

    [CommandGroup("levelup", "Levels your character.\nOptionally specify the number of levels: !levelup [count]")]
    public class LevelUpCommand : CommandGroup
    {
        [DefaultCommand]
        public string LevelUp(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var amount = 1;

            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out amount))
                    amount = 1;
            }

            for (int i = 0; i < amount; i++)
            {
                if (player.Level >= 70)
                {
                    player.UpdateExp((int)player.Attributes[GameAttribute.Alt_Experience_Next_Lo]);
                    player.PlayEffect(Effect.ParagonLevelUp, null, false);
                    player.World.PowerManager.RunPower(player, 252038);
                }
                else
                {
                    player.UpdateExp((int)player.Attributes[GameAttribute.Experience_Next_Lo]);
                    player.PlayEffect(Effect.LevelUp, null, false);
                    player.World.PowerManager.RunPower(player, 85954);
                }
            }




            player.Toon.GameAccount.NotifyUpdate();
            if (player.Level >= 70)
                return $"New paragon level: {player.ParagonLevel}";
            else
                return $"New level: {player.Toon.Level}";
        }
    }

    [CommandGroup("unlockart", "Unlock all artisans: !unlockart")]
    public class UnlockArtCommand : CommandGroup
    {
        [DefaultCommand]
        public string UnlockArt(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            
            player.BlacksmithUnlocked = true;
            player.JewelerUnlocked = true;
            player.MysticUnlocked = true;
            player.GrantAchievement(74987243307766); // Blacksmith
            player.GrantAchievement(74987243307780); // Jeweler
            player.GrantAchievement(74987247205955); // Mystic

            player.HirelingTemplarUnlocked = true;
            player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 1 });
            player.GrantAchievement(74987243307073);
            player.HirelingScoundrelUnlocked = true;
            player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 2 });
            player.GrantAchievement(74987243307147);
            player.HirelingEnchantressUnlocked = true;
            player.InGameClient.SendMessage(new HirelingNewUnlocked() { NewClass = 3 });
            player.GrantAchievement(74987243307145);

            player.LoadCrafterData();
            player.Toon.GameAccount.NotifyUpdate();
            return string.Format("All artisans Unlocked");
        }
    }

    [CommandGroup("platinum", "Platinum for your character.\nOptionally specify the number of levels: !platinum [count]")]
    public class PlatinumCommand : CommandGroup
    {
        [DefaultCommand]
        public string Platinum(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var amount = 1;

            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out amount))
                    amount = 1;
            }


            player.InGameClient.SendMessage(new PlatinumAwardedMessage
            {
                CurrentPlatinum = player.InGameClient.BnetClient.Account.GameAccount.Platinum,
                PlatinumIncrement = amount
            });

            player.InGameClient.BnetClient.Account.GameAccount.Platinum += amount;

            return string.Format("Platinum test");
        }
    }

    [CommandGroup("stashup", "Upgrade Stash.\n !stashup")]
    public class StashUpCommand : CommandGroup
    {
        [DefaultCommand]
        public string Stashup(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            
            player.Inventory.OnBuySharedStashSlots(null);

            return string.Format("Stash Upgraded");
        }
    }

    [CommandGroup("gold", "Gold for your character.\nOptionally specify the number of gold: !gold [count]")]
    public class GoldCommand : CommandGroup
    {
        [DefaultCommand]
        public string Gold(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var amount = 1;

            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out amount))
                    amount = 1;
            }

            player.Inventory.AddGoldAmount(amount);

            return $"Added Gold {amount}";
        }
    }

    [CommandGroup("achiplatinum", "Platinum for your character.\nOptionally specify the number of levels: !platinum [count]")]
    public class PlatinumAchiCommand : CommandGroup
    {
        [DefaultCommand]
        public string Platinum(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var amount = 1;
            var achiid = 74987243307074;

            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out amount))
                    amount = 1;

                //if (!Int32.TryParse(@params[1], out amount))
                //     achiid = 74987243307074;
            }


            player.InGameClient.SendMessage(new PlatinumAchievementAwardedMessage
            {
                CurrentPlatinum = 0,
                idAchievement = (ulong)achiid,
                PlatinumIncrement = amount
            });


            return string.Format("Achievement test");
        }
    }

    [CommandGroup("eff", "Platinum for your character.\nOptionally specify the number of levels: !eff [count]")]
    public class PlayEffectGroup : CommandGroup
    {
        [DefaultCommand]
        public string PlayEffectCommand(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var id = 1;

            if (@params != null)
            {
                if (!Int32.TryParse(@params[0], out id))
                    id = 1;
            }

            player.PlayEffectGroup(id);

            return $"PlayEffectGroup {id}";
        }
    }

    [CommandGroup("item", "Spawns an item (with a name or type).\nUsage: item [type <type>|<name>] [amount]")]
    public class ItemCommand : CommandGroup
    {
        [DefaultCommand]
        public string Spawn(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var name = "Dye_02";
            var amount = 1;


            if (@params == null)
                return Fallback();

            name = @params[0];

            if (!ItemGenerator.IsValidItem(name))
                return "You need to specify a valid item name!";


            if (@params.Count() == 1 || !Int32.TryParse(@params[1], out amount))
                amount = 1;

            if (amount > 100) amount = 100;

            for (int i = 0; i < amount; i++)
            {
                var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Z);

                var item = ItemGenerator.Cook(player, name);
                item.EnterWorld(position);
            }

            return $"Spawned {amount} items with name: {name}";

        }

        [Command("type", "Spawns random items of a given type.\nUsage: item type <type> [amount]")]
        public string Type(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;
            var name = "Dye";
            var amount = 1;


            if (@params == null)
                return "You need to specify a item type!";

            name = @params[0];

            var type = ItemGroup.FromString(name);

            if (type == null)
                return "The type given is not a valid item type.";

            if (@params.Count() == 1 || !Int32.TryParse(@params[1], out amount))
                amount = 1;

            if (amount > 100) amount = 100;

            for (int i = 0; i < amount; i++)
            {
                var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                                            player.Position.Z);

                var item = ItemGenerator.GenerateRandom(player, type);
                item.EnterWorld(position);
            }

            return $"Spawned {amount} items with type: {name}";
        }

        [Command("dropall", "Drops all items in Backpack.\nUsage: item dropall")]
        public string DropAll(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            var player = invokerClient.InGameClient.Player;

            var bpItems = new List<Item>(player.Inventory.GetBackPackItems());


            foreach (var item in bpItems)
            {
                var msg = new InventoryDropItemMessage { ItemID = item.DynamicID(player) };
                player.Inventory.Consume(invokerClient.InGameClient, msg);
            }
            return $"Dropped {bpItems.Count} Items for you";
        }
    }

    [CommandGroup("tp", "Transfers your character to another world.")]
    public class TeleportCommand : CommandGroup
    {
        [DefaultCommand]
        public string Portal(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            if (@params != null && @params.Count() > 0)
            {
                var worldId = 0;
                Int32.TryParse(@params[0], out worldId);

                if (worldId == 0)
                    return "Invalid arguments. Type 'help tp' to get help.";

                if (!MPQStorage.Data.Assets[SNOGroup.Worlds].ContainsKey(worldId))
                    return "There exist no world with SNOId: " + worldId;

                var world = invokerClient.InGameClient.Game.GetWorld((WorldSno)worldId);

                if (world == null)
                    return "Can't teleport you to world with snoId " + worldId;

                invokerClient.InGameClient.Player.ChangeWorld(world, world.StartingPoints.First().Position);

                var proximity = new System.Drawing.RectangleF(invokerClient.InGameClient.Player.Position.X - 1f, invokerClient.InGameClient.Player.Position.Y - 1f, 2f, 2f);
                var scenes = invokerClient.InGameClient.Player.World.QuadTree.Query<GSSystem.MapSystem.Scene>(proximity);
                if (scenes.Count == 0) return ""; // cork (is it real?)

                var scene = scenes[0]; // Parent scene /fasbat

                if (scenes.Count == 2) // What if it's a subscene?
                {
                    if (scenes[1].ParentChunkID != 0xFFFFFFFF)
                        scene = scenes[1];
                }

                var levelArea = scene.Specification.SNOLevelAreas[0];

                //handling quest triggers
                if (invokerClient.InGameClient.Player.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(levelArea)) //EnterLevelArea
                {
                    var trigger = invokerClient.InGameClient.Player.World.Game.SideQuestProgress.GlobalQuestTriggers[levelArea];
                    if (trigger.triggerType == QuestStepObjectiveType.EnterLevelArea)
                    {
                        try
                        {
                            trigger.questEvent.Execute(invokerClient.InGameClient.Player.World); // launch a questEvent
                        }
                        catch { }
                    }
                }
                foreach (var bounty in invokerClient.InGameClient.Player.World.Game.QuestManager.Bounties)
                    bounty.CheckLevelArea(levelArea);
                return $"Teleported to: {MPQStorage.Data.Assets[SNOGroup.Worlds][worldId].Name} [id: {worldId}]";
            }

            return "Invalid arguments. Type 'help tp' to get help.";
        }
    }

    [CommandGroup("conversation", "Starts a conversation. \n Usage: conversation snoConversation")]
    public class ConversationCommand : CommandGroup
    {
        [DefaultCommand]
        public string Conversation(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            if (@params.Count() != 1)
                return "Invalid arguments. Type 'help conversation' to get help.";

            try
            {
                var conversation = MPQStorage.Data.Assets[SNOGroup.Conversation][Int32.Parse(@params[0])];
                invokerClient.InGameClient.Player.Conversations.StartConversation(Int32.Parse(@params[0]));
                return $"Started conversation {conversation.FileName}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    [CommandGroup("speed", "Modify speed walk of you character.\nUsage: !speed <value>\nReset: !speed")]
    public class ModifySpeedCommand : CommandGroup
    {
        [DefaultCommand]
        public string ModifySpeed(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient?.InGameClient == null)
                return "This command can only be used in-game.";

            if (@params == null)
                return "Change the movement speed. Min 0 (Base), Max 2.\n You can use decimal values like 1,3 for example.";
            float speedValue;
            
            const float maxSpeed = 3; // 2;
            const float baseSpeed = 0.36f;
            
            if (@params.Any())
            {
                if (!float.TryParse(@params[0], out speedValue) || speedValue < 0 || speedValue > maxSpeed)
                    return ("Invalid speed value. Must be a number between 0 and 3.");
            }
            else
            {
                speedValue = 0;
            }

            var playerSpeed = invokerClient.InGameClient.Player.Attributes;

            if (playerSpeed.FixedMap.Contains(FixedAttribute.Speed))
                playerSpeed.FixedMap.Remove(FixedAttribute.Speed);

            if (speedValue <= baseSpeed) // Base Run Speed [Necrosummon]
            {
                playerSpeed[GameAttribute.Running_Rate] = baseSpeed;
                return $"Speed reset to Base Speed ({baseSpeed:0.000}).";
            }

            playerSpeed.FixedMap.Add(FixedAttribute.Speed, attr => attr[GameAttribute.Running_Rate] = speedValue);
            playerSpeed.BroadcastChangedIfRevealed();
            return $"Speed changed to {speedValue}";
        }

    [CommandGroup("quest", "Retrieves information about quest states and manipulates quest progress.\n Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]")]
    public class QuestCommand : CommandGroup
    {
        [DefaultCommand]
        public string Quest(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You can not invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while ingame.";

            return "";
        }

        [Command("advance", "Advances a quest by a single step\n Usage: advance")]
        public string Advance(string[] @params, BattleClient invokerClient)
        {
            try
            {
                invokerClient.InGameClient.Game.QuestManager.Advance();
                return String.Format("Advancing main quest line");
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Command("sideadvance", "Advances a side-quest by a single step\n Usage: sideadvance")]
        public string SideAdvance(string[] @params, BattleClient invokerClient)
        {
            try
            {
                invokerClient.InGameClient.Game.QuestManager.SideAdvance();
                return String.Format("Advancing side quest line");
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Command("event", "Launches chosen side-quest by snoID\n Usage: event snoId")]
        public string Event(string[] @params, BattleClient invokerClient)
        {
            if (@params == null)
                return Fallback();

            if (@params.Count() != 1)
                return "Invalid arguments. Type 'help text public' to get help.";

            int questId = Int32.Parse(@params[0]);

            try
            {
                invokerClient.InGameClient.Game.QuestManager.LaunchSideQuest(questId, true);
                return String.Format("Advancing side quest line");
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Command("timer", "Send broadcasted text message.\n Usage: public 'message'")]
        public string Timer(string[] @params, BattleClient invokerClient)
        {
            if (@params == null)
                return Fallback();

            if (@params.Count() != 2)
                return "Invalid arguments. Type 'help text public' to get help.";

            int eventId = Int32.Parse(@params[0]);
            int duration = Int32.Parse(@params[1]);

            invokerClient.InGameClient.Game.QuestManager.LaunchQuestTimer(eventId, (float)duration, new Action<int>((q) => { }));

            return String.Format("Message sended.");
        }
    }

    [CommandGroup("lookup", "Searches in sno databases.\nUsage: lookup [actor|conv|power|scene|la|sp|weather] <pattern>")]
    public class LookupCommand : CommandGroup
    {
        [DefaultCommand]
        public string Search(string[] @params, BattleClient invokerClient)
        {
            if (@params == null)
                return Fallback();

            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var groupPair in MPQStorage.Data.Assets)
            {
                foreach (var pair in groupPair.Value)
                {
                    if (pair.Value.Name.ToLower().Contains(pattern))
                        matches.Add(pair.Value);
                }
            }

            return matches.Aggregate(matches.Count >= 1 ? "Matches:\n" : "No matches found.",
                                     (current, match) => current +
                                                         $"[{match.SNOId.ToString("D6")}] [{match.Group}] {match.Name}\n");
        }

        [Command("actor", "Allows you to search for an actor.\nUsage: lookup actor <pattern>")]
        public string Actor(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Actor])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Actor Matches:\n" : "No match found.",
                                     (current, match) => current +
                                                         $"[{match.SNOId.ToString("D6")}] {match.Name} ({(match.Data as DiIiS_NA.Core.MPQ.FileFormats.Actor).Type} {(((match.Data as DiIiS_NA.Core.MPQ.FileFormats.Actor).Type == ActorType.Gizmo) ? ((int)(match.Data as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[ActorKeys.GizmoGroup]).ToString() : "")})\n");
        }

        [Command("rope", "Allows you to search for an rope.\nUsage: lookup rope <pattern>")]
        public string Rope(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Rope])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Rope Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOId.ToString("D6")}] {match.Name}\n");
        }

        [Command("conv", "Allows you to search for an conversation.\nUsage: lookup conv <pattern>")]
        public string Conversation(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Conversation])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Conversation Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOId.ToString("D6")}] {match.Name}\n");
        }

        [Command("power", "Allows you to search for a power.\nUsage: lookup power <pattern>")]
        public string Power(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup power' to get help.";

            if (@params[0].ToLower() == "id")
            {
                var num = Int32.Parse(@params[1]);
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Power])
                {
                    if (pair.Value.SNOId == num)
                        matches.Add(pair.Value);
                }
            }
            else
            {
                var pattern = @params[0].ToLower();
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Power])
                {
                    if (pair.Value.Name.ToLower().Contains(pattern))
                        matches.Add(pair.Value);
                }
            }

            return matches.Aggregate(matches.Count >= 1 ? "Power Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOId.ToString("D6")}] {match.Name}\n");
        }

        [Command("world", "Allows you to search for a world.\nUsage: lookup world <pattern> OR lookup world id <snoId>")]
        public string World(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup world' to get help.";

            if (@params[0].ToLower() == "id")
            {
                var num = Int32.Parse(@params[1]);
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Worlds])
                {
                    if (pair.Value.SNOId == num)
                        matches.Add(pair.Value);
                }
            }
            else
            {
                var pattern = @params[0].ToLower();
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Worlds])
                {
                    if (pair.Value.Name.ToLower().Contains(pattern))
                        matches.Add(pair.Value);
                }
            }

            return matches.Aggregate(matches.Count >= 1 ? "World Matches:\n" : "No match found.",
                                     (current, match) => current +
                                                         $"[{match.SNOId.ToString("D6")}] {match.Name} - {(match.Data as World).DynamicWorld}\n");
        }

        [Command("qr", "Show QuestRange of an actor.\nUsage: lookup qr <id>")]
        public string QuestRange(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup world' to get help.";

            var num = Int32.Parse(@params[0]);
            string qr_id = "-1";
            string qr_name = "None";
            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.QuestRange])
            {
                if (pair.Value.SNOId == num)
                {
                    qr_id = pair.Value.SNOId.ToString("D6");
                    qr_name = pair.Value.Name;
                }
            }

            return $"[{qr_id}] {qr_name}";
        }

        public static int GetExitBits(Asset scene)
        {
            if (scene.Name.Contains("_N_")) return 1;
            else if (scene.Name.Contains("_S_")) return 2;
            else if (scene.Name.Contains("_NS_")) return 3;
            else if (scene.Name.Contains("_E_")) return 4;
            else if (scene.Name.Contains("_NE_")) return 5;
            else if (scene.Name.Contains("_SE_")) return 6;
            else if (scene.Name.Contains("_NSE_")) return 7;
            else if (scene.Name.Contains("_W_")) return 8;
            else if (scene.Name.Contains("_NW_")) return 9;
            else if (scene.Name.Contains("_SW_")) return 10;
            else if (scene.Name.Contains("_NSW_")) return 11;
            else if (scene.Name.Contains("_EW_")) return 12;
            else if (scene.Name.Contains("_NEW_")) return 13;
            else if (scene.Name.Contains("_SEW_")) return 14;
            else if (scene.Name.Contains("_NSEW_")) return 15;
            else return 0;
        }

        [Command("la", "Allows you to search for a LevelArea.\nUsage: lookup la <pattern>")]
        public string LevelArea(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup la' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.LevelArea])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "LevelArea Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOId.ToString("D6")}] {match.Name}\n");
        }

        [Command("sp", "List all Starting Points in world.\nUsage: lookup sp")]
        public string StartingPoint(string[] @params, BattleClient invokerClient)
        {
            var matches = invokerClient.InGameClient.Player.World.StartingPoints;

            return matches.Aggregate(matches.Count >= 1 ? "Starting Points:\n" : "No match found.",
                                     (current, match) => current +
                                                         $"[{match.GlobalID.ToString("D6")}] {match.Name} - {match.TargetId}\n");
        }

        [Command("weather", "Allows you to search for a Weather.\nUsage: lookup weather <pattern>")]
        public string Weather(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup weather' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Weather])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Weather Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOId.ToString("D6")}] {match.Name}\n");
        }

        [Command("scene", "Allows you to search for a scene.\nUsage: lookup scene <pattern>")]
        public string Scene(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup scene' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Scene])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Scene Matches:\n" : "No match found.",
                                     (current, match) => current +
                                                         $"[{match.SNOId.ToString("D6")}] {match.Name} - {GetExitBits(match)}\n");
        }

        [Command("eg", "Allows you to search for an EffectGroup.\nUsage: lookup eg <pattern>")]
        public string EffectGroup(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup eg' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.EffectGroup])
            {
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "EffectGroup Matches:\n" : "No match found.",
                                     (current, match) => current +
                                                         $"[{match.SNOId.ToString("D6")}] {match.Name} - {GetExitBits(match)}\n");
        }

        [Command("item", "Allows you to search for an item.\nUsage: lookup item <pattern>")]
        public string Item(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<ItemTable>();

            if (@params.Count() < 1)
                return "Invalid arguments. Type 'help lookup item' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                var data = asset.Data as GameBalance;
                if (data == null || data.Type != BalanceType.Items) continue;

                foreach (var itemDefinition in data.Item)
                {
                    if (itemDefinition.Name.ToLower().Contains(pattern))
                        matches.Add(itemDefinition);
                }
            }
            return matches.Aggregate(matches.Count >= 1 ? "Item Matches:\n" : "No match found.",
                                     (current, match) => current + $"[{match.SNOActor.ToString("D6")}] {match.Name}\n");
        }
    }
}
