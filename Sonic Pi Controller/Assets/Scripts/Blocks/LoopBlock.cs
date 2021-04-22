using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBlock : MonoBehaviour
{
    #region Variables
    BlockShape shape;
    BlockAttributes attributes;

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

    List<BlockShape> blocks = new List<BlockShape>();   // List of blocks contained in this loop
    List<bool> blockChanges = new List<bool>();         // List of booleans indicating if the block of the same index has changes
    BlockShape fixedSleepBlock; // The sleep block that must contain the loop unless it is synced
    #endregion

    #region Initialization
    private void Start()
    {
        shape = GetComponent<BlockShape>();
        shape.SetColor(new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f)));
        shape.AddBottomExtension(shape.GetColor());

        attributes = GetComponent<BlockAttributes>();
        attributes.SetId(-1); // Set block ID to -1 por loops

        // Spawn initial Sleep block
        fixedSleepBlock = Instantiate(sleepBlockPF, loopContainerGO.transform);
        fixedSleepBlock.GetBlockAttributes().SetLoop(this);
        blocks.Add(fixedSleepBlock);
        blockChanges.Add(true);
        blockCount++;
        fixedSleepBlock.AddBottomExtension(shape.GetColor()); // Add an extension to the block to indicate hierarchy
    }
    #endregion

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
        for (int i = 0; i < blockChanges.Count; i++)
        {
            if (blockChanges[i])
            {
                messages.Add(blocks[i].GetBlockAttributes().GetActionMessage());
                blockChanges[i] = false;
            }
        }

        if (messages.Count <= 0)
            return;

        Debug.Log("Sending " + messages.Count + " messages.");

        //BlockAttributes ba = fixedSleepBlock.GetBlockAttributes();
        //SonicPiManager.instance.sendActionMessage(messages[0]);

        SonicPiManager.instance.sendActionMessageGroup(messages);
        messages.Clear();
        //messages.RemoveAt(0);
    }

    public void SetChangedBlock(int index)
    {
        blockChanges[index] = true;
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

        // Move the block to its corresponding position (+1 to skip loop block)
        block.transform.SetSiblingIndex(blockId + 1);

        // Assing the block loop to this loop
        block.SetLoop(this);

        // Activate the edge of the new block
        // TODO: unless it is last block (only when loop is synced)
        block.SetEdge(true);

        // Add bottom extension
        block.AddBottomExtension(shape.GetColor());

        // Move fixed sleep block to end
        fixedSleepBlock.transform.SetAsLastSibling();

        // Add it to the list
        blocks.Insert(blockId, block);
        blockChanges.Insert(blockId, true);
        blockCount++;

        // Set its id (-2 because sleep will always be last)
        // TODO: pass the id to this function
        block.GetBlockAttributes().SetId(blockId);

        //TODO: PROVISIONAL!!! Muevo el sleep al final
        //blocks.Remove(fixedSleepBlock);
        //blocks.Add(fixedSleepBlock);

        // Update the other blocks indexes
        for (int i = block.GetBlockAttributes().GetBlockId() + 1; i < blocks.Count; i++)
        {
            BlockAttributes bAttr = blocks[i].GetBlockAttributes();
            bAttr.SetId(i);
            // A message is needed to update the block position in Sonic Pi
            blockChanges[i] = true;
        }
        /*foreach (var b in blocks)
        {
            Debug.Log(b.GetBlockAttributes().GetBlockId() +": "+ b.GetBlockAttributes().GetActionMessage().actionName + "\n");
        }
        Debug.Log("\n");*/
    }

    public void RemoveBlockAt(int index)
    {
        blocks.RemoveAt(index);
        blockChanges.RemoveAt(index);

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
