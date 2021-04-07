using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteUpdater : MonoBehaviour
{

    public BlockAttributes attr;
    void Start()
    {
        baseText_ = GetComponentInChildren<Text>().text;
    }

    void Update()
    {
        int octave = gameObject.transform.parent.GetComponentInParent<NoteSelector>().GetOctave();
        GetComponentInChildren<Text>().text = baseText_ + octave.ToString();
    }

    public void AddNote()
    {
        attr.AddNote(GetComponentInChildren<Text>().text);
    }

    string baseText_;
}
