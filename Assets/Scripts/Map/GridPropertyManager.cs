using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertyManager : SingletonMonobehavior<GridPropertyManager>, ISavable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridPRoperties[] SO_GridPRopertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;

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

    private void ClearDisplayGroundDecoration()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //dug
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //select tile based on surrendering dug tiles
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //set 4 tiles if dug surrendering currrent tile - up, down, left, right now that this center tile has been dug
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY +1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //get whether surrendering tiles are dug or not
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool dowmDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region set appropriate tile based on whether surrendering tiles are dug or not

        if (!upDug && !dowmDug && !rightDug &&!leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && dowmDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && dowmDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && dowmDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && dowmDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && dowmDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && dowmDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && dowmDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && dowmDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !dowmDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !dowmDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !dowmDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !dowmDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !dowmDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !dowmDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !dowmDug !&& !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;
        #endregion
    }

    private bool IsGridSquareDug(int gridX, int gridY)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridX, gridY);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    private void DisplayGridPropertyDetails()
    {
        //loop through all grid items 
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
        }
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

        //get tilemaps
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecortion1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecortion2).GetComponent<Tilemap>();
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

            //if grid properties exist
            if (gridPropertyDictionary.Count > 0)
            {
                //grid property details found for the current scene destroy existing ground decoration
                ClearDisplayGridPropertyDetails();

                //instantiate grid property details for current scene
                DisplayGridPropertyDetails();
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
