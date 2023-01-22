//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text.RegularExpressions;
//Blizzless Project 2022 
using Wintellect.PowerCollections;

namespace DiIiS_NA.Core.MPQ
{
    public class MPQPatchChain
    {
        protected static readonly Logger Logger = LogManager.CreateLogger("FS");

        public bool Loaded { get; private set; }
        public List<string> BaseMPQFiles = new List<string>();
        public string PatchPattern { get; private set; }
        public int RequiredVersion { get; private set; }
        public readonly OrderedMultiDictionary<int, string> MPQFileList = new OrderedMultiDictionary<int, string>(false);
        public readonly MpqFileSystem FileSystem = new MpqFileSystem();

        protected MPQPatchChain(int requiredVersion, IEnumerable<string> baseFiles, string patchPattern = null)
        {
            this.Loaded = false;
            this.RequiredVersion = requiredVersion;

            foreach (var file in baseFiles)
            {
                var mpqFile = MPQStorage.GetMPQFile(file);
                if (mpqFile == null)
                {
                    Logger.Error("Cannot find base MPQ file: {0}.", file);
                    return;
                }
                this.BaseMPQFiles.Add(mpqFile);
                Logger.Trace("Added MPQ storage: {0}.", file);
            }

            this.PatchPattern = patchPattern;
            this.ConstructChain();

            var topMostMPQVersion = this.MPQFileList.Reverse().First().Key; // check required version.
            if (topMostMPQVersion == this.RequiredVersion)
                this.Loaded = true;
            else
            {
                Logger.Error("Required patch-chain version {0} is not satified (found version: {1}).", this.RequiredVersion, topMostMPQVersion);
            }
        }

        private void ConstructChain()
        {
            // add base mpq files;
            foreach (var mpqFile in this.BaseMPQFiles)
            {
                MPQFileList.Add(0, mpqFile);
            }

            if (PatchPattern == null) return;

            /* match the mpq files for the patch chain */
            var patchRegex = new Regex(this.PatchPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var file in MPQStorage.MPQList)
            {
                var match = patchRegex.Match(file);
                if (!match.Success) continue;
                if (!match.Groups["version"].Success) continue;

                var patchName = match.Groups[0].Value;
                var patchVersion = Int32.Parse(match.Groups["version"].Value);

                if (patchVersion > this.RequiredVersion) // ignore the patch if it's version is higher than our required.
                {
                    Logger.Trace("Ignoring patch file {0}.", patchName);
                    continue;
                }

                MPQFileList.Add(patchVersion, file);
                Logger.Trace("Found patch file: {0}.", patchName);
            }

            /* add mpq's to mpq-file system in reverse-order (highest version first) */
            foreach (var pair in this.MPQFileList.Reverse())
            {
                foreach (var mpq in pair.Value)
                {
                    Logger.Trace("MPQ: {0}, added to the system.", System.IO.Path.GetFileName(mpq));
                    this.FileSystem.Archives.Add(new MpqArchive(new FileStream(mpq, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true));
                }
            }
            Logger.Trace("MPQ system constructed.");
        }
    }
}
