using System;
using Source.Item;
using UnityEditor;
using UnityEngine;

namespace Source.MobGenerator
{
    [CustomEditor(typeof(MobData))]
    public class MobGenerateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mobEnum = Enum.GetValues(typeof(MobData.MobType));
            var mobArray = (MobData) target;
            if (mobArray.mobs == null || mobArray.mobs.Length != mobEnum.Length)
            {
                mobArray.mobPrefabs = new GameObject[mobEnum.Length];
                mobArray.mobPoolCount = new int[mobEnum.Length];
            }
            if (mobArray.toggled == null || mobArray.toggled.Length != mobEnum.Length)
                mobArray.toggled = new bool[mobEnum.Length];
            GUILayout.Space(20);
            
            foreach (var type in mobEnum)
            {
                var idx = (int)type;
                if (mobArray.toggled[idx] == EditorGUILayout.BeginFoldoutHeaderGroup(mobArray.toggled[idx], $"{type}: {((MobData.MobType) idx).ToString()}"))
                {
                    mobArray.mobPrefabs[idx] =
                        (GameObject)EditorGUILayout.ObjectField("Prefab", mobArray.mobPrefabs[idx], typeof(GameObject), false);
                    EditorGUILayout.Space(14);
                    mobArray.mobPoolCount[idx] = EditorGUILayout.IntField("Pool Count", mobArray.mobPoolCount[idx]);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorUtility.SetDirty(target); 
        }
    }
}
  
