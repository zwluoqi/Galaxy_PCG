using System;

namespace UnityTools.SimpleQRAlgorithm
{
public static class LinearAlgebra
    {
        /// <summary>
        /// Gets the j-th column of the given matrix.
        /// </summary>
        /// <param name="A">The matrix.</param>
        /// <param name="j">The index of the column.</param>
        /// <returns>The column vector.</returns>
        public static float[] GetColumn(float[,] A, int j)
        {
            int n = A.GetLength(0);
            float[] column = new float[n];

            for (int i = 0; i < n; i++)
            {
                column[i] = A[i, j];
            }

            return column;
        }

        /// <summary>
        /// Duplicates the given matrix.
        /// </summary>
        /// <param name="A">The matrix to duplicate.</param>
        /// <returns>The duplicated matrix.</returns>
        public static float[,] Duplicate(float[,] A)
        {
            int n = A.GetLength(0);
            float[,] B = new float[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    B[i, j] = A[i, j];
                }
            }

            return B;
        }

        /// <summary>
        /// Scales the given vector by a scalar.
        /// </summary>
        /// <param name="a">The vector to scale.</param>
        /// <param name="s">The scalar.</param>
        /// <returns></returns>
        public static float[] Scale(float[] a, float s)
        {
            int n = a.Length;
            float[] b = new float[n];

            for (int i = 0; i < n; i++)
            {
                b[i] = s * a[i];
            }
            return b;
        }

        /// <summary>
        /// Calculates the inner product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The inner product value.</returns>
        public static float InnerProduct(float[] a, float[] b)
        {
            float innerProduct = 0;

            for (int i = 0; i < a.Length; i++)
            {
                innerProduct += a[i] * b[i];
            }

            return innerProduct;
        }

        /// <summary>
        /// Calculates the outer product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The outer product matrix.</returns>
        public static float[,] OuterProduct(float[] a, float[] b)
        {
            int n = a.Length;
            float[,] outerProduct = new float[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    outerProduct[i, j] = a[i] * b[j];
                }
            }

            return outerProduct;
        }

        /// <summary>
        /// Projects the vector "a" orthogonally onto vector "u".
        /// </summary>
        /// <param name="a">The vector to project.</param>
        /// <param name="b">The vector on which will be projected.</param>
        /// <returns>The projection.</returns>
        public static float[] Project(float[] a, float[] b)
        {
            return Scale(a, InnerProduct(a, b) / InnerProduct(a, a));
        }

        /// <summary>
        /// Substracts vector "b" from vector "a".
        /// </summary>
        /// <param name="a">The vector to be subtracted.</param>
        /// <param name="b">The substracting vector.</param>
        /// <returns>The substracted vector.</returns>
        public static float[] Subtract(float[] a, float[] b)
        {
            int n = a.Length;
            float[] c = new float[n];

            for (int i = 0; i < n; i++)
            {
                c[i] = a[i] - b[i];
            }

            return c;
        }

        /// <summary>
        /// Adds vector "b" from vector "a".
        /// </summary>
        /// <param name="a">The vector to be added.</param>
        /// <param name="b">The adding vector.</param>
        /// <returns>The added vector.</returns>
        public static float[] Add(float[] a, float[] b)
        {
            int n = a.Length;
            float[] c = new float[n];

            for (int i = 0; i < n; i++)
            {
                c[i] = a[i] + b[i];
            }

            return c;
        }

        /// <summary>
        /// Adds matrix B to matrix A.
        /// </summary>
        /// <param name="a">The matrix to be added.</param>
        /// <param name="b">The adding matrix.</param>
        /// <returns>The added matrix.</returns>
        public static float[,] Add(float[,] A, float[,] B)
        {
            int n = A.GetLength(0);
            float[,] C = new float[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
                
            }

            return C;
        }

        /// <summary>
        /// Multiplies matrix A with matrix B.
        /// </summary>
        /// <param name="A">The first matrix.</param>
        /// <param name="B">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static float[,] Product(float[,] A, float[,] B)
        {
            int n = A.GetLength(0);
            float[,] C = new float[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        C[i, j] += A[i, k] * B[k, j];
                    }
                }
            }

            return C;
        }

        public static float[] Product(float[,] A, float[] b)
        {
            int n = A.GetLength(0);
            float[] c = new float[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    c[i] += A[i, j] * b[j];
                }
            }

            return c;
        }

        /// <summary>
        /// Calculates the transpose of the given matrix.
        /// </summary>
        /// <param name="A">The matrix to transpose.</param>
        /// <returns>The transpose.</returns>
        public static float[,] Transpose(float[,] A)
        {
            int n = A.GetLength(0);
            float[,] B = new float[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    B[i, j] = A[j, i];
                }
            }

            return B;
        }

        /// <summary>
        /// Calculates the magnitude of the given vector.
        /// </summary>
        /// <param name="a">The vector to compute the magnitude of.</param>
        /// <returns>The magnitude.</returns>
        public static float Magnitude(float[] a)
        {
            float magnitude = 0;

            for (int i = 0; i < a.Length; i++)
            {
                magnitude += a[i] * a[i];
            }
            return (float)System.Math.Sqrt(magnitude);
        }

        /// <summary>
        /// Constructs an n-by-n identity matrix.
        /// </summary>
        /// <param name="n">The size of the matrix.</param>
        /// <returns>The identity matrix.</returns>
        public static float[,] Identity(int n)
        {
            float[,] I = new float[n, n];
            for (int i = 0; i < n; i++)
            {
                I[i, i] = 1;
            }
            return I;
        }

        /// <summary>
        /// Calculates the inverse of the given matrix using the 
        /// Gauss-Jordan Method (see https://en.wikipedia.org/wiki/Gaussian_elimination).
        /// </summary>
        /// <param name="A">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix.</returns>
        public static float[,] Inverse(float[,] A)
        {
            // Initialize the augmented matrix B.
            int n = A.GetLength(0);
            float[,] B = new float[n, 2 * n];

            // In the augmented matrix B, the first 3 columns are the original 
            // matrix A, and the last 3 columns are the identity matrix C.
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    B[i, j] = A[i, j];
                }

                for (int j = n; j < 2 * n; j++)
                {
                    if (i == j - n) B[i, j] = 1;
                }
            }

            // Swap rows of the augmented matrix B.
            for (int i = n - 1; i > 0; i--)
            {
                if (B[i - 1, 0] >= B[i, 0]) continue;

                for (int j = 0; j < 2 * n; j++)
                {
                    float temp = B[i, j];
                    B[i, j] = B[i - 1, j];
                    B[i - 1, j] = temp;
                }
            }

            // Substract each row by a multiple of another row.
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;

                    float temp = B[j, i] / B[i, i];
                    for (int k = 0; k < 2 * n; k++)
                    {
                        B[j, k] -= B[i, k] * temp;
                    }
                }
            }

            // Divide each row element by the diagonal element.
            for (int i = 0; i < n; i++)
            {
                float temp = B[i, i];
                for (int j = 0; j < 2 * n; j++)
                {
                    B[i, j] = B[i, j] / temp;
                }
            }

            // Strip the augmented matrix B of the first three columns
            // to get the inverse matrix C of the original matrix A.
            float[,] C = new float[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = n; j < 2 * n; j++)
                {
                    C[i, j - n] = B[i, j];
                }
            }

            // Return the inverse matrix C.
            return C;
        }

        /// <summary>
        /// Takes the square root of the given diagonal matrix.
        /// </summary>
        /// <param name="A">The diagonal matrix.</param>
        /// <returns>The square root of the given matrix.</returns>
        public static float[,] Sqrt(float[,] A)
        {
            int n = A.GetLength(0);
            float[,] B = Duplicate(A);

            for (int i = 0; i < n; i++)
            {
                if (A[i, i] <= 0) { throw new DivideByZeroException(); };

                B[i, i] = (float)System.Math.Sqrt(A[i, i]);
            }

            return B;
        }

        /// <summary>
        /// Prints the given matrix.
        /// </summary>
        /// <param name="A">The matrix to print.</param>
        /// <returns>The string representation of the given matrix.</returns>
        public static string ToString(float[,] A)
        {
            int rowCount = A.GetLength(0);
            int columnCount = A.GetLength(1);

            string text = "";

            for (int i = 0; i < rowCount; i++)
            {
                if (i > 0) text += ',';

                text += '{';
                for (int j = 0; j < columnCount; j++)
                {
                    if (j > 0) text += ',';
                    text += A[i, j];
                }
                text += '}';
            }

            return text;
        }

        /// <summary>
        /// Prints the given vector.
        /// </summary>
        /// <param name="a">The vector to print.</param>
        /// <returns>The string representation of the given vector.</returns>
        public static string ToString(float[] a)
        {
            string text = "{";
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) text += ',';
                text += a[i];
            }
            text += '}';

            return text;
        }

        public static float SubAbs(float[,] preU, float[,] U)
        {
            float subSum = 0;
            int n = preU.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    subSum += (float)System.Math.Abs(preU[i, j] - U[i, j]);
                }
            }

            return subSum;
        }
    }
}