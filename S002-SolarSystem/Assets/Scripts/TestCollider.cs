using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{


    private void OnCollisionEnter(Collision other)
    {
        Debug.LogWarning(other.gameObject.name+" OnCollisionEnter");
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.LogWarning(other.gameObject.name+" OnCollisionExit");
    }
}
