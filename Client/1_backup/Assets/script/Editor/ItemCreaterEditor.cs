using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataExecute;

[CustomEditor(typeof(ItemCreater)), CanEditMultipleObjects]
public class ItemCreaterEditor : Editor
{
    ItemCreater item;
    SerializedProperty data;
    private void OnEnable()
    {
        item = target as ItemCreater;
        data = serializedObject.FindProperty("data");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(data.FindPropertyRelative("id"),new GUIContent("아이디"));
        if (GUILayout.Button("아이템 불려오기"))
        {
            var a = new UserDataManager();
           var b=  a.LoadItemData(data.FindPropertyRelative("id").stringValue);
            if (b != null)
            {
                item.data = b;
                EditorGUILayout.LabelField("불려오기 성송");
            }
            else
                EditorGUILayout.LabelField("불려오기 실패");
        }
        if (GUILayout.Button("초기화"))
        {
            item.data = new ItemData();

        }
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.PropertyField(data.FindPropertyRelative("Name"), new GUIContent("이름"));
        EditorGUILayout.LabelField("설명");
        data.FindPropertyRelative("desc").stringValue = EditorGUILayout.TextArea(data.FindPropertyRelative("desc").stringValue);
        EditorGUILayout.LabelField("\n");
        if (GUILayout.Button("아이템 생성"))
        {
            var a = new UserDataManager();
            a.CreateItemData(item.data);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
