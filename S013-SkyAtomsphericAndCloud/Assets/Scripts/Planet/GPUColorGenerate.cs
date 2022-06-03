using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class GPUColorGenerate:System.IDisposable
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
                
                //运行时频繁变化大小会导致gpu崩溃
                stride = sizeof(LatitudeSettingBuffer);
                _latitudeComputeBuffer = new ComputeBuffer(8, stride);
            }
        }

        
        public void UpdateColorFormatHeight(int resolution,ColorGenerate colorGenerate,ComputeBuffer _bufferVertices ,ComputeBuffer _bufferUVs,
            Vector2[] formatUVs)
        {
            CreateColorBuffer(colorGenerate.colorSettting,_bufferUVs);

            _bufferFormatUVs.SetData(formatUVs);

            _baseColorComputeBuffer.SetData(colorGenerate.colorSettting.GetBaseBuffer(colorGenerate.randomGenerate.randomData));
            _noiseLayerComputeBuffer.SetData(colorGenerate.colorSettting.GetNoiseLayersBuffer(colorGenerate.randomGenerate.randomData));
            _latitudeComputeBuffer.SetData(colorGenerate.colorSettting.GetLatitudeSettingsBuffer(colorGenerate.randomGenerate.randomData));

            var computeShader = colorGenerate.colorSettting.computeShader;
            
            computeShader.SetInt(ResolutionID, resolution);
            computeShader.SetInt(noiseAddLayerCountID, _noiseLayerComputeBuffer?.count ?? 0);
            computeShader.SetInt(latitudeCountID, Mathf.Min(_latitudeComputeBuffer.count, colorGenerate.colorSettting.LatitudeSettings.Length));
            
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
        }
        

        public void Dispose()
        {
            _baseColorComputeBuffer?.Dispose();
            _latitudeComputeBuffer?.Dispose();
            _noiseLayerComputeBuffer?.Dispose();
            _bufferFormatUVs?.Dispose();
        }
    }
}