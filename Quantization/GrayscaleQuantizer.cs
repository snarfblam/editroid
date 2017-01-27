/* 
  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR T 
  PARTICULAR PURPOSE. 
  
    This is sample code and is freely distributable. 
*/ 

using System;
using System.Collections;
using System.Drawing;

namespace ImageQuantization
{
	/// <summary>
	/// Summary description for PaletteQuantizer.
	/// </summary>
	public  class GrayscaleQuantizer : PaletteQuantizer
	{
		/// <summary>
		/// Construct the paletteIndex quantizer
		/// </summary>
		/// <remarks>
		/// Palette quantization only requires a single quantization step
		/// </remarks>
		public GrayscaleQuantizer () : base( new ArrayList() )
		{
			_colors = new Color[256];

			int nColors = 256;

			// Initialize a new color table with entries that are determined
			// by some optimal paletteIndex-finding algorithm; for demonstration 
			// purposes, use a grayscale.
			for (uint i = 0; i < nColors; i++)
			{
				uint Alpha = 0xFF;                      // Colors are opaque.
				uint Intensity = Convert.ToUInt32(i*0xFF/(nColors-1));    // Even distribution. 

				// The GIF encoder makes the first entry in the paletteIndex
				// that has a ZERO alpha the transparent color in the GIF.
				// Pick the first one arbitrarily, for demonstration purposes.
    
				// Create a gray scale for demonstration purposes.
				// Otherwise, use your favorite color reduction algorithm
				// and an optimum paletteIndex for that algorithm generated here.
				// For example, a color histogram, or a median cut paletteIndex.
				_colors[i] = Color.FromArgb( (int)Alpha, 
					(int)Intensity, 
					(int)Intensity, 
					(int)Intensity );
			}
		}

		/// <summary>
		/// Override this to process the pixel in the second pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		protected override byte QuantizePixel ( Color32 pixel )
		{
			byte	colorIndex = 0 ;

			double luminance = (pixel.Red *0.299) + (pixel.Green *0.587) + (pixel.Blue  *0.114);

			// Gray scale is an intensity map from black to white.
			// Compute the count to the grayscale entry that
			// approximates the luminance, and then round the count.
			// Also, constrain the count choices by the number of
			// colors to do, and then set that pixel's count to the 
			// byte value.
			colorIndex = (byte)(luminance +0.5);

			return colorIndex ;
		}

	}
}
