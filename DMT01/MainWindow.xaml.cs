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
			Debug.WriteLine(String.Format("{0} {1}", nameof(myOpenGLControl_OpenGLDraw), Draws));
			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Clear the color and depth buffer.
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			// Move Left And Into The Screen
			gl.LoadIdentity();
			//gl.Translate(0.0f, 0.0f, -6.0f);
			gl.Translate(0.0f, 0.0f, -Eye_Z_H_Sslider_UserControl.SliderValue);


			gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

			Teapot tp = new Teapot();
			tp.Draw(gl, 14, 1, OpenGL.GL_FILL);

			rotation += 3.0f;
			if (AxisDrawMe_CheckBox.IsChecked.GetValueOrDefault())
			{
				Axis_Arrow_Grid.Axis_Class.MyGlobalAxis(gl);
			}
			Draws_Label.Content = String.Format("Draw Count: {0}", Draws);
			Draws++;
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
            var a = DMT01.Properties.Resources.UNEP_NATDIS_disasters_2002_2010;

            
            
             myReoGridControl.Load(Path, unvell.ReoGrid.IO.FileFormat.Excel2007);
           

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
