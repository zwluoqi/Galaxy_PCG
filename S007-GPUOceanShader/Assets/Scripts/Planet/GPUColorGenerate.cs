using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class GPUColorGenerate
    {
        private readonly int ResolutionID = Shader.PropertyToID("Resolution");
        private readonly int verticesID = Shader.PropertyToID("vertices");
        private readonly int uvsID = Shader.PropertyToID("uvs");
        private readonly int noiseLayerSettingsId = Shader.PropertyToID("noiseLayerSettings");
        private readonly int noiseAddLayerCountID = Shader.PropertyToID("noiseAddLayerCount");
        
        private readonly int colorSettingId = Shader.PropertyToID("colorSetting");
        private readonly int latitudesSettingId = Shader.PropertyToID("latitudes");
        private readonly int latitudeCountID = Shader.PropertyToID("latitudeCount");
        private readonly int formatuvsID = Shader.PropertyToID("formatuvs");
        
        private ComputeBuffer _baseColorComputeBuffer;
        
        private ComputeBuffer _latitudeComputeBuffer;
        private ComputeBuffer _noiseLayerComputeBuffer;
        private ComputeBuffer _bufferFormatUVs;
        
        
        public GPUColorGenerate()
        {
            unsafe
            {
                int stride = sizeof(ColorSettingBuffer);
                _baseColorComputeBuffer = new ComputeBuffer(1, stride);
            }
        }

        
        public void UpdateColorFormatHeight(int resolution,ColorSettting colorSettting,ComputeBuffer _bufferVertices ,ComputeBuffer _bufferUVs,
            Vector2[] formatUVs)
        {
            CreateColorBuffer(colorSettting,_bufferUVs);

            _bufferFormatUVs.SetData(formatUVs);

            _baseColorComputeBuffer.SetData(colorSettting.GetBaseBuffer());
            _noiseLayerComputeBuffer.SetData(colorSettting.GetNoiseLayersBuffer());
            _latitudeComputeBuffer.SetData(colorSettting.GetLatitudeSettingsBuffer());

            var computeShader = colorSettting.computeShader;
            
            computeShader.SetInt(ResolutionID, resolution);
            computeShader.SetInt(noiseAddLayerCountID, _noiseLayerComputeBuffer?.count ?? 0);
            computeShader.SetInt(latitudeCountID, _latitudeComputeBuffer?.count ?? 0);
            
            //获取内核函数的索引
            var kernelVertices = computeShader.FindKernel("CSMainHeight");
            computeShader.SetBuffer(kernelVertices,verticesID,_bufferVertices);
            computeShader.SetBuffer(kernelVertices,uvsID,_bufferUVs);
            computeShader.SetBuffer(kernelVertices,formatuvsID,_bufferFormatUVs);
            computeShader.SetBuffer(kernelVertices,colorSettingId,_baseColorComputeBuffer);
            computeShader.SetBuffer(kernelVertices, noiseLayerSettingsId, _noiseLayerComputeBuffer);
            computeShader.SetBuffer(kernelVertices, latitudesSettingId, _latitudeComputeBuffer);
            // var threadGroupxy = ((resolution - 1) / 8) + 1;
            computeShader.Dispatch(kernelVertices, resolution, resolution, 1);
            
            _bufferFormatUVs.GetData(formatUVs);
        }

        private void CreateColorBuffer(ColorSettting colorSettting,ComputeBuffer _bufferUVs)
        {
            if (_bufferFormatUVs != null && _bufferFormatUVs.count != _bufferUVs.count)
            {
                _bufferFormatUVs.Release();
                _bufferFormatUVs.Dispose();
                _bufferFormatUVs = null;
            }
            if (_bufferFormatUVs == null)
            {
                _bufferFormatUVs = new ComputeBuffer(_bufferUVs.count, 2*4);
            }

            if (_noiseLayerComputeBuffer != null &&
                _noiseLayerComputeBuffer.count != colorSettting.noiseLayers.Length)
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
                    _noiseLayerComputeBuffer = new ComputeBuffer(Mathf.Max(1,colorSettting.noiseLayers.Length), stride);
                }
            }
            
            
            if (_latitudeComputeBuffer != null &&
                _latitudeComputeBuffer.count != colorSettting.LatitudeSettings.Length)
            {
                _latitudeComputeBuffer.Release();
                _latitudeComputeBuffer.Dispose();
                _latitudeComputeBuffer = null;
            }
            if (_latitudeComputeBuffer == null)
            {
                unsafe
                {
                    int stride = sizeof(LatitudeSettingBuffer);
                    _latitudeComputeBuffer = new ComputeBuffer(Mathf.Max(1,colorSettting.LatitudeSettings.Length), stride);
                }
            }
        }

        public void Dispose()
        {
            _baseColorComputeBuffer?.Release();
            _latitudeComputeBuffer?.Release();
            _bufferFormatUVs?.Release();
        }
    }
}