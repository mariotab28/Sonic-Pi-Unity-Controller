using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockShape : MonoBehaviour
{
    #region Variables

    public Color color = Color.white;

    public bool hasGap = false;
    public Sprite fullBodySprite;
    public Sprite gapBodySprite;

    public Image edgeGO;
    public BottomExtensionManager mainBlock;
    
    public BottomExtensionManager blockLayoutPF;

    // Components:
    public Image bodyImage;
    RectTransform rectTransform;

    // List of parent block colors
    List<Color> parentBlockColors = new List<Color>();

    // List of blockLayouts attached to this block
    List<BottomExtensionManager> attachedBlocks = new List<BottomExtensionManager>();

    // Block Attributes
    BlockAttributes blockAttributes;

    #endregion

    #region Initialization

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        blockAttributes = GetComponent<BlockAttributes>();

        if (fullBodySprite == null || gapBodySprite == null)
        {
            Debug.LogError("Block sprites not assigned!");
            return;
        }

        attachedBlocks.Add(mainBlock);

        // Set body sprite
        bodyImage.sprite = (hasGap ? gapBodySprite : fullBodySprite);

        // Set color
        SetColor(color);
    }
    #endregion

    #region Shape Config Methods

    // Sets the block sprite to the sprite with the gap
    public void SetGap(bool gap)
    {
        this.hasGap = gap;
        bodyImage.sprite = (gap ? gapBodySprite : fullBodySprite);
    }

    // Activates the edge at the right of the block
    public void SetEdge(bool edge)
    {
        edgeGO.gameObject.SetActive(edge);
    }

    // Adds another block object to extend the current block
    public void AddExtension()
    {
        // Instantiate the block layout and sets the color
        BottomExtensionManager b = Instantiate(blockLayoutPF, transform);
        // change color

        // Move the edge to the end
        edgeGO.transform.SetAsLastSibling();

        // Add block layout to the list
        attachedBlocks.Add(b);

        // Extend container width
        Vector2 newSize = new Vector2(rectTransform.sizeDelta.x + 300, rectTransform.sizeDelta.y);
        rectTransform.sizeDelta = newSize;

        // Set parent blocks
        b.UpdateBottomExtensions(mainBlock.GetBottomColors());
    }

    // Adds an extension at the bottom of the block with a color
    // representative of the parent block
    public void AddBottomExtension(Color bottomColor)
    {
        foreach (BottomExtensionManager block in attachedBlocks)
        {
            block.AddBottomExtension(bottomColor);
        }
    }

    public void Test()
    {
        AddBottomExtension(new Color(Random.Range(0.4f,1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f)));
    }

    // Sets the color of the elements of the block
    public void SetColor(Color color)
    {
        this.color = color;
        bodyImage.color = color;
        edgeGO.color = color;
    }
    #endregion

    #region Logic Methods

    public void SetLoop(LoopBlock loop)
    {
        blockAttributes.SetLoop(loop);
    }

    public LoopBlock GetLoop()
    {
        return blockAttributes.GetLoop();
    }

    public BlockAttributes GetBlockAttributes()
    {
        return blockAttributes;
    }
    #endregion
}
