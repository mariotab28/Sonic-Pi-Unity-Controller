using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockAttributes : MonoBehaviour
{
    public int id;
    public string action;

    // TODO: PROVISIONAL
    List<int> notes = new List<int>(); // List of notes to be played 
    int numOfNotes = 0; // Number of notes
    string mode = "tick"; // Notes play mode


    private void Awake()
    {
        // TODO: PROVISIONAL
        notes = new List<int>();
        //numOfNotes = notes.Count;
    }

    public ActionMessage GetActionMessage()
    {
        // Creates the message object
        ActionMessage msg = null;

        //TODO: De momento está hardcodeado para pruebas
        switch (action)
        {
            case "synth":
                msg = new SynthMessage();
                msg.actionName = action;
                msg.blockId = id;
                (msg as SynthMessage).playerName = "piano";
                (msg as SynthMessage).mode = mode;
                (msg as SynthMessage).notes = notes;
                (msg as SynthMessage).numOfNotes = numOfNotes;
                (msg as SynthMessage).pan = 0;
                (msg as SynthMessage).fx = "echo";
                (msg as SynthMessage).release = 5;
                break;

            case "sample":
                msg = new PlayerMessage();
                msg.actionName = "sample";
                msg.blockId = id;
                (msg as PlayerMessage).playerName = "bd_haus";
                (msg as PlayerMessage).fx = "echo";
                (msg as PlayerMessage).pan = 0;
                break;

            case "sleep":
                msg = new SleepMessage();
                msg.actionName = action;
                msg.blockId = id;
                (msg as SleepMessage).sleepDuration = 1;
                break;
            default:
                break;
        }

        // Returns the action message
        return msg; 
    }

    public void AddNote(string note)
    {
        int num = TranslateNote(note);
        notes.Add(num);
        numOfNotes++;
    }

    //Translate notes from English musical nomenclature to sonic pi's number system
    int TranslateNote(string note)
    {
        int aux = 0;
        switch(note[0])
        {
            case 'C':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1)+1;
                else aux = 12 * (int.Parse(note[1].ToString())+1);
                break;
            case 'D':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1)+3;
                else aux = 12 * (int.Parse(note[1].ToString())+1)+2;
                break;
            case 'E':
                aux = 12 * (int.Parse(note[1].ToString())+1)+4;
                break;
            case 'F':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1)+6;
                else aux = 12 * (int.Parse(note[1].ToString())+1)+5;
                break;
            case 'G':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1)+8;
                else aux = 12 * (int.Parse(note[1].ToString())+1)+7;
                break;
            case 'A':
                if (note[1] == '#') aux = 12 * (int.Parse(note[2].ToString()) + 1)+10;
                else aux = 12 * (int.Parse(note[1].ToString())+1)+9;
                break;
            case 'B':
                aux = 12 * (int.Parse(note[1].ToString())+1)+11;
                break;
        }
        return aux;
    }
}
