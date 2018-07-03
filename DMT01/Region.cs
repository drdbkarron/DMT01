using System;

namespace DMT01
{
	public class Region
		{
		public DMT01.MainWindow MW=  ( DMT01 . MainWindow ) System. Windows. Application . Current . MainWindow;
		public double Max;
		public double Min;
		public int N;
		public int Start_Rows;
		public int End_Rows;
		public int Start_Cols;
		public int End_Cols;
		public float[,] Cells;

		public Region ( float [ , ] C , int StartRows , int EndRows , int StartCols , int EndCols )
			{
			this . Start_Rows = StartRows;
			this . End_Rows = EndRows;
			this . Start_Cols = StartCols;
			this . End_Cols = EndCols;
			this . Max = -double . MaxValue;
			this . Min = double . MaxValue;
			this . Cells = C;

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
		}
	}

