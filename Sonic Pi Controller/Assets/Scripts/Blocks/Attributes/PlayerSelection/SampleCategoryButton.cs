using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCategoryButton : MonoBehaviour
{
    BlockAttributes bAttr;
    List<string> samples;
    SampleSelectionPanel panel;
    int index;

    [SerializeField]
    PlayerSelectButton selectButtonPF;
    
    public void Configure(SampleSelectionPanel panel, int index)
    {
        this.panel = panel;
        this.index = index;
    }

    public void Click()
    {
        panel.CategorySelect(index);
    }
}
