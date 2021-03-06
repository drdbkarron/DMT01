﻿using System;
using SharpGL;

namespace DMT01
{	public class Vertex
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
			var Eye = c . GetLength ( 0 ) ;
			var Jay = c . GetLength ( 1 ) ;
			if ( i > Eye )
			{
				this . V = 0 ;
				return ;
			}
			if ( j > Jay )
			{
				this . V = 0 ;
				return ;
			}
			float C = c [ i , j ];
			this . V =C;
			}

		internal void Draw ( )
			{
			if ( MainWindow . staticGLHook == null )
				{
				return;
				}

			if ( this . cf == null )
				{
				return;
				}

			OpenGL gl = MainWindow.staticGLHook;
			gl . Begin ( SharpGL . Enumerations . BeginMode . Points );
			gl . Vertex ( this . cf );
			gl . End ( );
			}
		public Boolean equals ( Vertex vertex1 , Vertex vertex2 )
		{
			Boolean cfs = equals ( vertex1 . cf , vertex2 . cf );
			Boolean vs = float . Equals ( vertex1 . V , vertex2 . V );
			Boolean VertexMatch = cfs && vs;
			return VertexMatch;
		}
		public Boolean equals ( float [ ] hit1 , float [ ] hit2 )
		{
		Boolean x = ( hit1 [ 0 ] == hit2 [ 0 ] );
		Boolean y = ( hit1 [ 1 ] == hit2 [ 1 ] );
		Boolean z = ( hit1 [ 2 ] == hit2 [ 2 ] );
		Boolean xyz = ( ( x && y ) && z );

		return xyz;
		}
	};

}

