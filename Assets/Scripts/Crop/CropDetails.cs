using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode; //this is the itemcode for the corresponding seed 
    public int[] growthDays; // days growth for each stage
    public GameObject[] growthPrefab; // prefab to use when instantiating growth stages
    public Sprite[] growthSprite; // growth sprite
    public Season[] seasons; // growth seasons
    public Sprite harvestSprite; // sprite used once harvested
    [ItemCodeDescription]
    public int harvestedTransformItemCode; // if the item transforms into another item when harvested this item cod will be populated
    public bool hideCropBeforeHarvestedAnimation; // if the crop should be disabled before the harvested animation
    public bool disableCropColliderBeforeHarvestedAnimation; // if colliders on crop should be disbaled to avoid the harvested animation effecting any other game object
    public bool isHarvestedAnimation; // true if harvested animation to be played on final growth stage prefab
    public bool isHarvestActionEffect = false; // flag to determine whether there is a harvest action effect
    public bool spawnCropProducedAtPlayerPosition;
    public HavertsActionEffect harvestActionEffect; // the harvest action effect for the crop

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // arrey of item codes for the tools that can harvest or 0 arrey elements if no tool required
    public int[] requiredHarvestAction; // number of harvest actions rquired for correspponding tool in harvest tool item code arrey
    [ItemCodeDescription]
    public int[] cropProducedItemCode; // arrey of item codes produced for the harvested crop
    public int[] cropProducedMinQuantity; // arrey of minimum quantities produced for the harvested crop 
    public int[] cropProducedMaxQuantity; // if max quanityt > min quantity, then a random number of crops between min and max are produced
    public int daysoGrow; // days to regrow next crop aor -1 if a single crop

    /// <summary>
    /// returns true if the tool item code can be used to harvest this crop, else returns false
    /// </summary>
    public bool CanUseToolToHravestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// returns -1 if the tool cant be used to harvest this crop, else returns the number of harvest actions required by this tool
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestAction[i];
            }
        }
        return -1;
    }
}
