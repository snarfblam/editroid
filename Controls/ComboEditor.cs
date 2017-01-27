using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Editroid.Actions;

namespace Editroid
{
	internal partial class ComboEditor:UserControl
	{
		Graphic.Blitter b = new Editroid.Graphic.Blitter();
        bool isDrawing;

		Bitmap tiles = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);

		public ComboEditor() {
			//InitializeComponent();


            this.lblSelection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSelection
            // 
            this.lblSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblSelection.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSelection.Location = new System.Drawing.Point(0, 0);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(16, 16);
            this.lblSelection.TabIndex = 0;
            this.lblSelection.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseMove);
            this.lblSelection.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseDown);
            this.lblSelection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblSelection_MouseUp);
            // 
            // ComboEditor
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.lblSelection);
            this.Name = "ComboEditor";
            this.ResumeLayout(false);

		}

		//public void SetPalette(NesPalette pal, int paltable) {
		//    ColorPalette p = tiles.Palette;

		//    pal.ApplyTable(p.Entries, 0, paltable * 4, 4);
		//}
		public void SetPalette(NesPalette pal) {
			ColorPalette p = tiles.Palette;
			pal.ApplyTable(p.Entries);
			tiles.Palette = p;

			this.DoubleBuffered = true;
		}

		private int _PaletteTable;

		public int PaletteTable {
			get { return _PaletteTable; }
			set { _PaletteTable = value; }
		}
	

		ComboTable _Combos;
		PatternTable patterns;
		public void SetCombos(ComboTable combos) {
			_Combos = combos;
		}

		public void SetPatterns(PatternTable p) {
			patterns = p;
		}

		private int _ScrollPos;

		public int ScrollPosition {
			get { return _ScrollPos; }
			set { _ScrollPos = value; }
		}

        private LevelIndex level;

        public LevelIndex Level {
            get { return level; }
            set { level = value; }
        }


		/// <summary>
		/// Draws the combos in the editor.
		/// </summary>
		public void Draw() {
            if (patterns == null || tiles == null) return;

			b.Begin(patterns.PatternImage, tiles);

			for(int row = 0; row < 16; row++) {
				for(int combo = 0; combo < 16; combo++ ) {
					int comboIndex = combo + (row + _ScrollPos)* 16;
					b.BlitTile(_Combos[comboIndex].GetByte(0), combo * 2, row * 2, (byte)_PaletteTable);
					b.BlitTile(_Combos[comboIndex].GetByte(1), combo * 2 + 1, row * 2, (byte)_PaletteTable);
					b.BlitTile(_Combos[comboIndex].GetByte(2), combo * 2, row * 2 + 1, (byte)_PaletteTable);
					b.BlitTile(_Combos[comboIndex].GetByte(3), combo * 2 + 1, row * 2 + 1, (byte)_PaletteTable);
				}
			}

			b.End();
			this.BackgroundImage = tiles;

			Invalidate();
		}

		private byte _CurrentTile;

		/// <summary>
		/// Gets/sets the tile that will be painted when the user clicks on a combo.
		/// </summary>
		public byte CurrentTile {
			get { return _CurrentTile; }
			set { _CurrentTile = value; }
		}


        /// <summary>
        /// (Hack) Used to prevent whole-combo-draw (ctrl+click) on every mouse move
        /// </summary>
        bool canDrawWholeCombo;
		/// <summary>
		/// Overrides base member.
		/// </summary>
		/// <param name="otherRow">otherRow argument.</param>
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);

            canDrawWholeCombo = true;
			ProcessMouse(e);

            if(ModifierKeys == Keys.None && e.Button != MouseButtons.Right) { // Make selection
                int selX = e.X / 16;
                int selY = e.Y / 16;
                if(selX < 0) selX = 0; if(selX > 15) selX = 15;
                if(selY < 0) selY = 0; if(selY > 15) selY = 15;

                lblSelection.Location = new Point(selX * 16, selY * 16);
                OnSelectionChanged();
            }
		}

		/// <summary>
		/// Overrides base member.
		/// </summary>
		/// <param name="otherRow">otherRow argument.</param>
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			//ProcessMouse(e);
		}
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Left && isDrawing) {
                isDrawing = false;
                BreakActionChain();
                OnComboEdited();
            }
        }

        ModifyCombo chainCombo;
        void BreakActionChain() {
            chainCombo = null;
        }

        /// <summary>
        /// Processes mouse events for combo editing.
        /// </summary>
        /// <param name="otherRow">Mouse event args for a mouse event.</param>
		private void ProcessMouse(MouseEventArgs e) {
            if (!new Rectangle(0, 0, Width, Height).Contains(e.X, e.Y))
                return;

			if(e.Button == MouseButtons.Left) {
				if(ModifierKeys == Keys.Control) { // Copy entire combo from tiles
					int Combo = e.X / 16 + (e.Y / 16) * 16;
                    CreateComboAutomatically(Combo);

                    canDrawWholeCombo = false; // See XML summary
                } else if(ModifierKeys == Keys.Shift) { // Copy single tile
                    int tileX = e.X / 8;
                    int tileY = (e.Y + _ScrollPos * 16) / 8;
                    int Combo = tileX / 2 + (tileY / 2) * 16;
                    int tile = tileX % 2 + 2 * (tileY % 2);

                    // Chain combo edits from a single mouse stroke into a single action
                    if (chainCombo == null)
                        chainCombo = Program.Actions.ModifyCombo(level, Combo, tile, _CurrentTile);
                    else {
                        chainCombo = chainCombo.CreateChainableAction(Combo, tile, _CurrentTile);
                    }

                    Program.PerformAction(chainCombo);

                    ////int ComboTileX = destTileX % 2;
                    ////int ComboTileY = destTileY % 2;

                    ////Combo ClickedCombo = _Combos[Combo];
                    ////ClickedCombo[ComboTileX + ComboTileY * 2] = _CurrentTile;

                    ////// Show changes
                    ////DrawCombo(otherRow.xTile / 16, otherRow.yTile / 16);
                    ////this.Invalidate(new Rectangle((otherRow.xTile / 16) * 16,
                    ////                            (otherRow.yTile / 16) * 16,
                    ////                            16, 16));
                    isDrawing = true;
                }

            } else if(e.Button == MouseButtons.Right) {
                // On right click, select the clicked combo for painting
                int tileX = e.X / 8;
                int tileY = (e.Y + _ScrollPos * 16) / 8;
                int Combo = tileX / 2 + (tileY / 2) * 16;
                int ComboTileX = tileX % 2;
                int ComboTileY = tileY % 2;

                Combo ClickedCombo = _Combos[Combo];
                _CurrentTile = ClickedCombo[ComboTileX + ComboTileY * 2];
                if(UserGrabbedCombo != null)
                    UserGrabbedCombo(this, null);
            }
		}

        /// <summary>
        /// Assembles a combo from the selected tile and the three following it.
        /// </summary>
        /// <param name="Combo">Index of the combo to overwrite.</param>
        private void CreateComboAutomatically(int Combo) {
            Combo comboToEdit = _Combos[Combo];

            // Isolate this action from other combo actions
            BreakActionChain();

            // Note the side effect in the conditional statements below.
            // They cause each action to be chained off the previous action.
            ModifyCombo action = Program.Actions.ModifyCombo(level, Combo, 0, _CurrentTile);
            Program.PerformAction(action);
            comboToEdit[0] = (byte)_CurrentTile;
            if (_CurrentTile < 254)
                Program.PerformAction(action = action.CreateChainableAction(Combo, 1, _CurrentTile + 1));
            if (_CurrentTile < 253)
                Program.PerformAction(action = action.CreateChainableAction(Combo, 2, _CurrentTile + 2));
            if (_CurrentTile < 252)
                Program.PerformAction(action = action.CreateChainableAction(Combo, 3, _CurrentTile + 3));

            BreakActionChain();
        }

        public event EventHandler SelectionChanged;
        public event EventHandler ComboEdited;
        public event EventHandler UserGrabbedCombo;
        protected virtual void OnSelectionChanged() { if(SelectionChanged != null) SelectionChanged(this, new EventArgs()); }
        protected virtual void OnComboEdited() { if(ComboEdited != null) ComboEdited(this, new EventArgs()); }


        public int SelectedCombo { 
            get { return lblSelection.Left / 16 + 16 * (lblSelection.Top / 16); }
            set {
                lblSelection.Location = new Point(16 * ((value % 16)), 16 * (value / 16));
            }
        }
		/// <summary>
		/// Draws a combo in the editor.
		/// </summary>
		/// <remarks>This function does not refresh the control.</remarks>
		public void DrawCombo(int X, int Y, bool invalidate) {
			b.Begin(patterns.PatternImage, tiles);

			int comboIndex = X + (Y + _ScrollPos) * 16;
			b.BlitTile(_Combos[comboIndex].GetByte(0), X * 2, Y * 2, (byte)_PaletteTable);
			b.BlitTile(_Combos[comboIndex].GetByte(1), X * 2 + 1, Y * 2, (byte)_PaletteTable);
			b.BlitTile(_Combos[comboIndex].GetByte(2), X * 2, Y * 2 + 1, (byte)_PaletteTable);
			b.BlitTile(_Combos[comboIndex].GetByte(3), X * 2 + 1, Y * 2 + 1, (byte)_PaletteTable);

			b.End();

            if (invalidate) {
                this.Invalidate(new Rectangle(X * 16, Y * 16, 16, 16));
                
                Update();
            }
		}
        /// <summary>
		/// Draws a combo in the editor.
		/// </summary>
		/// <remarks>This function does not refresh the control.</remarks>
        public void DrawCombo(int index, bool invalidate) {
            DrawCombo(index % 16, index / 16, invalidate);
        }


        private void lblSelection_MouseDown(object sender, MouseEventArgs e) {
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + lblSelection.Left, e.Y + lblSelection.Top, e.Delta));
        }

        private void lblSelection_MouseMove(object sender, MouseEventArgs e) {
            OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + lblSelection.Left, e.Y + lblSelection.Top, e.Delta));

        }

        private void lblSelection_MouseUp(object sender, MouseEventArgs e) {
            OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + lblSelection.Left, e.Y + lblSelection.Top, e.Delta));
        }
	}
}
