using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Cambiar las clases a ActionAttr, etc. y hacer una clase ActionMessage que tenga: numMsgs y List<ActionAttr> y un método para pasarlo todo a una List<object>

/// <summary>
/// MESSAGE VIRTUAL CLASS
/// </summary>  
public class ActionMessage
{
    public int loopId = 0;
    public int blockId = 0;
    public string actionName = "";

    public virtual List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        return list;
    }
}

public class EditMessage : ActionMessage
{
    public string attributeName = "";
    public float newValue = 0.0f;

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(attributeName);
        list.Add(newValue);
        return list;
    }
}

/// <summary>
/// SLEEP MESSAGE CLASS
/// </summary>
public class SleepMessage : ActionMessage
{
    public float sleepDuration = 1.0f;

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(sleepDuration);
        return list;
    }
}

/// <summary>
/// PLAYER MESSAGE PARENT CLASS
/// </summary>
public class PlayerMessage : ActionMessage
{
    public string playerName = "";
    public float amp = 1;
    public float pan = 0;
    public float attack = 0;
    public float sustain = 0;
    public float release = 1;
    public float decay = 0;
    public float attack_level = 1;
    public float sustain_level = 1;
    public float decay_level = 1;
    public string fx = "";

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(playerName);
        list.Add(amp);
        list.Add(pan);
        list.Add(attack);
        list.Add(sustain);
        list.Add(release);
        list.Add(decay);
        list.Add(attack_level);
        list.Add(sustain_level);
        list.Add(decay_level);
        list.Add(fx);
        return list;
    }
}

/// <summary>
/// SYNTH MESSAGE CLASS
/// </summary>
public class SynthMessage : PlayerMessage
{
    public int numOfNotes = 1;
    public List<int> notes = new List<int>(new int[] { 52 });
    public string mode = "tick";

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(playerName);
        list.Add(numOfNotes);
        foreach (int note in notes)
            list.Add(note);
        list.Add(mode);
        list.Add(amp);
        list.Add(pan);
        list.Add(attack);
        list.Add(sustain);
        list.Add(release);
        list.Add(decay);
        list.Add(attack_level);
        list.Add(sustain_level);
        list.Add(decay_level);
        list.Add(fx);
        return list;
    }
}

/// <summary>
/// SAMPLE MESSAGE CLASS
/// </summary>
public class SampleMessage : PlayerMessage
{
    // TODO: Atributos específicos
}

public class SonicPiManager : MonoBehaviour
{
    #region Singleton Constructors
    static SonicPiManager()
    {
    }

    SonicPiManager()
    {
    }

    public static SonicPiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("SonicPiManager").AddComponent<SonicPiManager>();
            }

            return _instance;
        }
    }

    #endregion

    #region Member Variables
    private static SonicPiManager _instance = null;
    #endregion

    #region Methods
    /// <summary>
    /// Initialize OSCHandler
    /// </summary>
    void Start()
    {
        print("Sonic Pi Manager started.");
        OSCHandler.Instance.Init();
    }

    /// <summary>
    /// Sends a message to Sonic Pi containing
    /// the action's name 
    /// </summary>
    public void sendActionMessage(ActionMessage msg)
    {
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", msg);

    }
    #endregion
}
