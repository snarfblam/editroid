using System;
using System.Collections.Generic;
using System.Text;

namespace Romulus
{
    /// <summary>
    /// Defines an event args class with a generic parameter.
    /// </summary>
    /// <typeparam name="T">The type for the generic parameter.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        private T value;

        /// <summary>
        /// Gets the value associated with this event.
        /// </summary>
        public T Value {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Creates a new EventArgs object.
        /// </summary>
        /// <param name="value">The value associated with an event.</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public EventArgs(T value) {
            this.value = value;
        }
    }

    public delegate void EventType<T> (object sender, EventArgs<T> args);
}
