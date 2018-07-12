using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    public struct Error
    {
        public readonly ErrorCode Code;
        public readonly string Message;
        public readonly int SourceLine;

        public Error(ErrorCode error, string message) {
            this.Code = error;
            this.Message = message;
            this.SourceLine = -1;
        }
        public Error(ErrorCode error, string message, int sourceLine) {
            this.Code = error;
            this.Message = message;
            this.SourceLine = sourceLine;
        }
        public Error(Error error, int sourceLine) {
            this.Code = error.Code;
            this.Message = error.Message;
            this.SourceLine = sourceLine;
        }


        public static Error None;

        public static string Msg_Engine_InvalidState = "(internal error) Invalid state.";

        public static string Msg_ExpectedValue = "Expected value.";
        public static string Msg_ExpectedText = "Expected symbol.";
        public static string Msg_NoTextExpected = "No text expected.";
        public static string Msg_ExpectedCommaOrEndline = "Expected comma or end of line.";


        public static string Msg_NoPreceedingIf = "No preceeding IF.";
        public static string Msg_MissingCondition = "Condition missing.";


        public static string Msg_InstructionUndefined = "The instruction is not defined.";
        public static string Msg_InstructionBadAddressing = "The specified addressing mode is not available.";
        public static string Msg_OpcodeInvalid = "The specified addressing mode or instruction is invalid.";

        public static string Msg_ZeroPageOutOfRange = "Address is out of range for zero-page instruction.";
        public static string Msg_BranchOutOfRange = "Branch target out of range.";

        public static string Msg_BadLine = "Could not parse the line.";
        public static string Msg_InvalidExpression = "Invalid expression.";
        public static string Msg_StringNotAllowed = "The directive does not accept string parameters.";
        public static string Msg_DataStringNotAscii = "Only ASCII characters are allowed in data strings.";
        public static string Msg_OrgLessThanCurrent = ".ORG address can not be less than the current address.";
        public static string Msg_BadNumberFormat = "Invalid number format.";
        public static string Msg_ValueOutOfRange = "Number out of range.";
        public static string Msg_ValueNotDefined_Name = "Value {0} is not defined.";
        public static string Msg_AnonLabelNotFound = "Anonymous label could not be found.";
        public static string Msg_ValueNotByte = "Specified value is not a byte.";
        public static string Msg_ValueNotWord = "Specified value is not a word.";
        public static string Msg_InvalidSymbolName_name = "\"{0}\" is not a valid symbol name.";
        public static string Msg_InvalidPatchOffset = "PATCH offset must be an integer literal, $HEX, 0xHEX, or bank:address literal.";
        public static string Msg_InvalidHexDigit_char = "Character \"{0}\" is not valid in a hex literal.";
        public static string Msg_UnpairedHexDigit = "Hex literal contains an unpaired digit.";

        public static string Msg_MissingCloseParen = "Missing closing parenthesis.";
        public static string Msg_MissingCloseQuote = "String missing closing quote.";
        
        public static string Msg_InvalidDirectiveOption_name_option = "Invalid directive: {0} is not a valid option for {1}";
        public static string Msg_DirectiveUndefined_name = "Directive {0} is not defined.";

        public static string Msg_FileNotFound_name = "File \"{0}\" not found.";
        public static string Msg_CantAccessFile_name = "Can access file {0}.";
        public static string Msg_FileError_name = "Error occurred with file {0}.";

        public static string Msg_EnumError = "Error occurred with ENUM";
        public static string Msg_ValueAlreadyDefined_name = "Symbol {0} is already defined.";

        public bool IsError { get { return Code != ErrorCode.None; } }
    }

    public enum ErrorCode
    {
        /// <summary>No error.</summary>
        None,
        /// <summary>Reached the end of a line or file when more text was expected.</summary>
        End_Of_Text,
        /// <summary>A number was encountered with an invalid format or value.</summary>
        Invalid_Number,
        /// <summary>A value was specified for a directive that was not valid.</summary>
        Invalid_Directive_Value,
        /// <summary>No directive found by specified name.</summary>
        Directive_Not_Defined,
        /// <summary>Encountered text where none was expected.</summary>
        Unexpected_Text,
        /// <summary>A mathematic operation had a result that was out of range.</summary>
        Overflow,
        /// <summary>An assignable variable was expected.</summary>
        Expected_LValue,
        /// <summary>An expression was expected.</summary>
        Expected_Expression,
        /// <summary>An internal error occurred.</summary>
        Engine_Error,
        /// <summary>A closing parethesis was missing from an expression.</summary>
        Expected_Closing_Paren,
        /// <summary>String is missing closing quote.</summary>
        Expected_Closing_Quote,
        /// <summary>An expression contained invalid text.</summary>
        Invalid_Expression,
        /// <summary>An attempt was made to branch to an anonymous label that could not be found.</summary>
        Anonymous_Label_Not_Found,
        /// <summary>Attempted to assign a value to a destination that was not of the same type or size.</summary>
        Type_Mismatch,
        /// <summary>The code contained a syntax error.</summary>
        Syntax_Error,
        /// <summary>An instruction is undefined or invalid.</summary>
        Invalid_Instruction,
        /// <summary>An undefined value was referenced.</summary>
        Value_Not_Defined,
        /// <summary>An error occurred with a file.</summary>
        File_Error,
        /// <summary>A file could not be found.</summary>
        File_Not_Found,
        /// <summary>A .org directive had an invalid address.</summary>
        Invalid_Org,
        /// <summary>Invalid escape sequence.</summary>
        Invalid_Escape,
        /// <summary>A non-ASCII character was encountered.</summary>
        Expected_Ascii,
        /// <summary>ENUM directives were nested.</summary>
        Nested_Enum,
        /// <summary>ENDE or ENDENUM encountered without a preceeding ENUM directive.</summary>
        Extraneous_End_Enum,
        /// <summary>Missing preceeding if.</summary>
        Missing_Preceeding_If,
        /// <summary>An IF or ELSEIF is missing the coniditon.</summary>
        If_Missing_Condition,
        /// <summary>An error was invoked, for example, by the .ERROR directive</summary>
        User_Error,
        /// <summary>An address was out of range, such as $100 for a zero-page instruction.</summary>
        Address_Out_Of_Range,
        /// <summary>A value by the specified name is already define</summary>
        Value_Already_Defined,
    }
}
