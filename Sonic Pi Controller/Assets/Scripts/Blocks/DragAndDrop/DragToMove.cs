using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToMove : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    LoopBlock loop;
    Canvas canvas;
    BlockAttributes blockAttributes;
    BlockShape shape;
    BlockDropHandler dropHandler;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    Vector2 initialPos;

    bool drag = false;

    private void Awake()
    {
        blockAttributes = GetComponent<BlockAttributes>();
        shape = GetComponent<BlockShape>();
        dropHandler = GetComponent<BlockDropHandler>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = LoopManager.instance.canvas;
    }

    private void Start()
    {
        loop = blockAttributes.GetLoop();
    }

    private void Update()
    {
        if (drag)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPos = rectTransform.anchoredPosition;
        drag = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;

        if (CompareTag("block"))
            LoopManager.instance.SetDestroyZone(true);
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

        if (CompareTag("block"))
            LoopManager.instance.SetDestroyZone(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("DOWN");
    }
}
