using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToMove : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Canvas canvas;

    Vector2 initialPos;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    bool drag = false;

    private void Update()
    {
        if (drag)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = LoopManager.instance.canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPos = rectTransform.anchoredPosition;
        drag = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAGGIN...");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAGGIN ENDS!");
        //rectTransform.anchoredPosition = initialPos;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        drag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("DOWN");
    }
}
