using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteUpdater : MonoBehaviour
{
    [SerializeField]
    NoteSelector noteSelector;
    string baseText_;
    Text buttonText;

    private void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        baseText_ = buttonText.text; // Initialize base text

        // Add the button to the note panel buttons list
        if (noteSelector) noteSelector.AddNoteButton(this);
        else Debug.LogError("Error: Note panel not found in note button.");
    }

    public void UpdateOctave(int octave)
    {
        buttonText.text = baseText_ + octave.ToString();
    }

    public void AddNote()
    {
        noteSelector.AddNote(GetComponentInChildren<Text>().text);
    }

}
