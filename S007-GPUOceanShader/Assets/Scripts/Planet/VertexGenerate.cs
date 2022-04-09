using Planet.Setting;
using UnityEngine;

namespace Planet
{

    public class LayerNoiseGenerate
    {
        private NoiseGenerate _noiseGenerate;
        private NoiseGenerate[] _addNoiseGenerate;

        private NoiseSetting _noiseSetting;
        private bool _noiseEnable;
        private NoiseLayer[] _noiseLayers;
       public void UpdateConfig(bool enable, NoiseSetting baseNoise, NoiseLayer[] layers)
        {
            this._noiseEnable = enable;
            this._noiseSetting = baseNoise;
            this._noiseLayers = layers;
            
            this._noiseGenerate = new NoiseGenerate(_noiseSetting);
            if (this._addNoiseGenerate == null ||
                this._addNoiseGenerate.Length != _noiseLayers.Length)
            {
                this._addNoiseGenerate = new NoiseGenerate[_noiseLayers.Length];
                for (int i = 0; i < _noiseLayers.Length; i++)
                {
                    this._addNoiseGenerate[i] = new NoiseGenerate(_noiseLayers[i].noiseSetting);
                }
            }
        }

       public Vector2 Exculate(Vector3 normalPos)
       {
           Vector2 noise = Vector2.zero;
           Vector2 baseNoise = Vector2.zero;
           if (_noiseEnable)
           {
               noise = this._noiseGenerate.Execute(normalPos);
               baseNoise = noise;
           }
           for (int i = 0; i < this._addNoiseGenerate.Length; i++)
           {
               if (_noiseLayers[i].enable)
               {
                   var mask = _noiseLayers[i].useMask ? baseNoise : Vector2.one;
                   noise += this._addNoiseGenerate[i].Execute(normalPos)*mask;
               }
           }

           return noise;
       }
    } 
    
    public class VertexGenerate
    {
        public ShapeSettting shapeSettting;
        private LayerNoiseGenerate _layerNoiseGenerate = new LayerNoiseGenerate();

        public void UpdateConfig(ShapeSettting shapeSettting)
        {
            this.shapeSettting = shapeSettting;
            this._layerNoiseGenerate.UpdateConfig(this.shapeSettting._noiseEnable, this.shapeSettting._noiseSetting,
                this.shapeSettting._noiseLayers);
        }
        
        //
        public Vector3 Execulate(Vector3 normalPos)
        {
            var noise = this._layerNoiseGenerate.Exculate(normalPos);
            return (normalPos+noise.x*normalPos)*shapeSettting.radius;
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

        public Vector2 Execute(Vector3 normalPos)
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
            float depth = value - _noiseSettting.minValue;
            value = Mathf.Max(0, value - _noiseSettting.minValue);
            float noise = (value) *this._noiseSettting.strength;
            depth *= this._noiseSettting.strength;
            return new Vector2(noise, depth);
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