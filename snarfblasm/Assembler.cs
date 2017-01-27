using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Romulus;
using System.IO;

namespace snarfblasm
{
    public sealed class Assembler
    {
        public Assembler(string asmName, string asm, IFileSystem fileLoader) {
            originalSource = asm;
            this.fileLoader = fileLoader;

            // Note that order of instantiation matters here, as some object depend on others in slightly stupid ways that can be fixed in needed to avoid circular issues with instantiation
            parser = new Parser(this);
            LineManager = new LineManager(asmName);
            assembly = new AssemblyData(this);
            evaluator = new ExpressionEvaluator(Values);
        }
        public Assembler(string asmName, Stream asmFile, IFileSystem fileLoader) {
            // Read file (as UTF-8) to string
            StreamReader reader = new StreamReader(asmFile,Encoding.UTF8, true);
            string asm = reader.ReadToEnd();

            originalSource = asm;
            this.fileLoader = fileLoader;

            // Note that order of instantiation matters here, as some object depend on others in slightly stupid ways that can be fixed in needed to avoid circular issues with instantiation
            parser = new Parser(this);
            LineManager = new LineManager(asmName);
            assembly = new AssemblyData(this);
            evaluator = new ExpressionEvaluator(Values);
        }

        #region Public Properties

        // Assembling objects
        internal LineManager LineManager { get; private set; }
        internal ExpressionEvaluator Evaluator { get { return evaluator; } }
        internal Parser Parser { get { return parser; } }
        internal AssemblyData Assembly { get { return assembly; } }
        internal Pass CurrentPass { get; private set; }
        public bool HasPatchSegments { get; private set; }
        public int DefaultPatchOffset { get; private set; }

        public IFileSystem FileSystem { get { return fileLoader; } }
        public List<StringSection> SourceCode { get { return _sourceCode; } }
        public bool Assembled { get { return assembled; } }
        internal IValueNamespace Values { get { return assembly; } }
        /// <summary>If true, unsupported instructions and addressing modes will be allowed.</summary>
        public bool AllowInvalidOpcodes { get { return _AllowInvalidOpcodes; } set { Require_BeforeAssemble(); _AllowInvalidOpcodes = value; } }

        public OverflowChecking OverflowChecking {
            get { return OverflowChecking; }
            set {
                Require_BeforeAssemble();
                _OverflowChecking = value;
            }
        }
        /// <summary>If false, failure to preceed a directive with a dot (".") will result in an error. (Set to false for ASM6 compatability.)</summary>
        public bool RequireDotOnDirectives { get { return _RequireDotOnDirectives; } set { Require_BeforeAssemble(); _RequireDotOnDirectives = value; } }
        public bool RequireColonOnLabels { get; set; }


        #endregion

        #region Fields

        // Assembling objects 
        Parser parser;
        ExpressionEvaluator evaluator;
        AssemblyData assembly;

        IFileSystem fileLoader;
        private List<StringSection> _sourceCode = new List<StringSection>();
        private string originalSource;
        List<Error> Errors = new List<Error>();
        List<ErrorDetail> errorDetails = new List<ErrorDetail>();

        bool assembled = false;
        bool _AllowInvalidOpcodes = false;
        OverflowChecking _OverflowChecking = OverflowChecking.None;
        bool _RequireDotOnDirectives = true;


        #endregion

        #region PhaseComplete event
        public event EventHandler<PhaseEventArgs> PhaseComplete;
        public event EventHandler<PhaseEventArgs> PhaseStarted;
        private void OnPhaseComplete(string msg, AssemblerPhase phase) {
            PhaseComplete.Raise(this, new PhaseEventArgs(msg, phase));
        }
        private void OnPhaseStart(string msg, AssemblerPhase phase) {
            PhaseStarted.Raise(this, new PhaseEventArgs(msg, phase));
        }

        public class PhaseEventArgs : EventArgs
        {
            public PhaseEventArgs(string msg, AssemblerPhase phase) {
                this.Message = msg;
                this.Phase = phase;
            }
            public string Message { get; private set; }
            public AssemblerPhase Phase { get; private set; }
        }

        public enum AssemblerPhase
        {
            Preprocess,
            Parsing,
            FirstPass,
            FinalPass
        }
        #endregion


        public IList<ErrorDetail> GetErrors() {
            return errorDetails.AsReadOnly();
        }



        private void Require_BeforeAssemble() {
            if (assembled) throw new InvalidOperationException("Operation is not allowed after assembling.");
        }
        private void Require_AfterAssemble() {
            if (!assembled) throw new InvalidOperationException("Operation is not allowed before assembling.");
        }


        public byte[] Assemble() {
            if (Assembled) throw new InvalidOperationException("Can not assemble more than once.");

            OnPhaseStart("Preprocessing...", AssemblerPhase.Preprocess);
            PreProcessor pp = new PreProcessor(this);
            pp.Process(originalSource, _sourceCode, fileLoader);
            if (Errors.Count > 0) {
                CreateErrorListing();
                return null;
            } else {
                OnPhaseComplete("Done", AssemblerPhase.Preprocess);
            }

            OnPhaseStart("Parsing...", AssemblerPhase.Parsing);
            parser.ParseCode(SourceCode);
            if (Errors.Count > 0) {
                CreateErrorListing();
                return null;
            } else {
                OnPhaseComplete("Done.", AssemblerPhase.Parsing);
            }


            OnPhaseStart("First pass...", AssemblerPhase.FirstPass);
            PerformPass(new FirstPass(this));
            if (Errors.Count > 0) {
                CreateErrorListing();
                return null;
            } else {
                OnPhaseComplete("Done.", AssemblerPhase.FirstPass);
            }
            
            OnPhaseStart("Second pass...", AssemblerPhase.FinalPass);
            PerformPass(new SecondPass(this));
            if (Errors.Count > 0) {
                CreateErrorListing();
                return null;
            } else {
                OnPhaseComplete("Done.", AssemblerPhase.FinalPass);
            }

            assembled = true;
            return output;
        }


        private void CreateErrorListing() {
            for (int i = 0; i < Errors.Count; i++) {
                var error = Errors[i];
                int sourceLine;
                string sourceName;
                LineManager.GetSourceLocation(error.SourceLine,out sourceLine,out sourceName);

                var detail = new ErrorDetail(sourceLine, sourceName, error.Code, error.Message);
                errorDetails.Add(detail);
            }
        }

        private void PerformPass(Pass passObject) {
            // Start with commandline/user specified checking (can be changed via directives)
            Evaluator.OverflowChecking = (_OverflowChecking != OverflowChecking.None) && passObject.AllowOverflowErrors;
            evaluator.SignedMode = _OverflowChecking == OverflowChecking.Signed;
            
            CurrentPass = passObject;
            CurrentPass.RunPass();

            if (CurrentPass.EmitOutput) {
                if (output != null)
                    throw new InvalidOperationException("Multiple passes returned output."); // Todo: More specific exception type (such as an engine exception/assembler exception that indicates internal error)
                this.output = CurrentPass.GetOutput();
                this.patchSegments = CurrentPass.GetPatchSegments();
                this.HasPatchSegments |= CurrentPass.HasPatchDirective;

                if (HasPatchSegments || patchSegments.Count == 0 || (patchSegments.Count == 1 && patchSegments[0].PatchOffset < 1)) {
                    DefaultPatchOffset = -1;
                }else {
                    DefaultPatchOffset = patchSegments[0].PatchOffset;
                }
            }
            CurrentPass = null;
        }

        public IList<PatchSegment> GetPatchSegments() {
            Require_AfterAssemble();
            return patchSegments;
        }

        public void AddError(Error error) {
            Errors.Add(error);
        }

        byte[] output;
        IList<PatchSegment> patchSegments;



        public byte[] GetOutput() {
            // Todo: Turn this into a property
            return output;
        }


////#if ROMULUS
        /// <summary>
        /// Set to enable debug output.
        /// </summary>
        public Romulus.Plugin.IAddressLabels Labels { get; set; }
        public void AddDebugLabel(int bank, int address, string name) {
            if (Labels != null) {
                if (bank < 0)
                    Labels.Ram.AddLabel((ushort)address, name);
                else
                    Labels.Banks[bank].AddLabel((ushort)address, name,name);
            }
        }
////#else
////        public void AddDebugLabel(int bank, int address, string name) {
////            // Do nothing
////        }
////#endif
    }

    public interface IFileSystem{
        string GetFileText(string filename);
        void WriteFile(string filename, byte[] data);
        long GetFileSize(string filename);
        System.IO.Stream GetFileReadStream(string filename);
        bool FileExists(string filename);
    }

    public enum OverflowChecking
    {
        None,
        Unsigned,
        Signed
    }

  
}

namespace Romulus
{
    public struct PatchSegment
    {
        public PatchSegment(int start, int len, int patchOffset)
            : this() {
            this.Start = start;
            this.Length = len;
            this.PatchOffset = patchOffset;
        }
        /// <summary>Where the patch begins in the output stream.</summary>
        public int Start { get; private set; }
        /// <summary>The length of the patch, in bytes. -1 indicates this patch extends to the end of the code stream.</summary>
        public int Length { get; private set; }
        /// <summary>The offset the patch should be applied to. -1 indicates no offset was specified.</summary>
        public int PatchOffset { get; private set; }

    }
}
