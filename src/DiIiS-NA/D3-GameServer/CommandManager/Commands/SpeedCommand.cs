using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("speed", "Modify speed walk of you character.\nUsage: !speed <value>\nReset: !speed")]
public class SpeedCommand : CommandGroup
{
    [DefaultCommand]
    public string ModifySpeed(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient == null)
            return "This command can only be used in-game.";

        if (@params == null)
            return
                "Change the movement speed. Min 0 (Base), Max 2.\n You can use decimal values like 1.3 for example.";
        float speedValue;

        const float maxSpeed = 2;
        const float baseSpeed = 0.36f;

        if (@params.Any())
        {
            if (!float.TryParse(@params[0], out speedValue) || speedValue is < 0 or > maxSpeed)
                return "Invalid speed value. Must be a number between 0 and 2.";
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

    [CommandGroup("quest",
        "Retrieves information about quest states and manipulates quest progress.\n Usage: quest [triggers | trigger eventType eventValue | advance snoQuest]")]
    public class QuestCommand : CommandGroup
    {
        [DefaultCommand]
        public string Quest(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient == null)
                return "You cannot invoke this command from console.";

            if (invokerClient.InGameClient == null)
                return "You can only invoke this command while in-game.";

            return Info(@params, invokerClient);
        }

        [Command("advance", "Advances a quest by a single step\n Usage: advance")]
        public string Advance(string[] @params, BattleClient invokerClient)
        {
            try
            {
                invokerClient.InGameClient.Game.QuestManager.Advance();
                return "Advancing main quest line";
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
                return "Advancing side quest line";
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

            if (@params.Length != 1)
                return "Invalid arguments. Type 'help text public' to get help.";

            var questId = int.Parse(@params[0]);

            try
            {
                invokerClient.InGameClient.Game.QuestManager.LaunchSideQuest(questId, true);
                return "Advancing side quest line";
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

            if (@params.Length != 2)
                return "Invalid arguments. Type 'help text public' to get help.";

            var eventId = int.Parse(@params[0]);
            var duration = int.Parse(@params[1]);

            invokerClient.InGameClient.Game.QuestManager.LaunchQuestTimer(eventId, (float)duration,
                new Action<int>((q) => { }));

            return "Message sent.";
        }

        [Command("info", "Retrieves information about quest states.\n Usage: info")]
        public string Info(string[] @params, BattleClient invokerClient)
        {
            if (invokerClient?.InGameClient?.Game?.QuestManager is not {} questManager)
                return "You can only invoke this command while in-game.";
            
            var act = questManager.CurrentAct;
            var quest = questManager.Game.CurrentQuest;
            var questStep = questManager.Game.CurrentStep;
            var currentSideQuest = questManager.Game.CurrentSideQuest;
            var currentSideQuestStep = questManager.Game.CurrentSideStep;
            return $"Act: {act}\n" +
                   $"Quest: {quest}\n" +
                   $"Quest Step: {questStep}\n" +
                   $"Side Quest: {currentSideQuest}\n" +
                   $"Side Quest Step: {currentSideQuestStep}";
        }
    }

    [CommandGroup("lookup",
        "Searches in sno databases.\nUsage: lookup [actor|conv|power|scene|la|sp|weather] <pattern>")]
    public class LookupCommand : CommandGroup
    {
        [DefaultCommand]
        public string Search(string[] @params, BattleClient invokerClient)
        {
            if (@params == null)
                return Fallback();

            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var groupPair in MPQStorage.Data.Assets) matches.AddRange(from pair in groupPair.Value where pair.Value.Name.ToLower().Contains(pattern) select pair.Value);

            return matches.Aggregate(matches.Count >= 1 ? "Matches:\n" : "No matches found.",
                (current, match) =>
                    $"{current} [{match.SNOId:D6}] [{match.Group}] {match.Name}\n");
        }

        [Command("actor", "Allows you to search for an actor.\nUsage: lookup actor <pattern>")]
        public string Actor(string[] @params, BattleClient invokerClient)
        {
            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            var matches = (from pair in MPQStorage.Data.Assets[SNOGroup.Actor] where pair.Value.Name.ToLower().Contains(pattern) select pair.Value).ToList();

            return matches.Aggregate(matches.Count >= 1 ? "Actor Matches:\n" : "No match found.",
                (current, match) => current +
                                    $"[{match.SNOId:D6}] {match.Name} ({((ActorData)match.Data).Type} {(((ActorData)match.Data).Type == ActorType.Gizmo ? ((int)((ActorData)match.Data).TagMap[ActorKeys.GizmoGroup]).ToString() : "")})\n");
        }

        [Command("rope", "Allows you to search for an rope.\nUsage: lookup rope <pattern>")]
        public string Rope(string[] @params, BattleClient invokerClient)
        {
            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            var matches = (from pair in MPQStorage.Data.Assets[SNOGroup.Rope] where pair.Value.Name.ToLower().Contains(pattern) select pair.Value).ToList();

            return matches.Aggregate(matches.Count >= 1 ? "Rope Matches:\n" : "No match found.",
                (current, match) => current + $"[{match.SNOId:D6}] {match.Name}\n");
        }

        [Command("conv", "Allows you to search for an conversation.\nUsage: lookup conv <pattern>")]
        public string Conversation(string[] @params, BattleClient invokerClient)
        {
            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup actor' to get help.";

            var pattern = @params[0].ToLower();

            var matches = (from pair in MPQStorage.Data.Assets[SNOGroup.Conversation] where pair.Value.Name.ToLower().Contains(pattern) select pair.Value).ToList();

            return matches.Aggregate(matches.Count >= 1 ? "Conversation Matches:\n" : "No match found.",
                (current, match) => current + $"[{match.SNOId:D6}] {match.Name}\n");
        }

        [Command("power", "Allows you to search for a power.\nUsage: lookup power <pattern>")]
        public string Power(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup power' to get help.";

            if (@params[0].ToLower() == "id")
            {
                var num = int.Parse(@params[1]);
                matches.AddRange(from pair in MPQStorage.Data.Assets[SNOGroup.Power] where pair.Value.SNOId == num select pair.Value);
            }
            else
            {
                var pattern = @params[0].ToLower();
                matches.AddRange(from pair in MPQStorage.Data.Assets[SNOGroup.Power] where pair.Value.Name.ToLower().Contains(pattern) select pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "World Matches:\n" : "No match found.",
                (current, match) => current +
                                    $"[{match.SNOId:D6}] {match.Name} - {((World)match.Data).DynamicWorld}\n");
        }

        [Command("world",
            "Allows you to search for a world.\nUsage: lookup world <pattern> OR lookup world id <snoId>")]
        public string World(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup world' to get help.";

            if (@params[0].ToLower() == "id")
            {
                var num = int.Parse(@params[1]);
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Worlds])
                    if (pair.Value.SNOId == num)
                        matches.Add(pair.Value);
            }
            else
            {
                var pattern = @params[0].ToLower();
                foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Worlds])
                    if (pair.Value.Name.ToLower().Contains(pattern))
                        matches.Add(pair.Value);
            }

            return matches.Aggregate(matches.Count >= 1 ? "World Matches:\n" : "No match found.",
                (current, match) => current +
                                    $"[{match.SNOId:D6}] {match.Name} - {(match.Data as World).DynamicWorld}\n");
        }

        [Command("qr", "Show QuestRange of an actor.\nUsage: lookup qr <id>")]
        public string QuestRange(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup world' to get help.";

            var num = int.Parse(@params[0]);
            var qr_id = "-1";
            var qr_name = "None";
            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.QuestRange])
                if (pair.Value.SNOId == num)
                {
                    qr_id = pair.Value.SNOId.ToString("D6");
                    qr_name = pair.Value.Name;
                }

            return $"[{qr_id}] {qr_name}";
        }

        public static int GetExitBits(Asset scene)
        {
            if (scene.Name.Contains("_N_")) return 1;
            if (scene.Name.Contains("_S_")) return 2;
            if (scene.Name.Contains("_NS_")) return 3;
            if (scene.Name.Contains("_E_")) return 4;
            if (scene.Name.Contains("_NE_")) return 5;
            if (scene.Name.Contains("_SE_")) return 6;
            if (scene.Name.Contains("_NSE_")) return 7;
            if (scene.Name.Contains("_W_")) return 8;
            if (scene.Name.Contains("_NW_")) return 9;
            if (scene.Name.Contains("_SW_")) return 10;
            if (scene.Name.Contains("_NSW_")) return 11;
            if (scene.Name.Contains("_EW_")) return 12;
            if (scene.Name.Contains("_NEW_")) return 13;
            if (scene.Name.Contains("_SEW_")) return 14;
            if (scene.Name.Contains("_NSEW_")) return 15;
            return 0;
        }

        [Command("la", "Allows you to search for a LevelArea.\nUsage: lookup la <pattern>")]
        public string LevelArea(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup la' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.LevelArea])
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);

            return matches.Aggregate(matches.Count >= 1 ? "LevelArea Matches:\n" : "No match found.",
                (current, match) => current + $"[{match.SNOId:D6}] {match.Name}\n");
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

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup weather' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Weather])
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);

            return matches.Aggregate(matches.Count >= 1 ? "Weather Matches:\n" : "No match found.",
                (current, match) => current + $"[{match.SNOId:D6}] {match.Name}\n");
        }

        [Command("scene", "Allows you to search for a scene.\nUsage: lookup scene <pattern>")]
        public string Scene(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup scene' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.Scene])
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);

            return matches.Aggregate(matches.Count >= 1 ? "Scene Matches:\n" : "No match found.",
                (current, match) => current +
                                    $"[{match.SNOId:D6}] {match.Name} - {GetExitBits(match)}\n");
        }

        [Command("eg", "Allows you to search for an EffectGroup.\nUsage: lookup eg <pattern>")]
        public string EffectGroup(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<Asset>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup eg' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var pair in MPQStorage.Data.Assets[SNOGroup.EffectGroup])
                if (pair.Value.Name.ToLower().Contains(pattern))
                    matches.Add(pair.Value);

            return matches.Aggregate(matches.Count >= 1 ? "EffectGroup Matches:\n" : "No match found.",
                (current, match) => current +
                                    $"[{match.SNOId:D6}] {match.Name} - {GetExitBits(match)}\n");
        }

        [Command("item", "Allows you to search for an item.\nUsage: lookup item <pattern>")]
        public string Item(string[] @params, BattleClient invokerClient)
        {
            var matches = new List<GameBalance.ItemTable>();

            if (!@params.Any())
                return "Invalid arguments. Type 'help lookup item' to get help.";

            var pattern = @params[0].ToLower();

            foreach (var asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                var data = asset.Data as GameBalance;
                if (data == null || data.Type != GameBalance.BalanceType.Items) continue;

                foreach (var itemDefinition in data.Item)
                    if (itemDefinition.Name.ToLower().Contains(pattern))
                        matches.Add(itemDefinition);
            }

            return matches.Aggregate(matches.Count >= 1 ? "Item Matches:\n" : "No match found.",
                (current, match) => current + $"[{match.SNOActor:D6}] {match.Name}\n");
        }
    }
}