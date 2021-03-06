using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkyCell))]
[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerCtrl : MonoBehaviour
{
    public Camera _camera;
    public Rigidbody _rigidbody;
    public float rotSpeed = 60;
    public float rotSmoothSpeed = 5;
    public float moveSpeed = 10;
    public float jumpForce = 100000;
    
    public Vector3 astronAcceleration;
    public Vector3 inputDir;

    public int groundingCounter = 0;
    public GameObject latestCollider;
    public bool ctrl = false;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        
        LandShip();
        SolarSystemSimulater.Inst.OnFixedUpdate += FixedUpdate0;
    }


    private void OnCollisionEnter(Collision other)
    {
        groundingCounter++;
        latestCollider = other.gameObject;
        Debug.LogWarning(this.name+" OnCollisionEnter:"+other.gameObject.name);
    }
    
    private void OnCollisionExit(Collision other)
    {
        groundingCounter--;
        if (groundingCounter <= 0)
        {
            latestCollider = null;
        }
        Debug.LogWarning(this.name+" OnCollisionExit:"+other.gameObject.name);
    }


    public int GetInputAxis(KeyCode l, KeyCode r)
    {
        if (Input.GetKey(l))
        {
            return -1;
        }else if (Input.GetKey(r))
        {
            return 1;
        }

        return 0;
    }


    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate0(float detalTime)
    {
        if (ctrl)
        {
            MoveOnPlanet(detalTime);
        }
    }

    private void MoveOnPlanet(float detalTime)
    {
        var astrons = GameObject.FindObjectsOfType<Astronomical>();
        astronAcceleration = Vector3.zero;
        var maxAcceleration = float.MinValue;
        Astronomical nearestAstronomical = null;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical.transform.position - _rigidbody.position);
            var forceDir = (astronomical.transform.position - _rigidbody.position).normalized;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            // var acceleration = force / _rigidbody.mass;
            // CurrentVelocity += acceleration * fixedTime;
            astronAcceleration += acceleration;
            if (acceleration.magnitude > maxAcceleration)
            {
                maxAcceleration = acceleration.magnitude;
                nearestAstronomical = astronomical;
            }
            // _rigidbody.AddForce(acceleration, ForceMode.Acceleration);
        }
        
        //???????????????
        _rigidbody.AddForce(astronAcceleration, ForceMode.Acceleration);


        //???????????????????????????
        var curUp = _rigidbody.rotation * Vector3.up;
        _rigidbody.rotation = Quaternion.FromToRotation(curUp, -(nearestAstronomical.transform.position - _rigidbody.position).normalized) * _rigidbody.rotation;

        
        var up = _rigidbody.rotation * Vector3.up;
        var right = _rigidbody.rotation * Vector3.right;
        var forward = _rigidbody.rotation * Vector3.forward;
        var moveDir = forward * inputDir.z + right * inputDir.x;
        moveDir = moveDir.normalized;
        
        //????????????
        var xRot = Quaternion.AngleAxis(xMouseRot, up);
        var targetRot =  xRot * _rigidbody.rotation;
        var smoothTargetRot = Quaternion.Slerp(_rigidbody.rotation,targetRot,detalTime*rotSmoothSpeed);
        _rigidbody.MoveRotation(smoothTargetRot);
        
        //???????????????
        var cameraRight = _camera.transform.localRotation * Vector3.right;
        var yRot = Quaternion.AngleAxis(yMouseRot, cameraRight);
        var localRotation = _camera.transform.localRotation;
        var targetCameraRot =  yRot * localRotation;
        var cameraSmoothTargetRot = Quaternion.Slerp(localRotation,targetCameraRot,detalTime*rotSmoothSpeed);
        _camera.transform.localRotation = cameraSmoothTargetRot;
        

        //??????
        if (groundingCounter > 0)
        {
            _rigidbody.MovePosition(_rigidbody.position + moveDir * detalTime * moveSpeed);
            
            //??????
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var jumpDir = (up + moveDir).normalized;
                _rigidbody.AddForce(jumpDir*jumpForce,ForceMode.Force);
            }
        }
    }

    public float yMouseRot;
    public float xMouseRot;
    void HandleInput()
    {
        var z = GetInputAxis(KeyCode.S, KeyCode.W);
        var x = GetInputAxis(KeyCode.A, KeyCode.D);
        var y = GetInputAxis(KeyCode.E, KeyCode.Q);
        inputDir = new Vector3(x, y, z);

        if (Input.GetMouseButton(0))
        {
            xMouseRot = Input.GetAxis("Mouse X") * rotSpeed;
            yMouseRot = -Input.GetAxis("Mouse Y") * rotSpeed;
        }
        else
        {
            yMouseRot = 0;
            xMouseRot = 0;
        }
    }

    public void LandPlanet(Vector3 velocity)
    {
        ctrl = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = velocity;
        GetComponent<Collider>().enabled = true;
    }
    
    public void LandShip()
    {
        ctrl = false;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
    }

    public bool IsInShip()
    {
        if (ctrl)
        {
            return groundingCounter > 0 && latestCollider && latestCollider.GetComponent<SimpleSpaceShip>();
        }
        else
        {
            return true;
        }
    }
    
}



