using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    class MapControlSelection: PictureBox
    {

        protected override void OnPaint(PaintEventArgs pe) {
            pe.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            base.OnPaint(pe);
            //pe.Graphics.DrawRectangle(System.Drawing.Pens.White, new System.Drawing.Rectangle(0, 0, Width, Height));
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {
            base.OnPaintBackground(pevent);
            pevent.Graphics.DrawRectangle(System.Drawing.Pens.White, new System.Drawing.Rectangle(0, 0, Width - 1, Height - 1));

        }
    }
}
