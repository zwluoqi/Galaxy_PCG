using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting ColorSettting;

        public ColorGenerate(ColorSettting colorSettting)
        {
            this.ColorSettting = colorSettting;
        }
        
        public Color Execute()
        {
            return ColorSettting.tinyColor;
        }
    }
}