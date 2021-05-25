using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteMenuManager : MonoBehaviour
{
    [SerializeField]
    NoteSelector notePanelPF;
    bool instantiated = false;

    [SerializeField]
    Note notePF;

    NoteSelector notePanel;
    //public Dropdown modeDropdown_;
    [SerializeField] TMPro.TMP_Dropdown modeDropdown_;

    public Transform list;
    
    BlockAttributes attributes;

    List<int> notes;        // List of notes to be played 
    int numOfNotes = 0;     // Number of notes
    string mode = "tick";   // Notes play mode

    private void Awake()
    {
        attributes = GetComponent<BlockAttributes>();
        // TODO: PROVISIONAL
        /*notes = new List<int>(new int[3] { 60, 65, 67 });
        numOfNotes = notes.Count;*/
    }

    private void Start()
    {
        SynthMessage msg = (attributes.GetActionMessage() as SynthMessage);
        notes = msg.notes;
        msg.numOfNotes = 0;
        numOfNotes = msg.numOfNotes;
        mode = msg.mode;
    }

    public void SpawnNotePanel(int note, TMPro.TMP_Text noteText)
    {
        if (!instantiated)
        {
            if (!notePanel)
            {
                notePanel = Instantiate(notePanelPF, LoopManager.instance.canvas.transform);
                // Moves the panel on top of the UI and to the center of the screen
                notePanel.transform.SetAsLastSibling();
                notePanel.transform.localPosition = new Vector3(0, 0, 0);
                notePanel.Configure(this, list, note, noteText);
            }
            else
            {
                notePanel.gameObject.SetActive(true);
                notePanel.Configure(this, list, note, noteText);
            }
        }
        else
        {
            if(note == notePanel.GetIndex())
                notePanel.gameObject.SetActive(false);
            else notePanel.Configure(this, list, note, noteText);
        }
        instantiated = !instantiated;
    }

    public int GetNoteCount()
    {
        return notes.Count;
    }


    #region Note Selection Methods


    public void SpawnNote()
    {
        AddNote("C4");
        Note newnote = Instantiate(notePF, list);
        newnote.SetIndex(this, "C4");
    }

    public void AddNote(string note)
    {
        int num = TranslateNote(note);
        ActionMessage msg = attributes.GetActionMessage();
        (msg as SynthMessage).notes.Add(num);
        (msg as SynthMessage).numOfNotes++;

        notes = (msg as SynthMessage).notes;
        numOfNotes = (msg as SynthMessage).numOfNotes;

        attributes.GetLoop().SetChangedBlock(attributes.GetBlockId());
    }

    public void RemoveNote(int index)
    {
        ActionMessage msg = attributes.GetActionMessage();
        (msg as SynthMessage).notes.RemoveAt(index);
        (msg as SynthMessage).numOfNotes--;

        notes = (msg as SynthMessage).notes;
        numOfNotes = (msg as SynthMessage).numOfNotes;
    }

    public void ChangeNote(int index, string note)
    {
        int num = TranslateNote(note);
        ActionMessage msg = attributes.GetActionMessage();
        (msg as SynthMessage).notes[index] = num;

        notes = (msg as SynthMessage).notes;
        numOfNotes = (msg as SynthMessage).numOfNotes;

        attributes.GetLoop().SetChangedBlock(attributes.GetBlockId());
    }

    public void ChangeMode()
    {
        mode = modeDropdown_.options[modeDropdown_.value].text;
        SynthMessage msg = (attributes.GetActionMessage() as SynthMessage);
        msg.mode = mode;

        attributes.GetLoop().SetChangedBlock(attributes.GetBlockId());
    }

    //Translate notes from English musical nomenclature to sonic pi's number system
    int TranslateNote(string note)
    {
        int aux = 0;
        switch (note[0])
        {
            case 'C':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1) + 1;
                else aux = 12 * (int.Parse(note[1].ToString()) + 1);
                break;
            case 'D':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1) + 3;
                else aux = 12 * (int.Parse(note[1].ToString()) + 1) + 2;
                break;
            case 'E':
                aux = 12 * (int.Parse(note[1].ToString()) + 1) + 4;
                break;
            case 'F':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1) + 6;
                else aux = 12 * (int.Parse(note[1].ToString()) + 1) + 5;
                break;
            case 'G':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1) + 8;
                else aux = 12 * (int.Parse(note[1].ToString()) + 1) + 7;
                break;
            case 'A':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1) + 10;
                else aux = 12 * (int.Parse(note[1].ToString()) + 1) + 9;
                break;
            case 'B':
                aux = 12 * (int.Parse(note[1].ToString()) + 1) + 11;
                break;
        }
        return aux;
    }
    #endregion
}
