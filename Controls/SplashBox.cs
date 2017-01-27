using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Editroid
{
    class SplashBox:Control
    {
        Image splash = SplashImages.Splash;
        Image screenShot;

        public SplashBox(){
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);

            PickScreenshot();
        }

        private void PickScreenshot() {
            Random r = new Random();
            int i = r.Next();
            screenShot = GetScreenshot(i);
        }

        private static Image GetScreenshot(int i) {
            switch ((i % 15)) {
                case 0:
                    return SplashImages.img1;
                case 1:
                    return SplashImages.img3;
                case 2:
                    return SplashImages.img4;
                case 3:
                    return SplashImages.img7;
                case 4:
                    return SplashImages.img8;
                case 5:
                    return SplashImages.img9;
                case 6:
                    return SplashImages.img10;
                case 7:
                    return SplashImages.img11;
                case 8:
                    return SplashImages.img12;
                case 9:
                    return SplashImages.img13;
                case 10:
                    return SplashImages.img14;
                case 11:
                    return SplashImages.img15;
                case 12:
                    return SplashImages.img16;
                case 13:
                    return SplashImages.img17;
                case 14:
                default:
                    return SplashImages.img6;
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (screenShot == null || splash == null) return;

            int centerX = Width / 2;
            int centerY = Height / 2;

            int screenShotX = centerX - screenShot.Width ;
            int screenShotY = centerY - screenShot.Height;
            Rectangle source = new Rectangle(0, 0, screenShot.Width, screenShot.Height);
            Rectangle dest = new Rectangle(screenShotX, screenShotY, screenShot.Width * 2, screenShot.Height * 2);

            e.Graphics.DrawImage(screenShot, dest, source, GraphicsUnit.Pixel);


            int splashX = centerX - splash.Width / 2;
            int splashY = centerY - splash.Height / 2;
            source = new Rectangle(0, 0, splash.Width, splash.Height);
            dest = new Rectangle(splashX, splashY, splash.Width, splash.Height);

            e.Graphics.DrawImage(splash, dest, source, GraphicsUnit.Pixel);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right && (ModifierKeys & Keys.Shift) == Keys.Shift) {
                // Show a different screen
                PickScreenshot();
                Invalidate();
            }
        }

        internal void ClearImages() {
            if (screenShot != null)
                screenShot.Dispose();
            screenShot = null;

            if (splash != null)
                splash.Dispose();
            splash = null;
        }
    }
}
