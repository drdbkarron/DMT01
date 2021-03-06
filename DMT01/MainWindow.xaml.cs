﻿using SharpGL;
using System;
using System .Collections .Generic;
using System .Diagnostics;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Controls;
using System .Windows .Data;
using System .Windows .Documents;
using System .Windows .Input;
using System .Windows .Media;
using System .Windows .Media .Imaging;
using System .Windows .Navigation;
using System .Windows .Resources;
using System .Windows .Shapes;
using System .Drawing;
using System .Xml;
using System .Resources;
using System .Reflection;
using System .IO;
using System .Globalization;
using System .Xml .Serialization;
using System .Threading;
using GlmNet;
using SharpGL .Enumerations;
using SharpGL .SceneGraph;
using SharpGL .SceneGraph .Cameras;
using SharpGL .SceneGraph .Core;
using SharpGL .SceneGraph .Primitives;
using SharpGL .WPF .SceneTree;
using unvell .ReoGrid .DataFormat;
using unvell .ReoGrid .CellTypes;
using unvell .ReoGrid;
using unvell .ReoGrid .Actions;
using unvell .ReoGrid .Rendering;
using unvell .ReoGrid .Graphics;
using WpfScreenHelper;
using LocalMaths;
using WpfControlLibrary1;
using Syroot . Windows . IO;

namespace DMT01
{
	public partial class MainWindow : Window
		{
		#region Persistance_classes2
		static long Draws ;
		static long Resizes ;
		static DateTime StartDateTime;
		static TimeSpan ElapsedDateTime;
		static int CurrentWorksheetIndex;
		public static SharpGL.OpenGL staticGLHook;

		public static SharpGL . SceneGraph . Matrix ProjectionMatrix = new SharpGL . SceneGraph . Matrix ( 4 , 4 );
		public static SharpGL . SceneGraph . Matrix ModelingMatrix = new SharpGL . SceneGraph . Matrix ( 4 , 4 );
		public static float myOpenGLControlViewportAspect = -1.1f;

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
			public int r1, c1, r0,c0;
			}

		static NWorksheety Sheety;
					  static public ReoGridControl myReoGridControl_STATIC;
		#endregion Persistance_classes2

		public MainWindow ( )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "MainWindow()" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Replace ( @"C:\Users\karro\Source\Repos\DMT01\" , "${Soln}" ) . Trim ( ) ) );

			this . InitializeComponent ( );

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "InitalizeComponent Completed" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

		private void DMTMainWindow_Initialized ( object sender , EventArgs e )
			{
			Debug . WriteLine ( String . Format ( "{0}" , nameof ( DMTMainWindow_Initialized ) ) );
			}

		private void DMTMainWindow_Loaded ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting scripty check buttons" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			if ( this . LaunchSavedStateOnInitalization_RadioButton_Control . IsChecked . Value )
				{
				this . Load_XML_Saved_Control_States_Button . PerformClick ( );
				}
			if ( this . LoadSpreadsheetAtInitalization_CheckBox_Control . IsChecked . Value )
				{
				this . spreadsheet_load_Button . PerformClick ( );
				}
			if ( this . Do_Execute_Select_Data_Button_on_Startup_CheckBox_Control . IsChecked . Value )
				{
				this . DoSelectDataRangeForNormalization_Button . PerformClick ( );
				}

			StartDateTime = System . DateTime . Now;

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Finished scripty" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

		private void myReoGridControl_Loaded ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Loading Spreadsheet" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			TextRange tr1 = new TextRange ( this.SpreadsheetDirPath_RichTextBox . Document . ContentStart , this.SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
			TextRange tr2 = new TextRange ( this.SpreadsheetFileName_RichTextBox . Document . ContentStart , this.SpreadsheetFileName_RichTextBox . Document . ContentEnd );

			String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );

			if ( System . IO . File . Exists ( Path ) )
				{
				this . myReoGridControl . Load ( Path , unvell . ReoGrid . IO . FileFormat . Excel2007 );
				}
			myReoGridControl_STATIC=this.myReoGridControl;
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Loading Spreadsheet completed" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

		private void myOpenGLControl_OpenGLDraw ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
			{
			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			//  Get the OpenGL object.
			OpenGL gl = this . myOpenGLControl . OpenGL;
			staticGLHook = gl;

			//  Clear the color and depth buffer.
			//gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );
			//gl . Disable ( OpenGL . GL_DOUBLEBUFFER );
			gl . Clear ( OpenGL . GL_COLOR_BUFFER_BIT | OpenGL . GL_DEPTH_BUFFER_BIT );
			//GlmSharp . mat4 M = GlmSharp . mat4 . Identity;

			gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Projection );
			gl . LoadIdentity ( );
			//gl . Flush ( );
			if ( this . DrawMouseScreenSpaceAnnotation_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				float fSize = this.MouseScreenAnnotationFontSize_H_Slider_UserControl . SliderValue + 0.5f;
				String text = String . Format ( "{0},{1}" , mouse_x , mouse_y );
				gl . DrawText ( x: viewport_cursor_x , y: viewport_cursor_y , r: 1f , g: 1f , b: 1f ,
					faceName: "Arial" ,
					fontSize: fSize ,
					text: text );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
				}

			if ( this . DrawScreenSpaceAnnotationGrid_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				const int bloppie = 40;
				float fSize = this.ScreenAnnotationFont_SizeH_Slider_UserControl . SliderValue;
				for ( int i = bloppie ; i < viewport_width - bloppie ; i += bloppie )
					{
					for ( int j = bloppie ; j < viewport_height - bloppie ; j += bloppie )
						{
						string t = String . Format ( "{0},{1}" , i , j );
						int jy = ( int ) viewport_height - j;
						gl . DrawText ( i , jy , 1 , 1 , 1 , "Arial" , fSize , t );
						}
					}
				gl . PopMatrix ( );
				gl . PopAttrib ( );
				}

			if ( this . UseOrthographic_Viewing_Transform_radioButton_Control . IsChecked . Value )
				{
				gl . Ortho ( left: this . Orthographic_Left_H_Slider_UserControl . SliderValue ,
							right: this . Orthographic_Right_H_Slider_UserControl . SliderValue ,
							bottom: this . Orthographic_Bottom_H_Slider_UserControl . SliderValue ,
							top: this . Orthographic_Top_H_Slider_UserControl . SliderValue ,
							zNear: this . Orthographic_Near_H_Slider_UserControl . SliderValue ,
							zFar: this . Orthographic_Far_H_Slider_UserControl . SliderValue
							);
				}

			if ( this . Use_Viewing_Frustrum_RadioButton_Control . IsChecked . Value )
				{
				gl . Frustum ( left: this . Frustum_Left_H_Slider_UserControl . SliderValue ,
					right: this . Frustum_Right_H_Slider_UserControl . SliderValue ,
					bottom: this . Frustum_Bottom_H_Slider_UserControl . SliderValue ,
					top: this . Frustum_Top_H_Slider_UserControl . SliderValue ,
					zNear: this . Frustum_zNear_H_Slider_UserControl . SliderValue ,
					zFar: this . Frustum_zFar_H_Slider_UserControl . SliderValue );
				}

			if ( this . UsePerspetiveViewingTransform_RadioButton_Control . IsChecked . Value )
				{
				gl . Perspective (
					fovy: this . Perspective_FOVY_H_Slider_UserControl . SliderValue ,
					aspect: this . Perspective_ASPECT_H_Slider_UserControl . SliderValue ,
					zNear: this . Perspective_Z_NEAR_H_Slider_UserControl . SliderValue ,
					zFar: this . Perspective_Z_FAR_H_Slider_UserControl . SliderValue );

				//M = M * Perspective ( gl );
				}

			//////LoadMatrix ( gl , M );

			//////var m=gl . GetProjectionMatrix ( );

			gl . MatrixMode ( SharpGL . Enumerations . MatrixMode . Modelview );
			gl . LoadIdentity ( );

			if ( this . UseLookAtViewingTransform_RadioButton_Control . IsChecked .Value )
				{
				float x_up = 0.0f;
				float y_up = 1.0f;
				float z_up = 0.0f;
				gl . LookAt (
					eyex: this . LookAt_Eye_X_H_Slider_UserControl . SliderValue ,
					eyey: this . LookAt_Eye_Y_H_Slider_UserControl . SliderValue ,
					eyez: this . LookAt_Eye_Z_H_Slider_UserControl . SliderValue ,
					centerx: this . LookAtTarget_X_H_Slider_UserControl . SliderValue ,
					centery: this . LookAtTarget_Y_H_Slider_UserControl . SliderValue ,
					centerz: this . LookAtTarget_Z_H_Slider_UserControl . SliderValue ,
					upx: x_up , upy: y_up , upz: z_up );
				}

			if ( this . Do_Orbit_Pull_Back_CheckBox_Control . IsChecked .Value )
				{
				gl . Translate (
					x: this . Eye_X_H_Slider_UserControl . SliderValue ,
					y: this . Eye_Y_H_Slider_UserControl . SliderValue ,
					z: this . Eye_Z_H_Slider_UserControl . SliderValue );
				}

			if ( this . Do_Orbit_CheckBox_Control . IsChecked .Value )
				{
				gl . Translate ( 0.0 , 0.0 , this . Orbit_Radius_H_Slider_UserControl . SliderValue );

				gl . Rotate ( angle: this . Orbit_Rotation_H_Slider_UserControl . SliderValue , x: 0.0f , y: 1.0f , z: 0.0f );

				this . Orbit_Rotation_H_Slider_UserControl . SliderValue += this . Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;

				if ( this . Orbit_Rotation_H_Slider_UserControl . SliderValue >= this . Orbit_Rotation_H_Slider_UserControl . SliderMaxValue )
					{
					this . Orbit_Delta_Angle_H_Slider_UserControl . SliderValue = -this . Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;
					}
				else if ( this . Orbit_Rotation_H_Slider_UserControl . SliderValue <= this . Orbit_Rotation_H_Slider_UserControl . SliderMinValue )
					{
					this . Orbit_Delta_Angle_H_Slider_UserControl . SliderValue = -this . Orbit_Delta_Angle_H_Slider_UserControl . SliderValue;
					}
				}

			if ( this . AxisDrawMe_CheckBox_Control . IsChecked.Value )
				{
				int Al = ( int ) ( ( this.Axis_Length_XYZ_H_Slider_UserControl . SliderValue ) + 0.5f );

				int lw = ( int ) ( ( this.Axis_Linewidth_H_Slider_UserControl . SliderValue ) + 0.5f );

				int ps = ( int ) ( ( this.Axis_TickSize_H_Slider_UserControl . SliderValue ) + 0.5f );

				Axis_Arrow_Grid . Axis_Class . MyGlobalAxis (
					gl: gl ,
					AxesLength: Al ,
					Pointsize: ps ,
					LineWidth: lw ,
					DoMinusTicks: this . Axis_DrawNegativeCheckBox_Control . IsChecked . Value ,
					TagOrigin: this . AnnotateOrigin_CheckBox_Control . IsChecked . Value ,
					DoXYZAnnotation: this . AnnotateAxisXYZ_CheckBox_Control . IsChecked . Value ,
					DoUnitTicks: this . DoDoTickMarks_CheckBox_Control . IsChecked . Value ,
					DoAnnotateZTicks: this . AnnotateZTickMarks_CheckBox_Control . IsChecked . Value ,
					DoAnnotateYTicks: this . AnnotateYTickMarks_CheckBox_Control . IsChecked . Value ,
					DoAnnotateXTicks: this . AnnotateXTickMarks_CheckBox_Control . IsChecked . Value ,
					tick_annotation_scale: this . Axis_tick_annotation_scale_H_Slider_UserControl . SliderValue ,
					Draw_Minus_Z_Axis_Leg: this . Draw_Minus_Z_Axis_Leg_CheckBox_Control . IsChecked . Value
					);
				}

			if ( this . DoYourArmsTooShortToBoxWithHashem . IsChecked . Value )
				{
				this.ArmsTooShortToBoxWithHashem ( gl );
				}

			if ( this . DrawTeaPot_CheckBox_Control . IsChecked . Value )
				{
				Teapot tp = new Teapot ( );
				tp . Draw ( gl , 14 , 1 , OpenGL . GL_FILL );
				}

			if ( this . Spreadsheet_Grid_CheckBox_Control . IsChecked . Value )
				{
				this . ReoGrid3DSpreadsheet ( gl: gl );
				}

			////DoAspect ( );

			//gl . Flush ( );

			this . Draws_Label . Content = String . Format ( "Draw Count: {0}" , Draws );
			Draws++;
			}

		private void ArmsTooShortToBoxWithHashem ( SharpGL . OpenGL gl )
		{
			if ( Sheety == null )
			{
				return;
			}

			staticGLHook = gl;
			if ( staticGLHook == null )
			{
				return;
			}

			Window mw = Application . Current . MainWindow;
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . PushMatrix ( );
			gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
			gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
			gl . Scale ( 1 , -1 , 1 );

			if ( Region . Selected_Region == null )
			{
				Region . Selected_Region = new Region ( C: Sheety . cells , StartRows: Sheety . r0 , EndRows: Sheety . r1 , StartCols: Sheety . c0 , EndCols: Sheety . c1 );
			}
			int ChokerLowRow = 12;
			int ChokerHighRow = 20;

			//Selected_Region .LoadRegionIntoQuadSelector (  this.RegionQuadComboBoxUser_Control );

			if( this . DoGuts_CheckBox_Control .IsChecked.Value)
			{
				for ( int j = Sheety . r0 ; j < Sheety . r1 ; j++ )
				{
					for ( int i = Sheety . c0 ; i < Sheety . c1 ; i++ )
					{
						if ( Tweeners.IsInBetween ( ChokerLowRow , j , ChokerHighRow ) )
						{
							Boxel B = Region . Selected_Region . B [ i , j ];
							if ( B == null )
							{
								B = new Boxel ( i , j , Sheety . cells )
								{
									MW = ( DMT01 . MainWindow ) mw ,
									ParentRegion = Region . Selected_Region
								};
								Region . Selected_Region . B [ i , j ] = B;
							}
							else
							{
								B = Region . Selected_Region . B [ i , j ];
							}

							B . DrawMe ( );
						}
					}
				}
			}

			if ( this . DoBorder_CheckBox_Control . IsChecked . Value )
			{
				if ( this . DoBorder_RIGHT_CheckBox_Control . IsChecked . Value )
				{
					for ( int j = ChokerLowRow ; j < ChokerHighRow ; j++ )
					{
						int i = Sheety . c1;
						BorderIceRows ( mw , ChokerLowRow , ChokerHighRow , i ,j);
					}
				}

				if ( this . DoBorder_LEFT_CheckBox_Control . IsChecked . Value )
				{
					for ( int j = ChokerLowRow ; j < ChokerHighRow ; j++ )
					{
						int i = Sheety . c0 - 1;
						BorderIceRows ( mw , ChokerLowRow , ChokerHighRow , i ,j );
					}
				}

				if ( this . DoBorder_BOTTOM_CheckBox_Control . IsChecked . Value )
				{
					for ( int i = Sheety . c0-1 ; i < Sheety . c1+1 ; i++ )
					{
						int j = ChokerLowRow - 1;
						BorderIceCols ( mw , ChokerLowRow , ChokerHighRow , i , j );
					}
				}

				if ( this . DoBorder_TOP_CheckBox_Control . IsChecked . Value )
				{
					for ( int i = Sheety . c0-1 ; i < Sheety . c1+1 ; i++ )
					{
						int j = ChokerHighRow;
						BorderIceCols ( mw , ChokerLowRow , ChokerHighRow , i , j );
					}
				}
			}

			if ( this . DoContourAnLabelAtThreshold . IsChecked . Value )
			{
			ClearRecursionCrumbs(ChokerLowRow,ChokerHighRow);
				for ( int i = Sheety . c0 ; i < Sheety . c1 ; i++ )
				{
					for ( int j = ChokerLowRow ; j < ChokerHighRow ; j++ )
					{
						if ( Region . Selected_Region . B [ i , j ] != null )
						{
							Boxel B = Region . Selected_Region . B [ i , j ];
							if ( B . BoxelKindEnum == BoxelKindEnum . IsNotCriticalTwoHits )
							{

								if ( Region.Selected_Region . RecursionSeedBoxel == null )
								{
									Region.Selected_Region . RecursionSeedBoxel = new Boxel [ 4 ];
								}
								Region.Selected_Region . RecursionSeedBoxel [ 0 ] = B;
								if(B.BreadCrumbByteLabel[0]!=0b0)
									continue;
								Boxel.BoxelScanStart ( B );
							}

						}

					}
				}
			}
			gl . PopMatrix ( );
			gl . PopAttrib ( );

			if ( this . DoGlobalSweepThreshold_CheckBox_Control . IsChecked . Value )
			{
				this . CriticalitySweeper_THRESHOLD_H_Slider_User_Control . SliderValue += this . CriticalitySweeper_DELTA_H_Slider_User_Control .SliderValue;
				Region . Selected_Region. Titration_Steps++;

				if ( this. CriticalitySweeper_THRESHOLD_H_Slider_User_Control . SliderValue <= this .CriticalitySweeper_LOW_H_Slider_User_Control . SliderValue )
				{
					this . CriticalitySweeper_DELTA_H_Slider_User_Control . SliderValue = -this . CriticalitySweeper_DELTA_H_Slider_User_Control . SliderValue;
					this .CriticalitySweeper_THRESHOLD_H_Slider_User_Control . SliderValue = this .  CriticalitySweeper_LOW_H_Slider_User_Control . SliderValue;
				}
				if ( this .CriticalitySweeper_THRESHOLD_H_Slider_User_Control . SliderValue >= this . CriticalitySweeper_HIGH_H_Slider_User_Control . SliderValue )
				{
					this . CriticalitySweeper_DELTA_H_Slider_User_Control . SliderValue = -this . CriticalitySweeper_DELTA_H_Slider_User_Control . SliderValue;
					this.region_threshold_H_Slider_UserControl . SliderValue = this . CriticalitySweeper_HIGH_H_Slider_User_Control . SliderValue;
				}
			}
		}

		private void ClearRecursionCrumbs (int ChokerLowRow, int ChokerHighRow )
		{
			for ( int i = Sheety . c0 ; i < Sheety . c1 ; i++ )
			{
				for ( int j = ChokerLowRow ; j < ChokerHighRow ; j++ )
				{
					if ( Region . Selected_Region . B [ i , j ] != null )
					{
						Boxel B = Region . Selected_Region . B [ i , j ];
						B.BreadCrumbByteLabel[0]=0b0;

					}

				}
			}

		}

		private void BorderIceCols ( Window mw , int ChoakerLowRow , int ChoakerHighRow , int i , int j )
		{
			if ( Region . Selected_Region . BorderB == null )
			{
				Region . Selected_Region . BorderB = new Boxel [ Sheety . c1 + 2 , ChoakerHighRow + 2 ];
			}
			var I_len = Region . Selected_Region . BorderB . GetLength ( 0 );
			var J_len = Region . Selected_Region . BorderB . GetLength ( 1 );
			if ( i >= I_len )
				return;
			if ( j >= J_len )
				return;
			Boxel BorderB = Region . Selected_Region . BorderB [ i , j ];
			if ( BorderB == null )
			{
				BorderB = new Boxel ( i , j , Sheety . cells )
				{
					MW = ( DMT01 . MainWindow ) mw ,
					ParentRegion = Region . Selected_Region
				};
				Region . Selected_Region . BorderB [ i , j ] = BorderB;
			}
			else
			{
				BorderB = Region . Selected_Region . BorderB [ i , j ];
			}

			BorderB . DrawMe ( );
		}

		private void BorderIceRows ( Window mw , int ChoakerLowRow , int ChoakerHighRow , int i , int j )
		{
			if ( Region . Selected_Region . BorderB == null )
			{
				Region . Selected_Region . BorderB = new Boxel [ Sheety . c1 + 2 , ChoakerHighRow + 2 ];
			}
			var I_len = Region . Selected_Region . BorderB.GetLength(0);
			var J_len= Region . Selected_Region . BorderB.GetLength(1);
			if(i>= I_len )
				return;
			if(j>= J_len )
				return;

			Boxel BorderB = Region . Selected_Region . BorderB [ i , j ];
			if ( BorderB == null )
			{
				BorderB = new Boxel ( i , j , Sheety . cells )
				{
					MW = ( DMT01 . MainWindow ) mw ,
					ParentRegion = Region . Selected_Region
				};
				Region . Selected_Region . BorderB [ i , j ] = BorderB;
			}
			else
			{
				BorderB = Region . Selected_Region . BorderB [ i , j ];
			}

			BorderB . DrawMe ( );
		}

		private static void OldDrawWithoutBoxel ( SharpGL . OpenGL gl , int j , int i )
			{
			if ( Sheety . cells [ i , j ] > 1.0f )
				{
				float overflow = Sheety . cells [ i , j ];
				Sheety . cells [ i , j ] = 1.0f;
				}
			int [ , ] c = new int [ 4 , 2 ] {
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
			float [ ] C = LocalMaths . LocalMathsClass . GetCentroid ( c );
			float S = LocalMaths . LocalMathsClass . GetMean ( s );

			for ( int k = 0 ; k < 4 ; k++ )
				{
				int m = k % 4;
				int n = ( k + 1 ) % 4;
				int o = ( k + 2 ) % 4;
				gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
				gl . Color ( s [ m ] , s [ m ] , s [ m ] );
				gl . Vertex ( c [ m , 0 ] , c [ m , 1 ] , s [ m ] );

				gl . Color ( S , S , S );
				gl . Vertex ( C [ 0 ] , C [ 1 ] , S );

				gl . Color ( s [ n ] , s [ n ] , s [ n ] , s [ n ] );
				gl . Vertex ( c [ n , 0 ] , c [ n , 1 ] , s [ n ] );
				gl . End ( );
				}
			}

		private void ReoGrid3DSpreadsheet ( SharpGL.OpenGL gl )
			{
			const float cell_height = 0.499f;
			const float cell_width = 0.5f;
			const int MaxDisplayCols = 20;
			const int MaxDisplayRows = 30;
			const int normal_col_width = 70;
			const int normal_row_height = 20;
			Worksheet CW = this.myReoGridControl . CurrentWorksheet;

			if ( this . DoDrawSpreadsheetSideBorder_CheckBox_Control . IsChecked . Value )
				{
				const float x0 = 0f;
				const float x1 = -1f;

				float y0 = 0.0f;
				float y1 = 0f;

				for ( int i = 0 ; i < MaxDisplayRows ; i++ )
					{
					float row_height = CW . GetRowHeight ( i );
					float norm_units = cell_width * row_height / normal_row_height;

					y0 = y1;
					y1 -= norm_units;

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

					float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

					gl . Translate ( c [ 0 ] - .25 , c [ 1 ] - .15 , c [ 2 ] );
					gl . Scale ( 0.6 , 0.6 , 1.0 );
					String iter_string = ( i + 1 ) . ToString ( "00" );
					gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: iter_string );
					gl . PopMatrix ( );
					}
				}

			if ( this . DoDrawSpreadsheetTopBorder_CheckBox_Control . IsChecked . Value )
				{
				float x0 = 0f;
				float x1 = 0f;
				const float y0 = 0f;
				const float y1 = cell_height;
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

					float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

					gl . Translate ( c [ 0 ] - 0.25f , c [ 1 ] - .25 , c [ 2 ] );
					gl . Scale ( 0.7 , 0.7 , 1.0 );
					gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: LocalMaths . LocalMathsClass . IntToLetter ( i ) );
					gl . PopMatrix ( );
					}
				}

			if ( this . DoDrawSpreadsheetFocusCell_s_CheckBox_Control . IsChecked . Value )
				{
				CellPosition FP = this.myReoGridControl . CurrentWorksheet . FocusPos;
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

			if ( this . DoDrawSpreadsheetSelectedCell_s_CheckBox_Control . IsChecked . Value )
				{
				RangePosition SR = this.myReoGridControl . CurrentWorksheet . SelectionRange;
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

			if ( this . DoDrawSpreadsheetGrid_CheckBox_Control . IsChecked . Value )
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
					x1 += width_factor;

					//Debug . WriteLine ( String . Format ( " GetColumnWidth {0}" , w ) );

					float y0 = 0;
					float y1 = 0;

					for ( int j = 0 ; j < CW . RowCount ; j++ )
						{
						int r = CW . GetRowHeight ( j );
						float height_factor = ( float ) r / ( float ) normal_row_height;

						y0 = y1;
						y1 += height_factor;

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

			if ( this . DoDrawAllSpreadsheetData_CheckBox_Control . IsChecked . Value )
				{
				gl . PushMatrix ( );
				gl . PushAttrib ( SharpGL . OpenGL . GL_CURRENT_BIT |
									SharpGL . OpenGL . GL_ENABLE_BIT |
									SharpGL . OpenGL . GL_LINE_BIT |
									SharpGL . OpenGL . GL_DEPTH_BUFFER_BIT );

				//gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
				//gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );

				gl . LineWidth ( 1 );
				if ( this . Spreadsheet_Aspect_Scale_Hack_checkBox_Control . IsChecked . Value )
					{
					gl . Scale ( x: .25 , y: 1 , z: 1 );
					}

				GetActualizedRange ( CW , out int maxRow , out int maxCol );

				float x1 = 0;
				float x0 = 0;
				for ( int i = 0 ; i < maxCol ; i++ )
					{
					int w = CW . GetColumnWidth ( i );
					float width_factor = ( float ) w / ( float ) normal_col_width;
					x0 = x1;
					x1 += width_factor;

					float y0 = 0;
					float y1 = 0;

					for ( int j = 0 ; j < maxRow ; j++ )
						{
						int r = CW . GetRowHeight ( j );
						float height_factor = ( float ) r / ( float ) normal_row_height;

						y0 = y1;
						y1 -= height_factor;

						Cell D = CW . Cells [ j , i ];
						if ( D . DataFormat == unvell . ReoGrid . DataFormat . CellDataFormatFlag . Text )
							{
							String Text = D . DisplayText;

							gl . PushMatrix ( );

							float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );
							int len = Text . Length;
							if ( len < 10 )
								{
								gl . Translate ( x: x0 , y: c [ 1 ] - .25 , z: c [ 2 ] );
								gl . Scale ( 0.5 , 0.7 , 1.0 );
								}
							else
								{
								gl . Translate ( x: x0 , y: c [ 1 ] - .25 , z: c [ 2 ] );
								float big_x_scaling = 4.0f / ( float ) len;
								gl . Scale ( big_x_scaling , 0.7 , 1.0 );
								}
							gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: Text );
							gl . PopMatrix ( );
							}
						else
						if ( D . DataFormat == unvell . ReoGrid . DataFormat . CellDataFormatFlag . Number )
							{
							object CellData = D . Data;
							if ( !float .TryParse ( s: D .DisplayText , result: out float number ) )
							{
								Debug .WriteLine ( String .Format ( "TryParse failed on {0}" , D .DisplayText ) );
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
							gl . Translate ( x: x0 + this . Spreadsheet3DNumericLeftMargin_H_Slider_User_Control . SliderValue , y: c [ 1 ] - .25 , z: c [ 2 ] - 1.0 );
							String Text = number . ToString ( "#,###,##0.##" );
							float len = Text . Length;
							float big_x_scaling = this.Spreadsheet3DNumericWidthFittingFactor_H_Slider_User_Control . SliderValue / len;
							gl . Scale ( x: big_x_scaling , y: 0.7 , z: 1.0 );

							gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 1.0f , extrusion: 0.9f , text: Text );

							gl . PopMatrix ( );
							gl . Enable ( SharpGL . OpenGL . GL_LIGHTING );
							gl . Enable ( SharpGL . OpenGL . GL_TEXTURE_2D );
							}
						else
						if ( D . DataFormat == CellDataFormatFlag . Percent )
							{
							object Nuber = D . Data;
							float .TryParse ( s: D .DisplayText , result: out float number );
							gl . PushMatrix ( );

							float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

							gl . Translate ( x: x0 + 0.01f , y: c [ 1 ] - .25 , z: c [ 2 ] );
							const float big_x_scaling = 2.0f / ( float ) 10;
							gl . Scale ( big_x_scaling , 0.7 , 1.0 );

							gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: number . ToString ( "00.0" ) );
							gl . PopMatrix ( );
							}
						else
						if ( D . DataFormat == CellDataFormatFlag . Currency )
							{
							object Nuber = D . Data;
							float .TryParse ( s: D .DisplayText , style: System .Globalization .NumberStyles .Currency , provider: new CultureInfo ( "en-US" ) , result: out float number );
							gl . PushMatrix ( );

							float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

							gl . Translate ( x: x0 + 0.01f , y: c [ 1 ] - .25 , z: c [ 2 ] );
							const float big_x_scaling = 2.0f / ( float ) 10;
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

			const int GL_Width1 = 605;
			const int GL_Width0 = 1060;

			Boolean IsReoGridControlVisible = this.myReoGridControl . IsVisible;

			if ( IsReoGridControlVisible )
				{
				this . myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnSpanProperty , 1 );
				this . myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnProperty , 1 );
				this . myOpenGLControl . Width = GL_Width1;
				}
			else
				{
				this . myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnSpanProperty , 2 );
				this . myOpenGLControl . SetValue ( System . Windows . Controls . Grid . ColumnProperty , 0 );
				this . myOpenGLControl . Width = GL_Width0;
				}

			float width = ( float ) this.myOpenGLControl . Width;
			float height = ( float ) this.myOpenGLControl . Height;

			myOpenGLControlViewportAspect = width / height;

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}

		private void myOpenGLControl_OpenGLInitialized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
			{
			Debug . WriteLine ( String . Format ( "{0}" , nameof ( myOpenGLControl_OpenGLInitialized ) ) );

			//  Get the OpenGL object.
			OpenGL gl = this.myOpenGLControl . OpenGL;

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

			// gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Back );
			gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );

			DoAspect ( );
			}

		private void spreadsheet_load_Button_Click ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Spreadsheet Load" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			TextRange tr1 = new TextRange ( this.SpreadsheetDirPath_RichTextBox . Document . ContentStart , this.SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
			TextRange tr2 = new TextRange ( this.SpreadsheetFileName_RichTextBox . Document . ContentStart , this.SpreadsheetFileName_RichTextBox . Document . ContentEnd );
			String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );

			String FileName = String . Format ( "{0}{1}" , scratchy , ".xll" );
			byte [ ] a = DMT01 . Properties . Resources . UNEP_NATDIS_disasters_2002_2010;
			System . IO . File . WriteAllBytes ( FileName , a );

			this . myReoGridControl . Load ( FileName , unvell . ReoGrid . IO . FileFormat . Excel2007 );

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed Spreadsheet Load" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

		private void myOpenGLControl_Resized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
			{
			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "resized" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			OpenGL gl = this.myOpenGLControl . OpenGL;

			DoAspect ( );

			Resizes++;
			}

		private void Save0_Button_Click ( object sender , RoutedEventArgs e )
			{
			XMLSeralizeDeSeralizeControlState.WalkLogicalTree ( this.DMT_Main_Window_Control );
			}

		private void DMT_Main_Window_Control_LocationChanged ( object sender , EventArgs e )
			{
			if ( !( sender is Window W ) )
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
			XMLSeralizeDeSeralizeControlState.LoadEm(this.DMT_Main_Window_Control);
			}

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
			this . LookAt_TabItem . IsSelected = true;
			}

		private void UsePerspetiveViewingTransform_Click ( object sender , RoutedEventArgs e )
			{
			this . Perspective_TabItem . IsSelected = true;
			}

		private void UseOrthographic_Viewing_Transform_radioButton_Control_Click ( object sender , RoutedEventArgs e )
			{
			this . Orthographics_Viewing_TabItem . IsSelected = true;
			}

		private void Use_Viewing_Frustrum_RadioButton_Click ( object sender , RoutedEventArgs e )
			{
			this . Viewing_Frustrum_TabItem . IsSelected = true;
			}

		private void Do_Orbit_Pull_Back_CheckBox_Control_Click ( object sender , RoutedEventArgs e )
			{
			this . Eye_and_Camera_TabItem . IsSelected = true;
			}

		private void SelectDataRangeForNormalization_DoNormalizationOnScratchSheet_Click ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Normalization" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			this . myReoGridControl . CurrentWorksheet = this . myReoGridControl . Worksheets [ 0 ];
			Worksheet CW = this.myReoGridControl . CurrentWorksheet;
			if ( CW . Name == scratchy )
				{
				return;
				}

			RangePosition IsThisActualiedRange = CW . UsedRange;
			int xx = CW . MaxContentCol;
			int yy = CW . MaxContentRow;
			GetActualizedRange ( CW: CW , maxRow: out int R , maxCol: out int C );

			unvell . ReoGrid . RangePosition DataRange = new unvell . ReoGrid . RangePosition ( row: 1 , col: 1 , rows: R , cols: C );

			unvell . ReoGrid . Worksheet Scratcheroo = this.myReoGridControl . Worksheets [ scratchy ];
			if ( Scratcheroo == null )
				{
				Scratcheroo = this . myReoGridControl . NewWorksheet ( name: scratchy );
				}

			this . myReoGridControl . CurrentWorksheet = Scratcheroo;
			for ( int j = 0 ; j <= R ; j++ )
				{
				Scratcheroo . SetCellData ( row: j , col: 0 , data: "XX" );
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
				Scratcheroo . SetCellData ( row: DataRange . EndPos . Row + 1 , col: j + 1 , data: Norm );
				for ( int i = DataRange . StartPos . Row ; i <= DataRange . EndPos . Row ; i++ )
					{
					double Val = CW . GetCellData<double> ( row: i , col: j );
					double Normalized = Val * Norm;

					Scratcheroo . SetCellData ( row: i , col: j , data: Normalized );
					}
				}
			unvell . ReoGrid . NamedRange NamedDataRange =
				new unvell . ReoGrid . NamedRange ( worksheet: Scratcheroo , name: scratchy , range: DataRange );
			bool exists = Scratcheroo . NamedRanges . Contains ( range: NamedDataRange );
			if ( !exists )
				{
				Scratcheroo . NamedRanges . Add ( NamedDataRange );
				}
			this . DoSaveSelectedData_Button . PerformClick ( );

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

		private void InstallHandRowPivotButtonsInScratchSpreadsheet_CheckBox_Control_Checked ( object sender , RoutedEventArgs e )
			{
			unvell . ReoGrid . Worksheet Scratcheroo = this.myReoGridControl . Worksheets [ scratchy ];
			if ( Scratcheroo == null )
				{
				Scratcheroo = this . myReoGridControl . NewWorksheet ( name: scratchy );
				}

			this . myReoGridControl . CurrentWorksheet = Scratcheroo;
			GetActualizedRange ( CW: Scratcheroo , maxRow: out int R , maxCol: out int C );
			for ( int j = 0 ; j <= R ; j++ )
				{
				MyCheckBox CBC = new MyCheckBox ( );
				CBC . Click += this . CBC_Click;
				Scratcheroo . SetCellData ( row: j , col: 0 , data: CBC );
				}
			for ( int i = 0 ; i <= C ; i++ )
				{
				CheckBoxCell CBC = new unvell . ReoGrid . CellTypes . CheckBoxCell ( );
				CBC . Click += this . CBC_Click;
				CBC . CheckChanged += this . CBC_CheckChanged;
				}
			}

		private void CBC_CheckChanged ( object sender , EventArgs e )
			{
			Debug . WriteLine ( String . Format ( "{0}" , nameof ( CBC_CheckChanged ) ) );
			}

		private void CBC_Click ( object sender , EventArgs e )
			{
			Debug . WriteLine ( String . Format ( "{0}" , nameof ( CBC_CheckChanged ) ) );
			CheckBoxCell CBC = sender as CheckBoxCell;
			Pivot.SwapRow ( CBC );
			}

		private void Do_Iterate_Resoures_Button_Click ( object sender , RoutedEventArgs e )
			{
			Debug . WriteLine ( String . Format ( "{0}: Manifest resources" , nameof ( Do_Iterate_Resoures_Button_Click ) ) );

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

			if ( value is Stream stream )
				{
				return "Stream: " + GetHead ( stream );
				}

			return value . ToString ( );
			}

		private static string GetHead ( Stream stream )
			{
			using ( StreamReader reader = new StreamReader ( stream ) )
				{
				char [ ] buffer = new char [ 40 ];
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
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
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
			System . Windows . Point p = Mouse . GetPosition ( this.myOpenGLControl );

			viewport_height = ( int ) this . myOpenGLControl . ActualHeight;
			viewport_width = ( int ) this . myOpenGLControl . ActualWidth;

			mouse_x = ( int ) ( p . X + 0.5d );
			mouse_y = ( int ) ( p . Y + 0.5d );
			mouse_corrected_y = ( int ) ( viewport_height - mouse_y );
			viewport_cursor_x = mouse_x;
			viewport_cursor_y = mouse_corrected_y;

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "[{0},{1}]", mouse_x, mouse_y ) );
			}

		private void DoDrawReoGridSpreadsheet_CheckBox_Control_Click ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			CheckBox CB = sender as CheckBox;

			if ( CB . IsChecked . Value )
				{
				this . myReoGridControl . Visibility = Visibility . Visible;
				}
			else
				{
				this . myReoGridControl . Visibility = Visibility . Collapsed;
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
			string b = this.myReoGridControl . CurrentWorksheet . Name;
			int a = this.myReoGridControl . GetWorksheetIndex ( b );
			CurrentWorksheetIndex = a;
			return a;
			}

		private void Aspect_Label_MouseLeave ( object sender , MouseEventArgs e )
			{
			Label L = sender as Label;
			DoAspect ( );
			L . Content = MainWindow . myOpenGLControlViewportAspect . ToString ( "#0.0#" );
			}

		private void SolutionDirectorySaveRestoreSupport_TabItem_Loaded ( object sender , RoutedEventArgs e )
		{
			Array b= Enum . GetValues ( typeof ( KnownFolderType ) );
			foreach ( KnownFolderType t in b )
			{
				KnownFolder knownFolder = new KnownFolder ( t );

				Debug . WriteLine ( knownFolder . Type . ToString ( ) );
				try
				{
					Console . Write ( "Current Path: " );
					Console . WriteLine ( knownFolder . Path );
					Console . Write ( "Default Path: " );
					Console . WriteLine ( knownFolder . DefaultPath );

				}
				catch ( Exception E )
				{
					System . Diagnostics . Debug . WriteLine ( String . Format ( "exception {0} " , 
						E ));
				}
			}
		}

		private void DoSaveSelectedData_Button_Click ( object sender , RoutedEventArgs e )
			{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Save Selected" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			Worksheet Scratcheroo = this.myReoGridControl . GetWorksheetByName ( scratchy );
			if ( Scratcheroo == null )
				{
				return;
				}

			this . myReoGridControl . CurrentWorksheet = Scratcheroo;
			Worksheet . ReoGridRangeCollection R = Scratcheroo . Ranges;
			NamedRange named_ranges = Scratcheroo . NamedRanges [ scratchy ];

			Sheety = new NWorksheety
			{
				r0 = named_ranges .StartPos .Row ,
				c0 = named_ranges .StartPos .Col
			};
			Sheety . r1 = named_ranges . Rows + Sheety . r0;
			Sheety . c1 = named_ranges . Cols + Sheety . c0;
			Sheety . cells = new float [ Sheety . c1 + 2 , Sheety . r1 + 2 ];

			System . Diagnostics . Debug . WriteLine ( String . Format ( "named ranged count {0} {1} " , named_ranges , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

			for ( int j = named_ranges . StartPos . Col ; j < named_ranges . EndPos . Col ; j++ )
				{
				for ( int i = named_ranges . StartPos . Row ; i < named_ranges . EndPos . Row ; i++ )
					{
					Sheety . cells [ j , i ] = named_ranges . Cells [ row: i , col: j ] . GetData<float> ( );
					}
				}
			this . DoDrawReoGridSpreadsheet_CheckBox_Control . PerformClick ( );
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
		}
	}

