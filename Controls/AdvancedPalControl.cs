using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.Drawing;
using System.Windows.Forms;

namespace Editroid.Controls
{
    class AdvancedPalControl:PalControlBase
    {
        PpuMacro macro;
        public PpuMacro Macro {
            get { return macro; }
            set {
                macro = value;
                
                if (macro.IsEmpty) {
                    SetGridSize(new Size(1, 1));
                } else {
                    SetGridSize(new System.Drawing.Size(macro.MacroSize, 1));
                }

                Redraw();
            }
        }

        private void Redraw() {
            gBuffer.Clear(BackColor);
            if (!macro.IsEmpty) {
                for (int i = 0; i < macro.MacroSize; i++) {
                    DrawSquare(i, 0, macro.GetMacroByte(i));
                }
            }

        }

        public event EventHandler<EventArgs<int>> ColorGrabbed;

        protected override void OnCellClicked(int entryIndex, int palIndex, MouseButtons button) {
            if (button == MouseButtons.Left) // Paint color
                base.OnCellClicked(entryIndex, palIndex, button);
            else if(button == MouseButtons.Right) { // Grab color
                if (!macro.IsEmpty && entryIndex < macro.MacroSize) {
                    var clickedCellValue = macro.GetMacroByte(entryIndex);
                    if (ColorGrabbed != null) {
                        ColorGrabbed(this, new EventArgs<int>(clickedCellValue));
                    }
                }
            }
            // <From LevelPaletteEditor.cs>
            //if (palData == null) return;

            //if (button == MouseButtons.Left) {
            //    if (entryIndex == 0) {
            //        ((frmMain)FindForm().Owner).PerformAction(Actions.SetPaletteColor(levelData.Index, PaletteType.ZeroEntry, palIndex, entryIndex, selectedColor));
            //    } else {
            //        ((frmMain)FindForm().Owner).PerformAction(Actions.SetPaletteColor(levelData.Index, palType, palIndex, entryIndex, selectedColor));
            //    }
            //} else if (button == MouseButtons.Right) {
            //    int palEntry = palIndex * 4 + entryIndex;
            //    ((frmPalette)FindForm()).SelectColor(palData.GetEntry(palEntry));

            //}
        }

    }
}
