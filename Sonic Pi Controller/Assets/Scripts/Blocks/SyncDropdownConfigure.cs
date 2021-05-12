using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SyncDropdownConfigure : MonoBehaviour
{
    // List of Dropdown options
    List<string> syncOptions;
    // Dropdown component
    [SerializeField] TMP_Dropdown dropdown;
    // LoopBlock component
    [SerializeField] LoopBlock loopBlock;

    public void SetSyncingOptions(List<string> options)
    {
        int current = dropdown.value;

        // Clear the old options of the Dropdown menu
        dropdown.ClearOptions();

        syncOptions = options;
        syncOptions.Insert(0, "Not syncing");

        // Add the options created in the List above
        dropdown.AddOptions(syncOptions);

        if(current < syncOptions.Count)
            dropdown.SetValueWithoutNotify(current);
    }

    public void OnValueChange()
    {
        string value = dropdown.options[dropdown.value].text;
        //Debug.Log("Syncing to " + value);
        loopBlock.SetSync(value != "Not syncing" ? value : "");
    }

    public void ResetSelection()
    {
        dropdown.SetValueWithoutNotify(0);
    }

    public void SetSelection(string selection)
    {
        int i = 0;
        bool changed = false;
        while (i < syncOptions.Count && !changed)
        {
            if (syncOptions[i] == selection)
            {
                dropdown.SetValueWithoutNotify(i);
                changed = true;
            }
            i++;
        }
    }
}