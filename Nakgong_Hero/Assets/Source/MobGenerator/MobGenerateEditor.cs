using System;
using Source.Item;
using UnityEditor;
using UnityEngine;

namespace Source.MobGenerator
{
    [CustomEditor(typeof(MobData))]
    public class MobGenerateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mobEnum = Enum.GetValues(typeof(MobData.MobType));
            var mobArray = (MobData) target;
            if (mobArray.Mobs is not { Length: > 0 } || mobArray.Mobs.Length != mobEnum.Length)
            {
                var newArray = new GameObject[mobEnum.Length];
                
                for (var i = 0; i < (mobArray.mobPrefabs?.Length ?? 0); i++)
                    if (mobArray.mobPrefabs != null)
                        newArray[i] = mobArray.mobPrefabs[i];
                for (var i = (mobArray.mobPrefabs?.Length ?? 0) - 1; i >= 0 && i < newArray.Length; i++) newArray[i] = new GameObject();
                mobArray.mobPrefabs = newArray;
                
                var countArray = new int[mobEnum.Length];
                
                for (var i = 0; i < (mobArray.mobPoolCount?.Length ?? 0); i++)
                    if (mobArray.mobPoolCount != null)
                        countArray[i] = mobArray.mobPoolCount[i];
                for (var i = (mobArray.mobPoolCount?.Length ?? 0) - 1; i >= 0 && i < countArray.Length; i++) countArray[i] = new int();

                mobArray.mobPoolCount = countArray;
            }
            if (mobArray.toggled == null || mobArray.toggled.Length != mobEnum.Length)
                mobArray.toggled = new bool[mobEnum.Length];
            GUILayout.Space(20);
            
            foreach (var type in mobEnum)
            {
                var idx = (int)type;
                var commonItem = commonItemArray.CommonItems[idx];
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
}
