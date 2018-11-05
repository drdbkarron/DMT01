using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Drawing;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Media . Imaging;
using unvell . ReoGrid . Rendering;

namespace DMT01
{
	class MyCheckBox : unvell . ReoGrid . CellTypes . CheckBoxCell
	{
		System . Windows . Media . Imaging . BitmapSource UpArrow ;
		System . Windows . Media . Imaging . BitmapSource DownArrow ;

		public MyCheckBox ( )
		{
			System . Drawing . Bitmap ua = DMT01 . Properties . Resources . UpArrowBMP;
			this . UpArrow = ConvertBitmap ( ua );
			System . Drawing . Bitmap da = DMT01 . Properties . Resources . down_arrowBMP;
			this . DownArrow = ConvertBitmap ( da );
		}

		protected override void OnContentPaint ( CellDrawingContext dc )
		{
			if ( this . IsChecked )
			{
				Debug . WriteLine ( String . Format ( "{0}" , nameof ( OnContentPaint ) ) );

				dc . Graphics . DrawImage ( image: this . UpArrow , rect: this . ContentBounds );
			}
			else
			{
				Debug . WriteLine ( String . Format ( "{0}" , nameof ( OnContentPaint ) ) );

				dc . Graphics . DrawImage ( image: this . DownArrow , rect: this . ContentBounds );
			}
		}

		public static BitmapSource ConvertBitmap ( Bitmap source )
		{
			BitmapSource xx = System . Windows . Interop . Imaging . CreateBitmapSourceFromHBitmap (
						  source . GetHbitmap ( ) ,
						  IntPtr . Zero ,
						  System . Windows . Int32Rect . Empty ,
						  BitmapSizeOptions . FromEmptyOptions ( ) );
			return xx;
		}
	}

}
