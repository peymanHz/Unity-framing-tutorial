public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkDown,
    walkUp,
    walkRight,
    walkLeft,
    runDown,
    runUp,
    runRight,
    runLeft,
    useToolDown,
    useToolUp,
    useToolRight,
    useToolLeft,
    swingToolDown,
    swingToolUp,    
    swingToolRight,
    swingToolLeft,
    liftToolDown,
    liftToolUp,
    liftToolRight,
    liftToolLeft,
    holdToolDown,
    holdToolUp,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickLeft,
    pickRight,
    count
}

public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}

public enum PartVariantColour
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum Season
{
    Spring,
    summer,
    Autumn,
    Winter,
    none,
    Count
}

public enum ToolEffect
{
    none,
    watering,
}

public enum Direction
{
    up, 
    down, 
    left, 
    right, 
    none
}

public enum ItemType
{
    Seed,
    Commodity,
    Watering_Tool,
    Hoeing_Tool,
    Chopping_Tool,
    Breaking_Tool,
    Reaping_Tool,
    Collecting_Tool,
    Reapable_scenary,
    furniture,
    none,
    count
}
