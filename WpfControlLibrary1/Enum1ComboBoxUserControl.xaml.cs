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
    public partial class Enum1ComboBoxUserControl : UserControl
    {
        public Enum1ComboBoxUserControl()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));

            this.InitializeComponent();

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }

        private void myStankeyEnum_Combo_Box_Control_Initialized(object sender, EventArgs e)
        {
			string [ ] a = Enum.GetNames(typeof(WpfControlLibrary1.LineStinkerClass));
			Array b = Enum.GetValues(typeof(WpfControlLibrary1.LineStinkerClass));

            foreach (WpfControlLibrary1.LineStinkerClass Num in (WpfControlLibrary1.LineStinkerClass[])Enum.GetValues(typeof(WpfControlLibrary1.LineStinkerClass)))
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "dippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
            }

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }

        private void myStankeyEnum_Combo_Box_Control_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }

        private void myStankyEnumComboBoxUserControl_Initialized(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", ((System.Environment.StackTrace).Split('\n'))[2].Trim()));
        }
    }
}
