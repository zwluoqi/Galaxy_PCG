namespace UnityTools.SimpleQRAlgorithm
{
public static class QRAlgorithm
    {
        /// <summary>
        /// Runs the QR algorithm to find the eigenvalues and eigenvectors of the given matrix (see https://en.wikipedia.org/wiki/QR_algorithm).
        /// </summary>
        /// <param name="A">The matrix for which eigenvalues and eigenvectors should be found.</param>
        /// <param name="iterations">The number of iterations.</param>
        /// <param name="eigenvalues">The eigenvalues stored as diagonal entries in a matrix.</param>
        /// <param name="eigenvectors">The eigenvectors stored as columns in a matrix.</param>
        public static void Diagonalize(float[,] A, out float[] eigenvalues, out float[,] eigenvectors ,int iterations = 100)
        {
            int n = A.GetLength(0);

            // Duplicate the original matrix A so it stays intact.
            float[,] B = LinearAlgebra.Duplicate(A);

            // Initialize the eigenvector matrix C.
            float[,] U = LinearAlgebra.Identity(n);

            // Perform the QR decomposition and update the B and C matrixes each iteration.
            for (int i = 0; i < iterations; i++)
            {
                QRDecomposition(B, out float[,] Q, out float[,] R);
                B = LinearAlgebra.Product(R, Q);
                var preU = U;
                U = LinearAlgebra.Product(U, Q);
                var s = LinearAlgebra.SubAbs(preU, U);
                if (s < float.Epsilon)
                {
                    break;
                }
            }

            // The eigenvalues are on the diagonal of the B matrix.
            eigenvalues = new float[n];
            for (int i = 0; i < n; i++)
            {
                eigenvalues[i] = B[i, i];
            }

            // The eigenvectors are the columns of the C matrix.
            eigenvectors = U;
        }

        /// <summary>
        /// Calculates the QR decomposition of the given matrix A (see https://en.wikipedia.org/wiki/QR_decomposition). 
        /// </summary>
        /// <param name="A">The matrix to decompose.</param>
        /// <param name="Q">The Q part of the decomposition.</param>
        /// <param name="R">The R part of the decomposition.</param>
        private static void QRDecomposition(float[,] A, out float[,] Q, out float[,] R)
        {
            int n = A.GetLength(0);

            // Duplicate the original matrix A so it stays intact.
            float[,] U = LinearAlgebra.Duplicate(A);

            // Calculate the U matrix using the Gram–Schmidt process (see https://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process).
            for (int j = 1; j < n; j++)
            {
                float[] u = LinearAlgebra.GetColumn(U, j);
                float[] v = LinearAlgebra.GetColumn(U, j);

                for (int k = j - 1; k >= 0; k--)
                {
                    float[] uk = LinearAlgebra.GetColumn(U, k);
                    u = LinearAlgebra.Subtract(u, LinearAlgebra.Project(uk, v));
                }

                // Update the column entries in U.
                for (int i = 0; i < n; i++)
                {
                    U[i, j] = u[i];
                }
            }

            // Normalize the column vectors of U.
            for (int j = 0; j < n; j++)
            {
                float[] u = LinearAlgebra.GetColumn(U, j);
                float magnitude = LinearAlgebra.Magnitude(u);

                // Update the column entries in U.
                for (int i = 0; i < n; i++)
                {
                    U[i, j] = u[i] / magnitude;
                }
            }

            // The U matrix is now the Q part of the decomposition.
            Q = U;

            // Calculate the R part of the decomposition.
            R = LinearAlgebra.Product(LinearAlgebra.Transpose(Q), A);
        }
    }
}