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
            DontDestroyOnLoad(gameObject);
        }
    }


    #endregion

    int loopCount = 0;

    List<LoopBlock> loops;
    [SerializeField] GameObject loopContainerGO;
    [SerializeField] LoopBlock initialLoopGO;
    [SerializeField] GameObject loopPF;

    public Canvas canvas;
    [SerializeField] GameObject destroyZone;
    [SerializeField] GameObject addLoopZone;

    RectTransform containerRect;

    private void Start()
    {
        containerRect = loopContainerGO.GetComponent<RectTransform>();
        if (!containerRect) Debug.LogError("Error: Loop container's rect not found");

        // Add an empty loop on Start
        initialLoopGO = AddLoop();
    }

    /**********************************/
    //  LOOP METHODS
    /**********************************/

    /// <summary>
    /// Tell each active loop to send its messages
    /// </summary>
    public void RunLoops()
    {
        foreach (LoopBlock loop in loops)
            loop.SendActionMessages();
    }

    /// <summary>
    /// Creates a new loop and adds it to the loop manager
    /// </summary>
    public LoopBlock AddLoop()
    {
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

        loopCount++;

        // Enlarge container
        RectTransform loopRect = newLoopParent.GetComponent<RectTransform>();
        if (!loopRect) Debug.LogError("Error: Cannot find rect component in loop object");
        Vector2 newSize = new Vector2(containerRect.sizeDelta.x, containerRect.sizeDelta.y + (!loopRect ? 0 : loopRect.sizeDelta.y + 200));
        loopContainerGO.GetComponent<RectTransform>().sizeDelta = newSize;

        return newLoop;
    }

    /**********************************/
    //  BLOCK METHODS
    /**********************************/
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
