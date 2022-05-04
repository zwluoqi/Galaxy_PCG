using UnityEngine;

namespace UnityTools.Math.Random
{
    public class RandomMathUtil
    {
        /// <summary>
        /// 高斯采样[-1,1]
        /// </summary>
        public static float GaussianRandom
        {
            get
            {
                float x1;
                float w;
                do
                {
                    x1 = 2f * UnityEngine.Random.value - 1f;
                    float x2 = 2f * UnityEngine.Random.value - 1f;
                    w = x1 * x1 + x2 * x2;
                }
                while ((double)w >= 1.0 || w == 0.0);
                float num4 = Mathf.Sqrt(-2f * Mathf.Log(w) / w);
                return x1 * num4;
            }
        }
        
        /// <summary>
        /// 半球面均匀采样
        /// </summary>
        /// <param name="u">[0-1]</param>
        /// <param name="v">[0-1]</param>
        /// <returns></returns>
        Vector3 hemisphereSample_uniform(float u, float v) {
            float phi = v * 2.0f * Mathf.PI;
            float cosTheta = 1.0f - u;
            float sinTheta = Mathf.Sqrt(1.0f - cosTheta * cosTheta);
            return new Vector3(Mathf.Cos(phi) * sinTheta, Mathf.Sin(phi) * sinTheta, cosTheta);
        }

        /// <summary>
        /// 球面均匀采样
        /// </summary>
        /// <param name="u">[0-1]</param>
        /// <param name="v">[0-1]</param>
        /// <returns></returns>
        Vector3 uniformCircle(float u,float v)
        {
            var theta = Mathf.Acos( 1-2*u);
            var phi = v * 2 * Mathf.PI;
            var y  =  Mathf.Cos(theta);
            var xz = Mathf.Sin(theta);
            var x = xz * Mathf.Cos(phi);
            var z = xz * Mathf.Sin(phi);
            return new Vector3(x,y,z);
        }
        
        
    }
}