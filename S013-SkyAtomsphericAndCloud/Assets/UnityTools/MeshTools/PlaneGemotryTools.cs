using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTools.MeshTools
{
    /// <summary>
    /// 平面几何顶点获取三角形
    /// </summary>
    public class PlaneGemotryTools
    {
        public static int[] GetTriangles8(List<Vector3> vector3s,out Vector2[] uvs)
        {
            int[] triangles = new int[6*3];
            uvs = new Vector2[vector3s.Count];
            int i = 0;
            triangles[i++] = 0;
            triangles[i++] = 1;
            triangles[i++] = 7;
                        
            triangles[i++] = 1;
            triangles[i++] = 2;
            triangles[i++] = 3;
                        
            triangles[i++] = 3;
            triangles[i++] = 4;
            triangles[i++] = 5;
                        
            triangles[i++] = 5;
            triangles[i++] = 6;
            triangles[i++] = 7;
                    
            triangles[i++] = 7;
            triangles[i++] = 1;
            triangles[i++] = 5;
                    
            triangles[i++] = 1;
            triangles[i++] = 3;
            triangles[i++] = 5;
                        
            uvs[0] = new Vector2(0,0.5f);
            uvs[1] = new Vector2(1.0f/4.0f,1.0f/4.0f);
            uvs[2] = new Vector2(2.0f/4.0f,0.0f);
            uvs[3] = new Vector2(3.0f/4.0f,1.0f/4.0f);
            uvs[4] = new Vector2(4.0f/4.0f,0.5f);
                    
            uvs[5] = new Vector2(3.0f/4.0f,3.0f/4.0f);
            uvs[6] = new Vector2(2.0f/4.0f,1);
            uvs[7] = new Vector2(1.0f/4.0f,3.0f/4.0f);
                        
            return triangles;
        }
        
        public static int[] GetTriangles7(List<Vector3> vector3s,out Vector2[] uvs)
        {
            int[] triangles = new int[5*3];
            uvs = new Vector2[vector3s.Count];
            int i = 0;
            triangles[i++] = 0;
            triangles[i++] = 1;
            triangles[i++] = 6;
                        
            triangles[i++] = 1;
            triangles[i++] = 2;
            triangles[i++] = 6;
                        
            triangles[i++] = 2;
            triangles[i++] = 3;
            triangles[i++] = 4;
                        
            triangles[i++] = 4;
            triangles[i++] = 5;
            triangles[i++] = 2;
                    
            triangles[i++] = 5;
            triangles[i++] = 6;
            triangles[i++] = 2;
                        
            uvs[0] = new Vector2(0,0.5f);
            uvs[1] = new Vector2(1.0f/3.0f,2/6.0f);
            uvs[2] = new Vector2(2.0f/3.0f,3/6.0f);
            uvs[3] = new Vector2(1,0.0f);
            uvs[4] = new Vector2(1,1.0f);
            uvs[5] = new Vector2(2.0f/3.0f,5/6.0f);
            uvs[6] = new Vector2(1.0f/3.0f,4/6.0f);
                        
            return triangles;
        }
        
        public static int[] GetTriangles6(List<Vector3> vector3s,out Vector2[] uvs)
        {
            int[] triangles = new int[4*3];
            uvs = new Vector2[vector3s.Count];
            int i = 0;
            triangles[i++] = 0;
            triangles[i++] = 1;
            triangles[i++] = 5;
                        
            triangles[i++] = 5;
            triangles[i++] = 1;
            triangles[i++] = 2;
                        
            triangles[i++] = 2;
            triangles[i++] = 4;
            triangles[i++] = 5;
                        
            triangles[i++] = 4;
            triangles[i++] = 2;
            triangles[i++] = 3;
                        
            uvs[0] = new Vector2(0,0.5f);
            uvs[1] = new Vector2(1.0f/3.0f,0.0f);
            uvs[2] = new Vector2(2.0f/3.0f,0.0f);
            uvs[3] = new Vector2(1,0.5f);
            uvs[4] = new Vector2(2.0f/3.0f,1.0f);
            uvs[5] = new Vector2(1.0f/3.0f,1.0f);
                        
            return triangles;
        }
        
        public static int[] GetTriangles4(List<Vector3> vector3s,out Vector2[] uvs)
        {
            int[] triangles = new int[2*3];
            uvs = new Vector2[vector3s.Count];
            int i = 0;
            triangles[i++] = 0;
            triangles[i++] = 1;
            triangles[i++] = 2;
                        
            triangles[i++] = 2;
            triangles[i++] = 3;
            triangles[i++] = 0;
                    
                        
            uvs[0] = new Vector2(0,0.5f);
            uvs[1] = new Vector2(0.5f,0.0f);
            uvs[2] = new Vector2(1,0.5f);
            uvs[3] = new Vector2(.5f,.5f);
                        
            return triangles;
        }
        
        public static int[] GetTriangles3(List<Vector3> vector3s,out Vector2[] uvs)
        {
            uvs = new Vector2[vector3s.Count];
            uvs[0] = new Vector2(0,0);
            uvs[1] = new Vector2(1,0);
            uvs[2] = new Vector2(1,1);
            return vector3s.Select(((vector3, i) => i)).ToArray();
        }
    }
}