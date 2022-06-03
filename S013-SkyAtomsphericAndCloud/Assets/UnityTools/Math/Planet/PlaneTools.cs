using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Math.Planet
{
    public class PlaneTools
    {
        /// <summary>
        /// 判断点集在一个平面内
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static bool CheckPointIn2D(List<Vector3> points,out Vector3 maxVec,out Vector3 normal)
        {
            var x = points[1] - points[0];
            Vector3 y = Vector3.zero;
            int ignoreIndex = 2;
            normal = Vector3.one;
            for (int i = 2; i < points.Count; i++)
            {
                y = points[i] - points[0];   
                normal = Vector3.Cross(x, y).normalized;
                if (normal != Vector3.zero)
                {
                    ignoreIndex = i;
                    break;
                }
            }
            maxVec = x.magnitude > y.magnitude ? x : y;

            for (int i = 2; i < points.Count; i++)
            {
                if (i == ignoreIndex)
                {
                    continue;
                }
                var p = points[i] - points[0];
                var d = Vector3.Dot(p.normalized, normal);
                if (Mathf.Abs(d) > 1e-5)
                {
                    return false;
                }

                if (p.magnitude > maxVec.magnitude)
                {
                    maxVec = p;
                }
            }

            return true;
        }
    }
}