using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Editroid.Graphic;
using System.Collections;
using Editroid.ROM;
using System.Drawing.Imaging;
using System.Threading;

namespace Editroid.Graphic
{
	/// <summary>
	/// Renders a Metroid screen.
	/// </summary>
	public partial class ScreenRenderer
	{
		byte[,] tiles = new byte[32, 30];
		byte[,] colors = new byte[32, 30];
        Physics[,] physics = new Physics[32, 30];


        Level _Level;
        

		/// <summary>
		/// Gets/sets which currentLevelIndex to obtain screen data from.
		/// </summary>
		public Level Level {
			get { return _Level; }
			set { _Level = value; }
		}


        private byte defaultPal;

        /// <summary>
        /// Get/set the default paletteIndex.
        /// </summary>
        public byte DefaultPalette {
            get { return defaultPal; }
            set { defaultPal = value; }
        }


		/// <summary>
		/// Clears the screen and sprite data.
		/// </summary>
		public void Clear() {
            Physics defaultPhysics= Physics.Air;
            if (Level != null) {
                defaultPhysics = Level.Rom.GetPhysics(0xff);
                if (Level.Rom.Format.SupportsCustomTilePhysics) {
                    defaultPhysics = Level.Rom.GetPhysics(Level.Rom.data[Level.TilePhysicsTableLocation + 0xFF]);
                }
            }

			for(int i = 0; i < 960; i++) {
				tiles[i % 32, i / 32] = 0xFF;
                colors[i % 32, i / 32] = defaultPal;// Palette.Palette0;
                physics[i % 32, i / 32] = defaultPhysics;
			}

            _sprites.Clear();
		}

        Physics GetCustomPhysics(byte tileNum){
            return _Level.Rom.GetPhysics(_Level.Rom.data[_Level.TilePhysicsTableLocation + tileNum]);
        }

		/// <summary>
		/// Draws an object to this screen display.
		/// </summary>
		/// <param name="obj">An instance of a combo. Palette and position information
		/// is specified in the StructInstance.</param>
		public void DrawObject(StructInstance obj) {


			int TileX = obj.X * 2;
			int ScreenY = obj.Y * 2;

            PhysicsGetter GetPhysics;
            if (_Level.Rom.Format.SupportsCustomTilePhysics) {
                GetPhysics = this.GetCustomPhysics;
            } else {
                GetPhysics = _Level.Rom.GetPhysics;
            }

            if (obj.ObjectType >= Level.StructCount) return;

            var structure = Level.GetStruct(obj.ObjectType);
			for(int structTileY = 0; structTileY < structure.RowCount; structTileY++) {
				int ScreenX = TileX;
                int structTileRight = Math.Min(Structure.StructureData.width, 16);
				for(int structTileX = 0; structTileX < structTileRight; structTileX++) {
                    int comboIndex = structure.Data[structTileX, structTileY];
                    Combo combo = Level.GetCombo(comboIndex);
					if(comboIndex != Struct.EmptyTile &&
                        ScreenX <= 31 && ScreenY <= 29) {



						// Apply tiles
                        byte tileIndex;
                        // Top-left
                        tileIndex = combo.GetByte(0);
						tiles[ScreenX, ScreenY] = tileIndex;
                        physics[ScreenX, ScreenY] = GetPhysics(tileIndex);
                        // Top-Right
                        tileIndex = combo.GetByte(1);
                        tiles[ScreenX + 1, ScreenY] = tileIndex;
                        physics[ScreenX + 1, ScreenY] = GetPhysics(tileIndex);
                        // Bottom-Left
                        tileIndex = combo.GetByte(2);
                        tiles[ScreenX, ScreenY + 1] = tileIndex;
                        physics[ScreenX, ScreenY + 1] = GetPhysics(tileIndex);
                        // Bottom-Right
                        tileIndex = combo.GetByte(3);
                        tiles[ScreenX + 1, ScreenY + 1] = tileIndex;
                        physics[ScreenX + 1, ScreenY + 1] = GetPhysics(tileIndex);

						// Apply colors
                        if(obj.PalData != defaultPal)
						    colors[ScreenX, ScreenY] =
						    colors[ScreenX + 1, ScreenY] =
						    colors[ScreenX, ScreenY + 1] =
						    colors[ScreenX + 1, ScreenY + 1] =
    							(byte)(obj.PalData | (colors[ScreenX, ScreenY] & 0x4));
					}
					ScreenX += 2;
				} // col

				ScreenY += 2;
			} // row
        }

        /////// <summary>
        /////// Gets the showPhysics of a tile.
        /////// </summary>
        /////// <param name="tile">The count of a tile.</param>
        /////// <returns>T Physics value for the showPhysics of a tile.</returns>
        ////public Physics GetPhysics(byte tile) {
        ////    return _Level.Rom.GetPhysics(tile);
        ////}
        delegate Physics PhysicsGetter(byte tile);

        private SpriteList _sprites = new SpriteList();
        /// <summary>
        /// Adds a sprite to a batch to be rendered to the output image.
        /// </summary>
        /// <param name="sprite">The graphic sprite.</param>
        /// <param name="X">The location of the sprite (in 8x8 tiles).</param>
        /// <param name="Y">The location of the sprite (int 8x8 tiles).</param>
        /// <param name="paletteIndex">The count of the paletteIndex to render with.</param>
        public void AddSprite(SpriteDefinition sprite, int X, int Y, byte paletteIndex) {
            _sprites.Add(new SpriteListItem(sprite, paletteIndex, new Point(X,Y)));
        }



        /// <summary>
        /// Draws an object to this screen display using the fourth paletteIndex table
        /// after the specified paletteIndex table to indicate that this object is selected
        /// </summary>
        /// <param name="obj">An instance of a combo. Palette and position information
        /// is specified in the StructInstance.</param>
        public void DrawObjectSelected(StructInstance obj) {
            int TileX = obj.X * 2;
            int ScreenY = obj.Y * 2;

            PhysicsGetter GetPhysics = _Level.Rom.GetPhysics;

            if (obj.ObjectType >= Level.StructCount) return;

            var structure = Level.GetStruct(obj.ObjectType);
            for (int structTileY = 0; structTileY < structure.RowCount; structTileY++) {
                int ScreenX = TileX;
                int structTileRight = Math.Min(Structure.StructureData.width, 16);
                for (int structTileX = 0; structTileX < structTileRight; structTileX++) {
                    int comboIndex = structure.Data[structTileX, structTileY];
                    Combo combo = Level.GetCombo(comboIndex);
                    if (comboIndex != Struct.EmptyTile &&
                        ScreenX <= 31 && ScreenY <= 29) {



                        // Apply tiles
                        byte tileIndex;
                        // Top-left
                        tileIndex = combo.GetByte(0);
                        tiles[ScreenX, ScreenY] = tileIndex;
                        physics[ScreenX, ScreenY] = GetPhysics(tileIndex);
                        // Top-Right
                        tileIndex = combo.GetByte(1);
                        tiles[ScreenX + 1, ScreenY] = tileIndex;
                        physics[ScreenX + 1, ScreenY] = GetPhysics(tileIndex);
                        // Bottom-Left
                        tileIndex = combo.GetByte(2);
                        tiles[ScreenX, ScreenY + 1] = tileIndex;
                        physics[ScreenX, ScreenY + 1] = GetPhysics(tileIndex);
                        // Bottom-Right
                        tileIndex = combo.GetByte(3);
                        tiles[ScreenX + 1, ScreenY + 1] = tileIndex;
                        physics[ScreenX + 1, ScreenY + 1] = GetPhysics(tileIndex);

                        // Apply colors
                        if (obj.PalData == defaultPal) {
                            colors[ScreenX, ScreenY] |= 4;
                            colors[ScreenX + 1, ScreenY] |= 4;
                            colors[ScreenX, ScreenY + 1] |= 4;
                            colors[ScreenX + 1, ScreenY + 1] |= 4;
                        } else {
                            colors[ScreenX, ScreenY] =
                            colors[ScreenX + 1, ScreenY] =
                            colors[ScreenX, ScreenY + 1] =
                            colors[ScreenX + 1, ScreenY + 1] =
                                (byte)(obj.PalData | 0x4);
                        }
                    }
                    ScreenX += 2;
                } // col

                ScreenY += 2;
            } // row
        } // DrawObject

		/// <summary>
		/// Constructs the screen data in memory.
		/// </summary>
		/// <param name="count">The count of the screen to draw.</param>
		/// <param name="SelectedObject">The object that is currently selected.</param>
		/// <remarks>This function does not render the screen. It prepares the 
		/// data necessary to render the screen.</remarks>
		public void DrawScreen(int index, int SelectedObject) {
			Screen screen = Level.Screens[index];
			var objects = screen.Structs;

			for(int i = 0; i < objects.Count; i++) {
				if(i == SelectedObject)
					DrawObjectSelected(objects[i]);
				else
					DrawObject(objects[i]);
			}
		}

		/// <summary>
		/// Constructs the screen data in memory.
		/// </summary>
		/// <param name="count">The count of the screen to draw.</param>
		/// <param name="SelectedObject">The object that is currently selected.</param>
		/// <remarks>This function does not render the screen. It prepares the 
		/// data necessary to render the screen.</remarks>
		public void DrawScreen(Screen screen, StructInstance SelectedObject) {
            if(Level == null) return;

			var objects = screen.Structs;

			for(int i = 0; i < objects.Count; i++) {
				if(objects[i].Equals(SelectedObject))
					DrawObjectSelected(objects[i]);
				else
					DrawObject(objects[i]);
			}
		}


		/// <summary>
		/// Constructs the screen data in memory.
		/// </summary>
		/// <param name="count">The count of the screen to draw.</param>
		/// <remarks>This function does not render the screen. It prepares the 
		/// data necessary to render the screen.</remarks>
		public void DrawScreen(int index) {
			DrawScreen(index, -1);
            
		}

		/// <summary>
		/// Renders a screen using GDI+.
		/// </summary>
		/// <param name="g">GDI+ graphic destination.</param>
		/// <remarks>This overload performs very poorly due to GDI+'s very poor
		/// optimizations for paletted bitmaps. This overload also does not render
		/// enemies in the screen, mainly because of the palette limitations of GDI+.</remarks>
		public void Render(Graphics g) {
			Rectangle source = new Rectangle(0, 0, 8, 8);
			Rectangle dest = source;

			PatternTable patTable = Level.Patterns;
			Bitmap patterns = patTable.PatternImage;

			//patTable.Palette = palettes[colors[0, 0]];

			for(int x = 0; x < 32; x++) {
				dest.Y = 0;
				for(int y = 0; y < 32; y++) {
					//patTable.Palette = palettes[colors[x, y]];
					source.X = tiles[x, y] * 8;
					g.DrawImage(patterns, dest, source, GraphicsUnit.Pixel);
					dest.Y += 8;
					//if(x == 0 && y == 8) System.Windows.Forms.MessageBox.Show(tiles[x, y].ToString());
				}
				dest.X += 8;
			}
		}

		/// <summary>
		/// Renders the screen in memory with no enemies or doors.
		/// </summary>
		/// <param name="b">The Blitter object used for rendering.</param>
		/// <param name="dest">The brush to render to.</param>
		public void Render(Blitter b, Bitmap dest) {
			Render(b, dest, null, null, false);	
		}

        /// <summary>
        /// Applies the current currentLevelIndex's (specified by the Level property) paletteIndex
        /// to a Bitmap, generally the Bitmap that is being rendered to.
        /// </summary>
        /// <param name="dest">The indexed brush to apply a color table to.</param>
        /// <param name="alt">If true, the currentLevelIndex's alternate background paletteIndex will be used instead of the normal paletteIndex.</param>
        public void ApplyPalette(Bitmap dest, bool alt) {
            NesPalette pal = alt ?
                _Level.BgAltPalette :
                _Level.BgPalette;

            ColorPalette destPal = dest.Palette;
            pal.ApplyTable(destPal.Entries);
            dest.Palette = destPal;
        }

        /////// <summary>
        /////// Queues an async render. Renders the screen in memory, additionally rendering any doors and enemies specified.
        /////// </summary>
        /////// <param name="b_UTF32">The Blitter object used for rendering.</param>
        /////// <param name="dest">The brush to render to.</param>
        /////// <param name="enemies">An array of enemies to be rendered, or null if there are no enemies.</param>
        /////// <param name="doors">An array of doors to render, or null if there are no doors.</param>
        /////// <param name="showPhysics">If true then the screen will be drawn such that the phyics of the tiles are appearent. Sprites, doors, currentLevelItems, and the selection will be omitted.</param>
        ////public Editroid.Graphic.ScreenRenderer.RenderTask BeginRender(Blitter b, Bitmap dest, EnemyInstance[] sprites, DoorInstance[] doors, bool showPhysics, bool invalidate) {
        ////    Editroid.Graphic.ScreenRenderer.RenderTask task = new Editroid.Graphic.ScreenRenderer.RenderTask(this, b, dest, sprites, doors, showPhysics, invalidate);
        ////    RenderThread.QueueTask(task, true);
        ////    return task;
        ////}

		/// <summary>
		/// Renders the screen in memory, additionally rendering any doors and enemies specified.
		/// </summary>
		/// <param name="b">The Blitter object used for rendering.</param>
		/// <param name="dest">The brush to render to.</param>
		/// <param name="enemies">An array of enemies to be rendered, or null if there are no enemies.</param>
        /// <param name="doors">An array of doors to render, or null if there are no doors.</param>
        /// <param name="showPhysics">If true then the screen will be drawn such that the phyics of the tiles are appearent. Sprites, doors, currentLevelItems, and the selection will be omitted.</param>
        public void Render(Blitter b, Bitmap dest, IList<EnemyInstance> sprites, IList<DoorInstance> doors, bool showPhysics) {
            using (b.Begin(Level.Patterns.PatternImage, dest)) {

                ////if (showPhysics) {
                ////    RenderPhysics(b);
                ////    RenderEnemies(b, sprites);
                ////} else {
                ////    RenderStandard(b);
                ////    RenderEnemies(b, sprites);
                ////    RenderDoors(b, doors, selectedDoor);
                ////}

                RenderStandard(b);
                if(showPhysics)
                    RenderPhysics(b);

                RenderEnemies(b, sprites);
                RenderDoors(b, doors, selectedDoor);

                RenderMiscSprites(b);


            }
        }


        /// <summary>Renders any sprites stored in the sprites array, and the GameItem stored in gameItem, if present, and the item selection, if applicable.</summary>
        private void RenderMiscSprites(Blitter b) {
            ////if (gameItem != null)
            ////    gameItem.Draw(b);


            for (int i = 0; i < this._sprites.Count; i++) {
                SpriteListItem item = _sprites[i];
                item.SpriteDefinition.Draw(b, item.Location.X, item.Location.Y, (byte)item.PaletteIndex);
            }

            if (selectedItem != null ) {
                Rectangle selection = selectedItem.GetCollisionRect(); //new Rectangle(selectedItem.ScreenLocation.X * 16, selectedItem.ScreenLocation.Y * 16, 16,16);
                b.DrawRect(selection, NesPalette.HighlightEntry);
            }
        }

        private static void RenderDoors(Blitter b, IList<DoorInstance> doors, int selectedDoorIndex) {
            if (doors != null) {
                for(int i = 0; i  < doors.Count; i++){
                    DoorInstance d = doors[i];
                //foreach (DoorInstance d in doors) {
                    byte pal = 9;
                    if (d.Type == DoorType.Missile) pal = 8;
                    else if (d.Type == DoorType.TenMissile) pal = 10;

                    if (d.Side == DoorSide.Left) {
                        if(d.Type == DoorType.MusicChange)
                            Graphic.DoorSprites.LeftDoorMusic.Draw(b, 2, 0xA, pal);
                        else
                            Graphic.DoorSprites.LeftDoor.Draw(b, 2, 0xA, pal);
                    } else {
                        if (d.Type == DoorType.MusicChange)
                            Graphic.DoorSprites.RightDoorMusic.Draw(b, 0x1D, 0xA, pal);
                        else
                            Graphic.DoorSprites.RightDoor.Draw(b, 0x1D, 0xA, pal);
                    }

                    if (selectedDoorIndex == i) b.DrawRect(d.Bounds, NesPalette.HighlightEntry);
                }
            }
        }

        private void RenderStandard(Blitter b) {
            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 30; y++) {
                    int tile = tiles[x, y];
                    b.BlitTile(tile, x, y, colors[x, y]);
                }
            }
        }

        private void RenderEnemies(Blitter b, IList<EnemyInstance> enemies) {
            b.ChangeSource(Level.SpritePatterns.PatternImage);
            
            if (enemies != null) {
                for (int i = 0; i < enemies.Count; i++) {
                    EnemyInstance s = enemies[i];
                    SpriteDefinition enemySprite = Level.GetSprite(s.EnemyType);
                    byte pal = (byte)(8 + Level.GetSpritePalette(s.EnemyType, s.DifficultByteValue == 8));

                    enemySprite.Draw(b, s.X * 2, s.Y * 2, pal);
                    if (s.Respawn)
                        EnemyInstance.RespawnSprite.Draw(b, s.X * 2, s.Y * 2, 0);
                    if (selectedEnemy == i) {
                        Rectangle spriteBounds = enemySprite.Measure();
                        spriteBounds.X += s.X * 16;
                        spriteBounds.Y += s.Y * 16;
                        b.DrawRect(spriteBounds, NesPalette.HighlightEntry);
                    }
                }
            }
        }

        ////private void RenderPhysics(Blitter b) {
        ////    for(int x = 0; x < 32; x++) {
        ////        for(int y = 0; y < 30; y++) {
        ////            int tile = tiles[x, y];

        ////            // Don't show non-solid tiles
        ////            if(physics[x, y] == Physics.Air)
        ////                tile = 0xFF;

        ////            b.BlitTile(tile, x, y, (byte)(colors[x, y]));

        ////            if(physics[x, y] == Physics.Breakable) {
        ////                b.DrawDitherTile(x, y, 0);
        ////            }
        ////        }
        ////    }
        ////}
        private void RenderPhysics(Blitter b) {
            surroundingTilesPhysics phys;

            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 30; y++) {
                    int tile = tiles[x, y];

                    GetSurroundingTilePhysics(out phys, x, y);
                    byte color = GetColorIndexForPhysics(phys.center);
                    Point location = new Point(x*8,y*8);

                    if (phys.center == Physics.Air) { // Fill air tiles
                        b.FillTile8(x, y, color);
                    } else { // Outline all others
                        var physSolid = phys.Solidify();
                        bool drawT = physSolid.top != physSolid.center;
                        bool drawL = physSolid.center != physSolid.left;
                        bool drawB = physSolid.center != physSolid.bottom;
                        bool drawR = physSolid.center != physSolid.right;
                        bool drawTL = (drawT | drawL) || ((physSolid.topleft != physSolid.center) && !(drawT | drawL));
                        bool drawTR = (drawT | drawR) || ((physSolid.topright != physSolid.center) && !(drawT | drawR));
                        bool drawBL = (drawB | drawL) || ((physSolid.bottomleft != physSolid.center) && !(drawB | drawL));
                        bool drawBR = (drawB | drawR) || ((physSolid.bottomright != physSolid.center) && !(drawB | drawR));

                        if (drawT) {
                            Rectangle top = Top;
                            top.Offset(location);
                            b.DrawRect(top, color);
                        }
                        if (drawL) {
                            Rectangle left = Left;
                            left.Offset(location);
                            b.DrawRect(left, color);
                        }
                        if (drawB) {
                            Rectangle bottom = Bottom;
                            bottom.Offset(location);
                            b.DrawRect(bottom, color);
                        }
                        if (drawR) {
                            Rectangle right = Right;
                            right.Offset(location);
                            b.DrawRect(right, color);
                        }
                        if (drawTL) {
                            Rectangle topLeft = TopLeft;
                            topLeft.Offset(location);
                            b.DrawRect(topLeft, color);
                        }
                        if (drawTR) {
                            Rectangle topRight = TopRight;
                            topRight.Offset(location);
                            b.DrawRect(topRight, color);
                        }
                        if (drawBL) {
                            Rectangle bottomLeft = BottomLeft;
                            bottomLeft.Offset(location);
                            b.DrawRect(bottomLeft, color);
                        }
                        if (drawBR) {
                            Rectangle bottomRight = BottomRight;
                            bottomRight.Offset(location);
                            b.DrawRect(bottomRight, color);
                        }

                        if (phys.center == Physics.Breakable)
                            b.DrawDitherTile(x, y, 0);
                        if (phys.center == Physics.Door || phys.center == Physics.DoorHorizontal || phys.center == Physics.DoorBubble) {
                            b.DrawDitherTile(x, y, color);
                        }

                        if (phys.center == Physics.SlopeRight && y > 0) {
                            b.DrawSlope_UR(x, y - 1, color);
                        }
                        if (phys.center == Physics.SlopeLeft && y > 0) {
                            b.DrawSlope_UL(x, y - 1, color);
                        }
                        if (phys.center == Physics.SlopeTop && y > 0) {
                            b.DrawSlope_Top(x, y - 1, color);
                        }
                    }
                }
            }
        }

        private static byte GetColorIndexForPhysics(Physics tileType) {
            byte color = 0;
            switch (tileType) {
                case Physics.Solid:
                case Physics.SlopeLeft:
                case Physics.SlopeRight:
                case Physics.SlopeTop:
                    color = (byte)ScreenEditor.EditorColor.PhysSolid;
                    break;
                case Physics.Breakable:
                    color = (byte)ScreenEditor.EditorColor.PhysBreakable;
                    break;
                case Physics.Air:
                    color = (byte)ScreenEditor.EditorColor.PhysAir;
                    break;
                case Physics.Door:
                    color = (byte)ScreenEditor.EditorColor.PhysDoor;
                    break;
                case Physics.DoorHorizontal:
                    color = (byte)ScreenEditor.EditorColor.PhysAltDoor;
                    break;
                case Physics.DoorBubble:
                    color = (byte)ScreenEditor.EditorColor.PhysDoorBubble;
                    break;
            }
            return color;
        }

        static Rectangle TopLeft = new Rectangle(0, 0, 2, 2);
        static Rectangle Top = new Rectangle(2, 0, 4, 2);
        static Rectangle TopRight = new Rectangle(6, 0, 2, 2);
        static Rectangle Left = new Rectangle(0, 2, 2, 4);
        static Rectangle Right = new Rectangle(6, 2, 2, 4);
        static Rectangle BottomLeft = new Rectangle(0, 6, 2, 2);
        static Rectangle Bottom = new Rectangle(2, 6, 4, 2);
        static Rectangle BottomRight = new Rectangle(6, 6, 2, 2);
        void GetSurroundingTilePhysics(out surroundingTilesPhysics phys, int x, int y) {
            phys = new surroundingTilesPhysics();
            bool top = y < 1;
            bool bottom = y >= 29;
            bool left = x < 1;
            bool right = x >= 31;

            phys.center = physics[x, y];

            if (!left) {
                phys.left = physics[x - 1, y];
                if (!top)
                    phys.topleft = physics[x - 1, y - 1];
                if (!bottom)
                    phys.bottomleft = physics[x - 1, y + 1];
            }

            if (!top)
                phys.top = physics[x, y - 1];
            if (!bottom)
                phys.bottom = physics[x, y + 1];

            if (!right) {
                phys.right = physics[x + 1, y];
                if (!top)
                    phys.topright = physics[x + 1, y - 1];
                if (!bottom)
                    phys.bottomright = physics[x + 1, y + 1];
            }
        }
        private struct surroundingTilesPhysics
        {
            public Physics topleft, top, topright, left, center, right, bottomleft, bottom, bottomright;

            internal surroundingTilesPhysics Solidify() {
                surroundingTilesPhysics result = new surroundingTilesPhysics();

                result.topleft = solidifyTile(topleft);
                result.top = solidifyTile(top);
                result.topright = solidifyTile(topright);
                result.left = solidifyTile(left);
                result.center = solidifyTile(center);
                result.right = solidifyTile(right);
                result.bottomleft = solidifyTile(bottomleft);
                result.bottom = solidifyTile(bottom);
                result.bottomright = solidifyTile(bottomright);

                return result;
            }

            private Physics solidifyTile(Physics t) {
                if (t == Physics.SlopeLeft || t == Physics.SlopeRight || t == Physics.SlopeTop) return Physics.Solid;
                return t;
            }
        }

        private int selectedEnemy;

        /// <summary>
        /// Gets or sets the count of the selected enemy.
        /// </summary>
        public int SelectedEnemy {
            get { return selectedEnemy; }
            set { selectedEnemy = value; }
        }

        private int selectedDoor;

        /// <summary>
        /// Gets or sets the count of the selected door.
        /// </summary>
        public int SelectedDoor {
            get { return selectedDoor; }
            set { selectedDoor = value; }
        }

        private ItemInstance selectedItem;

        public ItemInstance SelectedItem {
            get { return selectedItem; }
            set { selectedItem = value; }
        }


	}


}
