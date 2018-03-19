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
            

            foreach(Screen s in WpfScreenHelper.Screen.AllScreens)
            {

                Debug.WriteLine(String.Format("{0} {1}",s.DeviceName, s.Bounds.Left));

            }
            this.Left = -1290;
            this.Top = 0;

            InitializeComponent();
            Debug.WriteLine(String.Format("{0}", nameof(MainWindow)));

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
			//Debug.WriteLine(String.Format("{0} {1}", nameof(myOpenGLControl_OpenGLDraw), Draws));
			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Clear the color and depth buffer.
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Projection);

			// Move Left And Into The Screen
			gl.LoadIdentity();

			//gl.Translate(0.0f, 0.0f, -6.0f);

			if(UseLookAtViewingTransform_RadioButton.IsChecked.GetValueOrDefault())
            {
                LookAt(gl);
            }

            if (UsePerspetiveViewingTransform.IsChecked.GetValueOrDefault())
            {
                Perspective(gl);
            }

            //gl.Translate(Eye_X_H_Sslider_UserControl.SliderValue, Eye_Y_H_Sslider_UserControl.SliderValue, Eye_Z_H_Sslider_UserControl.SliderValue);


            gl.MatrixMode(SharpGL.Enumerations.MatrixMode.Modelview);

			gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            if (AxisDrawMe_CheckBox.IsChecked.GetValueOrDefault())
			{
				Axis_Arrow_Grid.Axis_Class.MyGlobalAxis(gl);
			}

			if (DrawTeaPot_CheckBox.IsChecked.GetValueOrDefault())
			{
				Teapot tp = new Teapot();
				tp.Draw(gl, 14, 1, OpenGL.GL_FILL);
			}
			rotation += 3.0f;
			Draws_Label.Content = String.Format("Draw Count: {0}", Draws);
			Draws++;
		}
		private void LookAt(OpenGL gl)
		{
			float x_up = 0.0f;
			float y_up = 1.0f;
			float z_up = 0.0f;

			if (LookAt_X_Up_RadioButton.IsChecked.GetValueOrDefault()) { x_up = 1.0f; } else { x_up = 0.0f; }
			if (LookAt_Y_Up_RadioButton.IsChecked.GetValueOrDefault()) { y_up = 1.0f; } else { y_up = 0.0f; }
			if (LookAt_Z_Up_RadioButton.IsChecked.GetValueOrDefault()) { z_up = 1.0f; } else { z_up = 0.0f; }

            gl.LookAt(
                LookAt_Eye_X_H_Slider_UserControl.SliderValue, 
                LookAt_Eye_Y_H_Slider_UserControl.SliderValue, 
                LookAt_Eye_Z_H_Slider_UserControl.SliderValue,
                LookAtTarget_X_H_Slider_UserControl.SliderValue, 
                LookAtTarget_Y_H_Slider_UserControl.SliderValue, 
                LookAtTarget_Z_H_Slider_UserControl.SliderValue,
                x_up, y_up, z_up);
        }

        private void Perspective(OpenGL gl)
		{
            //(double fovy, double aspect, double zNear, double zFar)
            gl.Perspective(Perspective_FOVY_H_Slider_UserControl.SliderValue,
                Perspective_ASPECT_H_Slider_UserControl.SliderValue,
                Perspective_Z_NEAR_H_Slider_UserControl.SliderValue,
                Perspective_Z_FAR_H_Slider_UserControl.SliderValue);
        }
        private void myOpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

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

			Debug.WriteLine(String.Format("{0}", nameof(myOpenGLControl_OpenGLInitialized)));
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

        private void LookAt_Eye_X_H_Slider_UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine(String.Format("{0}", nameof(LookAt_Eye_X_H_Slider_UserControl_Loaded)));

        }

        private void myOpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Set the projection matrix.
			gl.MatrixMode(OpenGL.GL_PROJECTION);

			Resizes++;

		}

	}
}
