using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    /// <summary>
    /// Stores data parsed from assembly code.
    /// </summary>
    class AssemblyData:IValueNamespace
    {
        Assembler assembler;
        public AssemblyData(Assembler asm) {
            this.assembler = asm;

            ParsedInstructions = new List<ParsedInstruction>();
            Labels = new List<Label>();
            Directives = new List<Directive>();
        }


        // Todo: consider making read only (modifications can be done via methods)
        public IList<ParsedInstruction> ParsedInstructions { get; private set; }
        public IList<Label> Labels { get; private set; }
        public IList<Directive> Directives { get; private set; }

        // Todo: move to assembly class, make private, add helper methods
        AnonymousLabelCollection anonymousLabels = new AnonymousLabelCollection();
        public AnonymousLabelCollection AnonymousLabels { get { return anonymousLabels; } }
        int anonLabelIndex = 0;
        /// <summary>
        /// Creates a named label with a unique name to correspond to an anonymous label.
        /// </summary>
        /// <param name="iInstruction"></param>
        /// <param name="iSourceLine"></param>
        internal void TagAnonLabel(int iInstruction, int iSourceLine) {
            Labels.Add(new Label("~" + anonLabelIndex.ToString(), iInstruction, iSourceLine,false));
            anonLabelIndex++;
        }

        #region IValueNamespace Members

        public int GetForwardLabel(int labelLevel, int iSourceLine) {
            return AnonymousLabels.FindLabel_Forward(labelLevel, iSourceLine);
        }

        public int GetBackwardLabel(int labelLevel, int iSourceLine) {
            return AnonymousLabels.FindLabel_Back(labelLevel, iSourceLine);
        }
        public int GetForwardBrace(int labelLevel, int iSourceLine) {
            return AnonymousLabels.FindBrace_Forward(labelLevel, iSourceLine);
        }

        public int GetBackwardBrace(int labelLevel, int iSourceLine) {
            return AnonymousLabels.FindBrace_Back(labelLevel, iSourceLine);
        }

        public void SetValue(Romulus.StringSection name, LiteralValue value, bool isFixed, out Error error) {
            error = Error.None;

            if (assembler.CurrentPass == null)
                throw new InvalidOperationException("Can only access variables when assembler is running a pass.");

            bool isDollar = Romulus.StringSection.Compare(name, "$", true) == 0;
            if (isDollar) {
                assembler.CurrentPass.SetAddress(value.Value);
            }

            string nameString = name.ToString();
            //assembler.CurrentPass.Values.Remove(nameString);
            //assembler.CurrentPass.Values.Add(nameString, value);
            bool fixedValueError;
            assembler.CurrentPass.Values.SetValue(nameString, value, isFixed, out fixedValueError);
            if (fixedValueError) {
                // Todo: replace this with assembler value, or return fixedValueError somehow.
                //throw new Exception("Attempted to assign to a fixed value. This exception should be replaced with an appropriate assembler error.");
                error = new Error(ErrorCode.Value_Already_Defined, string.Format(Error.Msg_ValueAlreadyDefined_name, nameString));
            }
        }

        public LiteralValue GetValue(Romulus.StringSection name) {
            bool isDollar = Romulus.StringSection.Compare(name, "$", true) == 0;
            if (isDollar) {
                return new LiteralValue((ushort)assembler.CurrentPass.CurrentAddress,false );
            }

            LiteralValue? result;
            if (null == (result = assembler.CurrentPass.Values.TryGetValue(name.ToString()))) {
                throw new Exception(); // Todo: Must be more specific, and handled!
            }
            return result.Value;
        }

        public bool TryGetValue(Romulus.StringSection name, out LiteralValue result) {
            bool isDollar = Romulus.StringSection.Compare(name, "$", true) == 0;
            if (isDollar) {
                result = new LiteralValue((ushort)assembler.CurrentPass.CurrentAddress, false);
                return true;
            }

            var value = assembler.CurrentPass.Values.TryGetValue(name.ToString());
            result = value.HasValue ? value.Value : default(LiteralValue);
            return value.HasValue;
        }

        #endregion
    }

    struct Label
    {
        public Label(string name, int location, int sourceLine, bool local) {
            this.name = name;
            this.iInstruction = location;
            this.SourceLine = sourceLine;
            this.address = 0;
            this.local = local;
        }
        public readonly string name;
        public readonly int iInstruction;
        public readonly int SourceLine;
        public ushort address;
        public bool local;

    }
    /// <summary>
    /// Identifies a value (literal or expression) used in ASM.
    /// </summary>
    struct AsmValue
    {
        public AsmValue(int value, bool isByte) {
            this.Literal = new LiteralValue((ushort)value, isByte);
            Expression = null;
        }
        public AsmValue(LiteralValue value) {
            this.Literal = value;
            Expression = null;
        }
        public AsmValue(string expression) {
            Literal = new LiteralValue();
            Expression = expression;
        }
        public LiteralValue Literal;
        public string Expression;

        public bool IsExpression { get { return Expression != null; } }
        public bool IsLiteral { get { return !IsExpression; } }
    }
    struct LiteralValue
    {
        public LiteralValue(ushort value, bool isbyte) {
            Value = value;
            IsByte = isbyte;

            // Todo: Is truncation best? Perhaps exception?
            if (isbyte) {
                Value &= 0xFF;
            }
        }
        public readonly ushort Value;
        public readonly bool IsByte;
    }
    struct ParsedInstruction
    {
        public readonly byte opcode;
        public readonly LiteralValue operandValue;
        /// <summary>The expression that specifies the operand value, or null if the expression has been evaluated.</summary>
        public readonly string operandExpression;
        public readonly int sourceLine;

        public ParsedInstruction(byte opcode, LiteralValue operandValue, int sourceLine) {
            this.opcode = opcode;
            this.operandValue = operandValue;
            this.operandExpression = null;
            this.sourceLine = sourceLine;
        }
        public ParsedInstruction(byte opcode, string operandExpression, int sourceLine) {
            this.opcode = opcode;
            this.operandValue = new LiteralValue();
            this.operandExpression = operandExpression;
            this.sourceLine = sourceLine;
        }
        public ParsedInstruction(ParsedInstruction oldLine, byte newOpcode) {
            this = oldLine; // Does this work?
            this.opcode = newOpcode;
        }
        static ParsedInstruction() {
            Empty = new ParsedInstruction(0xFF, null, -1);
        }

        /// <summary>
        /// Defines the value that specifies no value for the OperandValue field
        /// </summary>
        public const int NoLiteralOperand = -1;

        public static readonly ParsedInstruction Empty;
        bool IsEmpty { get { return sourceLine == -1; } }




    }

}
