namespace Planet
{
    public class MinMax
    {
        public float min = float.MaxValue;
        public float max = float.MinValue;

        public void AddValue(float value)
        {
            if (value < min)
            {
                min = value;
            }

            if (value > max)
            {
                max = value;
            }
        }
        
        public void AddValue(MinMax value)
        {
            if (value.min < min)
            {
                min = value.min;
            }

            if (value.min > max)
            {
                max = value.max;
            }
        }
    }
}