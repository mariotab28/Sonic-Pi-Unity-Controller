using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDropHandler : MonoBehaviour, IDropHandler
{
    LoopBlock loopC;

    private void Awake()
    {
        loopC = GetComponent<LoopBlock>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            LoopManager.instance.AddBlockToLoop(loopC.loopId, loopC.blockCount, eventData.pointerDrag.tag);
        }
    }
}
