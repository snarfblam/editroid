using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid
{
    /// <summary>
    /// Manages the automatic creation and destruction of screen editors for a RomViewport
    /// </summary>
    class ScreenEditorManager
    {
        RomViewport host;
        List<ScreenEditor> editors = new List<ScreenEditor>();

        public ScreenEditorManager(RomViewport host) {
            this.host = host;
            host.WorldViewChanged += new EventHandler<EventArgs<System.Drawing.Point>>(on_hostWorldViewChanged);
            CreateAndDestroyEditors();
        }

        void on_hostWorldViewChanged(object sender, EventArgs<System.Drawing.Point> e) {
            CreateAndDestroyEditors();
        }

        public void Paint(Rectangle screenBounds, Graphics g) {
            Rectangle worldBounds = screenBounds;
            worldBounds.X += host.WorldViewport.X;
            worldBounds.Y += host.WorldViewport.Y;

            foreach (ScreenEditor e in editors) {
                if (e.WorldBounds.IntersectsWith(worldBounds)) {
                    Rectangle invalidWorldRect = e.WorldBounds;
                    invalidWorldRect.Intersect(worldBounds);
                    e.Paint(invalidWorldRect, g);
                }
            }
        }
        

        public void PaintScans(RectangleF[] scans, Graphics g) {
            // List of invalid regions, one for each editor
            Rectangle[] invalidWorldRects = new Rectangle[editors.Count];
            Rectangle BlankRect = new Rectangle();
            
            // Invalidate editors
            for (int scanIndex = 0; scanIndex < scans.Length; scanIndex++) {
                Rectangle invalidWorldRect = Rectangle.Truncate(scans[scanIndex]);
                invalidWorldRect.X += host.WorldViewport.X;
                invalidWorldRect.Y += host.WorldViewport.Y;
                invalidWorldRect.Inflate(1, 1);
    
                // Check this invalid scan against each editor's bounds.
                for (int i = 0; i < editors.Count; i++) {
                    ScreenEditor e = editors[i];
                    Rectangle editorWorldRect = e.WorldBounds;

                    // If they overlap, invalidate the editor
                    if (invalidWorldRect.IntersectsWith(editorWorldRect)) {
                        Rectangle invalidClippedWorldRect = editorWorldRect;
                        invalidClippedWorldRect.Intersect(invalidWorldRect);
                    
                        if (invalidWorldRects[i] == BlankRect) {
                            // Initialize the editor with its first invalid rect
                            invalidWorldRects[i] = invalidClippedWorldRect;
                        } else {
                            // Or expand the editor's invalid rect to include this invalid scan.
                            invalidWorldRects[i] = Rectangle.Union(invalidWorldRects[i], invalidWorldRect);
                        }
                    }
                }
            }

            for(int i= 0; i < editors.Count; i++){
                ScreenEditor e = editors[i];
                if (invalidWorldRects[i] != BlankRect) {
                    e.Paint(invalidWorldRects[i], g);

                }
            }
        }

        private void CreateAndDestroyEditors() {
            if (host == null) return;
            Rectangle view = host.WorldViewport;

            // Destroy editors out of view
            for (int i = 0; i < editors.Count; i++) {
                
                ScreenEditor editor = editors[i];

                Rectangle bounds = editor.WorldBounds;
                if (!bounds.IntersectsWith(view) && host.FocusedEditor != editor) {
                    editor.Dispose();
                    editor.SelectionDragged -= new EventHandler(on_EditorSelectionDragged);
                    editor.SelectedObjectChanged -= new EventHandler(on_EditorSelectedObjectChanged);
                    editor.UserDraggedSelection -= new EventHandler<EventArgs<Point>>(newEditor_UserDraggedSelection);
                    editors.RemoveAt(i);
                    i--;
                }
            }

            int gridLeft = view.X / ScreenEditor.CellSize.Width; //Math.Max(view.xTile / ScreenEditor.CellSize.Width, 0);
            if (view.X < 0) gridLeft--;
            int gridRight = view.Right / ScreenEditor.CellSize.Width; //Math.Min(view.Right / ScreenEditor.CellSize.Width, 31);
            int gridTop = view.Y / ScreenEditor.CellSize.Height; //Math.Max(view.yTile / ScreenEditor.CellSize.Height, 0);
            if (view.Y < 0) gridTop--;
            int gridBottom = view.Bottom / ScreenEditor.CellSize.Height;//Math.Min(view.Bottom / ScreenEditor.CellSize.Height, 31);
            for (int x = gridLeft; x <= gridRight ; x++) {
                for (int y = gridTop; y <= gridBottom; y++) {
                    EnsureEditorExists(x, y);
                }
            }
        }

        /// <summary>
        /// Ensures that an editor exists at the specified map location if it is valid.
        /// </summary>
        private void EnsureEditorExists(int x, int y) {
            //if (x < 0 || x > 31 || y < 0 || y > 31) return;

            bool editorPresent = false;
            foreach (ScreenEditor e in editors) {
                if (e.MapLocation.X == x && e.MapLocation.Y == y)
                    editorPresent = true;
            }

            if (!editorPresent) {
                ScreenEditor newEditor = new ScreenEditor();
                newEditor.EditorHost = host;
                newEditor.MapLocation = new Point(x, y);
                newEditor.RenderScreen();
                newEditor.SelectionDragged += new EventHandler(on_EditorSelectionDragged);
                newEditor.SelectedObjectChanged += new EventHandler(on_EditorSelectedObjectChanged);
                newEditor.UserDraggedSelection += new EventHandler<EventArgs<Point>>(newEditor_UserDraggedSelection);
                editors.Add(newEditor);
            }
        }

        void on_EditorRenderComplete(object sender, EventArgs e) {
            OnScreenRendered(((ScreenEditor)sender).MapLocation);
        }

        void newEditor_UserDraggedSelection(object sender, EventArgs<Point> e) {
            OnUserDraggedSelection(e.Value);
        }

        public event EventHandler SelectionChanged;
            void on_EditorSelectedObjectChanged(object sender, EventArgs e) {
            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }


        /// <summary>Occurs when the selected object is dragged to inform the event handler to modify the selected object.</summary>
        public event EventHandler<EventArgs<Point>> UserDraggedSelection;
        /// <summary>
        /// Raises the UserDraggedSelection event.
        /// </summary>
        protected void OnUserDraggedSelection(Point p) { if (UserDraggedSelection != null)UserDraggedSelection(this, new EventArgs<Point>(p)); }
        /// <summary>


        /// <summary>Occurs when the selected object is dragged to inform the event handler to modify the selected object.</summary>
        public event EventHandler<EventArgs<Point>> ScreenRendered;
        /// <summary>
        /// Raises the UserDraggedSelection event.
        /// </summary>
        protected void OnScreenRendered(Point p) { if (ScreenRendered != null)ScreenRendered(this, new EventArgs<Point>(p)); }
        /// <summary>


        public event EventHandler SelectionDragged;
        void on_EditorSelectionDragged(object sender, EventArgs e) {
            if (SelectionDragged != null)
                SelectionDragged(this, new EventArgs());
        }


        public ScreenEditor GetEditorAt(int worldX, int worldY) {
            worldX /= ScreenEditor.CellSize.Width;
            worldY /= ScreenEditor.CellSize.Height;

            EnsureEditorExists(worldX, worldY);

            foreach (ScreenEditor e in editors)
                if (e.MapLocation.X == worldX && e.MapLocation.Y == worldY)
                    return e;

            return null;
        }

        ScreenEditor mouseCapture;
        internal void SendMouseDown(System.Windows.Forms.MouseButtons buttons, int x, int y) {
            if (mouseCapture == null) {
                ScreenEditor mouseEditor = GetEditorAt(x, y);
                if (mouseEditor != null) mouseEditor.OnMouseDown(buttons, x, y);
                mouseCapture = mouseEditor;
            } else {
                mouseCapture.OnMouseDown(buttons, x, y);
            }
        }

        internal void SendMouseMove(System.Windows.Forms.MouseButtons buttons, int x, int y) {
            if (mouseCapture == null) {
                ScreenEditor mouseEditor = GetEditorAt(x, y);
                if (mouseEditor != null) mouseEditor.OnMouseMove(buttons, x, y);
            } else {
                mouseCapture.OnMouseMove(buttons, x, y);
            }
        }

        internal void SendMouseUp(System.Windows.Forms.MouseButtons buttons, int x, int y) {
            if (mouseCapture == null) {
                ScreenEditor mouseEditor = GetEditorAt(x, y);
                if (mouseEditor != null) mouseEditor.OnMouseUp(buttons, x, y);
            } else {
                mouseCapture.OnMouseUp(buttons, x, y);
            }

            mouseCapture = null;

        }

        /// <summary>Redraws all screens with an optional currentLevelIndex and screen count filter.</summary>
        /// <param name="currentLevelIndex">The currentLevelIndex filter, or Invalid for no filter.</param>
        /// <param name="screenIndex">Screen count filter, or -1 for no filter.</param>
        internal void Rerender(LevelIndex level, int screenIndex, bool invalidate) {
            foreach (ScreenEditor e in editors) {
                if ((level == LevelIndex.None || e.LevelIndex == level) && (screenIndex == -1 || screenIndex == e.ScreenIndex)) {
                    e.RefreshData();
                    e.RenderScreen();
                    if (invalidate) host.InvalidateWorld(e.WorldBounds);
                }
            }
        }

        internal void Rerender(Point mapLocation, bool invalidate) {
            foreach (ScreenEditor e in editors) {
                if (e.MapLocation == mapLocation) {
                    e.RefreshData();
                    e.RenderScreen();
                    if (invalidate) host.InvalidateWorld(e.WorldBounds);
                }
            }
        }

        /////// <summary>Redraws all screens with an optional currentLevelIndex and screen count filter.</summary>
        /////// <param name="currentLevelIndex">The currentLevelIndex filter, or Invalid for no filter.</param>
        /////// <param name="screenIndex">Screen count filter, or -1 for no filter.</param>
        ////internal void BeginRerender(LevelIndex level, int screenIndex, bool invalidate) {
        ////    foreach (ScreenEditor e in editors) {
        ////        if ((level == LevelIndex.None || e.LevelIndex == level) && (screenIndex == -1 || screenIndex == e.ScreenIndex)) {
        ////            e.BeginRenderScreen(e.EditorHost.ShowPhysics, invalidate);
        ////        }
        ////    }
        ////}

        ////internal void BeginRerender(Point mapLocation, bool invalidate) {
        ////    foreach (ScreenEditor e in editors) {
        ////        if (e.MapLocation == mapLocation) {
        ////            e.RefreshData();
        ////            e.BeginRenderScreen(e.EditorHost.ShowPhysics, invalidate);
        ////        }
        ////    }
        ////}

        internal void MakeDefaultSelection() {
            host.RequestFocus(editors[0]);   
        }
    }
}
