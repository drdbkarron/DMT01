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


namespace WpfControlLibrary2
{
    /// <summary>
    /// Interaction logic for Enum1ComboBoxUserControl.xaml
    /// </summary>
    public partial class Enum1ComboBoxUserControl : UserControl
    {
        public Enum1ComboBoxUserControl()
        {
            InitializeComponent();
        }

        public DMT01.LineStinkerClass xxxx;

        public DMT01.LineStinkerClass Value
        {
        get {
                return this.xxxx;
            }
        }

        private void myStankeyEnum_Combo_Box_Control_Initialized(object sender, EventArgs e)
        {
            this.myStankeyEnum_Combo_Box_Control.SelectedValuePath = "Key";
            this.myStankeyEnum_Combo_Box_Control.DisplayMemberPath = "Value";
            var a = Enum.GetNames(typeof(DMT01.LineStinkerClass));
            var b = Enum.GetValues(typeof(DMT01.LineStinkerClass));

            foreach (DMT01.LineStinkerClass Num in (DMT01.LineStinkerClass[])Enum.GetValues(typeof(DMT01.LineStinkerClass)))
            {

                System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "dippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
            }


            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
        }

        private void myStankeyEnum_Combo_Box_Control_Loaded(object sender, RoutedEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
        }

        private void myStankyEnumComboBoxUserControl_Initialized(object sender, EventArgs e)
        {

            System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} ", "snippy", (((System.Environment.StackTrace).Split('\n'))[2].Trim())));
        }
    }
}
