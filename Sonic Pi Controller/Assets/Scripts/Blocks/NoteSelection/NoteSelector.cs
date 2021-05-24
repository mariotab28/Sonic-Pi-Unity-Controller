using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteSelector : MonoBehaviour
{
    NoteMenuManager noteMenu;

    Transform list;
    TMPro.TMP_Text noteText_;

    [SerializeField]
    Note notePF;

    // List of note buttons (NoteUpdater objects)
    List<NoteUpdater> noteButtons = new List<NoteUpdater>();

    int octave_ = 4;
    int noteIndex_ = 0;

    void Start()
    {
        UpdateButtonsOctave();
    }

    public int GetIndex() { return noteIndex_; }

    public void AddNoteButton(NoteUpdater noteButton)
    {
        noteButtons.Add(noteButton);
    }

    public void UpOctave()
    {
        if(octave_ < 8) octave_++;
        UpdateButtonsOctave();
    }

    public void DownOctave()
    {
        if(octave_ > 1) octave_--;
        UpdateButtonsOctave();
    }

    void UpdateButtonsOctave()
    {
        foreach (NoteUpdater noteButton in noteButtons)
            noteButton.UpdateOctave(octave_);
    }

    public int GetOctave()
    {
        return octave_;
    }

    public void Configure(NoteMenuManager noteMenu, Transform listTF, int note, TMPro.TMP_Text noteText)
    {
        this.noteMenu = noteMenu;
        list = listTF;
        noteIndex_ = note;
        noteText_ = noteText;
    }

    public void AddNote(string note)
    {
        noteMenu.AddNote(note);
        Note newnote = Instantiate(notePF, list);
        newnote.SetIndex(noteMenu, note);
    }

    public void ChangeNote(string note)
    {
        noteText_.text = note;
        noteMenu.ChangeNote(noteIndex_, note);
    }

}
