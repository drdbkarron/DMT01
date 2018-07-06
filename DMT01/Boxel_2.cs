using System;
using System .Linq;
using System .Collections .Generic;
using SharpGL;
using GlmSharp;
using WpfControlLibrary1;
using GlmNet;
using System .IO;

namespace DMT01
{
	public class Boxel
		{
		public DMT01.MainWindow MW;
		public int I;
		public int J;
		public Vertex [ ] V = new Vertex [ 4 ];
		public Vertex Centroid = new Vertex ( );
		public short Span=1;
		public short[]  SortedVertexIndicies={ 0 , 1 , 2 , 3 };
		public short LargestVertex, SmallestVertex;
		public Edge [ ] E = new Edge [ 4 ];
		public IsoEdge [ ] IsoEdge = new IsoEdge [ 4 ];
		public short IsoEdges ;
		public Boolean IsCritical;
		public float delta_v;
		public float max_V;
		public float min_V;

		public Boxel ( int i , int j , float [ , ] c )
			{
			this . I = i;
			this . J = j;
			this . V [ 0 ] = new Vertex ( 0 , i , j , c );
			this . V [ 1 ] = new Vertex ( 1 , i , j + this . Span , c );
			this . V [ 2 ] = new Vertex ( 2 , i + this . Span , j + this . Span , c );
			this . V [ 3 ] = new Vertex ( 3 , i + this . Span , j , c );

			this . V [ 0 ] . Next = this . V [ 1 ];
			this . V [ 1 ] . Next = this . V [ 2 ];
			this . V [ 2 ] . Next = this . V [ 3 ];
			this . V [ 3 ] . Next = this . V [ 0 ];

			this . V [ 0 ] . Previous = this . V [ 3 ];
			this . V [ 1 ] . Previous = this . V [ 0 ];
			this . V [ 2 ] . Previous = this . V [ 1 ];
			this . V [ 3 ] . Previous = this . V [ 2 ];

			this . Centroid = LoadCentroid ( );

			this . E [ 0 ] = new Edge ( 0 , this . V [ 0 ] , this . V [ 1 ] );
			this . E [ 1 ] = new Edge ( 1 , this . V [ 1 ] , this . V [ 2 ] );
			this . E [ 2 ] = new Edge ( 2 , this . V [ 2 ] , this . V [ 3 ] );
			this . E [ 3 ] = new Edge ( 3 , this . V [ 3 ] , this . V [ 0 ] );

			this . E [ 0 ] . Next = this . E [ 1 ];
			this . E [ 1 ] . Next = this . E [ 2 ];
			this . E [ 2 ] . Next = this . E [ 3 ];
			this . E [ 3 ] . Next = this . E [ 0 ];

			this . E [ 0 ] . Previous = this . E [ 3 ];
			this . E [ 1 ] . Previous = this . E [ 0 ];
			this . E [ 2 ] . Previous = this . E [ 1 ];
			this . E [ 3 ] . Previous = this . E [ 2 ];

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

			//            this. LoadTriangles ( );
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
			String strang = String . Format ( "[i{0},j{1}] {2}{3}{4}" , this . I , this . J ,
				this . min_V . ToString ( Stranger ) , Comparis ( this . min_V , this . max_V ) , this . max_V . ToString ( Stranger ) );
			return strang;
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

		private bool IsInBetween ( float v , Edge e )
			{
			float v0 = e . V [ 0 ] . V;
			float v1 = e . V [ 1 ] . V;

			return IsInBetween ( v0 , v , v1 );
			}

		private void DoMinMaxDelta ( )
			{
			this . max_V = -float . MaxValue;
			this . min_V = float . MaxValue;

			for ( int i = 0 ; i < 4 ; i++ )
				{
				Vertex V = this . V [ i ];
				float v = V . V;

				if ( this . max_V < v )
					{
					this . max_V = v;
					}

				if ( v < this . min_V )
					{
					this . min_V = v;
					}
				}
			this . delta_v = this . max_V - this . min_V;

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} {3} " , DisplayMe(), this.min_V, Comparis(this.min_V, this.max_V) , this.max_V, ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}

		private void IsThisCritical ( )
			{
			Boolean wiggle0 = (
				 this . E [ 0 ] . delta_V > 0 &&
				 this . E [ 1 ] . delta_V < 0 &&
				 this . E [ 2 ] . delta_V > 0 &&
				 this . E [ 3 ] . delta_V < 0 );

			if ( wiggle0 )
				{
				this . IsCritical = true;

				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				return;
				}

			Boolean wiggle1 = (
				 this . E [ 0 ] . delta_V < 0 &&
				 this . E [ 1 ] . delta_V > 0 &&
				 this . E [ 2 ] . delta_V < 0 &&
				 this . E [ 3 ] . delta_V > 0 );

			if ( wiggle1 )
				{
				this . IsCritical = true;
				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				return;
				}
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
				Edge SubEdge0 = new Edge ( E . EdgeIndex , 0 , V0 , V );
				Edge SubEdge1 = new Edge ( E . EdgeIndex , 1 , V , V1 );
				V . Previous = V0;
				V . Next = V1;
				E . SubEdgeCount = 2;
				E . SubEdge = new Edge [ 2 ] { SubEdge0 , SubEdge1 };
				SubEdge0 . Previous = E . Previous;
				SubEdge0 . IsSubEdge = true;
				SubEdge0 . Parent = E;
				SubEdge0 . Next = E . SubEdge [ 1 ];
				SubEdge1 . Previous = E . SubEdge [ 0 ];
				SubEdge1 . IsSubEdge = true;
				SubEdge1 . Parent = E;
				SubEdge1 . Next = E . Next;

				return;
				}
			else
			if ( E . TweenVerts == 2 )
				{
				Vertex V00 = E . TweenVerte [ 0 ];
				Edge SubEdge0 = new Edge ( E . EdgeIndex , 0 , V0 , V00 );
				Vertex V01 = E . TweenVerte [ 1 ];
				V00 . Previous = V0;
				V00 . Next = V01;
				V01 . Previous = V00;
				V01 . Next = V1;
				Edge SubEdge1 = new Edge ( E . EdgeIndex , 1 , V00 , V01 );
				Edge SubEdge2 = new Edge ( E . EdgeIndex , 2 , V01 , V1 );
				E . SubEdgeCount = 3;
				E . SubEdge = new Edge [ 3 ] { SubEdge0 , SubEdge1 , SubEdge2 };

				SubEdge0 . Previous = E . Previous;
				SubEdge0 . Parent = E;
				SubEdge0 . IsSubEdge = true;
				SubEdge0 . Next = SubEdge1;
				SubEdge1 . Previous = SubEdge0;
				SubEdge1 . Parent = E;
				SubEdge1 . IsSubEdge = true;
				SubEdge1 . Next = SubEdge2;
				SubEdge2 . Previous = SubEdge1;
				SubEdge2 . Parent = E;
				SubEdge2 . IsSubEdge = true;
				SubEdge2 . Next = E . Next;

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
						float [ ] cf = OnEdge ( v , e );
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

		private float [ ] OnEdge ( int v , int e )
			{
			Vertex V = this . V [ this.SortedVertexIndicies [ v ] ];
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
			float dx = x0 + (linageFraction *  deltax0x1 );
			float dy = y0 + (linageFraction *  deltay0y1 );
			float dz = z0 + (linageFraction *  deltaz0z1 );
			float [ ] c = { dx , dy , dz };
			return c;
			}

		private bool IsInBetween ( int v , int e )
			{
			int S = this.SortedVertexIndicies [ v ];
			Vertex V = this . V [ S ];
			Edge E = this . E [ e ];
			return IsInBetween ( V , E );
			}

		private bool IsInBetween ( Vertex v , Edge e )
			{
			return IsInBetween ( e . V [ 0 ] . V , v . V , e . V [ 1 ] . V );
			}

		private bool IsInBetween ( float v1 , float tweener , float v3 )
			{
			if ( v1 == tweener )
				{
				}
			if ( tweener == v3 )
				{
				}
			if ( v1 == v3 )
				{
				}
			if ( v1 < v3 )     // starting small moving up and rising from v2 to v3
				{
				bool vee = ( v1 <= tweener );
				if ( vee )
					{
					bool bee = tweener <= v3;
					if ( bee )
						{
						return true;
						}
					}
				}
			if ( v3 < v1 )     //   starting large, moving down and lowering from v3 to v1
				{
				bool booley = IsInBetween ( v3 , tweener , v1 );
				return booley;
				}
			return false;
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

			this . LargestVertex = this . SortedVertexIndicies [ 3 ];
			this . SmallestVertex = this . SortedVertexIndicies [ 0 ];

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , Sortee(SmallestVertex,LargestVertex) , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			}

		private String Sortee ( short smallestVertex , short largestVertex )
			{
			float v0 = this . V [ smallestVertex ] . V;
			float v1 = this . V [ largestVertex ] . V;
			const String strang_format = "0.000";
			String Strang = String . Format ( "{0}{1}{2}" , v0 . ToString ( strang_format ) , Comparis ( v0 , v1 ) , v1 . ToString ( strang_format ) );

			return Strang;
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

		internal void DrawMe ( )
			{

			DrawBoxelEdgeCycles ( );

			DrawCriticalBoxel ( );
			AnnotateBoxelAtCentroid ( );

			//VerifyCycles ( );
			}

		private void DrawCriticalBoxel ( )
		{

			if ( this .MW == null )
			{
				return;
			}

			OpenGL gl = this .MW .myOpenGLControl .OpenGL;
			if ( gl == null )
			{
				return;
			}

			Edge E0 = this .E [ 0 ];
			Edge E1 = this .E [ 1 ];
			Edge E2 = this .E [ 2 ];
			Edge E3 = this .E [ 3 ];

			AnnotateEdge ( gl , E0 );
			AnnotateEdge ( gl , E1 );
			AnnotateEdge ( gl , E2 );
			AnnotateEdge ( gl , E3 );

			float v = this .MW .region_threshold_H_Slider_UserControl .SliderValue;
			Boolean IsClearedAll=	( IsInBetween ( v , E0 ) && 
									IsInBetween ( v , E1 ) && 
									IsInBetween ( v , E2 ) && 
									IsInBetween ( v , E3 ) )
			if(!IsClearedAll)
			{
				return;
			}

			if ( IsInBetween ( v , E0 ) 
			{ gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .LineWidth ( 3 );
				gl .PointSize ( 5 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Vertex ( E0 .V [ 0 ] .cf );
				gl .Vertex ( E1 .V [ 1 ] .cf );
				gl .End ( );
				gl .PopAttrib ( ); }

			float x0 = cf0 [ 0 ];
			float y0 = cf0 [ 1 ];
			float z0 = cf0 [ 2 ];
			float v0 = E .V [ 0 ] .V;

			float x1 = cf1 [ 0 ];
			float y1 = cf1 [ 1 ];
			float z1 = cf1 [ 2 ];
			float v1 = E .V [ 1 ] .V;

			float u = ( v - v0 ) / ( v1 - v0 );
			float x = ( ( 1 - u ) * x0 ) + ( u * x1 );
			float y = ( ( 1 - u ) * y0 ) + ( u * y1 );
			float z = ( ( 1 - u ) * z0 ) + ( u * z1 );



			

		}

		private void AnnotateBoxelAtCentroid ( )
			{
			if ( this . MW == null )
				{
				return;
				}

			OpenGL gl = this.MW . myOpenGLControl . OpenGL;
			if ( gl == null )
				{
				return;
				}

			if ( this . MW . HackCheckBox_C10_R1_CheckBox_Control . IsChecked . Value )
				{
				const float ts = 0.3f;
				const string f0 = "Arial";
				String txt0 = String . Format ( "[{0},{1}]" , this.I, this.J );

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

			if ( this . MW . HackCheckBox_C9_R1_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( .9 , .2 , .2 );
				gl . PointSize ( 7 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( this . Centroid . cf );
				gl . End ( );
				gl . PopAttrib ( );
				}

			if ( this . MW . HackCheckBox_C8_R1_CheckBox_Control . IsChecked . Value )
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

			if ( this . MW . HackCheckBox_C1_R2_CheckBox_Control . IsChecked . Value )
				{
				float knobby = this.MW.CentroidToCornerLengthFraction_H_Slider_UserControl .SliderValue;
				if ( knobby < 0 )
					{
					return;
					}

				if ( knobby > 1 )
					{
					return;
					}

				LineStinkerModes Stankey=this.MW.lineStinkerModeenumcomboBoxUserControl.Mode;

				float [ ] [ ] [ ] Stinkers = new float [ 4 ] [ ] [ ];
				if (Stinkers == null)
					return;
				Stinkers [ 0 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 0 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 0 ] == null )
					return;
				Stinkers [ 1 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 1 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [ 1 ] == null )
					return;
				Stinkers [ 2 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 2 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [2 ] == null )
					return;
				Stinkers [ 3 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 3 ] . cf ,
					fraction: knobby , mode: Stankey );
				if ( Stinkers [3  ] == null )
					return;

				if ( this .MW .HackCheckBox_C2_R2_CheckBox_Control .IsChecked .Value )
				{
					gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
					gl .Color ( 1 , .2 , .9 );
					gl .LineWidth ( 1 );
					gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );

					gl .Vertex ( x: Stinkers [ 0 ] [ 0 ] [ 0 ] , y: Stinkers [ 0 ] [ 0 ] [ 1 ] , z: Stinkers [ 0 ] [ 0 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 0 ] [ 1 ] [ 0 ] , y: Stinkers [ 0 ] [ 1 ] [ 1 ] , z: Stinkers [ 0 ] [ 1 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 1 ] [ 0 ] [ 0 ] , y: Stinkers [ 1 ] [ 0 ] [ 1 ] , z: Stinkers [ 1 ] [ 0 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 1 ] [ 1 ] [ 0 ] , y: Stinkers [ 1 ] [ 1 ] [ 1 ] , z: Stinkers [ 1 ] [ 1 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 2 ] [ 0 ] [ 0 ] , y: Stinkers [ 2 ] [ 0 ] [ 1 ] , z: Stinkers [ 2 ] [ 0 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 2 ] [ 1 ] [ 0 ] , y: Stinkers [ 2 ] [ 1 ] [ 1 ] , z: Stinkers [ 2 ] [ 1 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 3 ] [ 0 ] [ 0 ] , y: Stinkers [ 3 ] [ 0 ] [ 1 ] , z: Stinkers [ 3 ] [ 0 ] [ 2 ] );
					gl .Vertex ( x: Stinkers [ 3 ] [ 1 ] [ 0 ] , y: Stinkers [ 3 ] [ 1 ] [ 1 ] , z: Stinkers [ 3 ] [ 1 ] [ 2 ] );

					gl .End ( );
					gl .PopAttrib ( );
				}
			}
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

			float [ ][ ] stinkey_vertices = new float [ 2 ][];

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
					float[] midpoint=new float[3];
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

		private float[] GetCentroid ( float [ ] [ ] a )
			{
			return GetCentroid ( a [ 0 ] , a [ 1 ] );
			}

		private float [ ] GetCentroid ( float [ ] a , float [ ] b )
			{
			float[] c={ 0,0,0 };
			c = add ( a , b );
			float[] d={0,0,0};
			d = divide ( c , 2 );
			return d;
			}

		private float [ ] divide ( float [ ] c , int v )
			{
			float V=v;
			float[] a={
				c [0]/V ,
				c [1]/V,
				c [2]/V
				};
			return a;
			}

		private float [ ] add ( float [ ] a , float [ ] b )
			{
			float [] c={
				a [0]+b[0],
				a [1]+b[1],
				a [2]+b[2]
				};
			return c;
			}

		private float [ ] Divide ( float [ ] a , float m )
			{
			float [ ] d = { a [ 0 ] / m , a [ 1 ] / m , a [ 2 ] / m };
			return d;
			}

		private float magnitude ( float [ ] a )
			{
			double b = (a [ 0 ] * a [ 0 ]) + (a [ 1 ] * a [ 1 ]) + (a [ 2 ] * a [ 2 ]);
			float c = (float)Math . Sqrt ( b );
			return c;
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

		private String DisplayMe ( Edge e )
			{
			String fomont = String . Format ( "{0} {1}" , this . DisplayMe ( ) , e . DisplayMe ( ) );
			return fomont;
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

		private void VerifyEdgeCyclesPrevious ( )
			{
			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} " , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			Vertex Vlargest = this . V [ this . LargestVertex ];

			Edge E = Vlargest . PreviousEdge;
			Boolean ExitCondition = true;
			while ( ExitCondition )
				{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} v{1} {2}" , this . DisplayMe ( ) , E . EdgeIndex , E . IsSubEdge ) );

				if ( E . SubEdge == null )
					{
					DrawEdge ( E , Edge . EdgeDirection . CounterClockwisePrevious );

					E = E . Previous;
					int VI = E . V [ 1 ] . VertexIndex;
					ExitCondition = this . LargestVertex != VI;
					}
				else
					{
					int last_subedge = E . SubEdgeCount - 1;
					Edge SE = E . SubEdge [ last_subedge ];

					while ( SE . IsSubEdge )
						{
						DrawEdge ( SE , Edge . EdgeDirection . CounterClockwisePrevious );
						SE = SE . Previous;
						}
					ExitCondition = false;
					}
				}
			}

		private void VerifyEdgeCyclesNext ( )
			{
			int largest_index = this.SortedVertexIndicies [ 0 ];
			int smallest_index = this.SortedVertexIndicies [ 3 ];
			Vertex Vlargest = this . V [ largest_index ];
			Edge E = Vlargest . NextEdge;
			Boolean ExitCondition = true;
			while ( ExitCondition )
				{
				//System . Diagnostics . Debug . Write  ( String . Format ( "^{0} " , E.EdgeIndex ) );
				if ( E . SubEdge == null )
					{
					DrawEdge ( E , Edge . EdgeDirection . ClockwiseNext );
					E = E . Next;
					int VI = E . V [ 0 ] . VertexIndex;
					ExitCondition = smallest_index != VI;
					}
				else
					{
					Edge SE = E . SubEdge [ 0 ];

					while ( SE . IsSubEdge )
						{
						//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , SE.SubEdgeIndex , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

						DrawEdge ( SE , Edge . EdgeDirection . ClockwiseNext );
						SE = SE . Next;
						}
					ExitCondition = false;
					}
				}
			}

		protected void DebugDrawEdge ( Edge E )
		{
			if ( this .MW == null )
			{
				return;
			}

			OpenGL gl = this .MW .myOpenGLControl .OpenGL;
			if ( gl == null )
			{
				return;
			}

			if(true)AnnotateEdge ( gl , E );

			float [ ] cf0 = E .V [ 0 ] .cf;
			float [ ] cf1 = E .V [ 1 ] .cf;

			if ( this .MW .HackCheckBox_C4_R1_CheckBox_Control .IsChecked .Value )
			{
				float [ ] cf00 = E .V [ 0 ] .cf;
				float [ ] cf10 = E .V [ 1 ] .cf;

				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .LineWidth ( 1 );
				gl .Color ( 0.9 , 0.8 , 0.8 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Vertex ( cf00 );
				gl .Vertex ( cf10 );
				gl .End ( );
				gl .PopAttrib ( );
			}

			float [ ] cent = { this .Centroid .cf [ 0 ] , this .Centroid .cf [ 1 ] , this .Centroid .cf [ 2 ] };

			GlmNet .vec3 vf0 = new GlmNet .vec3 ( x: cf0 [ 0 ] , y: cf0 [ 1 ] , z: cf0 [ 2 ] );
			GlmNet .vec3 vf1 = new GlmNet .vec3 ( x: cf1 [ 0 ] , y: cf1 [ 1 ] , z: cf1 [ 2 ] );
			GlmNet .vec3 vce = new GlmNet .vec3 ( x: this .Centroid .cf [ 0 ] , y: this .Centroid .cf [ 1 ] , z: this .Centroid .cf [ 2 ] );

			float [ ] faaa = subtract ( cf0 , cent );
			GlmNet .vec3 cv = new GlmNet .vec3 ( x: faaa [ 0 ] , y: faaa [ 1 ] , z: faaa [ 2 ] );
			GlmNet .vec3 v3z_RotateAxis = new GlmNet .vec3 ( 0 , 0 , 1 );
			GlmNet .mat4 ID = GlmNet .mat4 .identity ( );
			GlmNet .vec3 ZV = new GlmNet .vec3 ( 0 , 0 , 0 );
			GlmNet .vec3 noon = new GlmNet .vec3 ( x: 0 , y: 1 , z: 0 );

			if ( this .MW .HackCheckBox_C9_R2_CheckBox_Control .IsChecked .Value )
			{
				float TwistyKnob = ( float ) ( this .MW .HighSideHalfArrowAngleOffCentroid_H_Slider_UserControl .SliderValue * Math .PI );
				float StreatchyKnob = this .MW .HighSideHalfArrowHeadLengthFraction_H_Slider_UserControl .SliderValue;
				Vertex Bigger, Smaller;
				Boolean ShowCentroid = false;
				if ( E .V [ 0 ] .V > E .V [ 1 ] .V )
				{
					Bigger = E .V [ 0 ];
					Smaller = E .V [ 1 ];
				}
				else if ( E .V [ 0 ] .V < E .V [ 1 ] .V )
				{
					Bigger = E .V [ 1 ];
					Smaller = E .V [ 0 ];
					TwistyKnob = -TwistyKnob;
				}
				else
				{
					return; // == flat edge
				}
							// p prefix fixed point
							// v prefix direction unfixed vector

				GlmNet .vec3 pcf0		= new GlmNet .vec3 ( x: Smaller.cf [ 0 ] , y: Smaller .cf [ 1 ] , z: Smaller .cf [ 2 ] );
				GlmNet .vec3 p3Bigger	= new GlmNet .vec3 ( x: Bigger .cf [ 0 ] , y: Bigger.cf[ 1 ] , z: Bigger.cf [ 2 ] );
				GlmNet .vec3 p3Centroid = new GlmNet .vec3 ( this .Centroid .cf [ 0 ] , this .Centroid .cf [ 1 ] , this .Centroid .cf [ 2 ] );

				GlmNet .mat4 m4rotator = GlmNet .glm .rotate ( ID , TwistyKnob , v3z_RotateAxis );
				GlmNet .mat3 m3rotator = m4rotator .to_mat3 ( );
				GlmNet .vec3 v3Centroid = p3Centroid - p3Bigger;
				GlmNet .vec3 v3twisted = m3rotator * v3Centroid;
				//GlmNet .vec3 twisty1 = GlmNet.mat3.identity() * v3Centroid;
				GlmNet .vec3 v3twistedStreatchey = v3twisted * StreatchyKnob;
				GlmNet .vec3 pt = v3twistedStreatchey + p3Bigger;
				
				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Color ( 0 , .8 , 1 );
				gl .LineWidth ( 2 );
				gl .Vertex ( Smaller .cf );
				gl .Vertex ( Bigger .cf );
				gl .Color ( .9 , .7 , .4 );
				gl .LineWidth ( 1 );
				gl .Vertex ( Bigger .cf );
				gl .Vertex ( x: pt.x, y: pt.y,z: pt.z );
				if ( ShowCentroid )
				{
					gl .Color ( .5 , .7 , .8 );
					gl .LineWidth ( 1 );
					gl .Vertex ( Bigger .cf );
					gl .Vertex ( this .Centroid .cf );
				}
				gl .Color ( .9 , .2 , .3 );
				gl .Vertex ( Bigger .cf );
				gl .Vertex ( pt.x, pt.y, pt.z);
				gl .End ( );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Points );
				gl .PointSize ( 4 );
				gl .Color (red: .9 , green: .1 , blue: .1 );
				gl .Vertex ( Bigger .cf );
				if ( ShowCentroid )
				{
					gl .PointSize ( 2 );
					gl .Vertex ( this .Centroid .cf );
				}
				gl .PointSize ( 6 );
				gl .Vertex ( pt .x , pt .y , pt .z );
				gl .End ( );
				gl .PopAttrib ( );

			}

			if ( this .MW .HackCheckBox_C7_R1_CheckBox_Control .IsChecked .Value )
			{
				RotationTest ( E , gl );
			}

			if ( this .MW .HackCheckBox_C5_R2_CheckBox_Control .IsChecked .Value )
			{
				float knob1 = ( float ) ( this .MW .Hack_H_Slider_01_UserControl .SliderValue * 2f * Math .PI );
				float knobl1 = this .MW .Hack_H_Slider_03_UserControl .SliderValue;
				GlmNet .mat4 d0q = GlmNet .glm .rotate ( ID , knob1 , v3z_RotateAxis );
				GlmNet .mat3 mm0 = d0q .to_mat3 ( );
				GlmNet .vec3 twisty1 = mm0 * cv;
				GlmNet .vec3 twisty22 = twisty1 * knobl1;
				GlmNet .vec3 located0 = twisty22 + vf0;
				float [ ] FA0 = toFloatArray ( located0 );

				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .Color ( 0 , .8 , 1 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Vertex ( cf0 );
				gl .Vertex ( FA0 );
				gl .End ( );
				gl .PopAttrib ( );
			}

			if ( this .MW .HackCheckBox_C6_R1_CheckBox_Control .IsChecked .Value )
			{
				float knob2 = ( float ) ( this .MW .Hack_H_Slider_02_UserControl .SliderValue * 2f * Math .PI );
				float knobl2 = this .MW .ArrowHeadSide2Length2_H_Slider_UserControl1 .SliderValue;
				GlmNet .mat4 d1q = GlmNet .glm .rotate ( ID , knob2 , v3z_RotateAxis );
				GlmNet .mat3 mm1 = d1q .to_mat3 ( );
				GlmNet .vec3 twisty2 = mm1 * cv;
				GlmNet .vec3 twisty21 = twisty2 * knobl2;
				GlmNet .vec3 located1 = twisty21 + vf0;  // this is now a fixed point in space
				float [ ] FA1 = toFloatArray ( located1 );

				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .Color ( 1 , .2 , .9 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Vertex ( cf0 );
				gl .Vertex ( FA1 );
				gl .End ( );
				gl .PopAttrib ( );
			}

			if ( this .MW .HackCheckBox_C5_R1_CheckBox_Control .IsChecked .Value )
			{
				float u = this .MW .Threshold_Hack_H_Slider_2_UserControl .SliderValue;
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

				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .LineWidth ( 1 );
				gl .PointSize ( 5 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Points );
				gl .Vertex ( c_th [ 0 ] , c_th [ 1 ] , c_th [ 2 ] );
				gl .End ( );
				gl .PopAttrib ( );
			}

			if ( this .MW .HackCheckBox_C3_R2_CheckBox_Control .IsChecked .Value )
			{
				float v = this .MW .region_threshold_H_Slider_UserControl .SliderValue;
				if ( !IsInBetween ( v , E ) )
				{
					return;
				}
				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .LineWidth ( 3 );
				gl .PointSize ( 5 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
				gl .Vertex ( E .V [ 0 ] .cf );
				gl .Vertex ( E .V [ 1 ] .cf );
				gl .End ( );
				gl .PopAttrib ( );

				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];
				float z0 = cf0 [ 2 ];
				float v0 = E .V [ 0 ] .V;

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];
				float z1 = cf1 [ 2 ];
				float v1 = E .V [ 1 ] .V;

				float u = ( v - v0 ) / ( v1 - v0 );
				float x = ( ( 1 - u ) * x0 ) + ( u * x1 );
				float y = ( ( 1 - u ) * y0 ) + ( u * y1 );
				float z = ( ( 1 - u ) * z0 ) + ( u * z1 );

				gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
				gl .LineWidth ( 1 );
				gl .PointSize ( 5 );
				gl .Color ( .3 , .5 , .8 );
				gl .Begin ( SharpGL .Enumerations .BeginMode .Points );
				gl .Vertex ( x , y , z );
				gl .End ( );
				gl .PopAttrib ( );
			}
		}

			private void RotationTest ( Edge E , OpenGL gl  )
		{
			float knob00 = this .MW .Z_Fudge_H_Slider_0_UserControl .SliderValue;
			float [ ] cf0 = E .V [ 0 ] .cf;
			float [ ] cf1 = E .V [ 1 ] .cf;
			float [ ] cent = { this .Centroid .cf [ 0 ] , this .Centroid .cf [ 1 ] , this .Centroid .cf [ 2 ] };

			GlmNet .vec3 vf0 = new GlmNet .vec3 ( x: cf0 [ 0 ] , y: cf0 [ 1 ] , z: cf0 [ 2 ] );
			GlmNet .vec3 vf1 = new GlmNet .vec3 ( x: cf1 [ 0 ] , y: cf1 [ 1 ] , z: cf1 [ 2 ] );
			GlmNet .vec3 vce = new GlmNet .vec3 ( x: cent [ 0 ] , y: cent [ 1 ] , z: cent [ 2 ] );

			float [ ] faaa = subtract ( cf0 , cent );
			GlmNet .vec3 cv = new GlmNet .vec3 ( x: faaa [ 0 ] , y: faaa [ 1 ] , z: faaa [ 2 ] );
			GlmNet .vec3 z_rot_axis = new GlmNet .vec3 ( 0 , 0 , 1 );
			GlmNet .mat4 ID = GlmNet .mat4 .identity ( );
			GlmNet .vec3 ZV = new GlmNet .vec3 ( 0 , 0 , 0 );
			GlmNet .vec3 noon = new GlmNet .vec3 ( x: 0 , y: 1 , z: 0 );

			double spinner2 = this .MW .Hack_H_Slider_04_UserControl .SliderValue * 2f * Math .PI;
			float spinner2f = ( float ) spinner2;
			GlmNet .mat4 rotation = GlmNet .glm .rotate ( ID , spinner2f , z_rot_axis );
			GlmNet .mat3 normalMatrix = rotation .to_mat3 ( );
			GlmNet .vec3 rotated_noon = normalMatrix * noon;
			float [ ] ccc = { rotated_noon .x + cent [ 0 ] , rotated_noon .y + cent [ 1 ] , rotated_noon .z + cent [ 2 ] };
			gl .PushAttrib ( SharpGL .Enumerations .AttributeMask .All );
			gl .Color ( 1 , .2 , .9 );
			gl .LineWidth ( 1 );
			gl .Begin ( SharpGL .Enumerations .BeginMode .Lines );
			gl .Vertex ( cent );
			gl .Vertex ( ccc );
			gl .End ( );
			gl .PopAttrib ( );
		}

		public float [ ] toFloatArray ( GlmNet .vec3 l )
		{
			float[] f=new float[3]{l.x,l.y,l.z };
			return f;
		}

		private dvec3 Normalize ( dvec3 dvec3 )
			{
			double Mag = Magnitude ( dvec3 );
			dvec3 nal = Divide ( Mag , dvec3 );
			return nal;
			}

		private dvec3 Divide ( double mag , dvec3 v )
			{
			dvec3 a = new dvec3 ( v . x / mag , v . y / mag , v . z / mag );
			return a;
			}

		private double Magnitude ( dvec3 v )
			{
			double m = (v . x * v . x) + (v . y * v . y) + v . z + v . z;
			double mr = Math . Sqrt ( m );
			return mr;
			}

		private float [ ] toFloatArray ( dvec3 dv )
			{
			float [ ] fa = { (float)dv . x , (float)dv . y , (float)dv . z};
			return fa;
			}

		private dvec3 add ( dvec3 r , float [ ] f )
			{
			dvec3 a = new dvec3 ( x: r [ 0 ] + f [ 0 ] , y: r [ 1 ] + f [ 1 ] , z: r [ 2 ] + f [ 2 ] );
			return a;
			}

		private double [ ] ToDoubleArray ( float [ ] v )
			{
			double [ ] d = { v [ 0 ] , v [ 1 ], v [ 2 ] };
			return d;
			}

		private float [ ] Multiply ( float [ ] a , float [ ] b )
			{
			float [ ] c = { a [ 0 ] * b [ 0 ] , a [ 1 ] * b [ 1 ] , a [ 2 ] * b [ 2 ] };
			return c;
			}

		private float [ ] subtract ( float [ ] a , float [ ] b )
			{
			float [ ] c = { b [ 0 ] - a [ 0 ] , b [ 1 ] - a [ 1 ] , b [ 2 ] - a [ 2 ] };
			return c;
			}

		public void AnnotateEdge ( OpenGL gl , Edge E )
			{
			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			const float ts = 0.4f;
			const string f0 = "Arial";
			//String f1 = "Courrier New";
			float [ ] cf = Multiply ( Add ( cf0 , cf1 ) , 0.5f );
			String txt0 = String . Format ( "{0}" , E . V [ 0 ] . V . ToString ( "0.000" ) );
			String txt1 = String . Format ( "{0}" , E . V [ 1 ] . V . ToString ( "0.000" ) );
			String txt2 = String . Format ( "{0}" , E . delta_V . ToString ( "0.000" ) );

			if ( this . MW . HackCheckBox_C1_R1_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt0 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
				}

			if ( this . MW . HackCheckBox_C2_R1_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf [ 0 ] , cf [ 1 ] , ( z0 + z1 ) * .5d );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt2 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
				}

			if ( this . MW . HackCheckBox_C3_R1_CheckBox_Control . IsChecked . Value )
				{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , deviation: 0f , extrusion: .05f , text: txt1 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
				}
			}

		private float [ ] Multiply ( float [ ] a , float v )
			{
			float [ ] cf = {
					a [0]*v,
					a [1]*v,
					a [2]*v,
					};

			return cf;
			}

		private float [ ] Add ( float [ ] cf0 , float [ ] cf1 )
			{
			float [ ] cf = {
					cf0 [0]+cf1[0],
					cf0 [1]+cf1[1],
					cf0 [2]+cf1[2],
					};

			return cf;
			}

		private void DrawEdge ( Edge E )
			{
			OpenGL gl = this.MW . myOpenGLControl . OpenGL;

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 1 );

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

			AnnotateEdge ( gl , E );

			gl . Color ( .95 , .99 , .80 );
			gl . LineWidth ( 2 );
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );

			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );

			if ( IsInBetween ( E , this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue ) )
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
				float v = this.MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;

				float V0 = E . V [ 0 ] . V;
				float V1 = E . V [ 1 ] . V;

				float u = ( v - V0 ) / ( V0 + V1 );
				u = this . MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;
				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];

				float x = (( 1 - u ) * x0) + (u * x1);
				float y = (( 1 - u ) * y0) + (u * y1);
				float z = (( 1 - u ) * z0) + (u * z1);

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

		private bool IsInBetween ( Edge e , float v )
			{
			float v0 = e . V [ 0 ] . V;
			float v1 = e . V [ 1 ] . V;
			return IsInBetween ( v0 , v , v1 );
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
			float z0 = E . V [ 0 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * this.MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

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

		private bool IsInBetween ( float [ ] cf , Edge e )
			{
			float [ ] cf0 = e . V [ 0 ] . cf;
			float [ ] cf1 = e . V [ 1 ] . cf;
			Boolean xb = IsInBetween ( cf0 [ 0 ] , cf [ 0 ] , cf1 [ 0 ] );
			Boolean yb = IsInBetween ( cf0 [ 1 ] , cf [ 1 ] , cf1 [ 1 ] );
			Boolean zb = IsInBetween ( cf0 [ 2 ] , cf [ 2 ] , cf1 [ 2 ] );
			return xb && yb;
			}

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

		private void VerifyVertexCycles ( )
			{
			if ( this . IsCritical )
				{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} [{1},{2}] IsCritical" , nameof ( VerifyVertexCycles ) , this . I , this . J ) );
				return;
				}

			int largest_index = this.SortedVertexIndicies [ 0 ];
			int smallest_index = this.SortedVertexIndicies [ 3 ];
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
				if ( e . SubEdge != null )
					{
					}

				DrawPerimeterEdge ( e );
				}
			}

		public void DrawPerimeterEdge ( Edge e )
			{
			OpenGL gl = MainWindow.staticGLHook;
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

		private void DrawIsoEdges ( )
			{
			for ( int i = 0 ; i < this . IsoEdges ; i++ )
				{
				IsoEdge IE = this . IsoEdge [ i ];
				DrawIsoEdge ( IE );
				}
			}

		private void DrawIsoEdge ( IsoEdge iE )
			{
			OpenGL gl = MainWindow.staticGLHook;
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
		}
}