using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synth : MonoBehaviour
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

        msg.amp = 1;
        msg.note = 80;
        msg.pan = 0;
        msg.synthName = "piano";

        // Sends the action message
        SonicPiManager.Instance.sendActionMessage(msg);

    }
}
