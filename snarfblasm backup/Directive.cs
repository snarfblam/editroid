using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Romulus;

namespace snarfblasm
{
    abstract class Directive
    {
        public Directive(int instructionIndex, int sourceLine) {
            this.InstructionIndex = instructionIndex;
            this.SourceLine = sourceLine;
        }
        /// <summary>Gets the index of the first instruction following the directive.</summary>
        public int InstructionIndex { get; private set; }
        public int SourceLine { get; private set; }

        public abstract void Process(Pass pass, out Error error);
    }

    class Assignment : Directive
    {
        public Assignment(int instructionIndex, int sourceLine, StringSection variable, bool isLabel, AsmValue value)
            : base(instructionIndex, sourceLine) {
            this.Variable = variable;
            this.Value = value;
            this.IsLabel = isLabel;
        }
        public StringSection Variable { get; private set; }
        public AsmValue Value { get; private set; }
        public bool IsLabel { get; private set; }

        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            if (Value.IsLiteral) {
                pass.Assembler.Values.SetValue(Variable, Value.Literal,IsLabel, out error);

                if (error.IsError) {
                    error = new Error(error, SourceLine);
                    return;
                }

                if (IsLabel) {
                    int bank = pass.Bank;
                    pass.Assembler.AddDebugLabel(bank, Value.Literal.Value, Variable.ToString());
                }
            } else {
                StringSection exp = Value.Expression;
                var expressionValue = pass.Assembler.Evaluator.EvaluateExpression(ref exp, int.MinValue, out error);
                
                if (error.IsError) {
                    error = new Error(error, SourceLine);
                    return;
                }

                pass.Assembler.Values.SetValue(Variable, expressionValue ,IsLabel, out error);

                if (error.IsError) {
                    error = new Error(error, SourceLine);
                    return;
                }

                if (IsLabel){
                    int bank = pass.Bank;
                    if (bank < 0) bank = 0;
                    pass.Assembler.AddDebugLabel(bank, expressionValue.Value, Variable.ToString());
                }

                if (!exp.IsNullOrEmpty) {
                    error = new Error(ErrorCode.Invalid_Expression, Error.Msg_InvalidExpression, SourceLine);
                }
            }
        }

        
        
        ////public static Assignment Parse(int instructionIndex, int sourceLine, StringSection line) {
        ////    int iEquals = line.IndexOf('=');
        ////    StringSection varName = line.Substring(0, iEquals).Trim();
        ////    StringSection expression = line.Substring(iEquals + 1).Trim();

        ////    LiteralValue literal;
        ////    AsmValue value;
        ////    if (ExpressionEvaluator.TryParseLiteral(expression, out literal)) {
        ////        value = new AsmValue(literal);
        ////    } else {
        ////        value = new AsmValue(expression.ToString());
        ////    }

        ////    return new Assignment(instructionIndex, sourceLine, varName, value);
        ////}
    }
class ErrorDirective : Directive
        {
            public ErrorDirective(int instructionIndex, int sourceLine, StringSection msg)
                : base(instructionIndex, sourceLine) {
                this.errorMessage = msg.ToString();
            }
            string errorMessage;

            public override void Process(Pass pass, out Error error) {
                error = new Error(ErrorCode.User_Error, errorMessage, SourceLine);
            }
        }

    class IncBinDirective : Directive
    {
        public IncBinDirective(int instructionIndex, int sourceLine, StringSection file)
            : base(instructionIndex, sourceLine) {

            file = file.Trim();
            if (file.Length > 1 && file[0] == '\"' && file[file.Length - 1] == '\"')
                file = file.Substring(1, file.Length - 2);

            this.Filename = file.ToString();
        }

        public string Filename { get; private set; }

        /// <summary>Caches the size of the file to confirm it hasn't changed between passes.
        /// Am I being paranoid?</summary>
        private long cachedFileSize;

        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            if (pass.EmitOutput) {
                var fileStream = pass.Assembler.FileSystem.GetFileReadStream(Filename);
                
                byte[] buffer = new byte[0x1000];
                int readBytes = fileStream.Read(buffer, 0, buffer.Length);
                long totalBytesRead = readBytes;

                while (readBytes > 0) {
                    if(!pass.SurpressOutput)
                        pass.OutputStream.Write(buffer, 0, readBytes);

                    readBytes = fileStream.Read(buffer, 0, buffer.Length);
                    totalBytesRead += readBytes;
                }

                if (totalBytesRead != cachedFileSize) {
                    error = new Error(ErrorCode.File_Error, string.Format(Error.Msg_CantAccessFile_name, Filename), SourceLine);
                }

                pass.CurrentAddress += (int)totalBytesRead;
                pass.CurrentOutputOffset += (int)totalBytesRead;
            } else {
                if (pass.Assembler.FileSystem.FileExists(Filename)) {
                    cachedFileSize = pass.Assembler.FileSystem.GetFileSize(Filename);
                    if (cachedFileSize < 0)
                        error = new Error(ErrorCode.File_Error, string.Format(Error.Msg_CantAccessFile_name, Filename), SourceLine);
                } else {
                    error = new Error(ErrorCode.File_Not_Found, string.Format(Error.Msg_FileNotFound_name, Filename), SourceLine);
                }

                pass.CurrentAddress += (int)cachedFileSize;
                pass.CurrentOutputOffset += (int)cachedFileSize;
            }
        }
    }

    class OrgDirective : Directive
    {
        public OrgDirective(int instructionIndex, int sourceLine, AsmValue address)
            : base(instructionIndex, sourceLine) {
            this.Address = address;
        }



        public AsmValue Address { get; set; }
        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            LiteralValue value;

            // Either evaluate address expression if needed, or just grab the already-evaluated address
            if (Address.IsExpression) {
                StringSection exp = Address.Expression;

                value = pass.Assembler.Evaluator.EvaluateExpression(ref exp, SourceLine, out error);

                if (error.Code == ErrorCode.None) {
                    if (exp.Length != 0) {
                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                        return;
                    }
                } else {
                    return;
                }
            } else {
                value = Address.Literal;
            }

            int address = value.Value;

            if (address < pass.CurrentAddress && !pass.InPatchMode) {
                error = new Error(ErrorCode.Invalid_Org, Error.Msg_OrgLessThanCurrent, SourceLine);
            } else {
                pass.SetOrigin(address);
            }

        }

    }
    class PatchDirective : Directive
    {
        public PatchDirective(int instructionIndex, int sourceLine, string patchValue)
            : base(instructionIndex, sourceLine) {
            int org;
            int bank;
            this.PatchOffset = ParsePatchOffset(patchValue, out org, out bank);
            this.PatchOrigin = org;
            this.Bank = bank;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchValue"></param>
        /// <param name="origin">The origin address of the patch, or -1 if none is specified.</param>
        /// <returns></returns>
        private static int ParsePatchOffset(string patchValue, out int origin, out int bank) {
            origin = -1;
            bank = -1;
            bool hex = false;


            patchValue = patchValue.Trim();
            if (patchValue.StartsWith("$")) {
                patchValue = patchValue.Substring(1);
                hex = true;
            }else if(patchValue.StartsWith("0x")){
                patchValue = patchValue.Substring(2);
                hex = true;
            } else if (patchValue.Contains(":")) {
                var parts = patchValue.Split(':');
                if (parts.Length == 2) {
                    int bankNum;
                    int address;
                    if (!int.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out bankNum))
                        return -1;
                    if (!int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out address))
                        return -1;

                    bank = bankNum;
                    origin = address;
                    return bankNum * 0x4000 + (address % 0x4000) + 0x10;
                } else {
                    return -1;
                }
            }

            int result;
            if (int.TryParse(patchValue, hex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.Integer, null, out result)) {
                bank = (result - 0x10) / 0x4000;
                return result;
            } else {
                return  -1;
            }
        }



        public int PatchOffset { get; set; }
        public int Bank { get; set; }
        int PatchOrigin { get; set; }

        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            if (PatchOffset < 0) {
                pass.Assembler.AddError(new Error(ErrorCode.Invalid_Directive_Value, Error.Msg_InvalidPatchOffset, SourceLine));
            } else {
                if (PatchOrigin >= 0) {
                    pass.SetOrigin(PatchOrigin);
                }
                if (Bank >= 0)
                    pass.Bank = Bank;
                pass.SetPatchOffset(PatchOffset);
            }

        }

    }
    class BaseDirective : Directive
    {
        public BaseDirective(int instructionIndex, int sourceLine, AsmValue address)
            : base(instructionIndex, sourceLine) {
            this.Address = address;
        }



        public AsmValue Address { get; set; }
        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            LiteralValue value;

            if (Address.IsExpression) {
                StringSection exp = Address.Expression;

                value = pass.Assembler.Evaluator.EvaluateExpression(ref exp, SourceLine, out error);

                if (error.Code == ErrorCode.None) {
                    if (exp.Length != 0) {
                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                        return;
                    }
                } else {
                    return;
                }
            } else {
                value = Address.Literal;
            }

            pass.SetAddress(value.Value);

        }

    }


    // Todo: hex directive



    class EnumDirective : Directive
    {
        public EnumDirective(int iInstruction, int iSourceLine, StringSection addressExpression)
            : base(iInstruction, iSourceLine) {
            this.AddressExpression = addressExpression;
        }

        readonly StringSection AddressExpression;

        public override void Process(Pass pass, out Error error) {
            var addressExp = AddressExpression;

            var address = pass.Assembler.Evaluator.EvaluateExpression(ref addressExp, SourceLine, out error);

            ErrorCode beginEnumError = ErrorCode.None;
            if (error.IsError) {
                // We still open the enum if there is an error, because we don't want cascading errors from things
                // being process in a way they aren't supposed to, or from the ENDE
                beginEnumError = pass.BeginEnum(0);
            } else {
                // If there is test remaining in the expression, it is extraneous
                if (addressExp.Trim().IsNullOrEmpty) {
                    beginEnumError = pass.BeginEnum(address.Value);
                } else {
                    error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                }
            }

            // We can only return one error through the out parameter, but we can have a second error from BeginEnum,
            // so we use AddError.
            if (beginEnumError != ErrorCode.None) {
                pass.Assembler.AddError(new Error(beginEnumError, Error.Msg_EnumError, SourceLine));
            }

        }
    }

    class EndEnumDirective : Directive
    {
        public EndEnumDirective(int iInstruction, int iSourceLine)
            : base(iInstruction, iSourceLine) {
        }

        public override void Process(Pass pass, out Error error) {
            ErrorCode endEnumError = pass.EndEnum();

            if (endEnumError != ErrorCode.None) {
                error = new Error(endEnumError, Error.Msg_EnumError, SourceLine);
            } else {
                error = Error.None;
            }
        }
    }

    class StorageDirective : Directive
    {
        public StorageDirective(int iInstruction, int iSourceLine, StringSection line, DataType dataType)
            : base(iInstruction, iSourceLine) {
            this.line = line;
            this.dataType = dataType;
        }
        readonly StringSection line;
        readonly DataType dataType;


        public override void Process(Pass pass, out Error error) {
            error = Error.None;
            var lineExp = line;

            int entryCount = 1;
            ushort fillValue = 0;

            // Parse entry count (or default to 1)
            if (!lineExp.IsNullOrEmpty) {
                entryCount = pass.Assembler.Evaluator.EvaluateExpression(ref lineExp, SourceLine, out error).Value;
                if (error.IsError) return;
            }

            // The only valid remaining text is ',fillvalue'
            if (lineExp.Length > 0) {
                if (lineExp[0] == ',') { // expect comma
                    lineExp = lineExp.Substring(1).TrimLeft();

                    // Evaluate fill value
                    var fillExpValue = pass.Assembler.Evaluator.EvaluateExpression(ref lineExp, SourceLine, out error);

                    if (error.IsError) return;
                    if (lineExp.Length > 0) {
                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                        return;
                    }

                    // Get fill value, and ensure it is not too large
                    fillValue = fillExpValue.Value;
                    if (dataType == DataType.Bytes && fillValue > byte.MaxValue) {
                        error = new Error(ErrorCode.Overflow, Error.Msg_ValueOutOfRange, SourceLine);
                        return;
                    }
                } else {
                    // Text other than a comma is invalid
                    error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                }
            }

            switch (dataType) {
                case DataType.Bytes:
                    pass.OutputBytePadding(entryCount, (byte)fillValue);
                    break;
                case DataType.Words:
                    pass.OutputWordPadding(entryCount, (ushort)fillValue);
                    break;
                default:
                    error = new Error(ErrorCode.Engine_Error, "StorageDirective.dataType is invalid.", SourceLine);
                    break;
            }
        }
        public enum DataType
        {
            Bytes,
            Words,
        }
    }


    // Todo: pad directive (behaves very similar to non-initial ORG)
    // Todo: .define directive: define an immutable symbol (probably = $FFFF). This would really only be for the purpose of .ifdef

    class DefineDirective : Directive
    {
        public DefineDirective(int iInstruction, int iSourceLine, StringSection varName)
            : base(iInstruction, iSourceLine) {

            this.VarName = varName.ToString();
        }

        string VarName;
        
        public override void Process(Pass pass, out Error error) {
            error = Error.None;
            pass.Assembler.Assembly.SetValue(VarName, new LiteralValue(0xFFFF, false),false, out error);
        }
    }

    
    class ConditionalDirective : Directive
    {
        public ConditionalDirective(int iInstruction, int iSourceLine, StringSection directive, StringSection line, out Error error)
            : base(iInstruction, iSourceLine) {
            error = Error.None;
            condition = line;


            switch (directive.ToString().ToUpper()) {
                case "IF":
                    part = ConditionalPart.IF;
                    break;
                case "ELSEIF":
                    part = ConditionalPart.ELSEIF;
                    break;
                case "ELSE":
                    part = ConditionalPart.ELSE;
                    break;
                case "IFDEF":
                    part = ConditionalPart.IFDEF;
                    break;
                case "IFNDEF":
                    part = ConditionalPart.IFNDEF;
                    break;
                case "ENDIF":
                    part = ConditionalPart.ENDIF;
                    break;
                default:
                    part = ConditionalPart.none;
                    break;
            }

            return;
        }

        public ConditionalPart part { get; private set; }
        public StringSection condition { get; private set; }

        public enum ConditionalPart
        {
            none,
            IF,
            ELSEIF,
            ELSE,
            IFDEF,
            IFNDEF,
            ENDIF
        }


        public override void Process(Pass pass, out Error error) {
            pass.ProcessIfBlockDirective(this, out error);
            ////ErrorCode passError = ErrorCode.None;
            ////bool conditionValue;

            ////switch (part) {
            ////    case ConditionalPart.none:
            ////        error = new Error(ErrorCode.Engine_Error, "Invalid ConditionalPart in ConditionalDirective.Process.", SourceLine);
            ////        return;
            ////        break;
            ////    case ConditionalPart.IF:
            ////        if (pass.InExcludedCode) {
            ////            pass.If_EnterExcludedIf();
            ////        } else {
            ////            conditionValue = EvaluateCondition(pass, out error);
            ////            if (error.IsError) return;
            ////            pass.If_EnterIf(conditionValue);
            ////        }
            ////        break;
            ////    case ConditionalPart.ELSEIF:
            ////        // todo: we need to know if the condition has been met for the if block we are evaluating. this is currently private in the pass class.
            ////        // if the condition has been met, we shouldn't evaluate the expression.

            ////        conditionValue = EvaluateCondition(pass, out error);
            ////        //if(pass.InExcludedCode
            ////        if (error.IsError) return;
            ////        pass.If_EnterIf(conditionValue);
            ////        break;
            ////    case ConditionalPart.ELSE:
            ////        break;
            ////    case ConditionalPart.IFDEF:
            ////        break;
            ////    case ConditionalPart.IFNDEF:
            ////        break;
            ////    case ConditionalPart.ENDIF:
            ////        break;
            ////    default:
            ////        break;
            ////}
        }

        private bool EvaluateCondition(Pass pass, out Error error) {
            if (condition.IsNullOrEmpty) {
                error = new Error(ErrorCode.Expected_Expression, Error.Msg_ExpectedValue, SourceLine);
                return false;
            }

            var conditionExp = condition;
            var result = pass.Assembler.Evaluator.EvaluateExpression(ref conditionExp, SourceLine, out error);
            if (error.IsError) return false;

            error = Error.None;
            return result.Value != 0;
        }

    }

    class HexDirective : Directive
    {
        public HexDirective(int iInstruction, int iSourceLine, StringSection hexstring)
            : base(iInstruction, iSourceLine) {
            ParseHexBytes(hexstring);
        }

        private void ParseHexBytes(StringSection hexstring) {

            int firstDigitVal = 0;
            bool secondDigit = false; // First or second digit?

            for (int i = 0; i < hexstring.Length; i++) {
                char c = hexstring[i];
                if (!char.IsWhiteSpace(c)) {
                    int digitValue = Hex.ParseDigit(c);
                    // Invalid value?
                    if (digitValue == -1) {
                        invalidChar = c;
                        return;
                    }

                    // Ignore whitespace
                    if (secondDigit) {
                        bytes.Add((byte)((firstDigitVal << 4) | digitValue));
                        secondDigit = false;
                    } else {
                        firstDigitVal = digitValue;
                        secondDigit = true;
                    }

                }
            }
            if (secondDigit) {
                oddNumberOfDigits = true;
            }
        }
        /// <summary>
        /// A List of bytes representing the hex data.
        /// </summary>
        List<byte> bytes = new List<byte>();
        bool oddNumberOfDigits = false;
        char invalidChar = '\0';

        public override void Process(Pass pass, out Error error) {
            if (oddNumberOfDigits) {
                error = new Error(ErrorCode.Invalid_Directive_Value, Error.Msg_UnpairedHexDigit, SourceLine);
            } else if (invalidChar != '\0') {
                error = new Error(ErrorCode.Invalid_Directive_Value, string.Format(Error.Msg_InvalidHexDigit_char, invalidChar.ToString()), SourceLine);
            } else {
                error = default(Error);
                for (int i = 0; i < bytes.Count; i++) {
                    pass.WriteByte(bytes[i]);
                }
            }

        }
    }


    /// <summary>
    /// Directive that allows a value or list of values to be specified, such as .db or .word
    /// </summary>
    class DataDirective : Directive
    {
        public DataDirective(int iInstruction, int iSourceLine, StringSection line, DataType dataType)
            : base(iInstruction, iSourceLine) {
            this.line = line;
            this.dataType = dataType;
        }

        StringSection line;
        DataType dataType;

        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            var expressionList = line;

            while (!expressionList.IsNullOrEmpty) {
                // STRING is interpreted as a series of ASCII bytes
                if (expressionList[0] == '\"') {
                    expressionList = expressionList.Substring(1); // Remove quote

                    if (dataType != DataType.Bytes && dataType != DataType.Implicit) {
                        error = new Error(ErrorCode.Invalid_Directive_Value, Error.Msg_StringNotAllowed, SourceLine);
                        return;
                    }

                    ErrorCode stringErr;
                    var chars = Parser.ParseString(ref expressionList, out stringErr);
                    if (stringErr == ErrorCode.None) {
                        bool hasEndQuote = expressionList.Length > 0 && expressionList[0] == '\"';
                        if (hasEndQuote) {
                            expressionList = expressionList.Substring(1).TrimLeft(); // Remove closing quote

                            for (int i = 0; i < chars.Length; i++) {
                                char c = chars[i];

                                if (c < 128) {
                                    EmitData(pass, ref error, new LiteralValue((ushort)c, true));
                                } else {
                                    error = new Error(ErrorCode.Expected_Ascii, Error.Msg_DataStringNotAscii, SourceLine);
                                    return;
                                }
                            }
                        } else {
                            error = new Error(ErrorCode.Expected_Closing_Paren, Error.Msg_MissingCloseQuote, SourceLine);
                            return;
                        }
                    }else{
                        error = new Error(stringErr, "Invalid string", SourceLine);
                        return;
                    }

                    // Remove comma
                    if (expressionList.Length > 0){
                        if (expressionList[0] == ',')
                            expressionList = expressionList.Substring(1).TrimLeft();
                        else {
                            error = new Error(ErrorCode.Unexpected_Text, Error.Msg_ExpectedCommaOrEndline, SourceLine);
                            return;
                        }
                    }
                } else { // numeric expression \\
                    StringSection singleExpression;

                    var indexOfComma = expressionList.IndexOf(',');
                    if (indexOfComma == -1) {
                        singleExpression = expressionList.Trim();
                        expressionList = StringSection.Empty;
                    } else {
                        singleExpression = expressionList.Substring(0, indexOfComma).Trim();
                        expressionList = expressionList.Substring(indexOfComma + 1).TrimLeft();
                    }
                    var value = pass.Assembler.Evaluator.EvaluateExpression(ref singleExpression, SourceLine, out error);
                    if (error.Code != ErrorCode.None) {
                        error = new Error(error, SourceLine);
                        return;
                    }
                    if (!singleExpression.IsNullOrEmpty) {
                        error = new Error(ErrorCode.Unexpected_Text, Error.Msg_InvalidExpression, SourceLine);
                    }

                    EmitData(pass, ref error, value);
                }

            }
        }

        private void EmitData(Pass pass, ref Error error, LiteralValue value) {
            switch (dataType) {
                case DataType.Bytes:
                    pass.CurrentAddress += 1;
                    pass.CurrentOutputOffset += 1;
                    if (pass.EmitOutput) {
                        if (value.IsByte) {
                            pass.WriteByte((byte)value.Value);
                        } else {
                            error = new Error(ErrorCode.Type_Mismatch, Error.Msg_ValueNotByte, SourceLine);
                        }
                    }
                    break;
                case DataType.Words:
                    pass.CurrentAddress += 2;
                    pass.CurrentOutputOffset += 2;
                    if (pass.EmitOutput) {
                        // if (value.IsByte) {
                        //    error = new Error(ErrorCode.Type_Mismatch, Error.Msg_ValueNotWord, SourceLine);
                        //} else {
                            pass.WriteByte((byte)value.Value); // Low
                            pass.WriteByte((byte)(value.Value >> 8)); // High
                        //}
                    }
                    break;
                case DataType.Implicit:
                    if (value.IsByte) {
                        pass.CurrentAddress += 1;
                        pass.CurrentOutputOffset += 1;
                        if (pass.EmitOutput) {
                            pass.WriteByte((byte)value.Value);
                        }
                    } else {
                        pass.CurrentAddress += 2;
                        pass.CurrentOutputOffset += 2;
                        if (pass.EmitOutput) {
                            pass.WriteByte((byte)value.Value); // Low
                            pass.WriteByte((byte)(value.Value >> 8)); // High
                        }
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Fail("Invalid case");
                    break;
            }
        }


        public enum DataType
        {
            Bytes,
            Words,
            Implicit
        }
    }

    class OptionDirective : Directive
    {

        string name;
        string option;

        public OptionDirective(int iInstruction, int iSourceLine, string name, string option)
            : base(iInstruction, iSourceLine) {

            this.name = name.ToUpper();
            this.option = option.ToUpper();
        }

        public override void Process(Pass pass, out Error error) {
            error = Error.None;

            switch (name) {
                case "OVERFLOW":
                    SetOverflow(pass, ref error);
                    break;
                case "SIGNED":
                    SetSigned(pass, ref error);
                    break;
                default:
                    error = new Error(ErrorCode.Engine_Error, "OptionDirective was parsed with an invalid option name: " + name, SourceLine);
                    break;
            }
        }

        private void SetOverflow(Pass pass, ref Error error) {
            if (!pass.AllowOverflowErrors) return;

            switch (option) {
                case "ON":
                    pass.Assembler.Evaluator.OverflowChecking = true;
                    break;
                case "OFF":
                    pass.Assembler.Evaluator.OverflowChecking = false;
                    break;
                default:
                    error = GetInvalidValueError(error);
                    break;
            }
        }
        private void SetSigned(Pass pass, ref Error error) {
            switch (option) {
                case "ON":
                    pass.Assembler.Evaluator.SignedMode = true;
                    break;
                case "OFF":
                    pass.Assembler.Evaluator.SignedMode = false;
                    break;
                default:
                    error = GetInvalidValueError(error);
                    break;
            }
        }

        private Error GetInvalidValueError(Error error) {
            return new Error(ErrorCode.Invalid_Directive_Value, string.Format(Error.Msg_InvalidDirectiveOption_name_option, name, option), SourceLine);
        }


    }

}
