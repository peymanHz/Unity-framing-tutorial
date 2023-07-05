using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UiInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GridCursor gridCursor;
    private GameObject draggedItem;

    public Image inventorySlotHgihLight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProGUI;

    [SerializeField] private UiInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventorytexboxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemsDetails itemsDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;

    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

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
        gridCursor = FindObjectOfType<GridCursor>();
    }

    private void ClearCursors()
    {
        //disable cursor
        gridCursor.DisableCursor();

        //set item type to none
        gridCursor.SelectedItemType = ItemType.none;
    }

    /// <summary>
    /// set this inventory slot item to be selected
    /// </summary>
    private void SetSelecctedItem()
    {
        //clear currently highlighted item
        inventoryBar.ClearHighlightOnInventorySlots();

        //highlight item on the inventory bar
        isSelected = true;

        //set highlighted inventory slot
        inventoryBar.SetHighlightedInventoryslots();

        //set use radius for cursor
        gridCursor.ItemUseGridRadius = itemsDetails.itemUseGridRadius;

        //if item requires a grid cursor then enables cursor
        if (itemsDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        //set item type
        gridCursor.SelectedItemType = itemsDetails.itemType;

        //set item selected in the inventory
        InventoryManager.Instance.SetSelectedinventoryItem(InventoryLocation.player, itemsDetails.itemCode);

        if (itemsDetails.canBeCarried == true)
        {
            //show player carry item
            Player.Instance.ShowCarriedItem(itemsDetails.itemCode);
        }
        else
        {
            //show player carry nothing
            Player.Instance.ClearCarriedItem();
        }
    }

    private void ClearSelectedItem()
    {
        ClearCursors(); 

        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        InventoryManager.Instance.ClearSelectedinventoryItem(InventoryLocation.player);

        Player.Instance.ClearCarriedItem();
    }

    /// <summary>
    /// drops the item (if selected) at the current mouse position. called by the dropitem event
    /// </summary>
    private void DropSelctedItemAtMouseposition()
    {
        if (itemsDetails != null && isSelected)
        {
            //if a valid cursor position
            if (gridCursor.CursorPositionIsValid)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
                
                //create item from prefab at mouse position
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize/2f, worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemsDetails.itemCode;

                //remove item from players inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemsDetails != null)
        {
            //disable keyboard input
            Player.Instance.DisablePlayerInputAndResetMovment();

            //instantiate gameobject as dragged item
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            //get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelecctedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //move gameobject as dragged item
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            //destroy gameobject as dragged item
            Destroy(draggedItem);

            //if drag end over inventory bar, get item drag is over and swap them
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UiInventorySlot>() != null)
            {
                //get the slot number where the drag ended
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UiInventorySlot>().slotNumber;

                // Swap Inventory item in inventory list
                InventoryManager.Instance.SwapInventoryItem(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                ClearSelectedItem();
            }
            //else attempt the item if it can dropped
            else
            {
                if (itemsDetails.canBeDropped)
                {
                    DropSelctedItemAtMouseposition();
                }
            }

            //enable player input
            Player.Instance.EnablePlayerInput();    
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected == true) 
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelecctedItem();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //populate tex box with item details
        if (itemQuantity != 0)
        {
            //instantiate inventory text box
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventorytexboxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UiInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UiInventoryTextBox>();

            //set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemsDetails.itemType);

            //populate text Box
            inventoryTextBox.SetTextboxText(itemsDetails.itemDescription, itemTypeDescription, "", itemsDetails.itemLongDescription, "", "");

            //set text box position according to inventory bar position
            if (inventoryBar.isInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 70f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 70f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null) 
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;

    }

}
