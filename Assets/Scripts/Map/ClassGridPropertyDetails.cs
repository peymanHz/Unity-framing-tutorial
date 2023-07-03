[System.Serializable]
public class GridPropertyDetails
{
    public int gridX;
    public int gridY;
    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCOnsticale = false;
    public int daysSinceDug = -1;
    public int daysSinceWater = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHaverts = -1;

    public GridPropertyDetails() 
    { 
    
    }
}