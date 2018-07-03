﻿using SharpGL;

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
			this . V = c [ i , j ];
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
		};
	}
