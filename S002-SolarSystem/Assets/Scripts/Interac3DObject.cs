using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interac3DObject: MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Action onClick;
    public Action<bool,Vector2> onPress;

    public static Action<GameObject> onGlobalClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning(this.name+" click");
        onClick?.Invoke();
        onGlobalClick?.Invoke(this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.LogWarning(this.name+" enter");
        onPress?.Invoke(true,eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.LogWarning(this.name+" exit");
        onPress?.Invoke(false,eventData.position);

    }  
}