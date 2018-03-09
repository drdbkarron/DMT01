using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using MySimpleMath;

namespace Axis_Arrow_Grid
{
    public class Arrow_Class
    {
        private static Boolean ShowLocalAxis = true;
        public static void Arrow(OpenGL gl)
            {
            Arrow(gl, 1);
            }
        public static void Arrow(OpenGL gl, float zscale)
            {

            float r_Arrow_shaft_cylinder = 0.09f;
            float r_arrow_head = 0.20f;
            float step_radians = 30.0 * MySimpleMath.MySimpleMath.degrees_to_radians() ;
            float cyl_height = 0.50f;
            float cone_height = 0.5f;

            gl.PushMatrix();

            if (ShowLocalAxis)
                Axis_Class.LocalAxis(gl);

            ArrowShaftCylinder(gl, r_Arrow_shaft_cylinder, cyl_height + .05f, step_radians, zscale);

            gl.Translate(0, 0, (cone_height - .05f * 2f) * zscale);

            ArrowHeadCone(gl, r_arrow_head, cone_height + 0.05f, step_radians, zscale);
            gl.PopMatrix();
            }
        static void ArrowHeadCone(SharpGL.OpenGL gl, float r_arrow_head, float cone_height, double step, double zscale)
            {
            double stopper = Math.PI - step;
            for (double i = 0.0; i < stopper; i += step)
                {
                float[] Vertex0 = { r_arrow_head * (float)Math.Sin(i), r_arrow_head * (float)Math.Cos(i), 0.0f };
                float[] Vertex1 = { 0, 0, cone_height * zscale };

                double i0 = i + step;

                float[] Vertex2 = { r_arrow_head * (float)Math.Sin(i0), r_arrow_head * (float)Math.Cos(i0), 0.0f };
                float[] Vertex3 = { 0, 0, cone_height * zscale };

                Triangle.Draw(gl, Vertex0, Vertex1, Vertex2, true, false);
                Triangle.Draw(gl, Vertex1, Vertex2, Vertex3, false, false);
                }

            }
        static void ArrowShaftCylinder(SharpGL.OpenGL gl, float r_Arrow_shaft_cylinder, float cyl_height, double step, double zscale)
            {
            double stopper = Math.PI - step;
            for (double i = 0.0; i < stopper; i += step)
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

                Triangle.Draw(gl, Vertex0, Vertex1, Vertex2, true, false);
                Triangle.Draw(gl, Vertex1, Vertex2, Vertex3, false, false);
                }
            }
        }
    public static class Axis_Class
        {
        public static void MyGlobalAxis(SharpGL.OpenGL gl)
            {
            gl.PushAttrib(SharpGL.OpenGL.GL_CURRENT_BIT | SharpGL.OpenGL.GL_ENABLE_BIT | SharpGL.OpenGL.GL_LINE_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Disable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Disable(SharpGL.OpenGL.GL_TEXTURE_2D);
            gl.DepthFunc(SharpGL.OpenGL.GL_ALWAYS);

            gl.LineWidth(2f);

            int AxesLength = 2;
            String AxisLabelFont = "Times New Roman";
            float[] Red = { 1f, 0f, 0f, 1f };
            float[] Green = { 0f, 1f, 0f, 1f };
            float[] Blue = { 0f, 0f, 1f, 1f };
            float[] Origin = { 0f, 0f, 0f };
            //  Draw the axies and annotate the ends.

            gl.Color(Red);
            gl.Begin(SharpGL.OpenGL.GL_LINES);
            gl.Vertex(Origin);
            gl.Vertex(AxesLength, 0, 0);
            gl.End();

            gl.PushMatrix();
            gl.Translate(AxesLength, 0, 0);
            gl.DrawText3D(AxisLabelFont, 10.0f, 0.0f, 0.2f, @"+X");
            gl.PopMatrix();

            gl.Color(Green);
            gl.Begin(SharpGL.OpenGL.GL_LINES);
            gl.Vertex(Origin);
            gl.Vertex(0, AxesLength, 0);
            gl.End();

            gl.PushMatrix();
            gl.Translate(0, AxesLength, 0);
            gl.DrawText3D(AxisLabelFont, 10.0f, 0.0f, 0.1f, @"+Y");
            gl.PopMatrix();

            gl.Color(Blue);
            gl.Begin(SharpGL.OpenGL.GL_LINES);
            gl.Vertex(Origin);
            gl.Vertex(0, 0, AxesLength);
            gl.End();

            gl.PushMatrix();
            gl.Translate(0, 0, AxesLength);
            gl.DrawText3D(AxisLabelFont, 10.0f, 0.0f, 0.05f, @"+Z");
            gl.PopMatrix();

            //  Restore attributes.
            gl.PopAttrib();
            }
        public static void LocalAxis(OpenGL gl)
            {
            double[] Reddish = { 0.9, 0.3, 0.4 };
            double[] Greenish = { 0.1, 0.8, 0.3 };
            double[] Bluish = { 0.1, 0.2, 0.8 };

            gl.PushAttrib(SharpGL.OpenGL.GL_LIGHTING);
            gl.Disable(SharpGL.OpenGL.GL_LIGHTING);
            gl.LineWidth(3);

            gl.Color(Reddish);
            gl.Begin(SharpGL.Enumerations.BeginMode.LineLoop);
            gl.Vertex(0f, 0f, 0f);
            gl.Vertex(1f, 0f, 0f);
            gl.End();

            gl.Color(Greenish);
            gl.Begin(SharpGL.Enumerations.BeginMode.LineLoop);
            gl.Vertex(0f, 0f, 0f);
            gl.Vertex(0f, 1f, 0f);
            gl.End();

            gl.Color(Bluish);
            gl.Begin(SharpGL.Enumerations.BeginMode.LineLoop);
            gl.Vertex(0f, 0f, 0f);
            gl.Vertex(0f, 0f, 1f);
            gl.End();

            gl.PopAttrib();
            }
        }
    public static class Grid_Class
    {
        public static void MyGrid(SharpGL.OpenGL gl, int iLo, int iHi, int jLo, int jHi)
            {
            String ColumnAnnotationFont = "Times New Roman";
            int Ilo = 0;
            int Ihi = 10;
            int Jlo = 0;
            int Jhi = 10;

            gl.PushAttrib(SharpGL.OpenGL.GL_CURRENT_BIT | SharpGL.OpenGL.GL_ENABLE_BIT | SharpGL.OpenGL.GL_LINE_BIT);
            gl.Disable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Disable(SharpGL.OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(1.0f);

            //  Draw the grid lines.
            gl.Begin(SharpGL.OpenGL.GL_LINES);

            for (int i = iLo; i <= iHi; i++)
                {
                float fcol = ((i % 10) == 0) ? 0.3f : 0.15f;
                gl.Color(fcol, fcol, fcol);
                gl.Vertex(i, jLo, 0);
                gl.Vertex(i, jHi, 0);
                }
            for (int j = jLo; j <= jHi; j++)
                {
                float fcol = ((j % 10) == 0) ? 0.3f : 0.15f;
                gl.Vertex(iLo, j, 0);
                gl.Vertex(iHi, j, 0);
                }
            gl.End();
            // Draw Spreadsheet boarders

            for (int i = iLo; i < iHi; i++)
                {
                gl.PushMatrix();
                gl.Translate(i + 0.25, jLo - 0.5, 0);
                gl.Scale(0.5d, 0.5d, 0.5d);

                String Annotation = String.Format("{0,2} ", i);
                gl.DrawText3D(ColumnAnnotationFont, 8.0f, 0.0f, 0.1f, Annotation);
                gl.PopMatrix();
                }

            for (int i = iLo; i < iHi; i++)
                {
                gl.PushMatrix();
                gl.Translate(i + .25, jHi + 0.5, 0);
                gl.Scale(0.5d, 0.5d, 0.5d);
                String Annotation = String.Format("{0,2} ", i);
                gl.DrawText3D(ColumnAnnotationFont, 8.0f, 0.0f, 0.1f, Annotation);
                gl.PopMatrix();
                }

            for (int j = jLo; j < jHi; j++)
                {
                gl.PushMatrix();
                gl.Translate(iLo - 0.5, j + 0.25, 0);
                gl.Scale(0.5d, 0.5d, 0.5d);
                String Annotation = String.Format("{0,2} ", j + 1);
                gl.DrawText3D(ColumnAnnotationFont, 8.0f, 0.0f, 0.1f, Annotation);
                gl.PopMatrix();
                }

            for (int j = jLo; j < jHi; j++)
                {
                gl.PushMatrix();
                gl.Translate(iHi + 0.5, j + 0.25, 0);
                gl.Scale(0.5d, 0.5d, 0.5d);
                String Annotation = String.Format("{0,2} ", j + 1);
                gl.DrawText3D(ColumnAnnotationFont, 8.0f, 0.0f, 0.1f, Annotation);
                gl.PopMatrix();
                }

            gl.LineWidth(2);
            gl.Color(.5, .2, .1);
            gl.Begin(SharpGL.OpenGL.GL_LINE_LOOP);
            gl.Vertex(Jlo, Ilo);
            gl.Vertex(Jlo, Ihi + 1);
            gl.Vertex(Jhi + 1, Ihi + 1);
            gl.Vertex(Jhi + 1, Ilo);
            gl.End();

            gl.PopAttrib();
            }
        }
    }
