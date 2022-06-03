#ifndef QINGZHU_SHADER_LAB
#define QINGZHU_SHADER_LAB

#define maxFloat 10e7

//https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
//
///return distToSphere,distThroughSphere
///inside sphere disttosphere = 0
///missed sphere disttosphere = max,and distThroughSphere =0
///rayDir must normalise
float2 RaySphereIntersection(float3 center,float radius,float3 rayOrigin,float3 rayDir)
{
    float3 offset = rayOrigin - center;
    const float a=1;
    float b = 2*dot(offset,rayDir);
    float c = dot(offset,offset) - radius*radius;

    float discriminant = b*b-4*a*c;
    if(discriminant >0)
    {
        float s = sqrt(discriminant);
        float distToSphereNear = max(0,(-b-s)/(2*a));
        float distToSphereFar = (-b+s)/(2*a);
        if(distToSphereFar>=0)
        {
            return float2(distToSphereNear,distToSphereFar-distToSphereNear);
        }
    }

    return float2(maxFloat,0);
}


//https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
///return distToBox,distThroughBox
///inside sphere disttobox = 0
///missed sphere disttobox = max,and distThroughBox =0
///rayDir must normalise
float2 RayBoxIntersection(float3 rayOrigin,float3 rayDir,float3 boxmin,float3 boxmax)
{
    float3 invdir = 1.0 / rayDir;
    float3 sign= step(invdir,0.0);
    float3 bounds[2];
    bounds[0] = boxmin;
    bounds[1] = boxmax;
    
    
    float tmin, tmax, tymin, tymax, tzmin, tzmax; 
    
    
    tmin = (bounds[sign.x].x - rayOrigin.x) * invdir.x; 
    tmax = (bounds[1-sign.x].x - rayOrigin.x) * invdir.x; 
    tymin = (bounds[sign.y].y - rayOrigin.y) * invdir.y; 
    tymax = (bounds[1-sign.y].y - rayOrigin.y) * invdir.y; 

    //miss
    if ((tmin > tymax) || (tymin > tmax)) 
        return float2(maxFloat,0);

    tmin = max(tmin,tymin);
    tmax = min(tmax,tymax);
 
    tzmin = (bounds[sign.z].z - rayOrigin.z) * invdir.z; 
    tzmax = (bounds[1-sign.z].z - rayOrigin.z) * invdir.z; 

    //miss
    if ((tmin > tzmax) || (tzmin > tmax)) 
        return float2(maxFloat,0);

    tmin = max(tmin,tzmin);
    tmax = min(tmax,tzmax);

    
    if(tmax>0)
    {
        tmin = max(0,tmin);
        return float2(tmin,tmax-tmin);
    }

    //back but hit
    return float2(maxFloat,0);
}

#endif