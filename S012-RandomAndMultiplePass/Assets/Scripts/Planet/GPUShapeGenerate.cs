using System;
using Planet.Setting;
using UnityEngine;

namespace Planet
{

    public class MeshDataComputerBuffer
    {
        public ComputeBuffer _bufferVertices;
        public  ComputeBuffer _bufferNormals;
        public  ComputeBuffer _bufferTangents;
        
        public  ComputeBuffer _bufferUVs;
        public  ComputeBuffer _bufferTriangles;


        public void SetData(MeshData meshData)
        {
            _bufferVertices.SetData(meshData.vertices);
            _bufferTriangles.SetData(meshData.triangles);
        }

        public void GetData(MeshData meshData)
        {
            
            _bufferVertices.GetData(meshData.vertices);
            _bufferNormals.GetData(meshData.normals);
            _bufferTangents.GetData(meshData.tangents);
            
            _bufferUVs.GetData(meshData.uvs);
            _bufferTriangles.GetData(meshData.triangles);
        }

        public void CreateShapeBuffer(MeshData meshData)
        {

            ResizeComputerBuffer(ref _bufferVertices, ref meshData.vertices,3*4);
            ResizeComputerBuffer(ref _bufferNormals, ref meshData.normals,3*4);
            ResizeComputerBuffer(ref _bufferTangents, ref meshData.tangents,4*4);
            ResizeComputerBuffer(ref _bufferTriangles, ref meshData.triangles,4);
            ResizeComputerBuffer(ref _bufferUVs, ref meshData.uvs,2*4);
            
        }

        private void ResizeComputerBuffer<T>(ref ComputeBuffer bufferVertices, ref T[] meshDataVertices,int stride)
        {
                        
            if (bufferVertices != null&&bufferVertices.count != meshDataVertices.Length)
            {
                bufferVertices.Release();
                bufferVertices.Dispose();
                bufferVertices = null;
            }
            if(bufferVertices == null)
            {
                bufferVertices = new ComputeBuffer(meshDataVertices.Length,stride);
            }

        }

        public void Dispose()
        {
            _bufferVertices?.Dispose();
            _bufferNormals?.Dispose();
            _bufferTangents?.Dispose();
            
            _bufferUVs?.Dispose();
            _bufferTriangles?.Dispose();
        }
    }
    public class GPUShapeGenerate:System.IDisposable
    {


        private MeshDataComputerBuffer _meshDataComputerBuffer = new MeshDataComputerBuffer();
        
        private readonly int ResolutionID = Shader.PropertyToID("Resolution");
        private readonly int NormalID = Shader.PropertyToID("Normal");
        private readonly int BiNormalID = Shader.PropertyToID("BiNormal");
        
        private readonly int axisXID = Shader.PropertyToID("axisX");
        private readonly int axisYID = Shader.PropertyToID("axisY");
        private readonly int verticesID = Shader.PropertyToID("vertices");
        private readonly int normalsID = Shader.PropertyToID("normals");
        private readonly int tangentsID = Shader.PropertyToID("tangents");
        private readonly int uvsID = Shader.PropertyToID("uvs");
        private readonly int trianglesID = Shader.PropertyToID("triangles");
        private readonly int shapeSettingId = Shader.PropertyToID("shapeSetting");
        private readonly int noiseLayerSettingsId = Shader.PropertyToID("noiseLayerSettings");
        private readonly int noiseAddLayerCountID = Shader.PropertyToID("noiseAddLayerCount");
        private readonly int oceanID = Shader.PropertyToID("ocean");

        
        
        
        private ComputeBuffer _baseShapeComputeBuffer;
        private ComputeBuffer _noiseLayerComputeBuffer;
        
        GPUColorGenerate gpuColorGenerate = new GPUColorGenerate();

        public GPUShapeGenerate()
        {
            unsafe
            {
                int stride = sizeof(ShapeSettingBuffer);
                _baseShapeComputeBuffer = new ComputeBuffer(1, stride);
            }
        }

        public void UpdateShape(VertexGenerate vertexGenerate,MeshData _meshData,
            int resolution,FaceData _faceData ,PlanetSettingData planetSettingData)
        {
            if (resolution < 8)
            {
                throw new Exception("分辨率低于8不允许使用GPU");
            }
            CreateShapeBuffer(vertexGenerate,_meshData);
            
            _baseShapeComputeBuffer.SetData(vertexGenerate.shapeSettting.ToBaseBuffer(vertexGenerate.randomGenerate.randomData));
            _noiseLayerComputeBuffer.SetData(vertexGenerate.shapeSettting.ToLayerBuffer(vertexGenerate.randomGenerate.randomData));

            var computeShader = vertexGenerate.shapeSettting.computeShader;

            computeShader.SetInt(ResolutionID, resolution);
            computeShader.SetVector(NormalID, _faceData.Normal);
            computeShader.SetVector(BiNormalID, _faceData.BiNormal);
            
            computeShader.SetVector(axisXID, _faceData.axisX);
            computeShader.SetVector(axisYID, _faceData.axisY);
            computeShader.SetInt(noiseAddLayerCountID, _noiseLayerComputeBuffer?.count ?? 0);
            // computeShader.SetFloat(oceanID,planetSettingData.ocean?1.0f:0.0f);
            //获取内核函数的索引
            var kernelVertices = computeShader.FindKernel("CSMainVertices");
            computeShader.SetBuffer(kernelVertices,verticesID,_meshDataComputerBuffer._bufferVertices);
            computeShader.SetBuffer(kernelVertices,normalsID,_meshDataComputerBuffer._bufferNormals);
            computeShader.SetBuffer(kernelVertices,tangentsID,_meshDataComputerBuffer._bufferTangents);
            computeShader.SetBuffer(kernelVertices,uvsID,_meshDataComputerBuffer._bufferUVs);
            computeShader.SetBuffer(kernelVertices,trianglesID,_meshDataComputerBuffer._bufferTriangles);
            
            computeShader.SetBuffer(kernelVertices,shapeSettingId,_baseShapeComputeBuffer);
            computeShader.SetBuffer(kernelVertices, noiseLayerSettingsId, _noiseLayerComputeBuffer);
            computeShader.Dispatch(kernelVertices, resolution, resolution, 1);
            
            _meshDataComputerBuffer.GetData(_meshData);
            
        }

        private void CreateShapeBuffer(VertexGenerate vertexGenerate,MeshData meshData)
        {
            _meshDataComputerBuffer.CreateShapeBuffer(meshData);
            

            if (_noiseLayerComputeBuffer != null &&
                _noiseLayerComputeBuffer.count != vertexGenerate.shapeSettting._noiseLayers.Length)
            {
                _noiseLayerComputeBuffer.Release();
                _noiseLayerComputeBuffer.Dispose();
                _noiseLayerComputeBuffer = null;
            }
            if (_noiseLayerComputeBuffer == null )
            {
                unsafe
                {
                    int stride = sizeof(NoiseLayerBuffer);
                    _noiseLayerComputeBuffer = new ComputeBuffer(Mathf.Max(1,vertexGenerate.shapeSettting._noiseLayers.Length), stride);
                }
            }
        }


        


        public void UpdateColorFormatHeight(int resolution, ColorGenerate colorGenerate, MeshData meshData)
        {
            if (resolution < 8)
            {
                throw new Exception("分辨率低于8不允许使用GPU");
            }
            _meshDataComputerBuffer._bufferVertices.SetData(meshData.vertices);
            _meshDataComputerBuffer._bufferUVs.SetData(meshData.uvs);
            gpuColorGenerate.UpdateColorFormatHeight(resolution,colorGenerate,_meshDataComputerBuffer._bufferVertices,_meshDataComputerBuffer._bufferUVs,meshData.formatuvs);
        }

        public void Dispose()
        {
            _meshDataComputerBuffer.Dispose();
            _baseShapeComputeBuffer?.Dispose();
            _noiseLayerComputeBuffer?.Dispose();
            gpuColorGenerate?.Dispose();
        }
    }
}