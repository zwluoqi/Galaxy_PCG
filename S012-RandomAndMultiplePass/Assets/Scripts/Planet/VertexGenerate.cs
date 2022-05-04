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
            
            this._noiseGenerate = new NoiseGenerate(_noiseSetting.simpleNoise,_noiseSetting.noiseType);
            if (this._addNoiseGenerate == null ||
                this._addNoiseGenerate.Length != _noiseLayers.Length)
            {
                this._addNoiseGenerate = new NoiseGenerate[_noiseLayers.Length];
                for (int i = 0; i < _noiseLayers.Length; i++)
                {
                    this._addNoiseGenerate[i] = new NoiseGenerate(_noiseLayers[i].noiseSetting.simpleNoise,_noiseSetting.noiseType);
                }
            }
        }

       public Vector2 Exculate(Vector3 pos)
       {
           Vector2 noise = Vector2.zero;
           Vector2 baseNoise = Vector2.zero;
           if (_noiseEnable)
           {
               noise = this._noiseGenerate.Execute(pos);
               baseNoise = noise;
           }
           for (int i = 0; i < this._addNoiseGenerate.Length; i++)
           {
               if (_noiseLayers[i].enable)
               {
                   var mask = _noiseLayers[i].useMask ? baseNoise : Vector2.one;
                   noise += this._addNoiseGenerate[i].Execute(pos)*mask;
               }
           }

           return noise;
       }
    } 
    
    public class VertexGenerate
    {
        public ShapeSettting shapeSettting;
        private LayerNoiseGenerate _layerNoiseGenerate = new LayerNoiseGenerate();
        public RandomGenerate randomGenerate = new RandomGenerate();


        public void Update( ShapeSettting _shapeSettting,RandomSetting randomSetting)
        {
            this.shapeSettting = _shapeSettting;
            this.randomGenerate.Update(randomSetting);
            this._layerNoiseGenerate.UpdateConfig(this.shapeSettting._noiseEnable, this.shapeSettting._noiseSetting,
                this.shapeSettting._noiseLayers);
        }
        
        //
        public Vector3 Execulate(Vector3 pos,out Vector2 outNoise)
        {
            outNoise = this._layerNoiseGenerate.Exculate(pos);
            return pos*(1+outNoise.y)*shapeSettting.radius;
        }
    }


    internal class NoiseGenerate
    {
        private Noise _noise = new Noise();
        private SimpleNoise _noiseSettting;
        private NoiseType simpleNoiseType;

        public NoiseGenerate(SimpleNoise simpleNoise,NoiseType noiseType)
        {
            this._noiseSettting = simpleNoise;
            this.simpleNoiseType = noiseType;
            // _noise = new Noise(System.DateTime.Now.Millisecond);
        }

        public Vector2 Execute(Vector3 pos)
        {
            //(0,1);
            float value = 0;
            float roughness = _noiseSettting.roughness;
            float layerStrength = 1;
            for (int i = 0; i < _noiseSettting.layer; i++)
            {
                var v = ExecuteImp(simpleNoiseType,pos*roughness+this._noiseSettting.offset)*layerStrength;
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