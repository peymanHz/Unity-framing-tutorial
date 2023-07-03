using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehavior<SceneItemsManager>, ISavable
{
    private Transform parentItem;
    [SerializeField] private GameObject ItemPrefab = null;

    private string _iSavableUniqueID;
    public string ISavableUniqueID { get { return _iSavableUniqueID;} set { _iSavableUniqueID = value; }  }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        base.Awake();

        ISavableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    /// <summary>
    /// destroy item currently in the scene 
    /// </summary>
    private void DestroySceneItem()
    {
        Debug.Log("gfgf");
        //Get all items in the scene 
        Item[] itemInScene = GameObject.FindObjectsOfType<Item>();

        //loop through all scene item and destroy them
        for (int i = itemInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(ItemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItem(List<SceneItem> sceneItemList)
    {
        GameObject itemGaemObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGaemObject = Instantiate(ItemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);
            
            Item item = itemGaemObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }
    
    private void OnDisable()
    {
        ISavableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void OnEnable()
    {
        ISavableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    public void ISavableDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void ISavableRestoreScene(string sceneName)
    {
        Debug.Log($"TRYING TO Load {sceneName}");
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            Debug.Log("2");
            if (sceneSave.listSceneItem != null)
            {
                Debug.Log("3");
                //scene list item found - destroy exsiting item in the scene 
                DestroySceneItem();

                // now instantiate the list of scene items
                InstantiateSceneItem(sceneSave.listSceneItem);
            }
        }
    }

    public void ISavableRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void IsavableStoreScene(string sceneName)
    {
        //remove old scene save for gameobject if exists
        GameObjectSave.sceneData.Remove(sceneName);

        //Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemInScene = FindObjectsOfType<Item>();

        //loop though all scene items 
        foreach (Item item in itemInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            //add scene item to list 
            sceneItemList.Add(sceneItem);
        }

        //create list scene item in scene save and set to scene item list 
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;

        //add scene save to gameobject 
        Debug.Log($"Added Scene {sceneName}");
        GameObjectSave.sceneData.Add(sceneName, sceneSave );
    }

}
