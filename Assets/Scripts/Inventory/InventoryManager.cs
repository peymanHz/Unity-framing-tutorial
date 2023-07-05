using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehavior<InventoryManager>
{
    private Dictionary<int, ItemsDetails> itemDetailsDictionary;

    private int[] selectedInventoryitem; //the index of the item is in the inventory list, and the value is the item code

    public List<InventoryItem>[] inventoryLists;

    [HideInInspector] public int[] inventoryListCapacityInArray;

    [SerializeField] private SO_ItemList itemList = null;

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryList();

        CreateItemdetailDictionary();

        //initialize selected inventory arrey
        selectedInventoryitem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryitem.Length; i++)
        {
            selectedInventoryitem[i] = -1;
        }
    }

    private void CreateInventoryList()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //initialise inventory list capacity array
        inventoryListCapacityInArray = new int[(int)InventoryLocation.count];

        //initialise player inventory list capacity
        inventoryListCapacityInArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// Populates the itemDetailDictionary from the scriptable object item list
    /// </summary>
    private void CreateItemdetailDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemsDetails>();

        foreach (ItemsDetails itemDetails in itemList.itemDetails) 
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails); 
        }
    }

    /// <summary>
    /// Add an item to the inventory list for inventorylocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

        /// <summary>
        /// Add an item to the inventory list for inventorylocation
        /// </summary>
        public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> invetoryList = inventoryLists[(int)inventoryLocation];

        //check if the inventory already contains an item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(invetoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(invetoryList, itemCode);
        }

        //send event that inventory has been updated
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity;
        inventoryList[position] = inventoryItem;

        Debug.ClearDeveloperConsole();
        //DebugPrintInventoryList(inventoryList);
    }

    public void SwapInventoryItem(InventoryLocation inventoryLocation, int fromInt, int toInt)
    {
        //if from item index and to item index are within the bounds of the list, not the same and greater than or equal to zero
        if (fromInt < inventoryLists[(int)inventoryLocation].Count && toInt < inventoryLists[(int)inventoryLocation].Count && fromInt != toInt && fromInt >= 0 && toInt >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromInt];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toInt];

            inventoryLists[(int)inventoryLocation][toInt] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromInt] = toInventoryItem;

            //send event that inventory has been updated
            EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }

    /// <summary>
    /// Clear the inventory item for inventorylocation
    /// </summary>
    public void ClearSelectedinventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryitem[(int)inventoryLocation] = -1;
    }

    /// <summary>
    /// find if an item code is already in the inventory, return the item position in the 
    /// inventory list, or -1 if the item is not in the inventory
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the itemDetails for the itemCode, or null if the item code doesn't exist
    /// </summary>
    public ItemsDetails GetItemDetails(int itemCode)
    {
        ItemsDetails itemsDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemsDetails))
        {
            return itemsDetails;
        }
        else
        {
            return null;
        }
    }

    public ItemsDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1) 
        {
            return null; 
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    /// <summary>
    /// get the selected item for inventoryLocation - return itemcode or -1 if not selcted
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryitem[(int)inventoryLocation];
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;

        switch (itemType)
        {
            case ItemType.Breaking_Tool:
                itemTypeDescription = Settings.BreakingTool;
                break;

            case ItemType.Chopping_Tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_Tool:
                itemTypeDescription = Settings.HoeingTool;
                break;

            case ItemType.Reaping_Tool:
                itemTypeDescription = Settings.ReapingTool;
                break;

            case ItemType.Watering_Tool:
                itemTypeDescription = Settings.WateringTool;
                break;

            case ItemType.Collecting_Tool:
                itemTypeDescription = Settings.CollectingTool;
                break;

            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    /// <summary>
    /// Remove an item from the inventory and create a game object at position it was dropped
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //chack if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        //send event that inventory has been updated
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    public void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    /// <summary>
    /// Set the selected inventory item for inventorylocation to itemcode
    /// </summary>
    public void SetSelectedinventoryItem(InventoryLocation inventoryLocation, int itemcode)
    {
        selectedInventoryitem[(int)inventoryLocation] = itemcode;
    }

    /*private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        foreach (InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "        Item quantity: "+ inventoryItem.itemQuantity);
        }
        Debug.Log("********************************************************************************");
    }*/
}
