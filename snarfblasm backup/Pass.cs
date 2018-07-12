using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Romulus;

namespace snarfblasm
{
    /// <summary>
    /// Acts as a base class for Pass classes.
    /// </summary>
    abstract class Pass
    {
        public Pass(Assembler assembler) {
            this.Assembler = assembler;
            ResetOrigin();
            Assembly = assembler.Assembly;

            AddDefaultPatch();

            Bank = -1;
            MostRecentNamedLabel = "!unnamed!";

            Assembler.Evaluator.MostRecentNamedLabelGetter = getMostRecentNamedLabel;
            //Values = new Dictionary<string, LiteralValue>(StringComparer.InvariantCultureIgnoreCase);
            ////Errors = errors.AsReadOnly();
        }

        /// <summary>
        /// Gets/sets the current bank, used for debug output.
        /// </summary>
        public int Bank { get; set; }


        /// <summary>
        /// Returns true of a .PATCH directive is encountered.
        /// </summary>
        public bool HasPatchDirective { get; private set; }

        protected string MostRecentNamedLabel { get; private set; }
        string getMostRecentNamedLabel() {
            return MostRecentNamedLabel;
        }

        // Todo: what happens when address is FFFF and data is written? (Should currentaddress verify, and maybe add an error?)

        //public Dictionary<string, LiteralValue> Values = new Dictionary<string, LiteralValue>(StringComparer.InvariantCultureIgnoreCase);
        public PassValuesCollection Values = new PassValuesCollection();

        public int CurrentAddress { get; set; }
        public int OriginOffset { get; set; }

        public bool OriginSet { get { return OriginOffset >= 0; } }

        // Todo: option to stop running the processing loop if a certain number of errors occur (probably with a default value, maybe 10)

        public void RunPass() {
            if (Assembly == null) throw new InvalidOperationException("Must specify assembly before calling PerformPass.");

            PrepNextPointers();

            BeforePerformPass();
            PerformPass();
            AfterPerformPass();
        }

        protected virtual void AfterPerformPass() { }
        protected virtual void BeforePerformPass() { }


        /// <summary>
        /// If true, this pass has an output stream that should be written with assembled data. This
        /// value should not change during the lifetime of a Pass object. See also SurpressOutput.
        /// </summary>
        public bool EmitOutput { get; protected set; }
        public bool ErrorOnUndefinedSymbol { get; protected set; }
        public bool AllowOverflowErrors { get; protected set; }

        private void PerformPass() {

            for (iCurrentInstruction = 0; iCurrentInstruction < Assembly.ParsedInstructions.Count; iCurrentInstruction++) {
                ProcessPendingLabelsAndDirectives();
                if (!InExcludedCode)
                    ProcessCurrentInstruction();
            }

            // Process any remaining directives (those that come after the last instruction)
            iCurrentInstruction = int.MaxValue - 1;
            ProcessPendingLabelsAndDirectives();

            // todo: ensure that there are no open enums or if blocks
            // todo: throw error and stop processing if current address exceeds $FFFF

            if (EmitOutput) {
                CalculateLastPatchSegmentSize();
            }
        }

        protected abstract void ProcessCurrentInstruction();

        public abstract byte[] GetOutput();

        /// <summary>
        /// If true, nothing should be written to the output stream (but behavior shold not otherwise change)
        /// </summary>
        public bool SurpressOutput { get; protected set; }
        // Todo: get rid of current output offset
        public int CurrentOutputOffset { get; set; }


        bool hasORGed = false;
        // Todo: it looks like we have a field that is redundant (hasORGed versus OriginSet)
        public void SetOrigin(int address) {
            if (hasORGed) {
                int paddingAmt = address - CurrentAddress;

                if (InPatchMode) {
                    // In PATCH mode, .ORG just begins a new patch segment
                    if (EmitOutput) {
                        CalculateLastPatchSegmentSize();
                        var lastPatch = PatchSegments[PatchSegments.Count - 1];


                        int newPatchOffset = lastPatch.PatchOffset + lastPatch.Length + (CurrentAddress - address);
                        this.PatchSegments.Add(new PatchSegment(lastPatch.Start + lastPatch.Length, -1, newPatchOffset));
                    }
                    SetAddress(address);

                } else {
                    // .ORG (except first) pads to the specified address.
                    if (paddingAmt < 0) {
                        throw new ArgumentException("Specified address is less than the current address.");
                    } else {
                        OutputBytePadding(paddingAmt, (byte)0);
                    }
                }
            } else {
                // First .ORG acts like a .BASE
                SetAddress(address);
                OriginOffset = address;
            }

            // Todo: OriginOffset is probably completely unnecessary

            OriginOffset = address;

            hasORGed = true;
        }
        public void SetAddress(int address) {
            CurrentAddress = address;
            CurrentOutputOffset = address;

            // Any following .ORGs should pad
            hasORGed = true;
        }

        #region ENUM handling
        bool isInEnum;
        bool enumCache_SurpressOutput;
        int enumCache_Address;
        int enumCache_OutputOffset;

        public ErrorCode BeginEnum(int address) {
            if (isInEnum) {
                return ErrorCode.Nested_Enum;
            }

            isInEnum = true;
            enumCache_Address = CurrentAddress;
            enumCache_OutputOffset = CurrentOutputOffset;
            enumCache_SurpressOutput = SurpressOutput;

            CurrentAddress = address;
            SurpressOutput = true;

            return ErrorCode.None;
        }

        public ErrorCode EndEnum() {
            if (!isInEnum) {
                return ErrorCode.Extraneous_End_Enum;
            }

            isInEnum = false;
            CurrentAddress = enumCache_Address;
            CurrentOutputOffset = enumCache_OutputOffset;
            SurpressOutput = enumCache_SurpressOutput;

            return ErrorCode.None;
        }
        #endregion

        #region IF,IFDEF handling
        struct ifInfo
        {
            /// <summary>Set to true when a block's condition has been met. Any subsequent blocks (ELSEIF or ELSE) should be ignored.</summary>
            public bool foundCase;
            /// <summary>True if the current block should be processed.</summary>
            public bool processCurrentBlock;
            /////// <summary>True if the block occurs in excluded code (this state needs to be restored when the block closes)</summary>
            ////public bool isExcluded;
        }


        Stack<ifInfo> ifStack = new Stack<ifInfo>();

        bool inIfStack { get { return ifStack.Count > 0; } }
        /// <summary>
        /// If true, the pass is currently passing over excluded code, such as code in an IF block where the condition is not met.
        /// </summary>
        public bool InExcludedCode { get { return ifStack.Count > 0 && !ifStack.Peek().processCurrentBlock; } }

        public ErrorCode If_EnterExcludedIf() {
            ifInfo blockInfo;
            // Prevent any code of the if statement from running by...
            blockInfo.foundCase = true; // ...setting foundCase - no other blocks will be considered
            blockInfo.processCurrentBlock = false; // ...clearing processCurrentBlock - this block wont be considered
            ////blockInfo.isExcluded = true; // ...set InExcludedCode=true when ENDIF is reached
            ifStack.Push(blockInfo);

            return ErrorCode.None;
        }
        public ErrorCode If_EnterIf(bool conditionMet) {
            ifInfo blockInfo;
            blockInfo.foundCase = conditionMet;
            blockInfo.processCurrentBlock = conditionMet;
            ////blockInfo.isExcluded = false;
            ifStack.Push(blockInfo);

            return ErrorCode.None;
        }

        /// <summary>
        /// Call this method only when the next block, ELSE or ELSEIF, is encountered, AND NO BLOCKS HAVE BEEN EXECUTED YET.    
        /// </summary>
        /// <param name="conditionMet"></param>
        /// <returns></returns>
        public ErrorCode If_EnterElse(bool conditionMet) {
            if (!inIfStack)
                return ErrorCode.Missing_Preceeding_If;

            var currentBlock = ifStack.Pop();
            currentBlock.foundCase |= conditionMet;
            currentBlock.processCurrentBlock = conditionMet;
            ifStack.Push(currentBlock);

            return ErrorCode.None;
        }

        public ErrorCode If_CloseIf() {
            if (!inIfStack)
                return ErrorCode.Missing_Preceeding_If;

            var ifBlock = ifStack.Pop();
            ////InExcludedCode = ifBlock.isExcluded;

            return ErrorCode.None;
        }

        internal void ProcessIfBlockDirective(ConditionalDirective directive, out Error error) {
            error = Error.None;
            bool? ifCondition;

            switch (directive.part) {
                case ConditionalDirective.ConditionalPart.IF:
                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
                    if (InExcludedCode) {
                        If_EnterExcludedIf();
                    } else {
                        ifCondition = If_EvalCondition(directive, out error);
                        if (error.IsError) return;
                        if (ifCondition == null) {
                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
                            return;
                        }

                        If_EnterIf((bool)ifCondition);
                    }
                    break;
                case ConditionalDirective.ConditionalPart.ELSEIF:
                    // We can treat else/else if like it is not in excluded code (if it IS in excluded code, it will look as if 
                    // we already had a block executed, so the else/elseif won't be processed.)
                    if (!inIfStack) {
                        error = new Error(ErrorCode.Missing_Preceeding_If, Error.Msg_NoPreceedingIf, directive.SourceLine);
                        return;
                    }

                    bool considerElseIf = !ifStack.Peek().foundCase; // Has a previous block been executed? (We only process an else if it hasn't)
                    if (considerElseIf) {
                        ifCondition = If_EvalCondition(directive, out error);
                        if (error.IsError) return;
                        if (ifCondition == null) {
                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
                            return;
                        }

                        If_EnterElse((bool)ifCondition);
                    }
                    break;
                case ConditionalDirective.ConditionalPart.ELSE:
                    // We can treat else/else if like it is not in excluded code (if it IS in excluded code, it will look as if 
                    // we already had a block executed, so the else/elseif won't be processed.)
                    if (!inIfStack) {
                        error = new Error(ErrorCode.Missing_Preceeding_If, Error.Msg_NoPreceedingIf, directive.SourceLine);
                        return;
                    }

                    bool runElse = !ifStack.Peek().foundCase; // Has a previous block been executed? (We only process an else if it hasn't)
                    if (!directive.condition.IsNullOrEmpty) {
                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_NoTextExpected, directive.SourceLine);
                        return;
                    }

                    If_EnterElse(runElse);
                    break;
                case ConditionalDirective.ConditionalPart.IFDEF:
                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
                    if (InExcludedCode) {
                        If_EnterExcludedIf();
                    } else {
                        ifCondition = If_SymbolExists(directive);
                        if (ifCondition == null) {
                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
                            return;
                        }

                        If_EnterIf((bool)ifCondition);
                    }
                    break;
                case ConditionalDirective.ConditionalPart.IFNDEF:
                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
                    if (InExcludedCode) {
                        If_EnterExcludedIf();
                    } else {
                        ifCondition = If_SymbolExists(directive);
                        if (ifCondition == null) {
                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
                            return;
                        }

                        If_EnterIf(!(bool)ifCondition);
                    }
                    break;
                case ConditionalDirective.ConditionalPart.ENDIF:
                    If_CloseIf();
                    break;
                default:
                    error = new Error(ErrorCode.Engine_Error, "Unexpected IF block tiye in Pass.ProcessIfBlockDirective");
                    break;
            }
        }

        bool? If_EvalCondition(ConditionalDirective directive, out Error error) {
            error = Error.None;

            if (directive.condition.IsNullOrEmpty) return null;
            var condition = directive.condition;
            var result = Assembler.Evaluator.EvaluateExpression(ref condition, directive.SourceLine, out error);
            return (int)result.Value != 0;
        }
        bool? If_SymbolExists(ConditionalDirective directive) {
            if (directive.condition.IsNullOrEmpty) {
                return null;
            }

            return Values.NameExists(directive.condition.ToString());
        }

        #endregion

        /// <summary>
        /// Sets up pointers for the next label and next directive.
        /// </summary>
        private void PrepNextPointers() {
            if (Assembly.Labels.Count > 0) {
                iNextLabel = 0;
                NextLabel_iInstruction = Assembly.Labels[0].iInstruction;
            } else {
                Set_NoLabels();
            }
            if (Assembly.Directives.Count > 0) {
                iNextDirective = 0;
                NextDirective_iInstruction = Assembly.Directives[0].InstructionIndex;
            } else {
                Set_NoDirectives();
            }
        }

        public AssemblyData Assembly { get; private set; }
        public Assembler Assembler { get; private set; }


        protected void AddError(Error error) {
            Assembler.AddError(error);
        }

        protected void ProcessPendingLabelsAndDirectives() {
            bool doMoreThings = true; // Until we decide that there is nothing pending

            do {
                bool labelPending = NextLabel_iInstruction <= iCurrentInstruction;
                bool directivePending = NextDirective_iInstruction <= iCurrentInstruction;

                // If there is both a label and directive pending, we need to figure which came first
                if (labelPending && directivePending) {
                    bool labelComesFirst = Assembly.Labels[iNextLabel].SourceLine <= Assembly.Directives[iNextDirective].SourceLine;

                    if (labelComesFirst)
                        directivePending = false;
                    else
                        labelPending = false;
                }

                if (labelPending) {
                    if (!InExcludedCode) {
                        ProcessCurrentLabel();
                    }
                    NextLabel();
                } else if (directivePending) {
                    // If we are in excluded code, only conditional directives are processed.
                    if (!InExcludedCode || Assembly.Directives[iNextDirective] is ConditionalDirective)
                        ProcessCurrentDirective();
                    NextDirective();
                } else {
                    doMoreThings = false;
                }
            } while (doMoreThings);
        }

        protected virtual void ProcessCurrentLabel() {
            var label = Assembly.Labels[iNextLabel];
            bool isAnon = label.name[0] == '~';

            if (!label.local && !isAnon) {
                MostRecentNamedLabel = label.name;
            }
        }

        private void ProcessCurrentDirective() {
            Error error;
            Assembly.Directives[iNextDirective].Process(this, out error);
            if (error.Code != ErrorCode.None) {
                AddError(error);
            }

            if (Assembly.Directives[iNextDirective] is PatchDirective) {
                HasPatchDirective = true;
            }
        }

        public int iNextLabel;
        public int NextLabel_iInstruction;
        public int iNextDirective;
        public int NextDirective_iInstruction;

        public int iCurrentInstruction;

        void Set_NoLabels() {
            iNextLabel = -1;
            NextLabel_iInstruction = Int32.MaxValue;
        }
        void Set_NoDirectives() {
            iNextDirective = -1;
            NextDirective_iInstruction = Int32.MaxValue;

        }
        protected void NextLabel() {
            iNextLabel++;
            if (iNextLabel >= Assembly.Labels.Count) {
                Set_NoLabels();
            } else {
                NextLabel_iInstruction = Assembly.Labels[iNextLabel].iInstruction;
            }
        }
        protected void NextDirective() {
            iNextDirective++;
            if (iNextDirective >= Assembly.Directives.Count) {
                Set_NoDirectives();
            } else {
                NextDirective_iInstruction = Assembly.Directives[iNextDirective].InstructionIndex;
            }
        }
        // --------------------------------------------- <--This is a line.

        /// <summary>
        /// Writes a byte to the output stream. If there is no output stream
        /// or SurpressOutput is true, the call is ignored (no errors will occur).
        /// </summary>
        /// <param name="b"></param>
        public abstract void WriteByte(byte b);

        /// <summary>
        /// Writes padding to the output stream.  If there is no output stream
        /// or SurpressOutput is true, the call is ignored (no errors will occur).
        /// </summary>
        public abstract void OutputBytePadding(int paddingAmt, byte fillValue);
        /// <summary>
        /// Writes padding to the output stream.  If there is no output stream
        /// or SurpressOutput is true, the call is ignored (no errors will occur).
        /// </summary>
        public abstract void OutputWordPadding(int wordCount, ushort fillValue);

        /// <summary>
        /// Gets the stream that output is written to (if applicable). This stream should not be written to while SurpressOutput is true.
        /// </summary>
        public Stream OutputStream { get; protected set; }


        /// <summary>
        /// Resets the origin address (.ORG). For example, a .PATCH directive resets the origin. Any labels that follow a 
        /// .PATCH without a .ORG in between should cause an error since there is no origin.
        /// </summary>
        public void ResetOrigin() {
            OriginOffset = int.MinValue;
            hasORGed = false;
        }

        #region PATCH directive stuffs
        List<PatchSegment> PatchSegments = new List<PatchSegment>();

        public bool InPatchMode { get; private set; }

        /// <summary>
        /// Specifies the address that the following code will be patched to. See remarks.
        /// </summary>
        /// <remarks>Once a patch offset is applied, any following code will be in "patch mode." .ORG directives will
        /// not pad. Instead, a .ORG will begin a new patch section.</remarks>
        public void SetPatchOffset(int offset) {
            OnSetPatchOffset(offset);

            ResetOrigin();

            if (EmitOutput) {
                CalculateLastPatchSegmentSize();

                // Add a new patch segment
                PatchSegments.Add(new PatchSegment((int)OutputStream.Length, -1, offset));
            }
        }

        /// <summary>
        /// Computes length of the most recently added patch segment. SEE REMARKS
        /// </summary>
        /// <remarks>It is assumed that the last patch extends to the end of the code stream.</remarks>
        /// <returns></returns>
        private void CalculateLastPatchSegmentSize() {
            // 

            if (EmitOutput) {
                var lastPatch = PatchSegments[PatchSegments.Count - 1];
                int streamSize = (int)OutputStream.Length;
                int patchLen = streamSize - lastPatch.Start;
                PatchSegments[PatchSegments.Count - 1] = new PatchSegment(lastPatch.Start, patchLen, lastPatch.PatchOffset);
            } else {
                throw new InvalidOperationException("CalculateLastPatchSegmentSize() not valid on a pass that does not emit output.");
            }
        }

        /// <summary>
        /// Inheritors can call this method if necessary.
        /// </summary>
        /// <param name="offset"></param>
        protected virtual void OnSetPatchOffset(int offset) {
            InPatchMode = true;
        }

        private void AddDefaultPatch() {
            PatchSegments.Add(new PatchSegment(0, -1, -1));
        }

        public IList<PatchSegment> GetPatchSegments() {
            for (int i = PatchSegments.Count - 1; i >= 0; i--) {
                if (PatchSegments[i].Length == 0 && PatchSegments.Count > 1) {
                    PatchSegments.RemoveAt(i);
                }
            }

            return PatchSegments;
        }

        #endregion

        public class PassValuesCollection
        {
            Dictionary<string, PassValue> Values = new Dictionary<string, PassValue>(StringComparer.InvariantCultureIgnoreCase);

            public PassValuesCollection() {

            }

            public LiteralValue this[string name] {
                get {
                    return Values[name].value;
                }
            }

            public LiteralValue? TryGetValue(string name) {
                PassValue result;
                bool found = Values.TryGetValue(name, out result);
                return found ? (LiteralValue?)result.value : null;
            }

            public void SetValue(string name, LiteralValue value, bool isFixed, out bool valueIsFixedError) {
                PassValue existingValue;
                bool exists = Values.TryGetValue(name, out existingValue);

                if (exists) {
                    if (existingValue.isFixed) {
                        valueIsFixedError = true;
                        return;
                    } else {
                        Values.Remove(name);
                    }
                }

                Values.Add(name, new PassValue(value, isFixed));
                valueIsFixedError = false;
            }

            public bool RemoveValue(string name, out bool valueIsFixedError) {
                PassValue existingValue;
                bool exists = Values.TryGetValue(name, out existingValue);

                if (exists) {
                    if (existingValue.isFixed) {
                        valueIsFixedError = true;
                        return false;
                    } else {
                        Values.Remove(name);
                        valueIsFixedError = false;
                        return true;
                    }
                }

                valueIsFixedError = false;
                return false;
            }
            private struct PassValue
            {
                public PassValue(LiteralValue value, bool isFixed) {
                    this.value = value;
                    this.isFixed = isFixed;
                }
                public LiteralValue value;
                public bool isFixed;
            }

            internal bool NameExists(string name) {
                return Values.ContainsKey(name);
            }
        }
    }

    class FirstPass : Pass
    {
        public FirstPass(Assembler asm)
            : base(asm) {
            EmitOutput = false;
            AllowOverflowErrors = false;
            ErrorOnUndefinedSymbol = false;
        }

        protected override void BeforePerformPass() {
            base.BeforePerformPass();

            // Assume undefined symbols are unprocessed labels (if not, they will be caught on second pass)
            Assembler.Evaluator.ErrorOnUndefinedSymbols = false;
        }

        protected override void ProcessCurrentInstruction() {

            var currentLine = Assembly.ParsedInstructions[iCurrentInstruction];
            var opcode = Opcode.allOps[currentLine.opcode];

            bool SingleByteOperand = false;
            if (currentLine.operandExpression != null) {
                // If an AsmValue has not been resolved to a literal value yet, it is because it references a label, and thus must be 16-bit

                Error error;

                StringSection expression = currentLine.operandExpression;
                SingleByteOperand = Assembler.Evaluator.EvaluateExpression(ref expression, currentLine.sourceLine, out error).IsByte;
                if (error.Code != ErrorCode.None) {
                    AddError(new Error(error, currentLine.sourceLine));
                } else if (!expression.IsNullOrEmpty) {
                    AddError(new Error(ErrorCode.Invalid_Expression, Error.Msg_InvalidExpression, currentLine.sourceLine));
                }

            } else if (currentLine.operandValue.IsByte) {
                SingleByteOperand = true;
            }

            if (SingleByteOperand) {
                // Todo: Consider moving TryToConvertToZeroPage to more appropriate class
                if (Assembler.Parser.TryToConvertToZeroPage(ref currentLine)) {
                    // If the instruction can be coded as zero-page, update it
                    Assembly.ParsedInstructions[iCurrentInstruction] = currentLine; // Todo: consider method such as UpdateParsedInstruction
                }
            }

            var instructionLen = Opcode.GetParamBytes(currentLine.opcode) + 1;
            CurrentOutputOffset += instructionLen;
            CurrentAddress += instructionLen;
        }





        protected override void ProcessCurrentLabel() {
            base.ProcessCurrentLabel();

            var label = Assembly.Labels[iNextLabel];

            label.address = (ushort)CurrentAddress;
            Assembly.Labels[iNextLabel] = label;

            bool isAnonymous = label.name[0] == '~';
            if (isAnonymous) {
                Assembly.AnonymousLabels.ResolveNextPointer((ushort)CurrentAddress);
            } else {
                if (Values.NameExists(label.name)) {
                    AddError(new Error(ErrorCode.Value_Already_Defined, string.Format(Error.Msg_ValueAlreadyDefined_name, label.name), label.SourceLine));
                } else {
                    bool isFixedError; // Todo: handle this  (ummmm...don't know what this even is anymore)
                    if (label.address < 0x100) {
                        Values.SetValue(label.name, new LiteralValue((ushort)CurrentAddress, true), true, out isFixedError);
                    } else {
                        Values.SetValue(label.name, new LiteralValue((ushort)CurrentAddress, false), true, out isFixedError);
                    }
                }
            }
        }

        public override void WriteByte(byte b) {
        }



        public override byte[] GetOutput() {
            throw new NotImplementedException();
        }

        public override void OutputBytePadding(int paddingAmt, byte fillValue) {
            CurrentAddress += paddingAmt;
            CurrentOutputOffset += paddingAmt;
        }

        public override void OutputWordPadding(int wordCount, ushort fillValue) {
            CurrentAddress += wordCount * 2;
            CurrentOutputOffset += wordCount * 2;

        }



    }

    class SecondPass : Pass
    {
        public SecondPass(Assembler asm)
            : base(asm) {
            EmitOutput = true;
            AllowOverflowErrors = true;
            ErrorOnUndefinedSymbol = true;

            base.OutputStream = this.outputStream;
        }

        MemoryStream outputStream = new MemoryStream();
        byte[] output;
        public override byte[] GetOutput() {
            return output;
        }

        protected override void BeforePerformPass() {
            base.BeforePerformPass();

            LoadLabelValues();

            Assembler.Evaluator.ErrorOnUndefinedSymbols = true;
        }
        ////protected override void PerformPass() {

        ////    for (iCurrentInstruction = 0; iCurrentInstruction < Assembly.ParsedInstructions.Count; iCurrentInstruction++) {
        ////        ProcessPendingLabelsAndDirectives();

        ////        ProcessCurrentInstruction();
        ////    }

        ////    // Process any remaining directives (those that come after the last instruction)
        ////    iCurrentInstruction = int.MaxValue - 1;
        ////    ProcessPendingLabelsAndDirectives();


        ////}
        protected override void AfterPerformPass() {
            base.AfterPerformPass();


            output = outputStream.ToArray();
            outputStream = null;

        }


        protected override void ProcessCurrentInstruction() {
            Error error;

            var currentLine = Assembly.ParsedInstructions[iCurrentInstruction];

            if (!SurpressOutput)
                outputStream.WriteByte(currentLine.opcode);

            var addressing = Opcode.allOps[currentLine.opcode].addressing;
            switch (addressing) {
                case Opcode.addressing.implied:
                    // No operand
                    CurrentAddress++;
                    CurrentOutputOffset++;
                    break;
                case Opcode.addressing.absoluteIndexedX:
                case Opcode.addressing.absoluteIndexedY:
                case Opcode.addressing.absolute:
                case Opcode.addressing.indirect:
                    // 16-bit opeand
                    ushort operand16 = GetOperand(currentLine, out error);
                    if (error.Code == ErrorCode.None) {
                        if (!SurpressOutput) {
                            outputStream.WriteByte((byte)(operand16 & 0xFF));
                            outputStream.WriteByte((byte)(operand16 >> 8));
                        }

                        CurrentAddress += 3;
                        CurrentOutputOffset += 3;
                    } else {
                        AddError(error);
                    }
                    break;
                case Opcode.addressing.zeropageIndexedX:
                case Opcode.addressing.zeropageIndexedY:
                case Opcode.addressing.zeropage:
                case Opcode.addressing.indirectX:
                case Opcode.addressing.indirectY:
                case Opcode.addressing.immediate:

                    // 8-bit operand
                    ushort operand8 = GetOperand(currentLine, out error);
                    if (error.Code == ErrorCode.None) {
                        if (operand8 > 0xFF) {
                            if (IsZeroPage(addressing)) {
                                error = new Error(ErrorCode.Address_Out_Of_Range, Error.Msg_ZeroPageOutOfRange, currentLine.sourceLine);
                            } else {
                                error = new Error(ErrorCode.Overflow, Error.Msg_ValueOutOfRange, currentLine.sourceLine);
                            }
                            AddError(error);
                        }
                        if (!SurpressOutput)
                            outputStream.WriteByte((byte)(operand8 & 0xFF));

                        CurrentAddress += 2;
                        CurrentOutputOffset += 2;
                    } else {
                        AddError(error);
                    }
                    break;
                case Opcode.addressing.relative:
                    // Calculate relative offset
                    int operandAbs = GetOperand(currentLine, out error);
                    if (error.Code == ErrorCode.None) {
                        int relativeOrigin = CurrentAddress + 2;
                        int operandRel = operandAbs - relativeOrigin;
                        // Verify in range
                        if (operandRel < -128 || operandRel > 127) {
                            error = new Error(ErrorCode.Overflow, Error.Msg_BranchOutOfRange, currentLine.sourceLine);
                            AddError(error);
                        }
                        if (!SurpressOutput)
                            outputStream.WriteByte((byte)operandRel);

                        CurrentAddress += 2;
                        CurrentOutputOffset += 2;
                    } else {
                        AddError(error);
                    }
                    break;
            }
        }

        private bool IsZeroPage(Opcode.addressing addressing) {
            switch (addressing) {
                case Opcode.addressing.zeropage:
                case Opcode.addressing.zeropageIndexedX:
                case Opcode.addressing.zeropageIndexedY:
                    return true;
                case Opcode.addressing.implied:
                case Opcode.addressing.immediate:
                case Opcode.addressing.absolute:
                case Opcode.addressing.indirect:
                case Opcode.addressing.indirectX:
                case Opcode.addressing.indirectY:
                case Opcode.addressing.relative:
                case Opcode.addressing.absoluteIndexedX:
                case Opcode.addressing.absoluteIndexedY:
                default:
                    return false;
            }
        }

        private void LoadLabelValues() {
            bool isFixedError; // Todo: handle this
            foreach (var label in Assembly.Labels) {
                Values.SetValue(label.name, new LiteralValue((ushort)label.address, false), true, out isFixedError);
            }
        }

        private ushort GetOperand(ParsedInstruction currentLine, out Error error) {
            if (currentLine.operandExpression == null) {
                error = Error.None;
                return currentLine.operandValue.Value;
            }
            if (currentLine.operandExpression != null) {
                StringSection expression = currentLine.operandExpression;

                var result = Assembler.Evaluator.EvaluateExpression(ref expression, currentLine.sourceLine, out error).Value;

                if (error.Code != ErrorCode.None) {
                    error = new Error(error, currentLine.sourceLine);
                    return 0;
                } else if (!expression.IsNullOrEmpty) {
                    error = new Error(ErrorCode.Invalid_Expression, Error.Msg_InvalidExpression, currentLine.sourceLine);
                    return 0;
                }
                return result;
            }

            error = new Error(ErrorCode.Engine_Error, "Instruction required an operand, but none was present. Instruction may have been mis-parsed.", currentLine.sourceLine);
            return 0;
        }


        ////private static void ThrowOperandOutOfRangeException(int operandRel, int sourceLine) {
        ////    int outOfRangeAmt;
        ////    if (operandRel < 0)
        ////        outOfRangeAmt = Math.Abs(operandRel + 128);
        ////    else
        ////        outOfRangeAmt = Math.Abs(operandRel - 127);
        ////    throw new OperandOutOfRangeException("Operand is out of range (by " + outOfRangeAmt.ToString("X") + ")", sourceLine);
        ////}

        protected override void ProcessCurrentLabel() {
            base.ProcessCurrentLabel();

            // Nothing to do with labels for second pass.
            var label = Assembly.Labels[iNextLabel];

            bool isAnonymous = label.name != null && label.name.StartsWith("~");
            if (!isAnonymous) {

                if (label.address < 0x8000) {
                    // RAM
                    Assembler.AddDebugLabel(-1, CurrentAddress, label.name);
                }
                if (Bank >= 0) {
                    // ROM
                    Assembler.AddDebugLabel(Bank, CurrentAddress, label.name);
                }
            }
        }

        public override void WriteByte(byte b) {
            if (!SurpressOutput)
                outputStream.WriteByte(b);
        }

        public override void OutputBytePadding(int paddingAmt, byte fillValue) {
            if (!SurpressOutput) {
                for (int i = 0; i < paddingAmt; i++) {
                    outputStream.WriteByte(fillValue);
                }
            }

            CurrentAddress += paddingAmt;
            CurrentOutputOffset += paddingAmt;
        }

        public override void OutputWordPadding(int wordCount, ushort fillValue) {
            if (!SurpressOutput) {
                for (int i = 0; i < wordCount; i++) {
                    outputStream.WriteByte((byte)fillValue);
                    outputStream.WriteByte((byte)(fillValue >> 8));
                }
            }

            CurrentAddress += wordCount * 2;
            CurrentOutputOffset += wordCount * 2;
        }


    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Romulus;

//namespace snarfblasm
//{
//    /// <summary>
//    /// Acts as a base class for Pass classes.
//    /// </summary>
//    abstract class Pass
//    {
//        public Pass(Assembler assembler) {
//            this.Assembler = assembler;
//            ResetOrigin();
//            Assembly = assembler.Assembly;

//            AddDefaultPatch();

//            Bank = -1;
//            //Values = new Dictionary<string, LiteralValue>(StringComparer.InvariantCultureIgnoreCase);
//            ////Errors = errors.AsReadOnly();
//        }

//        /// <summary>
//        /// Gets/sets the current bank, used for debug output.
//        /// </summary>
//        public int Bank { get; set; }

//        // Todo: what happens when address is FFFF and data is written? (Should currentaddress verify, and maybe add an error?)

//        //public Dictionary<string, LiteralValue> Values = new Dictionary<string, LiteralValue>(StringComparer.InvariantCultureIgnoreCase);
//        public PassValuesCollection Values = new PassValuesCollection();

//        public int CurrentAddress { get; set; }
//        public int OriginOffset { get; set; }

//        public bool OriginSet { get { return OriginOffset >= 0; } }

//        // Todo: option to stop running the processing loop if a certain number of errors occur (probably with a default value, maybe 10)

//        public void RunPass() {
//            if (Assembly == null) throw new InvalidOperationException("Must specify assembly before calling PerformPass.");

//            PrepNextPointers();
            
//            BeforePerformPass();
//            PerformPass();
//            AfterPerformPass();
//        }

//        protected virtual void AfterPerformPass() { }
//        protected virtual void BeforePerformPass() { }


//        /// <summary>
//        /// If true, this pass has an output stream that should be written with assembled data. This
//        /// value should not change during the lifetime of a Pass object. See also SurpressOutput.
//        /// </summary>
//        public bool EmitOutput { get; protected set; }
//        public bool ErrorOnUndefinedSymbol { get; protected set; }
//        public bool AllowOverflowErrors { get; protected set; }

//        private void PerformPass() {

//            for (iCurrentInstruction = 0; iCurrentInstruction < Assembly.ParsedInstructions.Count; iCurrentInstruction++) {
//                ProcessPendingLabelsAndDirectives();
//                if (!InExcludedCode)
//                    ProcessCurrentInstruction();
//            }

//            // Process any remaining directives (those that come after the last instruction)
//            iCurrentInstruction = int.MaxValue - 1;
//            ProcessPendingLabelsAndDirectives();

//            // todo: ensure that there are no open enums or if blocks
//            // todo: throw error and stop processing if current address exceeds $FFFF

//            if (EmitOutput) {
//                CalculateLastPatchSegmentSize();
//            }
//        }

//        protected abstract void ProcessCurrentInstruction() ;

//        public abstract byte[] GetOutput();

//        /// <summary>
//        /// If true, nothing should be written to the output stream (but behavior shold not otherwise change)
//        /// </summary>
//        public bool SurpressOutput { get; protected set; }
//        // Todo: get rid of current output offset
//        public int CurrentOutputOffset { get; set; }


//        bool hasORGed = false;
//        // Todo: it looks like we have a field that is redundant (hasORGed versus OriginSet)
//        public void SetOrigin(int address) {
//            if (hasORGed) {
//                int paddingAmt = address - CurrentAddress;

//                if (InPatchMode) {
//                    // In PATCH mode, .ORG just begins a new patch segment
//                    if (EmitOutput) {
//                        CalculateLastPatchSegmentSize();
//                        var lastPatch = PatchSegments[PatchSegments.Count - 1];


//                        int newPatchOffset = lastPatch.PatchOffset + lastPatch.Length + (CurrentAddress - address);
//                        this.PatchSegments.Add(new PatchSegment(lastPatch.Start + lastPatch.Length, -1, newPatchOffset));
//                    }
//                    SetAddress(address);
                    
//                } else {
//                    // .ORG (except first) pads to the specified address.
//                    if (paddingAmt < 0) {
//                        throw new ArgumentException("Specified address is less than the current address.");
//                    } else {
//                        OutputBytePadding(paddingAmt, (byte)0);
//                    }
//                }
//            } else {
//                // First .ORG acts like a .BASE
//                SetAddress(address);
//                OriginOffset = address;
//            }

//            // Todo: OriginOffset is probably completely unnecessary

//            OriginOffset = address;

//            hasORGed = true;
//        }
//        public void SetAddress(int address) {
//            CurrentAddress = address;
//            CurrentOutputOffset = address;

//            // Any following .ORGs should pad
//            hasORGed = true;
//        }

//        #region ENUM handling
//        bool isInEnum;
//        bool enumCache_SurpressOutput;
//        int enumCache_Address;
//        int enumCache_OutputOffset;

//        public ErrorCode BeginEnum(int address) {
//            if (isInEnum) {
//                return ErrorCode.Nested_Enum;
//            }

//            isInEnum = true;
//            enumCache_Address = CurrentAddress;
//            enumCache_OutputOffset = CurrentOutputOffset;
//            enumCache_SurpressOutput = SurpressOutput;

//            CurrentAddress = address;
//            SurpressOutput = true;

//            return ErrorCode.None;
//        }

//        public ErrorCode EndEnum() {
//            if (!isInEnum) {
//                return ErrorCode.Extraneous_End_Enum;
//            }

//            isInEnum = false;
//            CurrentAddress = enumCache_Address;
//            CurrentOutputOffset = enumCache_OutputOffset;
//            SurpressOutput = enumCache_SurpressOutput;

//            return ErrorCode.None;
//        }
//        #endregion

//        #region IF,IFDEF handling
//        struct ifInfo
//        {
//            /// <summary>Set to true when a block's condition has been met. Any subsequent blocks (ELSEIF or ELSE) should be ignored.</summary>
//            public bool foundCase;
//            /// <summary>True if the current block should be processed.</summary>
//            public bool processCurrentBlock;
//            /////// <summary>True if the block occurs in excluded code (this state needs to be restored when the block closes)</summary>
//            ////public bool isExcluded;
//        }


//        Stack<ifInfo> ifStack = new Stack<ifInfo>();

//        bool inIfStack { get { return ifStack.Count > 0; } }
//        /// <summary>
//        /// If true, the pass is currently passing over excluded code, such as code in an IF block where the condition is not met.
//        /// </summary>
//        public bool InExcludedCode { get { return ifStack.Count > 0 && !ifStack.Peek().processCurrentBlock; } }

//        public ErrorCode If_EnterExcludedIf() {
//            ifInfo blockInfo;
//            // Prevent any code of the if statement from running by...
//            blockInfo.foundCase = true; // ...setting foundCase - no other blocks will be considered
//            blockInfo.processCurrentBlock = false; // ...clearing processCurrentBlock - this block wont be considered
//            ////blockInfo.isExcluded = true; // ...set InExcludedCode=true when ENDIF is reached
//            ifStack.Push(blockInfo);

//            return ErrorCode.None;
//        }
//        public ErrorCode If_EnterIf(bool conditionMet) {
//            ifInfo blockInfo;
//            blockInfo.foundCase = conditionMet;
//            blockInfo.processCurrentBlock = conditionMet;
//            ////blockInfo.isExcluded = false;
//            ifStack.Push(blockInfo);

//            return ErrorCode.None;
//        }

//        /// <summary>
//        /// Call this method only when the next block, ELSE or ELSEIF, is encountered, AND NO BLOCKS HAVE BEEN EXECUTED YET.    
//        /// </summary>
//        /// <param name="conditionMet"></param>
//        /// <returns></returns>
//        public ErrorCode If_EnterElse(bool conditionMet) {
//            if(!inIfStack)
//                return ErrorCode.Missing_Preceeding_If;

//            var currentBlock = ifStack.Pop();
//            currentBlock.foundCase |= conditionMet;
//            currentBlock.processCurrentBlock = conditionMet;
//            ifStack.Push(currentBlock);

//            return ErrorCode.None;
//        }

//        public ErrorCode If_CloseIf() {
//            if (!inIfStack)
//                return ErrorCode.Missing_Preceeding_If;

//            var ifBlock = ifStack.Pop();
//            ////InExcludedCode = ifBlock.isExcluded;

//            return ErrorCode.None;
//        }

//        internal void ProcessIfBlockDirective(ConditionalDirective directive, out Error error) {
//            error = Error.None;
//            bool? ifCondition;

//            switch (directive.part) {
//                case ConditionalDirective.ConditionalPart.IF:
//                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
//                    if (InExcludedCode) {
//                        If_EnterExcludedIf();
//                    } else {
//                        ifCondition = If_EvalCondition(directive, out error);
//                        if (error.IsError) return;
//                        if(ifCondition == null){
//                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition,directive.SourceLine);
//                            return;
//                        }

//                        If_EnterIf((bool)ifCondition);
//                    }
//                    break;
//                case ConditionalDirective.ConditionalPart.ELSEIF:
//                    // We can treat else/else if like it is not in excluded code (if it IS in excluded code, it will look as if 
//                    // we already had a block executed, so the else/elseif won't be processed.)
//                    if (!inIfStack) {
//                        error = new Error(ErrorCode.Missing_Preceeding_If, Error.Msg_NoPreceedingIf, directive.SourceLine);
//                        return;
//                    }

//                    bool considerElseIf = !ifStack.Peek().foundCase; // Has a previous block been executed? (We only process an else if it hasn't)
//                    if (considerElseIf) {
//                        ifCondition = If_EvalCondition(directive, out error);
//                        if (error.IsError) return;
//                        if (ifCondition == null) {
//                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
//                            return;
//                        }

//                        If_EnterElse((bool)ifCondition);
//                    }
//                    break;
//                case ConditionalDirective.ConditionalPart.ELSE:
//                    // We can treat else/else if like it is not in excluded code (if it IS in excluded code, it will look as if 
//                    // we already had a block executed, so the else/elseif won't be processed.)
//                    if (!inIfStack) {
//                        error = new Error(ErrorCode.Missing_Preceeding_If, Error.Msg_NoPreceedingIf, directive.SourceLine);
//                        return;
//                    }

//                    bool runElse = !ifStack.Peek().foundCase; // Has a previous block been executed? (We only process an else if it hasn't)
//                    if (!directive.condition.IsNullOrEmpty ) {
//                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_NoTextExpected, directive.SourceLine);
//                        return;
//                    }

//                    If_EnterElse(runElse);
//                    break;
//                case ConditionalDirective.ConditionalPart.IFDEF:
//                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
//                    if (InExcludedCode) {
//                        If_EnterExcludedIf();
//                    } else {
//                        ifCondition = If_SymbolExists(directive);
//                        if (ifCondition == null) {
//                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
//                            return;
//                        }

//                        If_EnterIf((bool)ifCondition);
//                    }
//                    break;
//                case ConditionalDirective.ConditionalPart.IFNDEF:
//                    // We must note IFs in excluded code to respect nesting (avoid misinterpreting inner ENDIF)
//                    if (InExcludedCode) {
//                        If_EnterExcludedIf();
//                    } else {
//                        ifCondition = If_SymbolExists(directive);
//                        if (ifCondition == null) {
//                            error = new Error(ErrorCode.If_Missing_Condition, Error.Msg_MissingCondition, directive.SourceLine);
//                            return;
//                        }

//                        If_EnterIf(!(bool)ifCondition);
//                    }
//                    break; 
//                case ConditionalDirective.ConditionalPart.ENDIF:
//                    If_CloseIf();
//                    break;
//                default:
//                    error = new Error(ErrorCode.Engine_Error, "Unexpected IF block tiye in Pass.ProcessIfBlockDirective");
//                    break;
//            }
//        }

//        bool? If_EvalCondition(ConditionalDirective directive, out Error error) {
//            error = Error.None;

//            if (directive.condition.IsNullOrEmpty) return null;
//            var condition = directive.condition;
//            var result = Assembler.Evaluator.EvaluateExpression(ref condition,directive.SourceLine,out error);
//            return (int)result.Value != 0;
//        }
//        bool? If_SymbolExists(ConditionalDirective directive) {
//            if (directive.condition.IsNullOrEmpty) {
//                return null;
//            }

//            return Values.NameExists(directive.condition.ToString());
//        }

//        #endregion

//        /// <summary>
//        /// Sets up pointers for the next label and next directive.
//        /// </summary>
//        private void PrepNextPointers() {
//            if (Assembly.Labels.Count > 0) {
//                iNextLabel = 0;
//                NextLabel_iInstruction = Assembly.Labels[0].iInstruction;
//            } else {
//                Set_NoLabels();
//            }
//            if (Assembly.Directives.Count > 0) {
//                iNextDirective = 0;
//                NextDirective_iInstruction = Assembly.Directives[0].InstructionIndex;
//            } else {
//                Set_NoDirectives();
//            }
//        }

//        public AssemblyData Assembly { get; private set; }
//        public Assembler Assembler { get; private set; }


//        protected void AddError(Error error) {
//            Assembler.AddError(error);
//        }

//        protected void ProcessPendingLabelsAndDirectives() {
//            bool doMoreThings = true; // Until we decide that there is nothing pending

//            do {
//                bool labelPending = NextLabel_iInstruction <= iCurrentInstruction;
//                bool directivePending = NextDirective_iInstruction <= iCurrentInstruction;

//                // If there is both a label and directive pending, we need to figure which came first
//                if (labelPending && directivePending) {
//                    bool labelComesFirst = Assembly.Labels[iNextLabel].SourceLine <= Assembly.Directives[iNextDirective].SourceLine;

//                    if (labelComesFirst)
//                        directivePending = false;
//                    else
//                        labelPending = false;
//                }

//                if (labelPending) {
//                    if (!InExcludedCode) {
//                        ProcessCurrentLabel();
//                    }
//                    NextLabel();
//                } else if (directivePending) {
//                    // If we are in excluded code, only conditional directives are processed.
//                    if (!InExcludedCode || Assembly.Directives[iNextDirective] is ConditionalDirective)
//                        ProcessCurrentDirective();
//                    NextDirective();
//                } else {
//                    doMoreThings = false;
//                }
//            } while (doMoreThings);
//        }

//        protected abstract void ProcessCurrentLabel();

//        private void ProcessCurrentDirective() {
//            Error error;
//            Assembly.Directives[iNextDirective].Process(this, out error);
//            if (error.Code != ErrorCode.None) {
//                //error.SourceLine = Assembly.Directives[iNextDirective].SourceLine;
//                error = new Error(error,Assembly.Directives[iNextDirective].SourceLine);
//                AddError(error);
//            }
//        }

//        public int iNextLabel;
//        public int NextLabel_iInstruction;
//        public int iNextDirective;
//        public int NextDirective_iInstruction;

//        public int iCurrentInstruction;

//        void Set_NoLabels() {
//            iNextLabel = -1;
//            NextLabel_iInstruction = Int32.MaxValue;
//        }
//        void Set_NoDirectives() {
//            iNextDirective = -1;
//            NextDirective_iInstruction = Int32.MaxValue;

//        }
//        protected void NextLabel() {
//            iNextLabel++;
//            if (iNextLabel >= Assembly.Labels.Count) {
//                Set_NoLabels();
//            } else {
//                NextLabel_iInstruction = Assembly.Labels[iNextLabel].iInstruction;
//            }
//        }
//        protected void NextDirective() {
//            iNextDirective++;
//            if (iNextDirective >= Assembly.Directives.Count) {
//                Set_NoDirectives();
//            } else {
//                NextDirective_iInstruction = Assembly.Directives[iNextDirective].InstructionIndex;
//            }
//        }
//        // --------------------------------------------- <--This is a line.

//        /// <summary>
//        /// Writes a byte to the output stream. If there is no output stream
//        /// or SurpressOutput is true, the call is ignored (no errors will occur).
//        /// </summary>
//        /// <param name="b"></param>
//        public abstract void WriteByte(byte b);

//        /// <summary>
//        /// Writes padding to the output stream.  If there is no output stream
//        /// or SurpressOutput is true, the call is ignored (no errors will occur).
//        /// </summary>
//        public abstract void OutputBytePadding(int paddingAmt, byte fillValue);
//        /// <summary>
//        /// Writes padding to the output stream.  If there is no output stream
//        /// or SurpressOutput is true, the call is ignored (no errors will occur).
//        /// </summary>
//        public abstract void OutputWordPadding(int wordCount, ushort fillValue);

//        /// <summary>
//        /// Gets the stream that output is written to (if applicable). This stream should not be written to while SurpressOutput is true.
//        /// </summary>
//        public Stream OutputStream { get; protected set; }


//        /// <summary>
//        /// Resets the origin address (.ORG). For example, a .PATCH directive resets the origin. Any labels that follow a 
//        /// .PATCH without a .ORG in between should cause an error since there is no origin.
//        /// </summary>
//        public void ResetOrigin() {
//            OriginOffset = int.MinValue;
//            hasORGed = false;
//        }

//        #region PATCH directive stuffs
//        List<PatchSegment> PatchSegments = new List<PatchSegment>();

//        public bool InPatchMode { get; private set; }

//        /// <summary>
//        /// Specifies the address that the following code will be patched to. See remarks.
//        /// </summary>
//        /// <remarks>Once a patch offset is applied, any following code will be in "patch mode." .ORG directives will
//        /// not pad. Instead, a .ORG will begin a new patch section.</remarks>
//        public void SetPatchOffset(int offset) {
//            OnSetPatchOffset(offset);

//            ResetOrigin();

//            if (EmitOutput) {
//                CalculateLastPatchSegmentSize();

//                // Add a new patch segment
//                PatchSegments.Add(new PatchSegment((int)OutputStream.Length, -1, offset));
//            }
//        }

//        /// <summary>
//        /// Computes length of the most recently added patch segment. SEE REMARKS
//        /// </summary>
//        /// <remarks>It is assumed that the last patch extends to the end of the code stream.</remarks>
//        /// <returns></returns>
//        private void CalculateLastPatchSegmentSize() {
//            // 

//            if (EmitOutput) {
//                var lastPatch = PatchSegments[PatchSegments.Count - 1];
//                int streamSize = (int)OutputStream.Length;
//                int patchLen = streamSize - lastPatch.Start;
//                PatchSegments[PatchSegments.Count - 1] = new PatchSegment(lastPatch.Start, patchLen, lastPatch.PatchOffset);
//            } else {
//                throw new InvalidOperationException("CalculateLastPatchSegmentSize() not valid on a pass that does not emit output.");
//            }
//        }

//        /// <summary>
//        /// Inheritors can call this method if necessary.
//        /// </summary>
//        /// <param name="offset"></param>
//        protected virtual void OnSetPatchOffset(int offset) {
//            InPatchMode = true;
//        }

//        private void AddDefaultPatch() {
//            PatchSegments.Add(new PatchSegment(0, -1, -1));
//        }

//        public IList<PatchSegment> GetPatchSegments() {
//            for (int i = PatchSegments.Count - 1; i >= 0; i--) {
//                if (PatchSegments[i].Length == 0 && PatchSegments.Count > 1) {
//                    PatchSegments.RemoveAt(i);
//                }
//            }

//            return PatchSegments;
//        }
      
//        #endregion

//        public class PassValuesCollection
//        {
//            Dictionary<string, PassValue> Values = new Dictionary<string, PassValue>(StringComparer.InvariantCultureIgnoreCase);

//            public PassValuesCollection() {

//            }

//            public LiteralValue this[string name]{
//                get{
//                    return Values[name].value;
//                }
//            }

//            public LiteralValue? TryGetValue(string name) {
//                PassValue result;
//                bool found = Values.TryGetValue(name, out result);
//                return found ? (LiteralValue?)result.value : null;
//            }

//            public void SetValue(string name, LiteralValue value, bool isFixed, out bool valueIsFixedError) {
//                PassValue existingValue;
//                bool exists = Values.TryGetValue(name,out existingValue);

//                if (exists) {
//                    if (existingValue.isFixed) {
//                        valueIsFixedError = true;
//                        return;
//                    } else {
//                        Values.Remove(name);
//                    }
//                }

//                Values.Add(name, new PassValue(value, isFixed));
//                valueIsFixedError = false;
//            }

//            public bool RemoveValue(string name, out bool valueIsFixedError){
//                PassValue existingValue;
//                bool exists = Values.TryGetValue(name,out existingValue);

//                if (exists) {
//                    if (existingValue.isFixed) {
//                        valueIsFixedError = true;
//                        return false;
//                    } else {
//                        Values.Remove(name);
//                        valueIsFixedError = false;
//                        return true;
//                    }
//                }

//                valueIsFixedError = false;
//                return false;
//            }
//            private struct PassValue
//            {
//                public PassValue(LiteralValue value, bool isFixed) {
//                    this.value = value;
//                    this.isFixed = isFixed;
//                }
//                public LiteralValue value;
//                public bool isFixed;
//            }

//            internal bool NameExists(string name) {
//                return Values.ContainsKey(name);
//            }
//        }
//    }

//    class FirstPass : Pass
//    {
//        public FirstPass(Assembler asm)
//            : base(asm) {
//            EmitOutput = false;
//            AllowOverflowErrors = false;
//            ErrorOnUndefinedSymbol = false;
//        }

//        protected override void BeforePerformPass() {
//            base.BeforePerformPass();

//            // Assume undefined symbols are unprocessed labels (if not, they will be caught on second pass)
//            Assembler.Evaluator.ErrorOnUndefinedSymbols = false;
//        }

//        protected override void ProcessCurrentInstruction() {

//            var currentLine = Assembly.ParsedInstructions[iCurrentInstruction];
//            var opcode = Opcode.allOps[currentLine.opcode];

//            bool SingleByteOperand = false;
//            if (currentLine.operandExpression != null) {
//                // If an AsmValue has not been resolved to a literal value yet, it is because it references a label, and thus must be 16-bit

//                Error error;

//                StringSection expression = currentLine.operandExpression;
//                SingleByteOperand = Assembler.Evaluator.EvaluateExpression(ref expression, currentLine.sourceLine, out error).IsByte;
//                if (error.Code != ErrorCode.None) {
//                    AddError(new Error(error, currentLine.sourceLine));
//                } else if (!expression.IsNullOrEmpty) {
//                    AddError(new Error(ErrorCode.Invalid_Expression, Error.Msg_InvalidExpression, currentLine.sourceLine));
//                }

//            } else if (currentLine.operandValue.IsByte) {
//                SingleByteOperand = true;
//            }

//            if (SingleByteOperand) {
//                // Todo: Consider moving TryToConvertToZeroPage to more appropriate class
//                if (Assembler.Parser.TryToConvertToZeroPage(ref currentLine)) {
//                    // If the instruction can be coded as zero-page, update it
//                    Assembly.ParsedInstructions[iCurrentInstruction] = currentLine; // Todo: consider method such as UpdateParsedInstruction
//                }
//            }

//            var instructionLen = Opcode.GetParamBytes(currentLine.opcode) + 1;
//            CurrentOutputOffset += instructionLen;
//            CurrentAddress += instructionLen;
//        }





//        protected override void ProcessCurrentLabel() {
//            var label = Assembly.Labels[iNextLabel];

//            label.address = (ushort)CurrentAddress;
//            Assembly.Labels[iNextLabel] = label;

//            bool isAnonymous = label.name[0] == '~';
//            if (isAnonymous) {
//                Assembly.AnonymousLabels.ResolveNextPointer((ushort)CurrentAddress);
//            } else {
//                if (Values.NameExists(label.name)) {
//                    AddError(new Error(ErrorCode.Value_Already_Defined, string.Format(Error.Msg_ValueAlreadyDefined_name, label.name), label.SourceLine));
//                } else {
//                    bool isFixedError; // Todo: handle this
//                    if (label.address < 0x100) {
//                        Values.SetValue(label.name, new LiteralValue((ushort)CurrentAddress, true),true, out isFixedError);
//                    } else {
//                        Values.SetValue(label.name, new LiteralValue((ushort)CurrentAddress, false),true,out isFixedError);
//                    }
//                }
//            }
//        }

//        public override void WriteByte(byte b) {
//        }



//        public override byte[] GetOutput() {
//            throw new NotImplementedException();
//        }

//        public override void OutputBytePadding(int paddingAmt, byte fillValue) {
//            CurrentAddress += paddingAmt;
//            CurrentOutputOffset += paddingAmt;
//        }

//        public override void OutputWordPadding(int wordCount, ushort fillValue) {
//            CurrentAddress += wordCount * 2;
//            CurrentOutputOffset += wordCount * 2;
            
//        }



//    }

//    class SecondPass : Pass
//    {
//        public SecondPass(Assembler asm)
//            : base(asm) {
//            EmitOutput = true;
//            AllowOverflowErrors = true;
//            ErrorOnUndefinedSymbol = true;

//            base.OutputStream = this.outputStream;
//        }

//        MemoryStream outputStream = new MemoryStream();
//        byte[] output;
//        public override byte[] GetOutput() {
//            return output;
//        }

//        protected override void BeforePerformPass() {
//            base.BeforePerformPass();

//            LoadLabelValues();

//            Assembler.Evaluator.ErrorOnUndefinedSymbols = true;
//        }
//        ////protected override void PerformPass() {

//        ////    for (iCurrentInstruction = 0; iCurrentInstruction < Assembly.ParsedInstructions.Count; iCurrentInstruction++) {
//        ////        ProcessPendingLabelsAndDirectives();

//        ////        ProcessCurrentInstruction();
//        ////    }

//        ////    // Process any remaining directives (those that come after the last instruction)
//        ////    iCurrentInstruction = int.MaxValue - 1;
//        ////    ProcessPendingLabelsAndDirectives();


//        ////}
//        protected override void AfterPerformPass() {
//            base.AfterPerformPass();


//            output = outputStream.ToArray();
//            outputStream = null;

//        }


//        protected override void ProcessCurrentInstruction() {
//            Error error;

//            var currentLine = Assembly.ParsedInstructions[iCurrentInstruction];

//            if(!SurpressOutput)
//                outputStream.WriteByte(currentLine.opcode);

//            var addressing = Opcode.allOps[currentLine.opcode].addressing;
//            switch (addressing) {
//                case Opcode.addressing.implied:
//                    // No operand
//                    CurrentAddress++;
//                    CurrentOutputOffset++;
//                    break;
//                case Opcode.addressing.absoluteIndexedX:
//                case Opcode.addressing.absoluteIndexedY:
//                case Opcode.addressing.absolute:
//                case Opcode.addressing.indirect:
//                    // 16-bit opeand
//                    ushort operand16 = GetOperand(currentLine, out error);
//                    if (error.Code == ErrorCode.None) {
//                        if (!SurpressOutput) {
//                            outputStream.WriteByte((byte)(operand16 & 0xFF));
//                            outputStream.WriteByte((byte)(operand16 >> 8));
//                        }

//                        CurrentAddress += 3;
//                        CurrentOutputOffset += 3;
//                    } else {
//                        AddError(error);
//                    }
//                    break;
//                case Opcode.addressing.zeropageIndexedX:
//                case Opcode.addressing.zeropageIndexedY:
//                case Opcode.addressing.zeropage:
//                case Opcode.addressing.indirectX:
//                case Opcode.addressing.indirectY:
//                case Opcode.addressing.immediate:

//                    // 8-bit operand
//                    ushort operand8 = GetOperand(currentLine, out error);
//                    if (error.Code == ErrorCode.None) {
//                        if (operand8 > 0xFF) {
//                            if (IsZeroPage(addressing)) {
//                                error = new Error(ErrorCode.Address_Out_Of_Range, Error.Msg_ZeroPageOutOfRange, currentLine.sourceLine);
//                            } else {
//                                error = new Error(ErrorCode.Overflow, Error.Msg_ValueOutOfRange, currentLine.sourceLine);
//                            }
//                            AddError(error);
//                        }
//                        if (!SurpressOutput)
//                            outputStream.WriteByte((byte)(operand8 & 0xFF));

//                        CurrentAddress += 2;
//                        CurrentOutputOffset += 2;
//                    } else {
//                        AddError(error);
//                    }
//                    break;
//                case Opcode.addressing.relative:
//                    // Calculate relative offset
//                    int operandAbs = GetOperand(currentLine, out error);
//                    if (error.Code == ErrorCode.None) {
//                        int relativeOrigin = CurrentAddress + 2;
//                        int operandRel = operandAbs - relativeOrigin;
//                        // Verify in range
//                        if (operandRel < -128 || operandRel > 127) {
//                            error = new Error(ErrorCode.Overflow, Error.Msg_BranchOutOfRange, currentLine.sourceLine);
//                            AddError(error);
//                        }
//                        if (!SurpressOutput)
//                            outputStream.WriteByte((byte)operandRel);

//                        CurrentAddress += 2;
//                        CurrentOutputOffset += 2;
//                    } else {
//                        AddError(error);
//                    }
//                    break;
//            }
//        }

//        private bool IsZeroPage(Opcode.addressing addressing) {
//            switch (addressing) {
//                case Opcode.addressing.zeropage:
//                case Opcode.addressing.zeropageIndexedX:
//                case Opcode.addressing.zeropageIndexedY:
//                    return true;
//                case Opcode.addressing.implied:
//                case Opcode.addressing.immediate:
//                case Opcode.addressing.absolute:
//                case Opcode.addressing.indirect:
//                case Opcode.addressing.indirectX:
//                case Opcode.addressing.indirectY:
//                case Opcode.addressing.relative:
//                case Opcode.addressing.absoluteIndexedX:
//                case Opcode.addressing.absoluteIndexedY:
//                default:
//                    return false;
//            }
//        }

//        private void LoadLabelValues() {
//            bool isFixedError; // Todo: handle this
//            foreach (var label in Assembly.Labels) {
//                Values.SetValue(label.name, new LiteralValue((ushort)label.address, false),true,out isFixedError);
//            }
//        }

//        private ushort GetOperand(ParsedInstruction currentLine, out Error error) {
//            if (currentLine.operandExpression == null) {
//                error = Error.None;
//                return currentLine.operandValue.Value;
//            }
//            if (currentLine.operandExpression != null) {
//                StringSection expression = currentLine.operandExpression;

//                var result = Assembler.Evaluator.EvaluateExpression(ref expression, currentLine.sourceLine, out error).Value;

//                if (error.Code != ErrorCode.None) {
//                    error = new Error(error, currentLine.sourceLine);
//                    return 0;
//                } else if (!expression.IsNullOrEmpty) {
//                    error = new Error(ErrorCode.Invalid_Expression, Error.Msg_InvalidExpression, currentLine.sourceLine);
//                    return 0;
//                }
//                return result;
//            }

//            error = new Error(ErrorCode.Engine_Error, "Instruction required an operand, but none was present. Instruction may have been mis-parsed.", currentLine.sourceLine);
//            return 0;
//        }


//        ////private static void ThrowOperandOutOfRangeException(int operandRel, int sourceLine) {
//        ////    int outOfRangeAmt;
//        ////    if (operandRel < 0)
//        ////        outOfRangeAmt = Math.Abs(operandRel + 128);
//        ////    else
//        ////        outOfRangeAmt = Math.Abs(operandRel - 127);
//        ////    throw new OperandOutOfRangeException("Operand is out of range (by " + outOfRangeAmt.ToString("X") + ")", sourceLine);
//        ////}

//        protected override void ProcessCurrentLabel() {
//            // Nothing to do with labels for second pass.
//            var label = Assembly.Labels[iNextLabel];

//            bool isAnonymous = label.name != null && label.name.StartsWith("~");
//            if (!isAnonymous) {

//                if (label.address < 0x8000) {
//                    // RAM
//                    Assembler.AddDebugLabel(-1, CurrentAddress, label.name);
//                }
//                if(Bank >= 0 ){
//                    // ROM
//                    Assembler.AddDebugLabel(Bank, CurrentAddress, label.name);
//                }
//            }
//        }

//        public override void WriteByte(byte b) {
//            if (!SurpressOutput)
//                outputStream.WriteByte(b);
//        }

//        public override void OutputBytePadding(int paddingAmt, byte fillValue) {
//            if (!SurpressOutput) {
//                for (int i = 0; i < paddingAmt; i++) {
//                    outputStream.WriteByte(fillValue);
//                }
//            }

//            CurrentAddress += paddingAmt;
//            CurrentOutputOffset += paddingAmt;
//        }

//        public override void OutputWordPadding(int wordCount, ushort fillValue) {
//            if (!SurpressOutput) {
//                for (int i = 0; i < wordCount; i++) {
//                    outputStream.WriteByte((byte)fillValue);
//                    outputStream.WriteByte((byte)(fillValue >> 8));
//                }
//            }

//            CurrentAddress += wordCount * 2;
//            CurrentOutputOffset += wordCount * 2;
//        }


//    }
//}
