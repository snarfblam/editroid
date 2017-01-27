using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    abstract class EditroidAction : EditroidUndoRedoQueue.Action
    {
        public EditroidAction(EditroidUndoRedoQueue q)
            : base() {
            base.Queue = q;
        }

        public new EditroidUndoRedoQueue Queue { get { return (EditroidUndoRedoQueue)base.Queue; } }

        public virtual LevelIndex AffectedLevel { get { return LevelIndex.None; } }
        public virtual int AffectedScreenIndex { get { return -1; } }
        public virtual System.Drawing.Point AffectedMapLocation { get { return nullMapLocation; } }

        public static readonly System.Drawing.Point nullMapLocation = new System.Drawing.Point(-1, -1);

        /// <summary>If possible, combines the two actions into a single action.</summary>
        /// <param name="newerAction">The newer action to try to combine with. If the actions combine, this object should be considered invalid.</param>
        /// <returns>Boolean that indicates whether the action succeeded.</returns>
        /// <remarks>Note for inheriting classes: combining two actions then
        /// performing the resulting action must have the same exact result as
        /// performing the two actions then combining them.</remarks>
        public virtual bool TryCombine(EditroidAction newerAction) {
            return false;
        }

        public virtual bool IsNullAction { get { return false; } }
    }
}
