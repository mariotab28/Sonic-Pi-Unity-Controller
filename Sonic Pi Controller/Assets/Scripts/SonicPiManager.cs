using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Usar enum?
public enum PiAction {  Play, Sleep }

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
    public void sendActionMessage(string action)
    {
        OSCHandler.Instance.SendMessageToClient("SonicPi", "/sonicpi/unity/trigger", action);
       
    }
    #endregion
}
