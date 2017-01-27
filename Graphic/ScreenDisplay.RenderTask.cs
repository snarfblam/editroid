using System;
using System.Collections.Generic;
using System.Text;
using Editroid.Graphic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Editroid.Graphic
{
    partial class ScreenRenderer
    {
        public class RenderTask: Editroid.RenderTask
        {
            ScreenRenderer display;
            Blitter b;
            EnemyInstance[] sprites;
            DoorInstance[] doors; 
            bool showPhysics;
            Bitmap dest;
            bool invalidate;

            public RenderTask(ScreenRenderer display, Blitter b, Bitmap dest, EnemyInstance[] sprites, DoorInstance[] doors, bool showPhysics, bool invalidate) {
                this.display = display;
                this.b = b;
                this.doors = doors;
                this.showPhysics = showPhysics;
                this.sprites = sprites;
                this.dest = dest;
                this.invalidate = invalidate;
            }

            public bool Invalidate { get { return invalidate; } }

            /// <summary>Applies highlight effect to highlight palettes (colors 0x10-0x1F and 0x30-0x3F, which should mirror 0x00-0x0F and 0x20-0x2F prior to calling this function).</summary>
            private void ApplyHighlightFilter(ColorPalette p) {
                ApplyHighlightFilterToRange(p, 16, 31);
                ApplyHighlightFilterToRange(p, 48, 63);
            }

            /// <summary>Applies highlight effect to the specified range within a paletteIndex.</summary>
            private void ApplyHighlightFilterToRange(ColorPalette p, int start, int end) {
                int clr;
                for (int i = start; i <= end; i++) {
                    p.Entries[i] = Color.FromArgb(255,
                        255 - ((255 - p.Entries[i].R) / 2),
                        255 - ((255 - p.Entries[i].G) / 2),
                        255 - ((255 - p.Entries[i].B) / 2));

                    //switch (selectionHighlight) {
                    //    case HighlightEffect.Invert:
                    //        clr = p.Entries[i].ToArgb();
                    //        p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                    //        break;
                    //    case HighlightEffect.InvertBack:
                    //        if (i % 4 == 0) {
                    //            clr = p.Entries[i].ToArgb();
                    //            p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                    //        }
                    //        break;
                    //    case HighlightEffect.Lighten:
                    //        p.Entries[i] = Color.FromArgb(255,
                    //            255 - ((255 - p.Entries[i].R) / 2),
                    //            255 - ((255 - p.Entries[i].G) / 2),
                    //            255 - ((255 - p.Entries[i].B) / 2));
                    //        break;
                    //    case HighlightEffect.LightenInvertBack:
                    //        if (i % 4 == 0) {
                    //            clr = p.Entries[i].ToArgb();
                    //            p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                    //        } else {
                    //            p.Entries[i] = Color.FromArgb(255,
                    //                255 - ((255 - p.Entries[i].R) / 2),
                    //                255 - ((255 - p.Entries[i].G) / 2),
                    //                255 - ((255 - p.Entries[i].B) / 2));
                    //        }
                    //        break;
                    //}
                }
            }



            public override void DoWork() {
                ColorPalette bitmapPalette = dest.Palette;
                NesPalette bgPalette = display._Level.BgPalette;
                NesPalette spritePalette = display._Level.SpritePalette;
                // Todo: alt pal support
                //if (display..UseAltPalette && (display._Level.LevelIdentifier == LevelIndex.Brinstar || LevelData.LevelIdentifier == LevelIndex.Norfair))
                //    bgPalette = display._Level.BgAltPalette;

                // Load palettes twice
                bgPalette.ApplyTable(bitmapPalette.Entries);
                bgPalette.ApplyTable(bitmapPalette.Entries, 16);
                spritePalette.ApplyTable(bitmapPalette.Entries, 32);
                spritePalette.ApplyTable(bitmapPalette.Entries, 48);
                // Invert second paletteIndex for selections
                ApplyHighlightFilter(bitmapPalette);
                dest.Palette = bitmapPalette;

                bitmapPalette.Entries[NesPalette.HighlightEntry] = SystemColors.Highlight;


                b.Begin(display.Level.Patterns.PatternImage, dest);

                if (showPhysics) {
                    display.RenderPhysics(b);
                    display.RederEnemies(b, sprites);
                } else {
                    for (int x = 0; x < 32; x++) {
                        for (int y = 0; y < 30; y++) {
                            int tile = display.tiles[x, y];
                            b.BlitTile(tile, x, y, display.colors[x, y]);
                        }
                    }


                    display.RederEnemies(b, sprites);
                    if (doors != null) {
                        foreach (DoorInstance d in doors) {
                            byte pal = 9;
                            if (d.Type == DoorType.Missile) pal = 8;
                            else if (d.Type == DoorType.TenMissile) pal = 10;

                            if (d.Side == DoorSide.Left) {
                                Graphic.SpriteDefinition.LeftDoor.Draw(b, 2, 0xA, pal);
                            } else {
                                Graphic.SpriteDefinition.RightDoor.Draw(b, 0x1D, 0xA, pal);
                            }
                        }
                    }
                }

                if (!showPhysics) {
                    ////if (display.gameItem != null)
                    ////    display.gameItem.Draw(b);


                    for (int i = 0; i < display._sprites.Count; i++) {
                        SpriteListItem item = display._sprites[i];
                        item.SpriteDefinition.Draw(b, item.Location.X, item.Location.Y, (byte)(item.PaletteIndex));
                    }

                }

                b.End();
            }
        }
    }
}
