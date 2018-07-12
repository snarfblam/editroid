using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    struct BinaryExpressionOperator
    {
        public readonly string Symbol;
        public readonly ExpressionResultType ResultType;
        public readonly bool CanOverflow;
        public readonly SignedOperatonMode SignedOperands;
        public readonly BinaryOperatorDelegate Operation;

        public BinaryExpressionOperator(string symbol, ExpressionResultType result, bool canOverflow, SignedOperatonMode signage, BinaryOperatorDelegate code) {
            this.Symbol = symbol;
            this.ResultType = result;
            this.CanOverflow = canOverflow;
            this.SignedOperands = signage;
            this.Operation = code;
        }


        public const int trueValue = -1;
        public const int falseValue = 0;

        /// <summary>
        /// Lists the symbols for binary operators. This list is sorted so that 
        /// searching for items with lower indecies results in greedy searches,
        /// which is necessary, for example, to avoid grabbing only the 
        /// first '&' in "&&"
        /// </summary>
        public static string[] OperatorSymbols = { "&&", "||", "^^", "<<", ">>", "*", "/", "%", "+", "-", "==", "!=", "<>", "<=", ">=", "<", ">", "&", "|", "^" };


        public static BinaryExpressionOperator[] BinaryOperators = {
            new BinaryExpressionOperator("&",
                ExpressionResultType.Widest,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return a & b;
                }),
            new BinaryExpressionOperator("|",
                ExpressionResultType.Widest,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return a | b;
                }),
            new BinaryExpressionOperator("^",
                ExpressionResultType.Widest,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return a ^ b;
                }),
            };
        public static BinaryExpressionOperator[] BitShiftOperators = {
            new BinaryExpressionOperator("<<",
                ExpressionResultType.LHand,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return a << b;
                }),
            new BinaryExpressionOperator(">>",
                ExpressionResultType.LHand,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return a >> b;
                }),
            };
        public static BinaryExpressionOperator[] MultiplicationOperators = {
            new BinaryExpressionOperator("*",
                ExpressionResultType.Widest,
                true,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return a * b;
                }),
            new BinaryExpressionOperator("/",
                ExpressionResultType.Widest,
                true,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return a / b;
                }),
            new BinaryExpressionOperator("%",
                ExpressionResultType.Widest,
                true,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return a % b;
                }),
            };
        public static BinaryExpressionOperator[] AdditiveOperators = {
            new BinaryExpressionOperator("+",
                ExpressionResultType.Widest,
                true,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return a + b;
                }),
            new BinaryExpressionOperator("-",
                ExpressionResultType.Widest,
                true,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return a - b;
                }),
            };
        public static BinaryExpressionOperator[] ComparisonOperators = {
            new BinaryExpressionOperator("==",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue(a == b);
                }),
            new BinaryExpressionOperator("!=",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue(a != b); 
                }),
            new BinaryExpressionOperator("<>",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue(a != b); 
                }),
            new BinaryExpressionOperator("<=",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return boolValue(a <= b); 
                }),
            new BinaryExpressionOperator(">=",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return boolValue(a >= b); 
                }),
            new BinaryExpressionOperator("<",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return boolValue(a < b); 
                }),
            new BinaryExpressionOperator(">",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.Full,
                delegate(int a, int b){
                    return boolValue(a > b); 
                }),
        };

        public static BinaryExpressionOperator[] BooleanOperators = {
            new BinaryExpressionOperator("&&",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue((a != 0) && (b != 0)); 
                }),
            new BinaryExpressionOperator("||",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue((a != 0) || (b != 0));
                }),
            new BinaryExpressionOperator("^^",
                ExpressionResultType.Byte,
                false,
                SignedOperatonMode.None,
                delegate(int a, int b){
                    return boolValue((a != 0) ^ (b != 0));
                }),
        };

        // This needs to come after above definitions because static field initializers are evaluated in order of apparance
        public static readonly BinaryExpressionOperator[][] Operators = {
            BooleanOperators,           // &  |  ^
            ComparisonOperators,        // == != <> <  >  <= >=
            AdditiveOperators,          // +  -
            MultiplicationOperators,    // *  /  %
            BitShiftOperators,          // << >>
            BinaryOperators             // && || ^^
        };

        static int boolValue(bool value) {
            return value ? trueValue : falseValue;
        }
    }


    enum ExpressionResultType
    {
        Widest,
        LHand,
        RHand,
        Byte,
        Word
    }
    enum SignedOperatonMode
    {
        None,
        Full,
        LHand,
        RHand
    }

    struct UnaryExpressionOperator
    {
        public readonly string Symbol;
        public readonly ExpressionResultType ResultType;
        public readonly bool CanOverflow;
        public readonly bool Signed;
        public readonly UnaryOperation OperationType;
        public readonly UnaryOperatorDelegate Operation;

        public UnaryExpressionOperator(string symbol, ExpressionResultType result, bool canOverflow, bool signed,UnaryOperation type, UnaryOperatorDelegate code) {
            this.Symbol = symbol;
            this.ResultType = result;
            this.CanOverflow = canOverflow;
            this.Signed = signed;
            this.OperationType = type;
            this.Operation = code;
        }


        public const int trueValue = -1;
        public const int falseValue = 0;

        public static UnaryExpressionOperator[] PreOperators = {
            new UnaryExpressionOperator("-",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Normal,
                delegate(int a){
                    return -a;
                }),
            new UnaryExpressionOperator("+",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Normal,
                delegate(int a){
                    return a;
                }),
            new UnaryExpressionOperator("~",
                ExpressionResultType.Widest,
                false,false,
                UnaryOperation.Normal,
                delegate(int a){
                    return ~a;
                }),
            new UnaryExpressionOperator("!",
                ExpressionResultType.Byte,
                false,false,
                UnaryOperation.Normal,
                delegate(int a){
                    return boolValue(a == 0);
                }),
            new UnaryExpressionOperator("++",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Modifing,
                delegate(int a){
                    return a + 1;
                }),           
            new UnaryExpressionOperator("--",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Modifing,
                delegate(int a){
                    return a - 1;
                }),
            new UnaryExpressionOperator("<>",
                ExpressionResultType.Word,
                false,false,
                UnaryOperation.Normal,
                delegate(int a){
                    return a;
                }),
            new UnaryExpressionOperator("<",
                ExpressionResultType.Byte,
                false,false,
                UnaryOperation.Normal,
                delegate(int a){
                    return a & 0xFF;
                }),
            new UnaryExpressionOperator(">",
                ExpressionResultType.Byte,
                false,false,
                UnaryOperation.Normal,
                delegate(int a){
                    return (a >> 8) & 0xFF;
                }),
        };
        public static UnaryExpressionOperator[] PostOperators = {

            new UnaryExpressionOperator("++",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Modifing,
                delegate(int a){
                    return a + 1;
                }),           
            new UnaryExpressionOperator("--",
                ExpressionResultType.Widest,
                true,true,
                UnaryOperation.Modifing,
                delegate(int a){
                    return a - 1;
                }),
          
        };
        static int boolValue(bool value) {
            return value ? trueValue : falseValue;
        }
    }

    public delegate int BinaryOperatorDelegate(int a, int b);
    public delegate int UnaryOperatorDelegate(int a);
    public enum UnaryOperation
    {
        /// <summary>Modifies a value.</summary>
        Normal,
        /// <summary>Modifies a variable (requires an l-value).</summary>
        Modifing
    }

}
