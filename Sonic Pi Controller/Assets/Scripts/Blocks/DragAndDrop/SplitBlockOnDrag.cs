using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SplitBlockOnDrag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    LoopBlock loop;
    Canvas canvas;
    BlockShape shape;
    BlockDropHandler dropHandler;

    List<Color> initialBottomColors;
    Transform initialParent;
    int initialSiblingIndex;

    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        //canvasGroup = GetComponent<CanvasGroup>();
        dropHandler = GetComponent<BlockDropHandler>();
        shape = GetComponent<BlockShape>();
        canvas = LoopManager.instance.canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable drop handler
        dropHandler.enabled = false;

        // Disable bottom extensions
        initialBottomColors = shape.GetBottomColors();
        shape.RemoveBottomExtensions();

        // Remove the block from the loop
        initialParent = gameObject.transform.parent;
        initialSiblingIndex = gameObject.transform.GetSiblingIndex();
        gameObject.transform.SetParent(canvas.transform);
        //loop.RemoveBlockAt(blockAttributes.GetBlockId());
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("DRAGGIN...");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Enable drop handler
        dropHandler.enabled = true;

        // Enable bottom extensions 
        shape.SetBottomColors(initialBottomColors);

        // Move back inside the loop
        gameObject.transform.SetParent(initialParent);
        gameObject.transform.SetSiblingIndex(initialSiblingIndex);
        //loop.RemoveBlockAt(blockAttributes.GetBlockId());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("DOWN");
    }
}
