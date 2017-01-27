using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    abstract class LevelAction: EditroidAction 
    {
        public override LevelIndex AffectedLevel { get { return Level.Index; } }

        public LevelAction(EditroidUndoRedoQueue q, Level level) 
        :base(q){
            this.Level = level;
        }

        //public Level Level { get { return Queue.Rom.GetLevel(affectedLevel); } }
        public Level Level { get; private set; }
    }

    
}
