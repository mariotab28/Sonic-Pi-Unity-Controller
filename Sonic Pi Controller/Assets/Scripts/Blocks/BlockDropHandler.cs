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

        shape.SetHighlighted(false);

        // Check if the dropped object is a block or a block spawner
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged") && !eventData.pointerDrag.CompareTag("loop"))
        {
            bool isBlock = eventData.pointerDrag.CompareTag("block");

            if (isBlock)  // Block
            {
                BlockAttributes otherAttr = eventData.pointerDrag.GetComponent<BlockAttributes>();
                if (!otherAttr) return;

                int thisLoop = blockAttributes.GetLoopId(), otherLoop = otherAttr.GetLoopId();
                // If the dropped block doesn't correspond to this loop, ignore it
                if (thisLoop != otherLoop)
                    return;

                // Change block position
                MoveBlock(eventData.pointerDrag);
            }
            else  // Block spawner
                LoopManager.instance.AddBlockToLoop(loopC.loopId, blockAttributes.GetBlockId() + 1, eventData.pointerDrag.tag);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        int thisLoop = blockAttributes.GetLoopId(), otherLoop;
        BlockAttributes otherAttr = eventData.pointerDrag.GetComponent<BlockAttributes>();
        if (!otherAttr) otherLoop = thisLoop;
        else otherLoop = otherAttr.GetLoopId();

        if(!eventData.pointerDrag.CompareTag("loop") && thisLoop == otherLoop) HighlightEdge(true, eventData.pointerDrag);
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
