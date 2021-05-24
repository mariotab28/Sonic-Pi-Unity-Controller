using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

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
    bool playing = true; // Indicates if the loop is playing or paused
    bool loopAttrsChanged = false; // Flag that indicates wheter the loop attributes have changed

    List<BlockShape> blocks = new List<BlockShape>();   // List of blocks contained in this loop
    List<BlockShape> updatedBlocks = new List<BlockShape>();   // List of the blocks as it is being played in Sonic Pi
    List<bool> blockChanges = new List<bool>();         // List of booleans indicating if the block of the same index has changes
    BlockShape fixedSleepBlock; // The sleep block that must contain the loop unless it is synced
    [SerializeField] BlockShape endLoopBlock; // A fixed block shape at the end of the loop


    // Loop name
    [SerializeField] TMP_Text nameText;
    string loopCustomName = "";

    // Sync options
    [SerializeField] SyncDropdownConfigure syncDropdown;

    // Indication of the playing block
    int playingBlockId = 0;
    #endregion

    #region Initialization
    public void Init(int index)
    {
        // Get Shape
        shape = GetComponent<BlockShape>();
        if (!shape) Debug.LogError("Error: Loop block missing shape component!");
        // Get Attributes
        attributes = GetComponent<BlockAttributes>();
        if (!attributes) Debug.LogError("Error: Loop block missing attributes component!");
        // Init block comparer
        bComparer = new BlockComparer();

        // Set id of the loop
        loopId = index;

        shape.SetColor(new Color(Random.Range(0.4f, 1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f)));
        shape.AddBottomExtension(shape.GetColor());
        
        attributes.SetId(-1); // Set block ID to -1 por loops

        // Spawn initial Sleep block
        fixedSleepBlock = Instantiate(sleepBlockPF, loopContainerGO.transform);
        fixedSleepBlock.GetBlockAttributes().SetLoop(this);
        fixedSleepBlock.SetDraggable(false);
        fixedSleepBlock.SetSplitable(false);
        fixedSleepBlock.SetEdge(true);
        blocks.Add(fixedSleepBlock);
        blockChanges.Add(true);
        blockCount++;
        fixedSleepBlock.AddBottomExtension(shape.GetColor()); // Add an extension to the block to indicate hierarchy

        // Configure end of loop block
        endLoopBlock.SetDraggable(false);
        endLoopBlock.SetSplitable(false);
        endLoopBlock.AddBottomExtension(shape.GetColor());
        endLoopBlock.transform.SetAsLastSibling();
        endLoopBlock.SetColor(shape.GetColor());

        loopAttrsChanged = true;
        UpdateLoopNameText();
    }

    #endregion

    public void AddMessage(ActionMessage msg)
    {
        messages.Add(msg);
    }

    public void ClearMessages()
    {
        messages.Clear();
    }

    public List<ActionMessage> GetActionMessages()
    {
        // Adds a message if the loop attributes changed
        if (loopAttrsChanged)
        {
            // Create the message
            EditLoopMessage editMsg = new EditLoopMessage(loopId);
            editMsg.active = playing ? 1 : 0;
            if (syncedWith != "")
                editMsg.syncedWith = LoopManager.instance.GetLoopIDFromName(syncedWith); 
            editMsg.bpm = bpm;
            // Add the message
            messages.Add(editMsg);
            // Reset changed flag
            loopAttrsChanged = false;
        }

        // Adds a message for each changed block
        for (int i = 0; i < blockChanges.Count; i++)
        {
            if (blockChanges[i])
            {
                messages.Add(blocks[i].GetBlockAttributes().GetActionMessage());
                blockChanges[i] = false;
            }
        }

        // Set updated blocks list
        updatedBlocks.Clear();
        foreach (var block in blocks)
            updatedBlocks.Add(block);

        if (messages.Count <= 0)
            return null;

        //Debug.Log("Sending " + messages.Count + " messages.");

        return messages;
    }

    public void SetChangedBlock(int index)
    {
        blockChanges[index] = true;
    }

    public void SetChangedLoopBlocks(bool changed)
    {
        for (int i = 0; i < blockChanges.Count; i++)
            blockChanges[i] = changed;
    }

    public void SetChangedLoop(bool changed)
    {
        loopAttrsChanged = changed;
    }

    public void SetLoopId(int id)
    {
        loopId = id;
    }

    public int GetLoopId()
    {
        return loopId;
    }

    public int GetBlockCount()
    {
        return blockCount;
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

    public void DeattachBlock(int index)
    {
        blocks.RemoveAt(index);
        blockCount--;
    }

    public void AttachBlock(BlockShape block, int index)
    {
        block.gameObject.transform.SetParent(transform);
        block.gameObject.transform.SetSiblingIndex(index);
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
        endLoopBlock.transform.SetAsLastSibling();

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
    }

    public void RemoveBlockAt(int index)
    {
        // Create an empty block message
        ActionMessage msg = new ActionMessage();
        msg.loopId = loopId;
        msg.blockId = blocks.Count - 1;
        msg.actionName = "empty";
        AddMessage(msg);
        blockChanges[msg.blockId] = true;

        // Removes the block
        blocks.RemoveAt(index);
        blockChanges.RemoveAt(index);

        // Update the blocks behind it
        for (int i = index; i < blocks.Count; i++)
        {
            BlockAttributes battr = blocks[i].GetBlockAttributes();
            battr.SetId(battr.GetBlockId() - 1);
            blockChanges[i] = true;
        }
        // Update block count
        blockCount--;
    }

    // Add empty messages for blocks in the range [beg, end)
    public void AddEmptyMessagesAtRange(int beg, int end)
    {
        for (int i = beg; i < end; i++)
        {
            // Create an empty block message
            ActionMessage msg = new ActionMessage();
            msg.loopId = loopId;
            msg.blockId = i;
            msg.actionName = "empty";
            AddMessage(msg);
        }
    }

    // Clear this loop's block list
    public void ClearBlocks()
    {
        blocks.Clear();
    }

    // Force the block layout to refresh and update the order of its children
    IEnumerator RefreshBlockLayout(BlockShape block, int index)
    {
        // Waits for next frame
        yield return null;

        // Update the moved block sibling index
        block.transform.SetSiblingIndex(index + 1);
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

        block.transform.SetParent(loopContainerGO.transform);

        // Sort to change blocks list index
        blocks.Sort(bComparer);

        foreach (var bl in blocks)
            Debug.Log(bl.name + " " + bl.transform.GetSiblingIndex());

        StartCoroutine(RefreshBlockLayout(block, newIndex));

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
            blockChanges[i] = true;
        }

        // Update the moving block
        block.SetId(newId);
        blockChanges[preId] = true;
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
            blockChanges[i] = true;
        }

        // Update the moving block
        block.SetId(newId);
        blockChanges[preId] = true;
    }

    // Asks the loop manager to remove this loop and stop playing it
    public void DeleteLoop()
    {
        // Remove from loop management
        LoopManager.instance.RemoveLoop(loopId);

        // Destroy objects
        foreach (var block in blocks)
            Destroy(block.gameObject);
        Destroy(loopContainerGO);
        Destroy(gameObject);
    }

    // Toggles the loop playing state
    public void TogglePlaying()
    {
        playing = !playing;
        loopAttrsChanged = true;
    }

    // Sets the playing state of the loop
    public void SetPlaying(bool value)
    {
        playing = value;
        loopAttrsChanged = true;
    }

    // Change the loop's bpm and adds a message with the change
    public void SetSync(string value)
    {
        syncedWith = value;
        syncDropdown.SetSelection(value);
        loopAttrsChanged = true;
    }

    // Returns the name of loop this loop is syncing with
    public string GetSync()
    {
        return syncedWith;
    }

    // Reset the syncing of this loop
    public void Desync()
    {
        SetSync("");
        syncDropdown.ResetSelection();
        Debug.Log("DESYNC!");
    }

    // Change the loop's bpm and adds a message with the change
    public void SetBPM(int value)
    {
        bpm = value;
        loopAttrsChanged = true;
    }

    public void UpdateLoopNameText()
    {
        if (loopCustomName == "")
            nameText.text = "Loop " + (loopId + 1).ToString();
    }

    public string GetName()
    {
        if (loopCustomName == "")
            return nameText.text;
        else
            return loopCustomName;
    }

    public void SetName(string newName)
    {
        string prevName = GetName();
        loopCustomName = newName; // Set custom name
        if (newName != "")
            LoopManager.instance.UpdateLoopNameInDict(prevName, newName, loopId); // Update the name in the loop manager dictionary
        else
        {
            LoopManager.instance.UpdateLoopNameInDict(prevName, "Loop " + (loopId + 1).ToString(), loopId); // Update the name in the loop manager dictionary
            UpdateLoopNameText();
        }
    }

    public void SetSyncingOptions(List<string> options)
    {
        options.Remove(GetName());
        syncDropdown.SetSyncingOptions(options);
    }

    // Update the indicator state of the blocks
    public void AdvancePlayingBlock(int comId)
    {
        if (playingBlockId < updatedBlocks.Count)
        {
            // Hide previous playing block indicator
            BlockShape prevPlayingBlockShape = updatedBlocks[playingBlockId];
            if (prevPlayingBlockShape) prevPlayingBlockShape.GetBlockIndicator().HideIndicator();
        }
        playingBlockId = comId;
        // Show current playing block indicator
        if (playingBlockId < updatedBlocks.Count)
        {
            BlockShape currentPlayingBlockShape = updatedBlocks[playingBlockId];
            if (currentPlayingBlockShape) currentPlayingBlockShape.GetBlockIndicator().ShowIndicator();
        }
    }

}
