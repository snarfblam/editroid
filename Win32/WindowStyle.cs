using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    [Flags]
    public enum WindowStyle : uint
    {
        ActiveCaption = 0x1,
        Border = 0x800000,
        Caption = 0xC00000,
        Child = 0x40000000,
        ChildWindow = Child,
        ClipChildren = 0x2000000,
        ClipSiblings = 0x4000000,
        Disabled = 0x8000000,
        DialogFrame = 0x400000,
        Group = 0x20000,
        WS_GT = Group | TabStop,
        HScroll = 0x100000,
        Iconic = Minimize,
        Maximize = 0x1000000,
        MaximizeBox = 0x10000,
        Minimize = 0x20000000,
        MinimizeBox = 0x20000,
        OverlappedWindow = Overlapped |Caption |SystemMenu |ThickFrame |MinimizeBox |MaximizeBox,
        Overlapped = 0x0,
        Popup = 0x80000000,
        PopupWindow = (Popup | Border | SystemMenu),
        SizeBox = ThickFrame,
        SystemMenu = 0x80000,
        TabStop = 0x10000,
        ThickFrame = 0x40000,
        Tiled = Overlapped,
        TiledWindow = OverlappedWindow,
        Visible = 0x10000000,
        VScroll = 0x200000,
    }

    [Flags]
    public enum WindowStyleEx : uint
    {

        AcceptFiles = 0x10,
        AppWindow = 0x40000,
        ClientEdge = 0x200,
        ContextHelp = 0x400,
        ControlParent = 0x10000,
        DialogModalFrame = 0x1,
        Layered = 0x80000,
        LayoutRtl = 0x400000,
        Left = 0x0,
        LeftScrollbar = 0x4000,
        LtrReading = 0x0,
        MdiChild = 0x40,
        NoActivate = 0x8000000,
        NoInheritLayout = 0x100000,
        NoParentNotify = 0x4,
        OverlappedWindow = (WindowEdge | ClientEdge),
        PaletteWindow = (WindowEdge | ToolWindow | TopMost),
        Right = 0x1000,
        RightScrollBar = 0x0,
        RtlReading = 0x2000,
        StaticEdge = 0x20000,
        ToolWindow = 0x80,
        TopMost = 0x8,
        Transparent = 0x20,
        WindowEdge = 0x100
    }

}
