using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Items;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

namespace CommonItemEditor
{
    [CustomEditor(typeof(ItemData))]
    public class CommonItemEditer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var CommonItemsEnum = Enum.GetValues(typeof(CommonItemType));
            
            var CommonItemArray = (ItemData)target;
            if (CommonItemArray.CommonItems is not { Length: > 0 } || CommonItemArray.CommonItems.Length != CommonItemsEnum.Length)
            {
                var newArray = new CommonItem[CommonItemsEnum.Length];
                
                for (var i = 0; i < (CommonItemArray.CommonItems?.Length ?? 0); i++) newArray[i] = CommonItemArray.CommonItems[i];
                for (var i = (CommonItemArray.CommonItems?.Length ?? 0) - 1; i >= 0 && i < newArray.Length; i++) newArray[i] = new CommonItem();

                CommonItemArray.CommonItems = newArray;
            }

            if (CommonItemArray.toggled == null || CommonItemArray.toggled.Length != CommonItemsEnum.Length)
                CommonItemArray.toggled = new bool[CommonItemsEnum.Length];
            
            GUILayout.Space(20);

            int idx;
            foreach (var type in CommonItemsEnum)
            {
                idx = (int)type;
                var commonItem = CommonItemArray.CommonItems[idx];
                if (CommonItemArray.toggled[idx] = EditorGUILayout.BeginFoldoutHeaderGroup(CommonItemArray.toggled[idx], $"{((CommonItemType)type).ToString()}: {commonItem.ItemName}"))
                {
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
                    EditorGUILayout.Space(14);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}

