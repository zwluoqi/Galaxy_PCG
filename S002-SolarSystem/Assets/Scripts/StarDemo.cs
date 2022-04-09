using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(ParticleSystem))]
public class StarDemo : MonoBehaviour
{
    public int numberStars;

    public Vector2 radius = new Vector2(100000,200000);

    public Vector2 startSize = new Vector2(1,10);
    public Color startColor = Color.red;

    private ParticleSystem.Particle[] particles;
    public ParticleSystem _particleSystem;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        GenerateStar();
    }

    void GenerateStar()
    {
        for (int i = 0; i < numberStars; i++)
        {
            // var color = UnityEngine.Random.Range(0.5f, 1.0f) * startColor;


            particles[i].position = radius.x* uniformCircle(UnityEngine.Random.Range(0.0f, 1.0f),
                UnityEngine.Random.Range(0.0f, 1.0f));
            particles[i].startSize = UnityEngine.Random.Range(startSize.x,startSize.y);
            particles[i].startColor = startColor;
        }
        _particleSystem.SetParticles(particles);
    }

    private void OnValidate()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[numberStars];
        var main = _particleSystem.main;
        main.simulationSpeed = 0;
        GenerateStar();
    }
    
    Vector3 hemisphereSample_uniform(float u, float v) {
        float phi = v * 2.0f * Mathf.PI;
        float cosTheta = 1.0f - u;
        float sinTheta = Mathf.Sqrt(1.0f - cosTheta * cosTheta);
        return new Vector3(Mathf.Cos(phi) * sinTheta, Mathf.Sin(phi) * sinTheta, cosTheta);
    }

    Vector3 uniformCircle(float u,float v)
    {
        var theta = Mathf.Acos( 1-2*u);
        var phi = v * 2 * Mathf.PI;
        // var rad = UnityEngine.Random.Range(0.5f, 1.0f) * radius;
        var y  =  Mathf.Cos(theta);
        var xz = Mathf.Sin(theta);
        var x = xz * Mathf.Cos(phi);
        var z = xz * Mathf.Sin(phi);
        return new Vector3(x,y,z);
    }
}
