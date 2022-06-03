using Unity.Mathematics;

// copy from https://github.com/Erkaman/glsl-worley

namespace UnityTools.Algorithm.Noise
{
    public class WorleyNoise3D
    {
        // Permutation polynomial: (34x^2 + x) fmod 289
        static float3 permute(float3 x) {
	        return math.fmod((34.0f * x + 1) * x, 289.0f);
        }

        static float3 dist(float3 x, float3 y, float3 z,  bool manhattanDistance) {
		  return manhattanDistance ? math. abs(x) + math.abs(y) + math.abs(z) :  (x * x + y * y + z * z);
		}

        // [0,1]
        public static float2 worley(float3 P, float jitter=1, bool manhattanDistance=false) {
			
	        float K = 0.142857142857f; // 1/7
			float Ko = 0.428571428571f; // 1/2-K/2
			float  K2 = 0.020408163265306f; // 1/(7*7)
			float Kz = 0.166666666667f; // 1/6
			float Kzo = 0.416666666667f; // 1/2-1/6*2

			float3 Pi = math.fmod(math.floor(P), 289.0f);
 			float3 Pf = math.frac(P) - 0.5f;

			float3 Pfx = Pf.x + new float3(1.0f, 0.0f, -1.0f);
			float3 Pfy = Pf.y + new float3(1.0f, 0.0f, -1.0f);
			float3 Pfz = Pf.z + new float3(1.0f, 0.0f, -1.0f);

			float3 p = permute(Pi.x + new float3(-1.0f, 0.0f, 1.0f));
			float3 p1 = permute(p + Pi.y - 1.0f);
			float3 p2 = permute(p + Pi.y);
			float3 p3 = permute(p + Pi.y + 1.0f);

			float3 p11 = permute(p1 + Pi.z - 1.0f);
			float3 p12 = permute(p1 + Pi.z);
			float3 p13 = permute(p1 + Pi.z + 1.0f);

			float3 p21 = permute(p2 + Pi.z - 1.0f);
			float3 p22 = permute(p2 + Pi.z);
			float3 p23 = permute(p2 + Pi.z + 1.0f);

			float3 p31 = permute(p3 + Pi.z - 1.0f);
			float3 p32 = permute(p3 + Pi.z);
			float3 p33 = permute(p3 + Pi.z + 1.0f);

			float3 ox11 = math.frac(p11*K) - Ko;
			float3 oy11 = math.fmod(math.floor(p11*K), 7.0f)*K - Ko;
			float3 oz11 = math.floor(p11*K2)*Kz - Kzo; // p11 < 289 guaranteed

			float3 ox12 = math.frac(p12*K) - Ko;
			float3 oy12 = math.fmod(math.floor(p12*K), 7.0f)*K - Ko;
			float3 oz12 = math.floor(p12*K2)*Kz - Kzo;

			float3 ox13 = math.frac(p13*K) - Ko;
			float3 oy13 = math.fmod(math.floor(p13*K), 7.0f)*K - Ko;
			float3 oz13 = math.floor(p13*K2)*Kz - Kzo;

			float3 ox21 = math.frac(p21*K) - Ko;
			float3 oy21 = math.fmod(math.floor(p21*K), 7.0f)*K - Ko;
			float3 oz21 = math.floor(p21*K2)*Kz - Kzo;

			float3 ox22 = math.frac(p22*K) - Ko;
			float3 oy22 = math.fmod(math.floor(p22*K), 7.0f)*K - Ko;
			float3 oz22 = math.floor(p22*K2)*Kz - Kzo;

			float3 ox23 = math.frac(p23*K) - Ko;
			float3 oy23 = math.fmod(math.floor(p23*K), 7.0f)*K - Ko;
			float3 oz23 = math.floor(p23*K2)*Kz - Kzo;

			float3 ox31 = math.frac(p31*K) - Ko;
			float3 oy31 = math.fmod(math.floor(p31*K), 7.0f)*K - Ko;
			float3 oz31 = math.floor(p31*K2)*Kz - Kzo;

			float3 ox32 = math.frac(p32*K) - Ko;
			float3 oy32 = math.fmod(math.floor(p32*K), 7.0f)*K - Ko;
			float3 oz32 = math.floor(p32*K2)*Kz - Kzo;

			float3 ox33 = math.frac(p33*K) - Ko;
			float3 oy33 = math.fmod(math.floor(p33*K), 7.0f)*K - Ko;
			float3 oz33 = math.floor(p33*K2)*Kz - Kzo;

			float3 dx11 = Pfx + jitter*ox11;
			float3 dy11 = Pfy.x + jitter*oy11;
			float3 dz11 = Pfz.x + jitter*oz11;

			float3 dx12 = Pfx + jitter*ox12;
			float3 dy12 = Pfy.x + jitter*oy12;
			float3 dz12 = Pfz.y + jitter*oz12;

			float3 dx13 = Pfx + jitter*ox13;
			float3 dy13 = Pfy.x + jitter*oy13;
			float3 dz13 = Pfz.z + jitter*oz13;

			float3 dx21 = Pfx + jitter*ox21;
			float3 dy21 = Pfy.y + jitter*oy21;
			float3 dz21 = Pfz.x + jitter*oz21;

			float3 dx22 = Pfx + jitter*ox22;
			float3 dy22 = Pfy.y + jitter*oy22;
			float3 dz22 = Pfz.y + jitter*oz22;

			float3 dx23 = Pfx + jitter*ox23;
			float3 dy23 = Pfy.y + jitter*oy23;
			float3 dz23 = Pfz.z + jitter*oz23;

			float3 dx31 = Pfx + jitter*ox31;
			float3 dy31 = Pfy.z + jitter*oy31;
			float3 dz31 = Pfz.x + jitter*oz31;

			float3 dx32 = Pfx + jitter*ox32;
			float3 dy32 = Pfy.z + jitter*oy32;
			float3 dz32 = Pfz.y + jitter*oz32;

			float3 dx33 = Pfx + jitter*ox33;
			float3 dy33 = Pfy.z + jitter*oy33;
			float3 dz33 = Pfz.z + jitter*oz33;

			float3 d11 = dist(dx11, dy11, dz11, manhattanDistance);
			float3 d12 =dist(dx12, dy12, dz12, manhattanDistance);
			float3 d13 = dist(dx13, dy13, dz13, manhattanDistance);
			float3 d21 = dist(dx21, dy21, dz21, manhattanDistance);
			float3 d22 = dist(dx22, dy22, dz22, manhattanDistance);
			float3 d23 = dist(dx23, dy23, dz23, manhattanDistance);
			float3 d31 = dist(dx31, dy31, dz31, manhattanDistance);
			float3 d32 = dist(dx32, dy32, dz32, manhattanDistance);
			float3 d33 = dist(dx33, dy33, dz33, manhattanDistance);

			float3 d1a = math.min(d11, d12);
			d12 = math.max(d11, d12);
			d11 = math.min(d1a, d13); // Smallest now not in d12 or d13
			d13 = math.max(d1a, d13);
			d12 = math.min(d12, d13); // 2nd smallest now not in d13
			float3 d2a = math.min(d21, d22);
			d22 = math.max(d21, d22);
			d21 = math.min(d2a, d23); // Smallest now not in d22 or d23
			d23 = math.max(d2a, d23);
			d22 = math.min(d22, d23); // 2nd smallest now not in d23
			float3 d3a = math.min(d31, d32);
			d32 = math.max(d31, d32);
			d31 = math.min(d3a, d33); // Smallest now not in d32 or d33
			d33 = math.max(d3a, d33);
			d32 = math.min(d32, d33); // 2nd smallest now not in d33
			float3 da = math.min(d11, d21);
			d21 = math.max(d11, d21);
			d11 = math.min(da, d31); // Smallest now in d11
			d31 = math.max(da, d31); // 2nd smallest now not in d31
			d11.xy = (d11.x < d11.y) ? d11.xy : d11.yx;
			d11.xz = (d11.x < d11.z) ? d11.xz : d11.zx; // d11.x now smallest
			d12 = math.min(d12, d21); // 2nd smallest now not in d21
			d12 = math.min(d12, d22); // nor in d22
			d12 = math.min(d12, d31); // nor in d31
			d12 = math.min(d12, d32); // nor in d32
			d11.yz = math.min(d11.yz,d12.xy); // nor in d12.yz
			d11.y = math. min(d11.y,d12.z); // Only two more to go
			d11.y = math.min(d11.y,d11.z); // Done! (Phew!)
			return math.sqrt(d11.xy); // F1, F2

		}

    }
}