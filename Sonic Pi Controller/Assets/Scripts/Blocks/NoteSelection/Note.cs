using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public int index_;

    NoteMenuManager noteManager_;

    public void RemoveNote()
    {
        noteManager_.RemoveNote(index_);
        Destroy(gameObject);
    }

    public void SetIndex(NoteMenuManager notemenu, string note)
    {
        GetComponentInChildren<Text>().text = note;
        noteManager_ = notemenu;
        index_ = noteManager_.GetNoteCount()-1;
    }
}
