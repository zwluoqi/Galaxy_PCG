using System;
using UnityEngine;

namespace Planet
{
    public class FaceGenerate
    {
        public float Radius;
        public Mesh Mesh;
        public Vector3 Normal;
        public int Resolution;

        private Vector3 axisA;
        private Vector3 axisB;
        
        public FaceGenerate(Mesh mesh, Vector3 normal, int resolution,float radius)
        {
            Radius = radius;
            Mesh = mesh;
            Normal = normal;
            Resolution = resolution;
            //unity 左手坐标系
            axisA = new Vector3(normal.y,normal.z,normal.x);
            axisB = Vector3.Cross(normal, axisA);
            
        }

        private Vector3[] vertices;
        private Vector3[] normals;
        private int[] triangles;
        public void Update()
        {
            vertices = new Vector3[(Resolution ) * (Resolution )];
            // var m = (Resolution - 2);
            var multiple = (Resolution - 1) * (Resolution - 1);
            triangles = new int[multiple*2*3];
            normals =  new Vector3[(Resolution ) * (Resolution )];
            int indicIndex = 0;
            for (int y = 0; y < Resolution; y++)
            {
                for (int x = 0; x < Resolution; x++)
                {
                    var index = x + y * (Resolution);
                    Vector2 percent = new Vector2(x, y) / (Resolution - 1);
                    var pos = Normal+2*axisB * (percent.x - 0.5f) + 2*axisA * (percent.y - 0.5f);
                    vertices[index] = Radius * pos.normalized;
                    normals[index] = pos.normalized;
                    
                    if (x < Resolution - 1 && y < Resolution - 1)
                    {
                        
                        // triangles[indicIndex++] = index+Resolution;
                        // triangles[indicIndex++] = index + (Resolution)+1;
                        // triangles[indicIndex++] = index+1;
                        //
                        // triangles[indicIndex++] = index+1;
                        // triangles[indicIndex++] = index;
                        // triangles[indicIndex++] = index+Resolution;
                        
                        //顺时针
                        // triangles[indicIndex++] = index;
                        // triangles[indicIndex++] = index + 1;
                        // triangles[indicIndex++] = index+1+Resolution;
                        //
                        // triangles[indicIndex++] = index+1+Resolution;
                        // triangles[indicIndex++] = index+Resolution;
                        // triangles[indicIndex++] = index;

                        //逆时针
                        triangles[indicIndex++] = index;
                        triangles[indicIndex++] = index+Resolution;
                        triangles[indicIndex++] = index+1+Resolution;
                        
                        triangles[indicIndex++] = index+1+Resolution;
                        triangles[indicIndex++] = index+1;
                        triangles[indicIndex++] = index;

                    }
                }
            }

            Mesh.Clear();
            Mesh.vertices = vertices;
            Mesh.triangles = triangles;
            Mesh.RecalculateNormals();
            // Mesh.normals = normals;
            Mesh.UploadMeshData(false);
        }
    }
}