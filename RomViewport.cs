    using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows;
using System.Drawing.Imaging;
using Windows.Enum;

namespace Editroid
{
    class RomViewport: Control, IScreenEditorHost 
    {
        int viewX = 1032;
        int viewY = 480;
        ScreenEditorManager screens;
        ScreenEditor _focus;
        SelectedItemPanel statusPanel;



        public RomViewport() {
            screens = new ScreenEditorManager(this);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);

            statusPanel = new SelectedItemPanel();
            statusPanel.Hide();
            statusPanel.AlternateMusicClicked += new EventHandler(statusPanel_AlternateMusicClicked);
            statusPanel.PasswordIconClicked += new EventHandler(statusPanel_PasswordIconClicked);
            statusPanel.DefaultPalClicked += new EventHandler(statusPanel_DefaultPalClicked);
            screens.SelectionChanged += new EventHandler(screens_SelectionChanged);
            Controls.Add(statusPanel);
        }

        /// <summary>
        /// Raised when the user wants the selected object to be shown in the object selector. (Left-click after selected). Should not
        /// activate a separate dialog.
        /// </summary>
        public event EventHandler RequestShowObject;
        /// <summary>
        /// Raised when the user wants change the palette of the selected object. (Right-click)
        /// </summary>
        public event EventHandler RequestChangePalette;


        void statusPanel_DefaultPalClicked(object sender, EventArgs e) {
            OnDefaultPaldIconClicked();
        }

        void statusPanel_PasswordIconClicked(object sender, EventArgs e) {
            OnPasswordIconClicked();   
        }

        void statusPanel_AlternateMusicClicked(object sender, EventArgs e) {
            Program.PerformAction(Program.Actions.ToggleAlternateMusic(statusPanel.musicButton.State));
        }

        void screens_SelectionChanged(object sender, EventArgs e) {
            UpdateStatus();
        }
        public override bool PreProcessMessage(ref Message msg) {
            var result = base.PreProcessMessage(ref msg);
            return result;

            System.Diagnostics.Debug.Write(msg.ToString());
        }
        protected override bool ProcessKeyMessage(ref Message m) {
            return base.ProcessKeyMessage(ref m);
        }

        public void UpdateStatus() {
            statusPanel.Display(_focus);
        }

        // Hack: instead of external code hiding status manually, it should happen automatically as part of a refresh
        public void HideStatus() {
            statusPanel.Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            //OnKeyDown(new KeyEventArgs(keyData));
            OnCmdKeyPressed(new KeyEventArgs(keyData));

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void OnCmdKeyPressed(KeyEventArgs e) {
            if (CmdKeyPressed != null)
                CmdKeyPressed(this, e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            UpdateCursor();
        }


        public event KeyEventHandler CmdKeyPressed;

        protected override void OnPaint(PaintEventArgs e) {
            screens.Paint(e.ClipRectangle, e.Graphics);
        }
        protected void OnPaintScans(RectangleF[] scans, PaintEventArgs e) {
            screens.PaintScans(scans, e.Graphics);
        }

        protected void OnWorldViewChanged() {
            if (WorldViewChanged != null) WorldViewChanged(this, new EventArgs<Point>(new Point(viewX,viewY)));
            if (map != null) map.SetViewport(this.WorldViewport);
        }

        public ScreenEditorManager Screens { get { return screens; } }

        public void InvalidateWorld(Rectangle worldRect) {
            worldRect.X -= viewX;
            worldRect.Y -= viewY;

            // hack: circumvented windows invalidation because I'm dumb and couldn't figure out how to get it to work properly (force re-paint)
            //Invalidate(worldRect);
            //Update();
            using (Graphics g = this.CreateGraphics()) {
                OnPaint(new PaintEventArgs(g, worldRect));
            }
        }

        private bool showPhysics;

        public bool ShowPhysics {
            get { return showPhysics; }
            set { showPhysics = value; }
        }


        bool isMouseScrolling = false;
        int scrollStartX, scrollStartY;
        Point mouseScrollScreenOrigin;
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            UpdateCursor();

            // If user left clicks and item and it is the item that was previously selected
            // (oldSelection), the UI will be alerted to bring it up the structure selector
            Editroid.ROM.ObjectInstance oldSelection = null;
            if (_focus != null) oldSelection = _focus.SelectedItem;

            if ((ModifierKeys & Keys.Control) == Keys.Control) {
                ScreenEditor clicked = screens.GetEditorAtWorld(e.X + viewX, e.Y + viewY);
                if (clicked != _focus) 
                    RequestFocus(clicked);
                
                PreviewEventArgs args = new PreviewEventArgs(
                    (ModifierKeys & Keys.Shift) == Keys.Shift,
                    new Point(e.X + viewX - _focus.WorldBounds.X, e.Y + viewY - _focus.WorldBounds.Y));

                OnLaunchPreview(args);
            } else {
                // Mouse dragging occurs with middle button, or while holding shift and/or alt
                if (e.Button == MouseButtons.Middle || UserHoldingMouseScrollKey) {
                    BeginDrag(e.X, e.Y);
                } else {
                    screens.SendMouseDown(e.Button, e.X + viewX, e.Y + viewY);

                    if (e.Button == MouseButtons.Left) {
                        // Scroll structure selector to selected structure type if user clicks twice
                        Editroid.ROM.ObjectInstance newSelection = null;
                        if (_focus != null) newSelection = _focus.SelectedItem;
                        if (newSelection != null && newSelection == oldSelection) {
                            RequestShowObject.Raise(this);
                        }
                    
                    }
                }
            }
        }

        public event EventHandler<PreviewEventArgs> LaunchPreview;
        private void OnLaunchPreview(PreviewEventArgs args) {
            if (LaunchPreview != null)
                LaunchPreview(this, args);
        }

        private void BeginDrag(int x, int y) {
            isMouseScrolling = true;
            scrollStartX = x;
            scrollStartY = y;
            mouseScrollScreenOrigin = Cursor.Position;
            UpdateCursor();
            statusPanel.Hide();

        }

        public void UpdateCursor() {
            if (isMouseScrolling || UserHoldingMouseScrollKey) {
                Cursor = Cursors.NoMove2D;
            } else {
                Cursor = Cursors.Default;
            }
        }

        private static bool UserHoldingMouseScrollKey {
            get {
                return (ModifierKeys & (Keys.Shift | Keys.Alt)) != Keys.None;
            }
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            OnWorldViewChanged();
        }
        private bool useAltPalette;

        public bool UseAltPalette {
            get { return useAltPalette; }
            set {
                useAltPalette = value; 
            }
        }
        public bool GetAltPaletteFlag(int mapX, int mapY) {
            return map.GetAltPal(mapX, mapY);
        }
        public int GetAnimation(int mapX, int mapY) {
            return map.GetAnimation(mapX, mapY);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (isMouseScrolling) {
                int dx = e.X - scrollStartX;
                int dy = e.Y - scrollStartY;

                if (dx != 0 || dy != 0) {

                    scrollStartX = e.X;
                    scrollStartY = e.Y;

                    int scaledDx = dx * scrollSpeed;
                    int scaledDy = dy * scrollSpeed;



                    ScrollView(scaledDx, scaledDy);
                    // Next three lines added to keep cursor still while scrolling
                    Cursor.Position = mouseScrollScreenOrigin;
                    scrollStartX -= dx; // Adjust scroll origin to compensate for cursor-repositioning
                    scrollStartY -= dy;
                }
            } else {
                screens.SendMouseMove(e.Button, e.X + viewX, e.Y + viewY);
            }
        }

        static System.Drawing.Drawing2D.Matrix identityMatrix = new System.Drawing.Drawing2D.Matrix();
        public void ScrollScreen(int dx, int dy) {
            Point loc = _focus.WorldBounds.Location;
            loc.Offset(ScreenEditor.CellSize.Width * dx, ScreenEditor.CellSize.Height * dy);
            ScreenEditor newEditor = screens.GetEditorAtWorld(loc.X, loc.Y);
            if (newEditor != null) {// && newEditor.CanEdit) {
                RequestFocus(newEditor);
                ScrollView(-ScreenEditor.CellSize.Width * dx, -ScreenEditor.CellSize.Height * dy);
            }
        }

        public void FocusScreen(int screenX, int screenY) {
            var editor = screens.GetEditorAt(screenX, screenY);
            if (editor != null) RequestFocus(editor);
        }

        public void ScrollView(int dx, int dy) {
            viewX -= dx;
            viewY -= dy;

            OnWorldViewChanged();

            RECT invalid = new RECT();

            HRgn invalidRegion = HRgn.CreateFromRECT(new RECT());
            try { // ensure that invalidRegion is deleted
                RegionType result = ((HWnd)Handle).ScrollWindow(dx, dy, invalidRegion, 0);

                // We will generally use an optimized painting method that avoids 
                // re-drawing most of the valid region by processing a list of invalid
                // rectangles that represent the invalidated region. Yup.
                if (result == Windows.Enum.RegionType.Complex || result == Windows.Enum.RegionType.Simple) {
                    using (Region r = Region.FromHrgn(invalidRegion)) {
                        RectangleF[] scans = r.GetRegionScans(identityMatrix);



                        using (Graphics g = this.CreateGraphics()) {
                            OnPaintScans(scans, new PaintEventArgs(g, invalid));
                        }
                        // Uncomment to highlight each invalid scan
                        //Graphics gr = CreateGraphics();
                        //for (int i = 0; i < scans.Length; i++) {
                        //    var rex = Rectangle.Round(scans[i]);
                        //    rex.Inflate(-1,-1);
                        //    gr.DrawRectangle(Pens.Yellow, rex);
                        //}
                        //gr.Dispose();                    
                    }
                } else {
                    using (Graphics g = this.CreateGraphics()) {
                        OnPaint(new PaintEventArgs(g, invalid));
                    }
                }
            } finally {
                invalidRegion.Delete();
            }
            UpdateStatusVisibilityForDragging();
        }

        private void UpdateStatusVisibilityForDragging() {
            if (shouldShowStatus)
                InitializeFocus();
            else
                statusPanel.Hide();
        }


        private int scrollSpeed = 2;

        public int ScrollSpeed {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            UpdateCursor();

            if (isMouseScrolling) {
                EndDrag();
            } else {
                screens.SendMouseUp(e.Button, e.X + viewX, e.Y + viewY);
            }
        }

        private void EndDrag() {
            isMouseScrolling = false;
            UpdateCursor();
            UpdateStatusVisibilityForDragging();
        }

        private MapControl map;

        public MapControl Map {
            get { return map; }
            set { map = value; }
        }

        bool shouldShowStatus { get { return externalScrollLevel == 0 && !isMouseScrolling; } }

        int externalScrollLevel = 0;
        public void BeginExternalScroll() {
            externalScrollLevel++;
            UpdateStatusVisibilityForDragging();
        }
        public void EndExternalScroll() {
            externalScrollLevel--;
            UpdateStatusVisibilityForDragging();

        }



        public event EventHandler PasswordIconClicked;
        protected void OnPasswordIconClicked() {
            if (PasswordIconClicked != null)
                PasswordIconClicked(this, EventArgs.Empty);
        }
        public event EventHandler DefaultPaletteIconClicked;
        protected void OnDefaultPaldIconClicked() {
            if (DefaultPaletteIconClicked != null)
                DefaultPaletteIconClicked(this, EventArgs.Empty);
        }

        #region IScreenEditorHost Members

        public event EventHandler<EventArgs<System.Drawing.Point>> WorldViewChanged;

        public System.Drawing.Rectangle WorldViewport {
            get { return new Rectangle(viewX, viewY, Width, Height); }
        }

        public void SetViewportLocation(Point location) {
            ScrollView(-(location.X - viewX), -(location.Y - viewY));

        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value;
            OnWorldViewChanged();
            }
        }

        public LevelIndex GetLevel(int mapX, int mapY) {
            if(map == null)
                return LevelIndex.Brinstar;
            return map.GetLevel(mapX, mapY);
        }


        public ScreenEditor FocusedEditor {
            get { return _focus; }
        }

        public void RequestFocus(ScreenEditor editor) {
            if (_focus != null) {
                ScreenEditor oldFocus = _focus;
                _focus = null;
                oldFocus.LoseFocus();
            }

            _focus = editor;

            if (_focus != null) {
                _focus.GetFocus();
                InitializeFocus();
            }
        }

        private void InitializeFocus() {
            if (_focus == null) return;

            if (_focus.CanEdit) {
                PositionStatusPanel();
            }
            statusPanel.Visible = _focus.CanEdit;

        }

        private void PositionStatusPanel() {
            statusPanel.Location = new Point(_focus.WorldBounds.X - viewX, _focus.WorldBounds.Bottom - viewY );
        }
        public void RequestUnfocus(ScreenEditor editor) {
            RequestFocus(null);
         }

        #endregion

        public void RedrawAll() {
             screens.Rerender(LevelIndex.None, -1, false);
             Invalidate();
         }

        public void Redraw(LevelIndex level, int screenIndex) {
            screens.Rerender(level, screenIndex, true);
            if(FocusedEditor != null && (level == LevelIndex.None || (FocusedEditor.LevelIndex == level && (screenIndex == -1 || screenIndex == FocusedEditor.ScreenIndex ))))
                UpdateStatus();
            //Invalidate();
        }
        public void Redraw(LevelIndex level) {
            screens.Rerender(level, -1, true);
            //Invalidate();
        }
        public void Redraw(Point mapLocation) {
            screens.Rerender(mapLocation, true);
            //Invalidate();
            UpdateStatus();
            InitializeFocus();
            


        }

        public Rectangle ConvertWorldToControl(Rectangle rect) {
            rect.Offset(-viewX, -viewY);
            return rect;
        }
        public Rectangle ConvertControlToWorld(Rectangle rect) {
            rect.Offset(viewX, viewY);
            return rect;
        }



     }

    public class PreviewEventArgs : EventArgs
    {
        public PreviewEventArgs(bool options, Point start) {
            this.showOptions = options;
            this.startPosititon = start;
        }

        private bool showOptions;
        public bool ShowOptions {
            get { return showOptions; }
        }

        private Point startPosititon;
        public Point StartPosition {
            get { return startPosititon; }
        }

    }

}
