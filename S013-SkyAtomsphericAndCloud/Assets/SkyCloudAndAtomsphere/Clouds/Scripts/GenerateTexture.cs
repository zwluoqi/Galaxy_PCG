using System;
using System.Collections;
using System.Collections.Generic;
using Clouds.Settings;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityTools.Algorithm.Noise;


namespace Clouds
{
    public class GenerateTexture
    {
        public static Texture2D Generate(TextureSetting textureSetting)
        {
            Texture2D texture2D = new Texture2D(textureSetting.resolution, textureSetting.resolution);
            Color[] colors = new Color[textureSetting.resolution*textureSetting.resolution];
            int i = 0;
            for (int x = 0; x < textureSetting.resolution; x++)
            {
                for (int z = 0; z < textureSetting.resolution; z++)
                {
                    var pos = Vector3.right*(x-textureSetting.resolution/2)
                              +  Vector3.up
                              +  Vector3.forward*(z-textureSetting.resolution/2);
                    float v = 0;
                    for (int layer = 0; layer < textureSetting.noiseLayers.Length; layer++)
                    {
                        if (textureSetting.noiseLayers[layer].enable)
                        {
                            v += NoiseGenerate(pos, textureSetting.noiseLayers[layer]);
                        }
                    }
                    colors[i++] = new Color(v, v, v, v);
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
            return texture2D;
        }
        

        private static float NoiseGenerate(Vector3 pos,NoiseLayer textureSettingNoiseLayer)
        {
            // pos += textureSettingNoiseLayer.offset;
            float val = 0;
            float weigth = 1;
            float amplitude = 1;
            float frequency = textureSettingNoiseLayer.frequency*0.001f;
            Vector3 offset = textureSettingNoiseLayer.offset;
            for (int i = 0; i < textureSettingNoiseLayer.layerCount; i++)
            {
                var curPos = frequency * pos + offset;
                float v = NoiseTypeGenerate(curPos,textureSettingNoiseLayer.noiseType);

                v *= weigth;
                
                val += v * amplitude;
                
                weigth = Mathf.Clamp01(v*textureSettingNoiseLayer.layerWeigthMultiper);
                frequency *= textureSettingNoiseLayer.layerFrequencyMultiper;
                amplitude *= textureSettingNoiseLayer.layerAmplifyMultiper;
                offset *= textureSettingNoiseLayer.layerOffsetMultiper;
            }
            

            // v = Mathf.Max(0,v);
            
            val *= textureSettingNoiseLayer.amplify;
            return val;
        }

        private static float NoiseTypeGenerate(Vector3 curPos, NoiseType noiseType)
        {
            float v = 0;
            if (noiseType == NoiseType.Simplex)
            {
                v = 0.5f*noise.snoise(curPos)+0.5f;
            }
            else if (noiseType == NoiseType.Perlin)
            {
                v = 0.5f*noise.cnoise(curPos)+0.5f;
            }
            else if (noiseType == NoiseType.CustomSimplex)
            {
                v = 0.5f*SimplexNoise3D.snoise(curPos)+0.5f;
            }
            else if (noiseType == NoiseType.Worley)
            {
                v = 1-WorleyNoise3D.worley(curPos).x;
            }

            return v;
        }
    }
}