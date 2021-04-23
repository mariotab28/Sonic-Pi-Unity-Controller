using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelectButton : MonoBehaviour
{

    PlayerBlockAttributes bAttr;
    string playerName;
    [SerializeField] TMPro.TMP_Text nameText;

    public void Configure(PlayerSelectionPanel panel, PlayerBlockAttributes bAttr, string playerName)
    {
        this.bAttr = bAttr;
        this.playerName = playerName;
        nameText.text = playerName;
    }

    public void Click()
    {
        bAttr.SetPlayer(playerName);
    }
}
