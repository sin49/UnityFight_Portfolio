using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataExecute;
using System.IO;
using System;
using Newtonsoft.Json;


[CustomEditor(typeof(accountEditorScript))]
public class AccountTempletEditor : Editor
{
    accountEditorScript editor;
    UserDataManager m;
    SerializedProperty account;
    bool characterlock;
    private void OnEnable()
    {
        m = new UserDataManager();
        editor = target as accountEditorScript;
        editor.AccountText = null;
        

    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
      
       

            //EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AccountText"), new GUIContent("계정"));

            //if (EditorGUI.EndChangeCheck())

            //{
            //    editor.account = JsonUtility.FromJson<UserAccountData>(editor.AccountText.text);

            //    account = serializedObject.FindProperty("account");
            //    serializedObject.FindProperty("FileName").stringValue = editor.AccountText.name;
            //}

        if (editor.AccountText != null)
        {
            //var a = JsonConvert.DeserializeObject<UserAccountDataEditorMember>(editor.AccountText.text);
            //if (a != null)
            //{
                editor.account = JsonConvert.DeserializeObject<UserAccountDataNetworkMember>(editor.AccountText.text);
    
                account = serializedObject.FindProperty("account");
                serializedObject.FindProperty("FileName").stringValue = editor.AccountText.name;
            //}
            //else
            //{
            //    Debug.Log("jsonloaderror?");
            //}
        }
    

        if (account != null)
        {
    
            //EditorGUILayout.LabelField("버전: " + account.FindPropertyRelative("LauncherVersion").stringValue);
            EditorGUILayout.LabelField("uid: " + account.FindPropertyRelative("_uid").stringValue);
            EditorGUILayout.PropertyField(account.FindPropertyRelative("username"), new GUIContent("이름"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("_accountlv"), new GUIContent("계정레벨"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("_MaxStamina"), new GUIContent("최대 스테미나"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("_Stamina"), new GUIContent("스테미나"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("_Max_Exp"), new GUIContent("최대 경험치"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("_Exp"), new GUIContent("경험치"));
            EditorGUILayout.PropertyField(account.FindPropertyRelative("StarChracter"), new GUIContent("대표 캐릭터"));
            EditorGUILayout.LabelField("최근 접속 날짜: " + account.FindPropertyRelative("LastestConnect").stringValue);


            EditorGUILayout.LabelField("\n");

            EditorGUILayout.LabelField("아이템");
         
            SerializedProperty s = account.FindPropertyRelative("Itemlist");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemData"), new GUIContent("아이템 정보"));

            if (GUILayout.Button("아이템 추가"))
            {
                ItemData d = JsonUtility.FromJson<ItemData>(editor.ItemData.text);
                if (d == null)
                    Debug.Log("잘못된 아이템 데이터 입니다.");
                else
                {
                    editor.account.Itemlist.Add(new item() { itemid = d.id, data = d });
                   
                }
                editor.ItemData = null;
            }
            EditorGUILayout.LabelField("\n");
            
            for (int n = 0; n < s.arraySize; n++)
            {
                SerializedProperty item = s.GetArrayElementAtIndex(n);

                EditorGUILayout.LabelField("ID: " + item.FindPropertyRelative("itemid").stringValue);
                ItemData data = m.LoadItemData(item.FindPropertyRelative("itemid").stringValue);
                EditorGUILayout.LabelField("이름: " + data.Name);
                EditorGUILayout.LabelField("설명: " + data.desc);
                EditorGUILayout.PropertyField(item.FindPropertyRelative("Amount"), new GUIContent("갯수"));
                if (GUILayout.Button("아이템 삭제"))
                {
                    editor.account.Itemlist.RemoveAt(n);
                   
                }

                EditorGUILayout.LabelField("\n");
            }
            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField("캐릭터");
            SerializedProperty chrlist = account.FindPropertyRelative("Characterlist");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterInfo"), new GUIContent("캐릭터 정보"));
            characterlock = EditorGUILayout.Toggle("캐릭터 잠금 상태", characterlock);
            if (GUILayout.Button("플레이어블 캐릭터 데이터 생성"))
            {
                if (editor.CharacterInfo == null)
                    Debug.Log("데이터가 없습니다.");
                else if (JsonUtility.FromJson<CharacterInformation>(editor.CharacterInfo.text) == null)
                    Debug.Log("잘못된 데이터 입니다.");
                else
                {
                    CharacterInformation info = JsonUtility.FromJson<CharacterInformation>(editor.CharacterInfo.text);
                    PlayerData d = new PlayerData
                    {
                        Grade = info.BaseGrade,
                        LV = 1
                        ,
                        SpecialLV = 1,
                        skillLV = new int[info.skills.Length]
                        ,
                        id = info.id,
                        characterlock = characterlock
                    };

                    for (int n = 0; n < d.skillLV.Length; n++)
                    {
                        d.skillLV[n] = 1;
                    }
                    editor.account.Characterlist.Add(d);
            
                    
                }
                editor.CharacterInfo = null;
            }


            for (int n = 0; n < chrlist.arraySize; n++)
            {
                
                SerializedProperty item = chrlist.GetArrayElementAtIndex(n);

                EditorGUILayout.LabelField("ID: " + item.FindPropertyRelative("id").stringValue);
              
                EditorGUILayout.PropertyField(item.FindPropertyRelative("characterlock"), new GUIContent("캐릭터 잠금 상태"));
                if (GUILayout.Button("캐릭터 삭제"))
                {
                    editor.account.Characterlist.RemoveAt(n);
                    
                }

                EditorGUILayout.LabelField("\n");
            }

            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField("스테이지");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StageData"), new GUIContent("스테이지 정보"));

            if (GUILayout.Button("스테이지 추가"))
            {
                StageInformation d = JsonUtility.FromJson<StageInformation>(editor.StageData.text);
                if (d == null)
                    Debug.Log("잘못된 스테이지 데이터 입니다.");
                else
                {
                    editor.account.Stagelist.Add(new StageData()
                    {
                        ID = d.ID,
                        name = d.name,
                        stagelock = false
                    ,
                        stagemission = d.missionarray
                    });
                   
                }
                editor.StageData = null;
            }
            SerializedProperty Stagelist = account.FindPropertyRelative("Stagelist");
            for (int n = 0; n < Stagelist.arraySize; n++)
            {
                SerializedProperty item = Stagelist.GetArrayElementAtIndex(n);

                EditorGUILayout.LabelField("ID: " + item.FindPropertyRelative("ID").stringValue);
                EditorGUILayout.LabelField("이름: " + item.FindPropertyRelative("name").stringValue);


                EditorGUILayout.PropertyField(item.FindPropertyRelative("stagelock"), new GUIContent("잠금상태"));
                if (GUILayout.Button("스테이지 삭제"))
                {
                    editor.account.Stagelist.RemoveAt(n);
                   
                }

                EditorGUILayout.LabelField("\n");
            }
            EditorGUILayout.LabelField("\n");


            EditorGUILayout.PropertyField(serializedObject.FindProperty("FIlePath"), new GUIContent("저장 경로"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FileName"), new GUIContent("파일 이름"));
            if (GUILayout.Button("저장"))
            {

                m.createAccount(editor.account,
                    serializedObject.FindProperty("FIlePath").stringValue,
                    serializedObject.FindProperty("FileName").stringValue);
            }
        }
        else
        {
            EditorGUILayout.LabelField("계정을 찿을 수 없습니다.");
        }
        serializedObject.ApplyModifiedProperties();
    }
}
