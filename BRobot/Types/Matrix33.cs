﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRobot
{

    //  ███╗   ███╗ █████╗ ████████╗██████╗ ██╗██╗  ██╗██████╗ ██████╗ 
    //  ████╗ ████║██╔══██╗╚══██╔══╝██╔══██╗██║╚██╗██╔╝╚════██╗╚════██╗
    //  ██╔████╔██║███████║   ██║   ██████╔╝██║ ╚███╔╝  █████╔╝ █████╔╝
    //  ██║╚██╔╝██║██╔══██║   ██║   ██╔══██╗██║ ██╔██╗  ╚═══██╗ ╚═══██╗
    //  ██║ ╚═╝ ██║██║  ██║   ██║   ██║  ██║██║██╔╝ ██╗██████╔╝██████╔╝
    //  ╚═╝     ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚═╝  ╚═╝╚═════╝ ╚═════╝ 
    //                                                                

    /// <summary>
    /// A class representing a 3x3 rotation matrix
    /// </summary>
    public class Matrix33 : Geometry
    {
        /// <summary>
        /// Elements of the rotation matrix, ordered in row to column way, 
        /// i.e. r[2] is r13 (row 1 col 3), r[6] is r31, etc. 
        /// </summary>
        public double[] R { get; internal set; }


        /// <summary>
        /// Alias
        /// </summary>
        public double m00 { get { return this.R[0]; } internal set { R[0] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m01 { get { return this.R[1]; } internal set { R[1] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m02 { get { return this.R[2]; } internal set { R[2] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m10 { get { return this.R[3]; } internal set { R[3] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m11 { get { return this.R[4]; } internal set { R[4] = value; } }
        
        /// <summary>
        /// Alias
        /// </summary>
        public double m12 { get { return this.R[5]; } internal set { R[5] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m20 { get { return this.R[6]; } internal set { R[6] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m21 { get { return this.R[7]; } internal set { R[7] = value; } }

        /// <summary>
        /// Alias
        /// </summary>
        public double m22 { get { return this.R[8]; } internal set { R[8] = value; } }

        /// <summary>
        /// Create a 3x3 identity matrix representing no rotation.
        /// </summary>
        public Matrix33()
        {
            R = new double[9];
            this.R[0] = 1;
            this.R[4] = 1;
            this.R[8] = 1;
        }

        /// <summary>
        /// Create a 3x3 Rotation Matrix from it's constituent components. 
        /// Elements of the rotation matrix, ordered in row to column way, 
        /// i.e. r[2] is r13 (row 1 col 3), r[6] is r31, etc. 
        /// This Matrix will be reorthogonalized if necessary.
        /// </summary>
        /// <param name="r00"></param>
        /// <param name="r01"></param>
        /// <param name="r02"></param>
        /// <param name="r10"></param>
        /// <param name="r11"></param>
        /// <param name="r12"></param>
        /// <param name="r20"></param>
        /// <param name="r21"></param>
        /// <param name="r22"></param>
        public Matrix33(double r00, double r01, double r02,
                        double r10, double r11, double r12,
                        double r20, double r21, double r22)
        {
            this.Initialize(r00, r01, r02,
                            r10, r11, r12,
                            r20, r21, r22, true);
        }

        /// <summary>
        /// Create a 3x3 Rotation Matrix from it's constituent components. 
        /// Elements of the rotation matrix, ordered in row to column way, 
        /// i.e. r[2] is r13 (row 1 col 3), r[6] is r31, etc. 
        /// </summary>
        /// <param name="rotationValues"></param>
        public Matrix33(double[] rotationValues)
        {
            R = new double[9];

            int i;
            for (i = 0; i < rotationValues.Length || i < 9; i++)
            {
                R[i] = rotationValues[i];
            } 

            // If rotationValues had less than 9 elements
            while (i < 9)
            {
                R[i++] = 0;
            }
        }

        /// <summary>
        /// Create a 3x3 Rotation Matrix as a shallow copy of another.
        /// </summary>
        /// <param name="rotationMatrix"></param>
        public Matrix33(Matrix33 rotationMatrix)
        {
            R = new double[9];

            // Make a shallow copy
            for (var i = 0; i < 9; i++)
            {
                R[i] = rotationMatrix.R[i];
            }
        }

        public Matrix33(Point vecX, Point vecY)
        {
            // Some sanity
            int dir = Point.CompareDirections(vecX, vecY);

            if (dir == 1 || dir == 3)
            {
                throw new Exception("Cannot create a Rotation Matrix with two parallel vectors");
            }

            Point XAxis, YAxis, ZAxis;

            // Create unit X axis
            XAxis = new Point(vecX);
            XAxis.Normalize();

            // Find normal vector to plane
            ZAxis = Point.CrossProduct(vecX, vecY);
            ZAxis.Normalize();

            // Y axis is the cross product of both
            YAxis = Point.CrossProduct(ZAxis, XAxis);

            // Initialize the Matrix
            this.Initialize(XAxis.X, YAxis.X, ZAxis.X,
                            XAxis.Y, YAxis.Y, ZAxis.Y,
                            XAxis.Z, YAxis.Z, ZAxis.Z, false);
        }


        /// <summary>
        /// An internal initializator to start this matrix from its components. 
        /// The method allows for optional re-orthogonalization of this Matrix.
        /// </summary>
        /// <param name="r00"></param>
        /// <param name="r01"></param>
        /// <param name="r02"></param>
        /// <param name="r10"></param>
        /// <param name="r11"></param>
        /// <param name="r12"></param>
        /// <param name="r20"></param>
        /// <param name="r21"></param>
        /// <param name="r22"></param>
        internal void Initialize(double r00, double r01, double r02,
                                    double r10, double r11, double r12,
                                    double r20, double r21, double r22, bool orthonogonalize)
        {
            R = new double[9];

            this.R[0] = r00;
            this.R[1] = r01;
            this.R[2] = r02;
            this.R[3] = r10;
            this.R[4] = r11;
            this.R[5] = r12;
            this.R[6] = r20;
            this.R[7] = r21;
            this.R[8] = r22;

            if (orthonogonalize)
            {
                this.Orthogonalize();
            }
        }
        
        public bool Orthogonalize()
        {
            //@TODOL: implement!
            return false;
        }

        public bool IsOrthogonal()
        {
            // @TODO: implement!
            return false;
        }

        /// <summary>
        /// Returns the determinant of this Matrix.
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            return R[0] * R[4] * R[8]
                + R[1] * R[5] * R[6]
                + R[2] * R[3] * R[7]
                - R[0] * R[5] * R[7]
                - R[1] * R[3] * R[8]
                - R[2] * R[4] * R[6];
        }

        /// <summary>
        /// Transposes this Matrix.
        /// </summary>
        public void Transpose()
        {
            double old01 = R[1],
                old02 = R[2],
                old12 = R[5];

            this.R[1] = R[3];
            this.R[2] = R[6];
            this.R[5] = R[7];

            this.R[3] = old01;
            this.R[6] = old02;
            this.R[7] = old12;
        }

        /// <summary>
        /// Multiplies two rotation matrices.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix33 Multiply(Matrix33 m1, Matrix33 m2)
        {
            Matrix33 m = new Matrix33();

            m.R[0] = m1.R[0] * m2.R[0] + m1.R[1] * m2.R[3] + m1.R[2] * m2.R[6];
            m.R[1] = m1.R[0] * m2.R[1] + m1.R[1] * m2.R[4] + m1.R[2] * m2.R[7];
            m.R[2] = m1.R[0] * m2.R[2] + m1.R[1] * m2.R[5] + m1.R[2] * m2.R[8];
            m.R[3] = m1.R[3] * m2.R[0] + m1.R[4] * m2.R[3] + m1.R[5] * m2.R[6];
            m.R[4] = m1.R[3] * m2.R[1] + m1.R[4] * m2.R[4] + m1.R[5] * m2.R[7];
            m.R[5] = m1.R[3] * m2.R[2] + m1.R[4] * m2.R[5] + m1.R[5] * m2.R[8];
            m.R[6] = m1.R[6] * m2.R[0] + m1.R[7] * m2.R[3] + m1.R[8] * m2.R[6];
            m.R[7] = m1.R[6] * m2.R[1] + m1.R[7] * m2.R[4] + m1.R[8] * m2.R[7];
            m.R[8] = m1.R[6] * m2.R[2] + m1.R[7] * m2.R[5] + m1.R[8] * m2.R[8];

            return m;
        }

        /// <summary>
        /// Returns a Quaternion representing the same rotation as this Matrix.
        /// </summary>
        /// <returns></returns>
        public Quaternion ToQuaternion()
        {
            // This conversion assumes the rotation matrix is special orthogonal .
            // As a result, the returned Quaternion will be a versor.
            // Based on http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            double trace = m00 + m11 + m22;
            Quaternion q = new Quaternion();
            double s;

            // Compute a regular conversion
            if (trace > EPSILON)
            {
                s = 2 * Math.Sqrt(trace + 1);
                q.W = 0.25 * s;
                q.X = (m21 - m12) / s;
                q.Y = (m02 - m20) / s;
                q.Z = (m10 - m01) / s;
            }

            // If trace is close to zero, avoid division by zero and floating-point degeneracy
            // by searching which major diagonal element has the greatest value:
            else
            {
                if (m00 > m11 && m00 > m22)
                {
                    s = 2 * Math.Sqrt(1 + m00 - m11 - m22);
                    q.W = (m21 - m12) / s;
                    q.X = 0.25 * s;
                    q.Y = (m01 + m10) / s;
                    q.Z = (m02 + m20) / s;
                }
                else if (m11 > m22)
                {
                    s = 2 * Math.Sqrt(1 + m11 - m00 - m22);
                    q.W = (m02 - m20) / s;
                    q.X = (m01 + m10) / s;
                    q.Y = 0.25 * s;
                    q.Z = (m12 + m21) / s;
                }
                else
                {
                    s = 2 * Math.Sqrt(1 + m22 - m00 - m11);
                    q.W = (m10 - m01) / s;
                    q.X = (m02 + m20) / s;
                    q.Y = (m12 + m21) / s;
                    q.Z = 0.25 * s;
                }
            }

            return q;
        }

        public override string ToString()
        {
            return string.Format("RotationMatrix[[{0}, {1}, {2}], [{3}, {4}, {5}], [{6}, {7}, {8}]]",
                Math.Round(m00, STRING_ROUND_DECIMALS_MM),
                Math.Round(m01, STRING_ROUND_DECIMALS_MM),
                Math.Round(m02, STRING_ROUND_DECIMALS_MM),
                Math.Round(m10, STRING_ROUND_DECIMALS_MM),
                Math.Round(m11, STRING_ROUND_DECIMALS_MM),
                Math.Round(m12, STRING_ROUND_DECIMALS_MM),
                Math.Round(m20, STRING_ROUND_DECIMALS_MM),
                Math.Round(m21, STRING_ROUND_DECIMALS_MM),
                Math.Round(m22, STRING_ROUND_DECIMALS_MM));
        }

    }
}
