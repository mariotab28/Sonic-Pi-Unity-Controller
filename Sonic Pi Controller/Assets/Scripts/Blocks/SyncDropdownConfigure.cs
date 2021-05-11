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
        // Clear the old options of the Dropdown menu
        dropdown.ClearOptions();

        syncOptions = options;
        syncOptions.Insert(0, "Not syncing");

        // Add the options created in the List above
        dropdown.AddOptions(syncOptions);
    }

    public void OnValueChange()
    {
        string value = dropdown.options[dropdown.value].text;
        //Debug.Log("Syncing to " + value);
        loopBlock.SetSync(value != "Not syncing" ? value : "");
    }
}