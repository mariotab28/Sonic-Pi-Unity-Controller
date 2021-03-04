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

    public BlockController blockPF;

    //TODO: Cambiar de sitio
    int blocksCount = 0;

    #endregion

    /// <summary>
    /// ...
    /// </summary>
    public void SelectActionBlock()
    {

        // Instantiate UI action block
        BlockController block = Instantiate(blockPF, blockContainerGO.transform);
        // TODO: Script para configuración de los bloques
        //block.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = blockActionName;
        block.ConfigureBlock(blocksCount, blockActionName);
        blocksCount++;

        // Hide selection menu 
        selectionMenuGO.SetActive(false);
    }
}
