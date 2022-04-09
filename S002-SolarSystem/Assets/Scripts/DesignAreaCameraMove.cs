using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesignAreaCameraMove : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerClickHandler
{

    public Transform target;
    

    private Vector2 cameraArea;

    private Vector2 _Screen;

    private Vector3 cacheMousePosition;

    private bool dragging = false;

    public Action<PointerEventData, bool> OnPointClickAction;

    public static DesignAreaCameraMove Inst;

    private void Awake()
    {
        Inst = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            var delta = eventData.delta * cameraArea /_Screen;
            target.position -= new Vector3(delta.x,delta.y,0);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        _Screen = new  Vector2(Screen.width, Screen.height);
        
        var targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        var leftDownpoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, targetScreenPos.z));
        var rightToppoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, targetScreenPos.z));
        var w = rightToppoint.x - leftDownpoint.x;
        var h = rightToppoint.y - leftDownpoint.y;
        cameraArea.x = w;
        cameraArea.y = h;
        Debug.LogWarning($"leftDown{leftDownpoint},rightTop{rightToppoint},w{w},h{h}");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointClickAction?.Invoke(eventData,dragging);
    }
}
