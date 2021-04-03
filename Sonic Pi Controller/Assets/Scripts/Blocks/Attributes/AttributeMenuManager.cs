using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeMenuManager : MonoBehaviour
{
    public AttributesMenuConfiguration attrPanelPF;
    AttributesMenuConfiguration attrPanel = null;

    BlockAttributes attributes;

    private void Start()
    {
        attributes = GetComponent<BlockAttributes>();
        if (!attributes)
            Debug.LogError("Error: Attributes script not found.");
    }

    public void SpawnAttributeMenu()
    {
        if (attrPanel == null)
        {
            attrPanel = Instantiate(attrPanelPF, LoopManager.instance.canvas.transform);
            attrPanel.Configure(attributes.action, attributes.GetActionMessage().attr);
        }
        else
            attrPanel.gameObject.SetActive(true);

        // Moves the panel on top of the UI and to the center of the screen
        attrPanel.transform.SetAsLastSibling();
        attrPanel.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void HideAttributeMenu()
    {
        if (attrPanel != null)
            attrPanel.gameObject.SetActive(false);
    }
}
