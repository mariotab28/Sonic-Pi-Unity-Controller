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
        ActionMessage msg = new ActionMessage();
        msg.actionName = action;
        msg.blockId = id;

        //TODO: De momento está hardcodeado para pruebas
        if (action == "synth")
        {
            if (id == 0)
            {
                msg.synthName = "piano";
                msg.note = 80;
                msg.pan = -1;
            }
            else
            {
                msg.synthName = "beep";
                msg.note = 40;
                msg.pan = 1;
            }
        }

        // Sends the action message
        SonicPiManager.Instance.sendActionMessage(msg);
    
    }
}
