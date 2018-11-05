using System;
using System . Diagnostics;
using SharpGL;

namespace DMT01
{
	public class Edge
		{
		public enum EdgeDirection
		{
			ClockwiseNext,
			CounterClockwisePrevious
		}
		public short EdgeIndex;
		public short SubEdgeIndex;
		public Boolean IsSubEdge;
		public Vertex [ ] V = new Vertex [ 2 ];
		public Edge[] SubEdges;
		public short SubEdgeCount;
		public Vertex [ ] TweenVerte;
		public short TweenVerts;
		public Edge NextPeer;
		public Edge ParentEdge;
		public Edge PreviousEdge;
		public float Delta_V;
		public float[] delta_cf;
		public float[] Hit;
		public Boxel ParentBoxel;
		public Boxel AdjacentBoxel;

		public Edge ( )
			{
			}

		public Edge ( short EI , Vertex V0 , Vertex V1, Boxel Parent )
			{
			this . EdgeIndex = EI;
			this . V = new Vertex [ 2 ] { V0 , V1 };
			this . Delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
			this . delta_cf = new float [ 3 ] {
					this . V [ 1 ] . cf[0] - this . V [ 0 ] . cf[0],
					this . V [ 1 ] . cf[1] - this . V [ 0 ] . cf[1],
					this . V [ 1 ] . cf[2] - this . V [ 0 ] . cf[2],
				};
			this.ParentBoxel=Parent;
			}

		public Edge ( short EI , short subEdgeIndex , Vertex V0 , Vertex V1, Boxel Parent )
			{
			this . EdgeIndex = EI;
			this . V [ 0 ] = V0;
			this . V [ 1 ] = V1;
			this . Delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
			this . delta_cf = new float [ 3 ] {
					this . V [ 1 ] . cf[0] - this . V [ 0 ] . cf[0],
					this . V [ 1 ] . cf[1] - this . V [ 0 ] . cf[1],
					this . V [ 1 ] . cf[2] - this . V [ 0 ] . cf[2]
					};

			this . SubEdgeIndex = subEdgeIndex;
			this.ParentBoxel=Parent;
			}

		internal String DisplayMe ( )
			{
			if ( this . IsSubEdge )
				{
				String fomont = String . Format ( "E{0} sub {1} of {2} {3}" , this . EdgeIndex , this . SubEdgeIndex , this . SubEdgeCount , this . Showspan ( ) );
				return fomont;
				}
			String fomont1 = String . Format ( "E{0} {1}" , this . EdgeIndex , this . Showspan ( ) );
			return fomont1;
			}

		private String Showspan ( )
			{
			const String Stranger = "0.000";
			string WaddaDo = String . Format ( "d{0} V[0]={1} -> V[1]={2}" ,
				this . Delta_V . ToString ( Stranger ) ,
				this . V [ 0 ] . V . ToString ( Stranger ) ,
				this . V [ 1 ] . V . ToString ( Stranger ) );
			return WaddaDo;
			}
	
		public static Boolean Equals ( Edge E0 , Edge E1 )
		{
			Boolean VertexMatch01 = ( Equals ( E0 . V [ 0 ] , E1 . V [ 1 ] ) );
			Boolean VertexMatch10 = ( Equals ( E0 . V [ 1 ] , E1 . V [ 0 ] ) );
			Boolean EdgeMatch = VertexMatch01 && VertexMatch10;
			if ( false && EdgeMatch )
			{
				Debug . WriteLine ( "" );
				String Message = "B[{0},{1}*E{2}]*V[{3},{4}*{5}]={6}=B[{7},{8}*E{9}]*V[{10},{11}*{12}]";
				System . Diagnostics . Debug . WriteLine ( String . Format ( Message ,
					E0 . ParentBoxel . I , E0 . ParentBoxel . J , E0 . EdgeIndex ,
					E0 . V [ 0 ] . I , E0 . V [ 0 ] . J , E0 . V [ 0 ] . VertexIndex ,
					VertexMatch01 ,
					E1 . ParentBoxel . I , E1 . ParentBoxel . J , E1 . EdgeIndex ,
					E1 . V [ 1 ] . I , E1 . V [ 1 ] . J , E1 . V [ 1 ] . VertexIndex ) );
				System . Diagnostics . Debug . WriteLine ( String . Format ( Message ,
					E0 . ParentBoxel . I , E0 . ParentBoxel . J , E0 . EdgeIndex ,
					E0 . V [ 1 ] . I , E0 . V [ 1 ] . J , E0 . V [ 1 ] . VertexIndex ,
					VertexMatch10 ,
					E1 . ParentBoxel . I , E1 . ParentBoxel . J , E1 . EdgeIndex ,
					E1 . V [ 0 ] . I , E1 . V [ 0 ] . J , E1 . V [ 0 ] . VertexIndex ) );
				Debug . WriteLine ( "" );
			}
			return EdgeMatch;
		}

		public static Boolean Equals ( Vertex vertex1 , Vertex vertex2 )
		{
			Boolean cfs =  Equals ( vertex1 . cf , vertex2 . cf );
			Boolean vs = float . Equals ( vertex1 . V , vertex2 . V );
			Boolean VertexMatch = cfs && vs;
			return VertexMatch;
		}

		public static Boolean Equals ( float [ ] hit1 , float [ ] hit2 )
		{
			Boolean x = ( hit1 [ 0 ] == hit2 [ 0 ] );
			Boolean y = ( hit1 [ 1 ] == hit2 [ 1 ] );
			Boolean z = ( hit1 [ 2 ] == hit2 [ 2 ] );
			Boolean xyz = ( ( x && y ) && z );

			return xyz;
		}

		public static Edge GetExitEdge ( Edge EntryEdge )
		{
			Boxel EntryBoxel = EntryEdge . ParentBoxel;
			float [ ] EntryHit = EntryEdge . Hit;
			for ( int i = 0 ; i < 4 ; i++ )
			{
				Edge E = EntryBoxel . E [ i ];
				if ( E == null )
				{
					continue;
				}

				Boolean CongruentEdges =  Equals ( E , EntryEdge );
				if ( CongruentEdges )
				{
					continue;
				}

				if ( E . Hit == null )
				{
					continue;
				}

				float [ ] Hit = E . Hit;
				if ( Equals ( EntryHit , Hit ) )
				{
					continue;
				}

				if ( E . Hit != null )
				{
					return E;
				}
			}
			return null;
		}

		public static void AnnotateRecursion ( Edge E0 , Edge E1 , int depth )
		{

			if ( E0 == null )
			{
				return;
			}

			if ( E0 . ParentBoxel == null )
			{
				return;
			}

			Boxel B0 = E0 . ParentBoxel;
			if ( E1 == null )
			{
				return;
			}

			if ( E1 . ParentBoxel == null )
			{
				return;
			}

			Boxel B1 = E1 . ParentBoxel;

			if ( false )
			{
				System . Diagnostics . Debug . WriteLine ( String . Format ( "{6}:B[{0},{1}*E{2}]-{7}->B[{3},{4}*E{5}] " ,
				B0 . I , B0 . J , E0 . EdgeIndex ,
				B1 . I , B1 . J , E1 . EdgeIndex , depth , Boxel.EncodeBoxelAdjacency ( B0 , B1 ) ) );
			}
			if ( true )
			{
				OpenGL gl = MainWindow . staticGLHook;
				if ( gl == null )
				{
					return;
				}

				float [ ] Centroid1 = E0 . ParentBoxel . Centroid . cf;
				gl . PushAttrib ( SharpGL . Enumerations . AttributeMask . All );
				gl . Color ( 0 , .9 , .1 );
				gl . PointSize ( 6 );

				B0 . SmallerBoxAbout ( gl , .65f );
				gl . Color ( .9 , .9 , .1 );
				B1 . SmallerBoxAbout ( gl , .55f );
				gl . PopAttrib ( );
			}
		}

	}

}

