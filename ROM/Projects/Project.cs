using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Editroid.Properties;

namespace Editroid.ROM.Projects
{
    internal partial class Project
    {
        string _expansionFile;
        MetroidRom rom;
        List<UnusedScreenLoadASM> unusedAsm = new List<UnusedScreenLoadASM>();
        List<AsmFile> _Files = new List<AsmFile>();

        private Project() {
            this.GenerateDebugFiles = true;
        }
        public Project(MetroidRom rom) 
        :this() {
            Files = _Files;
            this._expansionFile = ProjectResources.DefaultMainFile;
            this.DefinesFile = ProjectResources.DefaultDefinesFile;
            this.GeneralCodeFile = ProjectResources.DefaultGeneralFile;

            this.rom = rom;
        }
        public Project(MetroidRom rom, Stream projFile)
        :this() {
            Files = _Files;
            this.rom = rom;

            for (int i = 0; i < magicNumber.Length; i++) {
                if (projFile.ReadByte() != magicNumber[i]) {
                    throw new ProjectLoadException("Project file is missing magic number");
                }
            }


            BinaryReader reader = new BinaryReader(projFile);
            //_expansionFile = reader.ReadString();

            LoadEntries(reader, false);
            if (ExtendedFormat) {
                LoadEntries(reader, true);
            }
        }


        public Project(MetroidRom rom, string projFilename) :
            this(rom, new FileStream(projFilename, FileMode.Open, FileAccess.Read)) {
        }

        public IList<AsmFile> Files { get; private set; }
        public string ExpansionFile { get { return _expansionFile; } set { _expansionFile = value; } }
        public string GeneralCodeFile { get; set; }
        public string DefinesFile { get; set; }
        public string ChrAnimationNameList { get; set; }
        public bool GenerateDebugFiles { get; set; }
        /// <summary>
        /// Gets/sets the directory debug files are to be written to. If this property isn't assigned, debug output will not be generated, regardless of the value of GenerateDebugFiles.
        /// </summary>
        public string DebugOutputDirectory { get; set; }

        public bool Debug { get; set; }
        /// <summary>
        /// Set when project is loaded, to indicate that the project file contained an extended data section. Ignored when saving project.
        /// </summary>
        public bool ExtendedFormat { get; set; }
        public bool BuildOnTestRoom { get; set; }
        ////public bool BuildOnSaveAndPlay { get; set; }
        /// <summary>
        /// This property allows the project to be marked as having unsaved changes. This
        /// is not used by the Project class internally.
        /// </summary>
        public bool UnsavedChanges { get; set; }

        /// <summary>
        /// If the project file is loaded from a stream, and contains data that could not be parsed, this
        /// property will return true. This is likely and indicator that the file is from a newer version
        /// of Editriod.
        /// </summary>
        public bool CouldNotParseAllData { get; private set; }

        #region Saving/loading
        private void LoadEntries(BinaryReader reader, bool extendedFormat) {
            int entrySize = 0;
            while (true) { // INFINITY
                if (extendedFormat) entrySize = reader.ReadInt32();
                var entryType = (EntryTypeCodes)reader.ReadByte();

                switch (entryType) {
                    case EntryTypeCodes.screenLoadAsm:
                        LoadScreenAsm(reader);
                        break;
                    case EntryTypeCodes.specialFile:
                        LoadSpecialFile(reader);
                        break;
                    case EntryTypeCodes.userFile:
                        LoadUserFile(reader);
                        break;
                    case EntryTypeCodes.eof:
                        return;
                    case EntryTypeCodes.flag:
                        LoadFlag(reader);
                        break;
                    case EntryTypeCodes.depracated_debugFlag:
                        Debug = reader.ReadBoolean();
                        break;
                    default:
                        if (extendedFormat) {
                            // The extended format allows us to skip entries with an unknown format
                            reader.ReadBytes(entrySize); // Read and discard
                            CouldNotParseAllData = true; // We'll want to let the user know
                        } else {
                            throw new ProjectLoadException("The project file contains data that can not be parsed. It may have been created with a newer version of Editroid.");
                        }
                        break;
                }
            }
            // unexpected eof
            throw new ProjectLoadException("The project file appears to be invalid. End of file reached unexpectedly.");

        }



 

        private void LoadFlag(BinaryReader reader) {
            string name = reader.ReadString();
            bool value = reader.ReadBoolean();

            switch (name) {
                case FlagNames.Debug :
                    Debug = value;
                    break;
                case FlagNames.BuildOnTestRoom :
                    BuildOnTestRoom = value;
                    break;
                ////case FlagNames.deprecated_BuildOnSaveAndPlay:
                ////    BuildOnSaveAndPlay = value;
                ////    break;
                case FlagNames.ExtendedFormat:
                    ExtendedFormat = true;
                    break;
                default:
                    CouldNotParseAllData = true;
                    break;
            }
        }

        private void LoadUserFile(BinaryReader reader) {
            string name = reader.ReadString();
            string code = reader.ReadString();
            bool autobuild = false;

            StringReader sreader = new StringReader(code);
            if (string.Equals(sreader.ReadLine(), autoBuildToken, StringComparison.OrdinalIgnoreCase)) {
                // Remove autobuild token from file
                code = sreader.ReadToEnd();
                autobuild = true;
            }

            Files.Add(new AsmFile() { Name = name, Code = code , AutoBuild = autobuild});
        }
        private void LoadSpecialFile(BinaryReader reader) {
            try {
                string name = reader.ReadString();
                switch (name) {
                    case declaresFilename:
                        DefinesFile = reader.ReadString();
                        break;
                    case generalCodeFilename:
                        GeneralCodeFile = reader.ReadString();
                        break;
                    case expansionCodeFilename:
                        ExpansionFile = reader.ReadString();
                        break;
                    case ChrAnimationNamesFilename:
                        ChrAnimationNameList = reader.ReadString();
                        break;
                    default:
                        // special file that is not recognized (presumably a future version)
                        string code = reader.ReadString();
                        UnusedScreenLoadASM orphaned = new UnusedScreenLoadASM();
                        orphaned.Level = LevelIndex.None;
                        orphaned.ScreenIndex = 0;
                        orphaned.ASM = Environment.NewLine + "; Orphaned file - " + name + Environment.NewLine + Environment.NewLine + code;
                        unusedAsm.Add(orphaned);
                        break;
                }
            } catch (IOException ex) {
                throw new ProjectLoadException("An error was encountered in the project file.", ex);
            }
        }

        private void LoadScreenAsm(BinaryReader reader) {
            LevelIndex level = LevelIndex.None;
            int screen = 0;
            string asm = null;
            try {
                level = (LevelIndex)(int)reader.ReadByte();
                screen = reader.ReadByte();
                asm = reader.ReadString();
            } catch (IOException ex) {
                throw new ProjectLoadException("An error was encountered in the project file.", ex);
            }

            try {
                var levelData = rom.Levels[level];
                if (screen < levelData.Screens.Count) {
                    levelData.Screens[screen].ScreenLoadASM = asm;
                    return;
                }
            } catch (System.Collections.Generic.KeyNotFoundException ex) {
                // eat it up
                // (Invalid level value)
            }

            var unused = new UnusedScreenLoadASM();
            unused.ASM = asm;
            unused.Level = level;
            unused.ScreenIndex = screen;

            unusedAsm.Add(unused);
        }

        public void Save(Stream s) {
            bool extendedFormat = false;

            for (int i = 0; i < magicNumber.Length; i++) {
                s.WriteByte(magicNumber[i]);
            }

            var w = new BinaryWriter(s);
            SaveSpecialFile(w, generalCodeFilename, GeneralCodeFile, false);
            SaveSpecialFile(w, expansionCodeFilename, ExpansionFile, false);
            SaveSpecialFile(w, declaresFilename, DefinesFile, false);
            if (!string.IsNullOrEmpty(ChrAnimationNameList)) {
                SaveSpecialFile(w, ChrAnimationNamesFilename, ChrAnimationNameList, false);
            }

            for (int iLevel = 0; iLevel < rom.Levels.Count; iLevel++) {
                for (int iScreen = 0; iScreen < rom.Levels[(LevelIndex)iLevel].Screens.Count; iScreen++) {
                    var asm = rom.Levels[(LevelIndex)iLevel].Screens[iScreen].ScreenLoadASM;
                    if (!string.IsNullOrEmpty(asm)) {
                        w.Write((byte)EntryTypeCodes.screenLoadAsm);
                        w.Write((byte)iLevel);
                        w.Write((byte)iScreen);
                        w.Write(asm);
                    }
                }
            }


            WriteFlag(w, FlagNames.Debug, Debug);
            WriteFlag(w, FlagNames.BuildOnTestRoom, BuildOnTestRoom);
            if (Files.Count > 0) {
                WriteFlag(w, FlagNames.ExtendedFormat, true);
            }

            w.Write((byte)EntryTypeCodes.eof);



            if (Files.Count > 0) {
                extendedFormat = true;

                // Fucking idiot. I already do this five lines up.
                ////// Begin extended section
                ////WriteFlag(w, FlagNames.ExtendedFormat, true);
                ////w.Write((byte)EntryTypeCodes.eof);

                foreach (var file in Files) {
                    string code = file.Code;
                    if (file.AutoBuild) code = autoBuildToken + Environment.NewLine + code;
                    SaveUserFile(w, file.Name, code, true);
                }

                // All entries in extended section must be prepended with 32-bit length specifier, even EOF
                w.Write((int)0);
                w.Write((byte)EntryTypeCodes.eof);
            }

            w.Flush();
        }

        private void WriteFlag(BinaryWriter w, string flagName, bool value) {
            w.Write((byte)EntryTypeCodes.flag);
            w.Write(flagName);
            w.Write(value);
        }


        private void SaveSpecialFile(BinaryWriter w, string filename, string fileText, bool extended) {
            SaveAsmFile(w, filename, fileText, EntryTypeCodes.specialFile, extended);
        }

        private void SaveUserFile(BinaryWriter w, string filename, string fileText, bool extended) {
            SaveAsmFile(w, filename, fileText, EntryTypeCodes.userFile, extended);
        }

        private static void SaveAsmFile(BinaryWriter w, string filename, string fileText, EntryTypeCodes type, bool extended) {
            if (extended) {
                // Need to calculate size of data
                var stream = new MemoryStream();
                using (BinaryWriter tempBuffer = new BinaryWriter(stream)) {
                    tempBuffer.Write(filename);
                    tempBuffer.Write(fileText ?? string.Empty);

                    tempBuffer.Flush();
                    var bytes = stream.GetBuffer();
                    var len = (int)stream.Length;
                    w.Write(len);
                    w.Write((byte)type);
                    w.Write(bytes, 0, (int)len);
                }

            } else {
                w.Write((byte)type);
                w.Write(filename);
                w.Write(fileText ?? string.Empty);
            }
        }
        #endregion


        // "EditroidProject"
        static readonly byte[] magicNumber = { 0x45, 0x64, 0x69, 0x74, 0x72, 0x6f, 0x69, 0x64, 0x50, 0x72, 0x6f, 0x6a, 0x65, 0x63, 0x74 };
        public IList<UnusedScreenLoadASM> UnusedScreenLoadAsm { get { return unusedAsm; } }
        private enum EntryTypeCodes:byte
        {
            screenLoadAsm = 0x01,
            specialFile = 0x02,
            userFile = 0x03,
            depracated_debugFlag = 0x04,
            flag = 0x05,
            eof = 0xff
        }
        private static class FlagNames
        {
            public const string Debug = "debug";
            public const string deprecated_BuildOnSaveAndPlay = "buildOnSaveAndPlay";
            public const string BuildOnTestRoom = "buildOnTestRoom";
            /// <summary>Denotes that the file contains extra data that the Editroid 3.6 format can not accomodate, found in a second section following the EOF marker (entry type FF), which</summary>
            public const string ExtendedFormat = "extendedFormat";
        }

        public const string autoBuildToken = ";#atuobuild";
        public const string generalCodeFilename = "general";
        public const string expansionCodeFilename = "expansion";
        public const string declaresFilename = "projectdefines";
        public const string ChrAnimationNamesFilename = "chrAnimations";

        public static readonly IList<string> ReservedFilenames = new List<string>(new string[] { generalCodeFilename, expansionCodeFilename, declaresFilename }).AsReadOnly();
        
    }

    class AsmFile
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool AutoBuild { get; set; }
    }

    public struct UnusedScreenLoadASM
    {
        public LevelIndex Level { get; set; }
        public int ScreenIndex { get; set; }
        public string ASM { get; set; }
    }

    [global::System.Serializable]
    public class ProjectLoadException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ProjectLoadException() { }
        public ProjectLoadException(string message) : base(message) { }
        public ProjectLoadException(string message, Exception inner) : base(message, inner) { }
        protected ProjectLoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
