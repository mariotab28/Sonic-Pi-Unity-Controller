using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Note : MonoBehaviour
{
    public int index_;

    NoteMenuManager noteManager_;
    [SerializeField]TMPro.TMP_Text text;

    public void SetIndex(NoteMenuManager notemenu, string note)
    {
        //GetComponentInChildren<Text>().text = note;
        text.text = note;
        noteManager_ = notemenu;
        index_ = noteManager_.GetNoteCount()-1;
    }

    public void SpawnPanel()
    {
        noteManager_.SpawnNotePanel(index_, text);
    }
    
}
