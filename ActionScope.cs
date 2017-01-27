using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Editroid
{
    /// <summary>
    /// Encapsulates an action and provides a mechanism to complete the action.
    /// </summary>
    /// <remarks>IDisposable.Dispose is equivalent to calling End(). Calling either method 
    /// more than once has no effect.</remarks>
    public class ActionScope:IDisposable
    {
        Delegate endAction;
        IEnumerable endActionArgs = emptyArgs;
        static object[] emptyArgs = new object[0];

        public ActionScope(Delegate endAction) {
            this.endAction = endAction;
        }
        public ActionScope(Delegate endAction, ICollection endActionArgs) {
            this.endAction = endAction;
            if(endActionArgs != null)
                this.endActionArgs = endActionArgs;
        }

        bool complete = false;
        public void Dispose() {
            End();
        }

        /// <summary>
        /// Completes the action, performing any needed cleanup or finalization.
        /// </summary>
        public virtual void End() {
            if (!complete) {
                complete = true;

                List<Object> objs = new List<object>();
                foreach (object o in endActionArgs) {
                    objs.Add(o);
                }

                if(endAction != null)
                    endAction.DynamicInvoke(objs.ToArray());
            }
        }
    }

    /// <summary>
    /// Provides an interface suitable for objects that only perform one
    /// scoped action (which may be recursive). Classes that implement 
    /// this interface can use the
    /// SimpleActionScope class to encapsulate the scope of their action.
    /// </summary>
    public interface IActionScoped {
        /// <summary>
        /// Completes or finalizes an action.
        /// </summary>
        void EndAction();
    }

    /// <summary>
    /// Encapsulates the scope of an action for a class that implements IActionScoped.
    /// </summary>
    public class SimpleActionScope: ActionScope
    {
        public SimpleActionScope(IActionScoped actionScopeSource)
            : base((Delegate)(Action)delegate {
            actionScopeSource.EndAction();
        }) {
            
        }
        delegate void Action();
        
    }
}
