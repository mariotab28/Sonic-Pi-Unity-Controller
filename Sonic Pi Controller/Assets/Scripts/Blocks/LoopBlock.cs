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

    List<BlockController> blocks = new List<BlockController>();

    public void AddMessage(ActionMessage msg)
    {
        messages.Add(msg);
    }

    public void SendActionMessages()
    {
        /*
        int c = messages.Count;
        for (int i = 0; i < c; i++)
        {
            SonicPiManager.Instance.sendActionMessage(messages[i]);
            Debug.Log("Sending messages");
        }

        messages.Clear();
        */

        SonicPiManager.Instance.sendActionMessage(messages[0]);
        messages.RemoveAt(0);
    }

    public void AddBlock(BlockController block)
    {
        blocks.Add(block);
        blockCount++;
    }

    public void RemoveBlockAt(int index)
    {
        blocks.RemoveAt(index);

        for (int i = index; i < blocks.Count; i++)
        {
            blocks[i].id--;
        }
        blockCount--;
    }


    /*
    public void UndoMessage(ActionMessage msg)
    {
        messages.RemoveAt();
    }
    */
}
