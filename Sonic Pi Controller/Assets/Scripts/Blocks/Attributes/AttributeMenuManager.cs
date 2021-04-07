using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeMenuManager : MonoBehaviour
{
    public AttributesMenuConfiguration attrPanelPF;

    [SerializeField]
    AttributeInputField fieldPF;

    AttributesMenuConfiguration attrPanel = null;

    public BlockAttributes attributes;

    public bool onBlock = false; // if true, configures the fields directly
    public bool isLoopBlock = false;
    public Vector2 containerSize = new Vector2(300, 0);
    public Vector2 containerAnchoredOffset = new Vector2(0, 0);
    public Vector3 containerScale = new Vector3(1, 1, 1);

    private void Start()
    {
        if (!attributes)
            attributes = GetComponent<BlockAttributes>();
        if (!attributes && !isLoopBlock)
            Debug.LogError("Error: Attributes script not found.");

        if (onBlock)
            SpawnAttributeFieldsOnBlock();
    }

    public void SpawnAttributeFieldsOnBlock()
    {
        // Create a cointainer
        GameObject fieldContainer = new GameObject();
        fieldContainer.name = "FieldContainer";
        fieldContainer.transform.parent = gameObject.transform;
        RectTransform rt = fieldContainer.AddComponent<RectTransform>();
        rt.sizeDelta = containerSize;
        rt.anchoredPosition = containerAnchoredOffset;
        rt.localScale = containerScale;
        fieldContainer.AddComponent<VerticalLayoutGroup>();

        Vector2 fieldSize = fieldPF.GetComponent<RectTransform>().sizeDelta;

        Dictionary<string, float> attrs;
        if (!isLoopBlock)
            attrs = attributes.GetActionMessage().attrs;
        else
            attrs = SonicPiManager.instance.GetActionDictionary("loop");

        // Instantiate a field for each attribute and configure the field
        foreach (KeyValuePair<string, float> attr in attrs)
        {
            AttributeInputField field = Instantiate(fieldPF, fieldContainer.transform);

            field.Configure(attributes, attr.Key, attr.Value);

            // Extend the height of the container
            rt.sizeDelta += new Vector2(0, fieldSize.y);
        }
    }


    public void SpawnAttributeMenu()
    {
        if (attrPanel == null)
        {
            attrPanel = Instantiate(attrPanelPF, LoopManager.instance.canvas.transform);
            attrPanel.Configure(attributes.action, attributes.GetActionMessage().attrs, attributes);
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
