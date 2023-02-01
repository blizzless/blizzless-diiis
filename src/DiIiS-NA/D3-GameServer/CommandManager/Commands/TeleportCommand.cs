using System.Linq;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("tp", "Transfers your character to another world.")]
public class TeleportCommand : CommandGroup
{
    [DefaultCommand]
    public string Portal(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        if (@params != null && @params.Any())
        {
            int.TryParse(@params[0], out var worldId);

            if (worldId == 0)
                return "Invalid arguments. Type 'help tp' to get help.";

            if (!MPQStorage.Data.Assets[SNOGroup.Worlds].ContainsKey(worldId))
                return "There exist no world with SNOId: " + worldId;

            var world = invokerClient.InGameClient.Game.GetWorld((WorldSno)worldId);

            if (world == null)
                return "Can't teleport you to world with snoId " + worldId;

            invokerClient.InGameClient.Player.ChangeWorld(world, world.StartingPoints.First().Position);

            var proximity = new System.Drawing.RectangleF(invokerClient.InGameClient.Player.Position.X - 1f,
                invokerClient.InGameClient.Player.Position.Y - 1f, 2f, 2f);
            var scenes =
                invokerClient.InGameClient.Player.World.QuadTree.Query<GSSystem.MapSystem.Scene>(proximity);
            if (scenes.Count == 0) return ""; // cork (is it real?)

            var scene = scenes[0]; // Parent scene /fasbat

            if (scenes.Count == 2) // What if it's a subscene?
                if (scenes[1].ParentChunkID != 0xFFFFFFFF)
                    scene = scenes[1];

            var levelArea = scene.Specification.SNOLevelAreas[0];

            //handling quest triggers
            if (invokerClient.InGameClient.Player.World.Game.SideQuestProgress.GlobalQuestTriggers
                .ContainsKey(levelArea)) //EnterLevelArea
            {
                var trigger =
                    invokerClient.InGameClient.Player.World.Game.SideQuestProgress.GlobalQuestTriggers[levelArea];
                if (trigger.triggerType == QuestStepObjectiveType.EnterLevelArea)
                    try
                    {
                        trigger.questEvent.Execute(invokerClient.InGameClient.Player.World); // launch a questEvent
                    }
                    catch
                    {
                    }
            }

            foreach (var bounty in invokerClient.InGameClient.Player.World.Game.QuestManager.Bounties)
                bounty.CheckLevelArea(levelArea);
            return $"Teleported to: {MPQStorage.Data.Assets[SNOGroup.Worlds][worldId].Name} [id: {worldId}]";
        }

        return "Invalid arguments. Type 'help tp' to get help.";
    }
}