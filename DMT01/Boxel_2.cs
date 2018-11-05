using System;
using System . Linq;
using System . Collections . Generic;
using SharpGL;
using GlmSharp;
using WpfControlLibrary1;
using GlmNet;
using System . IO;
using System . Windows . Controls;
using System . Diagnostics;
using Microsoft . Windows . Design . Features;
using System . Net . Security;

namespace DMT01
{
	public enum BoxelKindEnum
		{
							 Unitalized,
							 FlatNoHit,
							 Weird1Hit,
							 Weird3Hit,
							 IsCritical,
							 IsNotCriticalTwoHits,
							 IsBorderBoxel,
							 IsCritical4Hit,
							 CriticalValueTitrated,
		}
	public class Boxel
	{
		public BoxelKindEnum BoxelKindEnum;
		public Boolean IsCritical;
		public Boolean IsBorderBoxel;
		public int EdgeHits;
		public Byte[] BreadCrumbByteLabel=new Byte[8];
		public Vertex Centroid = new Vertex ( );
		public int Titration_Steps;
		public float Penult_High_V;
		public float Penult_Low_V;
		public float Delta_V;
		public float Delta_Crit_V=.001f;
		public float Critical_Titrate_V;
		public float Critical_Mean_V;
		public short LargestVertexIndex;
		public short SmallestVertexIndex;
		public float LargestVertex_V;
		public float SmallestVertex_V;
		public Vertex[] Penult_High;
		public Vertex[] Penult_Low;
		public short[]  SortedVertexIndicies={ 0 , 1 , 2 , 3 };
		public int I;
		public int J;
		public DMT01.MainWindow MW;
		public DMT01.Region ParentRegion;
		public Edge [ ] E = new Edge [ 4 ];
		public IsoEdge [ ] IsoEdge = new IsoEdge [ 4 ];
		public short IsoEdges ;
		public Vertex [ ] V = new Vertex [ 4 ];

		public short Span=1;
		const short LineWidth = ( short )1;
		static readonly float [ ] LineColor = { .9f , .9f , .9f };
		const short PointSize = ( short ) 1;
		static readonly float [ ] PointColor = { 1 , 1 , 1 };
		static readonly float [ ] TextColor = { .9f , .9f , .9f };

		public Boxel ( int i , int j , float [ , ] c )
		{
			this . I = i;
			this . J = j;
			this . V [ 0 ] = new Vertex ( 0 , i					, j					, c );
			this . V [ 1 ] = new Vertex ( 1 , i					, j + this . Span   , c );
			this . V [ 2 ] = new Vertex ( 2 , i + this . Span	, j + this . Span   , c );
			this . V [ 3 ] = new Vertex ( 3 , i + this . Span	, j					, c );

			this . V [ 0 ] . Next = this . V [ 1 ];
			this . V [ 1 ] . Next = this . V [ 2 ];
			this . V [ 2 ] . Next = this . V [ 3 ];
			this . V [ 3 ] . Next = this . V [ 0 ];

			this . V [ 0 ] . Previous = this . V [ 3 ];
			this . V [ 1 ] . Previous = this . V [ 0 ];
			this . V [ 2 ] . Previous = this . V [ 1 ];
			this . V [ 3 ] . Previous = this . V [ 2 ];

			this . Centroid = LoadCentroid ( );

			this . E [ 0 ] = new Edge ( 0 , this . V [ 0 ] , this . V [ 1 ], this );
			this . E [ 1 ] = new Edge ( 1 , this . V [ 1 ] , this . V [ 2 ] , this );
			this . E [ 2 ] = new Edge ( 2 , this . V [ 2 ] , this . V [ 3 ] , this );
			this . E [ 3 ] = new Edge ( 3 , this . V [ 3 ] , this . V [ 0 ] , this );

			this . E [ 0 ] . NextPeer = this . E [ 1 ];
			this . E [ 1 ] . NextPeer = this . E [ 2 ];
			this . E [ 2 ] . NextPeer = this . E [ 3 ];
			this . E [ 3 ] . NextPeer = this . E [ 0 ];

			this . E [ 0 ] . PreviousEdge = this . E [ 3 ];
			this . E [ 1 ] . PreviousEdge = this . E [ 0 ];
			this . E [ 2 ] . PreviousEdge = this . E [ 1 ];
			this . E [ 3 ] . PreviousEdge = this . E [ 2 ];

			this . V [ 0 ] . NextEdge = this . E [ 0 ];
			this . V [ 1 ] . NextEdge = this . E [ 1 ];
			this . V [ 2 ] . NextEdge = this . E [ 2 ];
			this . V [ 3 ] . NextEdge = this . E [ 3 ];

			this . V [ 0 ] . PreviousEdge = this . E [ 3 ];
			this . V [ 1 ] . PreviousEdge = this . E [ 2 ];
			this . V [ 2 ] . PreviousEdge = this . E [ 1 ];
			this . V [ 3 ] . PreviousEdge = this . E [ 0 ];

			this . IsThisCritical ( );

			this . DoMinMaxDelta ( );

			this . SortVerticies ( );

			if ( !this . IsCritical )
			{
				this . LoadTweenVerts ( );
			}

			if ( !this . IsCritical )
			{
				this . LoadSubEdges ( );
			}

			//if ( this . IsCritical )
			//{
			//	DoCriticalIsoEdges ( );
			//}

			// this. LoadTriangles ( );
			this . Penult_High = new Vertex[]{ this . V [ this . SortedVertexIndicies [ 2 ] ]};
			this . Penult_Low = new Vertex[]{this . V [ this . SortedVertexIndicies [ 1 ] ]};
			this . Penult_High_V = this .Penult_High[0] . V;
			this . Penult_Low_V = this . Penult_Low[0] . V;
			float SanityCheck = this . Penult_High_V - this . Penult_Low_V;
			if ( SanityCheck < 0 )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "SanityCheck:Insane!!!" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}
			this.Critical_Mean_V=(this.Penult_High_V+this.Penult_Low_V)/2f;

		}

		private float [ ] add ( float [ ] a , float [ ] b )
		{
			float [ ] c ={
				a [0]+b[0],
				a [1]+b[1],
				a [2]+b[2]
				};
			return c;
		}

		private dvec3 add ( dvec3 r , float [ ] f )
		{
			dvec3 a = new dvec3 ( x: r [ 0 ] + f [ 0 ] , y: r [ 1 ] + f [ 1 ] , z: r [ 2 ] + f [ 2 ] );
			return a;
		}

		private float [ ] Add ( float [ ] cf0 , float [ ] cf1 )
		{
			float [ ] cf = {cf0 [0]+cf1[0],
							cf0 [1]+cf1[1],
							cf0 [2]+cf1[2],	 };

			return cf;
		}

		private void AnnotateBoxelAtCentroid ( )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}

			if ( this . MW . HackCheckBox_C10_R4_CheckBox_Control . IsChecked . Value )
			{
				const float ts = 0.3f;
				const string f0 = "Arial";
				String txt0 = String . Format ( "[{0},{1}]" , this . I , this . J );

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( .9 , .9 , .9 );
				gl . PushMatrix ( );
				gl . Translate ( x: this . Centroid . cf [ 0 ] , y: this . Centroid . cf [ 1 ] , z: this . Centroid . cf [ 2 ] );
				gl . Translate ( x: -.4 , y: 0 , z: 0 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt0 );
				gl . PopMatrix ( );

				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C9_R4_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( .9 , .2 , .2 );
				gl . PointSize ( 7 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( this . Centroid . cf );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C8_R4_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( 1 , .2 , .9 );
				gl . LineWidth ( 1 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( this . V [ 0 ] . cf );
				gl . Vertex ( this . Centroid . cf );
				gl . Vertex ( this . V [ 1 ] . cf );
				gl . Vertex ( this . Centroid . cf );
				gl . Vertex ( this . V [ 2 ] . cf );
				gl . Vertex ( this . Centroid . cf );
				gl . Vertex ( this . V [ 3 ] . cf );
				gl . Vertex ( this . Centroid . cf );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C1_R4_CheckBox_Control . IsChecked . Value )
			{
				float knobby = this . MW . CentroidToCornerLengthFraction_H_Slider_UserControl . SliderValue;
				if ( knobby < 0 )
				{
					return;
				}

				if ( knobby > 1 )
				{
					return;
				}

				LineStinkerModes Stankey = this . MW . lineStinkerModeenumcomboBoxUserControl . Mode;

				float [ ] [ ] [ ] Stinkers = new float [ 4 ] [ ] [ ];
				if ( Stinkers == null )
				{
					return;
				}

				Stinkers [ 0 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 0 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 0 ] == null )
				{
					return;
				}

				Stinkers [ 1 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 1 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 1 ] == null )
				{
					return;
				}

				Stinkers [ 2 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 2 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 2 ] == null )
				{
					return;
				}

				Stinkers [ 3 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 3 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 3 ] == null )
				{
					return;
				}

				if ( this . MW . HackCheckBox_C2_R2_CheckBox_Control . IsChecked . Value )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . Color ( 1 , .2 , .9 );
					gl . LineWidth ( 1 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );

					gl . Vertex ( x: Stinkers [ 0 ] [ 0 ] [ 0 ] , y: Stinkers [ 0 ] [ 0 ] [ 1 ] , z: Stinkers [ 0 ] [ 0 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 0 ] [ 1 ] [ 0 ] , y: Stinkers [ 0 ] [ 1 ] [ 1 ] , z: Stinkers [ 0 ] [ 1 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 1 ] [ 0 ] [ 0 ] , y: Stinkers [ 1 ] [ 0 ] [ 1 ] , z: Stinkers [ 1 ] [ 0 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 1 ] [ 1 ] [ 0 ] , y: Stinkers [ 1 ] [ 1 ] [ 1 ] , z: Stinkers [ 1 ] [ 1 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 2 ] [ 0 ] [ 0 ] , y: Stinkers [ 2 ] [ 0 ] [ 1 ] , z: Stinkers [ 2 ] [ 0 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 2 ] [ 1 ] [ 0 ] , y: Stinkers [ 2 ] [ 1 ] [ 1 ] , z: Stinkers [ 2 ] [ 1 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 3 ] [ 0 ] [ 0 ] , y: Stinkers [ 3 ] [ 0 ] [ 1 ] , z: Stinkers [ 3 ] [ 0 ] [ 2 ] );
					gl . Vertex ( x: Stinkers [ 3 ] [ 1 ] [ 0 ] , y: Stinkers [ 3 ] [ 1 ] [ 1 ] , z: Stinkers [ 3 ] [ 1 ] [ 2 ] );

					gl . End ( );
					gl . PopAttrib ( );
				}
			}
		}

		private String Comparis ( float v , Edge e )
		{
			float v0 = e . V [ 0 ] . V;
			float v1 = e . V [ 1 ] . V;

			return Comparis ( v0 , v , v1 );
		}

		private string Comparis ( float v0 , float v , float v1 )
		{
			const String Stranger = "0.00";
			String strang = String . Format ( "{0}{1}{2}{3}{4}" ,
				v0 . ToString ( Stranger ) , Comparis ( v0 , v ) , v . ToString ( Stranger ) , Comparis ( v , v1 ) , v1 . ToString ( Stranger ) );
			return strang;
		}

		//private void DoCriticalIsoEdges ( )
		//	{

		//	}

		//private bool IsBetween ( Edge e1 , Edge e2 )
		//	{
		//	throw new NotImplementedException ( );
		//	}

		//private void InsertVertexOnEdge ( float v , Edge e )
		//	{
		//	if ( !IsInBetween ( v , e ) )
		//		{
		//		System . Diagnostics . Debug . WriteLine ( String . Format ( "xx{0} E{1} {2} {3} " ,
		//			this . DisplayMe ( ) , e . EdgeIndex , Comparis ( v , e ) , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );

		//		//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " ,  Centroid.V.ToString("0.000") , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		//		return;
		//		}

		//	System . Diagnostics . Debug . WriteLine ( String . Format ( "++{0} E{1} {2} {3} " ,
		//		this . DisplayMe ( ) , e . EdgeIndex , Comparis ( v , e ) , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
		//	}

		private String DisplayMe ( )
		{
			const String Stranger = "0.00";
			String strang = String . Format ( "[i{0},j{1}] {2}{3}{4} {5}{6}{7}" , this . I , this . J ,
				this . SmallestVertex_V . ToString ( Stranger ) , Comparis ( this . SmallestVertex_V , this . LargestVertex_V ) , this . LargestVertex_V . ToString ( Stranger ),
				this . Penult_Low_V . ToString ( Stranger ) , Comparis ( this . Penult_Low_V , this . Penult_High_V ) , this . Penult_High_V . ToString ( Stranger ) );
			return strang;
		}

		private String DisplayMe ( Edge e )
		{
			String fomont = String . Format ( "{0} {1}" , this . DisplayMe ( ) , e . DisplayMe ( ) );
			return fomont;
		}

		private float [ ] divide ( float [ ] c , int v )
		{
			float V = v;
			float [ ] a = { c [ 0 ] / V , c [ 1 ] / V , c [ 2 ] / V };
			return a;
		}

		private float [ ] Divide ( float [ ] a , float m )
		{
			float [ ] d = { a [ 0 ] / m , a [ 1 ] / m , a [ 2 ] / m };
			return d;
		}

		private dvec3 Divide ( double mag , dvec3 v )
		{
			dvec3 a = new dvec3 ( v . x / mag , v . y / mag , v . z / mag );
			return a;
		}

		private void DoMinMaxDelta ( )
		{
			this . LargestVertex_V = -float . MaxValue;
			this . SmallestVertex_V = float . MaxValue;

			for ( int i = 0 ; i < 4 ; i++ )
			{
				Vertex V = this . V [ i ];
				float v = V . V;

				if ( this . LargestVertex_V < v )
				{
					this . LargestVertex_V = v;
				}

				if ( v < this . SmallestVertex_V )
				{
					this . SmallestVertex_V = v;
				}
			}
			this . Delta_V = this . LargestVertex_V - this . SmallestVertex_V;

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} {3} " , DisplayMe(), this.min_V, Comparis(this.min_V, this.max_V) , this.max_V, ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void DrawBoxelEdgeCycles ( )
		{
			if ( this . MW == null )
			{
				return;
			}

			if ( this . MW . EdgeFactorHack0CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 0 ];
				DebugDrawEdge ( E );
			}

			if ( this . MW . EdgeFactorHack1CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 1 ];
				DebugDrawEdge ( E );
			}

			if ( this . MW . EdgeFactorHack2CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 2 ];
				DebugDrawEdge ( E );
			}

			if ( this . MW . EdgeFactorHack3CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 3 ];
				DebugDrawEdge ( E );
			}
		}
		private void DrawCriticalBoxel ( )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}

			if ( !IsThisCritical ( ) )
			{
				return;
			}

			Edge E0 = this . E [ 0 ];
			Edge E1 = this . E [ 1 ];
			Edge E2 = this . E [ 2 ];
			Edge E3 = this . E [ 3 ];

			float v = this . MW . region_threshold_H_Slider_UserControl . SliderValue;

			float x0 = E0 . V [ 0 ] . cf [ 0 ];
			float y0 = E0 . V [ 0 ] . cf [ 1 ];
			float z0 = E0 . V [ 0 ] . cf [ 2 ];
			float v0 = E0 . V [ 0 ] . V;

			float x1 = E1 . V [ 0 ] . cf [ 0 ];
			float y1 = E1 . V [ 0 ] . cf [ 1 ];
			float z1 = E1 . V [ 0 ] . cf [ 2 ];
			float v1 = E1 . V [ 0 ] . V;

			float x2 = E2 . V [ 0 ] . cf [ 0 ];
			float y2 = E2 . V [ 0 ] . cf [ 1 ];
			float z2 = E2 . V [ 0 ] . cf [ 2 ];
			float v2 = E2 . V [ 0 ] . V;

			float x3 = E3 . V [ 0 ] . cf [ 0 ];
			float y3 = E3 . V [ 0 ] . cf [ 1 ];
			float z3 = E3 . V [ 0 ] . cf [ 2 ];
			float v3 = E3 . V [ 0 ] . V;

			float dv0 = Math . Abs ( E0 . Delta_V );
			float dv1 = Math . Abs ( E1 . Delta_V );
			float dv2 = Math . Abs ( E2 . Delta_V );
			float dv3 = Math . Abs ( E3 . Delta_V );

			double qp = dv0 * dv1 * dv2 * dv3;
			double qm = ( dv0 + dv1 + dv2 + dv3 ) / 4.0;

			double ms = ( dv0 * dv0 ) + ( dv1 * dv1 ) + ( dv2 * dv2 ) + ( dv3 * dv3 );
			float V = ( float ) Math . Sqrt ( ms ) + this . V [ this . SmallestVertexIndex ] . V;
			V = ( float ) qm;

			if ( !IsInBetween ( this . V [ this . SmallestVertexIndex ] . V , V , this . V [ this . LargestVertexIndex ] . V ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			//float u0 = ( V - v0 ) / ( v1 - v0 );
			//float u1 = ( V - v1 ) / ( v2 - v1 );
			//float u2 = ( V - v2 ) / ( v3 - v2 );
			//float u3 = ( V - v3 ) / ( v0 - v3 );

			float u0 = ( V - v0 ) / ( v1 - v0 );
			float u1 = ( V - v1 ) / ( v2 - v1 );
			float u2 = ( V - v2 ) / ( v3 - v2 );
			float u3 = ( V - v3 ) / ( v0 - v3 );

			//float x00 = ( ( 1 - u0 ) * x0 ) + ( u0 * x1 );

			float x00 = ( float ) ( ( x0 + x1 ) / 2.0 );

			if ( !IsInBetween ( x0 , x00 , x1 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			float y00 = ( ( 1 - u0 ) * y0 ) + ( u0 * y1 );
			if ( !IsInBetween ( y0 , y00 , y1 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float z00 = ( ( 1 - u0 ) * z0 ) + ( u0 * z1 );
			if ( !IsInBetween ( z0 , z00 , z1 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			float x10 = ( ( 1 - u1 ) * x1 ) + ( u1 * x2 );
			if ( !IsInBetween ( x1 , x10 , x2 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float y10 = ( ( 1 - u1 ) * y1 ) + ( u1 * y2 );
			if ( !IsInBetween ( y1 , y10 , y2 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float z10 = ( ( 1 - u1 ) * z1 ) + ( u1 * z2 );
			if ( !IsInBetween ( z1 , z10 , z2 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			float x20 = ( ( 1 - u2 ) * x2 ) + ( u2 * x3 );
			if ( !IsInBetween ( x2 , x20 , x3 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float y20 = ( ( 1 - u2 ) * y2 ) + ( u2 * y3 );
			if ( !IsInBetween ( y2 , y20 , y3 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float z20 = ( ( 1 - u2 ) * z2 ) + ( u2 * z3 );
			if ( !IsInBetween ( z2 , z20 , z3 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			float x30 = ( ( 1 - u3 ) * x3 ) + ( u3 * x0 );
			if ( !IsInBetween ( x3 , x30 , x0 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float y30 = ( ( 1 - u3 ) * y3 ) + ( u3 * y0 );
			if ( !IsInBetween ( y3 , y30 , y0 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}
			float z30 = ( ( 1 - u3 ) * z3 ) + ( u3 * z0 );
			if ( !IsInBetween ( z3 , z30 , z0 ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "not IsInBetween" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
			}

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 3 );
			gl . PointSize ( 6 );
			gl . Color ( 1 , 1 , 1 );
			gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
			gl . Vertex ( x: x00 , y: y00 , z: z00 );
			gl . Vertex ( x: x10 , y: y10 , z: z10 );
			gl . Vertex ( x: x20 , y: y20 , z: z20 );
			gl . Vertex ( x: x30 , y: y30 , z: z30 );
			gl . End ( );
			gl . PopAttrib ( );
		}

		private void DrawEdge ( Edge E )
		{
			OpenGL gl = this . MW . myOpenGLControl . OpenGL;

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 1 );

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * this . MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * this . MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

			AnnotateEdge ( gl , E );

			gl . Color ( .95 , .99 , .80 );
			gl . LineWidth ( 2 );
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );

			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );

			if ( IsInBetween ( this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue, E ) )
			{
				//tex:
				// $$ x = ( 1 - u ) X_{0} + u X_{1} $$
				// $$ y = ( 1 - u ) Y_{0} + u Y_{1} $$
				// $$ z = ( 1 - u ) Z_{0} + u Z_{1} $$
				// $$ v = ( 1 - u ) V_{0} + u V_{1} $$
				//											where $$ 0 \leq u \leq 1 $$
				//  Solve for u from v:
				//		expand multiplicatives
				// $$ v=V_{0} - u V_{0} + u V_{1} $$
				//	   collect u addives
				// $$ v=V_{0} - u ( V_{0} + V_{1} ) $$

				//tex:
				//	   subtract $V_{0}$ from both sides to isolate $u$ on RHS
				// $$ v-V_{0} = - u ( V_{0} + V_{1} ) $$
				//		divide $( V_{0} + V_{1} )$ from both sides to further isolate $u$ on RHS
				// $$ ( v - V_{0} ) / ( V_{0} + V_{1} ) = -u $$
				//  flip signs (multiply both sides by -1)
				// $$ ( -v + V_{0} ) / ( V_{0} + V_{1} ) = u $$
				// flip sign again and flip sides to move  $u$ to LHS and we are done
				// $$ u = ( v - V_{0} ) / ( V_{0} + V_{1} ) $$
				// if v=V_{0}, u=0,
				float v = this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;

				float V0 = E . V [ 0 ] . V;
				float V1 = E . V [ 1 ] . V;

				float u = ( v - V0 ) / ( V0 + V1 );
				u = this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;
				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];

				float x = ( ( 1 - u ) * x0 ) + ( u * x1 );
				float y = ( ( 1 - u ) * y0 ) + ( u * y1 );
				float z = ( ( 1 - u ) * z0 ) + ( u * z1 );

				float [ ] c_th = {
							x,y,z
					};

				if ( IsInBetween ( c_th , E ) )          // this should be catching the  imtermediate points
				{
					System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
				}

				gl . PointSize ( 5 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( c_th [ 0 ] , c_th [ 1 ] , c_th [ 2 ] );
				gl . End ( );
			}

			gl . PopAttrib ( );
		}

		private void DrawEdge ( Edge E , Edge . EdgeDirection Direction )
		{
			OpenGL gl = this . MW . myOpenGLControl . OpenGL;

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 1 );

			switch ( Direction )
			{
				case Edge . EdgeDirection . ClockwiseNext:
					gl . Color ( 0.1 , 0.9 , 0.1 );
					break;

				case Edge . EdgeDirection . CounterClockwisePrevious:
					gl . Color ( 0.9 , 0.8 , 0.1 );
					break;
			}
			if ( E . IsSubEdge )
			{
				switch ( E . SubEdgeIndex )
				{
					case 0:
						gl . Color ( 0.5 , 0.8 , 0.16 );
						break;
					case 1:
						gl . Color ( 0.5 , 0.8 , 0.99 );
						break;
					case 2:
						gl . Color ( 0.7 , 0.8 , .87 );
						break;
					default:
						break;
				}
			}

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * this . MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * this . MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );

			//if ( IsBetween ( E , threshold_hack1 ) )
			//	{
			//	float V0 = E . V [ 0 ] . V;
			//	float [ ] cf = E . V [ 0 ] . cf;
			//	float [ ] c_th = new float [ ]		 // this is all wrong here
			//		{
			//			cf[0]+(threshold_hack1)*E.delta_cf[0],
			//			cf[1]+(threshold_hack1)*E.delta_cf[1],
			//			cf[2]+threshold_hack1
			//		};
			//	if ( IsInBetween ( cf , E ) )		   // this should be catching the  imtermediate points
			//	{
			//		System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			//	}
			//	gl . PointSize ( 5 );
			//	gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
			//	gl . Vertex ( c_th [ 0 ] , c_th [ 1 ] , c_th[2] );
			//	gl . End ( );
			//	}

			gl . PopAttrib ( );
		}

		void DrawContoursInBoxel ( )
		{
			if ( this . MW == null )
			{
				return;
			}
			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}
			short LineWidth = ( short ) MW . GlobalHitEdgeLineWidth_ComboBox_User_Control . Linewidth;
			short DrawHitEdgesLineWidth = 1;
			float [ ] LineColor = { .9f , .9f , .9f };
			float [ ] DrawHitEdgesLineColor = { .5f , .5f , .9f };
			int LineStippleFactor=1;
			//ushort LineStipplePattern = 0xAAAA;
			//ushort LineStipplePattern = 0x00FF;
			ushort LineStipplePattern = 0x000F;
			short PointSize = ( short ) MW . SweeperHitPointSize_PointSizeComboBoxUser_Control . Selection;
			short DrawHitsPointSize = 5;
			float [ ] PointColor = { 1 , 1 , 1 };
			float [ ] DrawHitsPointColor = { .7f,.7f,.7f };
			float [ ] TextColor = { .9f , .9f , .9f };
			float V= this.MW. CriticalitySweeper_THRESHOLD_H_Slider_User_Control . SliderValue;
			this.EdgeHits = 0;
			if ( HitMaster ( V , this . E [ 0 ] ) )
			{
				this.EdgeHits++;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				if ( this . MW . DoDrawHits_CheckBox_Control . IsChecked . Value )
				{
					gl . PointSize ( DrawHitsPointSize );
					gl . Color ( DrawHitsPointColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
					gl . Vertex ( this . E [ 0 ]. Hit  );
					gl . End ( );
				}
				if ( this . MW . DoDrawHitEdges_CheckBox_Control . IsChecked . Value )
				{
					gl . LineWidth ( DrawHitEdgesLineWidth );
					gl.LineStipple(factor:LineStippleFactor, pattern: LineStipplePattern)  ;
					gl . Color ( DrawHitEdgesLineColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 0 ] . V [ 0 ] . cf );
					gl . Vertex ( this . E [ 0 ] . V [ 1 ] . cf );
					gl . End ( );

				}
				if ( this . MW . Edge0Annotation_CheckBox_Control.IsChecked.Value )
				{
				AnnotateEdge(gl, this.E[0]);
				}
				gl . PopAttrib ( );
			}
			if ( HitMaster ( V , this . E [ 1 ] ) )
			{
				this.EdgeHits++;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				if ( this . MW . DoDrawHits_CheckBox_Control . IsChecked . Value )
				{
					gl . PointSize ( PointSize );
					gl . Color ( PointColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . End ( );
				}
				if ( this . MW . DoDrawHitEdges_CheckBox_Control . IsChecked . Value )
				{
					gl . LineWidth ( DrawHitEdgesLineWidth );
					gl . LineStipple ( factor: LineStippleFactor , pattern: LineStipplePattern );
					gl . Color ( DrawHitEdgesLineColor );
					gl . Vertex ( this . E [ 1 ]  . V [ 0 ] . cf );
					gl . Vertex ( this . E [ 1 ]  . V [ 1 ] . cf );
					gl . End ( );
				}
				if ( this.MW.Edge1Annotation_CheckBox_Control.IsChecked.Value )
				{
					AnnotateEdge ( gl , this . E [ 1 ] );
				}
				gl . PopAttrib ( );
			}
			if ( HitMaster ( V , this . E [ 2 ] ) )
			{
				this . E [ 2 ].Hit  = CaculateEdgeHit(this . E [ 2 ] ,V);
				EdgeHits++;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				if ( this . MW . DoDrawHits_CheckBox_Control . IsChecked . Value )
				{
					gl . LineWidth ( DrawHitEdgesLineWidth );
					gl . LineStipple ( factor: LineStippleFactor , pattern: LineStipplePattern );
					gl . Color ( DrawHitEdgesLineColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
					gl . Vertex ( this . E [ 2 ] . Hit );
					gl . End ( );
				}
				if ( this . MW . DoDrawHitEdges_CheckBox_Control . IsChecked . Value )
				{
					gl . LineWidth ( DrawHitEdgesLineWidth );
					gl . LineStipple ( factor: LineStippleFactor , pattern: LineStipplePattern );
					gl . Color ( DrawHitEdgesLineColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 2 ]  . V [ 0 ] . cf );
					gl . Vertex ( this . E [ 2 ]  . V [ 1 ] . cf );
					gl . End ( );
				}
				if ( this . MW . Edge2Annotation_CheckBox_Control . IsChecked . Value )
				{
					AnnotateEdge ( gl , this . E [ 2 ] );
				}
				gl . PopAttrib ( );
			}
			if ( HitMaster ( V , this . E [ 3 ] ) )
			{
				this . EdgeHits++;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				if ( this . MW . DoDrawHits_CheckBox_Control . IsChecked . Value )
				{
					gl . PointSize ( PointSize );
					gl . Color ( PointColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
					gl . Vertex ( this . E [ 3 ] . Hit );
					gl . End ( );
				}
				if ( this . MW . DoDrawHitEdges_CheckBox_Control . IsChecked . Value )
				{
					gl . LineWidth ( DrawHitEdgesLineWidth );
					gl . LineStipple ( factor: LineStippleFactor , pattern: LineStipplePattern );
					gl . Color ( DrawHitEdgesLineColor );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 3 ] . V [ 0 ] . cf );
					gl . Vertex ( this . E [ 3 ] . V [ 1 ] . cf );
					gl . End ( );
				}
				if ( this . MW . Edge3Annotation_CheckBox_Control . IsChecked . Value )
				{
					AnnotateEdge ( gl , this . E [ 3 ] );
				}

				gl . PopAttrib ( );
			}
				 if ( this . MW . OneHitWondercheckBox_Control . IsChecked . Value && this .   EdgeHits == 1 )
			{
			this.BoxelKindEnum=BoxelKindEnum.Weird1Hit;
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "EdgeHits 1!!!" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}
			else if ( this . MW . TwoHitWonderCheckBox_Control . IsChecked . Value && this .   EdgeHits == 2 )
			{	 this.BoxelKindEnum=BoxelKindEnum.IsNotCriticalTwoHits;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( LineWidth-1 );
				gl . Color ( LineColor );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				if ( this . E [ 0 ] . Hit != null )
				{
					gl . Vertex ( this . E [ 0 ].Hit );
				}

				if ( this . E [ 1 ] . Hit != null )
				{
					gl . Vertex ( this . E [1]. Hit );
				}

				if ( this . E [ 2 ] . Hit != null )
				{
					gl . Vertex ( this . E [2].Hit );
				}

				if ( this . E [ 3 ] . Hit != null )
				{
					gl . Vertex ( this . E [3].Hit  );
				}

				gl . End ( );
				gl . PopAttrib ( );
			}
			else if ( this . MW . ThreeHitWonderCheckBox_Control . IsChecked . Value && this . EdgeHits == 3 )
			{
				this.BoxelKindEnum=BoxelKindEnum.Weird3Hit;
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "EdgeHits 3 !!!" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( LineWidth - 1 );
				gl . PointSize ( PointSize );
				gl . Color ( LineColor );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				if ( this . E [ 0 ] . Hit != null )
				{
					gl . Vertex ( this . E [ 0 ] . Hit );
				}

				if ( this . E [ 1 ] . Hit != null )
				{
					gl . Vertex ( this . E [ 1 ] . Hit );
				}

				if ( this . E [ 2 ] . Hit != null )
				{
					gl . Vertex ( this . E [ 2 ] . Hit );
				}

				if ( this . E [ 3 ] . Hit != null )
				{
					gl . Vertex ( this . E [ 3 ] . Hit );
				}

				gl . End ( );
				gl . PopAttrib ( );
			}
			else if ( this . MW . FourHitWonderCheckBox_Control . IsChecked . Value && this .  EdgeHits == 4 )
			{
				if ( V > this . Critical_Mean_V )
				{

					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . LineWidth ( 3 );
					gl . Color ( 1 , .1 , blue: .9 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 3 ] . Hit );
					gl . Vertex ( this . E [ 0 ] . Hit );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . Vertex ( this . E [ 2 ] . Hit );
					gl . End ( );
					gl . PopAttrib ( );
				}

				if ( V < this . Critical_Mean_V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . LineWidth ( 3 );
					gl . Color ( 1 , .9 , blue: .1 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 0 ] . Hit );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . Vertex ( this . E [ 2 ] . Hit );
					gl . Vertex ( this . E [ 3 ] . Hit );
					gl . End ( );
					gl . PopAttrib ( );
				}

			}
		}

		private bool HitMaster ( float V , Edge E )
		{
			E.Hit=null;
			Boolean H=IsInBetween(v: V, e:E)  ;
			if(H){
				E.Hit= CaculateEdgeHit ( E,V);
				}
			return H;
		}

		private void DrawIsoEdge ( IsoEdge iE )
		{
			OpenGL gl = MainWindow . staticGLHook;
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . AccumBuffer );
			gl . LineWidth ( 2 );
			gl . Color ( 0.1 , 0.9 , 0.1 );
			float [ ] cf0 = iE . E . V [ 0 ] . cf;
			float [ ] cf1 = iE . E . V [ 1 ] . cf;
			float z0 = iE . E . V [ 0 ] . V;
			float z1 = iE . E . V [ 1 ] . V;
			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );
			gl . PopAttrib ( );
		}

		private void DrawIsoEdges ( )
		{
			for ( int i = 0 ; i < this . IsoEdges ; i++ )
			{
				IsoEdge IE = this . IsoEdge [ i ];
				DrawIsoEdge ( IE );
			}
		}

		private void DrawPerimeterEdges ( )
		{
			for ( int i = 0 ; i < 4 ; i++ )
			{
				Edge E = this . E [ i ];
				DrawPerimeterEdges ( E );
			}
		}

		private void DrawPerimeterEdges ( Edge e )
		{
			if ( e . SubEdgeCount > 0 )
			{
				if ( e . SubEdges != null )
				{
				}

				DrawPerimeterEdge ( e );
			}
		}

		private void DrawVertices ( )
		{
			for ( int i = 0 ; i < 4 ; i++ )
			{
				Edge E = this . E [ i ];
				Vertex V0 = E . V [ 0 ];
				V0 . Draw ( );
				for ( int j = 0 ; j < E . TweenVerts ; j++ )
				{
					Vertex TV = E . TweenVerte [ j ];
					TV . Draw ( );
				}
				Vertex V1 = E . V [ 1 ];
				V1 . Draw ( );
			}
		}

		private float [ ] GetCentroid ( float [ ] [ ] a )
		{
			return GetCentroid ( a [ 0 ] , a [ 1 ] );
		}

		private float [ ] GetCentroid ( float [ ] a , float [ ] b )
		{
			float [ ] c = { 0 , 0 , 0 };
			c = add ( a , b );
			float [ ] d = { 0 , 0 , 0 };
			d = divide ( c , 2 );
			return d;
		}

		#region Betweeners
		private bool IsBetween ( Edge e , float v )
		{
			float V0 = e . V [ 0 ] . V;
			float V1 = e . V [ 1 ] . V;
			return IsBetween ( V0 , v , V1 );
		}

		private bool IsBetween ( float v0 , float v , float v1 )
		{
			if ( v0 == v )
			{
			}

			if ( v == v1 )
			{
			}

			if ( v0 < v )
			{
				if ( v < v1 )
				{
					return true;
				}
			}
			if ( v0 > v )
			{
				if ( v > v1 )
				{
					return true;
				}
			}
			return false;
		}

		private bool IsInBetween ( float v , Edge e )
		{
			float v0 = e . V [ 0 ] . V;
			float v1 = e . V [ 1 ] . V;

			return IsInBetween ( v0 , v , v1 );
		}

		private bool IsInBetween ( int v , int e )
		{
			int S = this . SortedVertexIndicies [ v ];
			Vertex V = this . V [ S ];
			Edge E = this . E [ e ];
			return IsInBetween ( V , E );
		}

		private bool IsInBetween ( Vertex v , Edge e )
		{
			return IsInBetween ( e . V [ 0 ] . V , v . V , e . V [ 1 ] . V );
		}

		private bool IsInBetween ( float [ ] cf , Edge e )
		{
			float [ ] cf0 = e . V [ 0 ] . cf;
			float [ ] cf1 = e . V [ 1 ] . cf;
			Boolean xb = IsInBetween ( cf0 [ 0 ] , cf [ 0 ] , cf1 [ 0 ] );
			Boolean yb = IsInBetween ( cf0 [ 1 ] , cf [ 1 ] , cf1 [ 1 ] );
			Boolean zb = IsInBetween ( cf0 [ 2 ] , cf [ 2 ] , cf1 [ 2 ] );
			return xb && yb;   // not zb until we get serious about 3d
		}

		private bool IsInBetween ( float v0 , float tweener , float v1 )
		{
			if ( v0 == v1 )
			{
				return false; // hmmm...
			}

			if ( v0 == tweener )
			{
				return false;
			}
			if ( tweener == v1 )
			{
				return false;
			}
			Boolean v0LTv1 = v0 < v1;
			if ( v0LTv1 )     // starting small moving up and rising from v2 to v3
			{
				Boolean vee = ( v0 < tweener );
				if ( vee )
				{
					Boolean bee = tweener < v1;
					if ( bee )
					{
						return true;
					}
				}
			}
			if ( v1 < v0 )     //   starting large, moving down and lowering from v3 to v1
			{
				Boolean booley = IsInBetween ( v1 , tweener , v0 );
				return booley;
			}
			return false;
		}
		#endregion Betweeners

		private Boolean IsThisCritical ( )
		{
			Boolean flat0 = ( this . E [ 0 ] . Delta_V == 0f );
			Boolean flat1 = ( this . E [ 1 ] . Delta_V == 0f );
			Boolean flat2 = ( this . E [ 2 ] . Delta_V == 0f );
			Boolean flat3 = ( this . E [ 3 ] . Delta_V == 0f );
			Boolean flat =  ( flat0 && flat1 && flat2 && flat3 );
			if ( flat )
			{
				return false;
			}
			Boolean wiggley00 = ( this . E [ 0 ] . Delta_V > 0 );
			Boolean wiggley01 = ( this . E [ 1 ] . Delta_V < 0 );
			Boolean wiggley02 = ( this . E [ 2 ] . Delta_V > 0 );
			Boolean wiggley03 = ( this . E [ 3 ] . Delta_V < 0 );

			Boolean wiggle0 = (	wiggley00 &&  wiggley01 &&	 wiggley02 &&	wiggley03 );
			if ( wiggle0 )
			{
				this . IsCritical = true;

				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				return true;
			}
			Boolean wiggley10 = ( this . E [ 0 ] . Delta_V > 0 );
			Boolean wiggley11 = ( this . E [ 1 ] . Delta_V < 0 );
			Boolean wiggley12 = ( this . E [ 2 ] . Delta_V > 0 );
			Boolean wiggley13 = ( this . E [ 3 ] . Delta_V < 0 );
			Boolean wiggle1 = (wiggley10 &&	wiggley11 &&  wiggley12 &&	 wiggley13 );
			if ( wiggle1 )
			{
				this . IsCritical = true;
				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				return true;
			}
			this.IsCritical=false;
			return false;
		}

		private float [ ] [ ] LineStinker ( float [ ] cf0 , float [ ] cf1 , float fraction , WpfControlLibrary1 . LineStinkerModes mode )
		{
			if ( fraction < 0 )
			{
				return null;
			}

			if ( fraction > 1 )
			{
				return null;
			}

			float [ ] a = new float [ 3 ];

			a = subtract ( cf0 , cf1 );

			float m = magnitude ( a );

			float [ ] n = Divide ( a , m );

			float [ ] v_forward = Multiply ( n , fraction );
			float [ ] v_reverse = Multiply ( n , -fraction );

			float [ ] [ ] stinkey_vertices = new float [ 2 ] [ ];

			float [ ] stinky0 = new float [ 3 ];
			float [ ] stinky1 = new float [ 3 ];

			switch ( mode )
			{
				case WpfControlLibrary1 . LineStinkerModes . StartAtNearVertex:
					stinky0 = cf0;
					stinky1 = Add ( v_forward , cf0 );
					break;

				case WpfControlLibrary1 . LineStinkerModes . StartAtFarVertex:
					stinky0 = cf1;
					stinky1 = Add ( v_reverse , cf1 );
					break;

				case WpfControlLibrary1 . LineStinkerModes . FloatBetweenVerticies:
					float [ ] midpoint = new float [ 3 ];
					midpoint = GetCentroid ( cf0 , cf1 );
					stinky1 = Add ( v_forward , midpoint );
					stinky0 = Add ( v_reverse , midpoint );
					break;
				default:
					return null;
			}

			stinkey_vertices [ 0 ] = new float [ 3 ] { stinky0 [ 0 ] , stinky0 [ 1 ] , stinky0 [ 2 ] };
			stinkey_vertices [ 1 ] = new float [ 3 ] { stinky1 [ 0 ] , stinky1 [ 1 ] , stinky1 [ 2 ] };

			return stinkey_vertices;
		}

		private Vertex LoadCentroid ( )
		{
			float X = ( this . V [ 0 ] . cf [ 0 ] + this . V [ 1 ] . cf [ 0 ] + this . V [ 2 ] . cf [ 0 ] + this . V [ 3 ] . cf [ 0 ] ) / 4f;
			float Y = ( this . V [ 0 ] . cf [ 1 ] + this . V [ 1 ] . cf [ 1 ] + this . V [ 2 ] . cf [ 1 ] + this . V [ 3 ] . cf [ 1 ] ) / 4f;
			float Z = ( this . V [ 0 ] . cf [ 2 ] + this . V [ 1 ] . cf [ 2 ] + this . V [ 2 ] . cf [ 2 ] + this . V [ 3 ] . cf [ 2 ] ) / 4f;
			float V = ( this . V [ 0 ] . V + this . V [ 1 ] . V + this . V [ 2 ] . V + this . V [ 3 ] . V ) / 4f;

			Vertex C = new Vertex ( X , Y , Z , V )
			{
				VertexIndex = 4
			};

			return C;
		}

		private void LoadSubEdges ( )
		{
			for ( int i = 0 ; i < 4 ; i++ )
			{
				LoadSubEdges ( i );
			}
		}

		private void LoadSubEdges ( int e )
		{
			Edge E = this . E [ e ];
			if ( E . TweenVerts == 0 )
			{
				return;
			}

			LoadSubEdges ( E );
		}

		private void LoadSubEdges ( Edge E )
		{
			Vertex V0 = E . V [ 0 ];
			Vertex V1 = E . V [ 1 ];
			if ( E . TweenVerts == 1 )
			{
				Vertex V = E . TweenVerte [ 0 ];
				Edge SubEdge0 = new Edge ( E . EdgeIndex , 0 , V0 , V, this );
				Edge SubEdge1 = new Edge ( E . EdgeIndex , 1 , V , V1, this );
				V . Previous = V0;
				V . Next = V1;
				E . SubEdgeCount = 2;
				E . SubEdges = new Edge [ 2 ] { SubEdge0 , SubEdge1 };
				SubEdge0 . PreviousEdge = E . PreviousEdge;
				SubEdge0 . IsSubEdge = true;
				SubEdge0 . ParentEdge = E;
				SubEdge0 . NextPeer = E . SubEdges [ 1 ];
				SubEdge1 . PreviousEdge = E . SubEdges [ 0 ];
				SubEdge1 . IsSubEdge = true;
				SubEdge1 . ParentEdge = E;
				SubEdge1 . NextPeer = E . NextPeer;
				return;
			}
			else
			if ( E . TweenVerts == 2 )
			{
				Vertex V00 = E . TweenVerte [ 0 ];
				Edge SubEdge0 = new Edge ( E . EdgeIndex , 0 , V0 , V00 , this);
				Vertex V01 = E . TweenVerte [ 1 ];
				V00 . Previous = V0;
				V00 . Next = V01;
				V01 . Previous = V00;
				V01 . Next = V1;
				Edge SubEdge1 = new Edge ( E . EdgeIndex , 1 , V00 , V01, this );
				Edge SubEdge2 = new Edge ( E . EdgeIndex , 2 , V01 , V1, this );
				E . SubEdgeCount = 3;
				E . SubEdges = new Edge [ 3 ] { SubEdge0 , SubEdge1 , SubEdge2 };
				SubEdge0 . PreviousEdge = E . PreviousEdge;
				SubEdge0 . ParentEdge = E;
				SubEdge0 . IsSubEdge = true;
				SubEdge0 . NextPeer = SubEdge1;
				SubEdge1 . PreviousEdge = SubEdge0;
				SubEdge1 . ParentEdge = E;
				SubEdge1 . IsSubEdge = true;
				SubEdge1 . NextPeer = SubEdge2;
				SubEdge2 . PreviousEdge = SubEdge1;
				SubEdge2 . ParentEdge = E;
				SubEdge2 . IsSubEdge = true;
				SubEdge2 . NextPeer = E . NextPeer;
				return;
			}
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "What are you doing here?" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
		}

		private void LoadTriangles ( )
		{

			//Debug . WriteLine ( String . Format ( "{0}" , nameof( LoadTriangles ) ) );

		}

		private void LoadTweenVerts ( )
		{
			for ( int e = 0 ; e < 4 ; e++ )
			{
				for ( int v = 1 ; v < 3 ; v++ )
				{
					if ( IsBetween ( v , e ) )
					{
						float [ ] cf = OnEdge ( v , e ); // needs testing
						Edge E = this . E [ e ];
						if ( E . TweenVerte == null )
						{
							E . TweenVerte = new Vertex [ 2 ];
						}
						Vertex V = this . V [ v ];
						float vv = V . V;
						Vertex VV = new Vertex ( cf , vv );
						E . TweenVerte [ E . TweenVerts ] = VV;
						E . TweenVerts++;
						IsoEdge IE = new IsoEdge ( this . IsoEdges , 0 , vv , VV , V );
						VV . E = IE;
						V . E = IE;
						this . IsoEdge [ this . IsoEdges ] = IE;
						this . IsoEdges++;
					}
				}
			}
		}

		private float magnitude ( float [ ] a )
		{
			double b = ( a [ 0 ] * a [ 0 ] ) + ( a [ 1 ] * a [ 1 ] ) + ( a [ 2 ] * a [ 2 ] );
			float c = ( float ) Math . Sqrt ( b );
			return c;
		}

		private double Magnitude ( dvec3 v )
		{
			double m = ( v . x * v . x ) + ( v . y * v . y ) + v . z + v . z;
			double mr = Math . Sqrt ( m );
			return mr;
		}

		private float [ ] Multiply ( float [ ] a , float [ ] b )
		{
			float [ ] c = { a [ 0 ] * b [ 0 ] , a [ 1 ] * b [ 1 ] , a [ 2 ] * b [ 2 ] };
			return c;
		}

		private float [ ] Multiply ( float [ ] a , float v )
		{
			float [ ] cf = { a [ 0 ] * v , a [ 1 ] * v , a [ 2 ] * v , };

			return cf;
		}

		private dvec3 Normalize ( dvec3 dvec3 )
		{
			double Mag = Magnitude ( dvec3 );
			dvec3 nal = Divide ( Mag , dvec3 );
			return nal;
		}

		private float [ ] OnEdge ( int v , int e )
		{
			Vertex V = this . V [ this . SortedVertexIndicies [ v ] ];
			Edge E = this . E [ e ];
			return OnEdge ( V , E );
		}

		private float [ ] OnEdge ( Vertex v , Edge e )
		{
			float ev0 = e . V [ 0 ] . V;
			float ev1 = e . V [ 1 ] . V;
			float deltaEdge = ( ev1 - ev0 );
			float vv = v . V;
			float vx = v . cf [ 0 ];
			float vy = v . cf [ 1 ];
			float vz = v . cf [ 2 ];
			float deltaVertex = ( vv - ev0 );
			float linageFraction = deltaVertex / deltaEdge;
			float x0 = e . V [ 0 ] . I;
			float y0 = e . V [ 0 ] . J;
			float z0 = e . V [ 0 ] . cf [ 2 ];
			float x1 = e . V [ 1 ] . I;
			float y1 = e . V [ 1 ] . J;
			float z1 = e . V [ 1 ] . cf [ 2 ];
			float deltax0x1 = x1 - x0;
			float deltay0y1 = y1 - y0;
			float deltaz0z1 = z1 - z0;
			float dx = x0 + ( linageFraction * deltax0x1 );
			float dy = y0 + ( linageFraction * deltay0y1 );
			float dz = z0 + ( linageFraction * deltaz0z1 );
			float [ ] c = { dx , dy , dz };
			return c;
		}

		private void RotationTest ( Edge E , OpenGL gl )
		{
			float knob00 = this . MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float [ ] cent = { this . Centroid . cf [ 0 ] , this . Centroid . cf [ 1 ] , this . Centroid . cf [ 2 ] };

			GlmNet . vec3 vf0 = new GlmNet . vec3 ( x: cf0 [ 0 ] , y: cf0 [ 1 ] , z: cf0 [ 2 ] );
			GlmNet . vec3 vf1 = new GlmNet . vec3 ( x: cf1 [ 0 ] , y: cf1 [ 1 ] , z: cf1 [ 2 ] );
			GlmNet . vec3 vce = new GlmNet . vec3 ( x: cent [ 0 ] , y: cent [ 1 ] , z: cent [ 2 ] );

			float [ ] faaa = subtract ( cf0 , cent );
			GlmNet . vec3 cv = new GlmNet . vec3 ( x: faaa [ 0 ] , y: faaa [ 1 ] , z: faaa [ 2 ] );
			GlmNet . vec3 z_rot_axis = new GlmNet . vec3 ( 0 , 0 , 1 );
			GlmNet . mat4 ID = GlmNet . mat4 . identity ( );
			GlmNet . vec3 ZV = new GlmNet . vec3 ( 0 , 0 , 0 );
			GlmNet . vec3 noon = new GlmNet . vec3 ( x: 0 , y: 1 , z: 0 );

			double spinner2 = this . MW . Hack_H_Slider_04_UserControl . SliderValue * 2f * Math . PI;
			float spinner2f = ( float ) spinner2;
			GlmNet . mat4 rotation = GlmNet . glm . rotate ( ID , spinner2f , z_rot_axis );
			GlmNet . mat3 normalMatrix = rotation . to_mat3 ( );
			GlmNet . vec3 rotated_noon = normalMatrix * noon;
			float [ ] ccc = { rotated_noon . x + cent [ 0 ] , rotated_noon . y + cent [ 1 ] , rotated_noon . z + cent [ 2 ] };
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . Color ( 1 , .2 , .9 );
			gl . LineWidth ( 1 );
			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cent );
			gl . Vertex ( ccc );
			gl . End ( );
			gl . PopAttrib ( );
		}

		private String Sortee ( short smallestVertex , short largestVertex )
		{
			float v0 = this . V [ smallestVertex ] . V;
			float v1 = this . V [ largestVertex ] . V;
			const String strang_format = "0.000";
			String Strang = String . Format ( "{0}{1}{2}" , v0 . ToString ( strang_format ) , Comparis ( v0 , v1 ) , v1 . ToString ( strang_format ) );

			return Strang;
		}

		private void SortVerticies ( )
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
					int j = this . SortedVertexIndicies [ ii ];
					int k = this . SortedVertexIndicies [ iii ];
					float v0 = this . V [ j ] . V;
					float v1 = this . V [ k ] . V;
					if ( v0 > v1 )
					{
						//Burblegiggle ( swaps , "pre -Swap" );
						short temp0 = this . SortedVertexIndicies [ ii ];
						short temp1 = this . SortedVertexIndicies [ iii ];
						this . SortedVertexIndicies [ ii ] = temp1;
						this . SortedVertexIndicies [ iii ] = temp0;
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
			} while ( !IsSorted );

			//Burblegiggle ( swaps , "Completed" );

			this . LargestVertexIndex = this . SortedVertexIndicies [ 3 ];
			this . SmallestVertexIndex = this . SortedVertexIndicies [ 0 ];

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , Sortee(SmallestVertex,LargestVertex) , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private float [ ] subtract ( float [ ] a , float [ ] b )
		{
			float [ ] c = { b [ 0 ] - a [ 0 ] , b [ 1 ] - a [ 1 ] , b [ 2 ] - a [ 2 ] };		// should subtract a-b, subtract(a,b)
			return c;
		}

		private double [ ] ToDoubleArray ( float [ ] v )
		{
			double [ ] d = { v [ 0 ] , v [ 1 ] , v [ 2 ] };
			return d;
		}

		private float [ ] toFloatArray ( dvec3 dv )
		{
			float [ ] fa = { ( float ) dv . x , ( float ) dv . y , ( float ) dv . z };
			return fa;
		}

		private void VerifyCycles ( )
		{
			if ( this . IsCritical )
			{
				return;
			}

			VerifyEdgeCycles ( );
			VerifyVertexCycles ( );
		}

		private void VerifyEdgeCycles ( Boolean NextEdges = true , Boolean PreviousEdges = true )
		{
			if ( NextEdges )
			{
				VerifyEdgeCyclesNext ( );
			}

			if ( PreviousEdges )
			{
				VerifyEdgeCyclesPrevious ( );
			}

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void VerifyEdgeCyclesNext ( )
		{
			int largest_index = this . SortedVertexIndicies [ 0 ];
			int smallest_index = this . SortedVertexIndicies [ 3 ];
			Vertex Vlargest = this . V [ largest_index ];
			Edge E = Vlargest . NextEdge;
			Boolean ExitCondition = true;
			while ( ExitCondition )
			{
				//System . Diagnostics . Debug . Write  ( String . Format ( "^{0} " , E.EdgeIndex ) );
				if ( E . SubEdges == null )
				{
					DrawEdge ( E , Edge . EdgeDirection . ClockwiseNext );
					E = E . NextPeer;
					int VI = E . V [ 0 ] . VertexIndex;
					ExitCondition = smallest_index != VI;
				}
				else
				{
					Edge SE = E . SubEdges [ 0 ];

					while ( SE . IsSubEdge )
					{
						//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , SE.SubEdgeIndex , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

						DrawEdge ( SE , Edge . EdgeDirection . ClockwiseNext );
						SE = SE . NextPeer;
					}
					ExitCondition = false;
				}
			}
		}

		private void VerifyEdgeCyclesPrevious ( )
		{
			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} " , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			Vertex Vlargest = this . V [ this . LargestVertexIndex ];

			Edge E = Vlargest . PreviousEdge;
			Boolean ExitCondition = true;
			while ( ExitCondition )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} v{1} {2}" , this . DisplayMe ( ) , E . EdgeIndex , E . IsSubEdge ) );

				if ( E . SubEdges == null )
				{
					DrawEdge ( E , Edge . EdgeDirection . CounterClockwisePrevious );

					E = E . PreviousEdge;
					int VI = E . V [ 1 ] . VertexIndex;
					ExitCondition = this . LargestVertexIndex != VI;
				}
				else
				{
					int last_subedge = E . SubEdgeCount - 1;
					Edge SE = E . SubEdges [ last_subedge ];

					while ( SE . IsSubEdge )
					{
						DrawEdge ( SE , Edge . EdgeDirection . CounterClockwisePrevious );
						SE = SE . PreviousEdge;
					}
					ExitCondition = false;
				}
			}
		}

		private void VerifyVertexCycles ( )
		{
			if ( this . IsCritical )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} [{1},{2}] IsCritical" , nameof ( VerifyVertexCycles ) , this . I , this . J ) );
				return;
			}

			int largest_index = this . SortedVertexIndicies [ 0 ];
			int smallest_index = this . SortedVertexIndicies [ 3 ];
			Vertex Vsmallest = this . V [ smallest_index ];
			int next_smallest_index = Vsmallest . Next . VertexIndex;
			int previous_smallest_index = Vsmallest . Previous . VertexIndex;

			Vertex Vlargest = this . V [ largest_index ];
			Vertex V = Vlargest;

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} [{1}.{2}] " , nameof ( VerifyVertexCycles ) , this . I , this . J ) );

			const Boolean reset_condition = true;
			Boolean exit_condition = reset_condition;
			while ( exit_condition )
			{
				System . Diagnostics . Debug . Write ( String . Format ( "^{0}->{1}" , V . VertexIndex , V . Next . VertexIndex ) );

				V = V . Next;
				exit_condition = V . VertexIndex != next_smallest_index;
			}

			V = Vlargest;
			exit_condition = reset_condition;
			while ( exit_condition )
			{
				System . Diagnostics . Debug . Write ( String . Format ( "V{0}->{1}" , V . VertexIndex , V . Previous . VertexIndex ) );

				V = V . Previous;
				exit_condition = V . VertexIndex != previous_smallest_index;
			}

			System . Diagnostics . Debug . WriteLine ( String . Format ( "\n{0} {1} " , "completed" , ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) );
		}

		protected void DebugDrawEdge ( Edge E )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}

			if ( true )
			{
				AnnotateEdge ( gl , E );
			}

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;

			if ( this . MW . HackCheckBox_C4_R1_CheckBox_Control . IsChecked . Value )
			{
				float [ ] cf00 = E . V [ 0 ] . cf;
				float [ ] cf10 = E . V [ 1 ] . cf;

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . Color ( 0.9 , 0.8 , 0.8 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( cf00 );
				gl . Vertex ( cf10 );
				gl . End ( );
				gl . PopAttrib ( );
			}

			float [ ] cent = { this . Centroid . cf [ 0 ] , this . Centroid . cf [ 1 ] , this . Centroid . cf [ 2 ] };

			GlmNet . vec3 vf0 = new GlmNet . vec3 ( x: cf0 [ 0 ] , y: cf0 [ 1 ] , z: cf0 [ 2 ] );
			GlmNet . vec3 vf1 = new GlmNet . vec3 ( x: cf1 [ 0 ] , y: cf1 [ 1 ] , z: cf1 [ 2 ] );
			GlmNet . vec3 vce = new GlmNet . vec3 ( x: this . Centroid . cf [ 0 ] , y: this . Centroid . cf [ 1 ] , z: this . Centroid . cf [ 2 ] );

			float [ ] faaa = subtract ( cf0 , cent );
			GlmNet . vec3 cv = new GlmNet . vec3 ( x: faaa [ 0 ] , y: faaa [ 1 ] , z: faaa [ 2 ] );
			GlmNet . vec3 v3z_RotateAxis = new GlmNet . vec3 ( 0 , 0 , 1 );
			GlmNet . mat4 ID = GlmNet . mat4 . identity ( );
			GlmNet . vec3 ZV = new GlmNet . vec3 ( 0 , 0 , 0 );
			GlmNet . vec3 noon = new GlmNet . vec3 ( x: 0 , y: 1 , z: 0 );

			if ( this . MW . HackCheckBox_C9_R2_CheckBox_Control . IsChecked . Value )
			{
				float TwistyKnob = ( float ) ( this . MW . HighSideHalfArrowAngleOffCentroid_H_Slider_UserControl . SliderValue * Math . PI );
				float StreatchyKnob = this . MW . HighSideHalfArrowHeadLengthFraction_H_Slider_UserControl . SliderValue;
				Vertex Bigger, Smaller;
				Boolean ShowCentroid = false;
				if ( E . V [ 0 ] . V > E . V [ 1 ] . V )
				{
					Bigger = E . V [ 0 ];
					Smaller = E . V [ 1 ];
				}
				else if ( E . V [ 0 ] . V < E . V [ 1 ] . V )
				{
					Bigger = E . V [ 1 ];
					Smaller = E . V [ 0 ];
					TwistyKnob = -TwistyKnob;
				}
				else
				{
					Bigger = E . V [ 0 ];
					Smaller = E . V [ 1 ];
					//return; // == flat edge
				}
				// p prefix fixed point
				// v prefix direction unfixed vector

				GlmNet . vec3 pcf0 = new GlmNet . vec3 ( x: Smaller . cf [ 0 ] , y: Smaller . cf [ 1 ] , z: Smaller . cf [ 2 ] );
				GlmNet . vec3 p3Bigger = new GlmNet . vec3 ( x: Bigger . cf [ 0 ] , y: Bigger . cf [ 1 ] , z: Bigger . cf [ 2 ] );
				GlmNet . vec3 p3Centroid = new GlmNet . vec3 ( this . Centroid . cf [ 0 ] , this . Centroid . cf [ 1 ] , this . Centroid . cf [ 2 ] );

				GlmNet . mat4 m4rotator = GlmNet . glm . rotate ( ID , TwistyKnob , v3z_RotateAxis );
				GlmNet . mat3 m3rotator = m4rotator . to_mat3 ( );
				GlmNet . vec3 v3Centroid = p3Centroid - p3Bigger;
				GlmNet . vec3 v3twisted = m3rotator * v3Centroid;
				//GlmNet .vec3 twisty1 = GlmNet.mat3.identity() * v3Centroid;
				GlmNet . vec3 v3twistedStreatchey = v3twisted * StreatchyKnob;
				GlmNet . vec3 pt = v3twistedStreatchey + p3Bigger;

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Color ( 0 , .8 , 1 );
				gl . LineWidth ( 2 );
				gl . Vertex ( Smaller . cf );
				gl . Vertex ( Bigger . cf );
				gl . Color ( .9 , .7 , .4 );
				gl . LineWidth ( 1 );
				gl . Vertex ( Bigger . cf );
				gl . Vertex ( x: pt . x , y: pt . y , z: pt . z );
				if ( ShowCentroid )
				{
					gl . Color ( .5 , .7 , .8 );
					gl . LineWidth ( 1 );
					gl . Vertex ( Bigger . cf );
					gl . Vertex ( this . Centroid . cf );
				}
				gl . Color ( .9 , .2 , .3 );
				gl . Vertex ( Bigger . cf );
				gl . Vertex ( pt . x , pt . y , pt . z );
				gl . End ( );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . PointSize ( 4 );
				gl . Color ( red: .9 , green: .1 , blue: .1 );
				gl . Vertex ( Bigger . cf );
				if ( ShowCentroid )
				{
					gl . PointSize ( 2 );
					gl . Vertex ( this . Centroid . cf );
				}
				gl . PointSize ( 6 );
				gl . Vertex ( pt . x , pt . y , pt . z );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C7_R1_CheckBox_Control . IsChecked . Value )
			{
				RotationTest ( E , gl );
			}

			if ( this . MW . HackCheckBox_C5_R2_CheckBox_Control . IsChecked . Value )
			{
				float knob1 = ( float ) ( this . MW . Hack_H_Slider_01_UserControl . SliderValue * 2f * Math . PI );
				float knobl1 = this . MW . Hack_H_Slider_03_UserControl . SliderValue;
				GlmNet . mat4 d0q = GlmNet . glm . rotate ( ID , knob1 , v3z_RotateAxis );
				GlmNet . mat3 mm0 = d0q . to_mat3 ( );
				GlmNet . vec3 twisty1 = mm0 * cv;
				GlmNet . vec3 twisty22 = twisty1 * knobl1;
				GlmNet . vec3 located0 = twisty22 + vf0;
				float [ ] FA0 = toFloatArray ( located0 );

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( 0 , .8 , 1 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( cf0 );
				gl . Vertex ( FA0 );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C6_R1_CheckBox_Control . IsChecked . Value )
			{
				float knob2 = ( float ) ( this . MW . Hack_H_Slider_02_UserControl . SliderValue * 2f * Math . PI );
				float knobl2 = this . MW . ArrowHeadSide2Length2_H_Slider_UserControl1 . SliderValue;
				GlmNet . mat4 d1q = GlmNet . glm . rotate ( ID , knob2 , v3z_RotateAxis );
				GlmNet . mat3 mm1 = d1q . to_mat3 ( );
				GlmNet . vec3 twisty2 = mm1 * cv;
				GlmNet . vec3 twisty21 = twisty2 * knobl2;
				GlmNet . vec3 located1 = twisty21 + vf0;  // this is now a fixed point in space
				float [ ] FA1 = toFloatArray ( located1 );

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( 1 , .2 , .9 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( cf0 );
				gl . Vertex ( FA1 );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C5_R1_CheckBox_Control . IsChecked . Value )
			{
				float u = this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;
				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];
				float z0 = cf0 [ 2 ];

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];
				float z1 = cf1 [ 2 ];

				float x = ( ( 1 - u ) * x0 ) + ( u * x1 );
				float y = ( ( 1 - u ) * y0 ) + ( u * y1 );
				float z = ( ( 1 - u ) * z0 ) + ( u * z1 );

				float [ ] c_th = { x , y , z };

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . PointSize ( 5 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( c_th [ 0 ] , c_th [ 1 ] , c_th [ 2 ] );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( this . MW . HackCheckBox_C3_R2_CheckBox_Control . IsChecked . Value )
			{
				float v = this . MW . region_threshold_H_Slider_UserControl . SliderValue;
				if ( !IsInBetween ( v , E ) )
				{
					return;
				}
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 3 );
				gl . PointSize ( 5 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( E . V [ 0 ] . cf );
				gl . Vertex ( E . V [ 1 ] . cf );
				gl . End ( );
				gl . PopAttrib ( );

				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];
				float z0 = cf0 [ 2 ];
				float v0 = E . V [ 0 ] . V;

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];
				float z1 = cf1 [ 2 ];
				float v1 = E . V [ 1 ] . V;

				float u = ( v - v0 ) / ( v1 - v0 );
				float x = ( ( 1 - u ) * x0 ) + ( u * x1 );
				float y = ( ( 1 - u ) * y0 ) + ( u * y1 );
				float z = ( ( 1 - u ) * z0 ) + ( u * z1 );

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . PointSize ( 5 );
				gl . Color ( .3 , .5 , .8 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( x , y , z );
				gl . End ( );
				gl . PopAttrib ( );
			}
		}

		internal void DrawMe ( )
		{
			//DrawBoxelEdgeCycles ( );
			DrawContoursInBoxel ( );
			//if ( this . IsCritical )
			//	DrawCriticalDiamond ();
			if ( this.MW.DoDrawSelfHits_CheckBox_Control.IsChecked.Value)
			{
				this . DrawOnEdges (  );
			}

			if ( this.MW.DoTitateCriticalities_CheckBox_Control.IsChecked.Value)
			{
				this .TitrateCriticalValues();
			}
			//DrawCriticalBoxel ( );
			AnnotateBoxelAtCentroid ( );

			//VerifyCycles ( );
		}

		private void TitrateCriticalValues ( )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}

			if(!this.IsCritical)
			{
				return;
			}

			if ( MW.Do_RESET_TitateCriticalities_CheckBox_Control.IsChecked.Value )
			{
				this . Titration_Steps	=0;
				return;
			}
			if ( this.Titration_Steps==0)
			{
				this.Critical_Titrate_V=Penult_Low_V;
				this . Delta_Crit_V=MW.CriticalitySweeper_DELTA_H_Slider_User_Control.SliderValue/50f;
			}
			this.EdgeHits = 0;
			//float [ ] [ ] EdgeHit = new float [ 4 ] [ ];
			if ( IsInBetween ( this . Critical_Titrate_V , this . E [ 0 ] ) ) {
				this . E [ 0 ] . Hit=CaculateEdgeHit(this.E[0], this.Critical_Titrate_V);
				this.EdgeHits++;
			}
			if ( IsInBetween ( this . Critical_Titrate_V , this . E [ 1 ] ) ) {
				this . E [ 1 ] . Hit = CaculateEdgeHit ( this . E [ 1 ] , this . Critical_Titrate_V );
				this . EdgeHits++;
			}
			if ( IsInBetween ( this . Critical_Titrate_V , this . E [ 2 ] ) ) {
				this . E [ 2 ] . Hit = CaculateEdgeHit ( this . E [ 2 ] , this . Critical_Titrate_V );
				this . EdgeHits++;
			}
			if ( IsInBetween ( this . Critical_Titrate_V , this . E [ 3 ] ) ) {
				this . E [ 3 ] . Hit = CaculateEdgeHit ( this . E [ 3 ] , this . Critical_Titrate_V );
				this . EdgeHits++;
			}
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			if ( this . EdgeHits == 4 )
			{
			this.BoxelKindEnum=BoxelKindEnum.IsCritical4Hit;
				if ( this . Critical_Titrate_V > this . Critical_Mean_V )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 1 , .1 , blue: .9 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 0 ] . Hit );
					gl . Vertex ( this . E [ 3 ] . Hit  );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . Vertex ( this . E [ 2 ] . Hit  );
					gl . End ( );																				  
				}

				if ( this . Critical_Titrate_V < this . Critical_Mean_V )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 1 , .9 , blue: .1 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( this . E [ 0 ] . Hit );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . Vertex ( this . E [ 2 ] . Hit );
					gl . Vertex ( this . E [ 3 ] . Hit );
					gl . End ( );
				}

				if ( true )
				{
					gl . LineWidth ( 1 );
					gl . Color ( .9 , .9 , blue: .9 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
					gl . Vertex ( this . E [ 0 ] . Hit );
					gl . Vertex ( this . E [ 1 ] . Hit );
					gl . Vertex ( this . E [ 2 ] . Hit );
					gl . Vertex ( this . E [ 3 ] . Hit );
					gl . End ( );
				}
			}
			if(true)
			{
				gl.PointSize( size: 4 );
				gl.Color(.9,.9,1);

				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				if ( this . E [ 0 ] != null )
				{
					gl . Vertex ( this . E [ 0 ] . Hit );
				}

				if ( this . E [ 1 ] != null )
				{
					gl . Vertex ( this . E [ 1 ] . Hit );
				}

				if ( this . E [ 2 ] != null )
				{
					gl . Vertex ( this . E [ 2 ] . Hit );
				}

				if ( this . E [ 3 ] != null )
				{
					gl . Vertex ( this . E [ 3 ] . Hit );
				}

				gl . End ( );
			}
			gl . PopAttrib ( );
			this . Titration_Steps++;
			this . Critical_Titrate_V += this . Delta_Crit_V;
			if ( this . Critical_Titrate_V > this . Penult_High_V )
			{
				this . Delta_Crit_V = -this . Delta_Crit_V ;
				this . Critical_Titrate_V = this . Penult_High_V;
				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "^" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}
			if ( this . Critical_Titrate_V < this . Penult_Low_V )
			{
				this . Delta_Crit_V = -this . Delta_Crit_V;
				this . Critical_Titrate_V = this . Penult_Low_V;
				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "V" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}
		}

		private void DrawOnEdges ( )
		{
			float v0 = this . Penult_Low_V;
			float v1 = this . Penult_High_V;
			this . DrawOnEdges ( v0 , v1 );
		}

		private void DrawOnEdges ( float v0 , float v1 )
		{
			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}
			DrawOnEdges ( gl , v0 , v1 );
		}

		private void DrawOnEdges ( OpenGL gl,  float V0 , float V1 )
		{
			Edge E0 = this . E [ 0 ];
			Edge E1 = this . E [ 1 ];
			Edge E2 = this . E [ 2 ];
			Edge E3 = this . E [ 3 ];
			float [ ] [ ] EdgeHits0 = DrawOnEdges ( gl , E0 , V0 , V1 );
			float [ ] [ ] EdgeHits1 = DrawOnEdges ( gl , E1 , V0 , V1 );
			float [ ] [ ] EdgeHits2 = DrawOnEdges ( gl , E2 , V0 , V1 );
			float [ ] [ ] EdgeHits3 = DrawOnEdges ( gl , E3 , V0 , V1 );
			float [][][] EdgeHits={EdgeHits0, EdgeHits1, EdgeHits2, EdgeHits3};
			if ( false )
			{
				for ( int i = 0 ; i < 4 ; i++ )
				{
					float [ ] e0 = this . E [ i ] . V [ 0 ] . cf;
					float[] hit0=EdgeHits[i][0]	;
					float D0=DistanceAlongEdgeCycle	(e0,hit0);
					float[] hit1=EdgeHits[i][1];

					float D1=DistanceAlongEdgeCycle(e0,hit1);
					if ( D0 == 0.0f )
					{

						//System . Diagnostics . Debug . WriteLine ( String . Format ( "zero distance Hit 0 {0} {1} " , DisplayMe(E[i]), ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

					}
					if ( D1 == 0.0f )
					{

						System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "zero distance 1" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

					}
					if ( D1 < D0 )
					{
						System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
					}
				}
			}

			if ( true)
			{
				Boolean SelfEdgeHits=false;
				Boolean CrossEdgeHits=true;

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				if ( SelfEdgeHits && ( EdgeHits [ 0 ] [ 0 ] != null ) && ( EdgeHits [ 0 ] [ 0 ] != null ) )
				{
					gl . LineWidth ( 1 );
					gl . Color ( 0.9 , 0.2 , 02 );
					gl . Vertex ( EdgeHits0 [ 0 ] );
					gl . Vertex ( EdgeHits0 [ 1 ] );
				}

				if ( CrossEdgeHits && ( EdgeHits [ 0 ] [ 1 ] != null ) && ( EdgeHits [ 1 ] [ 0 ] != null ) )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 0.9 , 0.7 , 0.8 );
					gl . Vertex ( EdgeHits0 [ 1 ] );
					gl . Vertex ( EdgeHits1 [ 0 ] );
				}

				if ( SelfEdgeHits )
				{
					gl . LineWidth ( 1 );
					gl . Color ( 0.9 , 0.9 , 0.9 );
					gl . Vertex ( EdgeHits1 [ 0 ] );
					gl . Vertex ( EdgeHits1 [ 1 ] );
				}

				if ( CrossEdgeHits )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 0.9 , 0.7 , 0.8 );
					gl . Vertex ( EdgeHits1 [ 1 ] );
					gl . Vertex ( EdgeHits2 [ 0 ] );
				}

				if ( SelfEdgeHits )
				{
					gl . LineWidth ( 1 );
					gl . Color ( 0.9 , 0.9 , 0.9 );
					gl . Vertex ( EdgeHits2 [ 0 ] );
					gl . Vertex ( EdgeHits2 [ 1 ] );
				}

				if ( CrossEdgeHits )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 0.9 , 0.7 , 0.8 );
					gl . Vertex ( EdgeHits2 [ 1 ] );
					gl . Vertex ( EdgeHits3 [ 0 ] );
				}

				if ( SelfEdgeHits )
				{
					gl . LineWidth ( 1 );
					gl . Color ( 0.9 , 0.9 , 0.9 );
					gl . Vertex ( EdgeHits3 [ 0 ] );
					gl . Vertex ( EdgeHits3 [ 1 ] );
				}

				if ( CrossEdgeHits )
				{
					gl . LineWidth ( 3 );
					gl . Color ( 0.9 , 0.1 , 0.1 );
					gl . Vertex ( EdgeHits3 [ 1 ] );
					gl . Vertex ( EdgeHits0 [ 0 ] );
				}
				gl . End ( );
				gl . PopAttrib ( );
			}

		}

		private float[][] DrawOnEdges ( OpenGL gl, Edge E , float v0 , float v1 )
		{
			float[][] EdgeHits=new float[][]{
				DrawOnEdge ( gl, E , v0 ),
				DrawOnEdge ( gl, E , v1 )};
			float [ ] DistanceAongEdgeCycle = 
				{ 
					DistanceAlongEdgeCycle ( E . V [ 0 ] . cf , EdgeHits [ 0 ] ) , 
					DistanceAlongEdgeCycle ( E . V [ 0 ] . cf , EdgeHits [ 1 ] ) 
				};
				Boolean OutOfOrder= DistanceAongEdgeCycle [ 0 ] > DistanceAongEdgeCycle [ 1 ];
				if ( OutOfOrder )
				{
				float[] SwapperRoonie= EdgeHits[1];
				EdgeHits[1]=EdgeHits[0];
				EdgeHits[0]= SwapperRoonie;

				}
			return EdgeHits;
		}

		private float DistanceAlongEdgeCycle ( float [ ] x0 , float [ ] x1 )
		{
			double dsquaredx = ( x1 [ 0 ] - x0 [ 0 ] ) * ( x1 [ 0 ] - x0 [ 0 ] );
			double dsquaredy = ( x1 [ 1 ] - x0 [ 1 ] ) * ( x1 [ 1 ] - x0 [ 1 ] );
			double dsquaredZ = ( x1 [ 2 ] - x0 [ 2 ] ) * ( x1 [ 2 ] - x0 [ 2 ] );
			double dsquared = dsquaredx + dsquaredy + dsquaredZ;

			float d = ( float ) Math . Sqrt ( dsquared );
			return d;
		}

		private float[] DrawOnEdge ( OpenGL gl, Edge E , float v0 )
		{
			float[] EdgeHit  = CaculateEdgeHit ( E,v0);

			if(false)
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . Color ( 0.5 , 0.6 , 0.8 );
				float [ ] cf0 = E . V [ 0 ] . cf;
				float [ ] cf1 = E . V [ 1 ] . cf;
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , cf0 [ 2 ] );
				gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , cf1 [ 2 ] );
				gl . End ( );
				gl.PopAttrib();
				}
			if(true)
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( 0.8 , 0.7, 0.8 );
				gl . PointSize(4)	 ;
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex (EdgeHit );
				gl . End ( );
				gl . PopAttrib ( );

			}
			return EdgeHit;

		}

		public float[] CaculateEdgeHit(Edge E, float v)
		{
			float u = ( v - E . V [ 0 ] . V ) / ( E . V [ 1 ] . V - E . V [ 0 ] . V );
			float [ ] EdgeHit =  
			{
					( 1 - u ) * E . V[1] . cf [0] + ( u * E . V[0] . cf[0] ),
					( 1 - u ) * E . V[1] . cf [1] + ( u * E . V[0] . cf[1] ),
					( 1 - u ) * E . V[1] . cf [2] + ( u * E . V[0] . cf[2] ),    
					};

			return EdgeHit;

		}

		private void DrawCriticalDiamond ( )
		{
			OpenGL gl = this . MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}
			DrawCriticalDiamond(gl);

		}

		private void DrawCriticalDiamond(OpenGL gl)
		{
			float [][][] EdgeHits=new float [4][][];
			for ( int i = 0 ; i < 4 ; i++ )
			{	
				Edge E = this.E[i];
				EdgeHits[i]  = new float [ ] [ ]
					{
					CaculateEdgeHit ( E , this . Penult_Low_V ),
					CaculateEdgeHit ( E , this . Penult_High_V )	 ,
					CaculateEdgeHit ( E , this.Critical_Mean_V ),
					};
			}
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			if ( false )
			{
				gl . Color ( 0.8 , 0.1 , 0.1 );
				gl . PointSize ( 4 );
				gl . LineWidth ( 1 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
				for ( int i = 0 ; i < 4 ; i++ )
				{
					gl . Vertex ( EdgeHits [ i ] [ 0 ] );
				}
				gl . End ( );
			}
			if ( false )
			{
				gl . Color ( 0.1 , 0.1 , 0.8 );
				gl . PointSize ( 1 );
				gl . LineWidth ( width: 1 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
				for ( int i = 0 ; i < 4 ; i++ )
				{
					gl . Vertex ( EdgeHits [ i ] [ 1 ] );
				}
				gl . End ( );
			}
			if ( true )
			{
			gl . LineWidth ( width: 3 );
			gl . Color ( .1 , 1 , .1 );
			gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
			for ( int i = 0 ; i < 4 ; i++ )
			{
				gl . Vertex ( EdgeHits [ i ] [ 2 ] );
			}
			gl . End ( );
			}
		gl . PopAttrib ( );
		}

		public void AnnotateEdge ( OpenGL gl , Edge E )
		{
			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float [ ] cf = Midpoint ( cf0 , cf1 );
			float [ ] ce = this . Centroid . cf;
			float [ ] cd = Midpoint ( cf , ce );
			float [ ] cef0 = Midpoint ( cf0 , cd );
			float [ ] cef1 = Midpoint ( cf1 , cd );
			const float ts = 0.4f;
			const string f0 = "Arial";
			//String f1 = "Courrier New";
			String EdgeVertex_0_txt0 = String . Format ( "{0}" , E . V [ 0 ] . V . ToString ( "0.00" ) );
			String EdgeVertex_1_txt1 = String . Format ( "{0}" , E . V [ 1 ] . V . ToString ( "0.00" ) );
			String EdgeDelta_txt2 = String . Format ( "{0}" , E . Delta_V . ToString ( "0.00" ) );
			String txt3 = String . Format ( "{0}" , E . EdgeIndex . ToString ( "0" ) );
			String txt4 = String . Format ( "[{0},{1}]" , this . I . ToString ( "#0" ) , this . J . ToString ( "#0" ) );
			if ( this . MW . HackCheckBox_C1_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( )	 ;
				gl.Color(.5,.2,.2);
				gl . Translate ( cef0 [ 0 ] , cef0 [ 1 ] , cef0 [ 2 ] );
				gl . Scale ( .3 , -.3 , .3 );
				gl . Translate ( -.5 , -.5 , 0 );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: EdgeVertex_0_txt0 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C2_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Color ( .6 , .3 , .3 );
				gl . Translate ( cd [ 0 ] , cd [ 1 ] , cd [ 2 ] );
				gl . Scale ( .3 , -.3 , .3 );
				gl . Translate ( -.5 , -.5 , 0 );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: EdgeDelta_txt2 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C3_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf1 [ 0 ] , cf1 [ 1 ] , cd [ 2 ] );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: EdgeVertex_1_txt1 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C4_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( x: cd [ 0 ] , cd [ 1 ] , cd [ 2 ] );
				gl . Translate ( x: -.1 , y: .1 , z: 0 );
				gl . Scale ( x: ts , y: -ts , z: ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt3 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C5_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( x: ce [ 0 ] , ce [ 1 ] , ce [ 2 ] );
				gl . Translate ( x: -.4 , y: .1 , z: 0 );
				gl . Scale ( x: .25 , y: -.3 , z: .3 );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt4 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C6_R1_CheckBox_Control . IsChecked . Value )
			{

				if ( E . V [ 0 ] . V > E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef0 [ 0 ] , cef0 [ 1 ] , cef0 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: .35f , y: -0.35f , z: 0.35f );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "+" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
				if ( E . V [ 0 ] . V < E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef1 [ 0 ] , cef1 [ 1 ] , cef1 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: .35f , y: -0.35f , z: 0.35f );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "+" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
				if ( E . V [ 0 ] . V == E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef1 [ 0 ] , cef1 [ 1 ] , cef1 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: ts , y: -ts , z: ts );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "=" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
			}
			if ( this . MW . HackCheckBox_C7_R1_CheckBox_Control . IsChecked . Value )
			{

				if ( E . V [ 0 ] . V < E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef0 [ 0 ] , cef0 [ 1 ] , cef0 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: ts , y: -ts , z: ts );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "-" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
				if ( E . V [ 0 ] . V > E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef1 [ 0 ] , cef1 [ 1 ] , cef1 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: ts , y: -ts , z: ts );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "-" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
				if ( E . V [ 0 ] . V == E . V [ 1 ] . V )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . PushMatrix ( );
					gl . Translate ( x: cef1 [ 0 ] , cef1 [ 1 ] , cef1 [ 2 ] );
					gl . Translate ( x: -.1 , y: .1 , z: 0 );
					gl . Scale ( x: ts , y: -ts , z: ts );
					gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: "=" );
					gl . PopMatrix ( );
					gl . PopAttrib ( );
				}
			}
			if ( this . MW . HackCheckBox_C8_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PointSize ( 5 );
				gl . Color ( .99f , .85f , .95f );
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( cf0 );
				gl . Vertex ( cf1 );
				gl . Vertex ( cf );
				gl . Vertex ( ce );
				gl . Vertex ( cef0 );
				gl . Vertex ( cef1 );
				gl . End ( );
				gl . PopAttrib ( );
			}
			if ( this . MW . HackCheckBox_C9_R1_CheckBox_Control . IsChecked . Value )
			{
				ShorterLine ( gl , cf , ce , .2f );
				ShorterLine ( gl , cf0 , cd , .2f );
				ShorterLine ( gl , cf1 , cd , .2f );
			}

		}

		public void SmallerBoxAbout(OpenGL gl, float f)
		{
		float []	  Centroid=this.Centroid.cf;
		float [][]Spokes=new float[4][]{this.V[0].cf, this.V[1].cf, this.V[2].cf,this.V[3].cf};

		float [] [ ] [ ] Shorty = new float[4][][]{
				ShorterLine ( Centroid , Spokes[0] , f ),
				ShorterLine ( Centroid , Spokes[1] , f ),
				ShorterLine ( Centroid , Spokes[2] , f ),
				ShorterLine ( Centroid , Spokes[3] , f ),};

			if ( true )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . Color ( .4 , .4 , .4 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
				gl . Vertex ( Shorty [ 0 ] [1] );
				gl . Vertex ( Shorty [ 1 ] [ 1 ] );
				gl . Vertex ( Shorty [ 2 ] [ 1 ] );
				gl . Vertex ( Shorty [ 3 ] [ 1 ] );
				gl . End ( );
				gl . PopAttrib ( );
			}

		}

		public void ShorterLine ( OpenGL gl , float [ ] a , float [ ] b , float f )
		{
			float [ ] [ ] Shorten0 = ShorterLine ( a , b , f );
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . Color ( .4 , .4 , .4 );
			if(false)
				{
				gl.LineWidth(1);
				gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
				gl . Vertex ( a );
				gl . Vertex ( b );
				gl . End ( );	
				}
			gl . LineWidth (1 );
			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( Shorten0 [ 0 ] );
			gl . Vertex ( Shorten0 [ 1 ] );
			gl . End ( );
			gl . PopAttrib ( );
		} 

		public float [ ][] ShorterLine ( float [ ] p0 , float [ ] p1,float fract)
		{
			float [ ] v1 = subtract ( p0 , p1 );
			float [ ] v2 = normalie ( v1 );
			float [ ] v3 = Multiply ( v2 , fract );
			float [ ] p3 = p0;
			float [ ] p4 = add ( v3 , p0 );
			float [ ] [ ] pee = new float [ 2 ] [ ] { p3 , p4 };
			return pee;
		}

		public float [ ] normalie ( float [ ] v1 )
		{
			float mag = magnitue ( v1 );
			float [ ] norm = divide ( v1 , mag );
			return norm;
		}

		public float [ ] divide ( float [ ] v1 , float mag )
		{
			return new float [ ] { v1 [ 0 ] / mag , v1 [ 1 ] / mag , v1 [ 2 ] / mag };
		}

		public float magnitue ( float [ ] v )
		{
			float mag_xqured = ( v [ 0 ] * v [ 0 ] ) + v [ 1 ] * v [ 1 ] + v [ 2 ] * v [ 2 ];
			return ( float ) Math . Sqrt ( mag_xqured );
		}

		public float [ ] Midpoint ( float [ ] a , float [ ] b )
		{
			return Multiply ( Add ( a , b ) , 0.5f );
				}

		public String Comparis ( float v0 , float v1 )
		{
			if ( v0 < v1 )
			{
				return "<";
			}

			if ( v0 == v1 )
			{
				return "==";
			}

			if ( v0 > v1 )
			{
				return ">";
			}

			return "??";
		}

		public void DrawPerimeterEdge ( Edge e )
		{
			OpenGL gl = MainWindow . staticGLHook;
			if ( gl == null )
			{
				return;
			}

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . AccumBuffer );
			gl . LineWidth ( 2 );
			gl . Color ( 0.1 , 0.0 , 0.8 );
			float [ ] cf0 = e . V [ 0 ] . cf;
			float [ ] cf1 = e . V [ 1 ] . cf;
			float z0 = e . V [ 0 ] . V;
			float z1 = e . V [ 1 ] . V;
			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );
			gl . PopAttrib ( );
		}

		public bool IsBetween ( int v , int e )
		{
			Vertex V = this . V [ v ];
			Edge E = this . E [ e ];
			return IsBetween ( V , E );
		}

		public bool IsBetween ( Vertex v , Edge e )
		{
			float V = v . V;
			float V0 = e . V [ 0 ] . V;
			float V1 = e . V [ 1 ] . V;
			return IsBetween ( V0 , V , V1 );
		}

		public float [ ] toFloatArray ( GlmNet . vec3 l )
		{
			float [ ] f = new float [ 3 ] { l . x , l . y , l . z };
			return f;
		}

		public static String EncodeBoxelAdjacency ( Boxel Start , Boxel Neighboor )
		{
			int deltaI = Neighboor . I - Start . I;
			int deltaJ = Neighboor . J - Start . J;
			String S = String . Format ( "dB[{0},{1}]" , deltaI . ToString ( "0" ) , deltaJ . ToString ( "0" ) );
			return S;
		}

		public static void BoxelScanStart ( Boxel StartingBoxel )
		{
			for ( int i = 0 ; i < 4 ; i++ )
			{
				Edge StartingEdge = StartingBoxel . E [ i ];
				if ( StartingEdge . Hit == null )
				{
					continue;
				}
				BoxelScan ( StartingEdge , 0 );
			}
		}

		private static void BoxelScan ( Edge ThisBoxelEntryEdge , int Depth )
		{
			if ( ThisBoxelEntryEdge == null )
				return;
			if ( Depth > 100 )
				return;
			Edge ExitEdge = Edge . GetExitEdge ( ThisBoxelEntryEdge );

			if ( false )
				Edge . AnnotateRecursion ( ThisBoxelEntryEdge , ExitEdge , Depth );
			Boxel AdjacentBoxel = GetAdjacentBoxelToEdge ( ExitEdge , out Edge AdjacentBoxelEntryEdge , Depth );
			ThisBoxelEntryEdge . AdjacentBoxel = AdjacentBoxel;
			if ( true )
				Edge . AnnotateRecursion ( ThisBoxelEntryEdge , AdjacentBoxelEntryEdge , Depth );
			if ( ThisBoxelEntryEdge . ParentBoxel . BreadCrumbByteLabel [ 0 ] == 0b0 )
			{
				Region.Selected_Region . Labels [ 0 ]++;
				ThisBoxelEntryEdge . ParentBoxel . BreadCrumbByteLabel [ 0 ] = Region . Selected_Region . Labels [ 0 ];
			}
			else
				return;

			BoxelScan ( AdjacentBoxelEntryEdge , Depth + 1 );

		}

		public static Boxel GetAdjacentBoxelToEdge ( Edge ThisEdge , out Edge AdjacentBoxelEntryEdge , int Depth )
		{

			if ( ThisEdge == null )
			{
				AdjacentBoxelEntryEdge = null;
				return null;
			}

			Boxel ThisBoxel = ThisEdge . ParentBoxel;
			Boxel [ ] AdjacentBoxelArray = new Boxel [ 4 ];
			int [ , ] AdjacentIndexArray = new int [ 4 , 2 ] {
					{ ThisBoxel . I + 1 ,   ThisBoxel . J     } ,
					{ ThisBoxel . I ,       ThisBoxel . J + 1 } ,
					{ ThisBoxel . I - 1 ,   ThisBoxel . J     } ,
					{ ThisBoxel . I ,       ThisBoxel . J - 1 } ,
					};
			for ( int i = 0 ; i < 4 ; i++ )
			{
				int i1Len = Region.Selected_Region . B . GetLength ( 0 );
				int j1Len = Region . Selected_Region . B . GetLength ( 1 );
				int I1 = AdjacentIndexArray [ i , 0 ];
				int J1 = AdjacentIndexArray [ i , 1 ];
				Boolean GutsInBounds = ( ( I1 >= 0 ) && ( I1 < i1Len ) && ( J1 >= 0 ) && ( J1 < j1Len ) );
				Boolean GutsGood = ( GutsInBounds && ( ( Region . Selected_Region . B [ I1 , J1 ] ) != null ) );
				int i2Len = Region . Selected_Region . BorderB . GetLength ( 0 );
				int j2Len = Region . Selected_Region . BorderB . GetLength ( 1 );
				int I2 = AdjacentIndexArray [ i , 0 ];
				int J2 = AdjacentIndexArray [ i , 1 ];
				Boolean BorderInBounds = ( ( I2 >= 0 ) && ( I2 < i2Len ) && ( J2 >= 0 ) && ( J2 < j2Len ) );
				Boolean BorderGood = ( BorderInBounds && ( Region . Selected_Region . BorderB [ AdjacentIndexArray [ i , 0 ] , AdjacentIndexArray [ i , 1 ] ] ) != null );

				if ( BorderGood )
				{
					AdjacentBoxelArray [ i ] = Region . Selected_Region . BorderB [ AdjacentIndexArray [ i , 0 ] , AdjacentIndexArray [ i , 1 ] ];
				}
				else if ( GutsGood )
				{
					AdjacentBoxelArray [ i ] = Region . Selected_Region . B [ AdjacentIndexArray [ i , 0 ] , AdjacentIndexArray [ i , 1 ] ];
				}
				else
				{
					AdjacentBoxelArray [ i ] = null;
				}
			}
			Boxel AdjacentBoxel = null;
			AdjacentBoxelEntryEdge = null;
			for ( int i = 0 ; i < 4 ; i++ )
			{
				if ( AdjacentBoxelArray [ i ] == null )
				{
					continue;
				}
				AdjacentBoxel = AdjacentBoxelArray [ i ];
				if ( false )
				{
					System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}:{1}:  B[{2},{3}] {4} has {5} hits. " ,
						Depth , i ,
						AdjacentBoxel . I , AdjacentBoxel . J , Boxel . EncodeBoxelAdjacency ( ThisBoxel , AdjacentBoxel ) , AdjacentBoxel . EdgeHits ,
						 ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				}
				for ( int j = 0 ; j < 4 ; j++ )
				{
					if ( AdjacentBoxel . E [ j ] == null )
						continue;
					Edge AdjacentBoxelEdge = AdjacentBoxel . E [ j ];
					Boolean EdgeMatch = Edge . Equals ( ThisEdge , AdjacentBoxelEdge );
					if ( false )
					{
						System . Diagnostics . Debug . WriteLine ( String . Format ( "{0}:{1}:{2}:B[{3},{4}*E{5}]={6}=B[{7},{8}*E{9}]" ,
							Depth , i , j ,
							ThisEdge . ParentBoxel . I , ThisEdge . ParentBoxel . J , ThisEdge . EdgeIndex ,
							EdgeMatch ,
							AdjacentBoxel . I , AdjacentBoxel . J , AdjacentBoxelEdge . EdgeIndex ,
							( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
					}
					if ( AdjacentBoxelEdge . Hit == null )
					{
						continue;
					}
					Boolean HitMatch = ( Edge . Equals ( AdjacentBoxelEdge . Hit , ThisEdge . Hit ) );
					if ( HitMatch )
					{
						ThisEdge . AdjacentBoxel = AdjacentBoxel;
						AdjacentBoxelEntryEdge = AdjacentBoxelEdge;
						goto bailout;
					}
				}
			}
bailout:
			return AdjacentBoxel;
		}

	}
}