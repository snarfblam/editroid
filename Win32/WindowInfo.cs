using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public struct WindowInfo
    {
        public uint Size;
        public RECT Window;
        public RECT Client;
        public WindowStyle Style;
        public WindowStyleEx ExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;
    }

}
