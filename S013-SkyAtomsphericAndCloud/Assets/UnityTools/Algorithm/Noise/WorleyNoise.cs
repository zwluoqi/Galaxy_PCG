
// using UnityEngine;

using Unity.Mathematics;

namespace UnityTools.Algorithm.Noise
{
    public class WorleyNoise
    {
        // Permutation polynomial: (34x^2 + x) mod 289
        static float3 permute(float3 x) {
            return math.fmod((34.0f * x + 1) * x, 289.0f);
        }

        static float3 dist(float3 x, float3 y,  bool manhattanDistance) {
            return manhattanDistance ?  math.abs(x) + math.abs(y) :  (x * x + y * y);
        }

        // [0,1]
        static  float2  worley(float2 P, float jitter=1, bool manhattanDistance = false) {
            float K= 0.142857142857f; // 1/7
            float Ko= 0.428571428571f ;// 3/7
            float2 Pi = math.fmod(math.floor(P), 289.0f);
            float2 Pf = math.frac(P);
            float3 oi = new float3(-1.0f, 0.0f, 1.0f);
            float3 of = new float3(-0.5f, 0.5f, 1.5f);
            float3 px = permute(Pi.x + oi);
            float3 p = permute(px.x + Pi.y + oi); // p11, p12, p13
            float3 ox = math.frac(p*K) - Ko;
            float3 oy = math.fmod(math.floor(p*K),7.0f)*K - Ko;
            float3 dx = Pf.x + 0.5f + jitter*ox;
            float3 dy = Pf.y - of + jitter*oy;
            float3 d1 = dist(dx,dy, manhattanDistance); // d11, d12 and d13, squared
            p = permute(px.y + Pi.y + oi); // p21, p22, p23
            ox = math.frac(p*K) - Ko;
            oy =  math.fmod( math.floor(p*K),7.0f)*K - Ko;
            dx = Pf.x - 0.5f + jitter*ox;
            dy = Pf.y - of + jitter*oy;
            float3 d2 = dist(dx,dy, manhattanDistance); // d21, d22 and d23, squared
            p = permute(px.z + Pi.y + oi); // p31, p32, p33
            ox =  math.frac(p*K) - Ko;
            oy =  math.fmod( math.floor(p*K),7.0f)*K - Ko;
            dx = Pf.x - 1.5f + jitter*ox;
            dy = Pf.y - of + jitter*oy;
            float3 d3 = dist(dx,dy, manhattanDistance); // d31, d32 and d33, squared
            // Sort out the two smallest distances (F1, F2)
            float3 d1a =  math.min(d1, d2);
            d2 =  math.max(d1, d2); // Swap to keep candidates for F2
            d2 =  math.min(d2, d3); // neither F1 nor F2 are now in d3
            d1 =  math.min(d1a, d2); // F1 is now in d1
            d2 =  math.max(d1a, d2); // Swap to keep candidates for F2
            d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
            d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
            d1.yz =  math.min(d1.yz, d2.yz); // F2 is now not in d2.yz
            d1.y =  math.min(d1.y, d1.z); // nor in  d1.z
            d1.y =  math.min(d1.y, d2.x); // F2 is in d1.y, we're done.
            return  math.sqrt(d1.xy);
        }
    }
}