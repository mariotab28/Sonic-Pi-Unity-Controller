using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    LoopBlock loopC;
    [SerializeField] BlockAttributes blockAttributes;
    [SerializeField] BlockShape shape;

    private void Start()
    {
        loopC = blockAttributes.GetLoop(); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            LoopManager.instance.AddBlockToLoop(loopC.loopId, blockAttributes.GetBlockId() + 1, eventData.pointerDrag.tag);
            shape.SetHighlighted(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            shape.SetHighlighted(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            shape.SetHighlighted(false);
        }
    }
}
