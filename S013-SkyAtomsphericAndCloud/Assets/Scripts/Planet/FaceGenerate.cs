using System;
using Planet.Setting;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Planet
{
    public class MeshData
    {
        public Vector3[] vertices;
        public  Vector2[] uvs;//x,高度（0,1）,y,深度(-1,1)
        public  int[] triangles;
        public  Vector3[] normals;
        public  Vector4[] tangents;
        public  Vector2[] formatuvs;//x latituderange，y,深度(-1,1)

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
        public MeshFilter MeshFilter;

        public int realResolution
        {
            get
            {
                return configResolution;
            }
        }
        private int configResolution = 2;
        private int lodLev = 0;

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
            configResolution = resolution;
            _meshData.UpdateSize(realResolution);
        }
        
        
        public bool UpdateLODLevel(int lodLevel)
        {
            if (this.lodLev != lodLevel)
            {
                this.lodLev = lodLevel;
                _meshData.UpdateSize(realResolution);
                return true;
            }
            return false;
        }

        public void Update(int resolution,VertexGenerate vertexGenerate,PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate)
        {
            if (configResolution != resolution)
            {
                configResolution = resolution;
                _meshData.UpdateSize(realResolution);
            }

            UpdateShape(vertexGenerate,planetSettingData,gpuShapeGenerate);
        }

        public Vector3 GetPlanePos(VertexGenerate vertexGenerate)
        {
            var transform = this.MeshFilter.transform;
            var worldNormal = transform.localToWorldMatrix *
                              new Vector4(_faceData.Normal.x, _faceData.Normal.y, _faceData.Normal.z, 0.0f);
            var planePos = transform.position + new Vector3(worldNormal.x,worldNormal.y,worldNormal.z) * vertexGenerate.shapeSettting.radius;
            return planePos;
        }
        

        public void UpdateShape(VertexGenerate vertexGenerate,PlanetSettingData planetSettingData,GPUShapeGenerate gpuShapeGenerate)
        {
            objectHeight = new MinMax();
            depth = new MinMax();
            var start = System.DateTime.Now;
            if (planetSettingData.gpu && realResolution >=8)
            {
                gpuShapeGenerate.UpdateShape(vertexGenerate,_meshData,
                    realResolution,
                    _faceData,planetSettingData);

                UpdateDepth();
                FillShapeMesh(MeshFilter,_meshData.vertices,planetSettingData);
            }    
            else
            {

                UpdateShape0(vertexGenerate);
                UpdateDepth();
                FillShapeMesh(MeshFilter,_meshData.vertices,planetSettingData);
            }
            var end = System.DateTime.Now;
        }

        private void UpdateDepth()
        {
            for (int i = 0; i < _meshData.uvs.Length; i++)
            {
                objectHeight.AddValue(_meshData.uvs[i].x);
                depth.AddValue(_meshData.uvs[i].y);
                _meshData.formatuvs[i].y = _meshData.uvs[i].y;
            }
        }

        private void UpdateShape0(VertexGenerate vertexGenerate)
        {
            
            int indicIndex = 0;
            for (int y = 0; y < realResolution; y++)
            {
                for (int x = 0; x < realResolution; x++)
                {
                    var index = x + y * (realResolution);
                    Vector2 percent = new Vector2(x, y) / (realResolution - 1);
                    var pos = _faceData.Normal + 2 * _faceData.axisX * (percent.x - 0.5f) + 2 * _faceData.axisY * (percent.y - 0.5f);
                    if (vertexGenerate.shapeSettting.generateType == GenerateType.QuadSphere)
                    {
                        var p2 = new Vector3(pos.x*pos.x,pos.y*pos.y,pos.z*pos.z);
                        var rx = pos.x*Mathf.Sqrt(1.0f - 0.5f * (p2.y + p2.z) + p2.y * p2.z / 3.0f);
                        var ry = pos.y*Mathf.Sqrt(1.0f - 0.5f * (p2.z + p2.x) + p2.z * p2.x / 3.0f);
                        var rz = pos.z*Mathf.Sqrt(1.0f - 0.5f * (p2.x + p2.y) + p2.x * p2.y / 3.0f);
                        pos = (new Vector3(rx,ry,rz));
                    }
                    else
                    {
                        pos = pos.normalized;
                    }
                    
                    _meshData.vertices[index] = vertexGenerate.Execulate(pos,out var outNoise );
                    _meshData.uvs[index] = new Vector2(pos.y,outNoise.y);

                    // vertices[index] = (2 * axisB * (percent.x - 0.5f) + 2 * axisA * (percent.y - 0.5f))*shapeGenerate.shapeSettting.radius;
                    if (x < realResolution - 1 && y < realResolution - 1)
                    {
                        //
                        // //逆时针
                        _meshData.triangles[indicIndex++] = index;
                        _meshData.triangles[indicIndex++] = index + realResolution;
                        _meshData.triangles[indicIndex++] = index + 1 + realResolution;
                            
                        _meshData.triangles[indicIndex++] = index + 1 + realResolution;
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
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = _vertices;
            mesh.triangles = _meshData.triangles;
            mesh.uv = _meshData.formatuvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            // mesh.normals = _meshData.normals;
            // mesh.tangents = _meshData.tangents;
            mesh.RecalculateBounds();
            if (meshFilter.TryGetComponent<MeshCollider>(out var meshCollider))
            {
                meshCollider.sharedMesh = mesh;
            }
        }

        public void FormatHeight(PlanetSettingData planetSettingData,ColorGenerate colorGenerate,GPUShapeGenerate gpuShapeGenerate)
        {
            //gpuShapeGenerate.UpdateShape(uvs,colorGenerate);
            if (planetSettingData.gpu && realResolution >=8)
            {
                gpuShapeGenerate.UpdateColorFormatHeight(realResolution,colorGenerate,_meshData);
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