using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlmSharp;
using SharpGL;
using DMT01;

namespace LocalMaths
    {
    public class LocalMathsClass
        {
        public float Float;

        public static float GetMean (float [ ] s)
            {
            float r = s [ 0 ] + s [ 1 ] + s [ 2 ] + s [ 3 ];
            float R = r / 4f;
            return R;
            }

        public static float [ ] GetCentroid (int [ , ] c)
            {
            float r0 = c [ 0 , 0 ] + c [ 1 , 0 ] + c [ 2 , 0 ] + c [ 3 , 0 ];
            float r1 = c [ 0 , 1 ] + c [ 1 , 1 ] + c [ 2 , 1 ] + c [ 3 , 1 ];

            float [ ] r = new float [ 2 ] { r0 / 4f , r1 / 4f };
            return r;
            }

         public static float [ ] GetCentroid (float x0 , float y0 , float x1 , float y1)
            {
			const float z = 0.0f;
            float [ ] X0 = new float [ ] { x0 , y0 , z };
            float [ ] X1 = new float [ ] { x0 , y1 , z };
            float [ ] X2 = new float [ ] { x1 , y1 , z };
            float [ ] X3 = new float [ ] { x1 , y0 , z };

            float [ ] X = new float [ ] {
                (X0 [ 0 ] + X1 [ 0 ] + X2 [ 0 ] + X3 [ 0 ])/4.0F ,
                (X0 [ 1 ] + X1 [ 1 ] + X2 [ 1 ] + X3 [ 1 ])/4.0F ,
                (X0 [ 2 ] + X1 [ 2 ] + X2 [ 2 ] + X3 [ 2 ])/4.0F };

            return X;
            }

        public static string IntToLetter (int i)
            {
            if ( i < 0 )
                {
                return "XXX";
                }

            switch ( i )
                {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                case 3:
                    return "D";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "G";
                case 7:
                    return "H";
                case 8:
                    return "I";
                case 9:
                    return "J";
                case 10:
                    return "K";
                case 11:
                    return "L";
                case 12:
                    return "M";
                case 13:
                    return "N";
                case 15:
                    return "O";
                case 16:
                    return "P";
                case 17:
                    return "Q";
                case 18:
                    return "R";
                case 19:
                    return "S";
                case 20:
                    return "T";
                case 21:
                    return "U";
                case 22:
                    return "V";
                case 23:
                    return "W";
                case 24:
                    return "X";
                case 25:
                    return "Y";
                case 26:
                    return "Z";
                }
            return "???";
            }

        public static String ComparisonString (float a , float b)
            {
            if ( a == b )
                {
                return "==";
                }
            if ( a > b )
                {
                return ">";
                }
            if ( a < b )
                {
                return "<";
                }
            return "??";
            }

        private void LoadMatrix (SharpGL . OpenGL gl , mat4 m4)
            {
            vec4 c0 = m4 . Column0;
            vec4 c1 = m4 . Column1;
            vec4 c2 = m4 . Column2;
            vec4 c3 = m4 . Column3;

            double [ ] m = new double [ 16 ]{ c0 [ 0 ] , c0 [ 1 ] , c0 [ 2 ] , c0 [ 3 ] ,
                                       c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] , c1 [ 3 ] ,
                                       c2 [ 0 ] , c2 [ 1 ] , c2 [ 2 ] , c2 [ 3 ] ,
                                       c3 [ 0 ] , c3 [ 1 ] , c3 [ 2 ] , c3 [ 3 ] };

            gl . LoadMatrix ( m );

            //LoadMatrix ( c0 [ 0 ] , c0 [ 1 ] , c0 [ 2 ] , c0 [ 3 ] ,
            //                           c1 [ 0 ] , c1 [ 1 ] , c1 [ 2 ] , c1 [ 3 ] ,
            //                           c2 [ 0 ] , c2 [ 1 ] , c2 [ 2 ] , c2 [ 3 ] ,
            //                           c3 [ 0 ] , c3 [ 1 ] , c3 [ 2 ] , c3 [ 3 ]
            //                         );
            }

        //public  GlmSharp . mat4 LookAt (OpenGL gl, Boolean LookAt_X_Up_RadioButton_Control=false, Boolean LookAt_Y_Up_RadioButton_Control=false,
        //    Boolean LookAt_Z_Up_RadioButton_Control=true,
        //    float LookAt_Eye_X_H_Slider_UserControl=0.0f ,
        //    float LookAt_Eye_Y_H_Slider_UserControl = 0.0f ,
        //    float LookAt_Eye_Z_H_Slider_UserControl = 0.0f , float LookAt_Target_X_H_Slider_UserControl = 0.0f ,
        //    float LookAt_Target_Y_H_Slider_UserControl = 0.0f ,
        //    float LookAt_Target_Z_H_Slider_UserControl = 0.0f )
        //    {
        //    float x_up = 0.0f;
        //    float y_up = 1.0f;
        //    float z_up = 0.0f;

        //    if ( LookAt_X_Up_RadioButton_Control )
        //        {
        //        x_up = 1.0f;
        //        }
        //    else
        //        {
        //        x_up = 0.0f;
        //        }
        //    if ( LookAt_Y_Up_RadioButton_Control)
        //        {
        //        y_up = 1.0f;
        //        }
        //    else
        //        {
        //        y_up = 0.0f;
        //        }
        //    if ( LookAt_Z_Up_RadioButton_Control)
        //        {
        //        z_up = 1.0f;
        //        }
        //    else
        //        {
        //        z_up = 0.0f;
        //        }

        //    GlmSharp . vec3 eye = new GlmSharp . vec3 (
        //        LookAt_Eye_X_H_Slider_UserControl ,
        //        LookAt_Eye_X_H_Slider_UserControl,
        //        LookAt_Eye_Z_H_Slider_UserControl  );

        //    GlmSharp . vec3 target = new GlmSharp . vec3 (
        //        LookAt_Target_X_H_Slider_UserControl ,
        //        LookAt_Target_Y_H_Slider_UserControl ,
        //        LookAt_Target_Z_H_Slider_UserControl );

        //    GlmSharp . vec3 up = new GlmSharp . vec3 (
        //        x_up , y_up , z_up );

        //    GlmSharp . mat4 M = GlmSharp . mat4 . LookAt ( eye , target , up );

        //    //gl.LookAt(
        //    //    LookAt_Eye_X_H_Slider_UserControl.SliderValue, 
        //    //    LookAt_Eye_Y_H_Slider_UserControl.SliderValue, 
        //    //    LookAt_Eye_Z_H_Slider_UserControl.SliderValue,
        //    //    LookAtTarget_X_H_Slider_UserControl.SliderValue, 
        //    //    LookAtTarget_Y_H_Slider_UserControl.SliderValue, 
        //    //    LookAtTarget_Z_H_Slider_UserControl.SliderValue,
        //    //    x_up, y_up, z_up);

        //    return M;
        //    }

        //public mat4 Perspective (OpenGL gl,
        //        float Perspective_FOVY_H_Slider_UserControl ,
        //        float Perspective_ASPECT_H_Slider_UserControl ,
        //        float Perspective_Z_NEAR_H_Slider_UserControl ,
        //        float Perspective_Z_FAR_H_Slider_UserControl)
        //    {
        //    //(double fovy, double aspect, double zNear, double zFar)

        //   mat4 M = GlmSharp . mat4 . Perspective ( Perspective_FOVY_H_Slider_UserControl ,
        //        Perspective_ASPECT_H_Slider_UserControl ,
        //        Perspective_Z_NEAR_H_Slider_UserControl ,
        //        Perspective_Z_FAR_H_Slider_UserControl );

        //    //Perspective ( Perspective_FOVY_H_Slider_UserControl ,
        //    //    Perspective_ASPECT_H_Slider_UserControl ,
        //    //    Perspective_Z_NEAR_H_Slider_UserControl ,
        //    //    Perspective_Z_FAR_H_Slider_UserControl );

        //    //gl . Perspective ( Perspective_FOVY_H_Slider_UserControl ,
        //    //     Perspective_ASPECT_H_Slider_UserControl ,
        //    //     Perspective_Z_NEAR_H_Slider_UserControl ,
        //    //     Perspective_Z_FAR_H_Slider_UserControl );

        //    return M;
        //    }
        }
    }