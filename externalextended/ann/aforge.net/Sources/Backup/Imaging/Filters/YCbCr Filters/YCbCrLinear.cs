// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Linear correction of YCbCr channels.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>YCbCr</b> color space and provides
    /// with the facility of linear correction of its channels - mapping specified channels'
    /// input ranges to specified output ranges.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// YCbCrLinear filter = new YCbCrLinear( );
    /// // configure the filter
    /// filter.InCb = new DoubleRange( -0.276, 0.163 );
    /// filter.InCr = new DoubleRange( -0.202, 0.500 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/ycbcr_linear.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="HSLLinear"/>
    /// <seealso cref="YCbCrLinear"/>
    /// 
    public class YCbCrLinear : BaseInPlacePartialFilter
    {
        private DoubleRange inY  = new DoubleRange(  0.0, 1.0 );
        private DoubleRange inCb = new DoubleRange( -0.5, 0.5 );
        private DoubleRange inCr = new DoubleRange( -0.5, 0.5 );

        private DoubleRange outY  = new DoubleRange(  0.0, 1.0 );
        private DoubleRange outCb = new DoubleRange( -0.5, 0.5 );
        private DoubleRange outCr = new DoubleRange( -0.5, 0.5 );

        #region Public Propertis

        /// <summary>
        /// Y component's input range.
        /// </summary>
        /// 
        /// <remarks>Y component is measured in the range of [0, 1].</remarks>
        ///
        public DoubleRange InY
        {
            get { return inY; }
            set { inY = value; }
        }

        /// <summary>
        /// Cb component's input range.
        /// </summary>
        /// 
        /// <remarks>Cb component is measured in the range of [-0.5, 0.5].</remarks>
        ///
        public DoubleRange InCb
        {
            get { return inCb; }
            set { inCb = value; }
        }

        /// <summary>
        /// Cr component's input range.
        /// </summary>
        /// 
        /// <remarks>Cr component is measured in the range of [-0.5, 0.5].</remarks>
        ///
        public DoubleRange InCr
        {
            get { return inCr; }
            set { inCr = value; }
        }

        /// <summary>
        /// Y component's output range.
        /// </summary>
        /// 
        /// <remarks>Y component is measured in the range of [0, 1].</remarks>
        ///
        public DoubleRange OutY
        {
            get { return outY; }
            set { outY = value; }
        }

        /// <summary>
        /// Cb component's output range.
        /// </summary>
        /// 
        /// <remarks>Cb component is measured in the range of [-0.5, 0.5].</remarks>
        ///
        public DoubleRange OutCb
        {
            get { return outCb; }
            set { outCb = value; }
        }

        /// <summary>
        /// Cr component's output range.
        /// </summary>
        /// 
        /// <remarks>Cr component is measured in the range of [-0.5, 0.5].</remarks>
        ///
        public DoubleRange OutCr
        {
            get { return outCr; }
            set { outCr = value; }
        }

        #endregion

        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCrLinear"/> class.
        /// </summary>
        /// 
        public YCbCrLinear( )
        {
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            RGB     rgb = new RGB( );
            YCbCr   ycbcr = new YCbCr( );

            double ky  = 0, by  = 0;
            double kcb = 0, bcb = 0;
            double kcr = 0, bcr = 0;

            // Y line parameters
            if ( inY.Max != inY.Min )
            {
                ky = ( outY.Max - outY.Min ) / ( inY.Max - inY.Min );
                by = outY.Min - ky * inY.Min;
            }
            // Cb line parameters
            if ( inCb.Max != inCb.Min )
            {
                kcb = ( outCb.Max - outCb.Min ) / ( inCb.Max - inCb.Min );
                bcb = outCb.Min - kcb * inCb.Min;
            }
            // Cr line parameters
            if ( inCr.Max != inCr.Min )
            {
                kcr = ( outCr.Max - outCr.Min ) / ( inCr.Max - inCr.Min );
                bcr = outCr.Min - kcr * inCr.Min;
            }

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    rgb.Red     = ptr[RGB.R];
                    rgb.Green   = ptr[RGB.G];
                    rgb.Blue    = ptr[RGB.B];

                    // convert to YCbCr
                    AForge.Imaging.YCbCr.FromRGB( rgb, ycbcr );

                    // correct Y
                    if ( ycbcr.Y >= inY.Max )
                        ycbcr.Y = outY.Max;
                    else if ( ycbcr.Y <= inY.Min )
                        ycbcr.Y = outY.Min;
                    else
                        ycbcr.Y = ky * ycbcr.Y + by;

                    // correct Cb
                    if ( ycbcr.Cb >= inCb.Max )
                        ycbcr.Cb = outCb.Max;
                    else if ( ycbcr.Cb <= inCb.Min )
                        ycbcr.Cb = outCb.Min;
                    else
                        ycbcr.Cb = kcb * ycbcr.Cb + bcb;

                    // correct Cr
                    if ( ycbcr.Cr >= inCr.Max )
                        ycbcr.Cr = outCr.Max;
                    else if ( ycbcr.Cr <= inCr.Min )
                        ycbcr.Cr = outCr.Min;
                    else
                        ycbcr.Cr = kcr * ycbcr.Cr + bcr;

                    // convert back to RGB
                    AForge.Imaging.YCbCr.ToRGB( ycbcr, rgb );

                    ptr[RGB.R] = rgb.Red;
                    ptr[RGB.G] = rgb.Green;
                    ptr[RGB.B] = rgb.Blue;
                }
                ptr += offset;
            }
        }
    }
}
