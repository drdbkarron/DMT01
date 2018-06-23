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
using System.Drawing;
using System.Xml;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;
using System.Threading;
using GlmSharp;
using GlmNet;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.WPF.SceneTree;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Graphics;
using WpfScreenHelper;
using LocalMaths;

namespace System.Windows.Controls
{
    public static class MyExt
        {
        public static void PerformClick ( this Button btn )
            {
            btn . RaiseEvent ( new RoutedEventArgs ( Button . ClickEvent ) );
            }
		public static void PerformClick ( this CheckBox cb )
		{
			cb . RaiseEvent ( new RoutedEventArgs ( CheckBox . ClickEvent ) );
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

    #region DMT_Geometry_Classes

    public class LineStinkerClass
	{
		public enum LineStinkerModes
		{
			StartAtNearVertex,
			StartAtFarVertex,
			FloatBetweenVerticies
		}
	}


public class Vertex
	{
		public short VertexIndex;
		public int I, J;
		public float V;
		public float [ ] cf;
		public IsoEdge E;
		public Edge NextEdge;
		public Edge PreviousEdge;
		public Vertex Next;
		public Vertex Previous;

		public Vertex ( )
		{
		}

		public Vertex ( short VIndex , int i , int j )
		{
			this . VertexIndex = VIndex;
			this . cf = new float [ 3 ] { i , j , 0 };
			this . I = i;
			this . J = j;
		}

		public Vertex ( float x , float y , float z , float v )
		{
			//this . x = x;
			//this . y = y;
			//this . z = z;
			this . cf = new float [ ] { x , y , z };
			this . I = ( int ) x;
			this . J = ( int ) y;
			this . V = v;

		}

		public Vertex ( float [ ] c1 , float v )
		{
			this . cf = new float [ 3 ] { c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] };
			this . V = v;
		}

		public Vertex ( short VIndex , int i , int j , float [ , ] c )
		{
			this . VertexIndex = VIndex;
			this . cf = new float [ 3 ] { i , j , 0 };
			this . I = i;
			this . J = j;
			this . V = c [ i , j ];
		}

		internal void Draw ( )
		{
			if ( MainWindow.staticGLHook == null )
				return;
			if ( this . cf == null )
				return;
			OpenGL gl = MainWindow.staticGLHook;
			gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
			gl . Vertex ( this . cf );
			gl . End ( );

		}
	};

	public class Edge
	{
		public short EdgeIndex;
		public short SubEdgeIndex;
		public Boolean IsSubEdge;
		public Vertex [ ] V = new Vertex [ 2 ];
		public Edge[] SubEdge   ;
		//public SubEdge SubEdges;
		public short SubEdgeCount;
		public Vertex [ ] TweenVerte;
		public short TweenVerts;
		public Edge Next;
		public Edge Parent=null;
		public Edge Previous;
		public float delta_V;
		public float[] delta_cf;
		public enum EdgeDirection
		{
			ClockwiseNext,
			CounterClockwisePrevious
		}

		public Edge ( )
		{
		}

		public Edge ( short EI , Vertex V0 , Vertex V1 )
		{
			this . EdgeIndex = EI;
			this . V = new Vertex [ 2 ] { V0 , V1 };
			this . delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
			this . delta_cf = new float [ 3 ] {
					this . V [ 1 ] . cf[0] - this . V [ 0 ] . cf[0],
					this . V [ 1 ] . cf[1] - this . V [ 0 ] . cf[1],
					this . V [ 1 ] . cf[2] - this . V [ 0 ] . cf[2],
				};

		}

		public Edge ( short EI , short subEdgeIndex , Vertex V0 , Vertex V1 )
		{
			this . EdgeIndex = EI;
			this . V [ 0 ] = V0;
			this . V [ 1 ] = V1;
			this . delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
			this . delta_cf = new float [ 3 ] {
					this . V [ 1 ] . cf[0] - this . V [ 0 ] . cf[0],
					this . V [ 1 ] . cf[1] - this . V [ 0 ] . cf[1],
					this . V [ 1 ] . cf[2] - this . V [ 0 ] . cf[2]
					};

			this . SubEdgeIndex = subEdgeIndex;
		}

		internal String DisplayMe ( )
		{
			if ( this . IsSubEdge )
			{
				String fomont = String . Format ( "E{0} sub {1} of {2} {3}" , this . EdgeIndex , this . SubEdgeIndex , this . SubEdgeCount , this . Showspan ( ) );
				return fomont;
			}
			String fomont1 = String . Format ( "{0} {1}" , this . EdgeIndex , this . Showspan ( ) );
			return fomont1;
		}

		private String Showspan ( )
		{
			String Stranger = "0.000";
			string WaddaDo = String . Format ( @"d{0} V[0]={1} -> V[1]={2}" ,
				this . delta_V . ToString ( Stranger ) ,
				this . V [ 0 ] . V . ToString ( Stranger ) ,
				this . V [ 1 ] . V . ToString ( Stranger ) );
			return WaddaDo;
		}
	}

	public class SubEdge : Edge
	{
		public Edge ParentEdge;
		public Edge E=new Edge();
	}

	public class IsoEdge : Edge
	{
		public float IsoEdgeIsoValue;
		public Edge E = new Edge ( );

		public IsoEdge ( float IsoValue , Vertex V0 , Vertex V1 )
		{
			//E = new Edge ( );
			IsoEdgeIsoValue = IsoValue;
			E . V [ 0 ] = V0;
			E . V [ 1 ] = V1;
			this . delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;

		}

		public IsoEdge ( short v1 , short v2 , float IsoValue , Vertex V0 , Vertex V1 )
		{
			E = new Edge ( );
			E . EdgeIndex = v1;
			E . SubEdgeIndex = v2;
			IsoEdgeIsoValue = IsoValue;
			this . E . V [ 0 ] = V0;
			this . E . V [ 1 ] = V1;
			this . delta_V = this . E . V [ 1 ] . V - this . E . V [ 0 ] . V;   // should be zero

		}
	}

	public  class Boxel	
	{
		public DMT01.MainWindow MW;
		public int I;
		public int J;
		public Vertex [ ] V = new Vertex [ 4 ];
		public Vertex Centroid = new Vertex ( );
		public short Span=1;
		public short[]  SortedVertexIndicies=new short [ 4 ] { 0 , 1 , 2 , 3 };
		public short LargestVertex, SmallestVertex;
		public Edge [ ] E = new Edge [ 4 ];
		public IsoEdge [ ] IsoEdge = new IsoEdge [ 4 ];
		public short IsoEdges = 0;
		public Boolean IsCritical;
		public float delta_v;
		public float max_V;
		public float min_V;

		public Boxel ( int i , int j , float [ , ] c )
		{
			this . I = i;
			this . J = j;
			this . V [ 0 ] = new Vertex ( 0 , i , j , c );
			this . V [ 1 ] = new Vertex ( 1 , i , j + Span , c );
			this . V [ 2 ] = new Vertex ( 2 , i + Span , j + Span , c );
			this . V [ 3 ] = new Vertex ( 3 , i + Span , j , c );

			this . V [ 0 ] . Next = V [ 1 ];
			this . V [ 1 ] . Next = V [ 2 ];
			this . V [ 2 ] . Next = V [ 3 ];
			this . V [ 3 ] . Next = V [ 0 ];

			this . V [ 0 ] . Previous = V [ 3 ];
			this . V [ 1 ] . Previous = V [ 0 ];
			this . V [ 2 ] . Previous = V [ 1 ];
			this . V [ 3 ] . Previous = V [ 2 ];

			this . Centroid = LoadCentroid ( );

			this . E [ 0 ] = new Edge ( 0 , this . V [ 0 ] , this . V [ 1 ] );
			this . E [ 1 ] = new Edge ( 1 , this . V [ 1 ] , this . V [ 2 ] );
			this . E [ 2 ] = new Edge ( 2 , this . V [ 2 ] , this . V [ 3 ] );
			this . E [ 3 ] = new Edge ( 3 , this . V [ 3 ] , this . V [ 0 ] );

			this . E [ 0 ] . Next = E [ 1 ];
			this . E [ 1 ] . Next = E [ 2 ];
			this . E [ 2 ] . Next = E [ 3 ];
			this . E [ 3 ] . Next = E [ 0 ];

			this . E [ 0 ] . Previous = E [ 3 ];
			this . E [ 1 ] . Previous = E [ 0 ];
			this . E [ 2 ] . Previous = E [ 1 ];
			this . E [ 3 ] . Previous = E [ 2 ];

			this . V [ 0 ] . NextEdge = E [ 0 ];
			this . V [ 1 ] . NextEdge = E [ 1 ];
			this . V [ 2 ] . NextEdge = E [ 2 ];
			this . V [ 3 ] . NextEdge = E [ 3 ];

			this . V [ 0 ] . PreviousEdge = E [ 3 ];
			this . V [ 1 ] . PreviousEdge = E [ 2 ];
			this . V [ 2 ] . PreviousEdge = E [ 1 ];
			this . V [ 3 ] . PreviousEdge = E [ 0 ];

			this . IsThisCritical ( );

			this . DoMinMaxDelta ( );

			this . SortVerticies ( );

			if ( !this . IsCritical )
				this . LoadTweenVerts ( );

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

		private void DoCriticalIsoEdges ( )
		{

		}

		private bool IsBetween ( Edge e1 , Edge e2 )
		{
			throw new NotImplementedException ( );
		}

		private void InsertVertexOnEdge ( float v , Edge e )
		{
			if ( !IsInBetween ( v , e ) )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "xx{0} E{1} {2} {3} " ,
					this . DisplayMe ( ) , e . EdgeIndex , Comparis ( v , e ) , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

				//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " ,  Centroid.V.ToString("0.000") , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
				return;
			}

			System . Diagnostics . Debug . WriteLine ( String . Format ( "++{0} E{1} {2} {3} " ,
				this . DisplayMe ( ) , e . EdgeIndex , Comparis ( v , e ) , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private String DisplayMe ( )
		{
			String Stranger = "0.00";
			String strang = String . Format ( @"[i{0},j{1}] {2}{3}{4}" , this . I , this . J ,
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
			String Stranger = "0.00";
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

				if ( v < min_V )
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

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "What are you doing here?" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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
			Vertex V = this . V [ SortedVertexIndicies [ v ] ];
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
			float dx = x0 + linageFraction * ( deltax0x1 );
			float dy = y0 + linageFraction * ( deltay0y1 );
			float dz = z0 + linageFraction * ( deltaz0z1 );
			float [ ] c = new float [ 3 ] { dx , dy , dz };
			return c;
		}

		private bool IsInBetween ( int v , int e )
		{
			int S = SortedVertexIndicies [ v ];
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
			} while ( IsSorted == false );

			//Burblegiggle ( swaps , "Completed" );

			this . LargestVertex = this . SortedVertexIndicies [ 3 ];
			this . SmallestVertex = this . SortedVertexIndicies [ 0 ];

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , Sortee(SmallestVertex,LargestVertex) , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private String Sortee ( short smallestVertex , short largestVertex )
		{
			float v0 = this . V [ smallestVertex ] . V;
			float v1 = this . V [ largestVertex ] . V;
			String strang_format = "0.000";
			String Strang = String . Format ( "{0}{1}{2}" , v0 . ToString ( strang_format ) , Comparis ( v0 , v1 ) , v1 . ToString ( strang_format ) );

			return Strang;
		}

		public String Comparis ( float v0 , float v1 )
		{
			if ( v0 < v1 )
				return "<";
			if ( v0 == v1 )
				return "==";
			if ( v0 > v1 )
				return ">";
			return "??";
		}

		private Vertex LoadCentroid ( )
		{
			float X = ( this . V [ 0 ] . cf [ 0 ] + this . V [ 1 ] . cf [ 0 ] + this . V [ 2 ] . cf [ 0 ] + this . V [ 3 ] . cf [ 0 ] ) / 4f;
			float Y = ( this . V [ 0 ] . cf [ 1 ] + this . V [ 1 ] . cf [ 1 ] + this . V [ 2 ] . cf [ 1 ] + this . V [ 3 ] . cf [ 1 ] ) / 4f;
			float Z = ( this . V [ 0 ] . cf [ 2 ] + this . V [ 1 ] . cf [ 2 ] + this . V [ 2 ] . cf [ 2 ] + this . V [ 3 ] . cf [ 2 ] ) / 4f;
			float V = ( this . V [ 0 ] . V + this . V [ 1 ] . V + this . V [ 2 ] . V + this . V [ 3 ] . V ) / 4f;

			Vertex C = new Vertex ( X , Y , Z , V );
			C . VertexIndex = 4;

			return C;
		}

		internal void DrawMe ( )
		{

			DrawBoxelEdgeCycles ( );

			AnnotateBoxelAtCentroid ( );

			//VerifyCycles ( );
		}

		private void AnnotateBoxelAtCentroid ( )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = MW . myOpenGLControl . OpenGL;
			if ( gl == null )
			{
				return;
			}

			if ( MW . HackCheckBox_C10_R1_CheckBox_Control . IsChecked . Value )
			{
				float ts = 0.3f;
				string f0 = "Arial";
				String txt0 = String . Format ( @"[{0},{1}]" , this.I, this.J );

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( .9 , .9 , .9 );
				gl . PushMatrix ( );
				gl . Translate ( x: this . Centroid . cf [ 0 ] , y: this . Centroid . cf [ 1 ] , z: this . Centroid . cf [ 2 ] );
				gl . Translate ( x: -.4, y: 0 , z: 0 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , extrusion: .05f , deviation: 0f , text: txt0 );
				gl . PopMatrix ( );

				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( MW . HackCheckBox_C9_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( .9 , .2 , .2 );
				gl . PointSize ( 7 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( this . Centroid . cf );
				gl . End ( );
				gl . PopAttrib ( );
			}

			if ( MW . HackCheckBox_C8_R1_CheckBox_Control . IsChecked . Value )
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

			if ( MW . HackCheckBox_C1_R2_CheckBox_Control . IsChecked . Value )
			{
				if ( MW . Hack_H_Slider_02_UserControl . SliderValue < 0 )
					return;

				if ( MW . Hack_H_Slider_02_UserControl . SliderValue > 1 )
					return;

				float [ ] [ ] [ ] Stinkers = new float [ 4 ] [ ] [ ];
				Stinkers [ 0 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 0 ] . cf ,
					fraction: MW . Hack_H_Slider_02_UserControl . SliderValue , mode: this . MW .Stankey );
				Stinkers [ 1 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 1 ] . cf ,
					fraction: MW . Hack_H_Slider_02_UserControl . SliderValue , mode: this . MW . Stankey );
				Stinkers [2 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 2 ] . cf ,
					fraction: MW . Hack_H_Slider_02_UserControl . SliderValue , mode: this . MW . Stankey );
				Stinkers [ 3 ] = LineStinker ( cf0: this . Centroid . cf , cf1: this . V [ 3 ] . cf ,
					fraction: MW . Hack_H_Slider_02_UserControl . SliderValue , mode: this . MW . Stankey );

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

		private float [ ][ ] LineStinker ( float [ ] cf0 , float [ ] cf1 , float fraction, DMT01. LineStinkerClass.LineStinkerModes mode )
		{

			if ( fraction < 0 )
				return  null;

			if ( fraction > 1 )
				return null;

			float [ ] a = new float [ 3 ];

			a= subtract ( cf0 , cf1 );

			float m = magnitude ( a );

			float [ ] n = Divide ( a , m );

			float [ ] v_forward = Multiply ( n , fraction );
			float [ ] v_reverse = Multiply ( n , -fraction );

			float [ ][ ] stinkey_vertices = new float [ 2 ][];

			float [ ] stinky0 = new float [ 3 ];
			float [ ] stinky1 = new float [ 3 ];
			switch ( mode )
			{
				case DMT01.LineStinkerClass.LineStinkerModes . StartAtNearVertex:
					stinky0= cf0;
					stinky1 = Add ( v_forward , cf0 );
					break;
				case DMT01 . LineStinkerClass . LineStinkerModes . StartAtFarVertex:
					stinky0= cf1;
					stinky1 = Add ( v_forward , cf1 );
					break;
				case DMT01 . LineStinkerClass . LineStinkerModes . FloatBetweenVerticies:
					stinky1 = Add ( v_forward , cf0 );
					stinky0 = Add ( v_reverse, cf0);


					break;
				default:
					break;
			}

			stinkey_vertices [ 0 ] = new float [ 3 ] { stinky0 [ 0 ] , stinky0 [ 1 ] , stinky0 [ 2 ] };
			stinkey_vertices [ 1 ] = new float [ 3 ] { stinky1 [ 0 ] , stinky1 [ 1 ] , stinky1 [ 2 ] };

			return stinkey_vertices;
		}

		private float [ ] Divide ( float [ ] a , float m )
		{

			float [ ] d = new float [ 3 ] { a [ 0 ] / m , a [ 1 ] / m , a [ 2 ] / m };
			return d;
		}

		private float magnitude ( float [ ] a )
		{
			double b = a [ 0 ] * a [ 0 ] + a [ 1 ] * a [ 1 ] + a [ 2 ] * a [ 2 ];
			float c = (float)Math . Sqrt ( b );
			return c;
		}

		private void DrawBoxelEdgeCycles ( )
		{
			if ( MW == null )
			{
				return;
			}

			if ( MW . EdgeFactorHack0CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 0 ];
				DebugDrawEdge ( E );
			}

			if ( MW . EdgeFactorHack1CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 1 ];
				DebugDrawEdge ( E );
			}

			if ( MW . EdgeFactorHack2CheckBox_Control . IsChecked . Value )
			{
				Edge E = this . E [ 2 ];
				DebugDrawEdge ( E );
			}

			if ( MW . EdgeFactorHack3CheckBox_Control . IsChecked . Value )
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
				return;

			VerifyEdgeCycles ();
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
					ExitCondition = !( this . LargestVertex == VI );
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
			int largest_index = SortedVertexIndicies [ 0 ];
			int smallest_index = SortedVertexIndicies [ 3 ];
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
					ExitCondition = !( smallest_index == VI );
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

		private void DebugDrawEdge ( Edge E )
		{
			if ( this . MW == null )
			{
				return;
			}

			OpenGL gl = MW.myOpenGLControl.OpenGL;
			if ( gl == null )
			{
				return;
			}

			AnnotateEdge ( gl , E );

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

			if ( MW . HackCheckBox_C4_R1_CheckBox_Control . IsChecked . Value )
			{
				float [ ] cf00 = new float [ ] { cf0 [ 0 ] , cf0 [ 1 ] , z0 };
				float [ ] ce = new float [ ] { Centroid.cf[0] , Centroid.cf [ 1 ] , z0 };

				float [ ] faaa = subtract ( cf0 , ce );
				double [ ] faa = ToDoubleArray ( faaa );
				dvec3 cv = new dvec3( faa );
				float sc = MW . Hack_H_Slider_03_UserControl . SliderValue;

				if ( MW . HackCheckBox_C7_R1_CheckBox_Control . IsChecked . Value )
				{
					double spinner2 =MW . Hack_H_Slider_04_UserControl . SliderValue * 2f * Math . PI;
					float spinner2f=(float)spinner2;
					var rotaxis = new GlmNet . vec3 ( 0 , 0 , 1 );
					GlmNet . mat4 rotation = GlmNet . glm . rotate ( GlmNet . mat4 . identity ( ) ,spinner2f , rotaxis);
					GlmNet . mat4 translation = GlmNet . glm . translate ( GlmNet . mat4 . identity ( ) , new GlmNet . vec3 ( 0 , 0 , 0 ) );
					GlmNet . mat4 modelviewMatrix = rotation * translation;
					GlmNet . mat3 normalMatrix = modelviewMatrix . to_mat3 ( );
					GlmNet.vec3 noon = new GlmNet . vec3 ( x: 1 , y: 0 , z: 0 );
					GlmNet . vec3 rotated_noon = normalMatrix * noon;
					float [ ] ccc = new float [ ] { rotated_noon . x + ce[0], rotated_noon . y+ce[1] , rotated_noon . z +ce[2]};
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . Color ( 1 , .2 , .9 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( ce );
					gl . Vertex ( ccc );
					gl . End ( );
					gl . PopAttrib ( );
				}

				dquat d0q = new dquat ( x: 0 , y: 0 , z: 1 , w: MW . Hack_H_Slider_01_UserControl . SliderValue * 2f*Math . PI );
				dmat3 mm0 = d0q . ToMat3;
				dvec3 aa = mm0 * cv;
				dvec3 rotated0 = Normalize(aa);
				dvec3 rotatified0 = rotated0 * sc;
				dvec3 located0 = add( rotatified0 , cf00);
				float [ ] FA0 = toFloatArray ( located0 );

				dquat d1q = new dquat ( x: 0 , y: 0 , z: -1 , w: MW . Hack_H_Slider_02_UserControl . SliderValue * 4f*Math . PI );
				dmat3 mm1 = d1q . ToMat3;
				dvec3 rotated1 = Normalize ( mm1 * cv )*sc;
				dvec3 located1 = add ( rotated1 , cf00 );
				float [ ] FA1 = toFloatArray ( located1 );

				if ( MW . HackCheckBox_C5_R1_CheckBox_Control . IsChecked . Value )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . Color ( 0 , .8 , 1 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( cf00 );
					gl . Vertex (FA0  );
					gl . End ( );
					gl . PopAttrib ( );
				}

				if ( MW . HackCheckBox_C6_R1_CheckBox_Control . IsChecked . Value )
				{
					gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
					gl . Color ( 1 , .2 , .9 );
					gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
					gl . Vertex ( cf00 );
					gl . Vertex ( FA1 );
					gl . End ( );
					gl . PopAttrib ( ); }
			}

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 1 );
			gl . Color ( 0.9 , 0.8 , 0.8 );
			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );
			gl . PopAttrib ( );
			
			if ( MW . HackCheckBox_C5_R1_CheckBox_Control . IsChecked . Value )
			{
				float u = MW.Threshold_Hack_H_Slider_2_UserControl.SliderValue;
				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];

				float x = ( 1 - u ) * x0 + u * x1;
				float y = ( 1 - u ) * y0 + u * y1;
				float z = ( 1 - u ) * z0 + u * z1;

				float [ ] c_th = new float [ ]     
					{
						x,y,z
					};

				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . LineWidth ( 1 );
				gl . PointSize ( 5 );
				gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
				gl . Vertex ( c_th [ 0 ] , c_th [ 1 ] , c_th [ 2 ] );
				gl . End ( );
				gl . PopAttrib ( );
			}
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
			double m = v . x * v . x + v . y * v . y + v . z + v . z;
			double mr = Math . Sqrt ( m );
			return mr;
		}

		private float[] toFloatArray ( dvec3 dv )
		{
			float [ ] fa = new float [ ] { (float)dv . x , (float)dv . y , (float)dv . z};
			return fa;	
		}

		private dvec3 add ( dvec3 r , float [ ] f )
		{
			dvec3 a = new dvec3 ( x: r [ 0 ] + f [ 0 ] , y: r [ 1 ] + f [ 1 ] , z: r [ 2 ] + f [ 2 ] );
			return a;
		}

		private double[] ToDoubleArray ( float [ ] v )
		{
			double [ ] d = new double [ ] { v [ 0 ] , v [ 1 ], v [ 2 ] };
			return d;
		}

		private float [ ] Multiply ( float[] a , float [ ] b )
		{
			float [ ] c = new float [ ] { a [ 0 ] * b [ 0 ] , a [ 1 ] * b [ 1 ] , a [ 2 ] * b [ 2 ] };
			return c;
		}

		private float[] subtract ( float [ ] a , float [ ] b )
		{
			float [ ] c = new float [ ] { b [ 0 ] - a [ 0 ] , b [ 1 ] - a [ 1 ] , b [ 2 ] - a [ 2 ] };
			return c;
		}

		public void AnnotateEdge ( OpenGL gl , Edge E )
		{
			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float ts = 0.4f;
			string f0 = "Arial";
			//String f1 = "Courrier New";
			float [ ] cf = Multiply ( Add ( cf0 , cf1 ) , 0.5f );
			String txt0 = String . Format ( "{0}" , E . V [ 0 ] . V . ToString ( "0.000" ) );
			String txt1 = String . Format ( "{0}" , E . V [ 1 ] . V . ToString ( "0.000" ) );
			String txt2 = String . Format ( "{0}" , E . delta_V . ToString ( "0.000" ) );

			if ( MW. HackCheckBox_C1_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , extrusion: .05f , deviation: 0f , text: txt0 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}

			if ( MW.HackCheckBox_C2_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf [ 0 ] , cf [ 1 ] , ( z0 + z1 ) * .5d );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , extrusion: .05f , deviation: 0f , text: txt2 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}

			if ( MW.HackCheckBox_C3_R1_CheckBox_Control . IsChecked . Value )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				gl . Translate ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
				gl . Scale ( ts , -ts , ts );
				gl . DrawText3D ( faceName: f0 , fontSize: 12f , extrusion: .05f , deviation: 0f , text: txt1 );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}
		}

		private float [ ] Multiply ( float [ ] a , float v )
		{
			float [ ] cf = new float [ ] {
					a [0]*v,
					a [1]*v,
					a [2]*v,
					};

			return cf;
		}

		private float [ ] Add ( float [ ] cf0 , float [ ] cf1 )
		{
			float [ ] cf = new float [ ] {
					cf0 [0]+cf1[0],
					cf0 [1]+cf1[1],
					cf0 [2]+cf1[2],
					};

			return cf;
		}

		private void DrawEdge ( Edge E )
		{
			OpenGL gl = MW . myOpenGLControl . OpenGL;

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . LineWidth ( 1 );

			float [ ] cf0 = E . V [ 0 ] . cf;
			float [ ] cf1 = E . V [ 1 ] . cf;
			float z0 = E . V [ 0 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;

			AnnotateEdge ( gl , E );

			gl . Color ( .95 , .99 , .80 );
			gl . LineWidth ( 2 );
			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );

			gl . Begin ( SharpGL . Enumerations . BeginMode . Lines );
			gl . Vertex ( cf0 [ 0 ] , cf0 [ 1 ] , z0 );
			gl . Vertex ( cf1 [ 0 ] , cf1 [ 1 ] , z1 );
			gl . End ( );

			if ( IsInBetween ( E , MW.Threshold_Hack_H_Slider_2_UserControl.SliderValue) )
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
				float v = MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;

				float V0 = E . V [ 0 ] . V;
				float V1 = E . V [ 1 ] . V;

				float u = ( v - V0 ) / ( V0 + V1 );
				u = MW . Threshold_Hack_H_Slider_2_UserControl . SliderValue;
				float x0 = cf0 [ 0 ];
				float y0 = cf0 [ 1 ];

				float x1 = cf1 [ 0 ];
				float y1 = cf1 [ 1 ];

				float x = ( 1 - u ) * x0 + u * x1;
				float y = ( 1 - u ) * y0 + u * y1;
				float z = ( 1 - u ) * z0 + u * z1;

				float [ ] c_th = new float [ ]       // this is all wrong here
					{
							x,y,z
					};

				if ( IsInBetween ( c_th , E ) )          // this should be catching the  imtermediate points
				{
					System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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
			float z0 = E . V [ 0 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;
			float z1 = E . V [ 1 ] . V * MW . Z_Fudge_H_Slider_0_UserControl . SliderValue;


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

			int largest_index = SortedVertexIndicies [ 0 ];
			int smallest_index = SortedVertexIndicies [ 3 ];
			Vertex Vsmallest = this . V [ smallest_index ];
			int next_smallest_index = Vsmallest . Next . VertexIndex;
			int previous_smallest_index = Vsmallest . Previous . VertexIndex;

			Vertex Vlargest = this . V [ largest_index ];
			Vertex V = Vlargest;

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} [{1}.{2}] " , nameof ( VerifyVertexCycles ) , this . I , this . J ) );

			Boolean reset_condition = true;
			Boolean exit_condition = reset_condition;
			while ( exit_condition )
			{
				System . Diagnostics . Debug . Write ( String . Format ( "^{0}->{1}" , V . VertexIndex , V . Next . VertexIndex ) );

				V = V . Next;
				exit_condition = !( V . VertexIndex == next_smallest_index );
			}

			V = Vlargest;
			exit_condition = reset_condition;
			while ( exit_condition )
			{
				System . Diagnostics . Debug . Write ( String . Format ( "V{0}->{1}" , V . VertexIndex , V . Previous . VertexIndex ) );

				V = V . Previous;
				exit_condition = !( V . VertexIndex == previous_smallest_index );

			}

			System . Diagnostics . Debug . WriteLine ( String . Format ( "\n{0} {1} " , "completed" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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
				return;
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
			for ( int i = 0 ; i < IsoEdges ; i++ )
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

	public class Region
	{
		public DMT01.MainWindow MW=  ( DMT01 . MainWindow ) System. Windows. Application . Current . MainWindow;
		public double Max;
		public double Min;		   
		public int N;
		public int Start_Rows;
		public int End_Rows;
		public int Start_Cols;
		public int End_Cols;
		public float[,] Cells;

		public Region ( float[,] C, int StartRows , int EndRows , int StartCols , int EndCols )
		{
			this . Start_Rows = StartRows;
			this . End_Rows = EndRows;
			this . Start_Cols = StartCols;
			this . End_Cols = EndCols;
			Max = -double . MaxValue;
			Min = double . MaxValue;
			this . Cells = C;

			for ( int j = Start_Rows ; j < End_Rows ; j++ )
			{
				for ( int i = Start_Cols ; i < End_Cols ; i++ )
				{
					double v = Cells[i,j];
					if (  v > this.Max)
					{
						this . Max = v;

						//System . Diagnostics . Debug . WriteLine ( String . Format ( "Max: {0} {1} " , this . Max , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
					}
					if ( v < 0 )
					{
						//System . Diagnostics . Debug . WriteLine ( String . Format ( "no negativity please {0} at [{1},{2}] {3} " , v, i, j , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
						Cells [ i , j ] = 0.0f;
					}
					else
					if ( v < this . Min )
					{
						this . Min = v;

						//System . Diagnostics . Debug . WriteLine ( String . Format ( "Min: {0} {1} " , this . Min , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
					}
					this . N++;
				}
			}
			MW . region_threshold_H_Slider_UserControl . SliderMinValue = Min;
			MW . region_threshold_H_Slider_UserControl . SliderMaxValue = Max;


			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , Display() , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private String Display ( )
		{
			String Stranger = "00.000";
			int Rows = End_Rows - Start_Rows;
			int Cols = End_Cols - Start_Cols;
			int n = Rows * Cols;
			if ( n != N )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "badness heree" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
			}

			String a = String . Format ( "{0} in {1}->{2}, {3} Rows; {4}->{5},{6} Cols; Min:{7} Max: {8}" , 
				N, 
				this.Start_Rows, this.End_Rows, Rows, 
				this.Start_Cols, this.End_Cols,	Cols,
				Min.ToString(Stranger), Max.ToString(Stranger));

			return a;
		}
	}

	#endregion DMT_Geometry_Classes

	public partial class MainWindow : Window
	{
		#region Persistance_classes2

		static long Draws = 0;
		static long Resizes = 0;
		static DateTime StartDateTime;
		static TimeSpan ElapsedDateTime;
		static int CurrentWorksheetIndex;
		public static OpenGL staticGLHook;


		public DMT01.LineStinkerClass.LineStinkerModes Stankey=DMT01.LineStinkerClass.LineStinkerModes.StartAtFarVertex;

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
			[XmlElement ( "SeralizeControlCommonFields" )]
			public SeralizeControlCommonFields CommonFields;
			public Boolean RadioCheckBoxState;
			public String RadioCheckBoxName;
			public String RadioGroupName;
			public RadioCheckBoxSaveState ( )
			{
				CommonFields = new SeralizeControlCommonFields ( );
			}
		}

		[Serializable]
		public class TabControlSaveState
		{
			[XmlElement ( "SeralizeControlCommonFields" )]
			public SeralizeControlCommonFields CommonFields=new SeralizeControlCommonFields();
			public short SelectedIndex;

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
			public int SheetTabControl;
			public int OtherControls;
			public int Calls;
			public int MaxDepth;
		};

		static String XmlFileContents;
		static String XmlFileName;
		static SavedControl SC;

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

		#endregion persistance


		public MainWindow ( )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "MainWindow()" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Replace ( @"C:\Users\karro\Source\Repos\DMT01\" , "${Soln}" ) . Trim ( ) ) ) );

			InitializeComponent ( );

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "InitalizeComponent Completed" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private void DMTMainWindow_Initialized ( object sender , EventArgs e )
		{


			Debug . WriteLine ( String . Format ( "{0}" , nameof ( DMTMainWindow_Initialized ) ) );

		}

		private void DMTMainWindow_Loaded ( object sender , RoutedEventArgs e )
		{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting scripty check buttons" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			if ( this . LaunchSavedStateOnInitalization_RadioButton_Control . IsChecked . GetValueOrDefault ( ) )
			{
				Load_XML_Saved_Control_States_Button . PerformClick ( );
			}
			if ( LoadSpreadsheetAtInitalization_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				spreadsheet_load_Button . PerformClick ( );
			}
			if ( Do_Execute_Select_Data_Button_on_Startup_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				DoSelectDataRangeForNormalization_Button . PerformClick ( );
			}

			StartDateTime = System . DateTime . Now;

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Finished scripty" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void myReoGridControl_Loaded ( object sender , RoutedEventArgs e )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Loading Spreadsheet" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			TextRange tr1 = new TextRange ( SpreadsheetDirPath_RichTextBox . Document . ContentStart , SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
			TextRange tr2 = new TextRange ( SpreadsheetFileName_RichTextBox . Document . ContentStart , SpreadsheetFileName_RichTextBox . Document . ContentEnd );

			String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );

			if ( System . IO . File . Exists ( Path ) )
			{
				myReoGridControl . Load ( Path , unvell . ReoGrid . IO . FileFormat . Excel2007 );
			}

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Loading Spreadsheet completed" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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
			if ( DrawMouseScreenSpaceAnnotation_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				float fSize = MouseScreenAnnotationFontSize_H_Slider_UserControl . SliderValue + 0.5f;
				String text = String . Format ( @"{0},{1}" , mouse_x , mouse_y );
				gl . DrawText ( x: viewport_cursor_x , y: viewport_cursor_y , r: 1f , g: 1f , b: 1f ,
					faceName: "Arial" ,
					fontSize: fSize ,
					text: text );
				gl . PopMatrix ( );
				gl . PopAttrib ( );
			}

			if ( DrawScreenSpaceAnnotationGrid_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . PushMatrix ( );
				int bloppie = 40;
				float fSize = ScreenAnnotationFont_SizeH_Slider_UserControl . SliderValue;
				for ( int i = bloppie ; i < viewport_width - bloppie ; i += bloppie )
				{
					for ( int j = bloppie ; j < viewport_height - bloppie ; j += bloppie )
					{
						var t = String . Format ( @"{0},{1}" , i , j );
						int jy = ( int ) viewport_height - j;
						gl . DrawText ( i , jy , 1 , 1 , 1 , "Arial" , fSize , t );
					}
				}
				gl . PopMatrix ( );
				gl . PopAttrib ( );
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

			if ( Do_Orbit_Pull_Back_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				gl . Translate (
					x: Eye_X_H_Slider_UserControl . SliderValue ,
					y: Eye_Y_H_Slider_UserControl . SliderValue ,
					z: Eye_Z_H_Slider_UserControl . SliderValue );
			}

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
					DoMinusTicks: Axis_DrawNegativeCheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					TagOrigin: AnnotateOrigin_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					DoXYZAnnotation: AnnotateAxisXYZ_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					DoUnitTicks: DoDoTickMarks_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					DoAnnotateZTicks: AnnotateZTickMarks_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					DoAnnotateYTicks: AnnotateYTickMarks_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					DoAnnotateXTicks: AnnotateXTickMarks_CheckBox_Control . IsChecked . GetValueOrDefault ( ) ,
					tick_annotation_scale: Axis_tick_annotation_scale_H_Slider_UserControl . SliderValue ,
					Draw_Minus_Z_Axis_Leg: Draw_Minus_Z_Axis_Leg_CheckBox_Control . IsChecked . Value
					);
			}

			if ( YourArmsTooShortToBoxWithHashem . IsChecked . Value )
			{
				ArmsTooShortToBoxWithHashem ( gl );
			}

			if ( DrawTeaPot_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				Teapot tp = new Teapot ( );
				tp . Draw ( gl , 14 , 1 , OpenGL . GL_FILL );
			}

			if ( Spreadsheet_Grid_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
			{
				ReoGrid3DSpreadsheet ( gl: gl );
			}

			////DoAspect ( );

			//gl . Flush ( );

			Draws_Label . Content = String . Format ( "Draw Count: {0}" , Draws );
			Draws++;
		}

		private void ArmsTooShortToBoxWithHashem ( OpenGL gl )
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

			Window mw =Application . Current . MainWindow;

			gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
			gl . PushMatrix ( );
			gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
			gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
			gl . Scale ( 1 , -1 , 1 );

			Region Selected_Region = new Region ( C: Sheety.cells, StartRows: Sheety . r0, EndRows: Sheety . r1 , StartCols: Sheety.c0, EndCols: Sheety.c1);

			for ( int j = Sheety . r0 ; j < Sheety . r1 ; j++ )
			{
				for ( int i = Sheety . c0 ; i < Sheety . c1 ; i++ )
				{
					if ( IsInBetween ( 47 , j , 50 ) )
					{
						Boxel B = new Boxel ( i , j , Sheety . cells );
						B . MW = ( DMT01 . MainWindow ) mw;
						if ( B . IsCritical )
						{
							B . DrawMe ( );
						}
					}
				}
			}
			gl . PopMatrix ( );
			gl . PopAttrib ( );
		}

		private bool IsInBetween ( int v1 , int j , int v2 )
		{
			if ( v1 <= j )
			{
				if ( j <= v2 )
				{
					return true;
				}
			}
			if ( v1 >= j )
			{
				if ( j >= v2 )
				{
					return true;
				}
			}

			return false;
		}

		private static void OldDrawWithoutBoxel ( OpenGL gl , int j , int i )
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

					float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

					gl . Translate ( c [ 0 ] - .25 , c [ 1 ] - .15 , c [ 2 ] );
					gl . Scale ( 0.6 , 0.6 , 1.0 );
					String iter_string = ( i + 1 ) . ToString ( "00" );
					gl . DrawText3D ( faceName: "Times New Roman" , fontSize: 1f , deviation: 0.0f , extrusion: 0.1f , text: iter_string );
					gl . PopMatrix ( );
				}
			}

			if ( DoDrawSpreadsheetTopBorder_CheckBox_Control . IsChecked . GetValueOrDefault ( ) )
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

					float [ ] c = LocalMaths . LocalMathsClass . GetCentroid ( x0 , y0 , x1 , y1 );

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
				gl . PushAttrib ( SharpGL . OpenGL . GL_CURRENT_BIT |
									SharpGL . OpenGL . GL_ENABLE_BIT |
									SharpGL . OpenGL . GL_LINE_BIT |
									SharpGL . OpenGL . GL_DEPTH_BUFFER_BIT );

				//gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
				//gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );

				gl . LineWidth ( 1 );
				if ( Spreadsheet_Aspect_Scale_Hack_checkBox_Control . IsChecked . GetValueOrDefault ( ) )
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
							gl . Translate ( x: x0 + Spreadsheet3DNumericLeftMargin_H_Slider_User_Control . SliderValue , y: c [ 1 ] - .25 , z: c [ 2 ] - 1.0 );
							String Text = number . ToString ( "#,###,##0.##" );
							float len = Text . Length;
							var big_x_scaling = Spreadsheet3DNumericWidthFittingFactor_H_Slider_User_Control . SliderValue / len;
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
							float number;
							if ( float . TryParse ( s: D . DisplayText , result: out number ) )
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

			// gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Back );
			gl . DrawBuffer ( SharpGL . Enumerations . DrawBufferMode . Front );

			DoAspect ( );

		}

		private void spreadsheet_load_Button_Click ( object sender , RoutedEventArgs e )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Spreadsheet Load" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			TextRange tr1 = new TextRange ( SpreadsheetDirPath_RichTextBox . Document . ContentStart , SpreadsheetDirPath_RichTextBox . Document . ContentEnd );
			TextRange tr2 = new TextRange ( SpreadsheetFileName_RichTextBox . Document . ContentStart , SpreadsheetFileName_RichTextBox . Document . ContentEnd );
			String Path = String . Format ( @"{0}\{1}" , tr1 . Text . Trim ( ) , tr2 . Text . Trim ( ) );

			String FileName = String . Format ( @"{0}{1}" , scratchy , ".xll" );
			var a = DMT01 . Properties . Resources . UNEP_NATDIS_disasters_2002_2010;
			System . IO . File . WriteAllBytes ( FileName , a );


			myReoGridControl . Load ( FileName , unvell . ReoGrid . IO . FileFormat . Excel2007 );


			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed Spreadsheet Load" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void myOpenGLControl_Resized ( object sender , SharpGL . SceneGraph . OpenGLEventArgs args )
		{

			//System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "resized" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			OpenGL gl = myOpenGLControl . OpenGL;

			DoAspect ( );

			Resizes++;
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
			SC . SheetTabControl = 0;
			SC . OtherControls = 0;
			SC . Calls = 0;
			SC . MaxDepth = -1;
		}

		public void WalkLogicalTree ( )
		{
			InitalizeSavedControl ( );

			Debug . WriteLine ( String . Format ( "{0} Starting " , nameof ( WalkLogicalTree ) ) );

			WalkLogicalTree ( DMT_Main_Window_Control as FrameworkElement , 0 );

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

		private void SheetTabControlSeralize ( UIElement e )
		{
			FrameworkElement FE = e as FrameworkElement;
			if ( FE == null )
			{
				return;
			}

			TabControl TC = FE as TabControl;

			if ( TC == null ) return;

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
		private int LoadEm_Counter=0;
		private int LoadEm_FileCount;
		private void LoadEm ( )
		{
			String [ ] Xmls = System . IO . Directory . GetFiles ( @".\" , @"*Control.xml" , System . IO . SearchOption . TopDirectoryOnly );
			LoadEm_FileCount = Xmls . Count ( );
			LoadEm_Counter = 0;

			Debug . WriteLine ( String . Format ( "{0} loaded {1} *Control.xml files" , nameof ( LoadEm ) , LoadEm_FileCount ) );
			Load_Each ( "DMT_Main_Window_Control.xml" );
			LoadEm ( Xmls );
		}

		private void LoadEm ( String [ ] Xmls )
		{
			foreach ( String F in Xmls )
			{
				Load_Each ( F );
			}
		}

		private void Load_Each ( string F )
		{

			if ( !File . Exists ( F ) )
			{

				System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} not found {1} " , F , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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

		private void TabControlDeseralizer ( SeralizeControlCommonFields p )
		{
			Object O = LogicalTreeHelper . FindLogicalNode ( DMT_Main_Window_Control , p . ControlName );
			TabControl TC = O as TabControl;
			if ( TC == null )
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

			var o = x . Deserialize ( xmlReader );

			pp = ( TabControlSaveState ) o;
			TC . SelectedIndex = pp . SelectedIndex;
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

		private void CheckBox_ControlDeseralizer ( SeralizeControlCommonFields p )
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
			Window C = LogicalTreeHelper . FindLogicalNode ( logicalTreeNode: this . DMT_Main_Window_Control , elementName: p . ControlName ) as Window;

			WindowsControlDeseralizer ( C );
		}
		private void WindowsControlDeseralizer ( Window C )
		{
			if ( C == null )
				return;
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

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Normalization" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			myReoGridControl . CurrentWorksheet = myReoGridControl . Worksheets [ 0 ];
			var CW = myReoGridControl . CurrentWorksheet;
			if ( CW . Name == scratchy )
			{
				return;
			}

			var IsThisActualiedRange = CW . UsedRange;
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
					var Val = CW . GetCellData<double> ( row: i , col: j );
					double Normalized = Val * Norm;

					Scratcheroo . SetCellData ( row: i , col: j , data: Normalized );
				}
			}
			unvell . ReoGrid . NamedRange NamedDataRange =
				new unvell . ReoGrid . NamedRange ( worksheet: Scratcheroo , name: scratchy , range: DataRange );
			var exists = Scratcheroo . NamedRanges . Contains ( range: NamedDataRange );
			if ( !exists )
			{
				Scratcheroo . NamedRanges . Add ( NamedDataRange );
			}
			DoSaveSelectedData_Button . PerformClick ( );


			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Completed" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
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
				CBC . Click += this . CBC_Click;
				Scratcheroo . SetCellData ( row: j , col: 0 , data: CBC );

			}
			for ( int i = 0 ; i <= C ; i++ )
			{
				var CBC = new unvell . ReoGrid . CellTypes . CheckBoxCell ( );
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
			SwapRow ( Snatcheroo: Snatcheroo , CBC: CBC , R: R , r: r , colMax: colMax );
		}

		void SwapRow ( Worksheet Snatcheroo , CheckBoxCell CBC , int R , int r , int colMax )
		{
			RangePosition RR = new RangePosition ( row: R , col: 1 , rows: 1 , cols: colMax );

			Snatcheroo . InsertRows ( row: R , count: 1 );
			CellPosition cP = new CellPosition ( row: R , col: 0 );

			var action0 = new unvell . ReoGrid . Actions . CopyRangeAction ( fromRange: RR , toPosition: cP );
			myReoGridControl . DoAction ( action0 );
			var action1 = new unvell . ReoGrid . Actions . RemoveRowsAction ( row: R , rows: 1 );
			myReoGridControl . DoAction ( action1 );
		}

		#endregion SwapRow

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
			L . Content = myOpenGLControlViewportAspect . ToString ( "#0.0#" );
		}

		private void Aspect_Label_MouseLeave ( object sender , MouseEventArgs e )
		{
			Label L = sender as Label;
			DoAspect ( );
			L . Content = MainWindow . myOpenGLControlViewportAspect . ToString ( "#0.0#" );
		}

		private void StankyLineStartModeEnum_ComboBox_Control_Initialized ( object sender , EventArgs e )
		{
			ComboBox CB = sender as ComboBox;
						 
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void StankyLineStartModeEnum_ComboBox_Control_Loaded ( object sender , RoutedEventArgs e )
		{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private void StankyLineStartModeEnum_ComboBox_Control_ContextMenuClosing ( object sender , ContextMenuEventArgs e )
		{
			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

		}

		private void StankyLineStartModeEnum_ComboBox_Control_ContextMenuOpening ( object sender , ContextMenuEventArgs e )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void StankyLineStartModeEnum_ComboBox_Control_DataContextChanged ( object sender , DependencyPropertyChangedEventArgs e )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "snippy" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );
		}

		private void DoSaveSelectedData_Button_Click ( object sender , RoutedEventArgs e )
		{

			System . Diagnostics . Debug . WriteLine ( String . Format ( "{0} {1} " , "Starting Save Selected" , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			Worksheet Scratcheroo = myReoGridControl . GetWorksheetByName ( scratchy );
			if ( Scratcheroo == null )
			{
				return;
			}

			myReoGridControl . CurrentWorksheet = Scratcheroo;
			Worksheet . ReoGridRangeCollection R = Scratcheroo . Ranges;
			NamedRange named_ranges = Scratcheroo . NamedRanges [ scratchy ];

			Sheety = new NWorksheety ( );
			Sheety . r0 = named_ranges . StartPos . Row;
			Sheety . c0 = named_ranges . StartPos . Col;
			Sheety . r1 = named_ranges . Rows + Sheety . r0;
			Sheety . c1 = named_ranges . Cols + Sheety . c0;
			Sheety . cells = new float [ Sheety . c1 + 2 , Sheety . r1 + 2 ];

			System . Diagnostics . Debug . WriteLine ( String . Format ( "named ranged count {0} {1} " , named_ranges , ( ( ( System . Environment . StackTrace ) . Split ( '\n' ) ) [ 2 ] . Trim ( ) ) ) );

			for ( int j = named_ranges . StartPos . Col ; j < named_ranges . EndPos . Col ; j++ )
			{
				for ( int i = named_ranges . StartPos . Row ; i < named_ranges . EndPos . Row ; i++ )
				{
					Sheety . cells [ j , i ] = named_ranges . Cells [ row: i , col: j ] . GetData<float> ( );
				}
			}
			DoDrawReoGridSpreadsheet_CheckBox_Control . PerformClick ( );
			System . Diagnostics . Debug . WriteLine(String . Format ( "{0} {1} " ,"Completed", (((System . Environment . StackTrace).Split ('\n') )[2]. Trim ( ) ) ) );

		}

    }
}
 
