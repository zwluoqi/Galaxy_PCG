using System.Collections.Generic;
using System.Text;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using UnityEngine;
using UnityTools.Math.Planet;
using UnityTools.Physics.Obb;

namespace UnityTools.MeshTools
{
    public class Delaunay3DTools
    {
        public class Axis
        {
            public Vector3 pos;
            public Vector3 x;
            public Vector3 y;
        }
        
        public class SortNode
        {
            public int index;
            public float val;

            public SortNode(int i, float f)
            {
                this.index = i;
                this.val = f;

            }
        }
        
        public class MinMax
        {
            public float min = float.MaxValue;
            public float max = float.MinValue;

            public void AddValue(float v)
            {
                if (v < min)
                {
                    min = v;
                }

                if (v > max)
                {
                    max = v;
                }
            }
        }

        public static (int[],Vector2[]) Delaunay3DPoint(List<Vector3> points)
        {
            List<Vector3> temps = new List<Vector3>(points.Count);
            temps.AddRange(points);
            NormalizePoints(temps);
            Axis axis = Get2DAxis(temps);
            List<Vector2> vector2S = ProjectToPlane(temps, axis);
            Delaunator detonator = new Delaunator(vector2S.ToPoints());
            var uvs = GetUVsXYaxis(vector2S);
            var triangles = detonator.Triangles;
            // DebugTriangles(triangles);
            return (triangles,uvs);
        }

        /// <summary>
        /// 点转UV
        /// </summary>
        /// <param name="vector3S"></param>
        /// <returns></returns>
        private static Vector2[] GetUVsXYaxis(List<Vector2> vector3S)
        {
            MinMax x = new MinMax();
            MinMax y = new MinMax();
            for (int i = 0; i < vector3S.Count; i++)
            {
                x.AddValue(vector3S[i].x);
                y.AddValue(vector3S[i].y);
            }
            Vector2[] uvs  = new Vector2[vector3S.Count];
            for (int i = 0; i < vector3S.Count; i++)
            {
                uvs[i] = new Vector2(
                    Mathf.InverseLerp(x.min, x.max, vector3S[i].x),Mathf.InverseLerp(y.min, y.max, vector3S[i].y));
            }

            return uvs;
        }

        private static  void DebugTriangles(int[] triangles)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < triangles.Length-2; i+=3)
            {
                stringBuilder.AppendLine(triangles[i] + "," + triangles[i+1] + "," + triangles[i+1] + ",");
            }
            Debug.Log(stringBuilder.ToString());
        }

        /// <summary>
        /// 3D坐标投影到2D坐标
        /// </summary>
        /// <param name="tmps"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static  List<Vector2> ProjectToPlane(List<Vector3> tmps, Axis axis)
        {
            List<Vector2> vector2s = new List<Vector2>(tmps.Count);

            for (int i = 0; i < tmps.Count; i++)
            {
                var x = Vector3.Dot(tmps[i], axis.x) / axis.x.magnitude;
                var y = Vector3.Dot(tmps[i], axis.y) / axis.y.magnitude;
                vector2s.Add(new Vector2(x,y));
            }

            return vector2s;
        }

        /// <summary>
        /// 获取投影平面
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static  Axis Get2DAxis(List<Vector3> points)
        {

            Axis axis = new Axis();
            if (PlaneTools.CheckPointIn2D(points,out var maxVec,out var normal))
            {
                var right = maxVec;
                var f = normal;
                axis.x = right;
                axis.y = Vector3.Cross(right, f);
            }
            else
            {

                //找OBB
                OBB obb = new OBB();
                obb.build_from_points(points);
                //obb查找失败 随便找一个平面
                if (obb.m_ext == Vector3.zero)
                {
                    var right = points[1] - points[0];
                    var up = points[2] - points[0];
                    var f = Vector3.Cross(right, up);
                    axis.x = right;
                    axis.y = Vector3.Cross(right, f);
                }
                else
                {
                    var f = Vector3.Cross(obb.right, obb.up);
                    var s = Vector3.Dot(f, obb.forward);

                    if (System.Math.Abs(s - 1) < float.Epsilon)
                    {
                        //点不在一个平面

                        //排序,找最大两边做平面
                        List<SortNode> sortExts = new List<SortNode>(3);
                        sortExts.Add(new SortNode(0, obb.m_ext[0]));
                        sortExts.Add(new SortNode(1, obb.m_ext[1]));
                        sortExts.Add(new SortNode(2, obb.m_ext[2]));
                        sortExts.Sort((indexA, indexB) => -indexA.val.CompareTo(indexB.val));


                        axis.x = obb.GetDir(sortExts[0].index);
                        axis.y = obb.GetDir(sortExts[0].index);
                    }
                    else
                    {
                        //点在一个平面
                        axis.x = obb.right;
                        axis.y = Vector3.Cross(obb.right, f);
                    }
                }
            }

            axis.pos = points[0];
            return axis;
        }

        /// <summary>
        /// 第一个点作为原点
        /// </summary>
        /// <param name="points"></param>
        private static  void NormalizePoints(List<Vector3> points)
        {
            for (int i = 1; i < points.Count; i++)
            {
                points[i] -= points[0];
            }

            points[0] = Vector3.zero;
        }
    }
}