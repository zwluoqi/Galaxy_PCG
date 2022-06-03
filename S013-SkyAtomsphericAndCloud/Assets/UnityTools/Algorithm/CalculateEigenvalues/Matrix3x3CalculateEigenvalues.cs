// using System.Numerics;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Vector3 = UnityEngine.Vector3;
using System;
using UnityTools.Math.Matrix;

//https://en.wikipedia.org/wiki/Eigenvalue_algorithm#3×3_matrices
//https://metric.ma.ic.ac.uk/metric_public/matrices/eigenvalues_and_eigenvectors/eigenvalues2.html

namespace UnityTools.Algorithm.CalculateEigenvalues
{
    public class Matrix3x3CalculateEigenvalues
    {
        public static void DiagonalizeSymmetric3X3(float[,] A, out float[] eigenvalues, out float[,] eigenvectors)
        {
            var engineVal = CalculateEigenvalues(A);
            eigenvalues = new float[3];
            eigenvalues[0] = engineVal[0];
            eigenvalues[1] = engineVal[1];
            eigenvalues[2] = engineVal[2];

            eigenvectors = new float[3,3];
            var rx = new Vector3(A[0, 0], A[0, 1], A[0, 2]);
            var ry = new Vector3(A[1, 0], A[1, 1], A[1, 2]);
            var rz = new Vector3(A[2, 0], A[2, 1], A[2, 2]);
            
            var engineVec0 = EigenVector(rx,ry,rz,eigenvalues[0]);
            var engineVec1 = EigenVector(rx,ry,rz,eigenvalues[1]);
            var engineVec2 = EigenVector(rx,ry,rz,eigenvalues[2]);
            eigenvectors[0, 0] = engineVec0[0];
            eigenvectors[0, 1] = engineVec0[0];
            eigenvectors[0, 2] = engineVec0[0];
            
            eigenvectors[1, 0] = engineVec1[0];
            eigenvectors[1, 1] = engineVec1[0];
            eigenvectors[1, 2] = engineVec1[0];
            
            eigenvectors[2, 0] = engineVec2[0];
            eigenvectors[2, 1] = engineVec2[0];
            eigenvectors[2, 2] = engineVec2[0];
            return;
        }


        private static Vector3 CalculateEigenvalues(float[,] A)
        {
            Vector3 val = new Vector3(0, 0, 0);

            float p1 = A[0,1] * A[0,1] + A[0,2] * A[0,2] + A[1,2] * A[1,2];
            if (p1 == 0)
            {
                val.x = A[0,0];
                val.y = A[1,1];
                val.z = A[2,2];
            }
            else
            {
                float q = TraceSum(A) / 3f;
                float p2 = (float)(System.Math.Pow(A[0,0] - q, 2) + System.Math.Pow(A[1,1] - q, 2) + System.Math.Pow(A[2,2] - q, 2)) + 2 * p1;
                float p = (float)System.Math.Sqrt(p2 / 6);
                Matrix4x4 I = Matrix4x4.Identity;
                Matrix4x4 tmp = Matrix4x4.Multiply(I,q);
                Matrix4x4 matrix4X4A = new Matrix4x4(A[0, 0], A[0, 1], A[0, 2], 0.0f,
                    A[0, 0], A[0, 1], A[0, 2], 0.0f,
                    A[0, 0], A[0, 1], A[0, 2], 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f
                );
                Matrix4x4 tmp2 = Matrix4x4.Subtract(matrix4X4A,tmp);
                Matrix4x4 B = Matrix4x4.Multiply(tmp2, 1 / p);
                Matrix3x3 b = new Matrix3x3((B.ToUnityEngineMatrix4x4()));
                
                // B.GetDeterminant(B);
                float r = b.GetDeterminant() / 2;

                float phi = 0;
                if (r <= -1)
                    phi = (float)System.Math.PI / 3;
                else if (r >= 1)
                    phi = 0;
                else
                    phi = (float)System.Math.Acos(r) / 3;
                val.x = q + 2 * p * (float)System.Math.Cos(phi);
                val.z = q + 2 * p * (float)System.Math.Cos(phi + (2 * System.Math.PI / 3));
                val.y = 3 * q - val.x - val.z;
            }
            return val;
        }

        private static float TraceSum(float[,] floats)
        {
            return floats[0, 0] + floats[1, 1] + floats[2, 2];
        }
        
        
        // Observe that the function doesn't use rZ,
        // it is expected that it will become zero vector in triangular form
        private static Vector3 EigenVector(Vector3 rX, Vector3 rY, Vector3 rZ, float lambda)
        {
            // Move RHS to LHS
            rX.x -= lambda;
            rY.y -= lambda;
            // Transform to upper triangle
            rY -= rX * (rY.x / rX.x);
            // Backsubstitute
            var res = Vector3.one;
            res.y = -rY.z / rY.y;
            res.x = -(rX.y * res.y + rX.z * res.z) / rX.x;
            return res;
        }
    }
}