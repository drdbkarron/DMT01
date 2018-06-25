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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static WpfControlLibrary2.LineStinkerModes SampleEnumDataContext = WpfControlLibrary2.LineStinkerModes.FloatBetweenVerticies;
        public MainWindow()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));

            this.InitializeComponent();

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
        }

        private void myTestComboBox_Initialized(object sender, EventArgs e)
        {
            this.myTestComboBox.Text = "Select Wisely";
			System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Arial");
			var f=FontFamily.FamilyTypefaces;
			
			InstalledFontCollection IFC=new InstalledFontCollection();

            this.myTestComboBox.ItemsSource = f;
        }

        private void myTextComboBoxValues_Initialized(object sender, EventArgs e)
        {
            Type T = typeof(WpfControlLibrary2.LineStinkerModes);
            var tt=Enum.GetUnderlyingType( T );
            this.myTextComboBoxValues.Text = "Select One";
           
            Array Values = Enum.GetValues(T);
            this.myTextComboBoxValues.ItemStringFormat = "--{0}--";
            foreach (object v in Values)
            {
                var s = v.ToString();
                string S = ((int)(v)).ToString(" 000 ")+s;
                int n = this.myTextComboBoxValues.Items.Add(newItem: v);
            }
        }

        private void myTextComboBoxValues_DropDownClosed(object sender, EventArgs e)
        {
            var a = myTextComboBoxValues.SelectedIndex;
            var b = myTextComboBoxValues.SelectedItem;
            Type T = typeof(WpfControlLibrary2.LineStinkerModes);
            WpfControlLibrary2.LineStinkerModes E = (WpfControlLibrary2.LineStinkerModes)b;
            var c = myTextComboBoxValues.SelectedValuePath;

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} \t{2} ", E, (int)E, (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
        }
    }
}
