using SharpGL;
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
using System.Windows.Resources;
using System.Windows.Shapes;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;
using WpfScreenHelper;
using GlmSharp;
using System . Xml;
using System . IO;
using System . Xml . Serialization;

namespace DMT01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Persistance_classes2

        static long Draws = 0;
        static long Resizes = 0;

 
        [Serializable()]
        public class SeralizeControlCommonFields
        {
            public String ControlClass;
            public String ControlName;
            public String SaveStateFileName;
            public SeralizeControlCommonFields ( )
            {
                ControlClass = string . Empty; ;
                ControlName = string . Empty;
                SaveStateFileName = string . Empty;
            }

        }
        [Serializable()]
        public class DMT_Main_Window_Control_SaveState
        {
            public DMT_Main_Window_Control_SaveState ( )
            {
                CommonFields = new SeralizeControlCommonFields ( ); ;
            }
            [XmlElement( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public double Left;
            public double Top;
        }
        [Serializable]
        public class CheckBoxTempSaveState
        {
            public CheckBoxTempSaveState ( ) {
                CommonFields = new SeralizeControlCommonFields ( );
            }
            [XmlElement ( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public Boolean CheckBoxState;
            public String CheckBoxName;
        }

        [Serializable]
        public class RadioCheckBoxTempSaveState
        {
            public RadioCheckBoxTempSaveState ( )
                {
                CommonFields = new SeralizeControlCommonFields ( );
                }
            [XmlElement ( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public Boolean RadioCheckBoxState;
            public String RadioCheckBoxName;
            public String RadioGroupName;
        }

        public static SharpGL.SceneGraph.Matrix ProjectionMatrix=new SharpGL.SceneGraph.Matrix(4,4);
        public static SharpGL.SceneGraph.Matrix  ModelingMatrix=new SharpGL.SceneGraph.Matrix(4,4);
        public static float aspect = -1.1f;

        private struct SavedControl
            {
            public int MainWindows;
            public int H_Sliders;
            public int CheckButtons;
            public int RadioButtons;
            public int OtherControls;
            public int Calls;
            public int MaxDepth;
            };

        private static SavedControl SC;
        #endregion Persistance_classes

        public MainWindow ( )
        {
            InitializeComponent ( );
        }

        private String Stringify ( Rect bounds )
        {

            String s1 = String.Format("{0}", nameof(bounds.Bottom), bounds.Bottom);
            String s2 = String.Format("{0}", nameof(bounds.Top), bounds.Top);
            String s3 = String.Format("{0}", nameof(bounds.Left), bounds.Left);
            String s4 = String.Format("{0}", nameof(bounds.Right), bounds.Right);
            String s5 = String.Format("{0}", nameof(bounds.TopRight), bounds.TopRight);
            return s1 + s2 + s3 + s4 + s5;
        }

        private void myReoGridControl_Loaded ( object sender , RoutedEventArgs e )
        {

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}" , nameof ( myReoGridControl_Loaded ) ) );
            TextRange tr1 = new TextRange(SpreadsheetDirPath_RichTextBox.Document.ContentStart, SpreadsheetDirPath_RichTextBox.Document.ContentEnd);
            TextRange tr2 = new TextRange(SpreadsheetFileName_RichTextBox.Document.ContentStart, SpreadsheetFileName_RichTextBox.Document.ContentEnd);
            String Path = String.Format(@"{0}\{1}", tr1.Text.Trim(), tr2.Text.Trim());
            if ( System . IO . File . Exists ( Path ) )
            {
                myReoGridControl . Load ( Path , unvell . ReoGrid . IO . FileFormat . Excel2007 );
            }
        }

        private void myOpenGLControl_OpenGLDraw ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
        {

            //Debug.WriteLine(String.Format("{0}", nameof(myOpenGLControl_OpenGLDraw)));

            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl.OpenGL;

            //  Clear the color and depth buffer.
 
            gl . Clear ( OpenGL . GL_COLOR_BUFFER_BIT | OpenGL . GL_DEPTH_BUFFER_BIT );
            gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Projection );

            gl . LoadIdentity ( );

            GlmSharp.mat4 M=GlmSharp.mat4.Identity;



            if ( UsePerspetiveViewingTransform_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
            {
                M = M * Perspective ( gl );
            }


            if ( UseLookAtViewingTransform_RadioButton . IsChecked . GetValueOrDefault ( ) )
            {
                M = M * LookAt ( gl );
            }

            LoadMatrix ( gl , M );

            
            gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Modelview );
            gl . LoadIdentity ( );

            gl . Translate ( Eye_X_H_Sslider_UserControl . SliderValue , Eye_Y_H_Sslider_UserControl . SliderValue , Eye_Z_H_Sslider_UserControl . SliderValue );


            if ( AxisDrawMe_CheckBox . IsChecked . GetValueOrDefault ( ) )
            {
                Axis_Arrow_Grid . Axis_Class . MyGlobalAxis ( gl );
            }

            if ( DrawTeaPot_CheckBox . IsChecked . GetValueOrDefault ( ) )
            {
                Teapot tp = new Teapot();
                tp . Draw ( gl , 14 , 1 , OpenGL . GL_FILL );
            }



            if( Do_Orbit_CheckBox .IsChecked.GetValueOrDefault(true))
                {
                gl . Rotate ( Orbit_Rotation_H_Slider_UserControl1.SliderValue , 0.0f , 1.0f , 0.0f );
                Orbit_Rotation_H_Slider_UserControl1 . SliderValue += Orbit_Delta_H_Slider_UserControl1 . SliderValue;
                }

            gl . Flush ( );
            Draws_Label . Content = String . Format ( "Draw Count: {0}" , Draws );
            Draws++;
        }

        private void DebugGl ( SharpGL . OpenGL gl , string v )
        {
            ProjectionMatrix = gl . GetProjectionMatrix ( );
            ModelingMatrix = gl . GetProjectionMatrix ( );
        }

        private void LoadMatrix ( SharpGL . OpenGL gl , mat4 m4 )
        {
            vec4 c0=m4.Column0;
            vec4 c1=m4.Column1;
            vec4 c2=m4.Column2;
            vec4 c3=m4.Column3;

            double[] m=new double[16]{ c0 [ 0 ] , c0 [ 1 ] , c0 [ 2 ] , c0 [ 3 ] ,
                                       c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] , c1 [ 3 ] ,
                                       c2 [ 0 ] , c2 [ 1 ] , c2 [ 2 ] , c2 [ 3 ] ,
                                       c3 [ 0 ] , c3 [ 1 ] , c3 [ 2 ] , c3 [ 3 ] };

            gl . LoadMatrix ( m );


            //LoadMatrix ( c0 [ 0 ] , c0 [ 1 ] , c0 [ 2 ] , c0 [ 3 ] ,
            //                           c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] , c1 [ 3 ] ,
            //                           c2 [ 0 ] , c2 [ 1 ] , c2 [ 2 ] , c2 [ 3 ] ,
            //                           c3 [ 0 ] , c3 [ 1 ] , c3 [ 2 ] , c3 [ 3 ]
            //                         );
        }

        private GlmSharp . mat4 LookAt ( OpenGL gl )
        {
            float x_up = 0.0f;
            float y_up = 1.0f;
            float z_up = 0.0f;

            if ( LookAt_X_Up_RadioButton . IsChecked . GetValueOrDefault ( ) ) { x_up = 1.0f; } else { x_up = 0.0f; }
            if ( LookAt_Y_Up_RadioButton . IsChecked . GetValueOrDefault ( ) ) { y_up = 1.0f; } else { y_up = 0.0f; }
            if ( LookAt_Z_Up_RadioButton . IsChecked . GetValueOrDefault ( ) ) { z_up = 1.0f; } else { z_up = 0.0f; }

            GlmSharp . vec3 eye = new GlmSharp . vec3 (
                LookAt_Eye_X_H_Slider_UserControl . SliderValue ,
                LookAt_Eye_Y_H_Slider_UserControl . SliderValue ,
                LookAt_Eye_Z_H_Slider_UserControl . SliderValue  );

            GlmSharp . vec3 target = new GlmSharp . vec3 (
                LookAtTarget_X_H_Slider_UserControl.SliderValue,
                LookAtTarget_Y_H_Slider_UserControl.SliderValue,
                LookAtTarget_Z_H_Slider_UserControl.SliderValue  );

            GlmSharp . vec3 up = new GlmSharp . vec3 (
                x_up, y_up, z_up );

            GlmSharp . mat4   M =GlmSharp.mat4.LookAt ( eye, target,up);

            //gl.LookAt(
            //    LookAt_Eye_X_H_Slider_UserControl.SliderValue, 
            //    LookAt_Eye_Y_H_Slider_UserControl.SliderValue, 
            //    LookAt_Eye_Z_H_Slider_UserControl.SliderValue,
            //    LookAtTarget_X_H_Slider_UserControl.SliderValue, 
            //    LookAtTarget_Y_H_Slider_UserControl.SliderValue, 
            //    LookAtTarget_Z_H_Slider_UserControl.SliderValue,
            //    x_up, y_up, z_up);

            return M;
        }

        private mat4 Perspective ( OpenGL gl )
        {
            //(double fovy, double aspect, double zNear, double zFar)

            mat4 M=new mat4();

            M = GlmSharp . mat4 . Perspective ( Perspective_FOVY_H_Slider_UserControl . SliderValue ,
                Perspective_ASPECT_H_Slider_UserControl . SliderValue ,
                Perspective_Z_NEAR_H_Slider_UserControl . SliderValue ,
                Perspective_Z_FAR_H_Slider_UserControl . SliderValue );

            //Perspective ( Perspective_FOVY_H_Slider_UserControl . SliderValue ,
            //    Perspective_ASPECT_H_Slider_UserControl . SliderValue ,
            //    Perspective_Z_NEAR_H_Slider_UserControl . SliderValue ,
            //    Perspective_Z_FAR_H_Slider_UserControl . SliderValue );

            //gl . Perspective ( Perspective_FOVY_H_Slider_UserControl . SliderValue ,
            //     Perspective_ASPECT_H_Slider_UserControl . SliderValue ,
            //     Perspective_Z_NEAR_H_Slider_UserControl . SliderValue ,
            //     Perspective_Z_FAR_H_Slider_UserControl . SliderValue );

            return M;
        }

        private void DoAspect ( )
            {
            float width = ( float ) myOpenGLControl . Width;
            float height = ( float ) myOpenGLControl . Height;
            aspect = width / height;
            }
        private void myOpenGLControl_OpenGLInitialized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
        {
            Debug . WriteLine ( String . Format ( "{0}" , nameof ( myOpenGLControl_OpenGLInitialized ) ) );

            DoAspect ( );
            
            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl.OpenGL;

            //  Set the clear color.
            gl . ClearColor ( .1f , 0.1f , 0.1f , 0 );

            gl . Enable ( OpenGL . GL_DEPTH_TEST );

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl . LightModel ( OpenGL . GL_LIGHT_MODEL_AMBIENT , lmodel_ambient );

            gl . LightModel ( OpenGL . GL_LIGHT_MODEL_AMBIENT , global_ambient );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_POSITION , light0pos );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_AMBIENT , light0ambient );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_DIFFUSE , light0diffuse );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_SPECULAR , light0specular );
            gl . Enable ( OpenGL . GL_LIGHTING );
            gl . Enable ( OpenGL . GL_LIGHT0 );

            gl . ShadeModel ( OpenGL . GL_SMOOTH );

            gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );


        }

        private void spreadsheet_load_Button_Click ( object sender , RoutedEventArgs e )
        {
            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}" , nameof ( spreadsheet_load_Button_Click ) ) );

            TextRange tr1 = new TextRange(SpreadsheetDirPath_RichTextBox.Document.ContentStart, SpreadsheetDirPath_RichTextBox.Document.ContentEnd);
            TextRange tr2 = new TextRange(SpreadsheetFileName_RichTextBox.Document.ContentStart, SpreadsheetFileName_RichTextBox.Document.ContentEnd);
            String Path = String.Format(@"{0}\{1}", tr1.Text.Trim(), tr2.Text.Trim());

            const String scratchy = "Scratcheroo";

            var a = DMT01.Properties.Resources.UNEP_NATDIS_disasters_2002_2010;
            System . IO . File . WriteAllBytes ( scratchy , a );


            myReoGridControl . Load ( scratchy , unvell . ReoGrid . IO . FileFormat . Excel2007 );
        }

        private void DMTWindow_Initialized ( object sender , EventArgs e )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( DMTWindow_Initialized ) ) );

        }

        private void DMTWindow_Loaded ( object sender , RoutedEventArgs e )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( DMTWindow_Loaded ) ) );

        }

        private void myOpenGLControl_Resized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( myOpenGLControl_Resized ) ) );

            var x=sender.GetType();
            var Height=this.ActualHeight;
            var width=this.ActualWidth;
            var H=this.Height;
            var W=this.Width;

            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl.OpenGL;

            //  Set the projection matrix.
            //gl.MatrixMode(OpenGL.GL_PROJECTION);

            Resizes++;

        }

        private void Projection_DataGrid_Loaded ( object sender , RoutedEventArgs e )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof( Projection_DataGrid_Loaded ) ) );

            DataGrid DG=ProjectionMatrix_DataGrid;
            DG . Items . Add ( ProjectionMatrix [ 0 , 0 ] );
            DG . Items . Add ( ProjectionMatrix [ 0 , 1 ] );
            DG . Items . Add ( ProjectionMatrix [ 0 , 2 ] );

        }

        private void Projection_DataGrid_Initialized ( object sender , EventArgs e )
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( Projection_DataGrid_Initialized ) ) );

            DataGrid DG=ProjectionMatrix_DataGrid;

        }

        private void Save0_Button_Click ( object sender , RoutedEventArgs e )
        {
            //WalkVisualTree ( );
            WalkLogicalTree ( );
        }

        private void InitalizeSavedControl ( )
            {
            SC . MainWindows = 0;
            SC . H_Sliders = 0;
            SC . CheckButtons = 0;
            SC . RadioButtons = 0;
            SC . OtherControls = 0;
            SC . Calls = 0;
            SC . MaxDepth = -1;
            }

        public void WalkVisualTree ( )
        {
            InitalizeSavedControl ( );

            Debug . WriteLine ( String . Format ( "{0} Starting " , nameof ( WalkVisualTree ) ) );
            WalkVisualTree ( DMT_Main_Window_Control as FrameworkElement , 0 , 0 );


            Debug . WriteLine ( String . Format ( "{0}" , nameof( WalkVisualTree ) ) );

            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MainWindows ) , SC . MainWindows ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . H_Sliders ) , SC . H_Sliders ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . CheckButtons ) , SC . CheckButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . RadioButtons ) , SC . RadioButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . OtherControls ) , SC . OtherControls ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MaxDepth ) , SC . MaxDepth ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . Calls ) , SC . Calls ) );
            Debug . WriteLine ( String . Format ( "{0} completed " , nameof ( WalkVisualTree ) ) );

            }

        public void WalkLogicalTree ( )
            {

            InitalizeSavedControl ( );

            Debug . WriteLine ( String . Format ( "{0} Starting " , nameof ( WalkLogicalTree ) ) );
            WalkLogicalTree ( DMT_Main_Window_Control as FrameworkElement , 0);


            Debug . WriteLine ( String . Format ( "{0}" , nameof ( WalkLogicalTree ) ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MainWindows ) , SC . MainWindows ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . H_Sliders ) ,   SC . H_Sliders ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . CheckButtons ) , SC . CheckButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . RadioButtons ) , SC . RadioButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . OtherControls ) , SC . OtherControls ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MaxDepth ) ,    SC . MaxDepth ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . Calls ) ,       SC . Calls ) );
            Debug . WriteLine ( String . Format ( "{0} completed " , nameof ( WalkLogicalTree ) ) );

            }

        private void WalkVisualTree ( UIElement e , int Depth , int Peer )
        {
            SC . Calls++;
            if ( e == null )
            {
                return;
            }

            if ( true )
                {
                    String NameString = GetName ( e );
                    String TypeString = e . GetType ( ) . ToString ( );
                    Debug . WriteLine ( String . Format ( "{0} UIElement {1} {2} {3} {4}" ,nameof(WalkVisualTree) , SC.Calls, Depth, Peer, TypeString, NameString ) );
                }

            var n=VisualTreeHelper . GetChildrenCount ( e );
            for ( int i = 0 ; i < n ; i++ )
            {
                DependencyObject o = VisualTreeHelper . GetChild ( e , i );

                UIElement E=o as UIElement;
                FrameworkElement FE = o as FrameworkElement;

                WalkVisualTree ( E , Depth + 1 , i );

            }

            Boolean baah=IsControlStateSavable ( e , Depth, Peer);

            if ( Depth > SC . MaxDepth )
                SC . MaxDepth = Depth;
 
            }

        private void WalkLogicalTree ( FrameworkElement f , int Depth)
            {
            SC . Calls++;
            if ( Depth > SC . MaxDepth )
                SC . MaxDepth = Depth;

            if ( f == null )
                {
                return;
                }

            if ( false )
                {
                String NameString = GetName ( f );
                String TypeString = f . GetType ( ) . ToString ( );
                Debug . WriteLine ( String . Format ( "{0} Framework Element {1} {2} {3} {4}" , 
                    nameof ( WalkLogicalTree ) , SC . Calls , Depth , TypeString , NameString ) );
                }

            var children = LogicalTreeHelper . GetChildren (f );
            foreach (var child in children)
                {
 
                FrameworkElement FE = child as FrameworkElement;

                WalkLogicalTree ( FE , Depth + 1 );

                }

            Boolean baah = IsControlStateSavable ( f , Depth);
            
            }

        private Boolean IsControlStateSavable ( UIElement e , int Depth  )
            {
            String NameString = GetName ( e );
            String TypeString = e . GetType ( ) . ToString ( );

            if ( NameString . EndsWith ( nameof ( DMT_Main_Window_Control ) ) )
                {
                DMT_Main_Window_Control_SaveState_Deseralize ( NameString );
                SC . MainWindows++;
                return true;
                }

            if ( TypeString . EndsWith ( nameof ( H_Slider_UserControl1 ) ) )
                {
                H_Slider_UserControl1 . Seralize_H__Slider_UserControl1_SaveState ( e );
                SC . H_Sliders++;
                return true;
                }

            if ( TypeString . EndsWith ( "CheckBox" ) )
                {
                CheckBoxSerialize ( NameString , e );
                SC . CheckButtons++;
                return true;
                }

            if ( TypeString . EndsWith ( "RadioButton" ) )
                {
                RadioButtonSerialize ( NameString , e );
                SC . RadioButtons++;
                return true;
                }


            SC . OtherControls++;
            return false;
            }

        private void RadioButtonSerialize ( string nameString , UIElement e )
        {
            RadioButton RB = e as RadioButton;
            if ( RB == null )
                {
                return;
                }

            String StateFileName = String . Format ( "{0}.xml" , nameString );

            var p = new RadioCheckBoxTempSaveState ( );
            p . CommonFields . SaveStateFileName = StateFileName;
            p . CommonFields . ControlClass = nameof ( RadioButton );
            p . CommonFields . ControlName = RB . Name;
            p . RadioCheckBoxState = RB . IsChecked . GetValueOrDefault ( );
            p . RadioCheckBoxName = RB . Name;
            p . RadioGroupName = RB . GroupName;

            }

        private void CheckBoxSerialize ( string nameString , UIElement E )
        {
            CheckBox CB=E as CheckBox;
            if ( CB == null ) return;
            String StateFileName=String.Format("{0}.xml",nameString);

            var p=new CheckBoxTempSaveState ();
            p . CommonFields.SaveStateFileName = StateFileName;
            p . CommonFields . ControlClass = nameof ( CheckBox );
            p . CommonFields . ControlName = CB . Name;
            p . CheckBoxState = CB . IsChecked . GetValueOrDefault ( );
            p . CheckBoxName = CB . Name;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(p.GetType());

            XmlWriterSettings s = new XmlWriterSettings();
            s . Indent = true;
            s . NewLineOnAttributes = true;
            s . OmitXmlDeclaration = true;
            XmlWriter w=XmlWriter.Create(StateFileName,s);

            x . Serialize ( w , p );
            w . Close ( );

        }

        private void RadioCheckBoxSerialize ( string nameString , UIElement E )
        {
            RadioButton CB=E as RadioButton;
            if ( CB == null ) return;
            String StateFileName=String.Format("{0}.xml",nameString);

            var p=new RadioCheckBoxTempSaveState ();
            p . CommonFields.SaveStateFileName = StateFileName;
            p . CommonFields . ControlName = CB . Name;
            p . CommonFields . ControlClass = nameof ( RadioButton );
            p . RadioCheckBoxState = CB . IsChecked . GetValueOrDefault ( );
            p . RadioCheckBoxName = CB . Name;
            p . RadioCheckBoxName = CB . GroupName;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(p.GetType());

            XmlWriterSettings s = new XmlWriterSettings();
            s . Indent = true;
            s . NewLineOnAttributes = true;
            s . OmitXmlDeclaration = true;
            XmlWriter w=XmlWriter.Create(StateFileName,s);

            x . Serialize ( w , p );
            w . Close ( );
        }

        private void DMT_Main_Window_Control_SaveState_Deseralize ( String name )
        {
            String StateFileName=String.Format("{0}.xml",Name);

            var p=new DMT_Main_Window_Control_SaveState ();
            p . CommonFields . ControlClass = nameof ( Window );
            p . CommonFields . ControlName = DMT_Main_Window_Control . Name;
            p . CommonFields . SaveStateFileName = StateFileName;
            p . Left = DMT_Main_Window_Control . Left;
            p . Top = DMT_Main_Window_Control . Top; ;


            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(p.GetType());

            XmlWriterSettings s = new XmlWriterSettings();
            s . Indent = true;
            s . NewLineOnAttributes = true;
            s . OmitXmlDeclaration = true;
            XmlWriter w=XmlWriter.Create(StateFileName,s);

            x . Serialize ( w , p );
            w . Close ( );

        }

        private String GetName ( UIElement E )
        {
            var element = E as FrameworkElement;
            if ( element != null )
            {
                return element . Name;
            }

            try
            {
                String S=E . GetType ( ) . GetProperty ( "Name" ) . GetValue ( E , null ) as String;
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

        public IEnumerable<DependencyObject> FindInputElements ( DependencyObject parent )
        {
            if ( parent == null )
            {
                yield break;
            }

            for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount ( parent ) ; i++ )
            {
                DependencyObject o = VisualTreeHelper.GetChild(parent, i);

                foreach ( DependencyObject obj in FindInputElements ( o ) )
                {
                    yield return ( UIElement ) obj;
                }
            }

            yield return parent;
        }

        private void DMT_Main_Window_Control_LocationChanged ( object sender , EventArgs e )
        {
            Window W=sender as Window;
            if ( W == null )
            {
                return;
            }

            Debug . WriteLine ( String . Format ( "{0} {1} {2} {3} {4} {5}" ,
                nameof ( DMT_Main_Window_Control_LocationChanged ) ,
                W . WindowStartupLocation ,
                W . Left ,
                W . Top ,
                W . Width ,
                W . Height ) );

        }

        private void load_Button_Click ( object sender , RoutedEventArgs e )
            {
            LoadEm ( );
            }

        private void LoadEm ( )
            {

            String [ ] Xmls = System . IO . Directory . GetFiles ( @".\" , @"*.xml" , System . IO . SearchOption . TopDirectoryOnly );


            Debug . WriteLine ( String . Format ( "{0} loaded {1} .xml files" , nameof ( LoadEm ) , Xmls . Count ( ) ) );

            LoadEm ( Xmls );
            }

        private void LoadEm ( String [ ] Xmls )
            {
            foreach ( String F in Xmls )
                {
                Load_Each ( F );
                }
             }

        static String XmlFileContents;

        private void Load_Each ( string XmlFileName )
            {
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( Load_Each ) , XmlFileName ) );

            XmlFileContents = System . IO . File . ReadAllText ( XmlFileName );

            StringReader XmlStringReader = new StringReader ( XmlFileContents );

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings ( );
            xmlReaderSettings . IgnoreComments = true;
            xmlReaderSettings . IgnoreProcessingInstructions = true;
            xmlReaderSettings . IgnoreWhitespace = true;

            XmlReader xmlReader = XmlReader . Create ( XmlStringReader , xmlReaderSettings );

             WalkXmlReader ( xmlReader );

            }

        private void WalkXmlReader ( XmlReader xmlReader )
            {
            if ( xmlReader == null )
                {
                return;
                }

            //xmlReader . ReadStartElement ( );
            IXmlLineInfo xmlInfo = ( IXmlLineInfo ) xmlReader;
            int lineNumber = xmlInfo . LineNumber;
            int linePosition = xmlInfo . LinePosition;
            
            while ( !xmlReader . EOF )
                {

                var NT = xmlReader . NodeType;

                var HA = xmlReader . HasAttributes;
                Debug . WriteLine ( String . Format ( @"NodeType:Element {0} at [{1},{2}] Attributes?? {3}" ,
                    xmlReader . Name , lineNumber , linePosition, HA ) );

                switch ( NT )
                    {
                    case XmlNodeType . Attribute:
                        continue;
                        break;

                    case XmlNodeType . CDATA:
                        continue;
                        break;

                    case XmlNodeType . Comment:
                        continue;
                        break;

                    case XmlNodeType . Document:
                        break;

                    case XmlNodeType . DocumentFragment:
                        break;

                    case XmlNodeType . DocumentType:
                        break;

                    case XmlNodeType . Element:
                        Debug . WriteLine ( String . Format ( @"NodeType:Element {0} at [{1},{2}] Attributes? {3}" ,  
                             xmlReader . Name , lineNumber,linePosition, HA) );

                        if( HA)
                            {
                            int AC = xmlReader . AttributeCount;
                            BlomblisieAttributes ( xmlReader , AC , lineNumber, linePosition);
                            }

                        IXmlLineInfo Li = ( IXmlLineInfo ) xmlReader;
                        lineNumber = Li . LineNumber;
                        linePosition = Li . LinePosition;

                        if ( xmlReader . Name . EndsWith ( "CommonFields" ) )
                            {
                            ParseControlClass ( xmlReader );
                            } 
                            
                        break;
   
                    case XmlNodeType . EndElement:
                        Debug . WriteLine ( String . Format ( "NodeType:EndElement {0}" ,
                            xmlReader . Name ) );
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
                        continue;
                        break;

                    case XmlNodeType . ProcessingInstruction:
                        continue;
                        break;

                    case XmlNodeType . SignificantWhitespace:
                        continue;
                        break;

                    case XmlNodeType . Text:
                        break;
                    case XmlNodeType . Whitespace:
                        xmlReader . MoveToContent ( );
                        continue;
                        break;

                    case XmlNodeType . XmlDeclaration:
                        break;
                    default:
                        break;
                    }

                Boolean State = xmlReader . Read ( );
                }
            }

        private void BlomblisieAttributes ( XmlReader xR , int AC , int ln, int lp )
            {
            String NT = xR . NodeType . ToString ( );
            String NN = xR . Name;
            //String CT = xmlReader . ReadContentAsString ( );
           
            for ( int i = 0 ; i < AC ; i++ )
                {
                xR . MoveToAttribute ( i );
                var n = xR . Name;
                var v = xR . Value;
                IXmlLineInfo xmlInfo = ( IXmlLineInfo ) xR;
                int lineNumber = xmlInfo . LineNumber;
                int linePosition = xmlInfo . LinePosition;

                Debug . WriteLine ( String . Format ( "NodeType {0} {1} {3} Attribute of {2} at [{4},{5}] <<{6}>> = <<{7}>>" , 
                 NT ,  NN, AC, i , lineNumber , linePosition, n,v ) );

                }
            }

        private void ParseControlClass ( XmlReader xR )
            {
            
            do
                {
                var D = xR . Depth;
                switch ( xR . NodeType )
                    {
                    case XmlNodeType . Element:
                        var n= xR . Name;

                        XmlReader pp = xR . ReadSubtree ( );
                        DumpSubtree ( pp );
                        //string uuu = xR . ReadOuterXml ( );
                        //string ppp = xR . ReadInnerXml ( );
                        //var xxx = typeof ( SeralizeControlCommonFields );
                        //StringReader sr = new StringReader ( uuu );

                        //XmlReader zz = XmlReader.Create ( sr );
                        //XmlSerializer seralizer = new XmlSerializer ( xxx );
                        //Boolean can= seralizer . CanDeserialize ( zz );
                        break;

                    case XmlNodeType . Text:
                        var t = xR . Value;
                        break;

                    case XmlNodeType . EndElement:
                        var e = xR . Name;
                        break;

                    default:
                        break;
                    }
                } while ( xR . Read());



            //
            //StringReader xx = new StringReader ( XmlFileContents );
            //XmlReader x =  XmlReader.Create ( xx );
            //SeralizeControlCommonFields des = ( SeralizeControlCommonFields ) seralizer . Deserialize ( x );


            }

        private void DumpSubtree ( XmlReader xR )
            {
            xR . MoveToContent ( );
            do
                {

                IXmlLineInfo xmlInfo = ( IXmlLineInfo ) xR;
                int lineNumber = xmlInfo . LineNumber;
                int linePosition = xmlInfo . LinePosition;
                var D = xR . Depth;
                switch ( xR . NodeType )
                    {
                    case XmlNodeType . Element:
                        var n = xR . Name;
                        xR . Read ( );
                        var v = xR . Value;
                        break;

                    case XmlNodeType . Text:
                        xR . Read ( );
                        var t = xR . Value;
                        break;

                    case XmlNodeType . EndElement:
                        xR . Read ( );
                        var e = xR . Name;
                        break;

                    default:
                        xR . Read ( );
                        break;
                    }

                } while ( !xR . EOF );
            }

        void reshape (OpenGL gl, int width, int height)
        {

            Debug . WriteLine ( String . Format ( "{0}" , nameof(reshape) ));

            // GLsizei for non-negative integer
            DoAspect ( );

            // Set the viewport to cover the new window
            gl.Viewport(0, 0, width, height);

            // Set the aspect ratio of the clipping volume to match the viewport
            gl.MatrixMode(OpenGL.GL_PROJECTION);  // To operate on the Projection matrix
            gl.LoadIdentity();             // Reset
                                          // Enable perspective projection with fovy, aspect, zNear and zFar
            gl.Perspective(45.0f, aspect, 0.1f, 100.0f);
        }

        private void UseLookAtViewingTransform_RadioButton_Control_Click ( object sender , RoutedEventArgs e )
            {
            LookAt_tabItem . IsSelected = true;
            }

        private void UsePerspetiveViewingTransform_Click ( object sender , RoutedEventArgs e )
            {
             Perspective_TabItem . IsSelected = true;
            }

        private void UseOrthographic_Viewing_Transform_radioButton_Control_Click ( object sender , RoutedEventArgs e )
            {
            Orthographics_Viewing_TabItem . IsSelected = true;
            }

        private void Use_Viewing_Frustrum_RadioButton_Click ( object sender , RoutedEventArgs e )
            {
            Viewing_Frustrum_TabItem . IsSelected = true;
            }
        }
}
