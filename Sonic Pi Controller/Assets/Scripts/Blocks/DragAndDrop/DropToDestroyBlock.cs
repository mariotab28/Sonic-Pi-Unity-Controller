using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToDestroyBlock : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        BlockAttributes battr = eventData.pointerDrag.GetComponent<BlockAttributes>();
        if (!battr) return;

        // Remove from loop
        battr.GetLoop().RemoveBlockAt(battr.GetBlockId());

        Destroy(eventData.pointerDrag);
        LoopManager.instance.SetDestroyZone(false);
    }
}
