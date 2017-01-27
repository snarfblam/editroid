using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using System.Drawing;
using Editroid.Properties;

namespace Editroid
{
    /////// <summary>
    /////// Provides a windowless UI component to edit an gameItem data entry.
    /////// </summary>
    ////class ItemRowControl_DEPRECATED : WindowlessControl, IDisposable
    ////{
    ////    public const int CellWidth = 16;
    ////    public const int CellHeight = 16;

    ////    ItemRowEntry entry;
    ////    //Level level;
    ////    //int mapY;

    ////    //ItemRowEntry entry { get { return level.ItemTable.GetRow(mapY); } }
    ////    static Bitmap spanImage = Resources.ItemEntryEditorBg;
        
    ////    public ItemRowControl_DEPRECATED(ItemRowEntry e) {
    ////        entry = e;
    ////        //this.level = this.Parent.Rom.Levels[this.Parent.CurrentLevel];


    ////        Height = CellHeight;

    ////        // Populate control
    ////        ItemSeeker seeker = e.Seek();
    ////        do {
    ////            ItemScreenControl datEditor = new ItemScreenControl(seeker, e);
                
    ////            Controls.Add(datEditor);
    ////        } while (seeker.NextScreen());

    ////        // Reposition Control
    ////        Bounds = new Rectangle(0, e.MapY * CellWidth, 512, CellHeight);
    ////        Trim();
    ////    }

    ////    public void Trim() {

    ////    }

    ////    protected override void OnPaint(PaintEventArgs pe) {
    ////        base.OnPaint(pe);

    ////        Rectangle ab = AbsoluteBounds;
    ////        if(Controls.Count > 0){
    ////            ab.X = this.Controls[0].Left;
    ////            ab.Width = this.Controls[Controls.Count - 1].Left + this.Controls[Controls.Count - 1].Width - ab.Left;
    ////        }

    ////        ////Rectangle src = new Rectangle(0,0,spanImage.Width,spanImage.Height);
    ////        ////pe.Graphics.DrawImage(spanImage, ab, src, GraphicsUnit.Pixel);
    ////    }

    ////    private MapItemDisplay mapDisplay;

    ////    public MapItemDisplay MapDisplay {
    ////        get { return mapDisplay; }
    ////        set {
    ////            mapDisplay = value;

    ////            // Each screen needs its data editor to be updated
    ////            foreach (WindowlessControl c in Controls) {
    ////                ItemScreenControl ide = c as ItemScreenControl;
    ////                if (ide != null) ide.MapDisplay = value;
    ////            }
    ////        }
    ////    }


    ////    public ItemRowEntry Entry { get { return entry; } }
    ////    public int MapY {
    ////        get {
    ////            return entry.MapY;
    ////        }
    ////        private set {
    ////            entry.MapY = value;
    ////            Top = value * CellHeight;
    ////        }
    ////    }

    ////    /// <summary>
    ////    /// Ensures that the target position of an ItemScreenControl is valid.
    ////    /// </summary>
    ////    /// <param name="gameItem">The itemDatEditor to check the target position of.</param>
    ////    /// <param name="destX">The target to be checked, and, if needed, modified.</param>
    ////    internal void VerifyItemPosition(ItemScreenControl item, ref int destX) {
    ////        foreach (ItemScreenControl editor in Controls) {
    ////            int position = item.ComparePosition(editor);
    ////            // If an item T is to the left of item B and the user tries to move B to the left of T,
    ////            if (position < 0 && destX <= editor.MapX)
    ////                // Move B one spot right of T.
    ////                destX = editor.MapX + 1;
    ////            // If an item T is to the right of item B and the user tries to move B to the right of T,
    ////            if (position > 0 && destX >= editor.MapX)
    ////                // Move B one spot left of T.
    ////                destX = editor.MapX - 1;
    ////        }
    ////    }

    ////    internal void RequestVerticalMove(int p) {
    ////        int destY = this.Top / CellHeight + p;
    ////        if (destY < 1) destY = 1;
    ////        if (destY > 30) destY = 30;

    ////        foreach (ItemRowControl_DEPRECATED otherRow in Parent.Controls) {
    ////            // We can't move to an occupied row.
    ////            if (otherRow != this && otherRow.MapY == destY) {
    ////                return;
    ////            }
    ////        }

    ////        Program.PerformAction(Program.Actions.MoveItemRow( Parent.Rom.Levels[Parent.CurrentLevel], entry.Seek().ID, destY)); 
    ////    }

    ////    new MapItemDisplay Parent { get { return base.Parent as MapItemDisplay; } }
    ////    #region IDisposable Members

    ////    public override void Dispose() {
    ////        foreach (WindowlessControl c in Controls)
    ////            c.Dispose();

    ////        Controls.Clear();
    ////    }

    ////    #endregion

    ////    internal void NotifyScreenEntryMoved(ItemIndex itemSeekerIndex) {
    ////        foreach (WindowlessControl c in Controls) {
    ////            ItemScreenControl screen = c as ItemScreenControl;
    ////            if (screen != null)
    ////                screen.Reposition();
    ////        }
    ////    }
    ////    internal void NotifyRowMoved(ItemIndex index) {
    ////        this.MapY = Parent.Rom.GetLevel(Parent.CurrentLevel).ItemTable_DEPRECATED.GetRowByIndex(index).MapY;
    ////        //this.mapY = this.MapY = entry.MapY;
    ////    }

    ////    internal void NotifyItemChanged(ItemIndex ID) {
    ////        foreach (WindowlessControl c in Controls) {
    ////            ItemScreenControl screen = c as ItemScreenControl;
    ////            if (screen != null && screen.Item.ID.Composite == ID.Composite)
    ////                screen.ReloadImage();
    ////        }
    ////    }
    ////}

    /// <summary>
    /// Provides a windowless UI component that represents a single screen's data in an gameItem data entry.
    /// </summary>
    class ItemScreenControl : WindowlessControl, IDisposable
    {
        const int CellWidth = 16;
        const int CellHeight = 16;

        ////ItemSeeker item;
        ////ItemRowEntry entry;

        public ItemScreenControl(ItemScreenData data) {
            this.ItemData = data;

            // Initialize
            Bounds = new Rectangle(ItemData.MapX * CellWidth, ItemData.MapY * CellHeight, CellWidth, CellHeight);
            ReloadImage();
        }

        public void ReloadImage() {
            Image = ItemEditTool.GetEditTool(this.ItemData.Items[0]).GetImage(this.ItemData.Items[0]);
        }

        Image Image;


        public ItemScreenData ItemData { get; private set; }

        public int MapX { get { return ItemData.MapX; } }
        public int MapY { get { return ItemData.MapY; } }

        /// <summary>
        /// Resets the position of this control based on the parent's position.
        /// </summary>
        public void Reposition() {
            ////Left = item.MapX * CellWidth;
            ////RowControl.Trim();

            Location = new Point(ItemData.MapX * CellWidth, ItemData.MapY * CellHeight);
        }

        bool dragging;
        int dragX, dragY;
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                dragging = true;
                dragX = e.X / CellWidth;
                dragY = e.Y / CellHeight;
            }

            base.OnMouseDown(e);
        }

        private MapItemDisplay mapDisplay;

        public MapItemDisplay MapDisplay {
            get { return mapDisplay; }
            set { mapDisplay = value; }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);

            Rectangle ab = AbsoluteBounds;
            Rectangle src = ab;
            src.Location = new Point(0, 0);
            if (Image != null)
                pe.Graphics.DrawImage(Image, ab, src, GraphicsUnit.Pixel);
        }


        #region IDisposable Members

        public override void Dispose() {
            this.mapDisplay = null;
            this.ItemData = null;
        }

        #endregion
    }
    /////// <summary>
    /////// Provides a windowless UI component that represents a single screen's data in an gameItem data entry.
    /////// </summary>
    ////class ItemScreenControl : WindowlessControl, IDisposable
    ////{
    ////    const int CellWidth = ItemRowControl_DEPRECATED.CellWidth;
    ////    const int CellHeight = ItemRowControl_DEPRECATED.CellHeight;

    ////    ItemSeeker item;
    ////    ItemRowEntry entry;

    ////    public ItemScreenControl(ItemSeeker item, ItemRowEntry entry) {
    ////        this.item = item;
    ////        this.entry = entry;

    ////        // Initialize
    ////        Bounds = new Rectangle(item.MapX * CellWidth, 0, CellWidth, CellHeight);
    ////        ReloadImage();
    ////    }

    ////    public void ReloadImage() {
    ////        Image = ItemEditTool.GetEditTool(this.item).GetImage(this.item);
    ////    }

    ////    Image Image;

    ////    /// <summary>
    ////    /// Compares the position of currentLevelItems.
    ////    /// </summary>
    ////    /// <param name="d">The gameItem to compare to.</param>
    ////    /// <returns>T negative value if the compared gameItem comes before this gameItem, a positive value if it comes after, or zero if they are at the same position.</returns>
    ////    public int ComparePosition(ItemScreenControl d) {
    ////        if (d.Item.MapX < MapX) return -1;
    ////        if (d.Item.MapX > MapX) return 1;
    ////        return 0;
    ////    }

    ////    public ItemRowControl_DEPRECATED RowControl { get { return this.Parent as ItemRowControl_DEPRECATED; } }
    ////    public ItemSeeker Item { get { return item; } }
    ////    public int MapX {
    ////        get {
    ////            return item.MapX;
    ////        }
    ////        ////set {
    ////        ////    item.MapX = value;
    ////        ////    Reposition();
    ////        ////}
    ////    }

    ////    /// <summary>
    ////    /// Resets the position of this control based on the parent's position.
    ////    /// </summary>
    ////    public void Reposition() {
    ////        Left = item.MapX * CellWidth;
    ////        RowControl.Trim();
    ////    }

    ////    bool dragging;
    ////    int dragX, dragY;
    ////    protected override void OnMouseDown(MouseEventArgs e) {
    ////        if (e.Button == MouseButtons.Left) {
    ////            dragging = true;
    ////            dragX = e.X / CellWidth;
    ////            dragY = e.Y / CellHeight;
    ////        }

    ////        base.OnMouseDown(e);
    ////    }

    ////    private MapItemDisplay mapDisplay;

    ////    public MapItemDisplay MapDisplay {
    ////        get { return mapDisplay; }
    ////        set { mapDisplay = value; }
    ////    }

    ////    protected override void OnPaint(PaintEventArgs pe) {
    ////        base.OnPaint(pe);

    ////        Rectangle ab = AbsoluteBounds;
    ////        Rectangle src = ab;
    ////        src.Location = new Point(0, 0);
    ////        if (Image != null)
    ////            pe.Graphics.DrawImage(Image, ab, src, GraphicsUnit.Pixel);
    ////    }

    ////    // ================================= 
    ////    //
    ////    // On-map item editing is no longer supported
    ////    //
    ////    // =================================

    ////    ////Cursor mapCursor, selectionCursor;
    ////    ////protected override void OnMouseEnter() {
    ////    ////    base.OnMouseEnter();

    ////    ////    if (mapCursor == null) {
    ////    ////        mapCursor = mapDisplay.MapControl.Cursor;
    ////    ////        selectionCursor = mapDisplay.MapControl.SelectionControl.Cursor;
    ////    ////    }

    ////    ////    mapDisplay.MapControl.Cursor = Cursors.Arrow;
    ////    ////    mapDisplay.MapControl.SelectionControl.Cursor = Cursors.Arrow;
    ////    ////}

    ////    ////protected override void OnMouseLeave() {
    ////    ////    base.OnMouseLeave();

    ////    ////    if (mapDisplay == null) return;

    ////    ////    mapDisplay.MapControl.Cursor = mapCursor;
    ////    ////    mapDisplay.MapControl.SelectionControl.Cursor = selectionCursor;

    ////    ////}

    ////    ////protected override void OnMouseMove(MouseEventArgs e) {
    ////    ////    if (dragging && RowControl != null) {
    ////    ////        int destX = MapX + e.X / CellWidth;
    ////    ////        if (e.X < 0) destX--; // Negative division rounds up instead of down
    ////    ////        if (destX < 1) destX = 1; if (destX > 30) destX = 30;
    ////    ////        int destY = e.Y / CellHeight;
    ////    ////        if (e.Y < 0) destY--; // Negative division rounds up instead of down

    ////    ////        bool dragHorizontal = destX != MapX;
    ////    ////        bool dragVertical = destY != 0;

    ////    ////        if (dragHorizontal) {
    ////    ////            RowControl.VerifyItemPosition(this, ref destX);
    ////    ////            Program.MainForm.PerformAction(
    ////    ////                Program.MainForm.Actions.MoveItemTile(((MapItemDisplay)(this.Parent.Parent)).CurrentLevel, this.item.ID, destX)
    ////    ////            );
    ////    ////        }
    ////    ////        if (dragVertical) {
    ////    ////            RowControl.RequestVerticalMove(destY);
    ////    ////        }
    ////    ////    }
    ////    ////    base.OnMouseMove(e);
    ////    ////}


    ////    ////protected override void OnMouseUp(MouseEventArgs e) {
    ////    ////    if (e.Button == MouseButtons.Left)
    ////    ////        dragging = false;

    ////    ////    base.OnMouseUp(e);
    ////    ////}

    ////    bool Leftmost {
    ////        get {
    ////            return this == Parent.Controls[0];
    ////        }
    ////    }



    ////    #region IDisposable Members

    ////    public override void Dispose() {
    ////        this.mapDisplay = null;
    ////        this.entry = null;
    ////    }

    ////    #endregion
    ////}
}
