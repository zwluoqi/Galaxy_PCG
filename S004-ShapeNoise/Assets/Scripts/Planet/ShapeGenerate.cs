using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public enum NoiseType
    {
            CNOISE,
            SNOISE,
            SIMPLE,
    }
    public class ShapeGenerate
    {
        public ShapeSettting ShapeSettting;
        private NoiseGenerate _noiseGenerate;
        public ShapeGenerate(ShapeSettting shapeSettting)
        {
            this.ShapeSettting = shapeSettting;
            this._noiseGenerate = new NoiseGenerate(this.ShapeSettting._noiseSetting);
        }
        //
        public Vector3 Execulate(Vector3 normalPos)
        {
            float noise = this._noiseGenerate.Execute(normalPos);
            return (normalPos+noise*normalPos)*ShapeSettting.radius;
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
                var v = ExecuteImp(normalPos*roughness+this._noiseSettting.offset)*layerStrength;
                value += v;
                layerStrength *= _noiseSettting.layerMultiple;
                roughness *= _noiseSettting.layerRoughness;
            }
            
            // float value = ExecuteImp(normalPos*this._noiseSettting.roughness+this._noiseSettting.offset);

            float noise = (value) *this._noiseSettting.strength;
            return noise;
        }

        private float ExecuteImp(Vector3 inputPos)
        {
            float value = 0;
            switch (this._noiseSettting.noiseType)
            {
                case NoiseType.CNOISE:
                    value = Unity.Mathematics.noise.cnoise(inputPos);
                    break;;
                case NoiseType.SNOISE:
                    value = Unity.Mathematics.noise.snoise(inputPos);
                    break;
                case NoiseType.SIMPLE:
                    value = _noise.Evaluate(inputPos);
                    break;
                default:
                    value = 1;
                    break;
            }
            value = (value + 1) * 0.5f;
            return value;
        }
    }
}