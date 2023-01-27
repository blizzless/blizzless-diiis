//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using Microsoft.Data.Sqlite;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Concurrent;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
using Spectre.Console;

namespace DiIiS_NA.Core.MPQ
{
    public class Data : MPQPatchChain
    {
        public Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>> Assets = new Dictionary<SNOGroup, ConcurrentDictionary<int, Asset>>();
        public readonly Dictionary<SNOGroup, Type> Parsers = new Dictionary<SNOGroup, Type>();
        private readonly List<Task> _tasks = new List<Task>();
        private static readonly SNOGroup[] PatchExceptions = new[] { SNOGroup.TimedEvent, SNOGroup.Script, SNOGroup.AiBehavior, SNOGroup.AiState, SNOGroup.Conductor, SNOGroup.FlagSet, SNOGroup.Code };
        #region Словари
        public static Dictionary<string, int> DictSNOAccolade = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAct = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOActor = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAdventure = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAmbientSound = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAnim = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAnimation2D = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOAnimSet = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOBossEncounter = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOCondition = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOConversation = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOEffectGroup = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOEncounter = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOGameBalance = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOMarkerSet = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOMonster = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOMusic = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOObserver = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOLore = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOLevelArea = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOPower = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOPhysMesh = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNORopes = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOQuest = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOQuestRange = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNORecipe = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOScene = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOSkillKit = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOTutorial = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOWeathers = new Dictionary<string, int>();
        public static Dictionary<string, int> DictSNOWorlds = new Dictionary<string, int>();
        #endregion


        private static new readonly Logger Logger = LogManager.CreateLogger("DataBaseWorker");

        public Data()
            //: base(0, new List<string> { "CoreData.mpq", "ClientData.mpq" }, "/base/d3-update-base-(?<version>.*?).mpq")
            : base(0, new List<string> { "Core.mpq", "Core1.mpq", "Core2.mpq", "Core3.mpq", "Core4.mpq" }, "/base/d3-update-base-(?<version>.*?).mpq")
        { }

        public void Init()
        {
            Logger.Info("Loading Diablo III Assets..");
            DictSNOAccolade = Dicts.LoadAccolade();
            DictSNOAct = Dicts.LoadActs();
            DictSNOActor = Dicts.LoadActors();
            DictSNOAdventure = Dicts.LoadAdventure();
            DictSNOAmbientSound = Dicts.LoadAmbientSound();
            DictSNOAnim = Dicts.LoadAnim();
            DictSNOAnimation2D = Dicts.LoadAnimation2D();
            DictSNOAnimSet = Dicts.LoadAnimSet();
            DictSNOBossEncounter = Dicts.LoadBossEncounter();
            DictSNOCondition = Dicts.LoadCondition();
            DictSNOConversation = Dicts.LoadConversation();
            DictSNOEffectGroup = Dicts.LoadEffectGroup();
            DictSNOEncounter = Dicts.LoadEncounter();
            DictSNOGameBalance = Dicts.LoadGameBalance();
            DictSNOMarkerSet = Dicts.LoadMarkerSet();
            DictSNOMonster = Dicts.LoadMonster();
            DictSNOMusic = Dicts.LoadMusic();
            DictSNOObserver = Dicts.LoadObserver();
            DictSNOLore = Dicts.LoadLore();
            DictSNOLevelArea = Dicts.LoadLevelArea();
            DictSNOPower = Dicts.LoadPower();
            DictSNOPhysMesh = Dicts.LoadPhysMesh();
            DictSNOQuest = Dicts.LoadQuest();
            DictSNOQuestRange = Dicts.LoadQuestRange();
            DictSNORecipe = Dicts.LoadRecipe();
            DictSNORopes = Dicts.LoadRopes();
            DictSNOScene = Dicts.LoadScene();
            DictSNOSkillKit = Dicts.LoadSkillKit();
            DictSNOTutorial = Dicts.LoadTutorial();
            DictSNOWeathers = Dicts.LoadWeathers();
            DictSNOWorlds = Dicts.LoadWorlds();

            this.InitCatalog();
            this.LoadCatalogs();
        }

        private void InitCatalog()
        {
            foreach (SNOGroup group in Enum.GetValues(typeof(SNOGroup)))
            {
                this.Assets.Add(group, new ConcurrentDictionary<int, Asset>());
            }

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(FileFormat))) continue;
                var attributes = (FileFormatAttribute[])type.GetCustomAttributes(typeof(FileFormatAttribute), false);
                if (attributes.Length == 0) continue;

                Parsers.Add(attributes[0].Group, type);
            }
        }

        private void LoadCatalogs()
        {
            /*
            string[] MyFiles = Directory.GetFiles(@"E:\\Unpacked\\2.7.1\\Rope\\", @"*", SearchOption.AllDirectories);
            string writePath = @"E:\Unpacked\Rope.txt";
            int i = 0;
            //Blizzless Project 2022 
using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {

                foreach (var file in MyFiles)
                {
                    var splited = file.Split('\\');
                    string name = splited[8].Split('.')[0];

                    if (name != "Axe Bad Data")
                    {
                        var asset = new MPQAsset(SNOGroup.Rope, i, name);
                        asset.MpqFile = this.GetFile(asset.FileName, PatchExceptions.Contains(asset.Group));
                        if (asset.MpqFile != null)
                            this.ProcessAsset(asset);
                        i++;

                        try
                        {
                            sw.WriteLine(@"('{0}', {1});", name, (asset.Data as FileFormats.Rope).Header.SNOId);
                        }
                        catch
                        {
                            Console.WriteLine("Ошибка ассета {0}", name);
                            sw.WriteLine(@"('{0}', {1});", name, (asset.Data as FileFormats.Rope).Header.SNOId);
                        }
                    }
                }
            }
            //*/
            this.LoadSNODict(DictSNOAccolade, SNOGroup.Accolade);
            this.LoadSNODict(DictSNOAct, SNOGroup.Act);
            this.LoadSNODict(DictSNOActor, SNOGroup.Actor);
            this.LoadSNODict(DictSNOAdventure, SNOGroup.Adventure);
            this.LoadSNODict(DictSNOAmbientSound, SNOGroup.AmbientSound);
            this.LoadSNODict(DictSNOAnim, SNOGroup.Anim);
            this.LoadSNODict(DictSNOAnimation2D, SNOGroup.Animation2D);
            this.LoadSNODict(DictSNOAnimSet, SNOGroup.AnimSet);
            this.LoadSNODict(DictSNOBossEncounter, SNOGroup.BossEncounter);
            this.LoadSNODict(DictSNOConversation, SNOGroup.Conversation);
            this.LoadSNODict(DictSNOEffectGroup, SNOGroup.EffectGroup);
            this.LoadSNODict(DictSNOEncounter, SNOGroup.Encounter);
            this.LoadSNODict(DictSNOGameBalance, SNOGroup.GameBalance);
            this.LoadSNODict(DictSNOLevelArea, SNOGroup.LevelArea);
            this.LoadSNODict(DictSNOLore, SNOGroup.Lore);
            this.LoadSNODict(DictSNOMarkerSet, SNOGroup.MarkerSet);
            this.LoadSNODict(DictSNOMonster, SNOGroup.Monster);
            this.LoadSNODict(DictSNOMusic, SNOGroup.Music);
            this.LoadSNODict(DictSNOObserver, SNOGroup.Observer);
            this.LoadSNODict(DictSNOPhysMesh, SNOGroup.PhysMesh);
            this.LoadSNODict(DictSNOPower, SNOGroup.Power);
            this.LoadSNODict(DictSNOQuest, SNOGroup.Quest);
            this.LoadSNODict(DictSNOQuestRange, SNOGroup.QuestRange);
            this.LoadSNODict(DictSNORecipe, SNOGroup.Recipe);
            this.LoadSNODict(DictSNORopes, SNOGroup.Rope);
            this.LoadSNODict(DictSNOScene, SNOGroup.Scene);
            this.LoadSNODict(DictSNOSkillKit, SNOGroup.SkillKit);
            this.LoadSNODict(DictSNOTutorial, SNOGroup.Tutorial);
            this.LoadSNODict(DictSNOWeathers, SNOGroup.Weather);
            this.LoadSNODict(DictSNOWorlds, SNOGroup.Worlds);

            #if DEBUG
            Console.WriteLine();
            AnsiConsole.Write(new BreakdownChart()
                .FullSize()
                .AddItem("Accolade", DictSNOAccolade.Count, Color.Gold1)
                .AddItem("Act", DictSNOAct.Count, Color.Green)
                .AddItem("Actor", DictSNOActor.Count, Color.Blue)
                .AddItem("Adventure", DictSNOAdventure.Count, Color.Orange4_1)
                .AddItem("Ambient Sound", DictSNOAmbientSound.Count, Color.OrangeRed1)
                .AddItem("Animations", DictSNOAnim.Count, Color.Orchid)
                .AddItem("Animation 2D", DictSNOAnimation2D.Count, Color.BlueViolet)
                .AddItem("Animation Set", DictSNOAnimSet.Count, Color.Blue3)
                .AddItem("Boss Encounter", DictSNOBossEncounter.Count, Color.Aquamarine1)
                .AddItem("Conversation", DictSNOConversation.Count, Color.Aquamarine1_1)
                .AddItem("Effect Group", DictSNOEffectGroup.Count, Color.Yellow)
                .AddItem("Encounter", DictSNOEncounter.Count, Color.Green3_1)
                .AddItem("Game Balance", DictSNOGameBalance.Count, Color.GreenYellow)
                .AddItem("Level Area", DictSNOLevelArea.Count, Color.Grey62)
                .AddItem("Lore", DictSNOLore.Count, Color.Plum4)
                .AddItem("Marker Set", DictSNOMarkerSet.Count, Color.Salmon1)
                .AddItem("Monster", DictSNOMonster.Count, Color.Red)
                .AddItem("Music", DictSNOMusic.Count, Color.Olive)
                .AddItem("Observer", DictSNOObserver.Count, Color.Violet)
                .AddItem("Phys Mesh", DictSNOPhysMesh.Count, Color.CornflowerBlue)
                .AddItem("Power", DictSNOPower.Count, Color.LightPink1)
                .AddItem("Quest", DictSNOQuest.Count, Color.LightGreen)
                .AddItem("Quest Range", DictSNOQuestRange.Count, Color.LightGreen_1)
                .AddItem("Recipe", DictSNORecipe.Count, Color.Yellow2)
                .AddItem("Ropes", DictSNORopes.Count, Color.Yellow1)
                .AddItem("Scene", DictSNOScene.Count, Color.DarkOrange3)
                .AddItem("Skill Kit", DictSNOSkillKit.Count, Color.DeepPink4_1)
                .AddItem("Tutorial", DictSNOTutorial.Count, Color.NavajoWhite3)
                .AddItem("Weather", DictSNOWeathers.Count, Color.Navy)
                .AddItem("Worlds", DictSNOWorlds.Count, Color.SlateBlue3_1)
            );
            Console.WriteLine();
            #endif
            this.LoadDBCatalog();
        }

        private void LoadSNODict(Dictionary<string, int> DictSNO, SNOGroup group)
        {
            foreach (var point in DictSNO)
            {
                var asset = new MPQAsset(group, point.Value, point.Key);
                asset.MpqFile = this.GetFile(asset.FileName, PatchExceptions.Contains(asset.Group));
                if (asset.MpqFile != null)
                    this.ProcessAsset(asset);
            }
            //Logger.Info("Loaded assets - {0}, Category - {1}", Assets[group].Count, group);

        }

        private void LoadCatalog(string fileName, bool useBaseMPQ = false, List<SNOGroup> groupsToLoad = null)
        {
            var catalogFile = this.GetFile(fileName, useBaseMPQ);
            this._tasks.Clear();

            if (catalogFile == null)
            {
                Logger.Error("Couldn't load catalog file: {0}.", fileName);
                return;
            }

            var stream = catalogFile.Open();
            var assetsCount = stream.ReadValueS32();

            var timerStart = DateTime.Now;

            while (stream.Position < stream.Length)
            {
                stream.Position += 8;
                var group = (SNOGroup)stream.ReadValueS32();
                var snoId = stream.ReadValueS32();
                var name = stream.ReadString(128, true);
                if (groupsToLoad != null && !groupsToLoad.Contains(group))
                    continue;

                var asset = new MPQAsset(group, snoId, name);
                asset.MpqFile = this.GetFile(asset.FileName, PatchExceptions.Contains(asset.Group)); 
                
                if (asset.MpqFile != null)
                    this.ProcessAsset(asset); // process the asset.
            }

            stream.Close();
                       
            if (this._tasks.Count > 0) // if we're running in tasked mode, run the parser tasks.
            {
                foreach (var task in this._tasks)
                {
                    task.Start();
                }

                Task.WaitAll(this._tasks.ToArray()); // Wait all tasks to finish.
            }

            GC.Collect(); // force a garbage collection.
            GC.WaitForPendingFinalizers();

            var elapsedTime = DateTime.Now - timerStart;

            //if (Storage.Config.Instance.LazyLoading)
            Logger.Trace("Found a total of {0} assets from {1} catalog and postponed loading because lazy loading is activated.", assetsCount, fileName);
            //else
            //    Logger.Trace("Found a total of {0} assets from {1} catalog and parsed {2} of them in {3:c}.", assetsCount, fileName, this._tasks.Count, elapsedTime);
        }

        /// <summary>
        /// Load the table of contents from the database. the database toc contains the sno ids of all objects
        /// that should / can no longer be loaded from mpq because it is zeroed out or because we need to edit
        /// some of the fields
        /// </summary>
        private void LoadDBCatalog()
        {
            int assetCount = 0;
            var timerStart = DateTime.Now;

            //Blizzless Project 2022 
using (var cmd = new SqliteCommand("SELECT * FROM TOC", Storage.DBManager.MPQMirror))
            {
                var itemReader = cmd.ExecuteReader();

                if (itemReader.HasRows)
                {
                    while (itemReader.Read())
                    {
                        ProcessAsset(new DBAsset(
                            (SNOGroup)Enum.Parse(typeof(SNOGroup), itemReader["SNOGroup"].ToString()),
                            Convert.ToInt32(itemReader["SNOId"]),
                            itemReader["Name"].ToString()));
                        assetCount++;
                    }
                }
            }


        }

        /// <summary>
        /// Adds the asset to the dictionary and tries to parse it if a parser
        /// is found and lazy loading is deactivated
        /// </summary>
        /// <param name="asset">New asset to be processed</param>
        private void ProcessAsset(Asset asset)
        {
            this.Assets[asset.Group].TryAdd(asset.SNOId, asset);
            if (!this.Parsers.ContainsKey(asset.Group)) return;

            asset.Parser = this.Parsers[asset.Group];
            this._tasks.Add(new Task(() => asset.RunParser()));
        }

        private MpqFile GetFile(string fileName, bool startSearchingFromBaseMPQ = false)
        {
            MpqFile file = null;

            if (!startSearchingFromBaseMPQ)
                file = this.FileSystem.FindFile(fileName);
            else
            {
                foreach (MpqArchive archive in this.FileSystem.Archives.Reverse()) //search mpqs starting from base
                {
                    file = archive.FindFile(fileName);
                    if (file != null)
                        break;
                }
            }

            return file;
        }
    }
}
