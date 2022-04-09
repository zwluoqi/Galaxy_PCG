using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Planet.Setting
{
    [CreateAssetMenu()]
    public class ColorSettting:ScriptableObject
    {
        public int resolution = 128;
        public Color tinyColor;
        public Material material;
        public Gradient gradient;
        public Gradient ocean;
    }
}