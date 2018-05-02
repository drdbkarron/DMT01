﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using MySimpleMath;
using DMT;
using SharpGL . SceneGraph . Core;
using SharpGL . SceneGraph;
using System . Xml . Serialization;
using SharpGL . SceneGraph . Primitives;
using DMT01;
using XamlGeneratedNamespace;

namespace Axis_Arrow_Grid
{
    public static class Arrow_Class
    {
        private static Boolean ShowLocalAxis = true;
        public static void Arrow ( OpenGL gl )
        {
            Arrow ( gl , 1 );
        }
        public static void Arrow ( OpenGL gl , float zscale )
        {

            float r_Arrow_shaft_cylinder = 0.09f;
            float r_arrow_head = 0.20f;
            float step_radians = 30.0f * MySimpleMath.MySimpleMath.degrees_to_radians() ;
            float cyl_height = 0.50f;
            float cone_height = 0.5f;

            gl . PushMatrix ( );


            if ( ShowLocalAxis )
                {
                Axis_Class . LocalAxis ( gl );
                }

            ArrowShaftCylinder ( gl , r_Arrow_shaft_cylinder , cyl_height + .05f , step_radians , zscale );

            gl . Translate ( 0 , 0 , ( cone_height - .05f * 2f ) * zscale );

            ArrowHeadCone ( gl , r_arrow_head , cone_height + 0.05f , step_radians , zscale );
            gl . PopMatrix ( );
        }
        static void ArrowHeadCone ( SharpGL . OpenGL gl , float r_arrow_head , float cone_height , float step , float zscale )
        {
            double stopper = Math.PI - step;
            for ( double i = 0.0 ; i < stopper ; i += step )
            {
                float[] Vertex0 = { r_arrow_head * (float)Math.Sin(i), r_arrow_head * (float)Math.Cos(i), 0.0f };
                float[] Vertex1 = { 0, 0, cone_height * zscale };

                double i0 = i + step;

                float[] Vertex2 = { r_arrow_head * (float)Math.Sin(i0), r_arrow_head * (float)Math.Cos(i0), 0.0f };
                float[] Vertex3 = { 0, 0, cone_height * zscale };
                Triangle . Draw ( gl , Vertex0 , Vertex1 , Vertex2 , true , false );
                Triangle . Draw ( gl , Vertex1 , Vertex2 , Vertex3 , false , false );
            }

        }
        static void ArrowShaftCylinder ( SharpGL . OpenGL gl , float r_Arrow_shaft_cylinder , float cyl_height , double step , double zscale )
        {
            double stopper = Math.PI - step;
            for ( double i = 0.0 ; i < stopper ; i += step )
            {
                float x = r_Arrow_shaft_cylinder * (float)Math.Sin(i);
                float y = r_Arrow_shaft_cylinder * (float)Math.Cos(i);
                float z = 0.0f;

                float x0 = x;
                float y0 = y;
                float z0 = cyl_height * (float)zscale;

                double i0 = i + step;

                float x1 = r_Arrow_shaft_cylinder * (float)Math.Sin(i0);
                float y1 = r_Arrow_shaft_cylinder * (float)Math.Cos(i0);
                float z1 = 0.0f;

                float x2 = x1;
                float y2 = y1;
                float z2 = cyl_height * (float)zscale;

                float[] Vertex0 = { x, y, z };
                float[] Vertex1 = { x0, y0, z0 };
                float[] Vertex2 = { x1, y1, z1 };
                float[] Vertex3 = { x2, y2, z2 };
                Triangle . Draw ( gl , Vertex0 , Vertex1 , Vertex2 , true , false );
                Triangle . Draw ( gl , Vertex1 , Vertex2 , Vertex3 , false , false );
            }
        }
    }
        public class CubeClass
    {
        public float rquad=0;
        public void DrawCube ( OpenGL gl )
        {
            gl . PushMatrix ( );
            gl . LoadIdentity ( );
            gl . Translate ( 1.5f , 1.0f , -7.0f );				// Move Right And Into The Screen

            gl . Rotate ( this . rquad , 1.0f , 1.0f , 1.0f );			// Rotate The Cube On X, Y & Z

            gl . Begin ( OpenGL . GL_QUADS );					// Start Drawing The Cube

            gl . Color ( 0.0f , 1.0f , 0.0f );			// Set The Color To Green
            gl . Vertex ( 1.0f , 1.0f , -1.0f );			// Top Right Of The Quad (Top)
            gl . Vertex ( -1.0f , 1.0f , -1.0f );			// Top Left Of The Quad (Top)
            gl . Vertex ( -1.0f , 1.0f , 1.0f );			// Bottom Left Of The Quad (Top)
            gl . Vertex ( 1.0f , 1.0f , 1.0f );			// Bottom Right Of The Quad (Top)


            gl . Color ( 1.0f , 0.5f , 0.0f );			// Set The Color To Orange
            gl . Vertex ( 1.0f , -1.0f , 1.0f );			// Top Right Of The Quad (Bottom)
            gl . Vertex ( -1.0f , -1.0f , 1.0f );			// Top Left Of The Quad (Bottom)
            gl . Vertex ( -1.0f , -1.0f , -1.0f );			// Bottom Left Of The Quad (Bottom)
            gl . Vertex ( 1.0f , -1.0f , -1.0f );			// Bottom Right Of The Quad (Bottom)

            gl . Color ( 1.0f , 0.0f , 0.0f );			// Set The Color To Red
            gl . Vertex ( 1.0f , 1.0f , 1.0f );			// Top Right Of The Quad (Front)
            gl . Vertex ( -1.0f , 1.0f , 1.0f );			// Top Left Of The Quad (Front)
            gl . Vertex ( -1.0f , -1.0f , 1.0f );			// Bottom Left Of The Quad (Front)
            gl . Vertex ( 1.0f , -1.0f , 1.0f );			// Bottom Right Of The Quad (Front)

            gl . Color ( 1.0f , 1.0f , 0.0f );			// Set The Color To Yellow
            gl . Vertex ( 1.0f , -1.0f , -1.0f );			// Bottom Left Of The Quad (Back)
            gl . Vertex ( -1.0f , -1.0f , -1.0f );			// Bottom Right Of The Quad (Back)
            gl . Vertex ( -1.0f , 1.0f , -1.0f );			// Top Right Of The Quad (Back)
            gl . Vertex ( 1.0f , 1.0f , -1.0f );			// Top Left Of The Quad (Back)

            gl . Color ( 0.0f , 0.0f , 1.0f );			// Set The Color To Blue
            gl . Vertex ( -1.0f , 1.0f , 1.0f );			// Top Right Of The Quad (Left)
            gl . Vertex ( -1.0f , 1.0f , -1.0f );			// Top Left Of The Quad (Left)
            gl . Vertex ( -1.0f , -1.0f , -1.0f );			// Bottom Left Of The Quad (Left)
            gl . Vertex ( -1.0f , -1.0f , 1.0f );			// Bottom Right Of The Quad (Left)

            gl . Color ( 1.0f , 0.0f , 1.0f );			// Set The Color To Violet
            gl . Vertex ( 1.0f , 1.0f , -1.0f );			// Top Right Of The Quad (Right)
            gl . Vertex ( 1.0f , 1.0f , 1.0f );			// Top Left Of The Quad (Right)
            gl . Vertex ( 1.0f , -1.0f , 1.0f );			// Bottom Left Of The Quad (Right)
            gl . Vertex ( 1.0f , -1.0f , -1.0f );			// Bottom Right Of The Quad (Right)
            gl . End ( );						// Done Drawing The Q
            gl . PopMatrix ( );
        }
        public void Rotate ( )
        {
            rquad -= 3.0f;// 0.15f;						// Decrease The Rotation Variable For The Quad 

        }
    }
        public class Tetrahedra_Class
    {
        public float rtri=0;
        public void DrawTetrahedra ( OpenGL gl )
        {
            gl . PushMatrix ( );
            gl . LoadIdentity ( );					// Reset The View
            gl . Translate ( -1.5f , 0.0f , -10.0f );
            //gl.Translate(-1.5f, 0.0f, -6.0f);				

            gl . Rotate ( rtri , 0.0f , 1.0f , 0.0f );				// Rotate The Pyramid On It's Y Axis

            gl . Begin ( OpenGL . GL_TRIANGLES );					// Start Drawing The Pyramid

            gl . Color ( 1.0f , 0.0f , 0.0f );			// Red
            gl . Vertex ( 0.0f , 1.0f , 0.0f );			// Top Of Triangle (Front)
            gl . Color ( 0.0f , 1.0f , 0.0f );			// Green
            gl . Vertex ( -1.0f , -1.0f , 1.0f );			// Left Of Triangle (Front)
            gl . Color ( 0.0f , 0.0f , 1.0f );			// Blue
            gl . Vertex ( 1.0f , -1.0f , 1.0f );			// Right Of Triangle (Front)

            gl . Color ( 1.0f , 0.0f , 0.0f );			// Red
            gl . Vertex ( 0.0f , 1.0f , 0.0f );			// Top Of Triangle (Right)
            gl . Color ( 0.0f , 0.0f , 1.0f );			// Blue
            gl . Vertex ( 1.0f , -1.0f , 1.0f );			// Left Of Triangle (Right)
            gl . Color ( 0.0f , 1.0f , 0.0f );			// Green
            gl . Vertex ( 1.0f , -1.0f , -1.0f );			// Right Of Triangle (Right)

            gl . Color ( 1.0f , 0.0f , 0.0f );			// Red
            gl . Vertex ( 0.0f , 1.0f , 0.0f );			// Top Of Triangle (Back)
            gl . Color ( 0.0f , 1.0f , 0.0f );			// Green
            gl . Vertex ( 1.0f , -1.0f , -1.0f );			// Left Of Triangle (Back)
            gl . Color ( 0.0f , 0.0f , 1.0f );			// Blue
            gl . Vertex ( -1.0f , -1.0f , -1.0f );			// Right Of Triangle (Back)

            gl . Color ( 1.0f , 0.0f , 0.0f );			// Red
            gl . Vertex ( 0.0f , 1.0f , 0.0f );			// Top Of Triangle (Left)
            gl . Color ( 0.0f , 0.0f , 1.0f );			// Blue
            gl . Vertex ( -1.0f , -1.0f , -1.0f );			// Left Of Triangle (Left)
            gl . Color ( 0.0f , 1.0f , 0.0f );			// Green
            gl . Vertex ( -1.0f , -1.0f , 1.0f );			// Right Of Triangle (Left)
            gl . End ( );						// Done Drawing The Pyramid
            gl . PopMatrix ( );

        }
        public void Rotate ( )
        {
            rtri += 3.0f;// 0.2f; }
        }
    }
    public static class Axis_Class
        {
        const String AxisLabelFont = "Times New Roman";
        static readonly float [ ] Origin = { 0f , 0f , 0f };
        static readonly float [ ] Red = { 1f , 0f , 0f , 1f };
        static readonly float [ ] Green = { 0f , 1f , 0f , 1f };
        static readonly float [ ] Blue = { 0f , 0f , 1f , 1f };
        static readonly float [ ] White = { 1f , 1f , 1f , 1f };

        public static void MyGlobalAxis ( SharpGL . OpenGL gl , int AxesLength , int LineWidth , int Pointsize , Boolean DoMinus ,
                Boolean TagOrigin = true , Boolean DoXYZAnnotation = true )
            {
            gl . PushAttrib ( SharpGL . OpenGL . GL_CURRENT_BIT | SharpGL . OpenGL . GL_ENABLE_BIT | SharpGL . OpenGL . GL_LINE_BIT | SharpGL . OpenGL . GL_DEPTH_BUFFER_BIT );

            gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
            gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
            gl . DepthFunc ( SharpGL . OpenGL . GL_ALWAYS );

            gl . LineWidth ( LineWidth );

            //  Draw the axis and annotate the ends.

            if ( TagOrigin )
                {
                gl . Color ( White );
                gl . PointSize ( Pointsize );
                gl . Vertex ( Origin );
                gl . DrawText3D ( AxisLabelFont , 10.0f , 0.0f , 0.2f , @"0" );
                }

            gl . Color ( Red );
            gl . Begin ( SharpGL . OpenGL . GL_LINES );
            gl . Vertex ( Origin );
            gl . Vertex ( AxesLength , 0 , 0 );
            gl . End ( );

            if ( DoMinus )
                {
                gl . Color ( Red );
                gl . Begin ( SharpGL . OpenGL . GL_LINES );
                gl . Vertex ( Origin );
                gl . Vertex ( -AxesLength , 0 , 0 );
                gl . End ( );
                }

            if ( DoXYZAnnotation )
                {
                gl . PushMatrix ( );
                gl . Translate ( AxesLength , 0 , 0 );
                gl . DrawText3D ( AxisLabelFont , 10.0f , 0.0f , 0.2f , @"+X" );
                gl . PopMatrix ( );
                }

            gl . Color ( Green );
            gl . Begin ( SharpGL . OpenGL . GL_LINES );
            gl . Vertex ( Origin );
            gl . Vertex ( 0 , AxesLength , 0 );
            gl . End ( );

            if ( DoMinus )
                {
                gl . Color ( Green );
                gl . Begin ( SharpGL . OpenGL . GL_LINES );
                gl . Vertex ( Origin );
                gl . Vertex ( 0 , -AxesLength , 0 );
                gl . End ( );
                }

            if ( DoXYZAnnotation )
                {
                gl . PushMatrix ( );
                gl . Translate ( 0 , AxesLength , 0 );
                gl . DrawText3D ( AxisLabelFont , 10.0f , 0.0f , 0.1f , @"+Y" );
                gl . PopMatrix ( );
                }

            gl . Color ( Blue );
            gl . Begin ( SharpGL . OpenGL . GL_LINES );
            gl . Vertex ( Origin );
            gl . Vertex ( 0 , 0 , AxesLength );
            gl . End ( );

            if ( DoMinus )
                {
                gl . Color ( Blue );
                gl . Begin ( SharpGL . OpenGL . GL_LINES );
                gl . Vertex ( Origin );
                gl . Vertex ( 0 , 0 , -AxesLength );
                gl . End ( );
                }

            if ( DoXYZAnnotation )
                {
                gl . PushMatrix ( );
                gl . Translate ( 0 , 0 , AxesLength );
                gl . DrawText3D ( AxisLabelFont , 10.0f , 0.0f , 0.05f , @"+Z" );
                gl . PopMatrix ( );
                }

            //  Restore attributes.
            gl . PopAttrib ( );
            }
        
            public static void LocalAxis ( OpenGL gl )
            {
                double[] Reddish = { 0.9, 0.3, 0.4 };
                double[] Greenish = { 0.1, 0.8, 0.3 };
                double[] Bluish = { 0.1, 0.2, 0.8 };

                gl . PushAttrib ( SharpGL . OpenGL . GL_LIGHTING );
                gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
                gl . LineWidth ( 3 );

                gl . Color ( Reddish );
                gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                gl . Vertex ( 0f , 0f , 0f );
                gl . Vertex ( 1f , 0f , 0f );
                gl . End ( );

                gl . Color ( Greenish );
                gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                gl . Vertex ( 0f , 0f , 0f );
                gl . Vertex ( 0f , 1f , 0f );
                gl . End ( );

                gl . Color ( Bluish );
                gl . Begin ( SharpGL . Enumerations . BeginMode . LineLoop );
                gl . Vertex ( 0f , 0f , 0f );
                gl . Vertex ( 0f , 0f , 1f );
                gl . End ( );

                gl . PopAttrib ( );
            }
        }
        public static class Grid_Class
        {
            public static void MyGrid ( SharpGL . OpenGL gl , int iLo , int iHi , int jLo , int jHi )
            {
                String ColumnAnnotationFont = "Times New Roman";
                int Ilo = 0;
                int Ihi = 10;
                int Jlo = 0;
                int Jhi = 10;

                gl . PushAttrib ( SharpGL . OpenGL . GL_CURRENT_BIT | SharpGL . OpenGL . GL_ENABLE_BIT | SharpGL . OpenGL . GL_LINE_BIT );
                gl . Disable ( SharpGL . OpenGL . GL_LIGHTING );
                gl . Disable ( SharpGL . OpenGL . GL_TEXTURE_2D );
                gl . LineWidth ( 1.0f );

                //  Draw the grid lines.
                gl . Begin ( SharpGL . OpenGL . GL_LINES );

                for ( int i = iLo ; i <= iHi ; i++ )
                {
                    float fcol = ((i % 10) == 0) ? 0.3f : 0.15f;
                    gl . Color ( fcol , fcol , fcol );
                    gl . Vertex ( i , jLo , 0 );
                    gl . Vertex ( i , jHi , 0 );
                }
                for ( int j = jLo ; j <= jHi ; j++ )
                {
                    float fcol = ((j % 10) == 0) ? 0.3f : 0.15f;
                    gl . Vertex ( iLo , j , 0 );
                    gl . Vertex ( iHi , j , 0 );
                }
                gl . End ( );
                // Draw Spreadsheet boarders

                for ( int i = iLo ; i < iHi ; i++ )
                {
                    gl . PushMatrix ( );
                    gl . Translate ( i + 0.25 , jLo - 0.5 , 0 );
                    gl . Scale ( 0.5d , 0.5d , 0.5d );

                    String Annotation = String.Format("{0,2} ", i);
                    gl . DrawText3D ( ColumnAnnotationFont , 8.0f , 0.0f , 0.1f , Annotation );
                    gl . PopMatrix ( );
                }

                for ( int i = iLo ; i < iHi ; i++ )
                {
                    gl . PushMatrix ( );
                    gl . Translate ( i + .25 , jHi + 0.5 , 0 );
                    gl . Scale ( 0.5d , 0.5d , 0.5d );
                    String Annotation = String.Format("{0,2} ", i);
                    gl . DrawText3D ( ColumnAnnotationFont , 8.0f , 0.0f , 0.1f , Annotation );
                    gl . PopMatrix ( );
                }

                for ( int j = jLo ; j < jHi ; j++ )
                {
                    gl . PushMatrix ( );
                    gl . Translate ( iLo - 0.5 , j + 0.25 , 0 );
                    gl . Scale ( 0.5d , 0.5d , 0.5d );
                    String Annotation = String.Format("{0,2} ", j + 1);
                    gl . DrawText3D ( ColumnAnnotationFont , 8.0f , 0.0f , 0.1f , Annotation );
                    gl . PopMatrix ( );
                }

                for ( int j = jLo ; j < jHi ; j++ )
                {
                    gl . PushMatrix ( );
                    gl . Translate ( iHi + 0.5 , j + 0.25 , 0 );
                    gl . Scale ( 0.5d , 0.5d , 0.5d );
                    String Annotation = String.Format("{0,2} ", j + 1);
                    gl . DrawText3D ( ColumnAnnotationFont , 8.0f , 0.0f , 0.1f , Annotation );
                    gl . PopMatrix ( );
                }

                gl . LineWidth ( 2 );
                gl . Color ( .5 , .2 , .1 );
                gl . Begin ( SharpGL . OpenGL . GL_LINE_LOOP );
                gl . Vertex ( Jlo , Ilo );
                gl . Vertex ( Jlo , Ihi + 1 );
                gl . Vertex ( Jhi + 1 , Ihi + 1 );
                gl . Vertex ( Jhi + 1 , Ilo );
                gl . End ( );

                gl . PopAttrib ( );
            }
        }
        /// <summary>
        /// The Grid design time primitive is displays a grid in the scene.
        /// </summary>
        public class Grid : SceneElement, IRenderable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Grid"/> class.
            /// </summary>
            public Grid ( )
            {
                Name = "Design Time Grid";
            }

            /// <summary>
            /// Render to the provided instance of OpenGL.
            /// </summary>
            /// <param name="gl">The OpenGL instance.</param>
            /// <param name="renderMode">The render mode.</param>
            public void Render ( OpenGL gl , RenderMode renderMode )
            {
                //  Design time primitives render only in design mode.
                if ( renderMode != RenderMode . Design )
                {
                return;
                }

            //  If we do not have the display list, we must create it.
            //  Otherwise, we can simple call the display list.
            if ( displayList == null )
                {
                CreateDisplayList ( gl );
                }
            else
                {
                displayList . Call ( gl );
                }
            }

            /// <summary>
            /// Creates the display list. This function draws the
            /// geometry as well as compiling it.
            /// </summary>
            private void CreateDisplayList ( OpenGL gl )
            {
                //  Create the display list. 
                displayList = new DisplayList ( );

                //  Generate the display list and 
                displayList . Generate ( gl );
                displayList . New ( gl , DisplayList . DisplayListMode . CompileAndExecute );

                //  Push attributes, set the color.
                gl . PushAttrib ( OpenGL . GL_CURRENT_BIT | OpenGL . GL_ENABLE_BIT |
                    OpenGL . GL_LINE_BIT );
                gl . Disable ( OpenGL . GL_LIGHTING );
                gl . Disable ( OpenGL . GL_TEXTURE_2D );
                gl . LineWidth ( 1.0f );

                //  Draw the grid lines.
                gl . Begin ( OpenGL . GL_LINES );
                for ( int i = -10 ; i <= 10 ; i++ )
                {
                    float fcol = ((i % 10) == 0) ? 0.3f : 0.15f;
                    gl . Color ( fcol , fcol , fcol );
                    gl . Vertex ( i , -10 , 0 );
                    gl . Vertex ( i , 10 , 0 );
                    gl . Vertex ( -10 , i , 0 );
                    gl . Vertex ( 10 , i , 0 );
                }
                gl . End ( );

                //  Restore attributes.
                gl . PopAttrib ( );

                //  End the display list.
                displayList . End ( gl );
            }

            /// <summary>
            /// The internal display list.
            /// </summary>
            [XmlIgnore]
            private DisplayList displayList;
        }

        public class Cube : Polygon
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PolyCubegon"/> class.
            /// </summary>
            public Cube ( )
            {
                //  Set the name.
                Name = "Cube";

                //  Create the cube geometry.
                CreateCubeGeometry ( );
            }

            /// <summary>
            /// This function makes a simple cube shape.
            /// </summary>
            private void CreateCubeGeometry ( )
            {
                UVs . Add ( new UV ( 0 , 0 ) );
                UVs . Add ( new UV ( 0 , 1 ) );
                UVs . Add ( new UV ( 1 , 1 ) );
                UVs . Add ( new UV ( 1 , 0 ) );

                //	Add the vertices.
                Vertices . Add ( new Vertex ( -1 , -1 , -1 ) );
                Vertices . Add ( new Vertex ( 1 , -1 , -1 ) );
                Vertices . Add ( new Vertex ( 1 , -1 , 1 ) );
                Vertices . Add ( new Vertex ( -1 , -1 , 1 ) );
                Vertices . Add ( new Vertex ( -1 , 1 , -1 ) );
                Vertices . Add ( new Vertex ( 1 , 1 , -1 ) );
                Vertices . Add ( new Vertex ( 1 , 1 , 1 ) );
                Vertices . Add ( new Vertex ( -1 , 1 , 1 ) );

                //	Add the faces.
                Face face = new Face(); //	bottom
                face . Indices . Add ( new Index ( 1 , 0 ) );
                face . Indices . Add ( new Index ( 2 , 1 ) );
                face . Indices . Add ( new Index ( 3 , 2 ) );
                face . Indices . Add ( new Index ( 0 , 3 ) );
                Faces . Add ( face );

                face = new Face ( );        //	top
                face . Indices . Add ( new Index ( 7 , 0 ) );
                face . Indices . Add ( new Index ( 6 , 1 ) );
                face . Indices . Add ( new Index ( 5 , 2 ) );
                face . Indices . Add ( new Index ( 4 , 3 ) );
                Faces . Add ( face );

                face = new Face ( );        //	right
                face . Indices . Add ( new Index ( 5 , 0 ) );
                face . Indices . Add ( new Index ( 6 , 1 ) );
                face . Indices . Add ( new Index ( 2 , 2 ) );
                face . Indices . Add ( new Index ( 1 , 3 ) );
                Faces . Add ( face );

                face = new Face ( );        //	left
                face . Indices . Add ( new Index ( 7 , 0 ) );
                face . Indices . Add ( new Index ( 4 , 1 ) );
                face . Indices . Add ( new Index ( 0 , 2 ) );
                face . Indices . Add ( new Index ( 3 , 3 ) );
                Faces . Add ( face );

                face = new Face ( );        // front
                face . Indices . Add ( new Index ( 4 , 0 ) );
                face . Indices . Add ( new Index ( 5 , 1 ) );
                face . Indices . Add ( new Index ( 1 , 2 ) );
                face . Indices . Add ( new Index ( 0 , 3 ) );
                Faces . Add ( face );

                face = new Face ( );        //	back
                face . Indices . Add ( new Index ( 6 , 0 ) );
                face . Indices . Add ( new Index ( 7 , 1 ) );
                face . Indices . Add ( new Index ( 3 , 2 ) );
                face . Indices . Add ( new Index ( 2 , 3 ) );
                Faces . Add ( face );
            }
        }
    }

