using Unity.Mathematics;

namespace UnityTools.Physics.RayMarch
{
    public class RayIntersection
    {
        private const float maxFloat = 10e9f;

        //https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
        ///return distToBox,distThroughBox
        ///inside sphere disttobox = 0
        ///missed sphere disttobox = max,and distThroughBox =0
        ///rayDir must normalise
        public static float2 RayBoxIntersection(float3 rayOrigin,float3 rayDir,float3 boxmin,float3 boxmax)
        {
            float3 invdir = 1.0f / rayDir;
            float3 sign= math.step(invdir,0.0f);
            float3[] bounds = new float3[2];
            bounds[0] = boxmin;
            bounds[1] = boxmax;
    
    
            float tmin, tmax, tymin, tymax, tzmin, tzmax; 
    
    
            tmin = (bounds[(int)sign.x].x - rayOrigin.x) * invdir.x; 
            tmax = (bounds[1-(int)sign.x].x - rayOrigin.x) * invdir.x; 
            tymin = (bounds[(int)sign.y].y - rayOrigin.y) * invdir.y; 
            tymax = (bounds[1-(int)sign.y].y - rayOrigin.y) * invdir.y; 

            //miss
            if ((tmin > tymax) || (tymin > tmax)) 
                return new float2(maxFloat,0);

            tmin = math.max(tmin,tymin);
            tmax = math. min(tmax,tymax);
 
            tzmin = (bounds[(int)sign.z].z - rayOrigin.z) * invdir.z; 
            tzmax = (bounds[1-(int)sign.z].z - rayOrigin.z) * invdir.z; 

            //miss
            if ((tmin > tzmax) || (tzmin > tmax)) 
                return new float2(maxFloat,0);

            tmin = math.max(tmin,tzmin);
            tmax = math.min(tmax,tzmax);

    
            if(tmax>0)
            {
                tmin = math.max(0,tmin);
                return new float2(tmin,tmax-tmin);
            }

            //back but hit
            return new float2(maxFloat,0);
        }

    }
}