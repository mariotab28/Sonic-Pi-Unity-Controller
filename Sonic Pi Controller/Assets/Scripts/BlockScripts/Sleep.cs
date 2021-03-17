using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    public TMPro.TMP_Text blockNameText;
    int id;
    int dur;
    public void ConfigureBlock(int blockId, string action)
    {
        blockNameText.text = action;
        id = blockId;

        // Creates the message object
        ActionMessage msg = new ActionMessage();
        msg.actionName = action;
        msg.blockId = id;
        msg.sleepDur = dur;

        // Sends the action message
        SonicPiManager.Instance.sendActionMessage(msg);

    }

    public void ChangeDur(string val)
    {
        dur = int.Parse(val);
        print(dur);
    }
}
