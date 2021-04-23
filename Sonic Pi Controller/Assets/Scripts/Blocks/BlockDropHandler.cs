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
        if(!shape.HasEdge())
            return;
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged"))
        {
            if (eventData.pointerDrag.CompareTag("block"))
                MoveBlock(eventData.pointerDrag);
            else
                LoopManager.instance.AddBlockToLoop(loopC.loopId, blockAttributes.GetBlockId() + 1, eventData.pointerDrag.tag);
            shape.SetHighlighted(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!shape.HasEdge())
            return;
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged"))
        {
            shape.SetHighlighted(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!shape.HasEdge())
            return;
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged"))
        {
            shape.SetHighlighted(false);
        }
    }

    public void MoveBlock(GameObject block)
    {
        BlockShape bshape = block.GetComponent<BlockShape>();
        if (!bshape) return;

        LoopManager.instance.ChangeBlockPosition(loopC.loopId, bshape, blockAttributes.GetBlockId() + 1);
    }

}
