using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockComparer : IComparer<BlockShape>
{
    public int Compare(BlockShape a, BlockShape b)
    {
        if (a == null || a.GetBlockAttributes() == null)
        {
            if (b == null || b.GetBlockAttributes() == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (b == null || b.GetBlockAttributes() == null)
                return 1;
            else
            {
                if (a.GetBlockAttributes().GetBlockId() > b.GetBlockAttributes().GetBlockId())
                    return 1;
                else if (a.GetBlockAttributes().GetBlockId() < b.GetBlockAttributes().GetBlockId())
                    return -1;
                else
                    return 0;
            }
        }
    }
}

public class BlockShape : MonoBehaviour
{
    #region Variables

    [SerializeField] Color color = Color.white;

    [SerializeField] bool hasGap = false;
    [SerializeField] Sprite fullBodySprite;
    [SerializeField] Sprite gapBodySprite;

    [SerializeField] Image edgeIMG;
    [SerializeField] Color edgeHighlightColor;
    Color edgeColor;
    [SerializeField] int initialExtensions = 0;

    [SerializeField] BottomExtensionManager mainBlock;
    
    [SerializeField] BottomExtensionManager blockLayoutPF;

    // Components:
    [SerializeField] Image bodyImage;
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

        // Add initial extensions
        for (int i = 0; i < initialExtensions; i++)
            AddExtension();

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
        edgeIMG.gameObject.SetActive(edge);
    }

    public bool HasEdge()
    {
        return edgeIMG.gameObject.activeSelf;
    }

    public void SetHighlighted(bool highlight)
    {
        edgeIMG.color = highlight ? edgeHighlightColor : edgeColor;
    }

    // Adds another block object to extend the current block
    public void AddExtension()
    {
        // Instantiate the block layout and sets the color
        BottomExtensionManager b = Instantiate(blockLayoutPF, transform);
        // change color

        // Move the edge to the end
        edgeIMG.transform.SetAsLastSibling();

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
            block.AddBottomExtension(bottomColor);
    }

    // Returns a list of the colors of the bottom extensions
    public List<Color> GetBottomColors()
    {
        return attachedBlocks[0].GetBottomColors();
    }

    // Set the bottom extensions to the list of colors received
    public void SetBottomColors(List<Color> colors)
    {
        foreach (BottomExtensionManager block in attachedBlocks)
            block.UpdateBottomExtensions(colors);
    }

    // Removes all bottom extensions
    public void RemoveBottomExtensions()
    {
        foreach (BottomExtensionManager block in attachedBlocks)
        {
            block.RemoveBottomExtensions();
        }
    }

    // Sets the color of the elements of the block
    public void SetColor(Color color)
    {
        this.color = color;
        bodyImage.color = color;
        edgeIMG.color = color;
        edgeColor = color;
        foreach (var ext in attachedBlocks)
        {
            ext.SetColor(color);
        }
    }

    // Get the color of the block
    public Color GetColor()
    {
        return color;
    }

    // Enable/disables the abbility to drag and move the block
    public void SetDraggable(bool canDrag)
    {
        DragToMove dragC = GetComponent<DragToMove>();
        if(dragC) dragC.enabled = canDrag;
    }

    // Enable/disables the abbility to split from the loop
    public void SetSplitable(bool canSplit)
    {
        SplitBlockOnDrag splitC = GetComponent<SplitBlockOnDrag>();
        if (splitC) splitC.enabled = canSplit;
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
