using DiIiS_NA.GameServer;
using DiIiS_NA.GameServer.CommandManager;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.D3_GameServer.CommandManager.Commands
{
    [CommandGroup("betrayal", "Gives player ability to attack other players. \n Usage: !betrayal", inGameOnly: true)]
    internal class BetrayalCommand : CommandGroup
    {
        [DefaultCommand(inGameOnly: true)]
        public string Betrayal(string[] @params, BattleClient invokerClient)
        {
            if (!GameServerConfig.Instance.BetrayalCommand)
            {
                return "Betrayal command disabled.";
            }

            foreach (var player in invokerClient.InGameClient.Game.Players)
            {
                var attributes = player.Value.Attributes;
                var fixedMap = attributes.FixedMap;
                if (!fixedMap.Contains(FixedAttribute.Betrayal))
                {
                    fixedMap.Add(FixedAttribute.Betrayal, (attributes) =>
                    {
                        attributes[GameAttributes.Team_Override] = 1;
                    },
                    attributes => // on removal
                    {
                        attributes[GameAttributes.Team_Override] = -1;
                    });

                    attributes.BroadcastIfRevealed();
                }
                else
                {
                    fixedMap.Remove(FixedAttribute.Betrayal);
                    attributes.BroadcastIfRevealed();
                }
            }

            return "Betrayal toggled: With betrayal mode Players can attack each other.";
        }
    }
}
