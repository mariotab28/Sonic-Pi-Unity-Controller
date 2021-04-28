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
        // If this block has no edge, the dropped block can't be handled
        if(!shape.HasEdge())
            return;

        // Check if the dropped object is a block or a block spawner
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged"))
        {
            if (eventData.pointerDrag.CompareTag("block"))  // Block
                MoveBlock(eventData.pointerDrag);
            else  // Block spawner
                LoopManager.instance.AddBlockToLoop(loopC.loopId, blockAttributes.GetBlockId() + 1, eventData.pointerDrag.tag);
            shape.SetHighlighted(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightEdge(true, eventData.pointerDrag);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightEdge(false, eventData.pointerDrag);
    }

    public void MoveBlock(GameObject block)
    {
        BlockShape bshape = block.GetComponent<BlockShape>();
        if (!bshape) return;

        LoopManager.instance.ChangeBlockPosition(loopC.loopId, bshape, blockAttributes.GetBlockId());
    }

    void HighlightEdge(bool highlight, GameObject pointerDrag)
    {
        if (!shape.HasEdge())
            return;
        if (pointerDrag != null && !pointerDrag.CompareTag("Untagged"))
        {
            shape.SetHighlighted(highlight);
        }
    }

}
