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
            loops.Add(initialLoopGO);
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    int loopCount = 1;

    List<LoopBlock> loops;
    public GameObject loopContainerGO;
    public LoopBlock initialLoopGO;
    public BlockController blockPF;

    public Canvas canvas;
    

    public void RunLoops()
    {
        foreach (LoopBlock loop in loops)
            loop.SendActionMessages();
    }

    public void RemoveBlockFromLoop(int loopId, int blockId)
    {
        ActionMessage msg = new ActionMessage();
        msg.loopId = loopId;
        msg.blockId = blockId;
        msg.actionName = "delete";
        loops[loopId].AddMessage(msg);
        loops[loopId].RemoveBlockAt(blockId);
    }

    public void EditBlockAttribute(int loopId, int blockId, string attribute, float value)
    {
        ActionMessage msg = new EditMessage();
        msg.loopId = loopId;
        msg.blockId = blockId;
        msg.actionName = "edit";
        (msg as EditMessage).attributeName = attribute;
        (msg as EditMessage).newValue = value;
        loops[loopId].AddMessage(msg);
    }

    public void AddBlockToLoop(int loopId, int blockId, string action)
    {
        // Ask the loop to add the block
        loops[loopId].AddBlock(action, blockId);
    }

    public void AddMessage(int loopId, ActionMessage message)
    {
        loops[loopId].AddMessage(message);
    }
}
