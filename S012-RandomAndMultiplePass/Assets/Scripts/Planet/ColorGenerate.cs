using Planet.Setting;
using Unity.Mathematics;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting colorSettting;
        public WaterRenderSetting waterRenderSetting;
        public RandomGenerate randomGenerate = new RandomGenerate();
        public LayerNoiseGenerate layerNoiseGenerate = new LayerNoiseGenerate();
        private Color[] colors;


        public void Update(ColorSettting _colorSettting,WaterRenderSetting _waterRenderSetting,RandomSetting _randomSetting)
        {
            if (this.colorSettting != null)
            {
                Object.DestroyImmediate(colorSettting);
            }
            this.colorSettting = Object.Instantiate(_colorSettting);
            this.waterRenderSetting = _waterRenderSetting;
            this.randomGenerate.Update(_randomSetting);
            if (this.randomGenerate.enableRandom)
            {
                RandomColorSetting();
            }
            if (colors == null || colors.Length != colorSettting.resolution  * colorSettting.LatitudeSettings.Length)
            {
                colors = new Color[colorSettting.resolution  * colorSettting.LatitudeSettings.Length];
            }
            this.layerNoiseGenerate.UpdateConfig(colorSettting.noiseEnable,colorSettting.noiseSetting,colorSettting.noiseLayers);
        }

        private void RandomColorSetting()
        {
            var latitudeMaxNum = this.randomGenerate.randomData.latitudeMaxNum;
            this.colorSettting.LatitudeSettings = new LatitudeSetting[latitudeMaxNum];
            for (int i = 0; i < latitudeMaxNum; i++)
            {
                var latitudeSetting = this.colorSettting.LatitudeSettings[i] = randomGenerate.GetRandomLatitude();
                var startHeight = i*1.0f / (latitudeMaxNum );;
                var endHeight = (i+1.0f)*1.0f / (latitudeMaxNum );;
                latitudeSetting.startHeight = randomGenerate.GetRandomValue(startHeight+0.01f,endHeight);
                this.colorSettting.LatitudeSettings[i] = latitudeSetting;
            }

            this.colorSettting.ocean = randomGenerate.GetRandomGradient(out var tinyColor, out var tinyPercent);
        }

        public void GenerateTexture2D(ref Texture2D texture2D)
        {
            if (texture2D == null || colorSettting.resolution != texture2D.width || colorSettting.LatitudeSettings.Length != texture2D.height)
            {
                if (texture2D != null)
                {
                    Object.DestroyImmediate(texture2D);
                }

                texture2D = new Texture2D(colorSettting.resolution , colorSettting.LatitudeSettings.Length,
                    TextureFormat.RGBA32, 1, false) {wrapMode = TextureWrapMode.Clamp};
                //不然在边界采样值会有问题,因为如多是爽
            }
            
            int colorIndex = 0;
            for (int latitude = 0; latitude < colorSettting.LatitudeSettings.Length; latitude++)
            {
                Gradient gradient = null;
                Color tinyColor;
                float tinyPercent;
                gradient = colorSettting.LatitudeSettings[latitude].gradient;
                tinyColor = colorSettting.LatitudeSettings[latitude].tinyColor;
                tinyPercent = colorSettting.LatitudeSettings[latitude].tinyPercent;

                for (int i = 0; i < colorSettting.resolution; i++)
                {
                    var gradientColor = gradient
                        .Evaluate(1.0f * (i ) / colorSettting.resolution);
                    colors[colorIndex++] = Color.Lerp(gradientColor,
                        tinyColor
                        , tinyPercent);
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
        }
        
        
        public void GenerateOceanTexture2D(ref Texture2D texture2D)
        {
            if (texture2D == null || colorSettting.resolution != texture2D.width || colorSettting.LatitudeSettings.Length != texture2D.height)
            {
                if (texture2D != null)
                {
                    Object.DestroyImmediate(texture2D);
                }

                texture2D = new Texture2D(colorSettting.resolution , colorSettting.LatitudeSettings.Length,
                    TextureFormat.RGBA32, 1, false) {wrapMode = TextureWrapMode.Clamp};
                //不然在边界采样值会有问题,因为如多是爽
            }

            Gradient ocean;
            ocean = colorSettting.ocean;


            int colorIndex = 0;
            for (int latitude = 0; latitude < colorSettting.LatitudeSettings.Length; latitude++)
            {
                for (int i = 0; i < colorSettting.resolution; i++)
                {
                    colors[colorIndex++] =
                        ocean.Evaluate(1.0f * (i) / colorSettting.resolution);
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
        }

        public float UpdateColorFormatHeight(Vector3 vertex,float height)
        {
            height = (height + 1) * 0.5f;
            float latitudeIndex = 0;
            // var noise = this.layerNoiseGenerate.Exculate(vertex);
            var noise = Vector2.zero;
            float noiseHeight = height + noise.y;
            float blendRange = colorSettting.blendRange + 0.01f;
            for (int i = 0; i < colorSettting.LatitudeSettings.Length; i++)
            {

                float dist = noiseHeight - colorSettting.LatitudeSettings[i].startHeight;
                float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);
                latitudeIndex *= (1.0f - weight);
                latitudeIndex += i + weight;
            }

            return (latitudeIndex*1.0f) / Mathf.Max(1,colorSettting.LatitudeSettings.Length-1);
        }
    }
}