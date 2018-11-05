using System;
using WpfControlLibrary1;

namespace DMT01
{
	public class Region
		{
		public struct SpanStruct
		{
			public int LoHi;
		};
		public SpanStruct Row,Col;
		public DMT01.MainWindow MW=  ( DMT01 . MainWindow ) System. Windows. Application . Current . MainWindow;
		public int RegionalHits;
		public double Max;
		public double Min;
		public int N;
		public int Start_Rows;
		public int End_Rows;
		public int Start_Cols;
		public int End_Cols;
		public float[,] Cells;
		public Boxel[,] B;
		public Boxel[,] BorderB;
		public Boxel[] RecursionSeedBoxel;
		public byte[] Labels=new byte[8];
		public long Titration_Steps;

		public static Region Selected_Region;

		public Region ( float [ , ] C , int StartRows , int EndRows , int StartCols , int EndCols )
			{
			this . Start_Rows = StartRows;
			this . End_Rows = EndRows;
			this . Start_Cols = StartCols;
			this . End_Cols = EndCols;
			this . Max = -double . MaxValue;
			this . Min = double . MaxValue;
			this . Cells = C;
			this . B = new Boxel [ this . End_Cols , this . End_Rows ];

			for ( int j = this . Start_Rows ; j < this . End_Rows ; j++ )
				{
				for ( int i = this . Start_Cols ; i < this . End_Cols ; i++ )
					{
					double v = this.Cells[i,j];
					if ( v > this . Max )
						{
						this . Max = v;

						//System . Diagnostics . Debug . WriteLine ( String . Format ( "Max: {0} {1} " , this . Max , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
						}
					if ( v < 0 )
						{
						//System . Diagnostics . Debug . WriteLine ( String . Format ( "no negativity please {0} at [{1},{2}] {3} " , v, i, j , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
						this . Cells [ i , j ] = 0.0f;
						}
					else
					if ( v < this . Min )
						{
						this . Min = v;

						//System . Diagnostics . Debug . WriteLine ( String . Format ( "Min: {0} {1} " , this . Min , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
						}
					this . N++;
					}
				}
			this . MW . region_threshold_H_Slider_UserControl . SliderMinValue = this . Min;
			this . MW . region_threshold_H_Slider_UserControl . SliderMaxValue = this . Max;
			this . MW . CriticalitySweeper_LOW_H_Slider_User_Control . SliderValue = (float)this . Min;
			this . MW . CriticalitySweeper_LOW_H_Slider_User_Control . SliderMinValue = this . Min;
			this . MW . CriticalitySweeper_LOW_H_Slider_User_Control . SliderMaxValue = this . Max;
			this . MW . CriticalitySweeper_HIGH_H_Slider_User_Control . SliderMaxValue = this . Max;
			this . MW . CriticalitySweeper_HIGH_H_Slider_User_Control . SliderMinValue = this.Min;
			this . MW . CriticalitySweeper_HIGH_H_Slider_User_Control . SliderMaxValue = this . Max;
			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , Display() , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private String Display ( )
			{
			const String Stranger = "00.000";
			int Rows = this.End_Rows - this.Start_Rows;
			int Cols = this.End_Cols - this.Start_Cols;
			int n = Rows * Cols;
			if ( n != this . N )
				{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "badness heree" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
				}

			String a = String . Format ( "{0} in {1}->{2}, {3} Rows; {4}->{5},{6} Cols; Min:{7} Max: {8}" ,
				this.N,
				this.Start_Rows, this.End_Rows, Rows,
				this.Start_Cols, this.End_Cols, Cols,
				this.Min.ToString(Stranger), this.Max.ToString(Stranger));

			return a;
			}

		public void LoadRegionIntoQuadSelector ( QuadDropDownRegionSelector Q )
		{
			if ( Q == null )
			{
				return;
			}

			Q .RowLo= this .Start_Rows;
			Q .RowHi = this .End_Rows;
			Q .ColLo = this .Start_Cols;
			Q .ColHi = this .End_Cols;
			Q .RowLowComboBox .Items .Clear ( );
			Q .RowHiComboBox .Items .Clear ( );
			Q .ColLowComboBox .Items .Clear ( );
			Q .ColHiComboBox .Items .Clear ( );
			for ( int i = Q .RowLo ; i < Q .RowHi ; i++ )
			{
				Q .RowLowComboBox .Items .Add ( i );
				Q .RowHiComboBox .Items .Add ( i );
			}
			for ( int i = Q .ColLo ; i < Q .ColHi ; i++ )
			{
				Q .ColLowComboBox .Items .Add ( i );
				Q .ColLowComboBox .Items .Add ( i );
			}
		}
	}
	}

