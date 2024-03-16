using DiIiS_NA.Core.Helpers.IO;
using DiIiS_NA.Core.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DiIiS_NA.Core.MPQ
{
    public static class MPQStorage
    {
        //private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly Logger Logger = LogManager.CreateLogger("DataBaseSystem");
        public static string MPQRoot
        {
            get { return "DataBase/"; }
        }
        private readonly static string MpqRoot = MPQRoot;

        public static List<string> MPQList { get; private set; }
        public static Data Data { get; private set; }
        public static bool Initialized { get; private set; }

        static MPQStorage()
        {
            Initialized = false;

            if (!Directory.Exists(MpqRoot))
            {
                Logger.Error("MPQ archives not found: {0}.", MpqRoot);
                return;
            }

            Logger.Info("Initializing MPQ archives...");
            MPQList = FileHelpers.GetFilesByExtensionRecursive(MpqRoot, ".mpq");

            Data = new Data();
            if (Data.Loaded)
            {
                Data.Init();
                Initialized = true;
            }
        }

        public static string GetMPQFile(string name)
        {
            return MPQList.FirstOrDefault(file => file.Contains(name));
        }
    }
}
