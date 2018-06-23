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
  
    public class H_Slider_UserControl1_SaveState_Class
        {
            public DMT01.SeralizeControlCommonFields SeralizeControlCommonFields = new DMT01 .SeralizeControlCommonFields ( );
            public float MaxValue =   -1.1f;
            public float MinValue =   -2.2f;
            public float ResetValue = -3.3f;
        }

    public partial class H_Slider_UserControl1 : UserControl
    {
    public H_Slider_UserControl1 ()
		{
             
        InitializeComponent ();

        //H_Slider_UserControl1_SaveState_Class SS = new H_Slider_UserControl1_SaveState_Class ( );

        // Debug . WriteLine ( String . Format ( "{0} {1} {3}" , this.Name , this . LabelText, this.HSliderControlClusterMain_Label ) );
				
        }

	public float SliderValue
	{
        get
        {
			return (float)this.theH_Slider.Value;
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

    public void ResetValue_Click ( object sender , RoutedEventArgs e )
        {
            Button t = sender as Button;
            DependencyObject ttt = t . Parent;
            System . Windows . Controls . Grid G = ttt as System . Windows . Controls . Grid;
            DependencyObject gg = G . Parent;
            H_Slider_UserControl1 ttyy = gg as H_Slider_UserControl1;
            Debug . WriteLine ( String . Format ( "{0} {1} {2} reset to " , nameof( ResetValue_Click ) , ttyy.Name,ttyy.SliderValue) );
            Deseralize_H_Slider_UserControl1 ( ttyy );
            Debug . WriteLine ( String . Format ( "{2}" , nameof ( ResetValue_Click ) , ttyy . Name , ttyy . SliderValue ) );

            }

    private void Save0_Button_Click ( object sender , RoutedEventArgs e )
            {
            Button t = sender as Button;
            DependencyObject ttt = t . Parent;
            System . Windows . Controls . Grid G = ttt as System . Windows . Controls . Grid;
            DependencyObject gg = G . Parent;
            H_Slider_UserControl1 ttyy = gg as H_Slider_UserControl1;
            Debug . WriteLine ( String . Format ( "{0} Seralizing/Saving {1} {2}  " , nameof ( Save0_Button_Click ) , ttyy . SliderValue, ttyy . Name  ) );
            Seralize_H__Slider_UserControl1_SaveState ( ttyy );
            Debug . WriteLine ( String . Format ( "{0}.xml in {1}" , ttyy.Name, Environment.CurrentDirectory ) );
            }

    public static void Seralize_H__Slider_UserControl1_SaveState ( UIElement e )
            {
            H_Slider_UserControl1 HS = e as H_Slider_UserControl1;
            if ( HS == null )
                {
                return;
                }

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
        p . SeralizeControlCommonFields. ControlClass = nameof ( H_Slider_UserControl1 );
        p . SeralizeControlCommonFields . ControlName = ControlName;
        p . SeralizeControlCommonFields . SaveStateFileName = StateFileName;
        p . ResetValue = ttyy. SliderValue;
        p . MaxValue = ( float ) ttyy . SliderMaxValue;
        p . MinValue = ( float ) ttyy. SliderMinValue;

            XmlSerializer x = new XmlSerializer ( p.GetType());

        XmlWriterSettings s = new XmlWriterSettings();
        s . Indent = true;
        s . NewLineOnAttributes = true;
        s . OmitXmlDeclaration = false;
        XmlWriter w=XmlWriter.Create(StateFileName,s);
            
        x . Serialize ( w , p );
        w . Close ( );     
        }

    public static H_Slider_UserControl1_SaveState_Class Deseralize_H_Slider_UserControl1 (H_Slider_UserControl1 HC )
            {
            String F = String . Format ( "{0}.xml" , HC . Name );
            if ( !System . IO . File . Exists ( F ) )
			{
				return null;
			}

			return Deseralize_H_Slider_UserControl1 ( F , HC );
            }

    public static H_Slider_UserControl1_SaveState_Class Deseralize_H_Slider_UserControl1 ( String F , H_Slider_UserControl1 HC )
        {
        Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( Deseralize_H_Slider_UserControl1 ) , F ) );

        if ( !System . IO . File . Exists ( F ) )
			{
				return null;
			}

			String XmlFileContents = System . IO . File . ReadAllText ( F );

        StringReader XmlStringReader = new StringReader ( XmlFileContents );

        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
            {
            IgnoreComments = true ,
            IgnoreProcessingInstructions = true ,
            IgnoreWhitespace = true
            };

        XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );
        while ( !xmlReader . EOF )
            {
            switch ( xmlReader . NodeType )
                {
                case XmlNodeType . Attribute:
                case XmlNodeType . CDATA:
                case XmlNodeType . Comment:
                case XmlNodeType . Document:
                case XmlNodeType . DocumentFragment:
                case XmlNodeType . DocumentType:

                case XmlNodeType . Element:

                H_Slider_UserControl1_SaveState_Class pp = new H_Slider_UserControl1_SaveState_Class ( );
                XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );

                var o = x . Deserialize ( xmlReader );
                pp = ( H_Slider_UserControl1_SaveState_Class ) o;

                HC . SliderValue = pp . ResetValue;
                HC . SliderMaxValue = pp . MaxValue;
                HC . SliderMinValue = pp . MinValue;
                    return pp;

                case XmlNodeType . EndElement:
                case XmlNodeType . EndEntity:
                case XmlNodeType . Entity:
                case XmlNodeType . EntityReference:
                case XmlNodeType . None:
                case XmlNodeType . Notation:
                case XmlNodeType . ProcessingInstruction:
                case XmlNodeType . SignificantWhitespace:
                case XmlNodeType . Text:
                case XmlNodeType . Whitespace:
                case XmlNodeType . XmlDeclaration:
                    break;

                default:
                    break;
                }

            Boolean State = xmlReader . Read ( );
            }

    return null;
    }

    private void Minus_Spread_Min_Max_Button_Click ( object sender , RoutedEventArgs e )
        {
        theH_Slider . Minimum++;
        theH_Slider . Maximum--;
        if ( this . theH_Slider . Minimum >= this . theH_Slider . Maximum )
            {
				this . theH_Slider . Maximum = theH_Slider . Minimum + 5.0;
            }
        }

    private void Plus_Spread_Min_Max_Button_Click ( object sender , RoutedEventArgs e )
            {
            theH_Slider . Maximum++;
            theH_Slider . Minimum--;
            if ( theH_Slider . Maximum >= theH_Slider . Minimum )
                {
                theH_Slider . Minimum = theH_Slider . Maximum - 5.0;
                }
            }
        }
}
