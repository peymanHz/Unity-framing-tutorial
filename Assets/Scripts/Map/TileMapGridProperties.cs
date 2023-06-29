using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TileMapGridProperties : MonoBehaviour
{
    private Tilemap tilemap;
    private Grid grid;
    [SerializeField] private SO_GridPRoperties gridProperties = null;
    [SerializeField] private GridBoolProperty GridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridProperties != null )
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null )
            {
                //this is required to ensure that we updated gridproperties gameobject gets saved when the game is saved
                //otherwise they are not saved
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        //compress tilemap bounds
        tilemap.CompressBounds();

        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null )
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), GridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        //only populate in the editor
        if (!Application.isPlaying)
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
}
