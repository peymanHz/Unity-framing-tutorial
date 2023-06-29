using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    public void Awake()
    {
        //only populate in the editor 
        if (!Application.IsPlaying(gameObject))
        {
            //ensures that object has a guaranteed unique id
            if (_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
