using System;
using System .Collections .Generic;
using System .Linq;
using System .Runtime .CompilerServices;
using System .Text;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Controls;
using System .Windows .Data;
using System .Windows .Documents;
using System .Windows .Input;
using System .Windows .Media;
using System .Windows .Media .Imaging;
using System .Windows .Navigation;
using System .Windows .Shapes;

namespace WpfControlLibrary1
{
	/// <summary>
	/// Interaction logic for QuadDropDownRegionSelector.xaml
	/// </summary>
	public partial class QuadDropDownRegionSelector : UserControl 
	{
		private int colHi;
		private int rowLo;
		private int rowHi;
		private int colLo;
		public ComboBox RowLowComboBox;
		public ComboBox RowHiComboBox;
		public ComboBox ColLowComboBox;
		public ComboBox ColHiComboBox;

		public QuadDropDownRegionSelector ( )
		{
			InitializeComponent ( );
			this .RowLowComboBox = this .Row_Low_ComboBox;
			this .RowHiComboBox = this .Row_High_ComboBox;
			this .ColLowComboBox = this .Col_Low_ComboBox;
			this .ColHiComboBox = this .Col_High_ComboBox;
			ColHiChanged += this .QuadDropDownRegionSelector_ColHiChanged;
			ColLoChanged += this .QuadDropDownRegionSelector_ColLoChanged;
			RowHiChanged += this .QuadDropDownRegionSelector_RowHiChanged;
			RowLoChanged += this .QuadDropDownRegionSelector_RowLoChanged;

		}

		private void QuadDropDownRegionSelector_RowLoChanged ( object sender , EventArgs e )
		{
			//throw new NotImplementedException ( );
		}

		private void QuadDropDownRegionSelector_RowHiChanged ( object sender , EventArgs e )
		{
			//throw new NotImplementedException ( );
		}

		private void QuadDropDownRegionSelector_ColLoChanged ( object sender , EventArgs e )
		{
			//throw new NotImplementedException ( );
		}

		private void QuadDropDownRegionSelector_ColHiChanged ( object sender , EventArgs e )
		{
			//throw new NotImplementedException ( );
		}

		public int RowLo
		{
			get
			{
				return this .rowLo;
			}

			set
			{
				this .rowLo = value;
				OnRowLoChanged ( EventArgs .Empty );
			}
		}

		protected virtual void OnRowLoChanged ( EventArgs e )
		{
			RowLoChanged?.Invoke ( this , e );
		}

		public event EventHandler RowLoChanged;

		public int RowHi
		{
			get
			{
				return this .rowHi;
			}

			set
			{
				this .rowHi = value;
				OnRowHiChanged ( EventArgs .Empty );
			}
		}

		protected virtual void OnRowHiChanged ( EventArgs e )
		{
			RowHiChanged?.Invoke ( this , e );
		}

		public event EventHandler RowHiChanged;

		public int ColLo
		{
			get
			{
				return this .colLo;
			}

			set
			{
				this .colLo = value;
				OnColLoChanged ( EventArgs .Empty );
			}
		}

		protected virtual void OnColLoChanged ( EventArgs e )
		{
			ColLoChanged?.Invoke ( this , e );
		}

		public event EventHandler ColLoChanged;

		public int ColHi
		{
			get
			{
				return this .colHi;
			}

			set
			{
				this .colHi = value;
				OnColHiChanged ( EventArgs .Empty );
			}
		}

		public void OnColHiChanged ( EventArgs e )
		{
			ColHiChanged?.Invoke ( this , e );
		}

		public event EventHandler ColHiChanged;

		private void ComboBoxesGrid_Loaded ( object sender , RoutedEventArgs e )
		{

			System .Diagnostics .Debug .WriteLine ( String .Format ( "{0} {1} " , "snippy" , ( ( ( System .Environment .StackTrace ) .Split ( '\n' ) ) [ 2 ] .Trim ( ) ) ) );
		}

		private void ComboBoxesGrid_Initialized ( object sender , EventArgs e )
		{

			System .Diagnostics .Debug .WriteLine ( String .Format ( "{0} {1} " , "snippy" , ( ( ( System .Environment .StackTrace ) .Split ( '\n' ) ) [ 2 ] .Trim ( ) ) ) );
		}

		private void Row_Low_ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ComboBox CB = sender as ComboBox;
			if ( CB == null )
				return;
			Object Selected = CB .SelectedItem;
			if ( Selected == null )
				return;
			String SelectedAsString = (String)Selected;
			if ( SelectedAsString == null )
				return;
			Boolean werked = int .TryParse ( s: SelectedAsString , result: out int Numbah );
			if ( !werked )
				return;
			if ( Numbah >= RowHi )
				return;
			RowLo = Numbah;
		}

		private void Row_High_ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ComboBox CB = sender as ComboBox;
			if ( CB == null )
				return;
			Object Selected = CB .SelectedItem;
			if ( Selected == null )
				return;
			String SelectedAsString = Selected as String;
			if ( SelectedAsString == null )
				return;
			Boolean werked = int .TryParse ( s: SelectedAsString , result: out int Numbah );
			if ( !werked )
				return;
			if ( Numbah <= RowLo )
				return;
			RowHi = Numbah;

		}

		private void Col_Low_ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ComboBox CB = sender as ComboBox;
			if ( CB == null )
				return;
			Object Selected = CB .SelectedItem;
			if ( Selected == null )
				return;
			String SelectedAsString = Selected as String;
			if ( SelectedAsString == null )
				return;
			Boolean werked = int .TryParse ( s: SelectedAsString , result: out int Numbah );
			if ( !werked )
				return;
			if ( Numbah <= ColLo )
				return;
			ColLo = Numbah;
		}

		private void Col_High_ComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ComboBox CB = sender as ComboBox;
			if ( CB == null )
				return;
			Object Selected = CB .SelectedItem;
			if ( Selected == null )
				return;
			String SelectedAsString = Selected as String;
			if ( SelectedAsString == null )
				return;
			Boolean werked = int .TryParse ( s: SelectedAsString , result: out int Numbah );
			if ( !werked )
				return;
			if ( Numbah <= ColHi )
				return;
			ColHi = Numbah;

		}
	}
}
