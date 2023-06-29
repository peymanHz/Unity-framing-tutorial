using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UiInventorySlot[] inventorySlot = null;
    public GameObject inventoryBarDraggedItem;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool isInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdatede;
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += InventoryUpdatede;
    }

    private void Update()
    {
        SwitchInventorybarPosition();
    }

    /// <summary>
    /// clear all highlight from the inventory bar
    /// </summary>
    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    inventorySlot[i].isSelected = false;
                    inventorySlot[i].inventorySlotHgihLight.color = new Color(0f, 0f, 0f, 0f);
                    InventoryManager.Instance.ClearSelectedinventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            //loos through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProGUI.text = "";
                inventorySlot[i].itemsDetails = null;
                inventorySlot[i].itemQuantity = 0;
                SetHighlightedInventoryslots(i);
            }
        }
    }

    private void InventoryUpdatede(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                //loos through inventory slots and update with correspondign inventory list item
                for (int i = 0; i < inventorySlot.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        //ItemDetails itemDetails = inventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemcode);
                        ItemsDetails itemsDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemsDetails != null)
                        {
                            //Add images and details to inventory item slot
                            inventorySlot[i].inventorySlotImage.sprite = itemsDetails.itemSprite;
                            inventorySlot[i].textMeshProGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].itemsDetails = itemsDetails;
                            inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventoryslots(i);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public void SetHighlightedInventoryslots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                SetHighlightedInventoryslots(i);
            }
        }
    }

    public void SetHighlightedInventoryslots(int itemPosition)
    {
        if (inventorySlot.Length > 0 && inventorySlot[itemPosition].itemsDetails != null)
        {
            if (inventorySlot[itemPosition].isSelected)
            {
                inventorySlot[itemPosition].inventorySlotHgihLight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedinventoryItem(InventoryLocation.player, inventorySlot[itemPosition].itemsDetails.itemCode);
            }
        }
    }

    private void SwitchInventorybarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewPortPosition();

        if (playerViewportPosition.y > 0.3f && isInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && isInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            isInventoryBarPositionBottom = false;
        }
    }
}
