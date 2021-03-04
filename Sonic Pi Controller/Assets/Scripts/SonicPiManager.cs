using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Duda: uso de class para valores predeterminados
// Sería mejor un struct? (no se pueden poner valores por defecto)
public class ActionMessage
{
    public int loopId = 0;
    public int blockId = 0;
    public string actionName = "";
    public string synthName = "beep";
    public string fxName = "echo";
    public string sampleName = "bd_pure";
    public int sleepDur = 1;
    public int note = 52;
    public float amp = 1;
    public float pan = 0;

    public List<object> ToObjectList()
    {
        List<object> list = new List<object>();
        list.Add(loopId);
        list.Add(blockId);
        list.Add(actionName);
        list.Add(synthName);
        list.Add(fxName);
        list.Add(sampleName);
        list.Add(sleepDur);
        list.Add(note);
        list.Add(amp);
        list.Add(pan);
        return list;
    }
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
