using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Editroid
{
    class ColorSelector:Control
    {
        PictureBox selection;
        public ColorSelector() {
            BackgroundImage = Editroid.Properties.Resources.Palette;

            selection = new PictureBox();
            selection.Bounds = new Rectangle(0, 0, 16, 16);
            selection.Image = Editroid.Properties.Resources.dot;
            selection.BackColor = Color.Transparent;
            Controls.Add(selection);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            selection.Hide();
            if (e.X < 208)
                selection.Location = new Point( e.X / 16 * 16, e.Y / 16 * 16);
            else
                selection.Location = new Point(224, 24);
            selection.Show();

            if (ColorSelected != null) ColorSelected(this, new EventArgs());
        }
        public event EventHandler ColorSelected;
        public int Selection {
            get {
                if (selection.Left >= 208) 
                    return 15;
                return (selection.Left / 16) + 16 * (selection.Top / 16);
            }
            set {
                selection.Hide();
                if ((value % 16) >= 13) {
                    selection.Location = new Point(224, 24);
                } else {
                    selection.Location = new Point((value % 16) * 16, (value / 16) * 16);
                }
                selection.Show();
            }
        }
    }
}
