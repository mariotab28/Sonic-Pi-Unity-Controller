using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoopDropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Check if the dropped object is a loop block
        if (eventData.pointerDrag != null && !eventData.pointerDrag.CompareTag("Untagged") && eventData.pointerDrag.CompareTag("loop"))
        {
            //Debug.Log("LOOP!");
            LoopManager.instance.AddLoop();
            gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Highlight On
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Highlight Off
    }

}
