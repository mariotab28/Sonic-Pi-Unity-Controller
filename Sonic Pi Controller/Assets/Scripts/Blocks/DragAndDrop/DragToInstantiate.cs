using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToInstantiate : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Canvas canvas;

    Vector2 initialPos;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    bool drag = false;

    private void Update()
    {
        if (drag) {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Draggable go = Instantiate(gameObjectToInstantiate, eventData.delta / canvas.scaleFactor, Quaternion.identity);
        //go.canvas = canvas;
        initialPos = rectTransform.anchoredPosition;
        drag = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;

        if (gameObject.CompareTag("loop"))
            LoopManager.instance.SetAddLoopZone(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAGGIN...");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAGGIN ENDS!");
        rectTransform.anchoredPosition = initialPos;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        drag = false;

        if (gameObject.CompareTag("loop"))
            LoopManager.instance.SetAddLoopZone(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("DOWN");
    }
}
