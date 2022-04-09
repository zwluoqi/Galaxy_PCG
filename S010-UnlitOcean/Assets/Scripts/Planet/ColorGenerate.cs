using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting ColorSettting;
        public LayerNoiseGenerate layerNoiseGenerate = new LayerNoiseGenerate();
        private Color[] colors;
        public void UpdateConfig(ColorSettting colorSettting)
        {
            this.ColorSettting = colorSettting;
            if (colors == null || colors.Length != ColorSettting.resolution  * ColorSettting.LatitudeSettings.Length)
            {
                colors = new Color[ColorSettting.resolution  * ColorSettting.LatitudeSettings.Length];
            }
            this.layerNoiseGenerate.UpdateConfig(colorSettting.noiseEnable,colorSettting.noiseSetting,colorSettting.noiseLayers);
        }
        
        public Color Execute()
        {
            return ColorSettting.tinyColor;
        }

        public void GenerateTexture2D(ref Texture2D texture2D,PlanetSettingData planetSettingData)
        {
            if (texture2D == null || ColorSettting.resolution != texture2D.width || ColorSettting.LatitudeSettings.Length != texture2D.height)
            {
                if (texture2D != null)
                {
                    Object.Destroy(texture2D);
                }

                texture2D = new Texture2D(ColorSettting.resolution , ColorSettting.LatitudeSettings.Length,
                    TextureFormat.RGBA32, 1, false) {wrapMode = TextureWrapMode.Clamp};
                //不然在边界采样值会有问题,因为如多是爽
            }
            
            int colorIndex = 0;
            for (int latitude = 0; latitude < ColorSettting.LatitudeSettings.Length; latitude++)
            {
                for (int i = 0; i < ColorSettting.resolution; i++)
                {
                    if (planetSettingData.ocean)
                    {
                        colors[colorIndex++] =
                            ColorSettting.ocean.Evaluate(1.0f * (i) / ColorSettting.resolution);
                    }
                    else
                    {
                        var gradientColor = ColorSettting.LatitudeSettings[latitude].gradient
                            .Evaluate(1.0f * (i ) / ColorSettting.resolution);
                        colors[colorIndex++] = Color.Lerp(gradientColor,
                            ColorSettting.LatitudeSettings[latitude].tinyColor
                            , ColorSettting.LatitudeSettings[latitude].tinyPercent);

                    }
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
            float blendRange = ColorSettting.blendRange + 0.01f;
            for (int i = 0; i < ColorSettting.LatitudeSettings.Length; i++)
            {

                float dist = noiseHeight - ColorSettting.LatitudeSettings[i].startHeight;
                float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);
                latitudeIndex *= (1.0f - weight);
                latitudeIndex += i + weight;
                // if (height >= noiseHeight)
                // {
                //     latitudeIndex = i;
                // }
            }

            return (latitudeIndex*1.0f) / Mathf.Max(1,ColorSettting.LatitudeSettings.Length-1);
        }
    }
}