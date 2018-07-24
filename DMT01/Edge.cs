using System;

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
		public float delta_V;
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
			this . delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
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
			this . delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
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
				this . delta_V . ToString ( Stranger ) ,
				this . V [ 0 ] . V . ToString ( Stranger ) ,
				this . V [ 1 ] . V . ToString ( Stranger ) );
			return WaddaDo;
			}
		}
	}

