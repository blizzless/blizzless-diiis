//Blizzless Project 2022
//Blizzless Project 2022 
 //Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
 //Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
 //Blizzless Project 2022 
using System;

namespace DiIiS_NA.Core.MPQ
{
    public abstract class Asset
    {
        public SNOGroup Group { get; private set; }
        public Int32 SNOId { get; private set; }
        public string Name { get; private set; }
        public string FileName { get; protected set; }
        public Type Parser { get; set; }

        protected FileFormat _data = null;
        protected static readonly Logger Logger = LogManager.CreateLogger("A");

        public FileFormat Data
        {
            get
            {
                if (_data == null && SourceAvailable && Parser != null)
                {
                    try
                    {
                        RunParser();
                    }
                    catch (Exception e)
                    {
                        Logger.FatalException(e, "Bad MPQ detected, failed parsing asset: {0}", this.FileName);
                    }
                }
                return _data;
            }
        }

        protected abstract bool SourceAvailable { get; }


        public Asset(SNOGroup group, Int32 snoId, string name)
        {
            this.FileName = group + "\\" + name + FileExtensions.Extensions[(int)group];
            this.Group = group;
            this.SNOId = snoId;
            this.Name = name;
        }

        public abstract void RunParser();
    }
}
