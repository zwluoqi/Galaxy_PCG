using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class VertexGenerate
    {
        public ShapeSettting shapeSettting;
        private NoiseGenerate _noiseGenerate;
        private NoiseGenerate[] _addNoiseGenerate;
        
        public void UpdateConfig(ShapeSettting shapeSettting)
        {
            this.shapeSettting = shapeSettting;
            this._noiseGenerate = new NoiseGenerate((this.shapeSettting)._noiseSetting);
            if (this._addNoiseGenerate == null ||
                this._addNoiseGenerate.Length != this.shapeSettting._noiseLayers.Length)
            {
                this._addNoiseGenerate = new NoiseGenerate[this.shapeSettting._noiseLayers.Length];
                for (int i = 0; i < this.shapeSettting._noiseLayers.Length; i++)
                {
                    this._addNoiseGenerate[i] = new NoiseGenerate(this.shapeSettting._noiseLayers[i].noiseSetting);
                }
            }
        }
        
        //
        public Vector3 Execulate(Vector3 normalPos)
        {
            float noise = 0;
            float baseNoise = 0;
            if (shapeSettting._noiseEnable)
            {
                noise = this._noiseGenerate.Execute(normalPos);
                baseNoise = noise;
            }
            for (int i = 0; i < this._addNoiseGenerate.Length; i++)
            {
                if (this.shapeSettting._noiseLayers[i].enable)
                {
                    var mask = this.shapeSettting._noiseLayers[i].useMask ? baseNoise : 1;
                    noise += this._addNoiseGenerate[i].Execute(normalPos)*mask;
                }
            }
            return (normalPos+noise*normalPos)*shapeSettting.radius;
        }
    }
    

    internal class NoiseGenerate
    {
        private Noise _noise = new Noise();
        private NoiseSetting _noiseSettting;


        public NoiseGenerate(NoiseSetting noiseSetting)
        {
            this._noiseSettting = noiseSetting;
            // _noise = new Noise(System.DateTime.Now.Millisecond);
        }

        public float Execute(Vector3 normalPos)
        {
            //(0,1);
            float value = 0;
            float roughness = _noiseSettting.roughness;
            float layerStrength = 1;
            for (int i = 0; i < _noiseSettting.layer; i++)
            {
                var v = ExecuteImp(_noiseSettting.noiseType,normalPos*roughness+this._noiseSettting.offset)*layerStrength;
                value += v;
                layerStrength *= _noiseSettting.layerMultiple;
                roughness *= _noiseSettting.layerRoughness;
            }
            
            // float value = ExecuteImp(normalPos*this._noiseSettting.roughness+this._noiseSettting.offset);
            value = Mathf.Max(0, value - _noiseSettting.minValue);
            float noise = (value) *this._noiseSettting.strength;
            return noise;
        }

        private float ExecuteImp(NoiseType noiseType,Vector3 inputPos)
        {
            if (noiseType == NoiseType.Simplex)
            {
                float value = 0;
                // value = _noise.Evaluate(inputPos);
                value = Unity.Mathematics.noise.snoise(inputPos);
                value = (value + 1) * 0.5f;
                return value;
            }else if (noiseType == NoiseType.Rigidbody)
            {
                float value = 0;
                // value = _noise.Evaluate(inputPos);
                value = Unity.Mathematics.noise.snoise(inputPos);
                value = 1 - Mathf.Abs( Mathf.Sin(value));
                value *= value;
                return value;
            }

            return 0;
        }
    }
}