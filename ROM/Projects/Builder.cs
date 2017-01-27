using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace Editroid.ROM.Projects
{
    internal partial class Project
    {
        internal class Builder
        {

            public const string UserFilePrefix = FileProvider.UserFilePrefix;

            Project project;
            MetroidRom sourceRom;
            MetroidRom outputRom;
            byte[] source;

            List<BuildError> _Errors = new List<BuildError>();
            /// <summary>
            /// Returns a list of build errors.
            /// </summary>
            public IList<BuildError> Errors { get; private set; }
            /// <summary>
            /// Returns true if there were fatal build errors.
            /// </summary>
            public bool FatalError { get; private set; }
            /// <summary>True if a build has been attempted (regardless of success or failure)</summary>
            bool _built = false;

            public string OutputFilename { get; private set; }

            int generalCodeFileBase = 0x8000;
            int definesFileBase = 0x8000;

            int _generalCodeAssembledSize = 0;

            static string newline = Environment.NewLine;

            ScreenList screenList;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="rom"></param>
            /// <param name="project"></param>
            /// <param name="outputFileName">Not used directly. Used for the generation of additional files whose names must be related (e.g. debug files). Should be file name only (i.e. NOT full path)</param>
            public Builder(MetroidRom rom, Project project, string outputFileName) {
                this.sourceRom = rom;
                this.project = project;
                this.OutputFilename = outputFileName;

                this.Errors = this._Errors.AsReadOnly();
            }

            /// <summary>
            /// Returns a new ROM image containing the ROM resulting from the build process, or null if there are fatal errors.
            /// </summary>
            /// <returns></returns>
            public MetroidRom BuildRom() {
                if (_built) throw new InvalidOperationException("Can not call BuildRom more than once.");
                _built = true;

                sourceRom.SerializeAllData();

                this.source = sourceRom.data;
                byte[] output = new byte[source.Length];

                Array.Copy(source, output, source.Length);
                this.outputRom = new MetroidRom(new MemoryStream(output));
                this.screenList = new ScreenList(outputRom);

                CopyScreenASM();
                Assemble();

                if (FatalError) return null;
                return outputRom;
            }

            private void CopyScreenASM() {
                for (int iLevel = 0; iLevel < sourceRom.Levels.Count; iLevel++) {
                    var srcLevel = sourceRom.Levels[(LevelIndex)iLevel];
                    var outputLevel = outputRom.Levels[(LevelIndex)iLevel];

                    for (int iScreen = 0; iScreen < srcLevel.Screens.Count; iScreen++) {
                        outputLevel.Screens[iScreen].ScreenLoadASM = srcLevel.Screens[iScreen].ScreenLoadASM;
                    }
                }
            }

            private void Assemble() {
                ClearScreenExtraBytes();

                // Assemble (first pass (1)) to calculate size of each screen's code
                {
                    string buildFile = ConstructBuildFile(1);
                    var buildFileAsBin = System.Text.Encoding.UTF8.GetBytes(buildFile);
                    snarfblasm.Assembler assembler = new snarfblasm.Assembler("Main File", new MemoryStream(buildFileAsBin), new FileProvider(this));
                    assembler.AllowInvalidOpcodes = false;
                    assembler.OverflowChecking = snarfblasm.OverflowChecking.None;
                    assembler.RequireColonOnLabels = true;
                    assembler.RequireDotOnDirectives = true;

                    var assembledBytes = assembler.Assemble();

                    ProcessAssembleErrors(assembler.GetErrors(), 1);
                    if (FatalError) return;

                    // Added first-pass-code to screens
                    var patchSegments = assembler.GetPatchSegments();
                    ApplyPatches(assembledBytes, patchSegments, false);
                    if (FatalError) return;

                    // Serializing data causes ROM data layout to be updated, which is then used to set the base address for each screen's code
                    CheckForOverrun();
                    if (FatalError) return;
                    outputRom.SerializeAllData();
                    ApplyScreenCodeBases();
                }

                // Assemble (second pass) with correct base addresses
                {
                    string buildFile = ConstructBuildFile(0); // Reconstruct with correct base addresses
                    byte[] buildFileAsBin = System.Text.Encoding.UTF8.GetBytes(buildFile);
                    var assembler = new snarfblasm.Assembler("Main File", new MemoryStream(buildFileAsBin), new FileProvider(this));
                    assembler.Labels = new snarfblasm.AddressLabels();
                    assembler.AllowInvalidOpcodes = false;
                    assembler.OverflowChecking = snarfblasm.OverflowChecking.Unsigned;
                    assembler.RequireColonOnLabels = true;
                    assembler.RequireDotOnDirectives = true;
                    var assembledBytes = assembler.Assemble();

                    ProcessAssembleErrors(assembler.GetErrors(), 0);
                    if (FatalError) return;

                    if (project.GenerateDebugFiles && !string.IsNullOrEmpty(project.DebugOutputDirectory.Trim())) {
                        BuildDebugFiles(assembler.Labels);
                    }

                    // Re-apply updated patch
                    var patchSegments = assembler.GetPatchSegments();
                    ApplyPatches(assembledBytes, patchSegments, true);
                    if (FatalError) return;

                    // This causes updated asm bytes to be written to ROM
                    outputRom.SerializeAllData();
                    CheckForOverrun();
                    if (FatalError) return;
                }
            }

            private void BuildDebugFiles(Romulus.Plugin.IAddressLabels labels) {
                BuildNlFile((snarfblasm.BankLabels)labels.Ram);
                var banks = ((snarfblasm.BankLabelList)labels.Banks).GetBanks();
                for (int i = 0; i < banks.Count; i++) {
                    BuildNlFile(banks[i]);
                }
            }

            /// <summary>
            /// Creates .NL files for the FCEUX debugger
            /// </summary>
            /// <param name="bankLabels"></param>
            private void BuildNlFile(snarfblasm.BankLabels bankLabels) {
                if (bankLabels == null) throw new ArgumentNullException("bankLabels");

                StringWriter output = new StringWriter();

                var labels = bankLabels.GetLabels();
                foreach (var entry in labels) {
                    var entryData = entry.Value;

                    string countString = (entryData.size > 0) ? ("/" + entryData.size.ToString("X")) : (string.Empty);
                    string nlEntry = "$" + entry.Key.ToString("X4") + countString + "#" + entryData.label + "#" + entryData.comment;

                    output.WriteLine(nlEntry);
                }

                string outputString = output.ToString();
                if (!string.IsNullOrEmpty(outputString)) {
                    string outputPath = Path.Combine(project.DebugOutputDirectory, OutputFilename);

                    string filename = outputPath;
                    if (bankLabels.BankIndex < 0)
                        filename += ".ram.nl";
                    else
                        filename += "." + bankLabels.BankIndex.ToString("X") + ".nl"; // Todo: are we sure these are hex?

                    
                    System.IO.File.WriteAllText(filename, outputString, Encoding.ASCII);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pass">The index of the pass. This value should be
            /// between 255 and 0, and should be lower on each successive pass,
            /// with a value of 0 on the final pass. A value of null will result
            /// in no "PASS" symbol being defined.</param>
            /// <returns></returns>
            private string ConstructBuildFile(int? pass) {
                StringBuilder result = new StringBuilder();

                if (pass != null) {
                    result.AppendLine("PASS = $" + pass.Value.ToString("X2"));
                }

                if (project.Debug)
                    result.AppendLine(".define DEBUG");

                // Defines file
                result.Append(".include ");
                result.AppendLine(declaresFilename);

                // Chr animation definitions
                if (project.rom.Format.HasChrAnimationTable) {
                    AddChrAnimationDefinitions(result);
                }

                // Expansion file
                result.Append(".include ");
                result.AppendLine(expansionCodeFilename);

                // General code file
                result.Append(".patch $");
                result.AppendLine(GeneralPatchID.ToString("x"));
                result.Append(".base $");
                result.AppendLine(generalCodeFileBase.ToString("x4"));
                result.Append(".include ");
                result.AppendLine(generalCodeFilename);

                ConstructFileIncludes(result);

                ConstructScreenIncludes(result);

                return result.ToString();
            }

            private void AddChrAnimationDefinitions(StringBuilder result) {
                foreach (var lvl in project.rom.ChrAnimationData.Values) {
                    foreach (var anim in lvl.Animations) {
                        if (isValidAnimationName(anim.Name)) {
                            result.AppendLine((anim.Name + " = $" + anim.SerializedFrameIndex.ToString("X2")).PadRight(20, ' ') + "; chr animation definition");
                        }
                    }
                }
            }

            private bool isValidAnimationName(string name) {
                if (string.IsNullOrEmpty(name)) return false;
                
                bool validFirst = char.IsLetter(name[0]) || name[0] == '_';
                if (!validFirst) return false;

                for (int i = 1; i < name.Length; i++) {
                    if (!char.IsLetterOrDigit(name[i]) && name[i] != '_') return false;
                }

                return true;
            }

            private void ConstructFileIncludes(StringBuilder result) {
                for (int i = 0; i < project.Files.Count; i++) {
                    var file = project.Files[i];
                    if (file.AutoBuild) {
                        result.AppendLine(".include " + FileProvider.UserFilePrefix + file.Name);
                    }
                }
            }

            private void ConstructScreenIncludes(StringBuilder output) {
                for (int iLevel = 0; iLevel < outputRom.Levels.Count; iLevel++) {
                    var level = outputRom.Levels[(LevelIndex)iLevel];
                    for (int iScreen = 0; iScreen < level.Screens.Count; iScreen++) {
                        var screen = level.Screens[iScreen];
                        var info = screenList[iLevel, iScreen];

                        if (!string.IsNullOrEmpty(screen.ScreenLoadASM)) {
                            // example
                            // .PATCH $100021
                            // .BASE $84CF
                            output.Append(".PATCH $");
                            output.AppendLine(info.PatchID.ToString("x"));
                            output.Append(".BASE $");
                            output.AppendLine(info.BaseAddress.ToString("x4"));

                            // example: for level 2 screen 1f
                            // .include l2s1f 
                            output.Append(".include l");
                            output.Append(iLevel);
                            output.Append('s');
                            output.Append(iScreen.ToString("x2"));
                            output.AppendLine();
                        }
                    }
                }
            }

            /// <summary>
            /// Verifies that screen-load code combined with general code does not exceed the free space in the screen data bank for each level.
            /// </summary>
            private void CheckForOverrun() {
                for (int i = 0; i < outputRom.Levels.Count; i++) {
                    var l = outputRom.Levels[(LevelIndex)i];

                    // Screen-load ASM subtracts from FreeScreenData, but general code does NOT
                    int remainingFreeSpace = l.Format.FreeGeneralAsmSpace - _generalCodeAssembledSize;
                    if(remainingFreeSpace < 0) {
                        FatalError = true;
                        _Errors.Add(new BuildError(string.Empty, 0 , ((LevelIndex)i).ToString() + " screen-load ASM exceeded available space in the screen data bank."));
                    }

                }
            }

            private void ProcessAssembleErrors(IList<snarfblasm.ErrorDetail> errors, int passIndex) {
                for (int i = 0; i < errors.Count; i++) {
                    var error = errors[i];
                    BuildError e = new BuildError(error.File, error.LineNumber, error.Message);
                    e.ErrorCode = (int)error.Code;
                    e.ErrorName = error.Code.ToString();

                    _Errors.Add(e);

                    FatalError = true;
                }
            }



            /// <summary>
            /// 
            /// </summary>
            /// <param name="assembledBytes"></param>
            /// <param name="patchSegments"></param>
            /// <param name="writeToImage">If true, all assembled code will be written onto the ROM image. Otherwise, ROM objects such as screen-load code will still be updated, but miscelaneous code will not be applied.</param>
            private void ApplyPatches(byte[] assembledBytes, IList<Romulus.PatchSegment> patchSegments, bool writeToImage) {
                for (int i = 0; i < patchSegments.Count; i++) {
                    var seg = patchSegments[i];
                    if (seg.PatchOffset == GeneralPatchID) {
                        if (writeToImage) {
                            ApplyGeneralPatch(assembledBytes, seg);
                        } else {
                            generalCodeFileBase = (int)this.sourceRom.Format.GeneralAsmInsertionAddress - seg.Length;
                        }
                        _generalCodeAssembledSize = seg.Length;
                    }  else if (ScreenList.IsScreenPatch(seg)) {
                        var screenEntry = screenList.GetAssociatedItem(seg);

                        var extraBytes = ((IProjectBuildScreen)screenEntry.Screen);
                        extraBytes.ExtraBytes = assembledBytes;
                        extraBytes.ExtraBytesStart = seg.Start;
                        extraBytes.ExtraBytesLength = seg.Length;
                    } else {
                        // Main file and misc patches
                        if (writeToImage) {
                            if (seg.PatchOffset < 0 || seg.PatchOffset + seg.Length > outputRom.data.Length) {
                                _Errors.Add(new BuildError("", 0, "Patch at offset $" + seg.PatchOffset.ToString("X") + " exceeded the ROM bounds."));
                                FatalError = true;
                                return;
                            }
                            Array.Copy(assembledBytes, seg.Start, outputRom.data, seg.PatchOffset, seg.Length);
                        }
                    }
                }
            }

            private void ApplyGeneralPatch(byte[] assembledBytes, Romulus.PatchSegment seg) {
                for (int i = 0; i < outputRom.Levels.Count; i++) {
                    var level = outputRom.Levels[(LevelIndex)i];

                    //var screenDataBank = level.Screens.DataBank;
                    int startOfBank, bankSize;
                    level.Format.GetGeneralAsmInsertionBank(out startOfBank, out bankSize);
                    int endOfBank = startOfBank +  bankSize;
                    int startOfGeneralPatch = endOfBank - seg.Length;

                    Array.Copy(assembledBytes, seg.Start, outputRom.data, startOfGeneralPatch, seg.Length);
                }
            }

            
            private void ClearScreenExtraBytes() {
                byte[] rts = new Byte[] { 0x60 };
                for (int iLevel = 0; iLevel < outputRom.Levels.Count; iLevel++) {
                    int screenCount = screenList.ScreensInLevel(iLevel);
                    for (int iScreen = 0; iScreen < screenCount; iScreen++) {
                        var extraBytes = (IProjectBuildScreen)screenList[iLevel, iScreen].Screen;

                        extraBytes.ExtraBytes = rts;
                        extraBytes.ExtraBytesStart = 0;
                        extraBytes.ExtraBytesLength = 1;
                    }
                }
            }


            /// <summary>
            /// For each screen in the ROM with ASM, the code's base address is updated to reflect the actual location of the code.
            /// </summary>
            private void ApplyScreenCodeBases() {
                for (int iLevel = 0; iLevel < outputRom.Levels.Count; iLevel++) {
                    int screenCount = screenList.ScreensInLevel(iLevel);
                    for (int iScreen = 0; iScreen < screenCount; iScreen++) {
                        var entry = screenList[iLevel, iScreen];

                        // Offset within 0x4000-byte bank (account for header--0x10)
                        int bankOffset = (entry.Screen.Offset - 0x10) & 0x3FFF;

                        // ASM is at end of screen data
                        bankOffset += (entry.Screen.Size - ((IProjectBuildScreen)entry.Screen).ExtraBytesLength);

                        if (project.rom.RomFormat == RomFormats.MMC3) {
                            // Data is banked at $A000
                            entry.BaseAddress = bankOffset | 0xA000;
                        } else {
                            // Data is banked at $8000
                            entry.BaseAddress = bankOffset | 0x8000;
                        }


                        screenList[iLevel, iScreen] = entry;
                    }
                }
            }



            private string CreateScreenIncludes() {
                StringBuilder builderino = new StringBuilder();

                for (int iLevel = 0; iLevel < outputRom.Levels.Count ; iLevel++) {
                    var level = outputRom.Levels[(LevelIndex)iLevel];
                    for (int iScreen = 0; iScreen < level.Screens.Count; iScreen++) {
                        var screen = level.Screens[iScreen];

                        if (!string.IsNullOrEmpty(screen.ScreenLoadASM)) {
                            // example: .include l2s1f for level 2 screen 1f
                            builderino.Append(".include l");
                            builderino.Append(iLevel);
                            builderino.Append('s');
                            builderino.Append(iScreen.ToString("x2"));
                            builderino.AppendLine();
                        }
                    }
                }

                return builderino.ToString();
            }


            // Screen-load patches start at 0x100000 and count up
            const int ScreenPatchID_0 = 0x100000;
            const int GeneralPatchID = ScreenPatchID_0 - 1;
            const string GeneralPatchName = generalCodeFilename; //"general code file";
            const int DeclaresPatchID = ScreenPatchID_0 - 2;
            const string DeclaresPatchName = declaresFilename; //"declares file";

            class ScreenList
            {
                const int LevelCount = 5;
                List<ScreenListItem>[] _screens;

                MetroidRom rom;

                public ScreenList(MetroidRom rom) {
                    this.rom=rom;

                    _screens = new List<ScreenListItem>[LevelCount];
                    int id = ScreenPatchID_0;
                    for (int i = 0; i < LevelCount; i++) {
                        _screens[i] = new List<ScreenListItem>();

                        var level = rom.Levels[(LevelIndex)i];
                        for (int iScreen = 0; iScreen < level.Screens.Count; iScreen++) {
                            _screens[i].Add(new ScreenListItem(id, level.Screens[iScreen]));
                            id++;
                        }
                    }
                }

                public static bool IsScreenPatch(Romulus.PatchSegment segment) { return segment.PatchOffset >= ScreenPatchID_0; }
                public void GetSegmentIndex(Romulus.PatchSegment segment, out int level, out int screen) {
                    int index = segment.PatchOffset - ScreenPatchID_0;
                    if (index < 0) throw new ArgumentException("Specified segment is not a screen segment");
                    
                    for (int iLevel = 0; iLevel < rom.Levels.Count; iLevel++) {
                        var levelData = rom.Levels[(LevelIndex)iLevel];

                        if (index >= levelData.Screens.Count ) {
                            // If screen is not in this level, keep looking
                            index -= levelData.Screens.Count;
                        }else{
                            level = iLevel;
                            screen = index;
                            return;
                        }
                    }

                    throw new ArgumentException("segment patch offset not a valid screen id");
                }
                public ScreenListItem GetAssociatedItem(Romulus.PatchSegment segment) {
                    int level, screen;
                    GetSegmentIndex(segment, out level, out screen);
                    return this[level, screen];
                }

                public ScreenListItem this[int level, int screen] {
                    get { return _screens[level][screen]; }
                    set {
                        _screens[level][screen] = value;
                    }
                }

                internal int ScreensInLevel(int iLevel) {
                    return _screens[iLevel].Count;
                }
            }
            struct ScreenListItem
            {
                public ScreenListItem(int id, Screen screen)
                    : this() {
                    this.PatchID = id;
                    this.Screen = screen;
                    BaseAddress = 0x8000;
                }
                public int BaseAddress { get; set; }
                public int PatchID { get; private set; }
                public Screen Screen { get; private set; }
            }

            internal static bool ParseScreenFilename(string filename, out int level, out int screen) {
                level = 0;
                screen = 0;

                if (filename.Length == 5 && filename[0] == 'l' && filename[2] == 's') {
                    if (char.IsDigit(filename[1])) {
                        level = (int)(filename[1] - '0');
                        
                        if (int.TryParse(filename.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out screen)) {
                           return true;
                        }

                    }
                }
                return false;
            }
            private class FileProvider : snarfblasm.IFileSystem
            {
                Builder b;
                internal FileProvider(Builder b) {
                    this.b = b;
                }

                public const string UserFilePrefix = "__userfile_";

                #region IFileSystem Members

                public string GetFileText(string filename) {
                    // core files
                    if (filename.Equals(GeneralPatchName, StringComparison.InvariantCultureIgnoreCase)) {
                        return b.project.GeneralCodeFile;
                    }
                    if (filename.Equals(declaresFilename, StringComparison.InvariantCultureIgnoreCase)) {
                        return b.project.DefinesFile;
                    }
                    if (filename.Equals(expansionCodeFilename, StringComparison.InvariantCultureIgnoreCase))
                        return b.project.ExpansionFile;

                    // autobuild files
                    var userfilename = TryGetUserFilename(filename);
                    if (userfilename != null) {
                        return b.project.GetFileByName(userfilename).Code;
                    }

                    // Screen load routines
                    int level, screen;
                    if (ParseScreenFilename(filename, out level, out screen)) {
                        if (level < b.outputRom.Levels.Count && screen < b.outputRom.Levels[(LevelIndex)level].Screens.Count) {
                            string patchText = b.outputRom.Levels[(LevelIndex)level].Screens[screen].ScreenLoadASM ?? string.Empty;
                            
                            return patchText;
                        }
                    }

                    // User .includes
                    var userFile = b.project.TryGetFileByName(filename);
                    if (userFile != null) {
                        return userFile.Code;
                    }

                    throw new FileNotFoundException("File " + filename + " not found");
                }

                public void WriteFile(string filename, byte[] data) {
                    throw new NotImplementedException();
                }

                public long GetFileSize(string filename) {
                    throw new NotImplementedException();
                }

                public Stream GetFileReadStream(string filename) {
                    throw new NotImplementedException();
                }

                public bool FileExists(string filename) {
                    if (filename.Equals(generalCodeFilename)) return true;
                    if (filename.Equals(declaresFilename)) return true;
                    if (filename.Equals(expansionCodeFilename)) return true;
                    
                    int level, screen;
                    if (ParseScreenFilename(filename, out level, out screen)) {
                        if (level < b.outputRom.Levels.Count && screen < b.outputRom.Levels[(LevelIndex)level].Screens.Count) {
                            return !string.IsNullOrEmpty(b.screenList[level, screen].Screen.ScreenLoadASM);
                        }
                    }
                    if (TryGetUserFilename(filename) != null) return true;
                    if (b.project.TryGetFileByName(filename) != null) return true; 

                    return false;
                }

                public static string TryGetUserFilename(string fullFilename) {
                    if (fullFilename.StartsWith(UserFilePrefix)) {
                        return fullFilename.Substring(UserFilePrefix.Length);
                    }
                    return null;
                }

                #endregion
            }
        }

        internal AsmFile GetFileByName(string userfilename) {
            var result = TryGetFileByName(userfilename);
            if(result == null)
                throw new ArgumentException("Specified file not found.");
            return result;
        }

        internal AsmFile TryGetFileByName(string userfilename) {
            for (int i = 0; i < Files.Count; i++) {
                if (string.Equals(userfilename, Files[i].Name, StringComparison.OrdinalIgnoreCase))
                    return Files[i];
            }
            return null;
        }
    }

    [Serializable()]
    public class ProjectBuildException : Exception
    {
        public ProjectBuildException() {
        }

        public ProjectBuildException(string message)
            : base(message) {
        }
        public ProjectBuildException(string message, Exception innerException) :
            base(message, innerException) {
        }
        protected ProjectBuildException(SerializationInfo info,
           StreamingContext context)
            : base(info, context) {
        }
    }

    class BuildError
    {
        public BuildError() { }
        public BuildError(string file, int line, string message) {
            this.File = file;
            this.Line = line;
            this.Message = message;
        }
        public string File { get; set; }
        public int Line { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorName { get; set; }
        public string Message { get; set; }
        public BuildErrorType ErrorType { get; set; }

    }
    public enum BuildErrorType
    {
        Error,
        Warning
    }
}
