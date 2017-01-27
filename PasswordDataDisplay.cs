using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.Drawing;
using System.Windows.Forms;
using Editroid.UndoRedo;
using Editroid.Actions;

namespace Editroid
{
    public class PasswordDataDisplay: WindowlessControl
    {
        public PasswordDataDisplay() {
            Bounds = new System.Drawing.Rectangle(0, 0, 512, 512);
            CreatePasswordDataControls();
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;
                if (value != null)
                    InitializeControls();
            }
        }

        bool controlsInstantiated;
        private void CreatePasswordDataControls() {
            if (!controlsInstantiated) {
                for (int i = 0; i < PasswordData.DataCount; i++) {
                    PasswordEntryControl c = new PasswordEntryControl();
                    c.Index = i;
                    Controls.Add(c);
                }
                controlsInstantiated = true;

                selection = (PasswordEntryControl)Controls[0];
            }
        }

        private void InitializeControls() {
            for (int i = 0; i < PasswordData.DataCount; i++) {
                ((PasswordEntryControl)Controls[i]).Data = rom.PasswordData.GetDatum(i);
            }
        }

        private PasswordEntryControl selection;

        /// <summary>
        /// Gets selection/sets selected control and raises the EntrySelected event.
        /// </summary>
        internal PasswordEntryControl Selection {
            get { return selection; }
            set {
                SetSelection(value);

                if(value != null)
                    OnEntrySelected();
            }
        }

        private void SetSelection(PasswordEntryControl value) {
            PasswordEntryControl oldSelection = selection;
            selection = value;

            if (oldSelection != null)
                oldSelection.Invalidate();
            if (value != null)
                value.Invalidate();
        }

        public int SelectedIndex {
            get {
                if (selection == null)
                    return -1;
                return selection.Index;
            }
            set {
                if (value < 0 || value >= Controls.Count)
                    SetSelection(null);
                else
                    SetSelection(Controls[value] as PasswordEntryControl);
            }
        }
        protected virtual void OnEntrySelected() {
            if (EntrySelected != null)
                EntrySelected(this, new EventArgs());
        }
        public event EventHandler EntrySelected;

        public new MapControl Host { get { return base.Host as MapControl; } set { base.Host = value; } }


        internal void NotifyActionOccurred(EditroidAction a) {
            PasswordDataAction action = a as PasswordDataAction;
            OverwritePasswordData ow = a as OverwritePasswordData;
            ////SetItemTilePosition t = a as SetItemTilePosition;
            ////SetItemRowPosition rowAction = a as SetItemRowPosition;

            if (action != null) {
                foreach (WindowlessControl c in Controls) {
                    PasswordEntryControl pc = c as PasswordEntryControl;
                    if (pc != null) {
                        if (pc.Index == action.DataIndex)
                            pc.UpdateBounds();
                    }
                }
            } else if (ow != null) {
                SuspendPaint();
                foreach (WindowlessControl c in Controls) {
                    PasswordEntryControl pc = c as PasswordEntryControl;
                    if (pc != null) {
                        pc.UpdateBounds();
                    }
                }
                ResumePaint();
                ////} else if (t != null && t.UpdatesPassword) {
                ////    ((PasswordEntryControl)Controls[t.PasswordDataIndex]).UpdateBounds();
                ////} else if (rowAction != null) {
                ////    rowAction.ForEachPasswordEntry(delegate(int index) {
                ////        PasswordEntryControl c = Controls[index] as PasswordEntryControl;
                ////        if (c != null) c.UpdateBounds();
                ////    });
            }
        }
        /// <summary>
        /// If a control is present that represents the specified map location, it
        /// will be selected. Otherwise this method fails silently.
        /// </summary>
        internal void SelectDatAt(Point mapLocation) {
            foreach (WindowlessControl c in Controls) {
                var datControl = c as PasswordEntryControl;
                if (datControl != null) {
                    if (datControl.Data.MapX == mapLocation.X && datControl.Data.MapY == mapLocation.Y) {
                        Selection = datControl;
                        return;
                    }
                }
            }
        }
    }



    /// <summary>
    /// Represents one piece of password data on the map.
    /// </summary>
    public class PasswordEntryControl : WindowlessControl 
    {
        const int TileSize = 16;
        public PasswordEntryControl() {
            Bounds = new System.Drawing.Rectangle(0, 0, TileSize, TileSize);
        }

        private PasswordDatum data;
        /// <summary>
        /// Gets/sets the data this control will represent
        /// </summary>
        public PasswordDatum Data {
            get { return data; }
            set {
                data = value;
                UpdateBounds();
            }
        }

        public void UpdateBounds() {
            this.Bounds = new System.Drawing.Rectangle(data.MapX * TileSize, data.MapY * TileSize, TileSize, TileSize);
        }

        private int index;
        /// <summary>
        /// Gets/sets an index that can be used to identify this control.
        /// </summary>
        public int Index {
            get { return index; }
            set { index = value; }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe) {
            base.OnPaint(pe);

            Rectangle ab = AbsoluteBounds;
            ab.Width -= 1;
            ab.Height -= 1;

            if (Parent.Selection == this)
                pe.Graphics.FillRectangle(Brushes.White, ab);
            pe.Graphics.DrawRectangle(Pens.Yellow, ab);
        }

        bool dragging = false;
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
            base.OnMouseDown(e);

            //PasswordDatum d = Parent.Selection.Data;
            //if (d.MapX != data.MapX || d.MapY != data.MapY || Parent.Selection == this) {
                Parent.Selection = this;
                if (e.Button == MouseButtons.Left) {
                    dragging = true;
                //}
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            dragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (dragging) {
                int dx = 0;
                int dy = 0;

                if (e.X > Width)
                    dx = e.X / Width;
                else if (e.X < 0)
                    dx = (e.X / Width) - 1; 
                if (e.Y > Height)
                    dy = e.Y / Height;
                else if (e.Y < 0)
                    dy = (e.Y / Height) - 1;

                if (dx != 0 || dy != 0) {
                    int newX = Left / TileSize + dx;
                    int newY = Top / TileSize + dy;
                    if (newX < 0) newX = 0;
                    if (newY < 0) newY = 0;
                    if (newX > 31) newX = 31;
                    if (newY > 31) newY = 31;

                    Program.PerformAction(
                        Program.Actions.SetPassDataMapLocation(index, newX, newY)
                    );
                }
            }
        }

        protected new PasswordDataDisplay Parent { get { return base.Parent as PasswordDataDisplay; } }
        protected MetroidRom Rom { get { return Parent.Rom; } }

        Cursor mapCursor, selectionCursor;
        protected override void OnMouseEnter() {
            base.OnMouseEnter();

            if (mapCursor == null) {
                mapCursor = Parent.Host.Cursor;
                selectionCursor = Parent.Host.SelectionControl.Cursor;
            }

            Parent.Host.Cursor = Cursors.Arrow;
            Parent.Host.SelectionControl.Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeave() {
            base.OnMouseLeave();

            if (Parent == null || Parent.Host == null) return;

            Parent.Host.Cursor = mapCursor;
            Parent.Host.SelectionControl.Cursor = selectionCursor;

        }



    }
}
