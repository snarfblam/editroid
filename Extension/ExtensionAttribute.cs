using System;
using System.Collections.Generic;
using System.Text;

    
    namespace System.Runtime.CompilerServices
    {
        /// <summary>
        /// Needed to enable extension methods for the DotNet framework 2.0.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public class ExtensionAttribute : Attribute
        {
        }
    }
