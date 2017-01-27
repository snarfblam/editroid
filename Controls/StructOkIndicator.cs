using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    class StructOkIndicator:OkIndicatorControl
    {
        public StructOkIndicator() {
            base.BackgroundImage = Editroid.Properties.Resources.StructBG;
            base.OkImage = Editroid.Properties.Resources.Check;
            base.InvalidImage = Editroid.Properties.Resources.X;

            Size = new Size(16, 16);
        }

        [Browsable(false)]
        [EditorBrowsable( EditorBrowsableState.Never)]
        public new Image BackgroundImage {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Image OkImage {
            get { return base.OkImage; }
            set { base.OkImage = value; }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Image InvalidImage {
            get { return base.InvalidImage; }
            set { base.InvalidImage = value; }
        }


        string ToolTipText {
            get {
                return StructForm.NeededBytesCaption;
            }
            set {
                StructForm.NeededBytesCaption = value;
            }
        }

        frmStruct StructForm {
            get {
                if (container == null) return null;
                return Container.FindForm() as frmStruct;
            }
        }

        private Control container;

        public Control Container {
            get { return container; }
            set {
                if (container != null) {
                    container.MouseMove -= new MouseEventHandler(container_MouseMove);
                    container.Controls.Remove(this);
                }

                container = value;

                if (container != null) {
                    container.MouseMove += new MouseEventHandler(container_MouseMove);
                    container.Controls.Add(this);
                }
            }
        }

        void container_MouseMove(object sender, MouseEventArgs e) {
            if (CheckPosition(e.X / 16, e.Y / 16)) {
                UpdateState();
                SetPosition();
            }
        }

        

        private void SetPosition() {
            Location = new Point(currentX * 16, currentY * 16);
        }

        public void UpdateState() {
            if (structData == null) return;

            StructAnalizer analizer = new StructAnalizer(structData);
                if (currentX < 0 || currentY < 0 || currentX > 15 || currentY > 15) {
                    // Invalid tile position
                    State = false;
                    ToolTipText = "You've entered the beyond section.";
                } else if (structData.Data[currentX, currentY] != Structure.EmptyTile) {
                    // There is already a tile here
                    Visible = false;
                    State = false;
                    ToolTipText = null;
                    return;
                } else {
                    int additionalMemoryNeeded = analizer.CostOfAddingTile(currentX, currentY) - freeSpace;
                    if (additionalMemoryNeeded <= 0) {
                        // Enough free mem
                        State = true;
                        ToolTipText = null;
                    } else {
                        // Not enough free mem
                        State = false;
                        ToolTipText = "Need " + additionalMemoryNeeded.ToString() + " additional byte" +
                            ((additionalMemoryNeeded == 1) ? "" : "s");
                    }
                }
            
            
            //} else if (currentY == structData.Height) {
            //    State = (currentX >= 0 && currentX < 16 && freeSpace >= 2);
            //} else {
            //    byte[] rowData = structData.Combos[currentY];
            //    if (currentX > rowData.Length) {
            //        State = false;
            //    } else if (currentX == rowData.Length) {
            //        State = freeSpace >= 1;
            //    } else {
            //        if (rowData[currentX] != Struct.EmptyTile) {
            //            Visible = false;
            //            return; // Avoid Visible=TRUE at end of method
            //        }
            //        State = (currentX < rowData.Length - 1 && rowData[currentX + 1] != Struct.EmptyTile &&  freeSpace >= 1);
            //    }
            //}

            Visible = true;
        }

        int currentX, currentY;
        
        /// <summary>Returns whether the specified new position is different from the current position.
        /// If so, currentX and currentY are updated. but the control is NOT moved (call SetPosition
        /// to do this).</summary>
        bool CheckPosition(int x, int y) {
            if (x == currentX && y == currentY) return false;

            currentX = x;
            currentY = y;
            return true;
        }

        private Structure structData;

        public Structure StructData {
            get { return structData; }
            set { structData = value; }
        }


        private int freeSpace = 2;

        public int FreeSpace {
            get { return freeSpace; }
            set { freeSpace = value; }
        }


    }
}
