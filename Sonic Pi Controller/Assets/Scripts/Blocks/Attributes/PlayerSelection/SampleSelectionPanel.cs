using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSelectionPanel : MonoBehaviour
{
    List<List<string>> samples; // List of lists of sample names

    List<GameObject> categoryButtons; // category buttons
    List<List<GameObject>> sampleButtons; // sample select buttons

    [SerializeField] SampleCategoryButton categoryButtonPF;
    [SerializeField] PlayerSelectButton sampleButtonPF;

    [SerializeField] GameObject buttonContainer;
    [SerializeField] GameObject backButton;

    PlayerBlockAttributes blockAttributes;

    int selectedCategoryIndex = -1;

    public void Configure(PlayerBlockAttributes blockAttributes)
    {
        this.blockAttributes = blockAttributes;
        // Ask for the names of the samples and categories
        samples = SonicPiManager.instance.GetSampleNames();

        // Create the buttons
        categoryButtons = new List<GameObject>();
        sampleButtons = new List<List<GameObject>>();
        CreateButtons();
    }

    void CreateButtons()
    {
        int categoryIndex = 0;
        foreach (List<string> category in samples)
        {
            // Create category button
            categoryButtons.Add(CreateCategoryButton(categoryIndex, category[0]));
            categoryIndex++;
            category.RemoveAt(0); // Remove the first name, which corresponds to the name of the category

            // Create sample buttons
            List<GameObject> auxButtonList = new List<GameObject>();
            foreach (string sampleName in category)
                auxButtonList.Add(CreateSampleButton(sampleName));
            sampleButtons.Add(auxButtonList);
        }
    }

    GameObject CreateCategoryButton(int index, string categoryName)
    {
        // Instantiate category button
        SampleCategoryButton categoryButton = Instantiate(categoryButtonPF, buttonContainer.transform);

        // Configure category button
        categoryButton.Configure(this, index, categoryName);

        categoryButton.gameObject.SetActive(false);
        return categoryButton.gameObject;
    }

    GameObject CreateSampleButton(string sampleName)
    {
        // Instantiate sample button
        PlayerSelectButton sampleButton = Instantiate(sampleButtonPF, buttonContainer.transform);

        // Configure sample button
        sampleButton.Configure(this, blockAttributes, sampleName);

        sampleButton.gameObject.SetActive(false);
        return sampleButton.gameObject;
    }

    public void CategorySelect(int index)
    {
        selectedCategoryIndex = index;

        // Hide category buttons
        foreach (GameObject categoryButton in categoryButtons)
            categoryButton.SetActive(false);

        // Show category's sample buttons
        foreach (GameObject sampleButton in sampleButtons[index])
            sampleButton.SetActive(true);

        backButton.SetActive(true);
    }

    public void ShowCategoryButtons()
    {
        // Hide sample buttons
        if (selectedCategoryIndex != -1)
            foreach (GameObject sampleButton in sampleButtons[selectedCategoryIndex])
                sampleButton.SetActive(false);

        foreach (GameObject categoryButton in categoryButtons)
            categoryButton.SetActive(true);
    }

    public void CloseSampleMenu()
    {
        if (selectedCategoryIndex < 0) return;
        foreach (GameObject sampleButton in sampleButtons[selectedCategoryIndex])
            sampleButton.SetActive(false);

        selectedCategoryIndex = -1;
    }
}
