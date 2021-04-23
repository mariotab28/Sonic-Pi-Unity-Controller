using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBlockAttributes : BlockAttributes
{
    [SerializeField] PlayerSelectionPanel playerSelectionPanelPF; // TODO: Generalize player selection
    private PlayerSelectionPanel playerSelectionPanel;

    [SerializeField] TMP_Text playerNameText;

    protected override void Start()
    {
        base.Start();
        //if(playerSelectionPanel) playerSelectionPanel.Configure(this);
        if (playerNameText) playerNameText.text = (msg as PlayerMessage).playerName; // Update player name text
    }

    public void SetPlayer(string playerName)
    {
        if (action != "synth" && action != "sample")
            return;
        (msg as PlayerMessage).playerName = playerName; // Set player name
        playerNameText.text = playerName; // Update player name text
        HidePanel();

        loop.SetChangedBlock(id);
    }

    public void ShowPanel()
    {
        if (!playerSelectionPanelPF) return;
        // Create the panel if does not exist yet
        if (!playerSelectionPanel)
        {
            playerSelectionPanel = Instantiate(playerSelectionPanelPF, LoopManager.instance.canvas.transform);
            playerSelectionPanel.Configure(this);
        }
        else if (playerSelectionPanel.gameObject.activeSelf)
        {
            // Hide the panel if it is already active
            HidePanel();
            return;
        }
        
        playerSelectionPanel.Show();
    }

    public void HidePanel()
    {
        if (!playerSelectionPanel) return;
        playerSelectionPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        //base.OnDestroy(); // TODO: Destroy attributes panel
        if (playerSelectionPanel) Destroy(playerSelectionPanel.gameObject); // Destroy player selection panel
    }
}
