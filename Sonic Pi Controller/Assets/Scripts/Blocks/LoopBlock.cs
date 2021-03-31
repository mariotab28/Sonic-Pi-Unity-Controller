using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBlock : MonoBehaviour
{
    #region Variables
    BlockShape shape;
    
    public GameObject loopContainerGO;
    // Block Prefabs
    public BlockShape sleepBlockPF;
    public BlockShape synthBlockPF;
    public BlockShape sampleBlockPF;

    List<ActionMessage> messages = new List<ActionMessage>();

    // Loop Block Attributes
    public int loopId = 0;  // Index of the loop
    public int blockCount = 0;  // Number of blocks inside this loop
    int bpm = 60; // Loop's BPM
    string syncedWith = ""; // Name of the loop this loop is synced with (if any)

    List<BlockShape> blocks = new List<BlockShape>(); // List of blocks contained in this loop
    BlockShape fixedSleepBlock; // The sleep block that must contain the loop unless it is synced
    #endregion

    #region Initialization
    private void Start()
    {
        shape = GetComponent<BlockShape>();
        shape.SetColor(new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f)));
        shape.AddBottomExtension(shape.color);

        // Spawn initial Sleep block
        fixedSleepBlock = Instantiate(sleepBlockPF, loopContainerGO.transform);
        blocks.Add(fixedSleepBlock);
        blockCount++;
        fixedSleepBlock.AddBottomExtension(shape.color); // Add an extension to the block to indicate hierarchy
        messages.Add(fixedSleepBlock.GetBlockAttributes().GetActionMessage());
    }
    #endregion

    public void AddMessage(ActionMessage msg)
    {
        messages.Add(msg);
    }

    public void SendActionMessages()
    {
        if (messages.Count <= 0)
            return;
        /*
        int c = messages.Count;
        for (int i = 0; i < c; i++)
        {
            SonicPiManager.Instance.sendActionMessage(messages[i]);
            Debug.Log("Sending messages");
        }

        messages.Clear();
        */
        Debug.Log("Sending: " + messages[0].actionName);
        SonicPiManager.Instance.sendActionMessage(messages[0]);
        messages.RemoveAt(0);
    }

    public BlockShape AddSynthBlock()
    {
        return Instantiate(synthBlockPF, loopContainerGO.transform);
    }

    public BlockShape AddSampleBlock()
    {
        return Instantiate(sampleBlockPF, loopContainerGO.transform);
    }

    public BlockShape AddSleepBlock()
    {
        return Instantiate(sleepBlockPF, loopContainerGO.transform);
    }

    public void AddBlock(string action, int blockId)
    {
        Debug.Log("Adding " + action + " block.");
     
        // Instantiate a block depending on the action
        BlockShape block;
        switch (action)
        {
            case "synth":
                block = AddSynthBlock();
                break;
            case "sample":
                block = AddSampleBlock();
                break;
            case "sleep":
                block = AddSleepBlock();
                break;
            default:
                Debug.LogError("Unknown action. Can't create block.");
                return;
        }

        // Move the block to its corresponding position 
        block.transform.SetSiblingIndex(blockId);

        // Assing the block loop to this loop
        block.SetLoop(this);

        // Activate the edge of the new block
        // TODO: unless it is last block
        block.SetEdge(true);

        // Add bottom extension
        block.AddBottomExtension(shape.color);

        // Move fixed sleep block to end
        fixedSleepBlock.transform.SetAsLastSibling();

        // Add it to the list and increase blockCount
        blocks.Add(block);
        blockCount++;

        // Set its id (-2 because sleep will always be last)
        // TODO: only if not syncing
        block.GetBlockAttributes().id = blockCount - 2;

        // Add the message to the message list for this loop
        ActionMessage msg = block.GetBlockAttributes().GetActionMessage();
        messages.Add(msg);
    }

    public void RemoveBlockAt(int index)
    {
        blocks.RemoveAt(index);

        for (int i = index; i < blocks.Count; i++)
        {
            //blocks[i].id--;
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
