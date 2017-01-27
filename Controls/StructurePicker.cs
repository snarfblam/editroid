using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Editroid.ROM;
using System.Runtime.InteropServices;

namespace Editroid.Controls
{
    class StructurePicker : Control, IMessageFilter
    {

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        StructRenderer renderer = new StructRenderer();
        Bitmap buffer;
        Graphics gBuffer;

        public int BaseIndex { get; private set; }
        public Level Level { get; private set; }
        public int Palette { get; private set; }
        public int HighlightedIndex { get; private set; }
        public int SelectedIndex { get; private set; }

        bool _UseAlternatePalette;
        public bool UseAlternatePalette {
            get { return _UseAlternatePalette; }
            set {
                if (_UseAlternatePalette != value) {
                    _UseAlternatePalette = value;
                    RenderStructures();
                    Invalidate();
                }
            }
        }

        ScrollBar ScrollBar = new VScrollBar();

        List<int> LevelScrollPositions = new List<int>(5);

        public StructurePicker() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.Selectable | ControlStyles.UserPaint, true);
            InitializeComponent();

            RecreateBuffer(128, 512);

            ScrollBar.Scroll += new ScrollEventHandler(OnScroll);
            ScrollBar.ValueChanged += new EventHandler(ScrollBar_ValueChanged);

            Application.AddMessageFilter(this);
        }



        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            Application.RemoveMessageFilter(this);
        }
        void ScrollBar_ValueChanged(object sender, EventArgs e) {
            ShowStructs(Level, Palette, 0, SelectedIndex);
            
        }

        int scrollDelta;
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            scrollDelta += e.Delta;

            while (scrollDelta >= 120) {
                scrollDelta -= 120;

                PerformPageUp();

            }
            while (scrollDelta <= -120) {
                scrollDelta += 120;

                PerformPageDown();
            }
        }

        private void PerformPageDown() {
            if (Level != null) {
                int currentValue = ScrollBar.Value;
                currentValue += SystemInformation.MouseWheelScrollLines;
                ScrollBarRangeCheck(ref currentValue);

                ScrollBar.Value = currentValue;
            }
        }

        /// <summary>
        /// Returns true and updates the specified value if it is out of the scrollbar's selectable range. Otherwise the return value is false and the specified value is not modified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ScrollBarRangeCheck(ref int value) {
            if (value < 0) {
                value = 0;
                return true;
            }
            if (Level == null) return false;
            if (value > Level.StructCount - ScrollBar.LargeChange + 1) {
                value = Level.StructCount - ScrollBar.LargeChange + 1;
                return true;
            }
            return false;
        }
        private void PerformPageUp() {
            if (Level != null) {
                int currentValue = ScrollBar.Value;
                currentValue -= SystemInformation.MouseWheelScrollLines;
                ScrollBarRangeCheck(ref currentValue);
                

                ScrollBar.Value = currentValue;
            }
        }
        private void RecreateBuffer(int width, int height) {
            var oldBuffer = buffer;
            var gOldBuffer = gBuffer;

            buffer = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gBuffer = Graphics.FromImage(buffer);
            gBuffer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gBuffer.Clear(Color.Black);

            if (oldBuffer != null) oldBuffer.Dispose();
            if (gOldBuffer != null) gOldBuffer.Dispose();
        }

        int oldHeight = 0;
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);

            if (Height > oldHeight && Level != null) {
                // Redraw and invalidate newly displayed area
                RenderStructures();
                Invalidate(new Rectangle(0, oldHeight, Width, Height - oldHeight));
            }

            oldHeight = Height;

            UpdateScrollRange();
        }

        void OnScroll(object sender, ScrollEventArgs e) {
            //ShowStructs(Level, Palette, 0, SelectedIndex);
        }


        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (buffer != null) {
                e.Graphics.DrawImage(buffer, 0, 0);
            }
        }


        void InitializeComponent() {
            SuspendLayout();

            ScrollBar.Dock = DockStyle.Right;
            Controls.Add(ScrollBar);

            ResumeLayout();
        }



        public int ShowStructs(Level l, int pallette, int baseIndex) {
            return ShowStructs(l, pallette, baseIndex, -1);
        }
        public int ShowStructs(Level l, int pallette, int baseIndex, int selectedIndex) {
            SetLevel(l);

            HighlightedIndex = selectedIndex;
            SelectedIndex = -1;

            this.Palette = pallette;
            this.BaseIndex = baseIndex;
            this.HighlightedIndex = selectedIndex;

            RenderStructures();
            Invalidate();
            return SelectedIndex;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && Level != null) {
                int itemIndex = e.Y / structCellSpacingY + ScrollBar.Value;
                int itemY = e.Y % structCellSpacingY;

                if (itemY < structCellHeight && itemIndex >= 0 && itemIndex < Level.Structures.Count) {
                    StructureClicked.Raise(this, new StructureIndexEventArgs(itemIndex));
                }
            }
            if (e.Button == MouseButtons.Right) {
                Palette = (Palette + 1) % 4;
                RenderStructures();
                Invalidate();
            }
        }

        public event EventHandler<StructureIndexEventArgs> StructureClicked;
        public class StructureIndexEventArgs : EventArgs<int>
        {
            public StructureIndexEventArgs(int value) 
            :base(value){
            }

        }
        private void SetLevel(Level l) {
            if (l != Level) {

                this.Level = l;
                {

                }
                // Todo: update selection index/invalidate
                UpdateScrollRange();
                int scrollValue = ScrollBar.Value;
                if (ScrollBarRangeCheck(ref scrollValue)) {
                    ScrollBar.Value = scrollValue;
                }
            }
        }

        private void UpdateScrollRange() {
            ScrollBar.LargeChange = VisibleItemCount;
            ScrollBar.SmallChange = 1;

            if(Level != null){
                ScrollBar.Maximum = Level.Structures.Count;
            }
        }

        const int structTileWidth = 16;
        const int structTileHeight = 16;
        const int structCellWidth_Tiles = 4;
        const int structCellHeight_Tiles = 4;
        const int structCellMargin = 8;
        const int structCellWidth = structTileWidth * structCellWidth_Tiles;
        const int structCellHeight = structTileHeight * structCellHeight_Tiles;
        const int structDisplayOffset = 8;
        const int structCellSpacingX = structCellWidth + structCellMargin;
        const int structCellSpacingY = structCellHeight + structCellMargin;

        public int VisibleItemCount { get { return (Height + structCellSpacingY - 1) / structCellSpacingY; } }

        private void RenderStructures() {
            EnsureBufferSize();


            this.gBuffer.Clear(Color.Black);
            if (Level == null) return;

            int firstVisibleStruct = ScrollBar.Value;
            int lastVisibleStruct = Math.Min(firstVisibleStruct + VisibleItemCount, Level.Structures.Count - 1);
            int visibleStructCount = lastVisibleStruct - firstVisibleStruct + 1;

            for (int i = 0; i < visibleStructCount; i++) {


                Bitmap img;
                bool palApplied = false;

                int structWidth, structHeight;
                img = renderer.RenderSingleStructure(Level, i + firstVisibleStruct, Palette, out structWidth, out structHeight);

                // Palette only needs to be applied once
                if (!palApplied) {
                    var pal = img.Palette;
                    if (UseAlternatePalette) {
                        Level.BgAltPalette.ApplyTable(pal.Entries);
                    } else {
                        Level.BgPalette.ApplyTable(pal.Entries);
                    }
                    img.Palette = pal;

                    palApplied = true;
                }

                Rectangle source = new Rectangle(0, 0, structWidth * structTileWidth, structHeight * structTileHeight);
                Rectangle dest = new Rectangle(structCellMargin, i * structCellSpacingY + structCellMargin, source.Width, source.Height);

                // Structs will be scaled down if larger that 64 px
                if (structWidth > structCellWidth_Tiles || structHeight > structCellHeight_Tiles) {
                    source = new Rectangle(0, 0, structTileWidth * structWidth, structTileHeight * structHeight);

                    float scaleX = (float)structCellWidth_Tiles / (float)structWidth;
                    float scaleY = (float)structCellHeight_Tiles / (float)structHeight;

                    // Whether scale is based on width or height depends on which is greater
                    if (scaleX > scaleY) { // Taller
                        dest = new Rectangle(dest.X, dest.Y, (int)(source.Width * scaleY), (int)(source.Height * scaleY));
                        // center (height - width) adjusted for scale and halved
                        // Note that this assumes the cells are square
                        dest.X += (int)((source.Height - source.Width) * scaleY / 2);
                    } else { // Wider
                        dest = new Rectangle(dest.X, dest.Y, (int)(source.Width * scaleX), (int)(source.Height * scaleX));
                        // center (width - height) adjusted for scale and halved
                        // Note that this assumes the cells are square
                        dest.Y += (int)((source.Width - source.Height) * scaleX / 2);
                    }

                } else {
                    // Otherwise, just center
                    dest.X += (structCellWidth - source.Width) / 2;
                    dest.Y += (structCellHeight - source.Height) / 2;
                }

                // Draw selection
                gBuffer.DrawImage(img, dest, source, GraphicsUnit.Pixel);
                if (i == HighlightedIndex) {
                    var selectionRect = dest;
                    selectionRect.X -= 2;
                    selectionRect.Y -= 2;
                    selectionRect.Width += 4;
                    selectionRect.Height += 4;
                    gBuffer.DrawRectangle(SystemPens.Highlight, selectionRect);
                }

            }



        }

        private void EnsureBufferSize() {
            if (buffer.Height < Height) {
                // At least twice as large
                int newSize = buffer.Height * 2;
                if (newSize < Height) {
                    newSize = Height;
                }


                RecreateBuffer(buffer.Width, newSize);

            }
        }


        private void picStructDisplay_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                // Need to compensate for images being offset and spaced
                int x = e.X - structDisplayOffset;
                int y = e.Y - structDisplayOffset;

                // Index of cell
                int structX = x / structCellSpacingX;
                int structY = y / structCellSpacingY;
                // Position within cell
                int cellX = x % structCellSpacingX;
                int cellY = y % structCellSpacingY;

                bool notInPadding = cellX < structCellWidth && cellY < structCellHeight;
                bool isValidCell = structX >= 0 && structX < 4 && structY >= 0 && structY < 4;
                if (isValidCell & notInPadding) {
                    SelectedIndex = structX + structY * 4 + BaseIndex;

                    throw new NotImplementedException();
                    // Todo: select object type/spawn object (based on settings)
                }
            } else {
                Palette = (Palette + 1) % 4;
                RenderStructures();
                Invalidate();
            }
        }


        bool hasMouse = false;
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);

            hasMouse = true;
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);

            hasMouse = false; 
        }

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m) {
            // Handle mouse scroll events while mouse in within control
            if (hasMouse && m.Msg == 0x20a) {
                // We only need to do this if we don't have focus (otherwise MouseScroll events work just fine)
                if (!Focused) {

                    SendMessage(Handle, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }

            return false;
        }

        #endregion

        internal void FocusOnStructure(int index, int palette) {
            bool needsRedraw = palette != this.Palette;
            this.Palette = palette;


            // Don't scroll if already in view
            if (index < ScrollBar.Value || index >= ScrollBar.Value + VisibleItemCount) {

                int scrollPosition = index - VisibleItemCount / 2;
                ScrollBarRangeCheck(ref scrollPosition);

                // If we don't redraw the control as a side-effect of scrolling, we need to do it manually
                ScrollBar.Value = scrollPosition;
                needsRedraw = false;
            }

            if (needsRedraw) {
                RenderStructures();
                Invalidate();
            }
        }

        internal void ReloadData() {
            // This forces data to be reloaded
            var lvl = Level;
            Level = null;

            int scrollPosition = ScrollBar.Value;
            ShowStructs(lvl, Palette, BaseIndex);
            ScrollBarRangeCheck(ref scrollPosition);
            ScrollBar.Value = scrollPosition;
        }
    }

    class StructRenderer
    {
        Bitmap buffer = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        Editroid.Graphic.Blitter blitter = new Editroid.Graphic.Blitter();

        const int maxStructWidth = 16;
        const int maxStructHeight = 16;
        const int paddingX = 1;
        const int paddingY = 1;
        const int cellWidth = maxStructWidth + paddingX;
        const int cellHeight = maxStructHeight + paddingY;
        const int cellCountX = 3;
        const int cellCountY = 3;

        public StructRenderer() {
            StructsPerRow = 3;
        }

        public int StructsPerRow { get; set; }

        Level rendering_level;
        int rendering_baseIndex;
        int rendering_palette;

        public Bitmap Render(Level level, int baseIndex, int palette) {
            int ignored1, ignored2;

            rendering_level = level;
            rendering_baseIndex = baseIndex;
            rendering_palette = palette;

            blitter.Begin(level.Patterns.PatternImage, buffer);
            blitter.Clear();

            for (int x = 0; x < cellCountX; x++) {
                for (int y = 0; y < cellCountY; y++) {
                    int index = baseIndex + x + y * StructsPerRow;
                    if (index < level.Structures.Count) {
                        RenderStructure(level.Structures[index], x * cellWidth, y * cellHeight, out ignored1, out ignored2);
                    }
                }
            }

            blitter.End();

            return buffer;
        }
        public Bitmap RenderSingleStructure(Level level, int index, int palette, out int width, out int height) {
            rendering_level = level;
            rendering_baseIndex = index;
            rendering_palette = palette;

            blitter.Begin(level.Patterns.PatternImage, buffer);
            blitter.Clear();

            if (index < level.Structures.Count) {
                RenderStructure(level.Structures[index], 0, 0, out width, out height);
            } else {
                throw new ArgumentException("Specified index is out of range.");
            }



            blitter.End();

            return buffer;
        }

        private void RenderStructure(Structure s, int x, int y, out int width, out int height) {
            width = 0;
            height = 0;
            for (int structX = 0; structX < maxStructWidth; structX++) {
                for (int structY = 0; structY < maxStructHeight; structY++) {
                    var dat = s.Data[structX, structY];
                    if (dat != 0xFF) {
                        RenderCombo(dat, x + structX, y + structY);
                        if (structX == width) width = structX + 1; // e.g. structX = 0, width = 0 -> width = 1
                        if (structY == height) height = structY + 1; //    structX = 1, width = 1 -> width = 2 (etc.)
                    }
                }
            }
        }

        private void RenderCombo(int index, int x, int y) {
            var cbo = rendering_level.Combos[index];

            x *= 2;
            y *= 2;
            blitter.BlitTile(cbo[0], x, y, (byte)rendering_palette);
            blitter.BlitTile(cbo[1], x + 1, y, (byte)rendering_palette);
            blitter.BlitTile(cbo[2], x, y + 1, (byte)rendering_palette);
            blitter.BlitTile(cbo[3], x + 1, y + 1, (byte)rendering_palette);
        }
    }
}
