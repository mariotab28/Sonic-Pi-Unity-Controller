using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockController : MonoBehaviour
{
    public TMPro.TMP_Text blockNameText;
    int id;

    public void ConfigureBlock(int blockId, string action)
    {
        blockNameText.text = action;
        id = blockId;
        
        // Creates the message object
        ActionMessage msg = null;

        //TODO: De momento está hardcodeado para pruebas
        if (action == "synth")
        {
            msg = new SynthMessage();
            msg.actionName = action;
            msg.blockId = id;
            if (id == 0)
            {
                (msg as SynthMessage).playerName = "piano";
                (msg as SynthMessage).notes = new List<int>(new int[4] { 64, 80, 95, 110 });
                (msg as SynthMessage).numOfNotes = (msg as SynthMessage).notes.Count;
                (msg as SynthMessage).pan = -1;
            }
            else
            {
                (msg as SynthMessage).playerName = "beep";
                (msg as SynthMessage).notes = new List<int>(new int[1] { 40 });
                (msg as SynthMessage).pan = 1;
            }
        }
        else if (action == "sleep")
        {
            msg = new SleepMessage();
            msg.actionName = action;
            msg.blockId = id;
            (msg as SleepMessage).sleepDuration = 1;
        }

            // Sends the action message
            SonicPiManager.Instance.sendActionMessage(msg);
    
    }
}
