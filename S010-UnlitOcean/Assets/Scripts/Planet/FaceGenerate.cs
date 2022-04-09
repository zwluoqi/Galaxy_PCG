using System;
using Planet.Setting;
using Unity.Mathematics;
using UnityEngine;

namespace Planet
{
    public class FaceGenerate
    {
        public MeshFilter MeshFilter;
        public Vector3 Normal;
        public int Resolution;

        private Vector3 axisA;
        private Vector3 axisB;

        private Vector3[] vertices;
        private Vector2[] uvs;
        private Vector2[] formatuvs;
        private int[] triangles;
        public MinMax objectHeight = new MinMax();
        public MinMax depth = new MinMax();
        public void Init(
            MeshFilter meshFilter ,Vector3 normal)
        {
            MeshFilter = meshFilter;
            Normal = normal.normalized;
            //unity 左手坐标系
            axisA = new Vector3(normal.y,normal.z,normal.x);
            axisA = axisA.normalized;
            axisB = Vector3.Cross(normal, axisA);
            axisB = axisB.normalized;
        }

        public void Update(int resolution,VertexGenerate vertexGenerate,PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate)
        {
            // var _computeShader = shapeGenerate.ShapeSettting.computeShader;

            if (Resolution != resolution)
            {
                Resolution = resolution;
                vertices = new Vector3[(Resolution ) * (Resolution )];
                uvs = new Vector2[(Resolution ) * (Resolution )];
                formatuvs = new Vector2[(Resolution ) * (Resolution )];
                var multiple = (Resolution - 1) * (Resolution - 1);
                triangles = new int[multiple*2*3];
            }

            UpdateShape(vertexGenerate,planetSettingData,gpuShapeGenerate);
        }

        public void UpdateShape(VertexGenerate vertexGenerate,PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate)
        {
            objectHeight = new MinMax();
            depth = new MinMax();
            var start = System.DateTime.Now;
            if (planetSettingData.gpu && Resolution >=8)
            {
                gpuShapeGenerate.UpdateShape(vertexGenerate,vertices,triangles,uvs,
                    Resolution,
                    Normal,axisA,axisB,planetSettingData);

                for (int i = 0; i < uvs.Length; i++)
                {
                    objectHeight.AddValue(uvs[i].x);
                    depth.AddValue(uvs[i].y);
                    formatuvs[i].y = uvs[i].y;
                }
                FillShapeMesh(MeshFilter,vertices);
            }    
            else
            {

                UpdateShape0(vertexGenerate);
                
                FillShapeMesh(MeshFilter,vertices);
            }
            var end = System.DateTime.Now;

            
        }

        private void UpdateShape0(VertexGenerate vertexGenerate)
        {
            
            int indicIndex = 0;
            for (int y = 0; y < Resolution; y++)
            {
                for (int x = 0; x < Resolution; x++)
                {
                    var index = x + y * (Resolution);
                    Vector2 percent = new Vector2(x, y) / (Resolution - 1);
                    var pos = Normal + 2 * axisB * (percent.x - 0.5f) + 2 * axisA * (percent.y - 0.5f);
                    vertices[index] = vertexGenerate.Execulate(pos.normalized);
                    // vertices[index] = (2 * axisB * (percent.x - 0.5f) + 2 * axisA * (percent.y - 0.5f))*shapeGenerate.shapeSettting.radius;
                    if (x < Resolution - 1 && y < Resolution - 1)
                    {
                            
                        //逆时针
                        // triangles[indicIndex++] = index;
                        // triangles[indicIndex++] = index + 1 + Resolution;
                        // triangles[indicIndex++] = index + 1;
                        //
                        // triangles[indicIndex++] = index + 1 + Resolution;
                        // triangles[indicIndex++] = index ;
                        // triangles[indicIndex++] = index + Resolution;
                            
                        //
                        // //逆时针
                        triangles[indicIndex++] = index;
                        triangles[indicIndex++] = index + Resolution;
                        triangles[indicIndex++] = index + 1 + Resolution;
                            
                        triangles[indicIndex++] = index + 1 + Resolution;
                        triangles[indicIndex++] = index + 1;
                        triangles[indicIndex++] = index;
                    }
                }
            }

        }

        private void FillShapeMesh(MeshFilter meshFilter,Vector3[] _vertices)
        {
            var mesh = meshFilter.sharedMesh;
            mesh.Clear();
            mesh.vertices = _vertices;
            mesh.triangles = triangles;
            mesh.uv = formatuvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        public void FormatHeight(PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate,ColorGenerate colorGenerate)
        {
            //gpuShapeGenerate.UpdateShape(uvs,colorGenerate);
            if (planetSettingData.gpu && Resolution >=8)
            {
                gpuShapeGenerate.UpdateColorFormatHeight(Resolution,colorGenerate.ColorSettting,formatuvs,vertices,uvs);
            }
            else
            {
                for (int i = 0; i < uvs.Length; i++)
                {
                    formatuvs[i].x = colorGenerate.UpdateColorFormatHeight(vertices[i], uvs[i].x);
                }
            }

            
            ResetUV(MeshFilter);
        }

        private void ResetUV(MeshFilter meshFilter)
        {
            var mesh = meshFilter.sharedMesh;
            mesh.uv = formatuvs;
        }

        public void UpdateMaterial(Material sharedMaterial)
        {
            MeshFilter.GetComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
        }
    }
}