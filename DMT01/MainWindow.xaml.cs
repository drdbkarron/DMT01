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


		private void myOpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			Debug.WriteLine(String.Format("{0} {1}", nameof(myOpenGLControl_OpenGLDraw), Draws));
			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Clear the color and depth buffer.
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			if(AxisDrawMe_CheckBox.IsChecked.GetValueOrDefault())
			{
				Axis_Arrow_Grid.Axis_Class.MyGlobalAxis(gl);
			}
			Draws++;
		}

		private void myOpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

			//  Get the OpenGL object.
			OpenGL gl = myOpenGLControl.OpenGL;

			//  Set the clear color.
			gl.ClearColor(.1f, 0, 0, 0);
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
