////using System;
////using System.Collections.Generic;
////using System.Text;
////using System.Windows.Forms;
////using Editroid.ROM;
////using System.Drawing;
////using Editroid.Properties;

////namespace Editroid
////{
////    /// <summary>
////    /// Provides a UI component to edit an gameItem data entry.
////    /// </summary>
////    class ItemEntryEditor: Panel
////    {
////        ItemRowEntry entry;

////        public ItemEntryEditor(ItemRowEntry e) {
////            entry = e;

////            // Initialize control
////            BackColor = Color.Transparent; //Color.FromArgb(0x60, Color.White);
////            BackgroundImage = Resources.ItemEntryEditorBg;
////            Height = 8;

////            // Populate control
////            ItemSeeker seeker = e.Seek();
////            do {
////                ////ItemEntryScreenTile datEditor = new ItemEntryScreenTile(seeker, e);
////                ////Controls.Add(datEditor);
////            } while(seeker.NextScreen());

////            // Reposition Control
////            Top = e.MapY * 8;
////            Trim();
////        }

////        public void Trim() {
////            int targetLeft = ((Controls[0] as ItemEntryScreenTile).MapX) * 8;
////            bool resetLefts = targetLeft != Left;
////            int targetWidth = 8;

////            if(resetLefts)
////                this.Left = targetLeft;

////            foreach(Control c in Controls) {
////                if(resetLefts)
////                    ((ItemEntryScreenTile)c).ResetLeft();
////                if(c.Right > targetWidth)
////                    targetWidth = c.Right;
////            }

////            if(targetWidth != this.Width)
////                this.Width = targetWidth;


////            //int x = 8;
////            //foreach(Control c in Controls) {
////            //    ((ItemScreenControl)c).Reposition();
////            //    if(c.Right > x)
////            //        x = c.Right;
////            //}

////            //Width = x;
////        }


////        private ItemDataEditor dataEditor;

////        public ItemDataEditor DataEditor {
////            get { return dataEditor; }
////            set {
////                dataEditor = value;
////                foreach (Control c in Controls) {
////                    ItemEntryScreenTile ide = c as ItemEntryScreenTile;
////                    if (ide != null) ide.DataEditor = value;
////                }
////            }
////        }


////        public ItemRowEntry Entry { get { return entry; } }
////        public int MapY {
////            get {
////                return entry.MapY;
////            }
////            set{
////                entry.MapY = value;
////                Top = value * 8;
////            }
////        }

////        /// <summary>
////        /// Ensures that the target position of an ItemScreenControl is valid.
////        /// </summary>
////        /// <param name="gameItem">The itemDatEditor to check the target position of.</param>
////        /// <param name="destX">The target to be checked, and, if needed, modified.</param>
////        internal void VerifyItemPosition(ItemEntryScreenTile item, ref int destX) {
////            foreach(ItemEntryScreenTile editor in Controls) {
////                int position = item.ComparePosition(editor);
////                // If an gameItem T is to the left of gameItem B and the user tries to move B to the left of T,
////                if(position < 0 && destX <= editor.MapX)
////                    // Move B one spot right of T.
////                    destX = editor.MapX + 1;
////                // If an gameItem T is to the right of gameItem B and the user tries to move B to the right of T,
////                if(position > 0 && destX >= editor.MapX)
////                    // Move B one spot left of T.
////                    destX = editor.MapX - 1;
////            }
////        }

////        internal void RequestVerticalMove(int p) {
////            int destY = this.Top / 8 + p;
////            if(destY < 1) destY = 1;
////            if(destY > 30) destY = 30;

////            //bool checkCollision = true;
////            //while(checkCollision){
////            //    checkCollision = false;

////                foreach(ItemEntryEditor e in Parent.Controls) {
////                    if(e != this) {
////                        if(e.MapY == destY) {
////                            return;
////                            //destY++;
////                            //if(destY == 30) destY = 1;
////                            //checkCollision = true;
////                        }
////                    }
////                }
////            //}

////            this.MapY = destY;
////        }
////    }

////    /// <summary>
////    /// Provides a UI component that represents a single screen's data in an gameItem data entry.
////    /// </summary>
////    class ItemEntryScreenTile:Label
////    {
////        ////ItemSeeker item;
////        ////ItemRowEntry entry;
////        ItemScreenData screen;
////        ItemData item;

////        public ItemEntryScreenTile(ItemScreenData screen, ItemData item) {
////            this.item = item;
////            this.screen = screen;

////            // Initialize
////            AutoSize = false;
////            Left = screen.MapX * 8;
////            Size = new System.Drawing.Size(8, 8);
////            Text = null;
////            Image = ItemEditTool.GetEditTool(item).GetImage(item);
////        }

////        /// <summary>
////        /// Compares the position of currentLevelItems.
////        /// </summary>
////        /// <param name="d">The gameItem to compare to.</param>
////        /// <returns>T negative value if the compared gameItem comes before this gameItem, a positive value if it comes after, or zero if they are at the same position.</returns>
////        public int ComparePosition(ItemEntryScreenTile d) {
////            if(d.Item.MapX < MapX) return -1;
////            if(d.Item.MapX > MapX) return 1;
////            return 0;
////        }

////        public ItemEntryEditor EntryEditor { get { return this.Parent as ItemEntryEditor; } }
////        public ItemSeeker Item { get { return item; } }
////        public int MapX {
////            get {
////                return item.MapX;
////            }
////            set {
////                item.MapX = value;
////                ResetLeft();
////            }
////        }

////        /// <summary>
////        /// Resets the position of this control based on the parent's position.
////        /// </summary>
////        public void ResetLeft() {
////            Left = item.MapX * 8 - Parent.Left;
////            EntryEditor.Trim();
////        }

////        bool dragging;
////        int dragX, dragY;
////        protected override void OnMouseDown(MouseEventArgs e) {
////            if(e.Button == MouseButtons.Left) {
////                dragging = true;
////                dragX = e.X / 8;
////                dragY = e.Y / 8;
////            }

////            base.OnMouseDown(e);

////            //(this.FindForm() as ItemEditor).SelectItem(this);
////            dataEditor.NotifyItemSelected(this);
////        }

////        private ItemDataEditor dataEditor;

////        public ItemDataEditor DataEditor {
////            get { return dataEditor; }
////            set { dataEditor = value; }
////        }

////        protected override void OnMouseMove(MouseEventArgs e) {
////            if(dragging) {
////                int destX = MapX + e.X / 8;
////                if(e.X < 0) destX--; // Negative division rounds up instead of down
////                if(destX < 1) destX = 1; if(destX > 30) destX = 30;
////                int destY = e.Y / 8;
////                if(e.Y < 0) destY--; // Negative division rounds up instead of down

////                bool dragHorizontal = destX != MapX;
////                bool dragVertical = destY != 0;

////                if(dragHorizontal) {
////                    EntryEditor.VerifyItemPosition(this, ref destX);
////                    MapX = destX;
////                }
////                if(dragVertical) {
////                    EntryEditor.RequestVerticalMove(destY);
////                }
////            }

////            base.OnMouseMove(e);
////        }
////        protected override void OnMouseUp(MouseEventArgs e) {
////            if(e.Button == MouseButtons.Left)
////                dragging = false;

////            base.OnMouseUp(e);
////        }

////        bool Leftmost {
////            get {
////                return this == Parent.Controls[0];
////            }
////        }

////    }
////}
