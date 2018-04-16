using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System . Xml;
using System . Xml . Serialization;
using System . IO;


namespace DMT01
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class H_Slider_UserControl1 : UserControl
    {
        #region Persistance_classes1
        public class H_Slider_UserControl1_SaveState_Class
        {
            public DMT01.MainWindow.SeralizeControlCommonFields SSC = new DMT01 . MainWindow . SeralizeControlCommonFields ( );
            public String ControlClass = nameof ( H_Slider_UserControl1 );
            public String ControlName = String . Empty;
            public string StateFileName = String . Empty;
            public float MaxValue = ( float ) -1.1;
            public float MinValue = -2.2f;
            public float ResetValue = -3.3f;
        }
        //public  static H_Slider_UserControl1_SaveState_Class H_Slider_Static;
        #endregion Persistant_classes
        public H_Slider_UserControl1 ()
		{
            
            InitializeComponent ();

            H_Slider_UserControl1_SaveState_Class SS = new H_Slider_UserControl1_SaveState_Class ( );

			Debug.WriteLine(String.Format("{0} {1}", nameof(H_Slider_UserControl1), this.Name));
																										
		}

		public float SliderValue
		{
            get
          	{
				return (float)theH_Slider.Value;
			}

			set
			{
				if (theH_Slider.Value == value)
				{
					return;
				}

				theH_Slider.Value = value;
			}
            
		}
        
		public Double SliderMaxValue
		{
			get
			{
				return theH_Slider.Maximum;
			}
			set
			{
				if (theH_Slider.Maximum == value)
				{
					return;
				}

				theH_Slider.Maximum = value;
			}
		}

        public Double SliderMinValue
		{
			get
			{
				return theH_Slider.Minimum;
			}

			set
			{
				if (theH_Slider.Minimum == value)
				{
					return;
				}

				theH_Slider.Minimum = value;
			}
		}

        public String LabelText
		{
			get
			{
				return HSliderControlClusterMain_Label.Content as String;
			}

			set
			{
				if (HSliderControlClusterMain_Label.Content as String == value)
				{
					return;
				}

				HSliderControlClusterMain_Label.Content = value;
			}
		}

		private  void Slider_Low_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_TextChanged)));

		}

		private  void Slider_Low_TextBox_TextInput(object sender, TextCompositionEventArgs e)
		{
			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_TextInput)));

		}

		private  void Slider_Low_TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_MouseWheel)));

            TextBox TB = sender as TextBox;
			int Low = 0;
			if (int.TryParse(TB.Text, out Low))
            {
            }
            else
            {
            }

		}

		private  void theH_Slider_MouseWheel(object sender, MouseWheelEventArgs e)
		{
            var delta = e.Delta;
            var time = e.Timestamp;
            var device = e.Device;
            var middle_button = e.MiddleButton;
            var LeftButton = e.LeftButton;
            var RightButton = e.RightButton;
            var Source = e.Source;
            int sign = 0;
            if (delta < 0)
            {
                sign = -1;
                theH_Slider.Value--;
            }
            else if (delta == 0)
            {
                sign = 0;
            }
            else if (delta > 0)
            {
                sign = 1;
                theH_Slider.Value++;
            }
            else
            {
            }

            Debug.WriteLine(String.Format("{0} {1} {2}", nameof(theH_Slider_MouseWheel), delta, sign, theH_Slider.Value));

		}

        public void ResetValue_Click ( object sender , RoutedEventArgs e )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof( ResetValue_Click ) ) );

        }

        private void Save0_Button_Click ( object sender , RoutedEventArgs e )
            {

            Button t = sender as Button;
            DependencyObject ttt = t . Parent;
            Grid G = ttt as Grid;
            DependencyObject gg = G . Parent;
            H_Slider_UserControl1 ttyy = gg as H_Slider_UserControl1;
            Seralize_H__Slider_UserControl1_SaveState ( ttyy );

            }

        public static void Seralize_H__Slider_UserControl1_SaveState ( UIElement e )
            {
            H_Slider_UserControl1 HS = e as H_Slider_UserControl1;
            if ( HS == null )
                return;

            Seralize_H__Slider_UserControl1_SaveState ( HS );

            }
    public static void Seralize_H__Slider_UserControl1_SaveState ( H_Slider_UserControl1 ttyy )
        {
        string ControlName = ttyy . Name;
        String StateFileName = String . Format ( "{0}.xml" , ControlName );
        Seralize_H__Slider_UserControl1_SaveState ( ControlName: ControlName , StateFileName: StateFileName , ttyy: ttyy );
        }

    public static void Seralize_H__Slider_UserControl1_SaveState ( string ControlName , String StateFileName , H_Slider_UserControl1 ttyy )
        {
        var p =new H_Slider_UserControl1_SaveState_Class ();
            p . ControlClass = nameof ( H_Slider_UserControl1 );
            p . ControlName = ControlName;
            p . StateFileName = StateFileName;
            p . ResetValue = ttyy. SliderValue;
            p . MaxValue = ( float ) ttyy . SliderMaxValue;
            p . MinValue = ( float ) ttyy. SliderMinValue;

        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(p.GetType());

        XmlWriterSettings s = new XmlWriterSettings();
        s . Indent = true;
        s . NewLineOnAttributes = true;
        s . OmitXmlDeclaration = true;
        XmlWriter w=XmlWriter.Create(StateFileName,s);
            
        x . Serialize ( w , p );
        w . Close ( );     
        }
     }
}
