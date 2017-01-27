using System;
using System.Collections.Generic;
using System.Text;
using Romulus;

    /// <summary>
    /// Provides extension methods for EventHandler classes.
    /// </summary>
    public static class Extension_EventHandler
    {
        /// <summary>
        /// Raises an event.
        /// </summary>
        /// <typeparam name="T">The type of event argument.</typeparam>
        /// <param name="handler">The event handler.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event args.</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args)
            where T : EventArgs {

            EventHandler<T> h = handler;
            if (h != null)
                h(sender, args );
        }
        /// <summary>
        /// Raises an event.
        /// </summary>
        /// <param name="handler">The event handler.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event args.</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void Raise(this EventHandler handler, object sender, EventArgs args) {
            EventHandler h = handler;
            if (h != null)
                h(sender, args ?? EventArgs.Empty);
        }

        /// <summary>
        /// Rasises an event.
        /// </summary>
        /// <param name="handler">The event handler.</param>
        /// <param name="sender">The event sender.</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void Raise(this EventHandler handler, object sender) {
            Raise(handler, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises an event.
        /// </summary>
        /// <typeparam name="T">The type of event argument.</typeparam>
        /// <param name="handler">The event handler.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="arg">The event args.</param>
        [System.Diagnostics.DebuggerStepThrough()]
        public static void Raise<T>(this EventType<T> handler, object sender, T arg) {
            EventArgs<T> eventArgs = new EventArgs<T>(arg);

            EventType<T> h = handler;
            if (h != null)
                h(sender, eventArgs);
        }
    }
