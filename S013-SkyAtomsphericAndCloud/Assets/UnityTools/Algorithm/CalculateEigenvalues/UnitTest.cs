using UnityEngine;
using UnityTools.SimpleQRAlgorithm;

namespace UnityTools.Algorithm.CalculateEigenvalues
{
    public class UnitTest
    {
        public static void Test()
        {
            float[,] A = new float[,]
            {
                {-2.0f, -4.0f, 2.0f},
                {-2.0f, 1.0f, 2.0f},
                {4.0f, 2.0f, 5.0f}
            };
            // Matrix3x3CalculateEigenvalues.Diagonalize3X3(A,out var eigenvalues,out var eigenvectors);
            QRAlgorithm.Diagonalize(A,out var eigenvalues,out var eigenvectors);

            Debug.LogWarning($"eigenvalues:{eigenvalues[0]},{eigenvalues[1]},{eigenvalues[2]}");
            for (int i = 0; i < 3; i++)
            {
                Debug.LogWarning($"eigenvectors:\n" +
                                 $"{eigenvectors[0,0]},{eigenvectors[0,1]},{eigenvectors[0,2]}\n" +
                                 $"{eigenvectors[1,0]},{eigenvectors[1,1]},{eigenvectors[1,2]}\n" +
                                 $"{eigenvectors[2,0]},{eigenvectors[2,1]},{eigenvectors[2,2]}");
            }
        }
    }
}