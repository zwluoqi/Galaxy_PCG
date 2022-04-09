using System;
using UnityEngine;

[RequireComponent(typeof(SkyCell))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleSpaceShip:MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float rotSpeed = 60;
    public float rotSmoothSpeed = 5;
    public float engineAcceleration = 20;

    
    public Vector3 astronAcceleration;
    public Vector3 inputDir;
    public bool grounding;
    
    private float changeDuration = 2.0f;
    private float changeTimer = 0;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        SolarSystemSimulater.Inst.OnFixedUpdate += FixedUpdate0;
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

    private void OnCollisionEnter(Collision other)
    {
        grounding = true;
        Debug.LogWarning("OnCollisionEnter:"+other.gameObject.name);
    }
    
    private void OnCollisionExit(Collision other)
    {
        grounding = false; 
        Debug.LogWarning("OnCollisionExit:"+other.gameObject.name);
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate0(float detalTime)
    {
        Move(detalTime);
    }

    private void Move(float detalTime)
    {
        changeTimer -= Time.deltaTime;
        var astrons = GameObject.FindObjectsOfType<Astronomical>();
        astronAcceleration = Vector3.zero;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical._rigidbody.position - _rigidbody.position);
            var forceDir = (astronomical._rigidbody.position - _rigidbody.position).normalized;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            astronAcceleration += acceleration;
        }
        //引力加速度
        _rigidbody.AddForce(astronAcceleration, ForceMode.Acceleration);
        
        
        var up = _rigidbody.rotation * Vector3.up;
        var right = _rigidbody.rotation * Vector3.right;
        var forward = _rigidbody.rotation * Vector3.forward;
        
        //飞船旋转
        if (!grounding)
        {
            var xRot = Quaternion.AngleAxis(xMouseRot, up);
            var yRot = Quaternion.AngleAxis(yMouseRot, right);
            var targetRot = xRot * yRot * _rigidbody.rotation;
            var smoothTargetRot =
                Quaternion.Slerp(_rigidbody.rotation, targetRot, detalTime * rotSmoothSpeed);
            _rigidbody.MoveRotation(smoothTargetRot);
        }


        //飞船动力
        // if (!grounding)
        {
            up = _rigidbody.rotation * Vector3.up;
            right = _rigidbody.rotation * Vector3.right;
            forward = _rigidbody.rotation * Vector3.forward;
            var moveDir = forward * inputDir.z + right * inputDir.x + up * inputDir.y;
            moveDir = moveDir.normalized;

            _rigidbody.AddForce(moveDir * engineAcceleration, ForceMode.Acceleration);
        }

        // var maxAccelerationAstronomical =
        //     SolarSystemSimulater.Inst.GetMaxAccelerationAstron(this._rigidbody.position, astrons);
        //
        // if (maxAccelerationAstronomical.Item1 != null)
        // {
        //     if (maxAccelerationAstronomical.Item1 != SolarSystemSimulater.Inst.centerTrans)
        //     {
        //         ChangeInertialFrameOfReference(maxAccelerationAstronomical.Item1);
        //     }
        // }
    }

    private void ChangeInertialFrameOfReference(Astronomical nearestAstronomical)
    {
        if (changeTimer > 0)
        {
            return;
        }

        changeTimer = changeDuration;
        var BeforeVelocity = SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity(); 
        //被天体捕获,调整惯性参考系
        SolarSystemSimulater.Inst.centerTrans = nearestAstronomical;
        var AfterVelocity = SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity();
        var speed = (BeforeVelocity - AfterVelocity);
        //调整相对速度
        var b = _rigidbody.velocity;
        // _rigidbody.AddForce(speed, ForceMode.VelocityChange);
        _rigidbody.velocity += speed;
        var a = _rigidbody.velocity;
        Debug.LogError("调整惯性参考系到:" + nearestAstronomical.name + "调整前:" + b + ",调整后：" + a+",差值:"+speed.magnitude);
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


    /// <summary>
    /// 紧急制动
    /// </summary>
    /// <param name="arg0"></param>
    public void Braking(bool arg0)
    {
        
    }
}