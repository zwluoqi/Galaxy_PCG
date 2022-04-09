using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScollMove : MonoBehaviour
{
    public Camera targetCamera;

    public float moveSpeed = 5;

    public float minSize = 10;
    public float maxSize = 30;

    private bool pressEnter = false;
    // Update is called once per frame
    void Update()
    {
        var scoll = Input.mouseScrollDelta.y;
        if (scoll == 0)
        {
            return;
        }
        var newSize = targetCamera.orthographicSize + scoll * Time.deltaTime * moveSpeed;
        newSize = Mathf.Clamp(newSize, minSize, maxSize);
        if (targetCamera.orthographicSize != newSize)
        {
            targetCamera.orthographicSize = newSize;
        }
    }
    
}
