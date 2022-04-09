using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTarget : MonoBehaviour
{
    public float distance = 100;
    public Transform target;

    public float smoothSpeed = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.LookAt(target.transform);
        this.transform.position = target.position - Vector3.forward * distance;
        // this.transform.position = Vector3.Lerp(target.position, target.position - Vector3.forward * distance,
        //     Time.deltaTime * smoothSpeed);
    }

    // private void OnValidate()
    // {
    //     LateUpdate();
    // }
}
