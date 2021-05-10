using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    #region Singleton Constructors
    public static LoopManager instance;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            instance = this;
            loops = new List<LoopBlock>();
            emptyLoops = new Queue<EmptyLoop>();
            DontDestroyOnLoad(gameObject);
        }
    }


    #endregion

    struct EmptyLoop
    {
        public int loopId;
        public int numBlocks;

        public EmptyLoop(int loopId, int numBlocks)
        {
            this.loopId = loopId;
            this.numBlocks = numBlocks;
        }
    }

    int loopCount = 0;

    List<LoopBlock> loops;
    Queue<EmptyLoop> emptyLoops;
    [SerializeField] GameObject loopContainerGO;
    [SerializeField] GameObject loopPF;

    public Canvas canvas;
    [SerializeField] GameObject destroyZone;
    [SerializeField] GameObject addLoopZone;


    private void Start()
    {
        // Add an empty loop on Start
        AddLoop();
    }

    /**********************************/
    //  LOOP METHODS
    /**********************************/

    /// <summary>
    /// Tell each active loop to send its messages
    /// </summary>
    public void RunLoops()
    {
        //List<List<ActionMessage>> loopMsgs = new List<List<ActionMessage>>();
        List<object> msgValues = new List<object>();
        List<object> auxValuesList = new List<object>();
        int numberOfLoops = 0;

        foreach (LoopBlock loop in loops)
        {
            // Get message group from loop
            List<ActionMessage> msgGroup = loop.GetActionMessages();

            // Check if the msgGroup is empty
            if (msgGroup != null)
            {
                // Increase number of loop messages
                numberOfLoops++;
                // Add id of the loop
                msgValues.Add(loop.GetLoopId());
                int valuesCount = 0;

                foreach (ActionMessage message in msgGroup)
                {
                    List<object> valueList = message.ToObjectList();
                    // Increment values count
                    valuesCount += valueList.Count;
                    // Append to message group value list
                    auxValuesList.AddRange(valueList);
                }

                // Add number of values
                msgValues.Add(valuesCount);
                // Add number of commands
                msgValues.Add(msgGroup.Count);
                // Add all command values
                msgValues.AddRange(auxValuesList);

                // Clear auxiliar value list
                auxValuesList.Clear();
                // Clear loop messages
                loop.ClearMessages();
            }
        }

        // Add empty loops
        while(emptyLoops.Count > 0)
        {
            EmptyLoop empty = emptyLoops.Dequeue();
            // Create list of empty messages
            List<ActionMessage> msgGroup = new List<ActionMessage>();
            for (int i = 0; i < empty.numBlocks; i++)
            {
                // Create an empty block message
                ActionMessage msg = new ActionMessage();
                msg.loopId = empty.loopId;
                msg.blockId = i;
                msg.actionName = "empty";
                msgGroup.Add(msg);
            }

            // Increase number of loop messages
            numberOfLoops++;
            // Add id of the loop
            msgValues.Add(empty.loopId);
            int valuesCount = 0;

            foreach (ActionMessage message in msgGroup)
            {
                List<object> valueList = message.ToObjectList();
                // Increment values count
                valuesCount += valueList.Count;
                // Append to message group value list
                auxValuesList.AddRange(valueList);
            }

            // Add number of values
            msgValues.Add(valuesCount);
            // Add number of commands
            msgValues.Add(msgGroup.Count);
            // Add all command values
            msgValues.AddRange(auxValuesList);

            // Clear auxiliar value list
            auxValuesList.Clear();
        }

        // Add number of loops at the start
        msgValues.Insert(0, numberOfLoops);

        // Send message to Sonic Pi
        SonicPiManager.instance.SendActionMessageGroup(msgValues);
    }

    /// <summary>
    /// Creates a new loop and adds it to the loop manager
    /// </summary>
    public LoopBlock AddLoop()
    {
        // Check loop number limit
        if (loopCount >= SonicPiManager.instance.GetNumberOfLoops()) return null;

        // Creates the loop Object
        GameObject newLoopParent = Instantiate(loopPF, loopContainerGO.transform);

        // Initialize the loop
        LoopBlock newLoop = newLoopParent.GetComponentInChildren<LoopBlock>();
        if (!newLoop)
        {
            Debug.LogError("Error: Loop component not found in new loop object.");
            Destroy(newLoopParent);
            return null;
        }
        newLoop.Init(loopCount);

        // Add loop to list of loops
        loops.Add(newLoop);

        // Send message to sonic pi asking for loop creation
        //SonicPiManager.instance.SendNewLoopMessage();

        loopCount++;

        return newLoop;
    }

    public void RemoveLoop(int loopId)
    {
        int lastLoopId = loopCount - 1;
        // For each loop from loopId + 1 onwards:
        // a) decrease its id
        // b) fill extra blocks with empty commands
        for (int i = loopId + 1; i < loops.Count; i++)
        {
            loops[i].SetLoopId(i - 1); // Decrease loop index
            loops[i].SetChangedLoopBlocks(true); // Set it to changed state
            loops[i].SetChangedLoop(true); 

            // If there were more blocks in the previous loop,
            // empty messages must be added to overwrite these blocks in Sonic Pi with empty commands
            int nBlocksA = loops[i - 1].GetBlockCount();
            int nBlocksB = loops[i].GetBlockCount();
            int dif = nBlocksA - nBlocksB;
            if (dif > 0)
                loops[i].AddEmptyMessagesAtRange(nBlocksB, nBlocksB + dif);
        }

        // Erase loops[loopId]
        int blockCount = loops[loopId].GetBlockCount();
        loops.RemoveAt(loopId);
        loopCount--;

        // Last loop must become empty in Sonic Pi:
        // Create an empty loop to remove commands from Sonic Pi's last loop
        EmptyLoop empty = new EmptyLoop(lastLoopId, blockCount);
        emptyLoops.Enqueue(empty);
    }

    /**********************************/
    //  BLOCK METHODS
    /**********************************/
    public void RemoveBlockFromLoop(int loopId, int blockId)
    {
        loops[loopId].RemoveBlockAt(blockId);
    }
    /* //TODO: Use this?
    public void EditBlockAttribute(int loopId, int blockId, string attribute, float value)
    {
        ActionMessage msg = new EditMessage();
        msg.loopId = loopId;
        msg.blockId = blockId;
        msg.actionName = "edit";
        (msg as EditMessage).attributeName = attribute;
        (msg as EditMessage).newValue = value;
        loops[loopId].AddMessage(msg);
    }*/

    public void AddBlockToLoop(int loopId, int blockId, string action)
    {
        // Ask the loop to add the block
        loops[loopId].AddBlock(action, blockId);
    }

    // Change the position of a block inside its loop
    public void ChangeBlockPosition(int loopId, BlockShape block, int newBlockId)
    {
        loops[loopId].ChangeBlockPosition(block, newBlockId);
    }

    public void AddMessage(int loopId, ActionMessage message)
    {
        loops[loopId].AddMessage(message);
    }

    public void SetDestroyZone(bool active)
    {
        destroyZone.SetActive(active);
    }

    public void SetAddLoopZone(bool active)
    {
        addLoopZone.SetActive(active);
    }
}
