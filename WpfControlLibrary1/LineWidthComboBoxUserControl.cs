using System;
using System .Collections .Generic;
using System .Linq;
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
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class LineWidthComboBoxUserControl : UserControl
	{
		public int linewidth;
		public String labelString;

		internal LineWidthComboBoxUserControl ( )
		{

			InitializeComponent ( );
		}

		public string LabelString
		{
			get
			{
				return this .labelString;
			}

			set
			{
				this .labelString = value;
			}
		}

		public int Linewidth
		{
			get
			{
				return this .linewidth;
			}

			set
			{
				this .linewidth = value;

			}
		}

		private void LinewidthComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
		{
			ComboBox CB= sender as ComboBox;
			if ( CB == null )
			{
				return;
			}

			ComboBoxItem SelectedCBI=(ComboBoxItem)CB.SelectedItem;
			if ( SelectedCBI == null )
			{
				return;
			}

			object C=SelectedCBI.Content;
			if ( C == null )
			{
				return;
			}

			String S=C as String;
			if ( S == null )
			{
				return;
			}

			Boolean babuska=int.TryParse(S, out int Numbah);
			if ( babuska == false ) return;

			Linewidth = Numbah;

			System .Diagnostics .Debug .WriteLine ( String .Format ( "{0} {1} " , Linewidth , ( ( ( System .Environment .StackTrace ) .Split ( '\n' ) ) [ 2 ] .Trim ( ) ) ) );
		}
	}
}
