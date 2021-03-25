using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBlock : MonoBehaviour
{
    public GameObject loopContainerGO;

    List<ActionMessage> messages = new List<ActionMessage>();

    public int loopId = 0;
    public int blockCount = 0;
    int bpm = 60;
    string syncedWith = "";


    public void AddMessage(ActionMessage msg)
    {
        messages.Add(msg);
    }

    public void SendActionMessages()
    {
        int c = messages.Count;
        for (int i = 0; i < c; i++)
        {
            SonicPiManager.Instance.sendActionMessage(messages[i]);
            Debug.Log("Sending messages");
        }

        messages.Clear();
    }


    /*
    public void UndoMessage(ActionMessage msg)
    {
        messages.RemoveAt();
    }
    */
}
