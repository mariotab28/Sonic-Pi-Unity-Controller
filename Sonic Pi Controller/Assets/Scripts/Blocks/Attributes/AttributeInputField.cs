using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using TMPro;

public class AttributeInputField : MonoBehaviour
{
    BlockAttributes block;
    string attrName = "";

    [SerializeField] TMPro.TMP_Text placeholderText;
    [SerializeField] TMPro.TMP_Text nameText;

    public void ValueChange(string value)
    {
        block.SetAttribute(attrName, float.Parse(value, CultureInfo.CurrentCulture/* .InvariantCulture.NumberFormat*/));
    }

    public void Configure(BlockAttributes block, string attrName, float value)
    {
        this.block = block;
        this.attrName = attrName;
        nameText.text = attrName;
        placeholderText.text = value.ToString();
    }
}
