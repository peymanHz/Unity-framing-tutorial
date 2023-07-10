using System.Collections.Generic;
using UnityEngine;

public static class HelperMethodes
{
    /// <summary>
    /// get components of type T at positionToCheack. returns true if at least one found and the found components are in componentsAtPositionList
    /// </summary>
    public static bool GetComponentAtCursorlocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArrey = Physics2D.OverlapPointAll(positionToCheck);

        //loop through all coliders to get an object of type T

        T tComponent = default(T);

        for (int i = 0; i <  collider2DArrey.Length; i++)
        {
            tComponent = collider2DArrey[i].GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArrey[i].GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }


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

    /// <summary>
    /// returns arrey of components of type t at box wiht center point and size and angle, the numberOfCollidersToTest for is passed as a parameter. found components are returned in the arrey
    /// </summary>
    public static T[] GetComponentAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    {
        Collider2D[] collider2DArrey = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArrey);

        T tComponent = default(T);

        T[] componentArrey = new T[collider2DArrey.Length];

        for (int i = collider2DArrey.Length -1; i >= 0; i--)
        {
            if (collider2DArrey[i] != null)
            {
                tComponent = collider2DArrey[i].gameObject.GetComponent<T>();

                if (tComponent != null)
                {
                    componentArrey[i] = tComponent;
                }
            }
        }

        return componentArrey;
    }
}