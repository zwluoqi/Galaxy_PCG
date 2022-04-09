using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetMove : MonoBehaviour
{
    private Transform _transform;
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
    }

    private Vector3 startPos;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }

        
        //
        // if (Input.GetMouseButton(0))
        // {
        //     var deltaPos = Input.mousePosition - startPos;
        //     startPos = Input.mousePosition;
        //     _transform.position -= new Vector3(deltaPos.x*moveSpeed * Time.fixedDeltaTime, deltaPos.y*moveSpeed * Time.fixedDeltaTime, 0f);
        // }

        if (Input.GetKey(KeyCode.A))
        {
            _transform.position -= new Vector3(moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _transform.position += new Vector3(moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.W))
        {
            _transform.position += new Vector3(0f, moveSpeed * Time.fixedDeltaTime, 0f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            _transform.position -= new Vector3(0f, moveSpeed * Time.fixedDeltaTime, 0f);
        }
    }
}
