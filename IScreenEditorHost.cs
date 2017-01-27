using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid
{
    public interface IScreenEditorHost
    {
        event EventHandler<EventArgs<Point>> WorldViewChanged;
        Rectangle WorldViewport { get;}
        MetroidRom Rom { get;}
        LevelIndex GetLevel(int mapX, int mapY);
        void InvalidateWorld(Rectangle worldRect);
        ScreenEditor FocusedEditor { get;}
        void RequestFocus(ScreenEditor editor);
        void RequestUnfocus(ScreenEditor editor);

        bool ShowPhysics{get;}
        bool UseAltPalette { get;}
    }

    public class EventArgs<T>: EventArgs 
    {
        private T value;

        public T Value {
            get { return value; }
            set { value = value; }
        }

        public EventArgs(T value) {
            this.value = value;
        }
    }
}
