using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    internal class GlobalEventManager
    {
        public event EventHandler<LevelEventArgs> PaletteChanged;
        public event EventHandler<LevelEventArgs> StructureChanged;
        public event EventHandler<ComboEventArgs> ComboChanged;
        public event EventHandler<ScreenObjectEventArgs> ObjectSelected;
        public event EventHandler<ScreenObjectEventArgs> ObjectEdited;

        public void OnPaletteChanged(LevelIndex level) {
            if(PaletteChanged != null)
                PaletteChanged(this, new LevelEventArgs(level));
        }
        public void OnStructureChanged(LevelIndex level) {
            if(StructureChanged != null)
                StructureChanged(this, new LevelEventArgs(level));
        }
        public void OnComboChanged(LevelIndex level, int ComboIndex) {
            if(ComboChanged != null)
                ComboChanged(this, new ComboEventArgs(level, ComboIndex));
        }
        public void OnObjectSelected(Control sender, LevelIndex level, ObjectInstance item) {
            if(ObjectSelected != null)
                ObjectSelected(sender, new ScreenObjectEventArgs(level, item));
        }
        public void OnObjectEdited(Control sender, LevelIndex level, ObjectInstance item) {
            if(ObjectEdited != null)
                ObjectEdited(sender, new ScreenObjectEventArgs(level, item));
        }

        private GlobalEventManager(){}
        static GlobalEventManager manager = new GlobalEventManager();
        public static GlobalEventManager Manager { get { return manager; } }

    }

    /// <summary>Provides information for an event pertaining to a currentLevelIndex.</summary>
    public class LevelEventArgs: EventArgs
    {
        LevelIndex level;
        /// <summary>The currentLevelIndex for which the event occurred.</summary>
        public LevelIndex Level { get { return level; } }
        /// <summary>Instantiates this class.</summary>
        public LevelEventArgs(LevelIndex level) {
            this.level = level;
        }
    }

    /// <summary>Provides information for an event pertaining to a combo.</summary>
    public class ComboEventArgs:LevelEventArgs
    {
        int comboIndex;
        /// <summary>The count of the combo for which this event occurred.</summary>
        public int ComboIndex { get { return ComboIndex; } }
        /// <summary>Instantiates this class.</summary>
        public ComboEventArgs(LevelIndex level, int comboIndex)
            : base(level) {
            this.comboIndex = comboIndex;
        }
    }

    /// <summary>Provides information for an event pertaining to a screen object.</summary>
    public class ScreenObjectEventArgs:LevelEventArgs
    {
        ObjectInstance item;
        /// <summary>The gameItem this event pertains to.</summary>
        public ObjectInstance Item { get { return item; } }
        /// <summary></summary>
        /// <param name="currentLevelIndex"></param>
        /// <param name="gameItem"></param>
        public ScreenObjectEventArgs(LevelIndex level, ObjectInstance item)
            : base(level) {
            this.item = item;
        }
    }
}
