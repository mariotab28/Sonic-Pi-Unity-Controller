using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionPanel : MonoBehaviour
{
    List<string> players; // List of player names
    List<GameObject> playerButtons; // player select buttons

    [SerializeField] protected PlayerSelectButton playerButtonPF;
    [SerializeField] protected GameObject buttonContainer;

    protected PlayerBlockAttributes blockAttributes;

    public virtual void Configure(PlayerBlockAttributes blockAttributes)
    {
        this.blockAttributes = blockAttributes;
        // Ask for the names of the samples and categories
        players = SonicPiManager.instance.GetInstrumentNames();

        // Create the buttons
        playerButtons = new List<GameObject>();
        CreateButtons();
    }

    void CreateButtons()
    {
        // Create sample buttons
        foreach (string name in players)
            playerButtons.Add(CreatePlayerButton(name));
    }

    protected GameObject CreatePlayerButton(string sampleName)
    {
        // Instantiate sample button
        PlayerSelectButton playerButton = Instantiate(playerButtonPF, buttonContainer.transform);

        // Configure sample button
        playerButton.Configure(this, blockAttributes, sampleName);

        playerButton.gameObject.SetActive(false);
        return playerButton.gameObject;
    }

    public virtual void HideButtons()
    {
        foreach (GameObject playerButton in playerButtons)
            playerButton.SetActive(false);
    }

    public virtual void ShowButtons()
    {
        foreach (GameObject playerButton in playerButtons)
            playerButton.SetActive(true);
    }

    public  void Show()
    {
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        ShowButtons();
    }
}
