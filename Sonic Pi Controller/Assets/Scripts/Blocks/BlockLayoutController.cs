using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockLayoutController : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] HorizontalLayoutGroup layoutGroup;
    public void Rebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        rect.ForceUpdateRectTransforms();
    }

    public void SetEnable(bool en)
    {
        layoutGroup.enabled = en;
    }
}
