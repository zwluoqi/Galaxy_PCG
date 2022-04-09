using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRigibody : MonoBehaviour
{
    public Rigidbody _Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _Rigidbody.AddForce(new Vector3(0,-10,0), ForceMode.Acceleration);
    }
}
