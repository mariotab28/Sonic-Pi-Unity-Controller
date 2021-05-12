using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopAttributeManager : MonoBehaviour
{
    [SerializeField] TMP_InputField bpmField;
    [SerializeField] TMP_InputField nameField;

    LoopBlock loop;

    private void Start()
    {
        loop = GetComponent<LoopBlock>();
        if (!loop) Debug.LogError("Error: Loop script not attached to Loop object.");
    }

    public void OnBPMChange()
    {
        int value = int.Parse(bpmField.text);
        //Debug.Log("SETTING BPM TO " + value);
        loop.SetBPM(value);
    }

    public void OnNameChange()
    {
        string newName = nameField.text;
        loop.SetName(newName);
    }
}
