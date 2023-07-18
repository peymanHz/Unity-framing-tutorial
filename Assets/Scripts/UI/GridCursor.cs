using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite GreenCursorSprite = null;
    [SerializeField] private Sprite RedCursorSprite = null;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRaduis = 0;

    public int ItemUseGridRadius { get => _itemUseGridRaduis; set => _itemUseGridRaduis = value; }

    private ItemType _selectedItemType;

    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            //get grid positioin for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            //get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        //chack item use radius is valid
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // get selected item details
        ItemsDetails itemsDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemsDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // get grid property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            //etermine cursor validity based on inventory item selected and grid property details
            switch (itemsDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Watering_Tool:
                case ItemType.Breaking_Tool:
                case ItemType.Chopping_Tool:
                case ItemType.Hoeing_Tool:
                case ItemType.Reaping_Tool:
                case ItemType.Collecting_Tool:
                    if (!IsCursorValidForTool(gridPropertyDetails, itemsDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.none: 
                    break;

                case ItemType.count :
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = GreenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = RedCursorSprite;
        CursorPositionIsValid = false;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// sets the cursor as either valid or invalid for the tool for the target gidProperyDetails. return true if valid and false if invalid
    /// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemsDetails itemsDetails)
    {
        //switch on tool
        switch (itemsDetails.itemType)
        {
            case ItemType.Hoeing_Tool:
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region need to get any items at location so we can check if they are reapable
                    //get world position for cursor
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    //get list of items at cursor location
                    List<Item> itemList = new List<Item>();

                    HelperMethodes.GetComponentAtBocLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);
                    #endregion

                    //loop through all items found to see any are reapable type - we are not going to let the player dig where there are reapable scenary
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            case ItemType.Watering_Tool:
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWater == -1)
                {
                    return true;
                }
                else
                { 
                    return false; 
                }

            case ItemType.Collecting_Tool:
                //check if item can be harvested with the selected item, then check if item is fully grown

                //check if seed planted
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    //get crop details for seed
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    //if crop details found
                    if (cropDetails != null)
                    {
                        //check if crop fully grown
                        if (gridPropertyDetails.growthDays >= cropDetails.totalGrowthDays)
                        {
                            //check if the crop can be harvested with the tool selected
                            if (cropDetails.CanUseToolToHravestCrop(itemsDetails.itemCode))
                            {
                                return true;
                            }
                            else 
                            { 
                                return false; 
                            }
                        }
                        else
                        {
                            return false;  
                        }
                    }
                }

                    return false;

            default: 
                return false;
        }
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);

        CursorIsEnabled = true; 
    }

    public Vector3Int GetGridPositionForCursor()
    {
        //z is how far the object are in front of camera - camera is -10 so objects are (-)-10 in front = 10
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }

}