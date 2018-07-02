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
using System . Drawing . Text;
using System . Drawing;
using WpfColorFontDialog;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static WpfControlLibrary1.LineStinkerModes SampleEnumDataContext = WpfControlLibrary1.LineStinkerModes.FloatBetweenVerticies;

        public MainWindow()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));

            this.InitializeComponent();

			System .Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }

        private void myTestComboBox_Initialized(object sender, EventArgs e)
        {
			System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Arial");
			FamilyTypefaceCollection f=this.FontFamily.FamilyTypefaces;

			InstalledFontCollection IFC=new InstalledFontCollection();
			System .Drawing.FontFamily[] ff=IFC.Families;

				foreach ( System .Drawing .FontFamily fff in ff )
				{
				string uu=fff.Name;
				if ( uu == "" )
					{
					continue;
					}

				int i=this . myTestComboBox . Items . Add ( uu );
				}
			//this.myTestComboBox.ItemsSource = f;
			}

        private void myTextComboBoxValues_Initialized(object sender, EventArgs e)
        {
            Type T = typeof(WpfControlLibrary1.LineStinkerModes);
			Type tt=Enum.GetUnderlyingType( T );
            this.myTextComboBoxValues.Text = "Select One";

            Array Values = Enum.GetValues(T);
            this.myTextComboBoxValues.ItemStringFormat = "--{0}--";
            foreach (object v in Values)
            {
				string s = v.ToString();
                string S = ((int)(v)).ToString(" 000 ")+s;
                int n = this.myTextComboBoxValues.Items.Add(newItem: v);
            }
        }

        private void myTextComboBoxValues_DropDownClosed(object sender, EventArgs e)
        {
			int a = this.myTextComboBoxValues.SelectedIndex;
			object b = this.myTextComboBoxValues.SelectedItem;
			if ( b == null )
			{
				return;
			}

			Type T = typeof(WpfControlLibrary1.LineStinkerModes);
            WpfControlLibrary1.LineStinkerModes E = (WpfControlLibrary1.LineStinkerModes)b;
			string c = this.myTextComboBoxValues.SelectedValuePath;

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} \t{2} ", E, (int)E, ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }

		private void ButtyButton_Click ( object sender , RoutedEventArgs e )
			{
			ColorFontDialog dialog = new ColorFontDialog ( true , true , true )
			{
				Owner = this ,
				Font = FontInfo .GetControlFont ( this .TextBlockSample )
			};
			//dialog.FontSizes = new int[] { 10, 12, 14, 16, 18 };
			if ( dialog . ShowDialog ( ) == true )
				{
				FontInfo font = dialog.Font;
				if ( font != null )
					{
					FontInfo . ApplyFont ( this . TextBlockSample , font );
					}
				}
			}

		private void LineWidthComboBoxUserControlClass_Loaded ( object sender , RoutedEventArgs e )
		{
			System .Diagnostics .Debug .WriteLine ( String .Format ( "{0} {1} " , "snippy" ,  ( ( System .Environment .StackTrace ) .Split ( '\n' ) ) [ 2 ] .Trim ( )  ) );
		}
	}
}
