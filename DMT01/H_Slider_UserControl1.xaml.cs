﻿using System;
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


namespace DMT01
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class H_Slider_UserControl1 : UserControl
	{
		public H_Slider_UserControl1()
		{
			InitializeComponent();

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
        public static float SliderDefaultValue;

		public Double SliderMaxValue
		{
			get
			{
				return theH_Slider.Maximum;
			}
			set
			{
				theH_Slider.Maximum = value;
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
            SliderValue = SliderDefaultValue;
        }
    }
}
