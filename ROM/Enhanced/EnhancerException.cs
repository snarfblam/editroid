using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Editroid.ROM.Enhanced
{
    /// <summary>
    /// Thrown when an error occurs while enhancing a ROM.
    /// </summary>
    public class EnhancerException : Exception, ISerializable
    {
        public EnhancerException() { }
        public EnhancerException(string message) : base(message) { }
        public EnhancerException(string message, Exception inner) : base(message, inner) { }
        protected EnhancerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}