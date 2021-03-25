using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas canvas;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGIN START");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGIN...");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGIN ENDS!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DOWN");
    }
}
