using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataExecute;

[CustomEditor(typeof(ShopCatalogCreater))]
public class ShopCatalogEditor:Editor
    {
    SerializedProperty data;
    ShopCatalogCreater Shop;
    UserDataManager manager;
    bool[] folder;
    bool checker;
    private void OnEnable()
    {
        Shop = target as ShopCatalogCreater;
        if (Shop.data==null)
            Shop.data = new ShopItemDictionary();
        data = serializedObject.FindProperty("data");
        if (Shop.data.ItemDictionary==null)
            Shop.data.ItemDictionary = new List<ShopItemList>();
        manager = new UserDataManager();
        checker = false;
        folder = new bool[0];
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(data.FindPropertyRelative("id"), new GUIContent("아이디"));
        if (GUILayout.Button("상점 목록 불려오기"))
        {
            var b = manager.readShopCatalog(data.FindPropertyRelative("id").stringValue);
            if (b != null)
            {
                EditorGUILayout.LabelField("불려오기 성송");
            }
            else
                EditorGUILayout.LabelField("불려오기 실패");
        }
        if (GUILayout.Button("초기화"))
        {
            Shop.data = new ShopItemDictionary();
        }

        EditorGUILayout.LabelField("\n");
        EditorGUILayout.PropertyField(data.FindPropertyRelative("desc"), new GUIContent("상세"));
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(data.FindPropertyRelative("itemCount"), new GUIContent("판매 아이템 수"));
        EditorGUILayout.LabelField("\n");
        if (EditorGUI.EndChangeCheck())
        {
            
            int i=0;
            if (Shop.data.ItemDictionary.Count> data.FindPropertyRelative("itemCount").intValue)
            {
                i = Shop.data.ItemDictionary.Count - data.FindPropertyRelative("itemCount").intValue;
                for (int n=0;n< i;n++)
                {

                    data.FindPropertyRelative("ItemDictionary").DeleteArrayElementAtIndex(data.FindPropertyRelative("ItemDictionary").arraySize - 1);
                }
            }else if (Shop.data.ItemDictionary.Count < data.FindPropertyRelative("itemCount").intValue)
            {
                i = -Shop.data.ItemDictionary.Count + data.FindPropertyRelative("itemCount").intValue;
                for (int n = 0; n < i; n++)
                {

                    //Shop.data.ItemDictionary.Add(new ShopItemList() {List=new List<ShopItem>() });
                    data.FindPropertyRelative("ItemDictionary").InsertArrayElementAtIndex(0);
                  
                    data.FindPropertyRelative("ItemDictionary").GetArrayElementAtIndex(0).FindPropertyRelative("List").ClearArray();
                    data.FindPropertyRelative("ItemDictionary").MoveArrayElement(0, data.FindPropertyRelative("ItemDictionary").arraySize - 1);
                }
            }
           
        }
        EditorGUILayout.LabelField("\n");

            if ((folder == null || folder.Length != data.FindPropertyRelative("ItemDictionary").arraySize))
                folder = new bool[data.FindPropertyRelative("ItemDictionary").arraySize];
            checker = true;
            for (int n = 0; n < data.FindPropertyRelative("ItemDictionary").arraySize; n++)
            {

                folder[n] = EditorGUILayout.BeginFoldoutHeaderGroup(folder[n], (n + 1) + "번째 아이템 슬롯");
                if (folder[n])
                {
                SerializedProperty list = data.FindPropertyRelative("ItemDictionary").GetArrayElementAtIndex(n).FindPropertyRelative("List");
            
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty itemproperty = list.GetArrayElementAtIndex(i);
                    //SerializedProperty itemproperty = data.FindPropertyRelative("ItemDictionary").GetArrayElementAtIndex(n);
                    //EditorGUILayout.LabelField((i + 1) + "번");

                    EditorGUILayout.PropertyField(itemproperty.FindPropertyRelative("itemid"), new GUIContent("아이템 코드"));

                        if (manager.LoadItemData(itemproperty.FindPropertyRelative("itemid").stringValue) != null)
                        {
                            EditorGUILayout.LabelField("아이템 이름: " + manager.LoadItemData(itemproperty.FindPropertyRelative("itemid").stringValue).Name);
                        }
                        else
                        {
                            checker &= false;
                            EditorGUILayout.LabelField("아이템 이름: " + "아이템 지정 안됨");
                        }

                        EditorGUILayout.PropertyField(itemproperty.FindPropertyRelative("Amount"), new GUIContent("수량"));

                        EditorGUILayout.PropertyField(itemproperty.FindPropertyRelative("Needitemid"), new GUIContent("요구 아이템(제화)"));

                        if (manager.LoadItemData(itemproperty.FindPropertyRelative("Needitemid").stringValue) != null)
                        {
                            EditorGUILayout.LabelField("아이템 이름: " + manager.LoadItemData(itemproperty.FindPropertyRelative("Needitemid").stringValue).Name);
                        }
                        else
                        {
                            checker &= false;
                            EditorGUILayout.LabelField("아이템 이름: " + "아이템 지정 안됨");
                        }

                        EditorGUILayout.PropertyField(itemproperty.FindPropertyRelative("price"), new GUIContent("가격"));


                        EditorGUILayout.LabelField("\n");
                }
                EditorGUILayout.LabelField("\n");
                    if (GUILayout.Button("아이템 추가"))
                    {
                    list.InsertArrayElementAtIndex(0);
                    }
                }
            EditorGUILayout.EndFoldoutHeaderGroup();
            }
        
        if (checker)
        {
            if (GUILayout.Button("상점 데이터 베이스 생성"))
            {
                manager.createShopCatalog(Shop.data);
            }
        }
        else
        {
            EditorGUILayout.LabelField("데이터베이스가 완성되지 않았습니다.");
        }
        serializedObject.ApplyModifiedProperties();
    }
}
