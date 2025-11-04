using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;

namespace DiIiS_NA
{
    public static class ActorExtensions
    {
        public static IEnumerable<Actor> WhereSceneId(this IEnumerable<Actor> worlds, int sceneId) => worlds.Where(World.WhereSceneId(sceneId));

        public static IEnumerable<T> CastWhere<T>(this IEnumerable<Actor> actors)
            where T : Actor =>
            actors.Where(s => s is T).Cast<T>();
    }
}
