using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySphere : MonoBehaviour
{
    private void Start()
    {
            
    }

    [Min(0.1f)] public float atomScale = 1;
    [Min(0.0f)]
    public float atomDensityFalloff = 1;
    [Min(0.01f)]
    public float lightPhaseValue = 1;
    [Min(1)]
    public int numberStepSky=1;
    
    [Min(1)]
    public int numberStepLight=1;
    public float lightAbsorptionTowardSun=1;
    [Min(0f)]
    public float darknessThreshold = 0f;

    public Vector3 rgbWaveLengths = new Vector3(700, 530, 440);

    [Min(0.0001f)]
    public float sunSmoothness = 1.0f;
}
