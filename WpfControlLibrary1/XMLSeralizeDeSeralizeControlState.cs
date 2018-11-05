using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Diagnostics . Contracts;
using System . IO;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Windows;
using System . Windows . Controls;
using System . Xml;
using System . Xml . Serialization;
using WpfControlLibrary1;

namespace WpfControlLibrary1
{

	//public WpfControlLibrary2.LineStinkerModes Stankey= WpfControlLibrary2.LineStinkerModes.StartAtFarVertex;

	[Serializable ( )]
	public class DMT_Main_Window_Control_SaveState
	{
		public DMT_Main_Window_Control_SaveState ( )
		{
			this . CommonFields = new SeralizeControlCommonFields ( );
		}

		[System.Xml.Serialization.XmlElement ( "SeralizeControlCommonFields" )]
		public WpfControlLibrary1.SeralizeControlCommonFields CommonFields;

		public double Left;
		public double Top;
	}

	[Serializable]
	public class CheckBoxControlSaveState
	{
		public CheckBoxControlSaveState ( )
		{
			this . CommonFields = new SeralizeControlCommonFields ( );
		}

		[XmlElement ( "SeralizeControlCommonFields" )]
		public SeralizeControlCommonFields CommonFields;

		public Boolean CheckBoxState;
		public String CheckBoxName;
	}

	[Serializable]
	public class RadioCheckBoxSaveState
	{
		[XmlElement ( "SeralizeControlCommonFields" )]
		public WpfControlLibrary1.SeralizeControlCommonFields CommonFields;

		public Boolean RadioCheckBoxState;
		public String RadioCheckBoxName;
		public String RadioGroupName;

		public RadioCheckBoxSaveState ( )
		{
			this . CommonFields = new SeralizeControlCommonFields ( );
		}
	}

	[Serializable]
	public class TabControlSaveState
	{
		[XmlElement ( "SeralizeControlCommonFields" )]
		public SeralizeControlCommonFields CommonFields=new SeralizeControlCommonFields();

		public short SelectedIndex;
	}

	public class XMLSeralizeDeSeralizeControlState
	{
		public XMLSeralizeDeSeralizeControlState ( )
		{
		}
		#region Serializers
		private struct SavedControl
		{
			public int MainWindows;
			public int H_Sliders;
			public int CheckButtons;
			public int RadioButtons;
			public int SheetTabControl;
			public int OtherControls;
			public int Calls;
			public int MaxDepth;
		};

		static String XmlFileContents;
		static String XmlFileName;
		static SavedControl SC;

		static private void InitalizeSavedControl ( )
		{
			SC . MainWindows = 0;
			SC . H_Sliders = 0;
			SC . CheckButtons = 0;
			SC . RadioButtons = 0;
			SC . SheetTabControl = 0;
			SC . OtherControls = 0;
			SC . Calls = 0;
			SC . MaxDepth = -1;
		}
		static public Window MW;
		static public void WalkLogicalTree (UIElement DMT_Main_Window )
		{
			MW=DMT_Main_Window as Window;
			InitalizeSavedControl ( );

			Debug . WriteLine ( String . Format ( "{0} Starting " , nameof ( WalkLogicalTree ) ) );

			WalkLogicalTree ( DMT_Main_Window as FrameworkElement , 0 );

			Debug . WriteLine ( String . Format ( "{0} Summary Stats" , nameof ( WalkLogicalTree ) ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MainWindows ) , SC . MainWindows ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . H_Sliders ) , SC . H_Sliders ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . CheckButtons ) , SC . CheckButtons ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . RadioButtons ) , SC . RadioButtons ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . SheetTabControl ) , SC . SheetTabControl ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . OtherControls ) , SC . OtherControls ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MaxDepth ) , SC . MaxDepth ) );
			Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . Calls ) , SC . Calls ) );
			Debug . WriteLine ( String . Format ( "{0} completed " , nameof ( WalkLogicalTree ) ) );
		}

		static public void WalkLogicalTree ( FrameworkElement f , int Depth )
		{
			SC . Calls++;
			if ( Depth > SC . MaxDepth )
			{
				SC . MaxDepth = Depth;
			}

			if ( f == null )
			{
				return;
			}

#pragma warning disable CS0162 // Unreachable code detected
			if ( false )
			{
				String NameString = GetName ( f );
				String TypeString = f . GetType ( ) . ToString ( );
				Debug . WriteLine ( String . Format ( "{0} Framework Element {1} {2} {3} {4}" ,
					nameof ( WalkLogicalTree ) , SC . Calls , Depth , TypeString , NameString ) );
			}
#pragma warning restore CS0162 // Unreachable code detected

			System . Collections . IEnumerable children = LogicalTreeHelper . GetChildren ( f );
			foreach ( object child in children )
			{
				FrameworkElement FE = child as FrameworkElement;

				WalkLogicalTree ( FE , Depth + 1 );
			}

			IsControlStateSavable ( f );
		}

		static private void IsControlStateSavable ( UIElement e )
		{
			String NameString = GetName ( e );

			if ( NameString . EndsWith ( "DMT_Main_Window_Control" ) )
			{
				DMT_Main_Window_Control_SaveState_Seralize ( e );
				SC . MainWindows++;
				return;
			}

			String TypeString = e . GetType ( ) . ToString ( );

			if ( TypeString . EndsWith ( nameof ( H_Slider_UserControl1 ) ) )
			{
				H_Slider_UserControl1 . Seralize_H__Slider_UserControl1_SaveState ( e );
				SC . H_Sliders++;
				return;
			}

			if ( TypeString . EndsWith ( "SheetTabControl" ) )
			{
				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , TypeString , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				SheetTabControlSeralize ( e );
				SC . SheetTabControl++;
				return;
			}

			if ( TypeString . EndsWith ( "CheckBox" ) )
			{
				CheckBoxSerialize ( NameString , e );
				SC . CheckButtons++;
				return;
			}

			if ( TypeString . EndsWith ( "RadioButton" ) )
			{
				RadioButtonSerialize ( NameString , e );
				SC . RadioButtons++;
				return;
			}

			SC . OtherControls++;
		}

		static private void SheetTabControlSeralize ( UIElement e )
		{
			if ( !( e is FrameworkElement FE ) )
			{
				return;
			}

			if ( !( FE is TabControl TC ) )
			{
				return;
			}

			String nameString = TC . Name;

			String StateFileName = String . Format ( "{0}.xml" , nameString );

			TabControlSaveState TCS = new TabControlSaveState ( );
			TCS . CommonFields . SaveStateFileName = StateFileName;
			TCS . CommonFields . ControlClass = nameof ( TabControl );
			TCS . CommonFields . ControlName = TC . Name;
			TCS . CommonFields . UpdatedFromXmlFiles = true;

			XmlSerializer x = new XmlSerializer ( TCS . GetType ( ) );

			XmlWriterSettings s = new XmlWriterSettings
			{
				Indent = true ,
				NewLineOnAttributes = true ,
				OmitXmlDeclaration = true
			};
			XmlWriter w = XmlWriter . Create ( StateFileName , s );

			x . Serialize ( w , TCS );
			w . Close ( );
		}

		static private void RadioButtonSerialize ( string nameString , UIElement e )
		{
			if ( !( e is RadioButton RB ) )
			{
				return;
			}

			String StateFileName = String . Format ( "{0}.xml" , nameString );

			RadioCheckBoxSaveState p = new RadioCheckBoxSaveState ( );
			p . CommonFields . SaveStateFileName = StateFileName;
			p . CommonFields . ControlClass = nameof ( RadioButton );
			p . CommonFields . ControlName = RB . Name;
			p . CommonFields . UpdatedFromXmlFiles = true;

			p . RadioCheckBoxState = RB . IsChecked . Value;
			p . RadioCheckBoxName = RB . Name;
			p . RadioGroupName = RB . GroupName;

			XmlSerializer x = new XmlSerializer ( p . GetType ( ) );

			XmlWriterSettings s = new XmlWriterSettings
			{
				Indent = true ,
				NewLineOnAttributes = true ,
				OmitXmlDeclaration = true
			};
			XmlWriter w = XmlWriter . Create ( StateFileName , s );

			x . Serialize ( w , p );
			w . Close ( );
		}

		static private void CheckBoxSerialize ( string nameString , UIElement E )
		{
			if ( !( E is CheckBox CB ) )
			{
				return;
			}

			if ( nameString == "" )
			{
				nameString = CB . Name;
			}

			if ( nameString == "" )
			{
				String Content = CB . Content as String;
				System . Diagnostics . Debug . WriteLine ( String . Format (
					"{0} contents <{1}> {2} " ,
					"null name string" ,
					Content ,
					( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
				return;
			}

			String StateFileName = String . Format ( "{0}.xml" , nameString );

			CheckBoxControlSaveState p = new CheckBoxControlSaveState ( );
			p . CommonFields . SaveStateFileName = StateFileName;
			p . CommonFields . ControlClass = nameof ( CheckBox );
			p . CommonFields . ControlName = CB . Name;
			p . CommonFields . UpdatedFromXmlFiles = true;

			p . CheckBoxState = CB . IsChecked . Value;
			p . CheckBoxName = CB . Name;

			XmlSerializer x = new XmlSerializer ( p . GetType ( ) );

			XmlWriterSettings s = new XmlWriterSettings
			{
				Indent = true ,
				NewLineOnAttributes = true ,
				OmitXmlDeclaration = true
			};
			XmlWriter w = XmlWriter . Create ( StateFileName , s );

			x . Serialize ( w , p );
			w . Close ( );

			System . Diagnostics . Debug . WriteLine ( String . Format ( "Wrote {0} at {1} " , StateFileName , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
		}

		static private void RadioCheckBoxSerialize ( string nameString , UIElement E )
		{
			if ( !( E is RadioButton RB ) )
			{
				return;
			}

			String StateFileName = String . Format ( "{0}.xml" , nameString );

			RadioCheckBoxSaveState p = new RadioCheckBoxSaveState ( );
			p . CommonFields . SaveStateFileName = StateFileName;
			p . CommonFields . ControlName = RB . Name;
			p . CommonFields . ControlClass = nameof ( RadioButton );
			p . CommonFields . UpdatedFromXmlFiles = true;

			p . RadioCheckBoxState = RB . IsChecked . Value;
			p . RadioCheckBoxName = RB . Name;
			p . RadioGroupName = RB . GroupName;

			XmlSerializer x = new XmlSerializer ( p . GetType ( ) );

			XmlWriterSettings s = new XmlWriterSettings
			{
				Indent = true ,
				NewLineOnAttributes = true ,
				OmitXmlDeclaration = true
			};
			XmlWriter w = XmlWriter . Create ( StateFileName , s );

			x . Serialize ( w , p );
			w . Close ( );
		}

		static private void DMT_Main_Window_Control_SaveState_Seralize (UIElement E )
		{
			FrameworkElement FE=E as FrameworkElement;
			String StateFileName = String . Format ( "{0}.xml" ,FE.Name );

			DMT_Main_Window_Control_SaveState p = new DMT_Main_Window_Control_SaveState ( );
			p . CommonFields . ControlClass = nameof ( Window );
			p . CommonFields . ControlName = FE.Name;
			p . CommonFields . SaveStateFileName = StateFileName;
			p . CommonFields . UpdatedFromXmlFiles = true;
			Window W=FE as Window;

			p . Left = W . Left;
			p . Top = W . Top;

			XmlSerializer x = new XmlSerializer ( p . GetType ( ) );

			XmlWriterSettings s = new XmlWriterSettings
			{
				Indent = true ,
				NewLineOnAttributes = true ,
				OmitXmlDeclaration = true
			};
			XmlWriter w = XmlWriter . Create ( StateFileName , s );

			x . Serialize ( w , p );
			w . Close ( );
		}

		static private String GetName ( UIElement E )
		{
			FrameworkElement element = E as FrameworkElement;
			if ( element != null )
			{
				return element . Name;
			}

			try
			{
				String S = E . GetType ( ) . GetProperty ( "Name" ) . GetValue ( E , null ) as String;
				return S;
			}
			catch
			{
				// Last of all, try reflection to get the value of a Name field.
				try
				{
					return ( string ) E . GetType ( ) . GetField ( "Name" ) . GetValue ( E );
				}
				catch
				{
					return null;
				}
			}
		}

		#endregion Serializers
		#region Deserializers
		static private int LoadEm_Counter;
		static private int LoadEm_FileCount;

		public static void LoadEm ( Window W )
		{
			MW=W;
			//String CWD=System.Environment.CurrentDirectory;
			//var ED=System.Environment.GetEnvironmentVariables();
			//int i=0;
			//foreach (String s in ED.Keys)
			//{
			//Debug.WriteLine(String.Format("{0}:{1}",i++,s));
			//}

			String [ ] Xmls = System . IO . Directory . GetFiles ( @".\" , "*Control.xml" , 
				System . IO . SearchOption . TopDirectoryOnly );
			LoadEm_FileCount = Xmls.Count();
			LoadEm_Counter = 0;

			Debug . WriteLine ( String . Format ( "{0} loaded {1} *Control.xml files" , 
				nameof ( LoadEm ) ,LoadEm_FileCount ) );
			Load_Each ( "DMT_Main_Window_Control.xml" );
			LoadEm ( Xmls );
		}

		static private void LoadEm ( String [ ] Xmls )
		{
			foreach ( String F in Xmls )
			{
				Load_Each ( F );
			}
		}

		static private void Load_Each ( string F )
		{
			if ( !File . Exists ( F ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} not found {1} " , F , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
				return;
			}
			Debug . WriteLine ( String . Format ( "{0} {1} of {2} Loading: {3}" , nameof ( Load_Each ) , LoadEm_Counter , LoadEm_FileCount , F ) );

			LoadEm_Counter++;

			XmlFileContents = System . IO . File . ReadAllText ( F );

			StringReader XmlStringReader = new StringReader ( XmlFileContents );

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );
			XmlFileName = F;
			WalkXmlReader ( xmlReader );
		}

		static private void WalkXmlReader ( XmlReader xmlReader )
		{
			if ( xmlReader == null )
			{
				return;
			}

			while ( !xmlReader . EOF )
			{
				switch ( xmlReader . NodeType )
				{
					case XmlNodeType . Attribute:
						break;

					case XmlNodeType . CDATA:
						break;

					case XmlNodeType . Comment:
						break;

					case XmlNodeType . Document:
						break;

					case XmlNodeType . DocumentFragment:
						break;

					case XmlNodeType . DocumentType:
						break;

					case XmlNodeType . Element:
						//BlufbelsePrettyPrintElements ( xmlReader );

						//Debug . WriteLine ( String . Format ( @"{0} at [{1},{2}] NodeType:{3} {4} {5} Attributes?? {6}" ,
						//      XmlFileName ,
						//      ( ( IXmlLineInfo ) xmlReader ). LineNumber ,
						//      ( ( IXmlLineInfo ) xmlReader )  . LinePosition ,
						//      xmlReader . NodeType . ToString(),
						//      xmlReader . Name ,
						//      xmlReader.Depth,
						//      xmlReader . HasAttributes
						//      ) );

						//if( false || HA)
						//    {
						//    int AC = xmlReader . AttributeCount;
						//    BlomblisieAttributes ( xmlReader , AC , lineNumber, linePosition);
						//    }

						//IXmlLineInfo Li = ( IXmlLineInfo ) xmlReader;
						//lineNumber = Li . LineNumber;
						//linePosition = Li . LinePosition;

						if ( xmlReader . Name . EndsWith ( "CommonFields" ) )
						{
							XmlReader pp = xmlReader . ReadSubtree ( );
							DumpSubtree ( pp );
						}

						break;

					case XmlNodeType . EndElement:
						//BlufbelsePrettyPrintElements ( xmlReader );

						//Debug . WriteLine ( String . Format ( @"{0} at [{3},{4}] NodeType:{1} {2} Attributes? {5} {6}" ,
						//       XmlFileName ,
						//        ( ( IXmlLineInfo ) xmlReader ) . LineNumber ,
						//        ( ( IXmlLineInfo ) xmlReader ) . LinePosition ,
						//       xmlReader . NodeType . ToString ( ) ,
						//       xmlReader . Name ,
						//       xmlReader . HasAttributes ,
						//       xmlReader.Depth
						//       ) );
						break;

					case XmlNodeType . EndEntity:
						break;

					case XmlNodeType . Entity:
						break;

					case XmlNodeType . EntityReference:
						break;

					case XmlNodeType . None:
						break;

					case XmlNodeType . Notation:
						break;

					case XmlNodeType . ProcessingInstruction:
						break;

					case XmlNodeType . SignificantWhitespace:
						break;

					case XmlNodeType . Text:
						//BlufbelsePrettyPrintElements ( xmlReader );

						//Debug . WriteLine ( String . Format ( @"{0} at [{1},{2}] NodeType:{3} {4} Attributes?? {5} {6} {7} {8}" ,
						//      XmlFileName ,
						//       ( ( IXmlLineInfo ) xmlReader ) . LineNumber ,
						//      ( ( IXmlLineInfo ) xmlReader ) . LinePosition ,
						//      xmlReader . NodeType . ToString ( ) ,
						//      xmlReader . Name ,
						//      xmlReader . HasAttributes ,
						//      xmlReader . ValueType ,
						//      xmlReader . Value,
						//      xmlReader.Depth ) );
						break;

					case XmlNodeType . Whitespace:
						break;

					case XmlNodeType . XmlDeclaration:
						break;

					default:
						break;
				}

				Boolean State = xmlReader . Read ( );
			}
		}

		static private void BlufbelsePrettyPrintElements ( XmlReader xmlReader )
		{
			Debug . Write ( String . Format ( "{0}" , XmlFileName ) );
			Debug . Write ( String . Format ( " at [{0},{1}]" ,
				( ( IXmlLineInfo ) xmlReader ) . LineNumber . ToString ( "00" ) ,
				( ( IXmlLineInfo ) xmlReader ) . LinePosition . ToString ( "00" ) ) );

			for ( int i = 0 ; i < xmlReader . Depth ; i++ )
			{
				Debug . Write ( " " );
			}

			switch ( xmlReader . NodeType )
			{
				case XmlNodeType . Element:
					Debug . Write ( String . Format ( "<" ) );
					break;

				case XmlNodeType . EndElement:
					Debug . Write ( String . Format ( @"<\" ) );
					break;

				default:
					break;
			}

			if ( xmlReader . HasValue )
			{
				Debug . Write ( String . Format ( "{0}" , xmlReader . Value ) );
			}
			else
			{
				Debug . Write ( String . Format ( "{0}" , xmlReader . Name ) );
			}

			switch ( xmlReader . NodeType )
			{
				case XmlNodeType . Element:
					if ( xmlReader . HasAttributes )
					{
						BlomblisieAttributes ( xmlReader );
					}
					Debug . Write ( String . Format ( ">" ) );
					break;

				case XmlNodeType . EndElement:
					Debug . Write ( String . Format ( ">" ) );
					break;

				default:
					break;
			}

			Debug . WriteLine ( "" );
		}

		static private void BlomblisieAttributes ( XmlReader xR )
		{
			BlomblisieAttributes ( xR , xR . AttributeCount , ( ( IXmlLineInfo ) xR ) . LineNumber , ( ( IXmlLineInfo ) xR ) . LinePosition );
		}

		static private void BlomblisieAttributes ( XmlReader xR , int AttributeCount , int LineNumber , int LinePosition )
		{
			String NT = xR . NodeType . ToString ( );
			String NN = xR . Name;
			//String CT = xmlReader . ReadContentAsString ( );

			for ( int i = 0 ; i < AttributeCount ; i++ )
			{
				xR . MoveToAttribute ( i );
				string n = xR . Name;
				string v = xR . Value;
				IXmlLineInfo xmlInfo = ( IXmlLineInfo ) xR;
				int lineNumber = xmlInfo . LineNumber;
				int linePosition = xmlInfo . LinePosition;

				Debug . Write ( String . Format ( @" {0}=""{1}""" , n , v ) );
			}
		}

		static private void DumpSubtree ( XmlReader xmlReader )
		{
			while ( !xmlReader . EOF )
			{
				switch ( xmlReader . NodeType )
				{
					case XmlNodeType . Attribute:
						break;

					case XmlNodeType . CDATA:
						break;

					case XmlNodeType . Comment:
						break;

					case XmlNodeType . Document:
						break;

					case XmlNodeType . DocumentFragment:
						break;

					case XmlNodeType . DocumentType:
						break;

					case XmlNodeType . Element:
						//Blufbelse ( xmlReader );
						SeralizeControlCommonFields p = new SeralizeControlCommonFields ( );
						XmlSerializer x = new XmlSerializer ( p . GetType ( ) );
						object o = x . Deserialize ( xmlReader );
						p = ( SeralizeControlCommonFields ) o;
						switch ( p . ControlClass )
						{
							case nameof ( Window ):
								WindowControlDeseralizer ( p );
								break;

							case nameof ( TabControl ):
								TabControlDeseralizer ( p );
								break;

							case nameof ( CheckBox ):
								CheckBox_ControlDeseralizer ( p );
								break;

							case nameof ( RadioButton ):
								RadioButtonControlDeseralizer ( p );
								break;

							case nameof ( H_Slider_UserControl1 ):
								H_SliderUserControlDeseralizer ( p );
								break;

							default:
								Debug . WriteLine ( String . Format ( "unknown{0}" , p . ControlClass ) );
								break;
						}
						xmlReader . Close ( );
						return;

					case XmlNodeType . EndEntity:
						break;

					case XmlNodeType . EndElement:
						BlufbelsePrettyPrintElements ( xmlReader );
						break;

					case XmlNodeType . Entity:
						break;

					case XmlNodeType . EntityReference:
						break;

					case XmlNodeType . None:
						break;

					case XmlNodeType . Notation:
						break;

					case XmlNodeType . ProcessingInstruction:
						break;

					case XmlNodeType . SignificantWhitespace:
						break;

					case XmlNodeType . Text:
						BlufbelsePrettyPrintElements ( xmlReader );
						break;

					case XmlNodeType . Whitespace:
						break;

					case XmlNodeType . XmlDeclaration:
						break;

					default:
						break;
				}
				xmlReader . Read ( );
			}
		}

		static private void TabControlDeseralizer ( SeralizeControlCommonFields p )
		{
			Object O = LogicalTreeHelper . FindLogicalNode ( MW , p . ControlName );
			if ( !( O is TabControl TC ) )
			{
				return;
			}

			TabControlSaveState pp = new TabControlSaveState ( );

			XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );

			StringReader xmlStringReader = new StringReader ( XmlFileContents );
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( xmlStringReader , xmlReaderSettings );

			object o = x . Deserialize ( xmlReader );

			pp = ( TabControlSaveState ) o;
			TC . SelectedIndex = pp . SelectedIndex;
		}

		static private void H_SliderUserControlDeseralizer ( SeralizeControlCommonFields p )
		{
			if ( p == null )
			{
				throw new ArgumentNullException ( nameof ( p ) );
			}

			Object O = LogicalTreeHelper . FindLogicalNode ( MW , p . ControlName );
			if ( !( O is H_Slider_UserControl1 HC ) )
			{
				return;
			}

			H_Slider_UserControl1_SaveState_Class pp = new H_Slider_UserControl1_SaveState_Class ( );
			XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );
			StringReader XmlStringReader = new StringReader ( XmlFileContents );

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );

			object o = x . Deserialize ( xmlReader );

			pp = ( H_Slider_UserControl1_SaveState_Class ) o;
			HC . SliderValue = pp . ResetValue;
			HC . SliderMaxValue = pp . MaxValue;
			HC . SliderMinValue = pp . MinValue;
		}

		static private void RadioButtonControlDeseralizer ( SeralizeControlCommonFields p )
		{
			Object O = LogicalTreeHelper . FindLogicalNode ( MW , p . ControlName );
			if ( !( O is RadioButton RB ) )
			{
				return;
			}

			RadioCheckBoxSaveState pp = new RadioCheckBoxSaveState ( );

			XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );

			StringReader XmlStringReader = new StringReader ( XmlFileContents );

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );

			object o = x . Deserialize ( xmlReader );

			pp = ( RadioCheckBoxSaveState ) o;
			RB . IsChecked = pp . RadioCheckBoxState;
		}

		static private void CheckBox_ControlDeseralizer ( SeralizeControlCommonFields p )
		{
			Object O = LogicalTreeHelper . FindLogicalNode ( MW , p . ControlName );
			if ( !( O is CheckBox CB ) )
			{
				return;
			}

			CheckBoxControlSaveState pp = new CheckBoxControlSaveState ( );

			XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );

			StringReader XmlStringReader = new StringReader ( XmlFileContents );

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );

			Object o = x . Deserialize ( xmlReader );

			pp = ( CheckBoxControlSaveState ) o;
			CB . IsChecked = pp . CheckBoxState;
		}

		static private void WindowControlDeseralizer ( SeralizeControlCommonFields p )
		{
				WindowsControlDeseralizer ( MW );
		}

		static private void WindowsControlDeseralizer ( Window C )
		{
			if ( C == null )
			{
				return;
			}

			DMT_Main_Window_Control_SaveState pp = new DMT_Main_Window_Control_SaveState ( );

			XmlSerializer x = new XmlSerializer ( pp . GetType ( ) );

			StringReader XmlStringReader = new StringReader ( XmlFileContents );

			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
			{
				IgnoreComments = true ,
				IgnoreProcessingInstructions = true ,
				IgnoreWhitespace = true
			};

			XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );

			Object o = x . Deserialize ( xmlReader );

			pp = ( DMT_Main_Window_Control_SaveState ) o;
			C . Left = pp . Left;
			C . Top = pp . Top;
		}

		#endregion Deseralizers

	}
}