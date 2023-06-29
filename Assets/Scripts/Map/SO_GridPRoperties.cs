using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_GridProperties", menuName = "scriptable objects/Grid Properties")]
public class SO_GridPRoperties : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth;
    public int gridHeight;
    public int orginalX;
    public int orginalY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;
}
