using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary1
{
	public partial class PointSizeComboBoxUserControlClass : UserControl
	{
		private int _selection;

		public PointSizeComboBoxUserControlClass()
		{
			InitializeComponent();
		}

		public int Selection
		{
			get
			{
				return this._selection;
			}
			set
			{
				this._selection = value;
			}
		}

		private void PointSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object WhatsThis =this.PointSizeComboBox.SelectedItem;
			ComboBoxItem ThisIsWhatItIs = WhatsThis as ComboBoxItem;
			object Content = ThisIsWhatItIs . Content;
			String ContentAsString = Content as String;
			Boolean werked = int . TryParse ( ContentAsString , out int Numbah );
			if ( !werked )
				return;
			_selection = Numbah;

			System.Diagnostics.Debug.WriteLine(String.Format("Numbah={0} {1} ", Numbah, (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
		}
	}
}
