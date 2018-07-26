namespace DMT01
{
	public class IsoEdge : Edge
		{
		public float IsoEdgeIsoValue;
		public Edge E = new Edge ( );

		public IsoEdge ( float IsoValue , Vertex V0 , Vertex V1 )
			{
			//E = new Edge ( );
			this . IsoEdgeIsoValue = IsoValue;
			this . E . V [ 0 ] = V0;
			this . E . V [ 1 ] = V1;
			this . Delta_V = this . V [ 1 ] . V - this . V [ 0 ] . V;
			}

		public IsoEdge ( short v1 , short v2 , float IsoValue , Vertex V0 , Vertex V1 )
			{
			this .E = new Edge
			{
				EdgeIndex = v1 ,
				SubEdgeIndex = v2
			};
			this . IsoEdgeIsoValue = IsoValue;
			this . E . V [ 0 ] = V0;
			this . E . V [ 1 ] = V1;
			this . Delta_V = this . E . V [ 1 ] . V - this . E . V [ 0 ] . V;   // should be zero
			}
		}
	}

