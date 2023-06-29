using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptonDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();// starts checking for changed values

            //draw item code 
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            //draw item description
            EditorGUI.LabelField(new Rect(position.x, position.y+position.height/2, position.width, position.height/2), "ItemDescription", GetItemDescription(property.intValue));

            //if item code value has changed, then set value to new value 
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_ItemList;

        so_ItemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/item/so_itemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemsDetails> itemDetailsList = so_ItemList.itemDetails;

        ItemsDetails itemDetail = itemDetailsList.Find(x=>x.itemCode == itemCode);

        if (itemDetail != null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return string.Empty;
        }
    }
}
