using System;
using UnityEngine;

namespace Planet.Setting
{
    [CreateAssetMenu()]
    public class RandomSetting : ScriptableObject
    {
        public bool enableRandom = false;
        [SerializeField]
        public RandomData randomData;

    }

    [Serializable]
    public class RandomData
    {
        [Range(-1,10)]
        public float frequencyAddPercent = 0;
        [Range(-1,10)]
        public float amplitudeAddPercent = 0;

        public Vector3 offsetRange;

        [Range(-1,1)]
        public float latitudeTinyPercent;

        public Color startColor = Color.black;
        public Color endColor= Color.white;
        [Range(2,8)]
        public int latitudeKeyMaxNum = 3;
        [Range(2,8)]
        public int latitudeMaxNum = 3;
    }
}