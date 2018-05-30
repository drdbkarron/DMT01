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
using unvell . ReoGrid . DataFormat;
using System . Globalization;
using unvell . ReoGrid . CellTypes;
using unvell . ReoGrid;
using unvell . ReoGrid . Actions;
using unvell . ReoGrid . Rendering;
using unvell . ReoGrid . Graphics;
using System . Drawing;
using System . Resources;
using System . Reflection;
using LocalMaths;

namespace System . Windows . Controls
    {
    public static class MyExt
        {
        public static void PerformClick ( this Button btn )
            {
            btn . RaiseEvent ( new RoutedEventArgs ( Button . ClickEvent ) );
            }
        }
    }
class MyCheckBox : CheckBoxCell
    {
    System . Windows . Media . Imaging . BitmapSource UpArrow = null;
    System . Windows . Media . Imaging . BitmapSource DownArrow = null;

    public MyCheckBox ( )
        {
        //checkedImage = System . Drawing . Image . FromFile ( @"down-arrow.png" );
        //uncheckedImage = System . Drawing . Image . FromFile ( @"up-arrow.png" );
        //Stream imageStreamSource = new FileStream ( "up-arrow.png" , FileMode . Open , FileAccess . Read , FileShare . Read );
        //PngBitmapDecoder decoder = new PngBitmapDecoder ( imageStreamSource , BitmapCreateOptions . PreservePixelFormat , BitmapCacheOption . Default );
        //this.bitmapSource = decoder . Frames [ 0 ];

        //Assembly assembly = Assembly . GetExecutingAssembly ( );
        //ResourceManager resource_manager = new ResourceManager ( baseName: "AppResources" ,assembly: assembly );
        //UnmanagedMemoryStream UpArrowStream = resource_manager . GetStream ( "up_Arrow" );
        //BitmapImage UparrowBitmapjImage =new BitmapImage ( );
        //UparrowBitmapjImage . StreamSource = UpArrowStream;
        System . Drawing . Bitmap ua = DMT01 . Properties . Resources . UpArrowBMP;
        UpArrow = ConvertBitmap ( ua );
        System . Drawing . Bitmap da = DMT01 . Properties . Resources . down_arrowBMP;
        DownArrow = ConvertBitmap ( da );


        }

    protected override void OnContentPaint ( CellDrawingContext dc )
        {


        if ( this . IsChecked )
            {
            Debug . WriteLine ( String . Format ( "{0}" , nameof ( OnContentPaint ) ) );

            dc . Graphics . DrawImage ( image: UpArrow , rect: this . ContentBounds );
            }
        else
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( OnContentPaint ) ) );

            dc . Graphics . DrawImage ( image: DownArrow , rect: this . ContentBounds );
            }
        }


public static BitmapSource ConvertBitmap ( Bitmap source )
        {
        BitmapSource xx = System . Windows . Interop . Imaging . CreateBitmapSourceFromHBitmap (
                      source . GetHbitmap ( ) ,
                      IntPtr . Zero ,
                      Int32Rect . Empty ,
                      BitmapSizeOptions . FromEmptyOptions ( ) );
        return xx;
        }

    }

namespace DMT01
    {
    public partial class MainWindow : Window 
        {
        #region Persistance_classes2

        static long Draws = 0;
        static long Resizes = 0;
        static DateTime StartDateTime;
        static TimeSpan ElapsedDateTime;
        static int CurrentWorksheetIndex;

        [Serializable ( )]
        public class SeralizeControlCommonFields
            {
            public String ControlClass;
            public String ControlName;
            public String SaveStateFileName;
            public Boolean UpdatedFromXmlFiles;
            //public Object Parent;
            public SeralizeControlCommonFields ( )
                {
                ControlClass = string . Empty;
                ControlName = string . Empty;
                SaveStateFileName = string . Empty;
                UpdatedFromXmlFiles = false;
                UpdatedFromXmlFiles = false;
                }
            }

        [Serializable ( )]
        public class DMT_Main_Window_Control_SaveState
            {
            public DMT_Main_Window_Control_SaveState ( )
                {
                CommonFields = new SeralizeControlCommonFields ( );
                ;
                }
            [XmlElement ( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public double Left;
            public double Top;
            }

        [Serializable]
        public class CheckBoxControlSaveState
            {
            public CheckBoxControlSaveState ( )
                {
                CommonFields = new SeralizeControlCommonFields ( );
                }
            [XmlElement ( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public Boolean CheckBoxState;
            public String CheckBoxName;
            }

        [Serializable]
        public class RadioCheckBoxSaveState
            {
            public RadioCheckBoxSaveState ( )
                {
                CommonFields = new SeralizeControlCommonFields ( );
                }
            [XmlElement ( "SeralizeControlCommonFields" )]
            public SeralizeControlCommonFields CommonFields;
            public Boolean RadioCheckBoxState;
            public String RadioCheckBoxName;
            public String RadioGroupName;
            }

        public static SharpGL . SceneGraph . Matrix ProjectionMatrix = new SharpGL . SceneGraph . Matrix ( 4 , 4 );
        public static SharpGL . SceneGraph . Matrix ModelingMatrix = new SharpGL . SceneGraph . Matrix ( 4 , 4 );
        public static float myOpenGLControlViewportAspect = -1.1f;

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

        const String scratchy = "Scratcheroo";

        static public int mouse_x, mouse_y;
        static public int mouse_corrected_y;
        static public int viewport_cursor_x;
        static public int viewport_cursor_y;
        static public float viewport_width;
        static public float viewport_height;

        public class NWorksheety
            {
            public float [ , ] cells;
            public int r, c, r0,c0;
            }

        static NWorksheety Sheety;


        public class Vertex
            {
            public int I, J;
            public float x, y, z;
            public int[] c;
            public float V;
            public Vertex ( )
                {
                c = new int [ 3 ] { 0 , 0 , 0 };
                }
            public Vertex ( int i , int j , float v )
                {
                c = new int [ 3 ] {i , j , 0 };
                I = i;
                J = j;
                V = v;

                }
            public Vertex ( int i , int j )
                {
                c = new int [ 3 ] {i , j , 0 };
                I = i;
                J = j;


                }
            public Vertex ( float [,] v )
                {
                if ( c == null )
                    return;

                V = v [ I, J ];
                
                }
            public Vertex (float x , float y , float z , float v)
                {
                this . x = x;
                this . y = y;
                this . z = z;
                this . I = (int)x;
                this . J = (int)y;
                this . c =new int [ 3 ] { this . I , this . J , 0 };
                this . V = v;

                }
            };

        public class Edge
        {
            public short EdgeIndex = -1;
            public Vertex [ ] V =null;
            public Edge[] SubEdge = null;
            public short SubEdges = 0;
            public Vertex [ ] TweenVerte = null;
            public Edge()
            {
                this.V = new Vertex [ 2 ];
            }
            public Edge( short EI, Vertex V0 , Vertex V1 )
            {
                this . EdgeIndex = EI;
                this. V = new Vertex [ 2 ];

                V [ 0 ] = V0;
                V [ 1 ] = V1;
            }
        }

        public class Boxel
            {
            public Vertex [ ] V = new Vertex [ 4 ];
            public Vertex Centroid = new Vertex ( );
            public int Span=1;
            public int[]  SortedVertexIndicies=new int [ 4 ] { 0 , 1 , 2 , 3 };
            public int LargestVertex, SmallestVertex;
            public Edge [ ] E = new Edge [ 4 ];

            public Boxel ( )
                {
                V = new Vertex [ 4 ];
                }
 
            public Boxel ( int i , int j,float [ , ] c)
                {
                this . V [ 0 ] = new Vertex ( i         , j      );
                this . V [ 1 ] = new Vertex ( i         , j+Span );
                this . V [ 2 ] = new Vertex ( i +Span   , j+Span );
                this . V [ 3 ] = new Vertex ( i +Span   , j      );

                this.LoadValue ( 0 , c );
                LoadValue ( 1 , c );
                LoadValue ( 2 , c );
                LoadValue ( 3 , c );

                this . Centroid = LoadCentroid ( );

                SortVerticies ( );

                this . E [ 0 ] = new Edge ( 0 , this . V [ 0 ] , this . V [ 1 ] );
                this . E [ 1 ] = new Edge ( 1 , this . V [ 1 ] , this . V [ 2 ] );
                this . E [ 2 ] = new Edge ( 2 , this . V [ 2 ] , this . V [ 3 ] );
                this . E [ 3 ] = new Edge ( 3 , this . V [ 3 ] , this . V [ 0 ] );

                LoadTweenVerts ( );
                }

            private void LoadTweenVerts()
            {
                for ( int e = 0 ; e < 4 ; e++ )
                {
                    for ( int v = 1 ; v < 3 ; v++ )
                    {
                        if ( IsInBetween ( v , e ) )
                        {
                            float [ ] c = OnEdge ( v , e );
                        }
                    }
                }
            }

            private float [ ] OnEdge( int v , int e )
            {
                Vertex V = this . V [ SortedVertexIndicies [ v ] ];
                Edge E = this . E [ e ];
                return OnEdge ( V , E );

            }

            private float [ ] OnEdge( Vertex v , Edge e )
            {
                float ev0 = e . V [ 0 ] . V;
                float ev1 = e . V [ 1 ] . V;
                float deltaEdge = ( ev1 - ev0 );
                float vv  = v . V;
                float vx  = v . c [ 0 ];
                float vy  = v . c [ 1 ];
                float vz  = v . c [ 2 ];
                float deltaVertex=( vv - ev0 );
                float linageFraction = deltaVertex / deltaEdge;
                float x0 = e . V [ 0 ] . I;
                float y0 = e . V [ 0 ] . J;
                float z0 = e . V [ 0 ] . c [ 2 ];
                float x1 = e . V [ 1 ] . I;
                float y1 = e . V [ 1 ] . J;
                float z1 = e . V [ 1 ] . c [ 2 ];

                return
            }

            private bool IsInBetween( int v , int e )
            {
                Vertex V = this . V [ SortedVertexIndicies [ v ] ];
                Edge E = this . E [ e ];
                return IsInBetween ( V , E );
            }

            private bool IsInBetween( Vertex v , Edge e )
            {
                return IsInBetween ( e . V [ 0 ] . V , v . V , e . V [ 1 ] . V );
            }

            private bool IsInBetween( float v1 , float v2 , float v3 )
            {
                bool vee = ( v1 <= v2 );
                if (vee)
                {
                    bool bee = v2 <= v3;
                    if ( bee )
                    {
                        return true;
                    }
                }

                return false;
            }

            private void SortVerticies ()
                {
                Boolean IsSorted = false;
                int swaps = 0;
                int passes = 0;
                do
                    {
FreshReset:
                    IsSorted = false;
                    
                    for ( int i = 0 ; i < 3 ; i++ )
                        {
                        int ii = i % 4;
                        int iii = ( i + 1 ) % 4;
                        int j = SortedVertexIndicies [ii ];
                        int k = SortedVertexIndicies [iii];
                        float v0 = V [ j ] . V;
                        float v1 = V [ k ] . V;
                        if ( v0 > v1 )
                            {
                            //Burblegiggle ( swaps , "pre -Swap" );

                            int temp0 = SortedVertexIndicies[ii];
                            int temp1 = SortedVertexIndicies[iii];
                            SortedVertexIndicies [ii ] =   temp1;
                            SortedVertexIndicies [iii  ] =  temp0 ;
                            IsSorted = false;
                            swaps++;
                            //Burblegiggle ( swaps, "post-Swap" );
                            goto FreshReset;

                            }
                        else
                            {
                            IsSorted = true;
                            }

                         }
                    passes++;
                    } while ( IsSorted == false );

                //Burblegiggle ( swaps , "Completed" );

                }

            private void Burblegiggle (int swaps, String annotation)
                {
                string StringerDinger = "0.000";
                var TestResults = String . Format ( "{0}{1}{2}" ,
                      LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 0 ] ] . V , V [ SortedVertexIndicies [ 1 ] ] . V ) ,
                      LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 1 ] ] . V , V [ SortedVertexIndicies [ 2 ] ] . V ) ,
                      LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 2 ] ] . V , V [ SortedVertexIndicies [ 3 ] ] . V ) );

                if ( TestResults == "<<<" || TestResults == "==<<" || TestResults == "<==<" || TestResults == "======" )
                    {
                    Debug . WriteLine ( String . Format ( "Sorted in {0} swaps {1} {2} << {3}" ,
                        swaps , TestResults ,
                        V [ SortedVertexIndicies [ 0 ] ] . V . ToString ( StringerDinger ) , V [ SortedVertexIndicies [ 3 ] ] . V . ToString ( StringerDinger ) ) );
                    }
                else
                    {
                    Debug . WriteLine ( String . Format ( "{8} {0} swaps {1}{2}{3}{4}{5}{6}{7}" ,
                         swaps ,
                         V [ SortedVertexIndicies [ 0 ] ] . V . ToString ( StringerDinger ) , LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 0 ] ] . V , V [ SortedVertexIndicies [ 1 ] ] . V ) ,
                         V [ SortedVertexIndicies [ 1 ] ] . V . ToString ( StringerDinger ) , LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 1 ] ] . V , V [ SortedVertexIndicies [ 2 ] ] . V ) ,
                         V [ SortedVertexIndicies [ 2 ] ] . V . ToString ( StringerDinger ) , LocalMathsClass . ComparisonString ( V [ SortedVertexIndicies [ 2 ] ] . V , V [ SortedVertexIndicies [ 3 ] ] . V ) ,
                         V [ SortedVertexIndicies [ 3 ] ] . V . ToString ( StringerDinger ), annotation ) );
                    }
                }

            private Vertex LoadCentroid ()
                {
                float X = ( this . V [ 0 ] . I + this . V [ 1 ] . I + this . V [ 2 ] . I + this . V [ 3 ] . I ) / 4f;
                float Y = ( this . V [ 0 ] . J + this . V [ 1 ] . J + this . V [ 2 ] . J + this . V [ 3 ] . J ) / 4f;
                float V = ( this . V [ 0 ] . V + this . V [ 1 ] . V + this . V [ 2 ] . V + this . V [ 3 ] . V ) / 4f;

                Vertex C = new Vertex ( X , Y , 0.0f , V );

                return C;
                }

            public void LoadValue (int n , float [ , ] c)
                {
                Vertex VV = this . V [ n ];

                int i = VV . I;
                int j = VV . J;
                float vv = c [ i , j ];
                VV . V = vv;
                //float vvv = this . V [ n ] . V;

                }

            }

        #endregion Persistance_classes

        public MainWindow ( )
            {
            InitializeComponent ( );
            }

        private void DMTWindow_Initialized ( object sender , EventArgs e )
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( DMTWindow_Initialized ) ) );

            }

        private void DMTWindow_Loaded ( object sender , RoutedEventArgs e )
            {
            if ( LaunchSavedStateOnInitalization_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
                {
                Load_XML_Saved_Control_States_Button . PerformClick ( );
                }
            if ( LoadSpreadsheetAtInitalization_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                spreadsheet_load_Button . PerformClick ( );
                }
            StartDateTime = DateTime . Now;
            if ( Do_Execute_Select_Data_Button_on_Startup_CheckBox_Control .IsChecked.GetValueOrDefault())
                {
                DoSelectDataRangeForNormalization_Button . PerformClick ( );
                }

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
            }

        private void myReoGridControl_Loaded ( object sender , RoutedEventArgs e )
            {

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}" , nameof ( myReoGridControl_Loaded ) ) );
            TextRange tr1 = new TextRange ( SpreadsheetDirPath_RichTextBox . Document . ContentStart , SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
            TextRange tr2 = new TextRange ( SpreadsheetFileName_RichTextBox . Document . ContentStart , SpreadsheetFileName_RichTextBox . Document . ContentEnd );
            String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );
            if ( System . IO . File . Exists ( Path ) )
                {
                myReoGridControl . Load ( Path , unvell . ReoGrid . IO . FileFormat . Excel2007 );
                }
            }

        private void myOpenGLControl_OpenGLDraw ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
            {

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl . OpenGL;

            //  Clear the color and depth buffer.

            gl . Clear ( OpenGL . GL_COLOR_BUFFER_BIT | OpenGL . GL_DEPTH_BUFFER_BIT );
            //GlmSharp . mat4 M = GlmSharp . mat4 . Identity;

            gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Projection );
            gl . LoadIdentity ( );

  
            if ( DrawMouseScreenSpaceAnnotation_CheckBox_Control.IsChecked.GetValueOrDefault())
                {
                gl . PushMatrix ( );
                float fSize = MouseScreenAnnotationFontSize_H_Slider_UserControl . SliderValue + 0.5f;
                String text = String . Format ( @"{0},{1}" , mouse_x , mouse_y );
                gl . DrawText ( x: viewport_cursor_x , y: viewport_cursor_y , r: 1f , g: 1f , b: 1f ,
                    faceName: "Arial" ,
                    fontSize:fSize ,
                    text: text );
                gl . PopMatrix ( );
                }

            if ( DrawScreenSpaceAnnotationGrid_CheckBox_Control .IsChecked.GetValueOrDefault())
                {
                gl . PushMatrix ( );
                int bloppie = 40;
                float fSize = ScreenAnnotationFont_SizeH_Slider_UserControl . SliderValue;
                for ( int i = bloppie ; i < viewport_width - bloppie ; i += bloppie )
                    {
                    for ( int j = bloppie ; j < viewport_height - bloppie ; j += bloppie )
                        {
                        var t = String . Format ( @"{0},{1}" , i , j );
                        int jy = ( int ) viewport_height - j;
                        gl . DrawText ( i , jy , 1 , 1 , 1 , "Arial" ,fSize , t );
                        }
                    }
                gl . PopMatrix ( );
                }

            if ( UseOrthographic_Viewing_Transform_radioButton_Control . IsChecked . GetValueOrDefault ( ) )
            {
                gl . Ortho ( left: Orthographic_Left_H_Slider_UserControl . SliderValue ,
                            right: Orthographic_Right_H_Slider_UserControl . SliderValue ,
                            bottom: Orthographic_Bottom_H_Slider_UserControl . SliderValue ,
                            top: Orthographic_Top_H_Slider_UserControl . SliderValue ,
                            zNear: Orthographic_Near_H_Slider_UserControl . SliderValue ,
                            zFar: Orthographic_Far_H_Slider_UserControl . SliderValue
                            );
            }

            if ( Use_Viewing_Frustrum_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
            {
                gl . Frustum ( left: Frustum_Left_H_Slider_UserControl . SliderValue ,
                    right: Frustum_Right_H_Slider_UserControl . SliderValue ,
                    bottom: Frustum_Bottom_H_Slider_UserControl . SliderValue ,
                    top: Frustum_Top_H_Slider_UserControl . SliderValue ,
                    zNear: Frustum_zNear_H_Slider_UserControl . SliderValue ,
                    zFar: Frustum_zFar_H_Slider_UserControl . SliderValue );
            }

            if ( UsePerspetiveViewingTransform_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
            {
                gl . Perspective (
                    fovy: Perspective_FOVY_H_Slider_UserControl . SliderValue ,
                    aspect: Perspective_ASPECT_H_Slider_UserControl . SliderValue ,
                    zNear: Perspective_Z_NEAR_H_Slider_UserControl . SliderValue ,
                    zFar: Perspective_Z_FAR_H_Slider_UserControl . SliderValue );

                //M = M * Perspective ( gl );
            }


            //////LoadMatrix ( gl , M );

            //////var m=gl . GetProjectionMatrix ( );

            gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Modelview );
            gl . LoadIdentity ( );

            if ( UseLookAtViewingTransform_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
            {
                float x_up = 0.0f;
                float y_up = 1.0f;
                float z_up = 0.0f;

                if ( LookAt_X_Up_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
                {
                    x_up = 1.0f;
                }
                else
                {
                    x_up = 0.0f;
                }
                if ( LookAt_Y_Up_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
                {
                    y_up = 1.0f;
                }
                else
                {
                    y_up = 0.0f;
                }
                if ( LookAt_Z_Up_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
                {
                    z_up = 1.0f;
                }
                else
                {
                    z_up = 0.0f;
                }

                gl . LookAt (
                    eyex: LookAt_Eye_X_H_Slider_UserControl . SliderValue ,
                    eyey: LookAt_Eye_Y_H_Slider_UserControl . SliderValue ,
                    eyez: LookAt_Eye_Z_H_Slider_UserControl . SliderValue ,
                    centerx: LookAtTarget_X_H_Slider_UserControl . SliderValue ,
                    centery: LookAtTarget_Y_H_Slider_UserControl . SliderValue ,
                    centerz: LookAtTarget_Z_H_Slider_UserControl . SliderValue ,
                    upx: x_up , upy: y_up , upz: z_up );

                //M = M * LookAt ( gl );
            }

            //if ( Do_Orbit_Pull_Back_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
            //    {
            //    gl . Translate (
            //        x: Eye_X_H_Slider_UserControl . SliderValue ,
            //        y: Eye_Y_H_Slider_UserControl . SliderValue ,
            //        z: Eye_Z_H_Slider_UserControl . SliderValue );
            //    }

            if ( Do_Orbit_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
            {
                gl . Translate ( 0.0 , 0.0 , Orbit_Radius_H_Slider_UserControl . SliderValue );

                gl . Rotate ( angle: Orbit_Rotation_H_Slider_UserControl . SliderValue , x: 0.0f , y: 1.0f , z: 0.0f );

                Orbit_Rotation_H_Slider_UserControl . SliderValue += Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;

                if ( Orbit_Rotation_H_Slider_UserControl . SliderValue >= Orbit_Rotation_H_Slider_UserControl . SliderMaxValue )
                {
                    Orbit_Delta_Angle_H_Slider_UserControl . SliderValue = -Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;
                }
                else if ( Orbit_Rotation_H_Slider_UserControl . SliderValue <= Orbit_Rotation_H_Slider_UserControl . SliderMinValue )
                {
                    Orbit_Delta_Angle_H_Slider_UserControl . SliderValue = -Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;
                }
            }

            if ( AxisDrawMe_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                int Al = ( int ) ( ( Axis_Length_XYZ_H_Slider_UserControl . SliderValue ) + 0.5f );

                int lw = ( int ) ( ( Axis_Linewidth_H_Slider_UserControl . SliderValue ) + 0.5f );

                int ps = ( int ) ( ( Axis_TickSize_H_Slider_UserControl . SliderValue ) + 0.5f );

                Axis_Arrow_Grid . Axis_Class . MyGlobalAxis (
                    gl: gl ,
                    AxesLength: Al ,
                    Pointsize: ps ,
                    LineWidth: lw ,
                    DoMinusTicks: Axis_DrawNegativeCheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    TagOrigin: AnnotateOrigin_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    DoXYZAnnotation: AnnotateAxisXYZ_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    DoUnitTicks: DoDoTickMarks_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    DoAnnotateZTicks: AnnotateZTickMarks_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    DoAnnotateYTicks: AnnotateYTickMarks_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    DoAnnotateXTicks: AnnotateXTickMarks_CheckBoxControl . IsChecked . GetValueOrDefault ( ) ,
                    tick_annotation_scale: Axis_tick_annotation_scale_H_Slider_UserControl . SliderValue
                    );
                }

            if ( YourArmsTooShortToBoxWithHashem.IsChecked.GetValueOrDefault() )
                {
                if ( Sheety == null )
                    goto scramo;
                gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
                gl . PushMatrix ( );
                gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
                gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
                gl . Scale ( 1 , -1 , 1 );
                gl . PointSize (3 );

                for ( int j = Sheety . r0 ; j < Sheety . r ; j++ )
                    {
                    for ( int i = Sheety . c0 ; i < Sheety . c ; i++ )
                        {
                        Boxel B = new Boxel ( i , j , Sheety . cells );

                        if ( Sheety . cells [ i , j ]>1.0f )
                            {
                            float overflow = Sheety . cells [ i , j ];
                            Sheety . cells [ i , j ] = 1.0f;
                            }
                        int [ , ] c = new int [ 4,2 ] { 
                            { i     , j },
                            { i + 1 , j } ,
                            { i+1   , j + 1 } , 
                            { i     , j + 1 },
                                };
                        float [ ] s = new float [ 4 ] {
                            Sheety . cells [ c[0,0], c[0,1] ],
                            Sheety . cells [ c[1,0], c[1,1] ],
                            Sheety . cells [ c[2,0], c[2,1] ],
                            Sheety . cells [ c[3,0], c[3,1] ],
                                                 };
                        float [ ] C =  LocalMaths.LocalMathsClass.GetCentroid ( c );
                        float S = LocalMaths. LocalMathsClass . GetMean ( s );

                        for ( int k = 0 ; k <4 ; k++ )
                            {
                            int m = k % 4;
                            int n = ( k + 1 ) % 4;
                            int o = ( k + 2 ) % 4;
                            gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                            gl . Color ( s [ m ] , s [ m ] , s [ m ] );
                            gl . Vertex ( c [ m , 0 ] , c [ m , 1 ] , s[m]);

                            gl . Color (S,S,S );
                            gl . Vertex ( C[0],C[1 ] , S);

                            gl . Color ( s [ n ] , s [ n ] , s [ n ] , s [ n ] );
                            gl . Vertex ( c [ n , 0 ] , c [ n , 1 ] , s [ n ] );
                            gl . End ( );
                            }
                        }
                    
                    }
                gl . PopMatrix ( );
                gl . PopAttrib ( );
scramo:
                ;
                }

            //if ( DrawTeaPot_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
            //    {
            //    Teapot tp = new Teapot ( );
            //    tp . Draw ( gl , 14 , 1 , OpenGL . GL_FILL );
            //    }

            //if ( Spreadsheet_Grid_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
            //    {
            //    ReoGrid3DSpreadsheet ( gl: gl );
            //    }

            ////DoAspect ( );

            gl . Flush ( );

            Draws_Label . Content = String . Format ( "Draw Count: {0}" , Draws );
            Draws++;
            }

        private void ReoGrid3DSpreadsheet ( OpenGL gl )
            {
            float cell_height = 0.499f;
            float cell_width = 0.5f;
            int MaxDisplayCols = 20;
            int MaxDisplayRows = 30;
            int normal_col_width = 70;
            int normal_row_height = 20;
            var CW = myReoGridControl . CurrentWorksheet;

            if ( DoDrawSpreadsheetSideBorder_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                float x0 = 0f;
                float x1 = -1f;

                float y0 = 0.0f;
                float y1 = 0f;

                for ( int i = 0 ; i < MaxDisplayRows ; i++ )
                    {
                    float row_height = CW . GetRowHeight ( i );
                    float norm_units = cell_width * row_height / normal_row_height;

                    y0 = y1;
                    y1 = y1 - norm_units;

                    gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
                    gl . Color ( .4f , .6f , .4f );
                    gl . LineWidth ( 2 );
                    gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                    gl . Vertex ( x0 , y0 , 0 );
                    gl . Vertex ( x1 , y0 , 0 );
                    gl . Vertex ( x1 , y1 , 0 );
                    gl . Vertex ( x0 , y1 , 0 );
                    gl . End ( );
                    gl . PopAttrib ( );

                    gl . PushMatrix ( );

                    float [ ] c = LocalMaths.LocalMathsClass.GetCentroid ( x0 , y0 , x1 , y1 );

                    gl . Translate ( c [ 0 ] - .25 , c [ 1 ] - .15 , c [ 2 ] );
                    gl . Scale ( 0.6 , 0.6 , 1.0 );
                    String iter_string = ( i + 1 ) . ToString ( "00" );
                    gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: iter_string );
                    gl . PopMatrix ( );
                    }
                }

            if ( DoDrawSpreadsheetTopBorder_CheckBoxControl . IsChecked . GetValueOrDefault ( ) )
                {
                float x0 = 0f;
                float x1 = 0f;
                float y0 = 0f;
                float y1 = cell_height;
                for ( int i = 0 ; i < MaxDisplayCols ; i++ )
                    {
                    float col_with0 = CW . GetColumnWidth ( i );
                    float norm_units = col_with0 / normal_col_width;

                    x0 = x1;
                    x1 = x0 + norm_units;

                    gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
                    gl . Color ( .9f , .9f , .9f , 0.1f );
                    gl . LineWidth ( 2 );
                    gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                    gl . Vertex ( x0 , y0 , 0 );
                    gl . Vertex ( x1 , y0 , 0 );
                    gl . Vertex ( x1 , y1 , 0 );
                    gl . Vertex ( x0 , y1 , 0 );
                    gl . End ( );
                    gl . PopAttrib ( );
                    gl . PushMatrix ( );

                    float [ ] c = LocalMaths.LocalMathsClass.GetCentroid ( x0 , y0 , x1 , y1 );

                    gl . Translate ( c [ 0 ] - 0.25f , c [ 1 ] - .25 , c [ 2 ] );
                    gl . Scale ( 0.7 , 0.7 , 1.0 );
                    gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: LocalMaths . LocalMathsClass . IntToLetter ( i ) );
                    gl . PopMatrix ( );
                    }
                }

            if ( DoDrawSpreadsheetFocusCell_s_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                var FP = myReoGridControl . CurrentWorksheet . FocusPos;
                gl . PushMatrix ( );
                gl . LineWidth ( 3 );
                gl . Color ( .9 , .1 , .1 );
                gl . Scale ( x: 1.0 , y: -0.50 , z: 1.0 );
                gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                gl . Vertex ( FP . Col , FP . Row );
                gl . Vertex ( FP . Col , FP . Row + 1 );
                gl . Vertex ( FP . Col + 1 , FP . Row + 1 );
                gl . Vertex ( FP . Col + 1 , FP . Row );
                gl . End ( );
                gl . PopMatrix ( );
                }

            if ( DoDrawSpreadsheetSelectedCell_s_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                var SR = myReoGridControl . CurrentWorksheet . SelectionRange;
                for ( int i = SR . StartPos . Row ; i <= SR . EndPos . Row ; i++ )
                    {
                    for ( int j = SR . StartPos . Col ; j <= SR . EndPos . Col ; j++ )
                        {
                        gl . PushMatrix ( );
                        gl . LineWidth ( 2 );
                        gl . Color ( .9 , .1 , .1 );
                        gl . Scale ( x: 1.0 , y: -0.50 , z: 1.0 );
                        gl . Scale ( x: 0.99 , y: 0.99 , z: 0.99 );
                        gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                        gl . Vertex ( j , i );
                        gl . Vertex ( j , i + 1 );
                        gl . Vertex ( j + 1 , i + 1 );
                        gl . Vertex ( j + 1 , i );
                        gl . End ( );
                        gl . PopMatrix ( );
                        }
                    }
                }

            if ( DoDrawSpreadsheetGrid_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                gl . PushMatrix ( );
                gl . LineWidth ( 1 );
                gl . Scale ( x: 1.0 , y: -0.50 , z: 1.0 );

                float x1 = 0;
                float x0 = 0;
                for ( int i = 0 ; i < CW . ColumnCount ; i++ )
                    {
                    int w = CW . GetColumnWidth ( i );
                    float width_factor = ( float ) w / ( float ) normal_col_width;
                    x0 = x1;
                    x1 = x1 + width_factor;

                    //Debug . WriteLine ( String . Format ( " GetColumnWidth {0}" , w ) );

                    float y0 = 0;
                    float y1 = 0;

                    for ( int j = 0 ; j < CW . RowCount ; j++ )
                        {
                        int r = CW . GetRowHeight ( j );
                        float height_factor = ( float ) r / ( float ) normal_row_height;

                        y0 = y1;
                        y1 = y1 + height_factor;

                        //Debug . WriteLine ( String . Format ( " GetRowHeight {0}" , r ) );

                        gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                        gl . Vertex ( x0 , y0 );
                        gl . Vertex ( x0 , y1 );
                        gl . Vertex ( x1 , y1 );
                        gl . Vertex ( x1 , y0 );
                        gl . End ( );
                        }
                    }
                gl . PopMatrix ( );
                }

            if ( DoDrawAllSpreadsheetData_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
                {
                gl . PushMatrix ( );
                gl . PushAttrib (   SharpGL . OpenGL . GL_CURRENT_BIT | 
                                    SharpGL . OpenGL . GL_ENABLE_BIT | 
                                    SharpGL . OpenGL . GL_LINE_BIT | 
                                    SharpGL . OpenGL . GL_DEPTH_BUFFER_BIT );

                //gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
                //gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );

                gl . LineWidth ( 1 );
                if( Spreadsheet_Aspect_Scale_Hack_checkBox_Control.IsChecked.GetValueOrDefault())
                    {
                    gl . Scale ( x: .25 , y: 1 , z: 1 );
                    }

                int maxRow;
                int maxCol;
                GetActualizedRange ( CW , out maxRow , out maxCol );

                float x1 = 0;
                float x0 = 0;
                for ( int i = 0 ; i < maxCol ; i++ )
                    {
                    int w = CW . GetColumnWidth ( i );
                    float width_factor = ( float ) w / ( float ) normal_col_width;
                    x0 = x1;
                    x1 = x1 + width_factor;

                    float y0 = 0;
                    float y1 = 0;

                    for ( int j = 0 ; j < maxRow ; j++ )
                        {
                        int r = CW . GetRowHeight ( j );
                        float height_factor = ( float ) r / ( float ) normal_row_height;

                        y0 = y1;
                        y1 = y1 - height_factor;
 
                        var D = CW . Cells [ j , i ];
                        if ( D . DataFormat == unvell . ReoGrid . DataFormat . CellDataFormatFlag . Text )
                            {
                            String Text = D . DisplayText;

                            gl . PushMatrix ( );

                            float [ ] c = LocalMaths.LocalMathsClass.GetCentroid ( x0 , y0 , x1 , y1 );
                            int len = Text . Length;
                            if ( len < 10 )
                                {
                                gl . Translate ( x: x0 , y: c [ 1 ] - .25 , z: c [ 2 ] );
                                gl . Scale ( 0.5 , 0.7 , 1.0 );
                                }
                            else
                                {
                                gl . Translate ( x: x0 , y: c [ 1 ] - .25 , z: c [ 2 ] );
                                var big_x_scaling = 4.0f / ( float ) len;
                                gl . Scale ( big_x_scaling , 0.7 , 1.0 );
                                }
                            gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: Text );
                            gl . PopMatrix ( );

                            }
                        else
                        if ( D . DataFormat == unvell . ReoGrid . DataFormat . CellDataFormatFlag . Number )
                            {
                            object CellData = D . Data;
                            float number;
                            if ( float . TryParse ( s: D . DisplayText , result: out number ) )
                                {
                                }
                            else
                                {
                                Debug . WriteLine ( String . Format ( "TryParse failed on {0}" , D . DisplayText ) );
                                return;
                                }
                            gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
                            gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
                            gl . Enable ( SharpGL . OpenGL . GL_ALPHA );
                            gl . Enable ( SharpGL . OpenGL . GL_DEPTH_BUFFER_BIT );
                            gl . PushMatrix ( );
                            gl . Color ( red: number , green: number , blue: number , alpha: 0 );

                            gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );

                            gl . Vertex ( x0 , y0 );
                            gl . Vertex ( x0 , y1 );
                            gl . Vertex ( x1 , y1 );
                            gl . Vertex ( x1 , y0 );
                            gl . End ( );

                            float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

                            gl . Color ( red: number , green: number , blue: number , alpha: 1 );
                            gl . Translate ( x: x0 + Spreadsheet3DNumericLeftMargin_H_Slider_User_Control.SliderValue , y: c [ 1 ] - .25 , z: c [ 2 ] -1.0 );
                            String Text = number . ToString ( "#,###,##0.##" );
                            float len = Text . Length;
                            var big_x_scaling = Spreadsheet3DNumericWidthFittingFactor_H_Slider_User_Control.SliderValue / len;
                            gl . Scale ( x: big_x_scaling , y: 0.7 , z:1.0 );

                            gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 1.0f , extrusion: 0.9f , text: Text );

                            gl . PopMatrix ( );
                            gl . Enable ( SharpGL . OpenGL . GL_LIGHTING );
                            gl . Enable ( SharpGL . OpenGL . GL_TEXTURE_2D );

                            }
                        else
                        if ( D . DataFormat == CellDataFormatFlag . Percent )
                            {
                            object Nuber = D . Data;
                            float number;
                            if ( float . TryParse ( s: D . DisplayText , result: out number ) )
                                {
                                }
                            else
                                {
                                }
                            gl . PushMatrix ( );

                            float [ ] c = LocalMaths.LocalMathsClass.GetCentroid ( x0 , y0 , x1 , y1 );

                            gl . Translate ( x: x0 + 0.01f , y: c [ 1 ] - .25 , z: c [ 2 ] );
                            var big_x_scaling = 2.0f / ( float ) 10;
                            gl . Scale ( big_x_scaling , 0.7 , 1.0 );

                            gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: number . ToString ( "00.0" ) );
                            gl . PopMatrix ( );
                            }
                        else
                        if ( D . DataFormat == CellDataFormatFlag . Currency )
                            {
                            object Nuber = D . Data;
                            float number;
                            if ( float . TryParse ( s: D . DisplayText , result: out number , style: System . Globalization . NumberStyles . Currency , provider: new CultureInfo ( "en-US" ) ) )
                                {
                                }
                            else
                                {
                                }
                            gl . PushMatrix ( );

                            float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

                            gl . Translate ( x: x0 + 0.01f , y: c [ 1 ] - .25 , z: c [ 2 ] );
                            var big_x_scaling = 2.0f / ( float ) 10;
                            gl . Scale ( big_x_scaling , 0.7 , 1.0 );

                            gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: number . ToString ( "00.0" ) );
                            gl . PopMatrix ( );
                            }
                        else
                            {
                            float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );
                            gl . PointSize ( 5 );
                            gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
                            gl . Vertex ( c );
                            gl . End ( );


                            }
                        }
                    }
                gl . PopAttrib ( );
                gl . PopMatrix ( );
                }
            }

        private void GetActualizedRange ( unvell . ReoGrid . Worksheet CW , out int maxRow , out int maxCol )
            {
            unvell . ReoGrid . RangePosition ColumnCaptionRange = new unvell . ReoGrid . RangePosition ( row: 0 , col: 0 , rows: 1 , cols: CW . ColumnCount );
            int maxCaptionCol = -1;
            CW . IterateCells ( range: ColumnCaptionRange , iterator: ( row , col , cell ) =>
                 {
                     if ( cell . DataFormat == CellDataFormatFlag . Text )
                         {
                         string text = cell . GetData<String> ( );
                         if ( text == null )
                             {
                             return false;
                             }

                         maxCaptionCol = col;
                         return true;
                         }
                     if ( cell . DataFormat == CellDataFormatFlag . General )
                         {
                         string text = cell . GetData<String> ( );
                         if ( text == null )
                             {
                             return false;
                             }

                         maxCaptionCol = col;
                         return true;
                         }
                     return false;
                 }
             );

            maxCol = maxCaptionCol;

            unvell . ReoGrid . RangePosition RowCaptionRange = new unvell . ReoGrid . RangePosition ( row: 0 , col: 0 , rows: CW . RowCount , cols: 1 );
            int maxCaptionRow = -1;
            CW . IterateCells ( range: RowCaptionRange , iterator: ( row , col , cell ) =>
                {
                    if ( cell . DataFormat == CellDataFormatFlag . Text )
                        {
                        string text = cell . GetData<String> ( );
                        if ( text == null )
                            {
                            return false;
                            }

                        maxCaptionRow = row;
                        return true;
                        }
                    if ( cell . DataFormat == CellDataFormatFlag . General )
                        {
                        string text = cell . GetData<String> ( );
                        if ( text == null )
                            {
                            return false;
                            }

                        maxCaptionRow = row;
                        return true;
                        }
                    return false;
                }
            );

            maxRow = maxCaptionRow;

            return;

#pragma warning disable CS0162 // Unreachable code detected
            unvell . ReoGrid . RangePosition range = new unvell . ReoGrid . RangePosition ( row: 0 , col: 0 , rows: CW . RowCount , cols: CW . ColumnCount );

            maxRow = -1;
            maxCol = -1;
            int mRow = maxRow;
            int mCol = maxCol;

            CW . IterateCells (
                range: range , iterator: ( row , col , cell ) =>
                    {
                        if ( cell . Row > mRow )
                            {
                            mRow = cell . Row;
                            }

                        if ( cell . Column > mCol )
                            {
                            mCol = cell . Column;
                            }
                        // return true to continue iterate, return false to abort
                        return true;
                    }
                );
            maxRow = mRow;
            maxCol = mCol;
#pragma warning restore CS0162 // Unreachable code detected
            }

        private void DoAspect ( )
            {

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            int GL_Width1 = 605;
            int GL_Width0 = 1060;

            Boolean IsReoGridControlVisible = myReoGridControl . IsVisible;

            if ( IsReoGridControlVisible )
                {
                myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnSpanProperty , 1 );
                myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnProperty , 1 );
                myOpenGLControl . Width = GL_Width1;
                }
            else
                {
                myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnSpanProperty , 2 );
                myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnProperty , 0 );
                myOpenGLControl . Width = GL_Width0;
                }

            float width = ( float ) myOpenGLControl . Width;
            float height = ( float ) myOpenGLControl . Height;

            myOpenGLControlViewportAspect = width / height;

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
            }

        private void myOpenGLControl_OpenGLInitialized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
            {
            Debug . WriteLine ( String . Format ( "{0}" , nameof ( myOpenGLControl_OpenGLInitialized ) ) );

            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl . OpenGL;

            //  Set the clear color.
            gl . ClearColor ( .01f , 0.01f , 0.01f , 0 );

            gl . Enable ( OpenGL . GL_DEPTH_TEST );

            float [ ] global_ambient = new float [ ] { 0.5f , 0.5f , 0.5f , 1.0f };
            float [ ] light0pos = new float [ ] { 0.0f , 5.0f , 10.0f , 1.0f };
            float [ ] light0ambient = new float [ ] { 0.2f , 0.2f , 0.2f , 1.0f };
            float [ ] light0diffuse = new float [ ] { 0.3f , 0.3f , 0.3f , 1.0f };
            float [ ] light0specular = new float [ ] { 0.8f , 0.8f , 0.8f , 1.0f };

            float [ ] lmodel_ambient = new float [ ] { 0.2f , 0.2f , 0.2f , 1.0f };

            gl . LightModel ( OpenGL . GL_LIGHT_MODEL_AMBIENT , lmodel_ambient );

            gl . LightModel ( OpenGL . GL_LIGHT_MODEL_AMBIENT , global_ambient );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_POSITION , light0pos );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_AMBIENT , light0ambient );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_DIFFUSE , light0diffuse );
            gl . Light ( OpenGL . GL_LIGHT0 , OpenGL . GL_SPECULAR , light0specular );
            gl . Enable ( OpenGL . GL_LIGHTING );
            gl . Enable ( OpenGL . GL_LIGHT0 );

            gl . ShadeModel ( OpenGL . GL_SMOOTH );

            gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Back );

            DoAspect ( );

            }

        private void spreadsheet_load_Button_Click ( object sender , RoutedEventArgs e )
            {
            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}" , nameof ( spreadsheet_load_Button_Click ) ) );

            TextRange tr1 = new TextRange ( SpreadsheetDirPath_RichTextBox . Document . ContentStart , SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
            TextRange tr2 = new TextRange ( SpreadsheetFileName_RichTextBox . Document . ContentStart , SpreadsheetFileName_RichTextBox . Document . ContentEnd );
            String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );

            String FileName = String . Format ( @"{0}{1}" , scratchy , ".xll" );
            var a = DMT01 . Properties . Resources . UNEP_NATDIS_disasters_2002_2010;
            System . IO . File . WriteAllBytes ( FileName , a );


            myReoGridControl . Load ( FileName , unvell . ReoGrid . IO . FileFormat . Excel2007 );
            }

        private void myOpenGLControl_Resized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
            {

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            //  Get the OpenGL object.
            OpenGL gl = myOpenGLControl . OpenGL;

            DoAspect ( );

            Resizes++;

            }

        private void Projection_DataGrid_Loaded ( object sender , RoutedEventArgs e )
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( Projection_DataGrid_Loaded ) ) );

            DataGrid DG = ProjectionMatrix_DataGrid;
            DG . Items . Add ( ProjectionMatrix [ 0 , 0 ] );
            DG . Items . Add ( ProjectionMatrix [ 0 , 1 ] );
            DG . Items . Add ( ProjectionMatrix [ 0 , 2 ] );

            }

        private void Projection_DataGrid_Initialized ( object sender , EventArgs e )
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( Projection_DataGrid_Initialized ) ) );

            DataGrid DG = ProjectionMatrix_DataGrid;

            }

        private void Save0_Button_Click ( object sender , RoutedEventArgs e )
            {
            WalkLogicalTree ( );
            }

        #region Serializers
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

        public void WalkLogicalTree ( )
            {
            InitalizeSavedControl ( );

            Debug . WriteLine ( String . Format ( "{0} Starting " , nameof ( WalkLogicalTree ) ) );
            WalkLogicalTree ( DMT_Main_Window_Control as FrameworkElement , 0 );

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( WalkLogicalTree ) ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MainWindows ) , SC . MainWindows ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . H_Sliders ) , SC . H_Sliders ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . CheckButtons ) , SC . CheckButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . RadioButtons ) , SC . RadioButtons ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . OtherControls ) , SC . OtherControls ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . MaxDepth ) , SC . MaxDepth ) );
            Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( SC . Calls ) , SC . Calls ) );
            Debug . WriteLine ( String . Format ( "{0} completed " , nameof ( WalkLogicalTree ) ) );

            }

        private void WalkLogicalTree ( FrameworkElement f , int Depth )
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

            var children = LogicalTreeHelper . GetChildren ( f );
            foreach ( var child in children )
                {

                FrameworkElement FE = child as FrameworkElement;

                WalkLogicalTree ( FE , Depth + 1 );

                }

            IsControlStateSavable ( f );

            }

        private void IsControlStateSavable ( UIElement e )
            {
            String NameString = GetName ( e );

            if ( NameString . EndsWith ( nameof ( DMT_Main_Window_Control ) ) )
                {
                DMT_Main_Window_Control_SaveState_Seralize ( NameString );
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

        private void RadioButtonSerialize ( string nameString , UIElement e )
            {
            RadioButton RB = e as RadioButton;
            if ( RB == null )
                {
                return;
                }

            String StateFileName = String . Format ( "{0}.xml" , nameString );

            var p = new RadioCheckBoxSaveState ( );
            p . CommonFields . SaveStateFileName = StateFileName;
            p . CommonFields . ControlClass = nameof ( RadioButton );
            p . CommonFields . ControlName = RB . Name;
            p . CommonFields . UpdatedFromXmlFiles = true;

            p . RadioCheckBoxState = RB . IsChecked . GetValueOrDefault ( );
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

        private void CheckBoxSerialize ( string nameString , UIElement E )
            {
            CheckBox CB = E as CheckBox;
            if ( CB == null )
                {
                return;
                }

            String StateFileName = String . Format ( "{0}.xml" , nameString );

            var p = new CheckBoxControlSaveState ( );
            p . CommonFields . SaveStateFileName = StateFileName;
            p . CommonFields . ControlClass = nameof ( CheckBox );
            p . CommonFields . ControlName = CB . Name;
            p . CommonFields . UpdatedFromXmlFiles = true;

            p . CheckBoxState = CB . IsChecked . GetValueOrDefault ( );
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

            }

        private void RadioCheckBoxSerialize ( string nameString , UIElement E )
            {
            RadioButton RB = E as RadioButton;
            if ( RB == null )
                {
                return;
                }

            String StateFileName = String . Format ( "{0}.xml" , nameString );

            var p = new RadioCheckBoxSaveState ( );
            p . CommonFields . SaveStateFileName = StateFileName;
            p . CommonFields . ControlName = RB . Name;
            p . CommonFields . ControlClass = nameof ( RadioButton );
            p . CommonFields . UpdatedFromXmlFiles = true;

            p . RadioCheckBoxState = RB . IsChecked . GetValueOrDefault ( );
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

        private void DMT_Main_Window_Control_SaveState_Seralize ( String name )
            {
            String StateFileName = String . Format ( "{0}.xml" , Name );

            var p = new DMT_Main_Window_Control_SaveState ( );
            p . CommonFields . ControlClass = nameof ( Window );
            p . CommonFields . ControlName = DMT_Main_Window_Control . Name;
            p . CommonFields . SaveStateFileName = StateFileName;
            p . CommonFields . UpdatedFromXmlFiles = true;

            p . Left = DMT_Main_Window_Control . Left;
            p . Top = DMT_Main_Window_Control . Top;
            ;

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

        private String GetName ( UIElement E )
            {
            var element = E as FrameworkElement;
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
        private void DMT_Main_Window_Control_LocationChanged ( object sender , EventArgs e )
            {
            Window W = sender as Window;
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

        #region Deserializers
        private void LoadEm ( )
            {
            String [ ] Xmls = System . IO . Directory . GetFiles ( @".\" , @"*Control.xml" , System . IO . SearchOption . TopDirectoryOnly );

            Debug . WriteLine ( String . Format ( "{0} loaded {1} *Control.xml files" , nameof ( LoadEm ) , Xmls . Count ( ) ) );

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
        static String XmlFileName;

        private void Load_Each ( string F )
            {
            //Debug . WriteLine ( String . Format ( "{0} {1}" , nameof ( Load_Each ) , F ) );

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

        private void WalkXmlReader ( XmlReader xmlReader )
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

        private void BlufbelsePrettyPrintElements ( XmlReader xmlReader )
            {
            Debug . Write ( String . Format ( @"{0}" , XmlFileName ) );
            Debug . Write ( String . Format ( @" at [{0},{1}]" ,
                ( ( IXmlLineInfo ) xmlReader ) . LineNumber . ToString ( "00" ) ,
                ( ( IXmlLineInfo ) xmlReader ) . LinePosition . ToString ( "00" ) ) );

            for ( int i = 0 ; i < xmlReader . Depth ; i++ )
                {
                Debug . Write ( " " );
                }

            switch ( xmlReader . NodeType )
                {
                case XmlNodeType . Element:
                    Debug . Write ( String . Format ( @"<" ) );
                    break;

                case XmlNodeType . EndElement:
                    Debug . Write ( String . Format ( @"<\" ) );
                    break;

                default:
                    break;
                }


            if ( xmlReader . HasValue )
                {
                Debug . Write ( String . Format ( @"{0}" , xmlReader . Value ) );
                }
            else
                {
                Debug . Write ( String . Format ( @"{0}" , xmlReader . Name ) );
                }

            switch ( xmlReader . NodeType )
                {
                case XmlNodeType . Element:
                    if ( xmlReader . HasAttributes )
                        {
                        BlomblisieAttributes ( xmlReader );
                        }
                    Debug . Write ( String . Format ( @">" ) );
                    break;

                case XmlNodeType . EndElement:
                    Debug . Write ( String . Format ( @">" ) );
                    break;

                default:
                    break;
                }

            Debug . WriteLine ( "" );
            }

        private void BlomblisieAttributes ( XmlReader xR )
            {
            BlomblisieAttributes ( xR , xR . AttributeCount , ( ( IXmlLineInfo ) xR ) . LineNumber , ( ( IXmlLineInfo ) xR ) . LinePosition );
            }
        private void BlomblisieAttributes ( XmlReader xR , int AttributeCount , int LineNumber , int LinePosition )
            {
            String NT = xR . NodeType . ToString ( );
            String NN = xR . Name;
            //String CT = xmlReader . ReadContentAsString ( );

            for ( int i = 0 ; i < AttributeCount ; i++ )
                {
                xR . MoveToAttribute ( i );
                var n = xR . Name;
                var v = xR . Value;
                IXmlLineInfo xmlInfo = ( IXmlLineInfo ) xR;
                int lineNumber = xmlInfo . LineNumber;
                int linePosition = xmlInfo . LinePosition;

                Debug . Write ( String . Format ( @" {0}=""{1}""" , n , v ) );
                }
            }

        private void DumpSubtree ( XmlReader xmlReader )
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
                        var o = x . Deserialize ( xmlReader );
                        p = ( SeralizeControlCommonFields ) o;
                        switch ( p . ControlClass )
                            {
                            case nameof ( Window ):
                                WindowControlDeseralizer ( p );
                                break;

                            case nameof ( CheckBox ):
                                CheckBoxControlDeseralizer ( p );
                                break;

                            case nameof ( RadioButton ):
                                RadioButtonControlDeseralizer ( p );
                                break;

                            case nameof ( H_Slider_UserControl1 ):
                                H_SliderUserControlDeseralizer ( p );
                                break;

                            default:
                                Debug . WriteLine ( String . Format ( "{0}" , p . ControlClass ) );
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

        private void H_SliderUserControlDeseralizer ( SeralizeControlCommonFields p )
            {
            if ( p == null )
                {
                throw new ArgumentNullException ( nameof ( p ) );
                }

            Object O = LogicalTreeHelper . FindLogicalNode ( DMT_Main_Window_Control , p . ControlName );
            H_Slider_UserControl1 HC = O as H_Slider_UserControl1;
            if ( HC == null )
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


            var o = x . Deserialize ( xmlReader );

            pp = ( H_Slider_UserControl1_SaveState_Class ) o;
            HC . SliderValue = pp . ResetValue;
            HC . SliderMaxValue = pp . MaxValue;
            HC . SliderMinValue = pp . MinValue;
            }

        private void RadioButtonControlDeseralizer ( SeralizeControlCommonFields p )
            {
            Object O = LogicalTreeHelper . FindLogicalNode ( DMT_Main_Window_Control , p . ControlName );
            RadioButton RB = O as RadioButton;
            if ( RB == null )
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

            var o = x . Deserialize ( xmlReader );

            pp = ( RadioCheckBoxSaveState ) o;
            RB . IsChecked = pp . RadioCheckBoxState;

            }

        private void CheckBoxControlDeseralizer ( SeralizeControlCommonFields p )
            {
            Object O = LogicalTreeHelper . FindLogicalNode ( DMT_Main_Window_Control , p . ControlName );
            CheckBox CB = O as CheckBox;
            if ( CB == null )
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

        private void WindowControlDeseralizer ( SeralizeControlCommonFields p )
            {
            Window C = LogicalTreeHelper . FindLogicalNode ( DMT_Main_Window_Control , p . ControlName ) as Window;

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
        void reshape ( OpenGL gl , int width , int height )
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof ( reshape ) ) );

            // GLsizei for non-negative integer
            DoAspect ( );

            // Set the viewport to cover the new window
            gl . Viewport ( 0 , 0 , width , height );

            //// Set the aspect ratio of the clipping volume to match the viewport
            //gl.MatrixMode(OpenGL.GL_PROJECTION);  // To operate on the Projection matrix
            //gl.LoadIdentity();             // Reset
            //                              // Enable perspective projection with fovy, aspect, zNear and zFar
            //gl.Perspective(45.0f, aspect, 0.1f, 100.0f);
            }

        private void UseLookAtViewingTransform_RadioButton_Control_Click ( object sender , RoutedEventArgs e )
            {
            LookAt_TabItem . IsSelected = true;
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

        private void Do_Orbit_Pull_Back_CheckBox_Control_Click ( object sender , RoutedEventArgs e )
            {
            Eye_and_Camera_TabItem . IsSelected = true;
            }

        private void SelectDataRangeForNormalization_DoNormalizationOnScratchSheet_Click ( object sender , RoutedEventArgs e )
            {
            myReoGridControl . CurrentWorksheet = myReoGridControl . Worksheets [ 0 ];
            var CW = myReoGridControl . CurrentWorksheet;
            if ( CW . Name == scratchy )
                {
                return;
                }

            var IsThisActualiedRange=CW . UsedRange;
            var xx = CW . MaxContentCol;
            var yy = CW . MaxContentRow;
            int R, C;

            GetActualizedRange ( CW: CW , maxRow: out R , maxCol: out C );

            unvell . ReoGrid . RangePosition DataRange = new unvell . ReoGrid . RangePosition ( row: 1 , col: 1 , rows: R , cols: C );

            unvell . ReoGrid . Worksheet Scratcheroo = myReoGridControl . Worksheets [ scratchy ];
            if ( Scratcheroo == null )
                {
                Scratcheroo = myReoGridControl . NewWorksheet ( name: scratchy );
                }
            
            myReoGridControl . CurrentWorksheet = Scratcheroo;
            for ( int j = 0 ; j <= R ; j++ )
                {
                Scratcheroo . SetCellData ( row: j , col: 0,data:"XX" );
                }
            for ( int i = 0 ; i <= C ; i++ )
                {
                Scratcheroo . SetCellData ( row: 0 , col: i , data: "XX" );
                }

            Scratcheroo . SelectionRange = DataRange;

            for ( int j = DataRange . StartPos . Col ; j <= DataRange . EndPos . Col ; j++ )
                {
                double Sigma = 0;
                double n = 0;
                for ( int i = DataRange . StartPos . Row ; i <= DataRange . EndPos . Row ; i++ )
                    {
                    Sigma += CW . GetCellData<double> ( row: i , col: j );
                    n++;
                    }
                double Norm = n / Sigma;
                Scratcheroo . SetRangeDataFormat ( range: DataRange , format: CellDataFormatFlag . Number ,
                    dataFormatArgs: new NumberDataFormatter . NumberFormatArgs ( )
                        {
                        DecimalPlaces = 3 ,
                        UseSeparator = true ,
                        } );
                Scratcheroo . SetCellData ( row: DataRange . EndPos . Row+1  , col: j+1 , data: Norm );
                for ( int i = DataRange . StartPos . Row ; i <= DataRange . EndPos . Row ; i++ )
                    {
                    var Val= CW . GetCellData<double> ( row: i , col: j );
                    double Normalized = Val * Norm;

                    Scratcheroo . SetCellData ( row: i , col: j , data: Normalized );
                    }
                }
            unvell . ReoGrid . NamedRange NamedDataRange = 
                new unvell . ReoGrid . NamedRange ( worksheet: Scratcheroo , name: scratchy , range:DataRange );
            var exists = Scratcheroo . NamedRanges . Contains ( range: NamedDataRange );
            if(!exists)
                {
                Scratcheroo . NamedRanges . Add ( NamedDataRange );
                }
            DoSaveSelectedData_Button . PerformClick ( );
            }

        private void InstallHandRowPivotButtonsInScratchSpreadsheet_CheckBox_Control_Checked ( object sender , RoutedEventArgs e )
            {

            unvell . ReoGrid . Worksheet Scratcheroo = myReoGridControl . Worksheets [ scratchy ];
            if ( Scratcheroo == null )
                {
                Scratcheroo = myReoGridControl . NewWorksheet ( name: scratchy );
                }

            myReoGridControl . CurrentWorksheet = Scratcheroo;
            int R, C;
            GetActualizedRange ( CW: Scratcheroo , maxRow: out R , maxCol: out C );
            for ( int j = 0 ; j <= R ; j++ )
                {
                //var CBC = new unvell . ReoGrid . CellTypes . CheckBoxCell ( );
                var CBC = new MyCheckBox ( );
                CBC .  Click += this . CBC_Click;
                Scratcheroo . SetCellData ( row: j , col: 0 , data: CBC );

                }
            for ( int i = 0 ; i <= C ; i++ )
                {
                var CBC=new unvell . ReoGrid . CellTypes . CheckBoxCell ( );
                CBC . Click += this . CBC_Click;
                CBC . CheckChanged += this . CBC_CheckChanged;
                }
            }

        private void CBC_CheckChanged ( object sender , EventArgs e )
            {

            Debug . WriteLine ( String . Format ( "{0}" , nameof(CBC_CheckChanged) ) );

            }

        private void CBC_Click ( object sender , EventArgs e )
            {
            Debug . WriteLine ( String . Format ( "{0}" , nameof ( CBC_CheckChanged ) ) );
            CheckBoxCell CBC = sender as CheckBoxCell;
            SwapRow ( CBC );
            }

        #region SwapRow
        void SwapRow ( CheckBoxCell CBC )
            {
            Worksheet Snatcheroo = CBC . Cell . Worksheet;
            SwapRow ( Snatcheroo , CBC );
            }

        void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC )
            {
            int R = CBC . Cell . Row;
            SwapRow ( Snatcheroo , CBC , R );
            }

        void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC , int R )
            {
            int r = -1;
            int colMax = -1;
            GetActualizedRange ( CW: Snatcheroo , maxRow: out r , maxCol: out colMax );
            SwapRow ( Snatcheroo: Snatcheroo, CBC: CBC, R:R, r: r, colMax: colMax);
            }

        void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC , int R, int r, int colMax)
            {
            RangePosition RR = new RangePosition ( row: R ,col: 1, rows: 1 , cols: colMax );

            Snatcheroo . InsertRows ( row: R , count: 1 );
            CellPosition cP = new CellPosition ( row: R , col: 0 );

            var action0 = new unvell . ReoGrid . Actions . CopyRangeAction ( fromRange: RR , toPosition: cP );
            myReoGridControl. DoAction ( action0 );
            var action1 = new unvell . ReoGrid . Actions . RemoveRowsAction ( row: R , rows: 1 );
            myReoGridControl . DoAction ( action1 ); 
            }
        
        #endregion SwapRow

        private void Do_Iterate_Resoures_Button_Click ( object sender , RoutedEventArgs e )
            {
            Debug . WriteLine ( String.Format("{0}: Manifest resources", nameof( Do_Iterate_Resoures_Button_Click)) );

            System . Reflection . Assembly assembly = System . Reflection . Assembly . GetExecutingAssembly ( );
            string [ ] names = assembly . GetManifestResourceNames ( );

            foreach ( string name in names )
                {
                PrintResourceFile ( name );
                }
            }

        private static void PrintResourceFile ( string name )
            {
            Debug . WriteLine ( "Items in " + name + ":" );

            System . Resources . ResourceManager rm = new System . Resources . ResourceManager ( baseName: name , assembly: System . Reflection . Assembly . GetExecutingAssembly ( ) );
            System . Resources . ResourceSet rset = null;

            try
                {
                rset = rm . GetResourceSet ( culture: CultureInfo . CurrentUICulture , createIfNotExists: true , tryParents: true );
                }
            catch ( Exception e )
                {

                Debug . WriteLine ( String . Format ( "{0}" , e ) );

                }
            if ( rset == null )
                {
                return;
                }

            foreach ( System . Collections . DictionaryEntry entry in rset )
                {
                Debug . WriteLine ( "\t{0}: {1}" , entry . Key , GetStringForValue ( entry . Value ) );
                }
            }

        private static string GetStringForValue ( object value )
            {
            if ( value == null )
                {
                return "null";
                }

            if ( value is Stream )
                {
                return "Stream: " + GetHead ( ( Stream ) value );
                }

            return value . ToString ( );
            }

        private static string GetHead ( Stream stream )
            {
            using ( var reader = new StreamReader ( stream ) )
                {
                var buffer = new char [ 40 ];
                int nChars = reader . Read ( buffer , 0 , buffer . Length );
                string text = new String ( buffer , 0 , nChars );

                if ( !reader . EndOfStream )
                    {
                    text += "...";
                    }

                return text;
                }

            }
 
        private void Elapsed_Label_Initialized ( object sender , EventArgs e )
            {
            Label L = sender as Label;
            UpdateElapsedTime ( L );
            }

        private void Elapsed_Label_MouseEnter ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            UpdateElapsedTime ( L );
            }

        private void Elapsed_Label_MouseLeave ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            UpdateElapsedTime ( L );
            }

        private void UpdateElapsedTime ( Label L )
            {
            ElapsedDateTime = DateTime . Now - StartDateTime;
            L . Content = ElapsedDateTime . ToString ( @"hh\:mm\:ss" );
            }

        private void myOpenGLControl_MouseMove ( object sender , MouseEventArgs e )
            {
            GetMyMouseXYPosition ( );
            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
            }

        private void myOpenGLControl_MouseWheel ( object sender , MouseWheelEventArgs e )
            {

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            }

        private void myOpenGLControl_MouseEnter ( object sender , MouseEventArgs e )
            {
            GetMyMouseXYPosition ( );

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
            }

        private void myOpenGLControl_MouseLeave ( object sender , MouseEventArgs e )
            {
            GetMyMouseXYPosition ( );

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
            }

        private void GetMyMouseXYPosition ( )
            {
            System . Windows . Point p = Mouse . GetPosition ( myOpenGLControl );

            viewport_height = ( int ) myOpenGLControl . ActualHeight;
            viewport_width = ( int ) myOpenGLControl . ActualWidth;

            mouse_x = ( int ) ( p . X + 0.5d );
            mouse_y = ( int ) ( p . Y + 0.5d );
            mouse_corrected_y = ( int ) ( viewport_height - mouse_y );
            viewport_cursor_x = mouse_x;
            viewport_cursor_y = mouse_corrected_y;

            //System . Diagnostics . Debug . WriteLine ( String . Format ( "[{0},{1}]", mouse_x, mouse_y ) );
            }

        private void DoDrawReoGridSpreadsheet_CheckBox_Control_Click ( object sender , RoutedEventArgs e )
            {

            System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            CheckBox CB = sender as CheckBox;

            if ( CB . IsChecked . GetValueOrDefault ( ) )
                {
                myReoGridControl . Visibility = Visibility . Visible;
                }
            else
                {
                myReoGridControl . Visibility = Visibility . Collapsed;
                }
            myOpenGLControl_Resized ( sender: null , args: null );
            }

        private void Sheet_Number_Label_MouseEnter ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            L . Content = GetCurrentSheetIndex ( );
            }

        private void Sheet_Number_Label_MouseLeave ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            L . Content = GetCurrentSheetIndex ( );
            }

        private int GetCurrentSheetIndex ( )
            {
            var b = myReoGridControl . CurrentWorksheet . Name;
            var a = myReoGridControl . GetWorksheetIndex ( b );
            CurrentWorksheetIndex = a;
            return a;
            }

        private void Aspect_Label_MouseEnter ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            DoAspect ( );
            L . Content = myOpenGLControlViewportAspect . ToString ( "#0.0#");
            }

        private void Aspect_Label_MouseLeave ( object sender , MouseEventArgs e )
            {
            Label L = sender as Label;
            DoAspect ( );
            L . Content = MainWindow.myOpenGLControlViewportAspect.ToString("#0.0#");
            }

        private void DoSaveSelectedData_Button_Click ( object sender , RoutedEventArgs e )
            {
            Worksheet Scratcheroo = myReoGridControl.GetWorksheetByName( scratchy );
            if ( Scratcheroo == null )
                {
                return;
                }

            myReoGridControl . CurrentWorksheet = Scratcheroo;
            Worksheet . ReoGridRangeCollection R=Scratcheroo . Ranges;
            NamedRange named_ranges = Scratcheroo . NamedRanges[ scratchy ];

            Sheety = new NWorksheety ( );
            Sheety . r0 = named_ranges . StartPos . Row;
            Sheety . c0 = named_ranges . StartPos . Col;
            Sheety . r = named_ranges . Rows + Sheety . r0;
            Sheety . c = named_ranges . Cols + Sheety . c0;
            Sheety . cells = new float [ Sheety.c+2 , Sheety.r+2 ];

            System . Diagnostics . Debug . WriteLine ( String . Format ( "named ranged count {0} {1} " , named_ranges , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

            for ( int j = named_ranges . StartPos . Col ; j < named_ranges . EndPos . Col ; j++ )
                {
                for ( int i = named_ranges . StartPos . Row ; i < named_ranges . EndPos . Row ; i++ )
                    {
                    Sheety.cells [ j , i ] = named_ranges . Cells [ row: i , col: j ] . GetData<float> ( );
                    }
                }
            }
        }
    }
 
