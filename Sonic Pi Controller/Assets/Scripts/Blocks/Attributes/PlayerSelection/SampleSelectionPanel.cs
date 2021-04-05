using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSelectionPanel : MonoBehaviour
{
    List<List<string>> samples; // List of lists of sample names

    List<GameObject> categoryButtons; // category buttons
    List<List<GameObject>> sampleButtons; // sample select buttons

    SampleCategoryButton categoryButtonPF;
    PlayerSelectButton sampleButtonPF;

    void Start()
    {
        // Ask for the names of the samples and categories
        samples = SonicPiManager.instance.GetSampleNames();

        // Create the buttons
        categoryButtons = new List<GameObject>();
        sampleButtons = new List<List<GameObject>>();
        CreateButtons();
    }

    void CreateButtons()
    {
        foreach (List<string> category in samples)
        {
            // Create category button

            // Create sample button

        }
    }

    void CreateCategoryButton(string categoryName)
    {
        // Instantiate category button

        // Configure category button

    }

    void CreateSampleButton(string categoryName)
    {
        // Instantiate sample button

        // Configure sample button

    }

    public void CategorySelect(int index)
    { 
        // Show category's sample buttons
    }
}
