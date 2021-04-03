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
    GameObject fieldContainer;
    [SerializeField]
    TMPro.TMP_Text nameText;

    public void Configure(string action, Dictionary<string, float> attrs)
    {
        // Change the text at the top of the panel
        nameText.text = action.ToUpper() + " ATTRIBUTES";
    
        // Instantiate a field for each attribute and configure the field

    }

}
