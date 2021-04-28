using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopBlock : MonoBehaviour
{
    #region Variables
    BlockShape shape;
    BlockAttributes attributes;
    BlockComparer bComparer; // Used por sorting the blocks list

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

        bComparer = new BlockComparer();

        attributes = GetComponent<BlockAttributes>();
        attributes.SetId(-1); // Set block ID to -1 por loops

        // Spawn initial Sleep block
        fixedSleepBlock = Instantiate(sleepBlockPF, loopContainerGO.transform);
        fixedSleepBlock.GetBlockAttributes().SetLoop(this);
        fixedSleepBlock.SetDraggable(false);
        fixedSleepBlock.SetSplitable(false);
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

    // Instantiate a block of a specific action and adds it to the loop
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
                Debug.LogError("Unknown action \"" + action + "\". Can't create block.");
                return;
        }

        AddBlock(block, blockId);
    }

    // Adds a block to the loop
    public void AddBlock(BlockShape block, int blockId)
    {
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

        // Update the other blocks indexes
        for (int i = block.GetBlockAttributes().GetBlockId() + 1; i < blocks.Count; i++)
        {
            BlockAttributes bAttr = blocks[i].GetBlockAttributes();
            bAttr.SetId(i);
            // A message is needed to update the block position in Sonic Pi
            blockChanges[i] = true;
        }

        foreach (BlockShape blockShape in blocks)
        {
            BlockAttributes attributes = blockShape.GetBlockAttributes();
            Transform t = blockShape.transform;
            t.SetSiblingIndex(attributes.GetBlockId() + 1);
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
            BlockAttributes battr = blocks[i].GetBlockAttributes();
            battr.SetId(battr.GetBlockId() - 1);
            blockChanges[i] = true;
        }
        blockCount--;
    }

    /// <summary>
    /// Change the position of a block both in visual representation and in the order of the block list.
    /// </summary>
    /// <param name="block">
    /// The block that is being moved.
    /// </param>
    /// <param name="newIndex">
    /// The index of the block in which it has been dropped.
    /// </param>
    public void ChangeBlockPosition(BlockShape block, int newIndex)
    {
        block.transform.SetParent(loopContainerGO.transform);

        // Change block id for this and successive blocks' attributes
        // Also
        // Change sibling index for this and successive blocks
        int prevIndex = block.GetBlockAttributes().GetBlockId();
        if (prevIndex > newIndex)
        {
            newIndex++; // Moves to the place in front of the block in which the moving block was dropped
            if (prevIndex == newIndex) // Moves to the same place
            {
                block.transform.SetSiblingIndex(newIndex);
                return;
            }
            MoveBlockLeft(block.GetBlockAttributes(), prevIndex, newIndex);
        }
        else if (prevIndex < newIndex)
        {
            MoveBlockRight(block.GetBlockAttributes(), prevIndex, newIndex);
        }
        else
        {
            block.transform.SetSiblingIndex(newIndex);
            return;
        }

        // Sort to change blocks list index
        blocks.Sort(bComparer);

        /*
        foreach (var bl in blocks)
        {
            Debug.Log(bl.name);
            bl.transform.SetParent(loopContainerGO.transform);
            bl.transform.SetAsLastSibling();
        }
        */

        Debug.Log("Moved from " + prevIndex + " to " + newIndex);
    }

    /// <summary>
    /// Changes block id from preIndex to newIndex inside the list of blocks and updates its parent's child hierarchy.
    /// Also, updates the id of the other blocks.
    /// The new id is lower than the previous
    /// </summary>
    private void MoveBlockLeft(BlockAttributes block, int preId, int newId)
    {
        if (newId >= preId) return;

        // Update the other blocks
        for (int i = newId; i <= preId - 1; i++)
        {
            int id = i + 1;
            blocks[i].GetBlockAttributes().SetId(id);
            //blocks[i].transform.SetSiblingIndex(id + 1);
        }

        // Update the moving block
        block.SetId(newId);
    }

    /// <summary>
    /// Changes block id from preIndex to newIndex inside the list of blocks and updates its parent's child hierarchy.
    /// Also, updates the id of the other blocks.
    /// The new id is higher than the previous
    /// </summary>
    private void MoveBlockRight(BlockAttributes block, int preId, int newId)
    {
        if (newId <= preId) return;

        // Update the other blocks
        for (int i = preId + 1; i <= newId; i++)
        {
            int id = i - 1;
            blocks[i].GetBlockAttributes().SetId(id);
            //blocks[i].transform.SetSiblingIndex(id + 1);
        }

        // Update the moving block
        block.SetId(newId);
    }
}
