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

		private static void Slider_Low_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_TextChanged)));

		}

		private static void Slider_Low_TextBox_TextInput(object sender, TextCompositionEventArgs e)
		{
			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_TextInput)));

		}

		private static void Slider_Low_TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Debug.WriteLine(String.Format("{0}", nameof(Slider_Low_TextBox_MouseWheel)));
			TextBox TB = sender as TextBox;
			int Low = 0;
			if (int.TryParse(TB.Text, out Low)) { } else { };
				

		}
	}
}
