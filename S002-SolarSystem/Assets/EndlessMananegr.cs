using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMananegr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform cameraTrans;
    public float distanceThreshold = 1000;
    private void FixedUpdate()
    {
        var colliders = GameObject.FindObjectsOfType<Collider>();
        var originPos = cameraTrans.transform.position;
        float distFromOrigin = originPos.magnitude;
        if (distFromOrigin > distanceThreshold)
        {
            foreach (var collider in colliders)
            {
                collider.transform.position -= originPos;
            }
        }
    }
}
