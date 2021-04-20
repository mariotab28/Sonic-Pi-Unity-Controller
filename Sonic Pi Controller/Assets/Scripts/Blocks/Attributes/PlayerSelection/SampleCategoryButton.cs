using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SampleCategoryButton : MonoBehaviour
{
    BlockAttributes bAttr;
    List<string> samples;

    SampleSelectionPanel panel;
    int index;
    [SerializeField] TMPro.TMP_Text nameText;

    [SerializeField] PlayerSelectButton selectButtonPF;
    
    public void Configure(SampleSelectionPanel panel, int index, string name)
    {
        this.panel = panel;
        this.index = index;
        nameText.text = name;
    }

    public void Click()
    {
        panel.CategorySelect(index);
    }
}
