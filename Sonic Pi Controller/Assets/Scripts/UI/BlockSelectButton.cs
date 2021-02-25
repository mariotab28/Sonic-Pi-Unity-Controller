using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockSelectButton : MonoBehaviour
{
    #region Variables
    public string blockActionName;

    public GameObject selectionMenuGO;

    public GameObject blockContainerGO;

    public GameObject blockPF;

    #endregion

    /// <summary>
    /// ...
    /// </summary>
    public void SelectActionBlock()
    {
        // Sends the action message
        SonicPiManager.Instance.sendActionMessage(blockActionName);

        // Instantiate UI action block
        GameObject block = Instantiate(blockPF, blockContainerGO.transform);
        // TODO: Script para configuración de los bloques
        block.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = blockActionName;

        // Hide selection menu 
        selectionMenuGO.SetActive(false);
    }
}
