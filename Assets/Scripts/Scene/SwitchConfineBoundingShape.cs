using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    } 

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    /// <summary>
    /// switches the collider that cinemachine uses to define the edge of the screen
    /// </summary>
    private void SwitchBoundingShape()
    {
        // get the polygon collider on 'boundsConfiner' gameobject which is used by cinemachine to prevent going beyond edge of the screen
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // clear cache

        cinemachineConfiner.InvalidatePathCache();

    }
}
