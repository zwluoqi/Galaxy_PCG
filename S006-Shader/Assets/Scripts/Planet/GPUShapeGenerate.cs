using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class GPUShapeGenerate:System.IDisposable
    {
        
        
        private ComputeBuffer _bufferVertices;
        private ComputeBuffer _bufferUVs;
        private ComputeBuffer _bufferTriangles;
        
        private readonly int ResolutionID = Shader.PropertyToID("Resolution");
        private readonly int NormalID = Shader.PropertyToID("Normal");
        private readonly int axisAID = Shader.PropertyToID("axisA");
        private readonly int axisBID = Shader.PropertyToID("axisB");
        private readonly int verticesID = Shader.PropertyToID("vertices");
        private readonly int uvsID = Shader.PropertyToID("uvs");
        private readonly int trianglesID = Shader.PropertyToID("triangles");
        private readonly int shapeSettingId = Shader.PropertyToID("shapeSetting");
        private readonly int noiseLayerSettingsId = Shader.PropertyToID("noiseLayerSettings");
        private readonly int noiseAddLayerCountID = Shader.PropertyToID("noiseAddLayerCount");
        
        private readonly int colorSettingId = Shader.PropertyToID("colorSetting");


        private ComputeBuffer _baseComputeBuffer;
        private ComputeBuffer _noiseLayerComputeBuffer;


        public GPUShapeGenerate()
        {
            unsafe
            {
                int stride = sizeof(ShapeSettingBuffer);
                _baseComputeBuffer = new ComputeBuffer(1, stride);
            }
        }

        public void UpdateShape(VertexGenerate vertexGenerate,Vector3[] vertices,int[] triangles,Vector2[] uvs,
            int resolution,Vector3 Normal,Vector3 axisA,Vector3 axisB)
        {
            CreateBuffer(vertexGenerate,vertices,triangles,uvs);
            
            _bufferVertices.SetData(vertices);
            _bufferTriangles.SetData(triangles);
            
            _baseComputeBuffer.SetData(vertexGenerate.shapeSettting.ToBaseBuffer());
            _noiseLayerComputeBuffer.SetData(vertexGenerate.shapeSettting.ToLayerBuffer());

            
            var _computeShader = vertexGenerate.shapeSettting.computeShader;

            _computeShader.SetInt(ResolutionID, resolution);
            _computeShader.SetVector(NormalID, Normal);
            _computeShader.SetVector(axisAID, axisA);
            _computeShader.SetVector(axisBID, axisB);
            _computeShader.SetInt(noiseAddLayerCountID, _noiseLayerComputeBuffer.count);
            //获取内核函数的索引
            var kernelVertices = _computeShader.FindKernel("CSMainVertices");
            _computeShader.SetBuffer(kernelVertices,verticesID,_bufferVertices);
            _computeShader.SetBuffer(kernelVertices,uvsID,_bufferUVs);
            _computeShader.SetBuffer(kernelVertices,trianglesID,_bufferTriangles);
            _computeShader.SetBuffer(kernelVertices,shapeSettingId,_baseComputeBuffer);
            _computeShader.SetBuffer(kernelVertices,noiseLayerSettingsId,_noiseLayerComputeBuffer);
            _computeShader.Dispatch(kernelVertices, resolution, resolution, 1);
            _bufferVertices.GetData(vertices);
            _bufferUVs.GetData(uvs);
                
                
            // var kernelTriangle = _computeShader.FindKernel("CSMainTriangle");
            // _computeShader.SetBuffer(kernelTriangle,trianglesID,_bufferTriangles);
            // _computeShader.Dispatch(kernelTriangle, 1, ((resolution - 1) * (resolution - 1)-1)/8+1 , 1);
            _bufferTriangles.GetData(triangles);
        }

        private void CreateBuffer(VertexGenerate vertexGenerate,Vector3[] vertices,int[] triangles,Vector2[] uvs)
        {
            if (_bufferVertices != null&&_bufferVertices.count != vertices.Length)
            {
                _bufferVertices.Release();
                _bufferVertices.Dispose();
                _bufferVertices = null;
            }
            if(_bufferVertices == null)
            {
                _bufferVertices = new ComputeBuffer(vertices.Length,3*4);
            }

            if (_bufferTriangles != null && _bufferTriangles.count != triangles.Length)
            {
                _bufferTriangles.Release();
                _bufferTriangles.Dispose();
                _bufferTriangles = null;
            }
            if (_bufferTriangles == null)
            {
                _bufferTriangles = new ComputeBuffer(triangles.Length, 4);
            }
            
            if (_bufferUVs != null && _bufferUVs.count != uvs.Length)
            {
                _bufferUVs.Release();
                _bufferUVs.Dispose();
                _bufferUVs = null;
            }
            if (_bufferUVs == null)
            {
                _bufferUVs = new ComputeBuffer(uvs.Length, 2*4);
            }

            if (_noiseLayerComputeBuffer != null &&
                _noiseLayerComputeBuffer.count != vertexGenerate.shapeSettting._noiseLayers.Length)
            {
                _noiseLayerComputeBuffer.Release();
                _noiseLayerComputeBuffer.Dispose();
                _noiseLayerComputeBuffer = null;
            }
            if (_noiseLayerComputeBuffer == null)
            {
                unsafe
                {
                    int stride = sizeof(NoiseLayerBuffer);
                    _noiseLayerComputeBuffer = new ComputeBuffer(vertexGenerate.shapeSettting._noiseLayers.Length, stride);
                }
            }

        }

        

        public void Dispose()
        {
            _bufferTriangles?.Release();

            _bufferVertices?.Release();
            
            _bufferUVs?.Release();
            
            _baseComputeBuffer?.Release();
            _noiseLayerComputeBuffer?.Release();
        }
    }
}