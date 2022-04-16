using System;
using Planet.Setting;
using Unity.Mathematics;
using UnityEngine;

namespace Planet
{
    public class MeshData
    {
        public Vector3[] vertices;
        public  Vector2[] uvs;
        public  int[] triangles;
        public  Vector3[] normals;
        public  Vector4[] tangents;
        public  Vector2[] formatuvs;

        public void UpdateSize(int Resolution)
        {
            vertices = new Vector3[(Resolution ) * (Resolution )];
            normals = new Vector3[(Resolution ) * (Resolution )];
            tangents = new Vector4[(Resolution ) * (Resolution )];
                
            uvs = new Vector2[(Resolution ) * (Resolution )];
            formatuvs = new Vector2[(Resolution ) * (Resolution )];
            var multiple = (Resolution - 1) * (Resolution - 1);
            triangles = new int[multiple*2*3];
        }

        
    }

    public class FaceData
    {
        public Vector3 Normal;
        public Vector3 BiNormal;

        public Vector3 axisY;
        public Vector3 axisX;
    }
    
    public class FaceGenerate
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        public MeshFilter MeshFilter;
        public int Resolution;

        private FaceData _faceData = new FaceData();

        private MeshData _meshData = new MeshData();
        
        public MinMax objectHeight = new MinMax();
        public MinMax depth = new MinMax();


        public FaceGenerate(Vector3 normal)
        {
            _faceData.Normal = normal.normalized;

            // normals[index] = normal;
            if (Math.Abs(Math.Abs(normal.y) - 1.0) < Mathf.Epsilon)
            {
                if (normal.y > 0)
                {
                    _faceData.BiNormal = Vector3.forward;
                }
                else
                {
                    _faceData.BiNormal = Vector3.back;
                }
            }
            else
            {
                _faceData.BiNormal = Vector3.up;
                
            }
            Vector3 tangent = Vector3.Cross(normal, _faceData.BiNormal);
            Vector3 biTangent = -1*Vector3.Cross(normal,tangent);
            //unity 左手坐标系
            _faceData.axisY = biTangent;
            _faceData.axisX = tangent;
        }
        
        public void UpdateMeshFilter(
            MeshFilter meshFilter ,int resolution)
        {
            MeshFilter = meshFilter;
            _meshData.UpdateSize(resolution);
        }

        public void Update(int resolution,VertexGenerate vertexGenerate,PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate)
        {
            // var _computeShader = shapeGenerate.ShapeSettting.computeShader;

            if (Resolution != resolution)
            {
                Resolution = resolution;
                _meshData.UpdateSize(resolution);
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
                gpuShapeGenerate.UpdateShape(vertexGenerate,_meshData,
                    Resolution,
                    _faceData,planetSettingData);

                for (int i = 0; i < _meshData.uvs.Length; i++)
                {
                    objectHeight.AddValue(_meshData.uvs[i].x);
                    depth.AddValue(_meshData.uvs[i].y);
                    _meshData.formatuvs[i].y = _meshData.uvs[i].y;
                }
                FillShapeMesh(MeshFilter,_meshData.vertices,planetSettingData);
            }    
            else
            {

                UpdateShape0(vertexGenerate);
                
                FillShapeMesh(MeshFilter,_meshData.vertices,planetSettingData);
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
                    var pos = _faceData.Normal + 2 * _faceData.axisX * (percent.x - 0.5f) + 2 * _faceData.axisY * (percent.y - 0.5f);
                    _meshData.vertices[index] = vertexGenerate.Execulate(pos.normalized);
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
                        _meshData.triangles[indicIndex++] = index;
                        _meshData.triangles[indicIndex++] = index + Resolution;
                        _meshData.triangles[indicIndex++] = index + 1 + Resolution;
                            
                        _meshData.triangles[indicIndex++] = index + 1 + Resolution;
                        _meshData.triangles[indicIndex++] = index + 1;
                        _meshData.triangles[indicIndex++] = index;
                    }
                }
            }

        }

        private void FillShapeMesh(MeshFilter meshFilter,Vector3[] _vertices,PlanetSettingData planetSettingData)
        {
            var mesh = meshFilter.sharedMesh;
            mesh.Clear();
            mesh.vertices = _vertices;
            mesh.triangles = _meshData.triangles;
            mesh.uv = _meshData.formatuvs;
            if (planetSettingData.ocean)
            {
                mesh.normals = _meshData.normals;
                mesh.tangents = _meshData.tangents;
            }
            else
            {
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }
            mesh.RecalculateBounds();
        }

        public void FormatHeight(PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate,ColorGenerate colorGenerate)
        {
            //gpuShapeGenerate.UpdateShape(uvs,colorGenerate);
            if (planetSettingData.gpu && Resolution >=8)
            {
                gpuShapeGenerate.UpdateColorFormatHeight(Resolution,colorGenerate.ColorSettting,_meshData);
            }
            else
            {
                for (int i = 0; i < _meshData.uvs.Length; i++)
                {
                    _meshData.formatuvs[i].x = colorGenerate.UpdateColorFormatHeight(_meshData.vertices[i], _meshData.uvs[i].x);
                }
            }

            
            ResetUV(MeshFilter);
        }

        private void ResetUV(MeshFilter meshFilter)
        {
            var mesh = meshFilter.sharedMesh;
            mesh.uv = _meshData.formatuvs;
        }

        public void UpdateMaterial(Material sharedMaterial)
        {
            var meshRender = MeshFilter.GetComponent<MeshRenderer>();
            meshRender.sharedMaterial = sharedMaterial;
        }

        public void OnDrawGizmos(float radius)
        {
            if (MeshFilter.gameObject.activeInHierarchy)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _meshData.vertices.Length; i++)
                {
                    Gizmos.DrawLine(MeshFilter.transform.position+_meshData.vertices[i],MeshFilter.transform.position+_meshData.vertices[i]+10.0f*radius/256.0f*_meshData.normals[i]);    
                }
                Gizmos.color = Color.blue;
                for (int i = 0; i < _meshData.vertices.Length; i++)
                {
                    Vector3 tangent = new Vector3(_meshData.tangents[i].x,_meshData.tangents[i].y,_meshData.tangents[i].z);
                    Gizmos.DrawLine(MeshFilter.transform.position+_meshData.vertices[i],MeshFilter.transform.position+_meshData.vertices[i]+4*radius/256.0f*(tangent));    
                }
            }
        }
    }
}