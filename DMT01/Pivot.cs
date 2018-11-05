using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using unvell . ReoGrid;
using unvell . ReoGrid . Actions;
using unvell . ReoGrid . CellTypes;
using unvell . ReoGrid . DataFormat;

namespace DMT01
{
	static class Pivot
	{
		static public void SwapRow ( CheckBoxCell CBC )
		{
			Worksheet Snatcheroo = CBC . Cell . Worksheet;
			SwapRow ( Snatcheroo , CBC );
		}
		static void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC )
		{
			int R = CBC . Cell . Row;
			SwapRow ( Snatcheroo , CBC , R );
		}

		public static void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC , int R )
		{
			int r = -1;
			int colMax = -1;
			GetActualizedRange ( CW: Snatcheroo , maxRow: out r , maxCol: out colMax );
			SwapRow ( Snatcheroo: Snatcheroo , CBC: CBC , R: R , r: r , colMax: colMax );
		}

		public static void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC , int R , int r , int colMax )
		{
			RangePosition RR = new RangePosition ( row: R , col: 1 , rows: 1 , cols: colMax );

			Snatcheroo . InsertRows ( row: R , count: 1 );
			CellPosition cP = new CellPosition ( row: R , col: 0 );

			CopyRangeAction action0 = new unvell . ReoGrid . Actions . CopyRangeAction ( fromRange: RR , toPosition: cP );
			DMT01.MainWindow.myReoGridControl_STATIC . DoAction ( action0 );
			RemoveRowsAction action1 = new unvell . ReoGrid . Actions . RemoveRowsAction ( row: R , rows: 1 );
			DMT01.MainWindow.myReoGridControl_STATIC . DoAction ( action1 );
		}

		public static void GetActualizedRange ( unvell . ReoGrid . Worksheet CW , out int maxRow , out int maxCol )
		{
			unvell . ReoGrid . RangePosition ColumnCaptionRange = new unvell . ReoGrid . RangePosition ( row: 0 , col: 0 , rows: 1 , cols: CW . ColumnCount );
			int maxCaptionCol = -1;
			CW . IterateCells ( range: ColumnCaptionRange , iterator: ( row , col , cell ) =>
			{
				if ( cell . DataFormat == CellDataFormatFlag . Text )
				{
					string text = cell . GetData<String> ( );
					if ( text == null )
					{
						return false;
					}

					maxCaptionCol = col;
					return true;
				}
				if ( cell . DataFormat == CellDataFormatFlag . General )
				{
					string text = cell . GetData<String> ( );
					if ( text == null )
					{
						return false;
					}

					maxCaptionCol = col;
					return true;
				}
				return false;
			}
			 );

			maxCol = maxCaptionCol;

			unvell . ReoGrid . RangePosition RowCaptionRange = new unvell . ReoGrid . RangePosition ( row: 0 , col: 0 , rows: CW . RowCount , cols: 1 );
			int maxCaptionRow = -1;
			CW . IterateCells ( range: RowCaptionRange , iterator: ( row , col , cell ) =>
			{
				if ( cell . DataFormat == CellDataFormatFlag . Text )
				{
					string text = cell . GetData<String> ( );
					if ( text == null )
					{
						return false;
					}

					maxCaptionRow = row;
					return true;
				}
				if ( cell . DataFormat == CellDataFormatFlag . General )
				{
					string text = cell . GetData<String> ( );
					if ( text == null )
					{
						return false;
					}

					maxCaptionRow = row;
					return true;
				}
				return false;
			}
			);

			maxRow = maxCaptionRow;

			return;
		}
	}
}
