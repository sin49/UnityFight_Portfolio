using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataExecute;


[CustomEditor(typeof(StageCreater)), CanEditMultipleObjects]
    public class StageCreaterEditor : Editor
    {
    StageCreater stage;
    SerializedProperty DataJson;
        SerializedProperty data;
    SerializedProperty CList;
    SerializedProperty Missions;
    SerializedProperty Terrain;
    bool clistfolder;
    SerializedProperty RewardList;
    bool RewardListfolder;
    UserDataManager manager;
    SerializedProperty spawn;
    bool spawnfolder;
    bool missionfolder;
    GUIStyle g;
        private void OnEnable()
        {
        manager = new UserDataManager();
        stage = target as StageCreater;
        data = serializedObject.FindProperty("data");
      spawn = data.FindPropertyRelative("spawn");
        CList = data.FindPropertyRelative("SpawnCharacterList");
        RewardList = data.FindPropertyRelative("rewardlist");
        Missions = data.FindPropertyRelative("missionarray");
        DataJson = serializedObject.FindProperty("DataJson");
        Terrain= data.FindPropertyRelative("TerrainDataName");
        g = new GUIStyle();
        
        g.normal.textColor = Color.red;
        if (stage.data.spawn == null)
            stage.data.spawn = new List<spawnInformation>();
        if (stage.data.SpawnCharacterList == null)
            stage.data.SpawnCharacterList = new List<CharacterData>();
        if (stage.data.rewardlist == null)
            stage.data.rewardlist = new List<StageReward>();
        if (stage.data.missionarray == null)
            stage.data.missionarray = new List<StageMission>();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(data.FindPropertyRelative("ID"), new GUIContent("ID"));
        if (GUILayout.Button("맵정보 불려오기"))
        {
            var a = manager.LoadStageinfo(data.FindPropertyRelative("ID").stringValue);
            if (a != null)
            {
                stage.data = a;
                EditorGUILayout.LabelField("맵정보 불려오기 성공");
            }
            else
            {
                EditorGUILayout.LabelField("맵정보 불려오기 실패");
            }
        }
        EditorGUILayout.PropertyField(DataJson, new GUIContent("Json"));
        if (GUILayout.Button("맵정보 불려오기2"))
        {
            var a = JsonUtility.FromJson<StageInformation>(stage.DataJson.text);
            if (a != null)
            {
                stage.data = a;
                EditorGUILayout.LabelField("맵정보 불려오기 성공");
            }
            else
            {
                EditorGUILayout.LabelField("맵정보 불려오기 실패");
            }
            //DataJson= null;
        }
        if (GUILayout.Button("초기화"))
        {
            stage.data = new StageInformation();
            stage.data.spawn = new List<spawnInformation>();
            stage.data.SpawnCharacterList = new List<CharacterData>();
            stage.data.rewardlist = new List<StageReward>();
        
            
        }
        EditorGUILayout.LabelField("\n");

        EditorGUILayout.PropertyField(data.FindPropertyRelative("name"), new GUIContent("이름"));
        EditorGUILayout.PropertyField(data.FindPropertyRelative("StaminaNeed"), new GUIContent("스테미나 요구량"));
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.IntSlider(data.FindPropertyRelative("waveNumber"), 1, 5, new GUIContent("웨이브 숫자"));
        if (EditorGUI.EndChangeCheck())
        {
            if (stage.data.spawn == null)
                stage.data.spawn = new List<spawnInformation>();
            foreach (spawnInformation s in stage.data.spawn)
            {
                if (s.round > data.FindPropertyRelative("waveNumber").intValue)
                    s.round = data.FindPropertyRelative("waveNumber").intValue;
            }
        }
        EditorGUILayout.LabelField("\n");
        clistfolder= EditorGUILayout.BeginFoldoutHeaderGroup(clistfolder,"등장 적 종류");
        if (clistfolder)
        {
            for (int i = 0; i < CList.arraySize; i++)
            {
                var data = CList.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField(i + 1 + "번 적");

              
                EditorGUILayout.PropertyField(data.FindPropertyRelative("id"), new GUIContent("ID"));
                CharacterInformation obj = null;
                if ((obj = manager.readCharacterinformation(data.FindPropertyRelative("id").stringValue)) == null)
                {
                    EditorGUILayout.LabelField("id와 일치하는 캐릭터를 발견할 수 없습니다!");
                    EditorGUILayout.LabelField("이 데이터를 저장하지 않습니다", g);
                }
                else
                {
                    if (stage.data.SpawnCharacterList[i].id == "")
                    {
                        Debug.Log("불려오기");
                        stage.data.SpawnCharacterList[i].setData(obj);
                    }
                 
                    EditorGUILayout.PropertyField(data.FindPropertyRelative("LV"), new GUIContent("레벨"));

                    if (GUILayout.Button("스폰 정보 생성"))
                    {
                        stage.data.spawn.Add(new spawnInformation(i));
                    }
                    if (GUILayout.Button("삭제"))
                    {
                        stage.data.SpawnCharacterList.RemoveAt(i);
                        for (int n = 0; n < stage.data.spawn.Count; n++)
                        {
                            if (stage.data.spawn[n].index == i)
                            {
                                stage.data.spawn.RemoveAt(n--);
                            }
                        }
                    }
                }

                EditorGUILayout.LabelField("\n");


            }

            EditorGUILayout.LabelField("\n");

            EditorGUILayout.LabelField("\n");
            if (GUILayout.Button("등장 적 생성"))
            {
                stage.data.SpawnCharacterList.Add(new CharacterData());
                stage.data.spawn.Add(new spawnInformation(stage.data.SpawnCharacterList.Count - 1));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.LabelField("\n");
        spawnfolder = EditorGUILayout.BeginFoldoutHeaderGroup(spawnfolder, "적 생성 정보");
        if (spawnfolder)
        {
            for (int i = 0; i < spawn.arraySize; i++)
            {
                var spawndata = spawn.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField(i + 1 + "번 생성 정보");

 
                EditorGUILayout.LabelField( stage.data.SpawnCharacterList[spawndata.FindPropertyRelative("index").intValue].id);
                EditorGUILayout.PropertyField(spawndata.FindPropertyRelative("number"), new GUIContent("생성 수"));
                spawndata.FindPropertyRelative("round").intValue=EditorGUILayout.IntSlider("등장 라운드",spawndata.FindPropertyRelative("round").intValue, 1, stage.data.waveNumber);


                if (GUILayout.Button("삭제"))
                {

                    stage.data.spawn.RemoveAt(i);

                }

            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("\n");
            RewardListfolder = EditorGUILayout.BeginFoldoutHeaderGroup(RewardListfolder, "흭득 가능한 아이템");
            if (RewardListfolder)
            {
                for (int i = 0; i < RewardList.arraySize; i++)
                {
                    var rewarddata = RewardList.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField(i + 1 + "번 아이템");
                    var item = rewarddata.FindPropertyRelative("item");
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("itemid"), new GUIContent("ID"));
                ItemData d = null ;
                    if ((d=manager.LoadItemData(item.FindPropertyRelative("itemid").stringValue) )== null)
                    {
                        EditorGUILayout.LabelField("id와 일치하는 아이템을 발견할 수 없습니다!");
                        EditorGUILayout.LabelField("이 데이터를 저장하지 않습니다", g);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(d.Name);
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("Amount"), new GUIContent("갯수"));
                        EditorGUILayout.PropertyField(rewarddata.FindPropertyRelative("per"), new GUIContent("등장 확률"));
                    }
                    EditorGUILayout.LabelField("\n");

                }
            
            EditorGUILayout.LabelField("\n");

            if (GUILayout.Button("드랍 아이템 생성"))
            {
                stage.data.rewardlist.Add(new StageReward());
            }
            }
       
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.LabelField("\n");
        missionfolder = EditorGUILayout.BeginFoldoutHeaderGroup(missionfolder, "미션 종류");
        if (missionfolder)
        {
            if(GUILayout.Button("미션 추가"))
            {
                stage.data.missionarray.Add(new StageMission());
            }
            for(int n = 0; n < Missions.arraySize; n++)
            {
                EditorGUILayout.PropertyField(Missions.GetArrayElementAtIndex(n).FindPropertyRelative("mission")
                    , new GUIContent($"{n}번째 미션 종류"));
                
            }
        }
        EditorGUILayout.LabelField("\n");

        EditorGUILayout.PropertyField(Terrain, new GUIContent("사용할 테라인 프리팹"));
        EditorGUILayout.LabelField("\n");
     



        if (GUILayout.Button("맵 데이터 생성"))
            {
                bool chk = true;
                foreach (CharacterData data in stage.data.SpawnCharacterList)
                {
                    if (manager.readCharacterinformation(data.id) == null)
                    {
                        chk &= false;
                    }
                }
            foreach (StageReward data in stage.data.rewardlist)
            {
                if (manager.LoadItemData(data.item.itemid) == null)
                {
                    chk &= false;
                }
            }

            if (chk)
                {
                    string s = PlayerSettings.bundleVersion;
                    manager.createStageinfo(stage.data, s);
                EditorGUILayout.LabelField("데이터 생성 성공!");
            }
                else
                {
                    EditorGUILayout.LabelField("데이터 생성 실패!", g);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    
    }
