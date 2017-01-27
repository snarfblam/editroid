/* 
  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR T 
  PARTICULAR PURPOSE. 
  
    This is sample code and is freely distributable. 
*/ 

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageQuantization
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public abstract class Quantizer
    {

        /// <summary>
        /// Construct the quantizer
        /// </summary>
        /// <param name="singlePass">If true, the quantization only needs to loop through the source pixels once</param>
        /// <remarks>
        /// If you construct this class with a true value for singlePass, then the code will, when quantizing your image,
        /// only call the 'QuantizeImage' function. If two passes are required, the code will call 'InitialQuantizeImage'
        /// and then 'QuantizeImage'.
        /// </remarks>
        public Quantizer(bool singlePass)
        {
            _singlePass = singlePass;
            _pixelSize = Marshal.SizeOf(typeof (Color32));
        }

        /// <summary>
        /// Quantize an image and return the resulting output brush
        /// </summary>
        /// <param name="source">The image to quantize</param>
        /// <returns>T quantized version of the image</returns>
        public Bitmap Quantize(Image source)
        {
            // Get the size of the source image
            int height = source.Height;
            int width = source.Width;

            // And construct a rectangle from these dimensions
            Rectangle bounds = new Rectangle(0, 0, width, height);

            // First off take a 32bpp copy of the image
            Bitmap copy = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // And construct an 8bpp version
            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            byte[] sourceData;
            // Now lock the brush into memory
            using (Graphics g = Graphics.FromImage(copy))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                // Draw the source image onto the copy brush,
                // which will effect a widening as appropriate.
                g.DrawImage(source, bounds);

            }

            // Define a pointer to the brush data
            BitmapData sourceLock = null;

            try
            {
                // Get the source image bits and lock into memory
                sourceLock = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                sourceData = new byte[sourceLock.Stride * source.Height];
                Marshal.Copy(sourceLock.Scan0, sourceData, 0, sourceData.Length);

                // Call the FirstPass function if not a single pass algorithm.
                // For something like an octree quantizer, this will run through
                // all image pixels, build a data structure, and create a paletteIndex.
                if (!_singlePass)
                    FirstPass(sourceData, width, height, sourceLock.Stride);

                // Then set the color paletteIndex on the output brush. I'm passing in the current paletteIndex 
                // as there's no way to construct a new, empty paletteIndex.
                output.Palette = GetPalette(output.Palette);


                // Then call the second pass which actually does the conversion
                SecondPass(sourceData, output, width, height, bounds, sourceLock.Stride);
            }
            finally
            {
                // Ensure that the bits are unlocked
                copy.UnlockBits(sourceLock);
            }

            // Last but not least, return the output brush
            return output;
        }

        /// <summary>
        /// Execute the first pass through the pixels in the image
        /// </summary>
        /// <param name="sourceData">The source data</param>
        /// <param name="x">The x in pixels of the image</param>
        /// <param name="height">The height in pixels of the image</param>
        /// <param name="stride"></param>
        protected  virtual void FirstPass(byte[] sourceData, int width, int height, int stride)
        {
            // Define the source data pointers. The source row is a byte to
            // keep addition of the stride value easier (as this is in bytes)              
            //IntPtr pSourceRow = sourceData.Scan0;
            int srcRow = 0;

            // Loop through each row
            for (int row = 0; row < height; row++)
            {
                // Set the source pixel to the first pixel in this row
                //IntPtr pSourcePixel = pSourceRow;
                int srcPixel = srcRow;

                // And loop through each column
                for (int col = 0; col < width; col++)
                {            
                    InitialQuantizePixel(new Color32(sourceData, srcPixel)); 
                    //pSourcePixel = (IntPtr)((Int32)pSourcePixel + _pixelSize);
                    srcPixel += _pixelSize;
                }	// Now I have the pixel, call the FirstPassQuantize function...

                // Add the stride to the source row
                //pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
                srcRow += stride;
            }
        }

        /// <summary>
        /// Execute a second pass through the brush
        /// </summary>
        /// <param name="sourceData">The source brush, locked into memory</param>
        /// <param name="output">The output brush</param>
        /// <param name="x">The x in pixels of the image</param>
        /// <param name="height">The height in pixels of the image</param>
        /// <param name="bounds">The bounding rectangle</param>
        /// <param name="stride"></param>
        protected virtual void SecondPass(byte[] sourceData, Bitmap output, int width, int height, Rectangle bounds, int stride)
        {
            BitmapData outputLock = null;

            try
            {
                // Lock the output brush into memory
                outputLock = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                byte[] outputData = new byte[outputLock.Stride * output.Height];
                
                // Define the source data pointers. The source row is a byte to
                // keep addition of the stride value easier (as this is in bytes)
                //IntPtr pSourceRow = sourceData.Scan0;
                //IntPtr pSourcePixel = pSourceRow;
                //IntPtr pPreviousPixel = pSourcePixel;
                int srcRow = 0;
                int srcPixel = 0;
                int srcPrevPixel = 0;

                // Now define the destination data pointers
                //IntPtr pDestinationRow = outputLock.Scan0;
                //IntPtr pDestinationPixel = pDestinationRow;
                int dstRow = 0;
                int dstPixel = 0;

                // And convert the first pixel, so that I have values going into the loop

                byte pixelValue = QuantizePixel(new Color32(sourceData, srcPixel));

                // Assign the value of the first pixel
                //Marshal.WriteByte(pDestinationPixel, pixelValue);
                outputData[dstPixel] = pixelValue;

                // Loop through each row
                for (int row = 0; row < height; row++)
                {
                    // Set the source pixel to the first pixel in this row
                    //pSourcePixel = pSourceRow;
                    srcPixel = srcRow;

                    // And set the destination pixel pointer to the first pixel in the row
                    //pDestinationPixel = pDestinationRow;
                    dstPixel = dstRow;

                    // Loop through each pixel on this scan line
                    for (int col = 0; col < width; col++)
                    {
                        // Check if this is the same as the last pixel. If so use that value
                        // rather than calculating it again. This is an inexpensive optimisation.
                        //if (Marshal.ReadByte(pPreviousPixel) != Marshal.ReadByte(pSourcePixel))
                        //{
                            // Quantize the pixel
                            pixelValue = QuantizePixel(new Color32(sourceData, srcPixel));

                            // And setup the previous pointer
                            //pPreviousPixel = pSourcePixel;
                            srcPrevPixel = srcPixel;
                        //}

                        // And set the pixel in the output
                        //Marshal.WriteByte(pDestinationPixel, pixelValue);
                        outputData[dstPixel] = pixelValue;

                        //pSourcePixel = (IntPtr)((long)pSourcePixel + _pixelSize);
                        srcPixel += _pixelSize;
                        //pDestinationPixel = (IntPtr)((long)pDestinationPixel + 1);
                        dstPixel++;

                    }

                    // Add the stride to the source row
                    //pSourceRow = (IntPtr)((long)pSourceRow + sourceData.Stride);
                    srcRow += stride;

                    // And to the destination row
                    //pDestinationRow = (IntPtr)((long)pDestinationRow + outputLock.Stride);
                    dstRow += outputLock.Stride;
                }
                Marshal.Copy(outputData, 0, outputLock.Scan0, outputData.Length);
            }
            finally
            {
                // Ensure that I unlock the output bits
                output.UnlockBits(outputLock);
            }
        }

        /// <summary>
        /// Override this to process the pixel in the first pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <remarks>
        /// This function need only be overridden if your quantize algorithm needs two passes,
        /// such as an Octree quantizer.
        /// </remarks>
        protected virtual void InitialQuantizePixel(Color32 pixel)
        {
        }

        /// <summary>
        /// Override this to process the pixel in the second pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <returns>The quantized value</returns>
        protected abstract byte QuantizePixel(Color32 pixel);

        /// <summary>
        /// Retrieve the paletteIndex for the quantized image
        /// </summary>
        /// <param name="original">Any old paletteIndex, this is overrwritten</param>
        /// <returns>The new color paletteIndex</returns>
        protected abstract ColorPalette GetPalette(ColorPalette original);

        /// <summary>
        /// Flag used to indicate whether a single pass or two passes are needed for quantization.
        /// </summary>
        private bool _singlePass;
        private int _pixelSize;

     

        /// <summary>
        /// Struct that defines a 32 bpp colour
        /// </summary>
        /// <remarks>
        /// This struct is used to read data from a 32 bits per pixel image
        /// in memory, and is ordered in this manner as this is the way that
        /// the data is layed out in memory
        /// </remarks>
        [StructLayout(LayoutKind.Explicit)]
        public struct Color32
        {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pSourcePixel"></param>
            public Color32(IntPtr pSourcePixel)
            {
              this = (Color32) Marshal.PtrToStructure(pSourcePixel, typeof(Color32));
                           
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="data"></param>
            /// <param name="offset"></param>
            public Color32(byte[] data, int offset) {
                ARGB = 0;
                Blue = data[offset];
                Green = data[offset + 1];
                Red = data[offset + 2];
                Alpha = data[offset + 3];
            }
            /// <summary>
            /// Holds the blue component of the colour
            /// </summary>
            [FieldOffset(0)]
            public byte Blue;
            /// <summary>
            /// Holds the green component of the colour
            /// </summary>
            [FieldOffset(1)]
            public byte Green;
            /// <summary>
            /// Holds the red component of the colour
            /// </summary>
            [FieldOffset(2)]
            public byte Red;
            /// <summary>
            /// Holds the alpha component of the colour
            /// </summary>
            [FieldOffset(3)]
            public byte Alpha;

            /// <summary>
            /// Permits the color32 to be treated as an int32
            /// </summary>
            [FieldOffset(0)]
            public int ARGB;

            /// <summary>
            /// Return the color for this Color32 object
            /// </summary>
            public Color Color
            {
                get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            }
        }
    }
}
