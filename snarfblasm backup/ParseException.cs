using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace snarfblasm
{
    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public ParseException(string message, int lineNumber) : base(message) { this.LineNumber = lineNumber; }

        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public ParseException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public ParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public int LineNumber { get; private set; }
    }

    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class SyntaxErrorException : ParseException
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public SyntaxErrorException(string message, int lineNumber) : base(message, lineNumber) { }
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public SyntaxErrorException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public SyntaxErrorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class UnknownIdentifierException : ParseException
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public UnknownIdentifierException(string message, int lineNumber) : base(message, lineNumber) { }
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public UnknownIdentifierException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public UnknownIdentifierException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class UnknownDirectiveException : ParseException
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public UnknownDirectiveException(string message, int lineNumber) : base(message, lineNumber) { }
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public UnknownDirectiveException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public UnknownDirectiveException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class OperandOutOfRangeException : ParseException
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public OperandOutOfRangeException(string message, int lineNumber) : base(message, lineNumber) { }
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public OperandOutOfRangeException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public OperandOutOfRangeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// Thrown when there is an error parsing code.
    /// </summary>
    public class InstructionException : ParseException
    {
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        public InstructionException(string message, int lineNumber) : base(message, lineNumber) { }
        /// <summary>Creates the exception.</summary>
        /// <param name="message">The message that describes this error.</param>
        /// <param name="inner">The exception that resulted in this exception.</param>
        public InstructionException(string message, Exception inner) : base(message, inner) { }

        /// <summary>Deserializes exception.</summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public InstructionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
