using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributesMenuConfiguration : MonoBehaviour
{
    public AttributeInputField fieldPF;
    List<AttributeInputField> fields = new List<AttributeInputField>();

    [SerializeField]
    RectTransform fieldContainer; // RectTransform of the field container
    [SerializeField]
    TMPro.TMP_Text nameText;

    Vector2 fieldSize; // Size of a field's RectTransform 

    public void Configure(string action, Dictionary<string, float> attrs, BlockAttributes block)
    {
        fieldSize = fieldPF.GetComponent<RectTransform>().sizeDelta;

        // Change the text at the top of the panel
        nameText.text = action.ToUpper() + " ATTRIBUTES";

        // Instantiate a field for each attribute and configure the field
        foreach (KeyValuePair<string, float> attr in attrs)
        {
            AttributeInputField field = Instantiate(fieldPF, fieldContainer.transform);
            fields.Add(field);

            field.Configure(block, attr.Key, attr.Value);

            // Extend the height of the container
            fieldContainer.sizeDelta += new Vector2(0, fieldSize.y);
        }
        
    }

}
