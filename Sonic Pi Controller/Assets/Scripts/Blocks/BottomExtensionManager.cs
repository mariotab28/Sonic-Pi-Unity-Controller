using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomExtensionManager : MonoBehaviour
{
    public Image bottomPF;
    // List of bottom extension blocks
    List<Image> bottomExtensions = new List<Image>();
    public float widthOffset = 15;

    // Adds an extension at the bottom of the block with a color
    // representative of the parent block
    public void AddBottomExtension(Color bottomColor)
    {
        Image ext = Instantiate(bottomPF, transform);
        ext.color = bottomColor;
        RectTransform rectTransform = ext.rectTransform;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + widthOffset, rectTransform.sizeDelta.y);
        bottomExtensions.Add(ext);
    }

    // Adds one bottom extension for each parent block
    public void UpdateBottomExtensions(List<Color> parentColors)
    {
        // Reset bottom extensions
        foreach (Image ext in bottomExtensions)
            Destroy(ext.gameObject);

        // Add a Bottom Block
        foreach (Color parentColor in parentColors)
            AddBottomExtension(parentColor);
    }

    public List<Color> GetBottomColors()
    {
        List<Color> colors = new List<Color>();

        foreach (Image image in bottomExtensions)
            colors.Add(image.color);

        return colors;
    }
}
