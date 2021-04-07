using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectButton : MonoBehaviour
{

    BlockAttributes bAttr;
    string playerName;

    public void Configure(BlockAttributes bAttr, string playerName)
    {
        this.bAttr = bAttr;
        this.playerName = playerName;
    }

    public void Click()
    {
        bAttr.SetPlayer(playerName);
    }
}
