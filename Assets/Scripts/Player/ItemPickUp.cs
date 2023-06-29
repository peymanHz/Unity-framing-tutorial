using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            // get item details
            ItemsDetails itemsDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            //if item can be picked up
            if (itemsDetails.canBePickedUp == true)
            {
                //add item to inventory
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}
