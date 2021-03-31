using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        notes = new List<int>(new int[3] { 60, 65, 67 });
        numOfNotes = notes.Count;
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
}
