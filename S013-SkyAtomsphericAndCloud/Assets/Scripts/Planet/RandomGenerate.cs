using Planet.Setting;
using UnityEngine;

namespace Planet
{

    public class RandomGenerate
    {
        public bool enableRandom;
        public RandomData randomData = new RandomData();
        private RandomSetting randomSetting;
        MinMax _minMax = new MinMax();
        public void Update(RandomSetting _randomSetting)
        {
            enableRandom = _randomSetting.enableRandom;
            randomSetting = _randomSetting;


            if (enableRandom)
            {
                
                var x = GetRandomValue(_randomSetting.randomData.offsetRange.x);
                var y = GetRandomValue(_randomSetting.randomData.offsetRange.y);
                var z = GetRandomValue(_randomSetting.randomData.offsetRange.z);
                randomData.offsetRange = new Vector3(x, y, z);
                randomData.amplitudeAddPercent = GetRandomValue(_randomSetting.randomData.amplitudeAddPercent);
                randomData.frequencyAddPercent = GetRandomValue(_randomSetting.randomData.frequencyAddPercent);
                randomData.latitudeMaxNum = Mathf.FloorToInt(GetRandomValue(2,_randomSetting.randomData.latitudeMaxNum));
            }
            else
            {
                randomData.offsetRange = Vector3.zero;
                randomData.amplitudeAddPercent = 0;
                randomData.frequencyAddPercent = 0;
                randomData.latitudeMaxNum = 0;
            }
        }

        public float GetRandomValue(float randomDataAmplitudeAddPercent)
        {
            _minMax.AddValue(0);
            _minMax.AddValue(randomDataAmplitudeAddPercent);
            var z = Random.Range(_minMax.min, _minMax.max);
            _minMax.Clear();
            return z;
        }
        
        public float GetRandomValue(float x,float y)
        {
            _minMax.AddValue(x);
            _minMax.AddValue(y);
            var z = Random.Range(_minMax.min, _minMax.max);
            _minMax.Clear();
            return z;
        }

        Color Range( Color start, Color end)
        {
            var r = GetRandomValue(start.r,end.r);
            var g = GetRandomValue(start.g,end.g);
            var b = GetRandomValue(start.b,end.b);
            var a = GetRandomValue(start.a,end.a);
            return new Color(r, g, b, a);
        }

        public Gradient GetRandomGradient(out Color tinyColor, out float tinyPercent)
        {
            
            // var latitudeGradient = colorSettting.LatitudeSettings[latitude].gradient;
            var gradient = new Gradient();
            var gradientNum = (int)GetRandomValue(2.0f, randomSetting.randomData.latitudeKeyMaxNum);
            var colorKeys = new GradientColorKey[gradientNum];
            var alphaKeys = new GradientAlphaKey[0];
            for (int i = 0; i < gradientNum; i++)
            {
                colorKeys[i].color = Range(randomSetting.randomData.startColor,
                    randomSetting.randomData.endColor);
                colorKeys[i].time = i / (gradientNum - 1.0f);
                // alphaKeys[i].alpha = 1;
                // alphaKeys[i].time = 0;
            }

            gradient.SetKeys(colorKeys, alphaKeys);
            tinyColor = Range(randomSetting.randomData.startColor,
                randomSetting.randomData.endColor);
            tinyPercent = GetRandomValue(randomSetting.randomData.latitudeTinyPercent);
            return gradient;
        }

        public LatitudeSetting GetRandomLatitude()
        {
            LatitudeSetting setting = new LatitudeSetting();
            setting.gradient = GetRandomGradient(out setting.tinyColor, out setting.tinyPercent);
            return setting;
        }
    }

}