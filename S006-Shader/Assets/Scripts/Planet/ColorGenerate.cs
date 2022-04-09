using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting ColorSettting;
        private Texture2D texture2D;
        public void UpdateConfig(ColorSettting colorSettting)
        {
            this.ColorSettting = colorSettting;
            if (texture2D == null || colorSettting.resolution*2 != texture2D.width)
            {
                texture2D = new Texture2D(colorSettting.resolution*2, 1, TextureFormat.RGBA32, 1,true);
                texture2D.wrapMode = TextureWrapMode.Clamp;//不然在边界采样值会有问题,因为如多是爽
            }

        }
        
        public Color Execute()
        {
            return ColorSettting.tinyColor;
        }

        public Texture2D GenerateTexture2D()
        {
            Color[] colors = new Color[ColorSettting.resolution*2];
            for (int i = 0; i < ColorSettting.resolution*2; i++)
            {
                if (i < ColorSettting.resolution)
                {
                    colors[i] = ColorSettting.ocean.Evaluate(1.0f*(i) / ColorSettting.resolution);
                }
                else
                {
                    colors[i] = ColorSettting.gradient.Evaluate(1.0f*(i - ColorSettting.resolution) / ColorSettting.resolution);
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
            return texture2D;
        }
    }
}