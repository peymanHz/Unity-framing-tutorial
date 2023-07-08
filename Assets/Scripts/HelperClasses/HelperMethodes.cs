using System.Collections.Generic;
using UnityEngine;

public static class HelperMethodes
{
    /// <summary>
    /// get components of type T at box with center point and size and angle. returns true if at least one found and the found components are retunred to the list
    /// </summary>
    public static bool GetComponentAtBocLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArrey = Physics2D.OverlapBoxAll(point, size, angle);

        //loop through all colliders to get an onbejct of type T
        for (int i = 0; i < collider2DArrey.Length; i++)
        {
            T tComponent = collider2DArrey[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArrey[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found; 
    }
}