using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum BlockType
{
    LOOP, SYNTH, SAMPLE, SLEEP
}


#region Message Classes

/// <summary>
/// Virtual class for the action messages
/// </summary>
public class ActionMessage
{
    public int loopId = 0;
    public int blockId = 0;
    public string actionName = "";
    public Dictionary<string, float> attrs;

    public virtual List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        //list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        return list;
    }
}

/*
 * TODO: Use EditMessage??
 *
public class EditMessage : ActionMessage
{
    public string attributeName = "";
    public float newValue = 0.0f;

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        //list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(attributeName);
        list.Add(newValue);
        return list;
    }
}
*/

public class EditLoopMessage : ActionMessage
{
    public int active = 1; // 0 = Not active | 1 = Active
    public string syncedWith = "";
    public int bpm = 60;

    public EditLoopMessage(int loopId)
    {
        actionName = "loop";
        blockId = -1;
        this.loopId = loopId;
    }

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();

        list.Add(blockId);
        list.Add(actionName);
        list.Add(active);
        list.Add(syncedWith);
        list.Add(bpm);
        return list;
    }
}

/// <summary>
/// SLEEP MESSAGE CLASS
/// </summary>
public class SleepMessage : ActionMessage
{
    public SleepMessage()
    {
        actionName = "sleep";
    }

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();

        list.Add(blockId);
        list.Add(actionName);
        list.Add(attrs["duration"]);
        return list;
    }
}

/// <summary>
/// PLAYER MESSAGE PARENT CLASS
/// </summary>
public class PlayerMessage : ActionMessage
{
    public string playerName = "beep";
    public string fx = "";

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();

        list.Add(blockId);
        list.Add(actionName);
        list.Add(playerName);
        foreach (var attribute in attrs.Values)
            list.Add(attribute);
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
    public List<int> notes = new List<int>(new int[] { });
    public string mode = "tick";

    public SynthMessage()
    {
        actionName = "synth";
    }

    public override List<object> ToObjectList()
    {
        List<object> list = new List<object>();

        list.Add(blockId);
        list.Add(actionName);
        list.Add(playerName);
        list.Add(numOfNotes);
        foreach (int note in notes)
            list.Add(note);
        list.Add(mode);
        foreach (var attribute in attrs.Values)
            list.Add(attribute);
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
    public SampleMessage()
    {
        actionName = "sample";
        playerName = "drum_heavy_kick";
    }
}


#endregion

public class SonicPiManager : MonoBehaviour
{
    #region Singleton Variables

    public static SonicPiManager instance = null;


    #endregion

    #region Member Variables

    // Size of the rack of loops in SonicPi
    [SerializeField] int numberOfLoops = 10;

    // Block attribute dictionaries
    [SerializeField]
    TextAsset synthAttributes;
    [SerializeField]
    TextAsset sampleAttributes;
    [SerializeField]
    TextAsset sleepAttributes;
    [SerializeField]
    TextAsset loopAttributes;

    Dictionary<string, float> synthDictionary;
    Dictionary<string, float> sampleDictionary;
    Dictionary<string, float> sleepDictionary;
    Dictionary<string, float> loopDictionary;

    // Lists of samples and synth types (instruments)
    [SerializeField] TextAsset sampleNamesFile;
    List<List<string>> sampleNames;
    [SerializeField] TextAsset instrumentNamesFile;
    List<string> instrumentNames;

    #endregion

    #region Methods

    /// <summary>
    /// Initialize OSCHandler
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Another sonic pi manager found!");
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        print("Sonic Pi Manager started.");
        OSCHandler.Instance.Init();

        // Initialize attribute dictionaries
        synthDictionary = JsonConvert.DeserializeObject<Dictionary<string, float>>(synthAttributes.text);
        sampleDictionary = JsonConvert.DeserializeObject<Dictionary<string, float>>(sampleAttributes.text);
        sleepDictionary = JsonConvert.DeserializeObject<Dictionary<string, float>>(sleepAttributes.text);
        loopDictionary = JsonConvert.DeserializeObject<Dictionary<string, float>>(loopAttributes.text);

        // Initialize list of samples and instruments
        sampleNames = JsonConvert.DeserializeObject<List<List<string>>>(sampleNamesFile.text);
        instrumentNames = JsonConvert.DeserializeObject<List<string>>(instrumentNamesFile.text);

        // Initialize Sonic Pi loop listening/processing
        instance.SendInitMessage(numberOfLoops);

        DontDestroyOnLoad(gameObject);
    }

    public static Dictionary<TKey, TValue> GetDictionaryClone<TKey, TValue>(Dictionary<TKey, TValue> original)
    {
        Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                original.Comparer);
        foreach (KeyValuePair<TKey, TValue> entry in original)
            ret.Add(entry.Key, entry.Value);

        return ret;
    }

    public Dictionary<string, float> GetActionDictionary(string action)
    {
        switch (action)
        {
            case "synth":
                return GetDictionaryClone(synthDictionary);
            case "sample":
                return GetDictionaryClone(sampleDictionary);
            case "sleep":
                return GetDictionaryClone(sleepDictionary);
            case "loop":
                return GetDictionaryClone(loopDictionary);
            default:
                return null;
        }
    }

    public int GetNumberOfLoops()
    {
        return numberOfLoops;
    }

    /// <summary>
    /// Creates and initializes a new action message of a specific type
    /// </summary>
    /// <param name="action">
    /// The type of the block which determines the message attributes
    /// </param>
    /// <returns>
    /// An ActionMessage with the default attributes of the block's type
    /// </returns>
    public ActionMessage GetMessageTemplate(string action)
    {
        ActionMessage msg = null;

        switch (action)
        {
            case "synth":
                msg = new SynthMessage();
                msg.attrs = GetDictionaryClone(synthDictionary);
                break;
            case "sample":
                msg = new SampleMessage();
                msg.attrs = GetDictionaryClone(sampleDictionary);
                break;
            case "sleep":
                msg = new SleepMessage();
                msg.attrs = GetDictionaryClone(sleepDictionary);
                break;
            default:
                break;
        }

        return msg;
    }

    public List<List<string>> GetSampleNames()
    {
        return sampleNames;
    }

    public List<string> GetInstrumentNames()
    {
        return instrumentNames;
    }

    /// <summary>
    /// Sends a message to Sonic Pi containing
    /// </summary>
    public void SendActionMessage(ActionMessage msg)
    {
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", msg);
    }

    /// <summary>
    /// Sends a list of messages to Sonic Pi
    /// </summary>
    public void SendActionMessageGroup(List<object> msg)
    {
        //OSCHandler.Instance.SendMessageGroupToClient("SonicPi", "/sonicpi/unity/trigger", msgList);
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", msg);
    }
    public void SendInitMessage(int numLoops)
    {
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", new List<object> { "init", numLoops });
    }

    public void SendDeleteLoopMessage(int loopId)
    {
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", new List<object> { "del_loop", loopId });
    }
    #endregion
}
