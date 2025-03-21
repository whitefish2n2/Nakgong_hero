#if UNITY_EDITOR

using System;
using Source.Item;
using Source.Item.Datas;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Editor
{
    [CustomEditor(typeof(ActiveItemData))]
    public class ActiveItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        { 
            base.OnInspectorGUI();
            
            var itemsEnum = Enum.GetValues(typeof(ActiveItemType));
            
            var commonItemArray = (ActiveItemData)target;
            if (commonItemArray.commonItems is not { Length: > 0 } || commonItemArray.commonItems.Length != itemsEnum.Length)
            {
                var newArray = new CommonItem[itemsEnum.Length];
                
                for (var i = 0; i < (commonItemArray.commonItems?.Length ?? 0); i++) newArray[i] = commonItemArray.commonItems![i];
                for (var i = (commonItemArray.commonItems?.Length ?? 0) - 1; i >= 0 && i < newArray.Length; i++) newArray[i] = new CommonItem();

                commonItemArray.commonItems = newArray;
            }

            if (commonItemArray.toggled == null || commonItemArray.toggled.Length != itemsEnum.Length)
                commonItemArray.toggled = new bool[itemsEnum.Length];
            
            GUILayout.Space(20);

            foreach (var type in itemsEnum)
            {
                var idx = (int)type;
                var commonItem = commonItemArray.commonItems[idx];
                if (commonItemArray.toggled[idx] == EditorGUILayout.BeginFoldoutHeaderGroup(commonItemArray.toggled[idx], $"{type}: {commonItem.ItemName}"))
                {
                    commonItem.itemType = ItemCategory.Active;
                    commonItem.ItemName = EditorGUILayout.TextField("Name", commonItem.ItemName);
                    EditorGUILayout.LabelField("Description");
                    commonItem.Discription = EditorGUILayout.TextArea(commonItem.Discription, new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true
                    });
                    commonItem.InvSprite =
                        (Sprite)EditorGUILayout.ObjectField("InvSprite", commonItem.InvSprite, typeof(Sprite), false);
                    commonItem.prefab =
                        (GameObject)EditorGUILayout.ObjectField("Prefab", commonItem.prefab, typeof(GameObject), false);
                    commonItem.isDestroyItem = EditorGUILayout.Toggle("Is DestroyItem", commonItem.isDestroyItem);
                    EditorGUILayout.Space(14);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
#endif
