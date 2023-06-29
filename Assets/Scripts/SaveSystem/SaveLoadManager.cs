using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    public List<ISavable> iSavableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSavableObjectList = new List<ISavable>();
    }

    public void StoreCurrentSceneDate()
    {
        //loop through all ISavable objects and trigger store scene data for each
        foreach(ISavable iSavableObject in iSavableObjectList)
        {
            iSavableObject.IsavableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RrestoreCurrentSceneData()
    {
        //loop through all ISavable objects and trigger restore scene data for each
        foreach (ISavable iSavableObject in iSavableObjectList)
        {
            iSavableObject.ISavableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
