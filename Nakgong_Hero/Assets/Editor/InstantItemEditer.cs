using System;
using Source.Item;
using Source.Item.Datas;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(InstantItemData))]
    public class InstantItemEditer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var itemsEnum = Enum.GetValues(typeof(InstantItemType));
            
            var commonItemArray = (InstantItemData)target;
            if (commonItemArray.CommonItems is not { Length: > 0 } || commonItemArray.CommonItems.Length != itemsEnum.Length)
            {
                var newArray = new CommonItem[itemsEnum.Length];
                
                for (var i = 0; i < (commonItemArray.CommonItems?.Length ?? 0); i++) newArray[i] = commonItemArray.CommonItems[i];
                for (var i = (commonItemArray.CommonItems?.Length ?? 0) - 1; i >= 0 && i < newArray.Length; i++) newArray[i] = new CommonItem();

                commonItemArray.CommonItems = newArray;
            }

            if (commonItemArray.toggled == null || commonItemArray.toggled.Length != itemsEnum.Length)
                commonItemArray.toggled = new bool[itemsEnum.Length];
            
            GUILayout.Space(20);

            foreach (var type in itemsEnum)
            {
                var idx = (int)type;
                var commonItem = commonItemArray.CommonItems[idx];
                if (commonItemArray.toggled[idx] == EditorGUILayout.BeginFoldoutHeaderGroup(commonItemArray.toggled[idx], $"{type}: {commonItem.ItemName}"))
                {
                    commonItem.itemType = ItemCategory.Instance;
                    commonItem.ItemName = EditorGUILayout.TextField("Name", commonItem.ItemName);
                    EditorGUILayout.LabelField("Description");
                    commonItem.Discription = EditorGUILayout.TextArea(commonItem.Discription, new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true
                    });
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

