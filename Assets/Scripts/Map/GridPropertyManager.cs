using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertyManager : SingletonMonobehavior<GridPropertyManager>, ISavable
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridPRoperties[] SO_GridPRopertiesArray = null;

    private string _ISavableUniqueID;

    public string ISavableUniqueID { get { return _ISavableUniqueID; } set { _ISavableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISavableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISavableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
    }

    private void OnDisable()
    {
        ISavableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    /// <summary>
    /// this initialise the grid property dictionary with the values from the SO_GridProperties assets and stores the value for eac scene in GameOnjectSave SceneData
    /// </summary>
    private void InitialiseGridProperties()
    {
        //loop through all gridProperties in the arrey
        foreach (SO_GridPRoperties so_GridPRoperties in SO_GridPRopertiesArray)
        {
            //create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            //populate grid property dictionary - iterate through all the grid properties in the so gridproperties list
            foreach (GridProperty gridProperty in so_GridPRoperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                            gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                            break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCOnsticale = gridProperty.gridBoolValue;
                        break;

                    default: 
                         break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            //create scene save for this game object 
            SceneSave sceneSave = new SceneSave();

            //add grid property dictionary to scene save data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            //if starting scene set the gridPropertyDictionary member varibale to the current iteration
            if (so_GridPRoperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }
            
            // add scene save to gme object scene data 
            GameObjectSave.sceneData.Add(so_GridPRoperties.sceneName.ToString(), sceneSave);    
        }
    }

    private void AfterSceneLoaded()
    {
        //get grid
        grid = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// returns the gridPropertyDetails at the gridLocation for the supplied dictionary, or null if no properties exist at that location
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //construct key from coordinate 
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // check if the grid property details exists for the grid coorinate and retrieves
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            //if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    /// <summary>
    /// get the grid property details for the tile at (gridX,gridY). if no grid property details exist null is returned and can assume all grid property details values are null or false
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }
    public void ISavableRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void ISavableDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void IsavableStoreScene(string sceneName)
    {
        //remove sceneSave from scene
        GameObjectSave.sceneData.Remove(sceneName);

        //create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // create and add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        //add scene save to game object scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public void ISavableRestoreScene(string sceneName)
    {
        //get sceneSave for scene - it exists since we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            //get grid property details dictionary - it exists since we created it in initialise
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
        }
    }

    /// <summary>
    /// set the grid property details to the GridPropertyDetails for the tile at (gridX,GridY) for the current scene
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    /// <summary>
    /// set the grid property details to the GridPropertyDetails for the tile at (gridX,GridY) for the gridPropertdictionary
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY,  GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertDictionary)
    {
        //construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        //set value
        gridPropertDictionary[key] = gridPropertyDetails;
    }

}
