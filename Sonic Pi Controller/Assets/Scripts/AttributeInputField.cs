using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeInputField : MonoBehaviour
{
    public BlockController block;
    public string attrName = "";

    public void ValueChange(string value)
    {
        block.EditAttribute(attrName, value);
    }
}
