using UnityEngine;

public class MathUtil
{
    public static Vector3 RandomVector()
    {
        
        var x = Random.Range(0.1f, 1)*(Random.Range(0.0f,1)>0.5f?1:-1);
        var y = Random.Range(0.1f, 1)*(Random.Range(0.0f,1)>0.5f?1:-1);
        var z = Random.Range(0.1f, 1)*(Random.Range(0.0f,1)>0.5f?1:-1);
        return (new Vector3(x, y, z)).normalized;
    }

    public static float RandomPosOrNag()
    {
        var val = Random.Range(0.0f, 1.0f);
        if ( val> 0.5f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}