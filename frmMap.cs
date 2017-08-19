using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ImageQuantization;

namespace Editroid
{
    internal  partial class frmMap:Form
    {
        public frmMap() {
            InitializeComponent();
        }

        public Image MapImage {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if(dlgSave.ShowDialog() == DialogResult.OK) {
                ImageCodecInfo codec = null;
                ImageCodecInfo[] allCodecs = ImageCodecInfo.GetImageEncoders();
                EncoderParameters prams = new EncoderParameters();

                switch(dlgSave.FilterIndex) {
                    case 1: // BMP
                        pictureBox1.Image.Save(dlgSave.FileName, ImageFormat.Bmp);
                        return;
                    case 2: // Indexed BMP
                        foreach(ImageCodecInfo c in allCodecs) {
                            if(c.FilenameExtension.ToUpper().Contains(".BMP"))
                                codec = c;
                        }

                        (pictureBox1.Image as Bitmap).SetPixel(0, 0, Color.White);
                        OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);
                        Image quantized = quantizer.Quantize(pictureBox1.Image);
                        quantized.Save(dlgSave.FileName, ImageFormat.Bmp);
                        quantized.Dispose();
                        break;
                    case 3: // Gif
                        foreach(ImageCodecInfo c in allCodecs) {
                            if(c.FilenameExtension.ToUpper().Contains(".GIF"))
                                codec = c;
                        }

                        (pictureBox1.Image as Bitmap).SetPixel(0, 0, Color.White);
                        OctreeQuantizer quantizerGif = new OctreeQuantizer(255, 8);
                        Image quantizedGif = quantizerGif.Quantize(pictureBox1.Image);
                        quantizedGif.Save(dlgSave.FileName, ImageFormat.Gif);
                        quantizedGif.Dispose();
                        break;
                    case 4: // JPeg High
                        foreach(ImageCodecInfo c in allCodecs) {
                            if(c.FilenameExtension.ToUpper().Contains(".JPG"))
                                codec = c;
                        }
                        prams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)80);
                        pictureBox1.Image.Save(dlgSave.FileName, codec, prams);
                        return;
                    case 5: // PNG
                        pictureBox1.Image.Save(dlgSave.FileName, ImageFormat.Png);
                        return;
                    //// Below options are no longer available
                    ////case 5: // JPeg (Normal)
                    ////    foreach(ImageCodecInfo c in allCodecs) {
                    ////        if(c.FilenameExtension.ToUpper().Contains(".JPG"))
                    ////            codec = c;
                    ////    }
                    ////    prams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)40);
                    ////    pictureBox1.Image.Save(dlgSave.FileName, codec, prams);
                    ////    return;
                    ////case 6: // JPeg Low
                    ////    foreach(ImageCodecInfo c in allCodecs) {
                    ////        if(c.FilenameExtension.ToUpper().Contains(".JPG"))
                    ////            codec = c;
                    ////    }
                    ////    prams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)10);
                    ////    pictureBox1.Image.Save(dlgSave.FileName, codec, prams);
                    ////    return;
                }
                //Bitmap saveImage = pictureBox1.Image as Bitmap;


                //if(dlgSave.FilterIndex == 2 || dlgSave.FilterIndex == 3) {
                //    saveImage = Create8bppImage(saveImage);
                //}

                //if(codec == null) { // If a codec was not found:
                //    MessageBox.Show("The image codec for the selected file format could not be found.", "Codec Not Found");
                //    return;
                //} else {
                //    try {
                //        saveImage.Save(dlgSave.FileName, codec, prams);
                //    }
                //    catch(Exception ex) {
                //        ;
                //    }
                //}
            }
        }

        private Bitmap Create8bppImage(Bitmap saveImage) {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            if(pictureBox1.Image != null) {
                Image i = pictureBox1.Image;
                pictureBox1.Image = null;
                i.Dispose();
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void panel1_Click(object sender, EventArgs e) {
            panel1.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            panel1.Focus();
        }
    }
}