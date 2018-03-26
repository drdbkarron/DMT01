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

namespace DMT01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		static long Draws=0, Resizes = 0;
        public MainWindow()
        {
            
            Debug.WriteLine(String.Format("{0}", nameof(MainWindow)));

            foreach(Screen s in WpfScreenHelper.Screen.AllScreens)
            {

                Debug.WriteLine(String.Format("{0} {1}",s.DeviceName, s.Bounds));

            }
 
            InitializeComponent();

		}

        private String Stringify(Rect bounds)
        {

            String s1 = String.Format("{0}", nameof(bounds.Bottom), bounds.Bottom);
            String s2 = String.Format("{0}", nameof(bounds.Top), bounds.Top);
            String s3 = String.Format("{0}", nameof(bounds.Left), bounds.Left);
            String s4 = String.Format("{0}", nameof(bounds.Right), bounds.Right);
            String s5 = String.Format("{0}", nameof(bounds.TopRight), bounds.TopRight);
            return s1+s2+s3+s4+s5;
        }

        private void myReoGridControl_Loaded( object sender , RoutedEventArgs e )
		{

		System.Diagnostics.Debug.WriteLine(String.Format("{0}", nameof(myReoGridControl_Loaded)));
			TextRange tr1 = new TextRange(SpreadsheetDirPath_RichTextBox.Document.ContentStart, SpreadsheetDirPath_RichTextBox.Document.ContentEnd);
			TextRange tr2 = new TextRange(SpreadsheetFileName_RichTextBox.Document.ContentStart, SpreadsheetFileName_RichTextBox.Document.ContentEnd);
			String Path = String.Format(@"{0}\{1}", tr1.Text.Trim(), tr2.Text.Trim());
			if(System.IO.File.Exists(Path))
			{
				myReoGridControl.Load(Path, unvell.ReoGrid.IO.FileFormat.Excel2007);
			}
		}

		float rotation = 0;
		private void myOpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			
            Debug.WriteLine(String.Format("{0}", nameof(myOpenGLControl_OpenGLDraw)));
			
            //  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );

            gl . Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Projection);

			gl.LoadIdentity();

            GlmSharp.mat4 M=GlmSharp.mat4.Identity;



            if (UsePerspetiveViewingTransform.IsChecked.GetValueOrDefault())
            {
                M=M*Perspective(gl);
            }
            

			if(UseLookAtViewingTransform_RadioButton.IsChecked.GetValueOrDefault())
            {
                M=M*LookAt(gl);
            }

            LoadMatrix ( gl , M );

            DebugGl ( gl, "bongotronic" );

            gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);
            gl . LoadIdentity ( );

            gl . Translate(Eye_X_H_Sslider_UserControl.SliderValue, Eye_Y_H_Sslider_UserControl.SliderValue, Eye_Z_H_Sslider_UserControl.SliderValue);


            if (AxisDrawMe_CheckBox.IsChecked.GetValueOrDefault())
			{
				Axis_Arrow_Grid.Axis_Class.MyGlobalAxis(gl);
			}

			if (DrawTeaPot_CheckBox.IsChecked.GetValueOrDefault())
			{
				Teapot tp = new Teapot();
				tp.Draw(gl, 14, 1, OpenGL.GL_FILL);
			}

            gl . Rotate(rotation, 0.0f, 1.0f, 0.0f);

            rotation += 3.0f;
            gl . Flush ( );
			Draws_Label.Content = String.Format("Draw Count: {0}", Draws);
			Draws++;
		}
        static SharpGL.SceneGraph.Matrix ProjectionMatrix=new SharpGL.SceneGraph.Matrix(4,4);
        static SharpGL.SceneGraph.Matrix  ModelingMatrix=new SharpGL.SceneGraph.Matrix(4,4);

        private void DebugGl ( SharpGL.OpenGL gl, string v )
        {
            ProjectionMatrix = gl . GetProjectionMatrix ( );
            ModelingMatrix = gl . GetProjectionMatrix ( );
        }

        private void LoadMatrix ( SharpGL.OpenGL gl , mat4 m4 )
        {
            vec4 c0=m4.Column0;
            vec4 c1=m4.Column1;
            vec4 c2=m4.Column2;
            vec4 c3=m4.Column3;

            double[] m=new double[16]{ c0 [ 0 ] , c0 [ 1 ] , c0 [ 2 ] , c0 [ 3 ] ,
                                       c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] , c1 [ 3 ] ,
                                       c2 [ 0 ] , c2 [ 1 ] , c2 [ 2 ] , c2 [ 3 ] ,
                                       c3 [ 0 ] , c3 [ 1 ] , c3 [ 2 ] , c3 [ 3 ] };

            gl.  LoadMatrix ( m );

           
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

        private mat4 Perspective(OpenGL gl)
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

        private void myOpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			Debug.WriteLine(String.Format("{0}", nameof(myOpenGLControl_OpenGLInitialized)));

			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Set the clear color.
			gl.ClearColor(.1f, 0.1f, 0.1f, 0);

			gl.Enable(OpenGL.GL_DEPTH_TEST);

			float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
			float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
			float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
			float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
			float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

			float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
			gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

			gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
			gl.Enable(OpenGL.GL_LIGHTING);
			gl.Enable(OpenGL.GL_LIGHT0);

			gl.ShadeModel(OpenGL.GL_SMOOTH);

            gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );


		}

        private void spreadsheet_load_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0}", nameof(spreadsheet_load_Button_Click)));

            TextRange tr1 = new TextRange(SpreadsheetDirPath_RichTextBox.Document.ContentStart, SpreadsheetDirPath_RichTextBox.Document.ContentEnd);
            TextRange tr2 = new TextRange(SpreadsheetFileName_RichTextBox.Document.ContentStart, SpreadsheetFileName_RichTextBox.Document.ContentEnd);
            String Path = String.Format(@"{0}\{1}", tr1.Text.Trim(), tr2.Text.Trim());

			const String scratchy = "Scratcheroo";

			var a = DMT01.Properties.Resources.UNEP_NATDIS_disasters_2002_2010;
			System.IO.File.WriteAllBytes(scratchy, a);
            
            
             myReoGridControl.Load(scratchy, unvell.ReoGrid.IO.FileFormat.Excel2007);
           

        }


        private void DMTWindow_Initialized(object sender, EventArgs e)
        {

            Debug.WriteLine(String.Format("{0}", nameof(DMTWindow_Initialized)));

        }

        private void DMTWindow_Loaded(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine(String.Format("{0}", nameof(DMTWindow_Loaded)));

        }

        private void myOpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

            Debug.WriteLine(String.Format("{0}", nameof(myOpenGLControl_Resized)));

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

            Debug . WriteLine ( String . Format ( "{0}" , "snippy" ) );
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

        void reshape (OpenGL gl, int width, int height)
        {  // GLsizei for non-negative integer
           // Compute aspect ratio of the new window
            if (height == 0)
            {
                height = 1;                // To prevent divide by 0
            }

            float aspect = (float)width / (float)height;

            // Set the viewport to cover the new window
            gl.Viewport(0, 0, width, height);

            // Set the aspect ratio of the clipping volume to match the viewport
            gl.MatrixMode(OpenGL.GL_PROJECTION);  // To operate on the Projection matrix
            gl.LoadIdentity();             // Reset
                                          // Enable perspective projection with fovy, aspect, zNear and zFar
            gl.Perspective(45.0f, aspect, 0.1f, 100.0f);
        }
    }
}
