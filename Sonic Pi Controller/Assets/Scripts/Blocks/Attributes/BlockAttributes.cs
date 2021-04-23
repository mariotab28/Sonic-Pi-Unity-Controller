using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAttributes : MonoBehaviour
{
    protected int id;
    public string action;
    
    // The loop containing this block
    [SerializeField] protected LoopBlock loop;

    // Message for the block
    //  - It contains the index of the block and its loop, the name of its action and a dictionary of attributes
    protected ActionMessage msg;


    #region Initialization
    

    protected virtual void Start()
    {
        // Creates the message object
        msg = SonicPiManager.instance.GetMessageTemplate(action);

        //TODO: De momento está hardcodeado para pruebas
        /*switch (action)
        {
            case "synth":
                msg.actionName = action;
                msg.blockId = id;
                (msg as SynthMessage).playerName = "piano";
                (msg as SynthMessage).mode = mode;
                (msg as SynthMessage).notes = notes;
                (msg as SynthMessage).numOfNotes = numOfNotes;
                (msg as SynthMessage).attrs["pan"] = 0;
                (msg as SynthMessage).fx = "echo";
                (msg as SynthMessage).attrs["release"] = 5;
                break;

            case "sample":
                msg.actionName = "sample";
                msg.blockId = id;
                (msg as PlayerMessage).playerName = "bd_haus";
                (msg as PlayerMessage).fx = "echo";
                (msg as PlayerMessage).attrs["pan"] = 0;
                break;

            case "sleep":
                msg.actionName = action;
                msg.blockId = id;
                (msg as SleepMessage).attrs["duration"] = 1;
                break;
            default:
                break;
        }*/
    }

    #endregion

    #region Methods
    public ActionMessage GetActionMessage()
    {
        if (msg == null)
            msg = SonicPiManager.instance.GetMessageTemplate(action);

        msg.loopId = loop.loopId;
        msg.blockId = id;
        // Returns the action message
        return msg;
    }

    public void SetAttribute(string attr, float value)
    {
        msg.attrs[attr] = value;
        loop.SetChangedBlock(id);
    }

    public void SetLoop(LoopBlock loop)
    {
        this.loop = loop;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public LoopBlock GetLoop()
    {
        return loop;
    }

    public int GetLoopId()
    {
        return loop.loopId;
    }

    public int GetBlockId()
    {
        return id;
    }

    #endregion

    
}
