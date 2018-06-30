using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Navigation;
using System . Windows . Shapes;

namespace WpfControlLibrary1
	{
	public partial class LineStinkerComboBoxUserControl : UserControl
		{
		public event EventHandler LineStinkerEnumValueChanged;
		private LineStinkerModes _mode;

		public LineStinkerComboBoxUserControl ( )
			{
			InitializeComponent ( );
			this . LineStinkerComboBox . SelectionChanged += this . LineStinkerComboBox_SelectionChanged;
			}

		private void LineStinkerComboBox_SelectionChanged ( object sender , SelectionChangedEventArgs e )
			{
			OnLineStinkerComboBoxUserControlSelectedChanged ( this , e );
			}

		public void OnLineStinkerComboBoxUserControlSelectedChanged ( object sender , EventArgs e )
			{
			if ( this. LineStinkerEnumValueChanged != null )
				{
				this . LineStinkerEnumValueChanged (this , e );
				}
			}

		public LineStinkerModes Mode {
			get
				{
				return this . _mode;
				}
			set
				{
				this . _mode = this . Mode;
				this .LineStinkerComboBox.SelectedItem = this . Mode;
				}
			}

		public String LabelString
			{
			get
				{
				return this . LineStinkerLabel . Content as String;
				}
			set
				{
				Run RU=new Run(this.LabelString);
				TextBlock TB=new TextBlock(RU);
				this . LineStinkerLabel . Content = TB;
				}
			}

		private void LineStinkerComboBox_Initialized ( object sender , EventArgs e )
			{
			var m=Enum.GetValues(typeof(LineStinkerModes));
			this . LineStinkerComboBox . ItemsSource  = m;
			this . LineStinkerComboBox . SelectedItem = this . _mode;
			}

		private void LineStinkerComboBox_DropDownClosed ( object sender , EventArgs e )
			{
			if ( this . LineStinkerComboBox . SelectedItem == null )
				{
				return;
				}

			this . _mode = ( LineStinkerModes ) this . LineStinkerComboBox . SelectedItem;
			}
		}
	}
