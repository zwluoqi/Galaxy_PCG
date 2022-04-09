

using System.Collections.Generic;
using UnityEngine;

public class AutoGenerateSolarSystem:MonoBehaviour
{

        
        [Range(3000,5000)]
        public float sunRadius = 3000;
        //sun*(10-100)R
        //moon*(0.1-0.01)R
        [Header("地球半径缩放")]
        public Vector2 earthRadiusScale = new Vector2(0.05f, 0.1f);

        [Range(1.0f,30)]
        public float sunGravity = 10;
        // [Header("地球引力缩放")]
        // public Vector2 earthGravityScale = new Vector2(0.03f, 0.05f);
        
        [Header("行星三维分布")]
        public Vector3 axis = Vector3.one;
        // [Header("行星初速度三维分布")]
        // public Vector3 speedAxis = Vector3.one;

        [Header("地球数量")]
        public int numberEarth = 9;

        public bool moonNormalFollow = true;

        public static AutoGenerateSolarSystem _Inst;
        public static AutoGenerateSolarSystem Inst
        {
                get
                {
                        if (_Inst == null)
                        {
                                _Inst = FindObjectOfType<AutoGenerateSolarSystem>();
                        }

                        return _Inst;
                }
        }


        public void Generator()
        {
                var randomSunRadius = sunRadius;
                var randomSunSurfaceGravity = sunGravity;
                var sun = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Astronomical>();
                sun.name = "sun";
                sun.transform.position = Vector3.zero;
                sun.Radius = randomSunRadius;
                sun.surfaceGravity = randomSunSurfaceGravity;
                // sun.InitVelocity = MathUtil.RandomVector() * 100;
                var vec = (MathUtil.RandomVector());
                sun._color = new Color(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
                sun.OnValidate();
                SolarSystemSimulater.Inst.centerTrans = sun; 

                

                //Mass = surfaceGravity * Radius * Radius / GlobalDefine.G;
                // var minCircleGravity = sun.surfaceGravity * SolarSystemSimulater.Inst.minGravityScale;
                // var maxCircleGravity = sun.surfaceGravity * SolarSystemSimulater.Inst.maxGravityScale;
                //
                var circleMin = sun.Radius * Mathf.Sqrt(1/SolarSystemSimulater.Inst.minGravityScale);
                var circleMax = sun.Radius * Mathf.Sqrt(1/SolarSystemSimulater.Inst.maxGravityScale);

                // var tile = (circleMax - circleMin) / numberEarth;
                float accumulativeRadius = 0;
                List<Astronomical> astronomicals = new List<Astronomical>(); 
                for (int i = 0; i < numberEarth; i++)
                {
                        var earth = CreateChildAstronomical(sun,circleMin,ref accumulativeRadius,i,MathUtil.RandomVector(), 0);
                        astronomicals.Add(earth);
                }

                // if (afterVelocity)
                // {
                //         var astronomicalArray = GameObject.FindObjectsOfType<Astronomical>();
                //         for (int i = 0; i < numberEarth; i++)
                //         {
                //                 astronomicals[i].InitVelocity = AstronomicalUtil.GetUpCircleInitVelocity(astronomicals[i],
                //                         Vector3.forward, sun, astronomicalArray);
                //                 astronomicals[i].OnValidate();
                //         }
                // }

                Debug.LogWarning("generator done");
        }

        private Astronomical CreateChildAstronomical(Astronomical parent,float circleMin,ref float accumulativeRadius,int i,Vector3 normal,int recursionCount)
        {
                var radiusScale = Random.Range(earthRadiusScale.x, earthRadiusScale.y);
                var autoRadius = parent.Radius * radiusScale;
                var subCircleMin = autoRadius * Mathf.Sqrt(1/SolarSystemSimulater.Inst.minGravityScale);
                var subCircleMax = autoRadius * Mathf.Sqrt(1/SolarSystemSimulater.Inst.maxGravityScale);
                accumulativeRadius += subCircleMax * Random.Range(3f, 6f);
                var distance = circleMin + accumulativeRadius;
                accumulativeRadius += subCircleMax;
                
                var randomDir = normal;
                randomDir.x *= axis.x;
                randomDir.y *= axis.y;
                randomDir.z *= axis.z;

                randomDir = randomDir.normalized;
                var pos = parent.transform.position + randomDir * distance;
                        
                var curGravityBySun = parent.Mass * GlobalDefine.G / (distance*distance);
                //地球表面引力远大于恒星给的引力
                var autoEarthGravity = curGravityBySun * 10;

                // var tmpMass = (sun.Mass / ((distance) * distance) * 10.0f) * (sun.Radius * radiusScale) *
                //               ((sun.Radius * radiusScale));
                


                var earth = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Astronomical>();
                earth.name = recursionCount==0 ? ("earth" + i) : ("moon" + i);
                earth.transform.position = pos;
                earth.Radius = autoRadius;
                earth.surfaceGravity = autoEarthGravity;
                earth.circleAstronomical = parent;
                var vecColor = (MathUtil.RandomVector());
                earth._color = new Color(Mathf.Abs(vecColor.x), Mathf.Abs(vecColor.y), Mathf.Abs(vecColor.z));
                if (axis.y==0&&axis.z==0)
                {
                        earth.InitVelocity = parent.GetUpCircleInitVelocity(earth.transform.position,Vector3.forward);
                }
                else
                {

                        earth.InitVelocity = parent.GetACircleInitVelocity(earth.transform.position);
                }

                
                earth.OnValidate();
                // return earth;
                if (recursionCount == 1)
                {
                        return earth;
                }
                
                
                var scaleTile = (earthRadiusScale.y - earthRadiusScale.x) / 3;
                var subChildCount = Mathf.CeilToInt((radiusScale - earthRadiusScale.x) / scaleTile);
                float subAccumulativeRadius = 0;
                for (int j = 0; j < subChildCount; j++)
                {
                        if (!moonNormalFollow)
                        {
                                randomDir = MathUtil.RandomVector();
                        }
                        CreateChildAstronomical(earth, subCircleMin, ref subAccumulativeRadius,j,randomDir,recursionCount+1);
                }

                return earth;
        }
}