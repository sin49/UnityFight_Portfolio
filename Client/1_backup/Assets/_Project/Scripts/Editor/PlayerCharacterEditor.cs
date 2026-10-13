using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using DataExecute;
using System;

[CustomEditor(typeof(GameCharacter)),CanEditMultipleObjects]
public class PlayerCharacterEditor: Editor
{
    UserDataManager u;
    GameCharacter c;
    SerializedProperty portrait;
    SerializedProperty currenthp;
    SerializedProperty currentatk;
    SerializedProperty isshader;
    SerializedProperty material;
    SerializedProperty color;
    SerializedProperty isanimation;
    SerializedProperty this_ani_controller;
    SerializedProperty ID;
    SerializedProperty LV;
    SerializedProperty chrname;
    SerializedProperty grade;
    GetActionType GetActionType;
    SerializedProperty info;
    SerializedProperty normal;
    SerializedProperty skill;
    SerializedProperty LB;
    bool lbcheck;
    bool Characterlock;
    private void OnEnable()
    {
        GetActionType = new GetActionType();
        c = target as GameCharacter;
        u = new UserDataManager();
        if (c.information==null||c.information.LB == null || GetActionType.checkType(c.information.LB.action) == false)
        {
            lbcheck = false;
        }
        else
        {
            lbcheck = true;
        }
        LV = serializedObject.FindProperty("LV");
        ID = serializedObject.FindProperty("ID");
        chrname = serializedObject.FindProperty("chr_name");
        grade = serializedObject.FindProperty("grade");
       
        currenthp = serializedObject.FindProperty("current_hp");
        currentatk = serializedObject.FindProperty("current_atk");
    
        portrait = serializedObject.FindProperty("chr_sprite");
        isshader = serializedObject.FindProperty("isShader");
        material = serializedObject.FindProperty("material");
        color= serializedObject.FindProperty("_Color");
        isanimation = serializedObject.FindProperty("is_animation");
        this_ani_controller= serializedObject.FindProperty("this_ani_controller");

        ID.stringValue = CharacterContentMapping.Canonicalize(ID.stringValue);
        c.information = SetPlayerActInterface(ID.stringValue);
        info = serializedObject.FindProperty("_information");
        normal = info.FindPropertyRelative("normalattack");
        skill = info.FindPropertyRelative("skills");
        LB = info.FindPropertyRelative("LB");
        if (c.information.normalattack.ActionDesc == null)
            c.information.normalattack.ActionDesc = new List<ActionClass>();
        if (c.information.LB.ActionDesc == null)
            c.information.LB.ActionDesc = new List<ActionClass>();
        Characterlock = false;


    }
    void MakeSkillClassOnGUI(SerializedProperty property)
    {
        EditorGUILayout.PropertyField(property.FindPropertyRelative("ActionDesc"), new GUIContent("행동 유형"));
        if (property.FindPropertyRelative("ActionDesc").enumValueIndex == (int)ActionDescription.Buff)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("bufftype"), new GUIContent("효과 종류"));

            EditorGUILayout.LabelField("효과 위력은 효과에 따라 사용되지 않을 수도 있음");
          EditorGUILayout.PropertyField(property.FindPropertyRelative("figure"), new GUIContent("효과 위력"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("figureByMaxLV"), new GUIContent("최대 레벨 효과 위력"));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("EffectPower"), new GUIContent("지속 시간(s)"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("EffectPowerByMaxLV"), new GUIContent("레벨 별 상승 지속 시간(s)"));
        }
        else if (property.FindPropertyRelative("ActionDesc").enumValueIndex == (int)ActionDescription.Debuff)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("debufftype"), new GUIContent("효과 종류"));

            EditorGUILayout.LabelField("효과 위력은 효과에 따라 사용되지 않을 수도 있음");
            EditorGUILayout.PropertyField(property.FindPropertyRelative("figure"), new GUIContent("효과 위력"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("figureByMaxLV"), new GUIContent("최대 레벨 효과 위력"));

            EditorGUILayout.PropertyField(property.FindPropertyRelative("EffectPower"), new GUIContent("지속 시간(s)"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("EffectPowerByMaxLV"), new GUIContent("레벨 별 상승 지속 시간(s)"));
        }
        else
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("figure"), new GUIContent("배율(%)"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("figureByMaxLV"), new GUIContent("최대 레벨  배율(%)"));
        }
        EditorGUILayout.PropertyField(property.FindPropertyRelative("target_team"), new GUIContent("목표 팀"));

        EditorGUILayout.PropertyField(property.FindPropertyRelative("attacktype"), new GUIContent("공격 종류"));
        if(property.FindPropertyRelative("attacktype").enumValueIndex!=(int)AttackType.single)
            EditorGUILayout.PropertyField(property.FindPropertyRelative("attacksize"), new GUIContent("공격 범위"));

        EditorGUILayout.PropertyField(property.FindPropertyRelative("priority"), new GUIContent("우선순위 정렬기준"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("PrioritySort"), new GUIContent("우선순위 목표기준"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("TargetMemberNumber"), new GUIContent("목표 대상 수"));

      
    }
    public override void OnInspectorGUI()
    {
      
        serializedObject.Update();
        GUIStyle bold = new GUIStyle();
        bold.fontStyle = FontStyle.Bold;
       
        bold.normal.textColor = Color.white;
        EditorGUILayout.LabelField("캐릭터", bold);
       
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"), new GUIContent("아이디"));
        if (EditorGUI.EndChangeCheck())
        {
           ID.stringValue = CharacterContentMapping.Canonicalize(ID.stringValue);
           SetPlayerActInterface(ID.stringValue);
        }
        if (GUILayout.Button("초기화"))
        {
    
            c.information = new CharacterInformation();
            c.chr_name = "";
            c.ID = "";
            
        }

        EditorGUILayout.PropertyField(chrname, new GUIContent("이름"));
        EditorGUILayout.LabelField("설명");
        info.FindPropertyRelative("Desc").stringValue = EditorGUILayout.TextArea(info.FindPropertyRelative("Desc").stringValue);
        EditorGUILayout.PropertyField(portrait, new GUIContent("초상화"));
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("능력치", bold);
        EditorGUILayout.LabelField("레벨:" +LV.intValue.ToString());
        EditorGUILayout.LabelField("등급:" + grade.intValue.ToString());
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("최대 체력:" + info.FindPropertyRelative("BaseHP").intValue.ToString() + $"+({info.FindPropertyRelative("HP_Grownth").floatValue * LV.intValue})");
        EditorGUILayout.LabelField("현재 체력:" + currenthp.intValue.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("최대 공격력:" + info.FindPropertyRelative("BaseATK").intValue.ToString() + $"+({info.FindPropertyRelative("ATK_Grownth").floatValue * LV.intValue})");
        EditorGUILayout.LabelField("현재 공격력:" + currentatk.intValue.ToString());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("사거리:" + info.FindPropertyRelative("distancenumber").intValue.ToString());
        EditorGUILayout.LabelField("포지션:" + info.FindPropertyRelative("position").enumDisplayNames[info.FindPropertyRelative("position").enumValueIndex]);
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("능력치 설정", bold);
        GUILayout.BeginHorizontal();
  
       EditorGUILayout.PropertyField(LV, new GUIContent("레벨"));
        EditorGUILayout.PropertyField(grade, new GUIContent("등급"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField( info.FindPropertyRelative("BaseHP"), new GUIContent("체력 초기치"));
       
         EditorGUILayout.PropertyField(info.FindPropertyRelative("HP_Grownth"), new GUIContent("체력 성장치"));

        GUILayout.EndHorizontal();
       
        GUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(info.FindPropertyRelative("BaseATK"), new GUIContent("공격력 초기치"));
        EditorGUILayout.PropertyField(info.FindPropertyRelative("ATK_Grownth"), new GUIContent("공격력 성장치"));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        info.FindPropertyRelative("distancenumber").intValue = EditorGUILayout.IntSlider("사거리", info.FindPropertyRelative("distancenumber").intValue, 2, 50);
     
        if (EditorGUI.EndChangeCheck())
        {
           
            if (info.FindPropertyRelative("distancenumber").intValue <= 8)
                info.FindPropertyRelative("position").enumValueIndex =(int)Position.front;
            else if (info.FindPropertyRelative("distancenumber").intValue <= 16)
                info.FindPropertyRelative("position").enumValueIndex = (int)Position.mid;
            else
                info.FindPropertyRelative("position").enumValueIndex = (int)Position.back;
          
        }
        c.T = (Team)EditorGUILayout.EnumPopup("팀",c.T);
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("\n");


        EditorGUILayout.LabelField("행동", bold);
        EditorGUILayout.LabelField("\n");
       



            EditorGUILayout.PropertyField(normal.FindPropertyRelative("action"), new GUIContent("함수를 입력(기본 값(attack_basic))"));

                if (GetActionType.checkType(normal.FindPropertyRelative("action").stringValue) == false)
                {
                    EditorGUILayout.LabelField("함수 탐색 실패");
            }
            else
            {
               
                 GUILayout.BeginHorizontal();

            normal_folder = EditorGUILayout.BeginFoldoutHeaderGroup(normal_folder, "일반 공격" + ":" + normal.FindPropertyRelative("action").stringValue);
            if (GUILayout.Button("추가"))
            {
                    c.information.actionset.Add("normal");
            }
            GUILayout.EndHorizontal();
            if (normal_folder)
            {
                    EditorGUILayout.PropertyField(normal.FindPropertyRelative("skillname"), new GUIContent("이름"));
                    EditorGUILayout.LabelField("설명");
                    normal.FindPropertyRelative("skilldescription").stringValue=  EditorGUILayout.TextArea( normal.FindPropertyRelative("skilldescription").stringValue, GUILayout.MinHeight(80));

                    EditorGUILayout.PropertyField(normal.FindPropertyRelative("init_time"), new GUIContent("시전 시간"));
                    SerializedProperty List = normal.FindPropertyRelative("ActionDesc");
            
                if (GUILayout.Button("기능 추가"))
                    {
                        c.information.normalattack.ActionDesc.Add(new ActionClass());
                    }

                    


                    for(int n = 0; n < List.arraySize; n++)
                    {
                        EditorGUILayout.LabelField("\n");
                        EditorGUILayout.LabelField($"{n+1} 번 기능");
                        MakeSkillClassOnGUI(List.GetArrayElementAtIndex(n));
                    if (GUILayout.Button("기능 삭제"))
                    {
                        c.information.normalattack.ActionDesc.RemoveAt(n);
                    }
                    EditorGUILayout.LabelField("\n");
                    }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            }


        EditorGUILayout.LabelField("\n");
        EditorGUI.BeginChangeCheck();
        skill.arraySize= EditorGUILayout.IntSlider(new GUIContent("스킬 갯수"), skill.arraySize,0,10);
       
        if (skill.arraySize != 0)
        {
            if (skill_folder==null||skill_folder.Length != skill.arraySize)
                skill_folder = new bool[skill.arraySize];
            for (int i = 0; i < skill.arraySize; i++)
            {
                if (skill.GetArrayElementAtIndex(i).FindPropertyRelative("action").stringValue == "")
                {
                    skill.GetArrayElementAtIndex(i).FindPropertyRelative("action").stringValue = "attack_basic";
                }
                EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("action"), new GUIContent($"{i + 1}번 스킬의 함수를 입력"));
                if (GetActionType.checkType(skill.GetArrayElementAtIndex(i).FindPropertyRelative("action").stringValue) == false)
                {
                    EditorGUILayout.LabelField("함수 탐색 실패");
                }
                else
                {
           
                GUILayout.BeginHorizontal();

                skill_folder[i] = EditorGUILayout.BeginFoldoutHeaderGroup(skill_folder[i], "스킬" + (i + 1) + ":" + skill.GetArrayElementAtIndex(i).FindPropertyRelative("action").stringValue);

                if (GUILayout.Button("추가"))
                {
                    c.information.actionset.Add("skill" + (i + 1));
                }

                GUILayout.EndHorizontal();
                if (skill_folder[i])
                {
                        EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("skillname"), new GUIContent("이름"));
                        EditorGUILayout.LabelField("설명");
                        
                        skill.GetArrayElementAtIndex(i).FindPropertyRelative("skilldescription").stringValue = EditorGUILayout.TextArea(skill.GetArrayElementAtIndex(i).FindPropertyRelative("skilldescription").stringValue, GUILayout.MinHeight(80));
                        EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("ActionType"), new GUIContent("타입"));
                        EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("init_time"), new GUIContent("시전 시간"));

                        SerializedProperty List = skill.GetArrayElementAtIndex(i).FindPropertyRelative("ActionDesc");
                        if (GUILayout.Button("기능 추가"))
                        {
                            c.information.skills[i].ActionDesc.Add(new ActionClass());
                        }
                       



                        for (int n = 0; n < List.arraySize; n++)
                        {
                            EditorGUILayout.LabelField("\n");
                            EditorGUILayout.LabelField($"{n + 1} 번 기능");
                            MakeSkillClassOnGUI(List.GetArrayElementAtIndex(n));
                            if (GUILayout.Button("기능 삭제"))
                            {
                                c.information.skills[i].ActionDesc.RemoveAt(n);
                            }
                            EditorGUILayout.LabelField("\n");
                        }

                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("AtkPercent"), new GUIContent("공격력배율(%)"));
                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("ATKUpgPer"), new GUIContent("레벨 별 상승 배율(%)"));
                        //skill.GetArrayElementAtIndex(i).FindPropertyRelative("skillLV").intValue = EditorGUILayout.IntSlider(new GUIContent("스킬 레벨"), skill.GetArrayElementAtIndex(i).FindPropertyRelative("skillLV").intValue, 1, 10);
                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("target_team"), new GUIContent("목표 팀"));


                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("priority"), new GUIContent("우선순위 정렬기준"));
                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("PrioritySort"), new GUIContent("우선순위 목표기준"));
                        //EditorGUILayout.PropertyField(skill.GetArrayElementAtIndex(i).FindPropertyRelative("TargetMemberNumber"), new GUIContent("목표 대상 수"));

                    }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            }
        }else
            EditorGUILayout.LabelField("스킬 없음");

        EditorGUILayout.LabelField("\n");
        lbcheck= GUILayout.Toggle(lbcheck, "필살기 사용 여부");
        if (lbcheck)
        {
            EditorGUILayout.PropertyField(LB.FindPropertyRelative("action"), new GUIContent("필살기 함수를 입력(기본 값:attack_basic)"));
            if (GetActionType.checkType(LB.FindPropertyRelative("action").stringValue) == false)
            {
                EditorGUILayout.LabelField("함수 탐색 실패");
            }
            else
            {

                GUILayout.BeginHorizontal();

                lbfolder = EditorGUILayout.BeginFoldoutHeaderGroup(lbfolder, "필살기" + ":" + LB.FindPropertyRelative("action").stringValue);

                GUILayout.EndHorizontal();
                if (lbfolder)
                {
                    EditorGUILayout.PropertyField(LB.FindPropertyRelative("skillname"), new GUIContent("이름"));
                    EditorGUILayout.LabelField("설명");
                    LB.FindPropertyRelative("skilldescription").stringValue= EditorGUILayout.TextArea(LB.FindPropertyRelative("skilldescription").stringValue, GUILayout.MinHeight(80));
                    EditorGUILayout.PropertyField(LB.FindPropertyRelative("ActionType"), new GUIContent("타입"));
                    EditorGUILayout.PropertyField(LB.FindPropertyRelative("init_time"), new GUIContent("시전 시간"));
                    SerializedProperty List = LB.FindPropertyRelative("ActionDesc");
                    if (GUILayout.Button("기능 추가"))
                    {
                        c.information.LB.ActionDesc.Add(new ActionClass());
                    }




                    for (int n = 0; n < List.arraySize; n++)
                    {
                        EditorGUILayout.LabelField("\n");
                        EditorGUILayout.LabelField($"{n + 1} 번 기능");
                        MakeSkillClassOnGUI(List.GetArrayElementAtIndex(n));
                        if (GUILayout.Button("기능 삭제"))
                        {
                            c.information.LB.ActionDesc.RemoveAt(n);
                        }
                        EditorGUILayout.LabelField("\n");
                    }

                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("AtkPercent"), new GUIContent("공격력배율(%)"));
                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("ATKUpgPer"), new GUIContent("레벨 비례 최대 상승 배율(%)"));
                    //LB.FindPropertyRelative("skillLV").intValue = EditorGUILayout.IntSlider(new GUIContent("스킬 레벨"), LB.FindPropertyRelative("skillLV").intValue, 1, 10);
                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("target_team"), new GUIContent("목표 팀"));

                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("priority"), new GUIContent("우선순위 정렬기준"));
                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("PrioritySort"), new GUIContent("우선순위 목표기준"));
                    //EditorGUILayout.PropertyField(LB.FindPropertyRelative("TargetMemberNumber"), new GUIContent("목표 대상 수"));

                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else
            EditorGUILayout.LabelField("필살기 없음");

        EditorGUILayout.LabelField("\n");
        string actionset = "";
        foreach(string s in c.information.actionset)
        {
            actionset += s+"-";
        }
        if (actionset.Length != 0)
            actionset = actionset.Substring(0, actionset.Length - 1);
        EditorGUILayout.LabelField("행동 순서:" +actionset);
        if (actionset.Length != 0)
        {
            if (GUILayout.Button("행동 빼기"))
            {
                c.information.actionset.RemoveAt(c.information.actionset.Count-1);
            }
        }
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("쉐이더 설정", bold);
        EditorGUILayout.PropertyField(isshader,new GUIContent("쉐이더"));
        if (isshader.boolValue)
        {
            EditorGUILayout.PropertyField(material, new GUIContent("메터리얼"));

            EditorGUILayout.PropertyField(color, new GUIContent("색깔"));
        }

        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("에니메이션 설정", bold);
        EditorGUILayout.PropertyField(isanimation, new GUIContent("에니메이션"));
        if (isanimation.boolValue)
        {
          
            EditorGUILayout.PropertyField(this_ani_controller, new GUIContent("에니메이션 무브"));

        }
        EditorGUILayout.LabelField("\n");
        if (GUILayout.Button("데이터 만들기"))
        {
             this.c.information.normalattack.action_name = "normal";
            for(int i=0;i < this.c.information.skills.Length; i++)
            {
                this.c.information.skills[i].action_name = "Skill" + (i + 1);
            }
           
            this.c.information.LB.action_name = "LB";
            CharacterInformation c = new CharacterInformation()
            {
  
                BaseHP = info.FindPropertyRelative("BaseHP").intValue,
                BaseATK =  info.FindPropertyRelative("BaseATK").intValue,
                HP_Grownth = info.FindPropertyRelative("HP_Grownth").floatValue,
                ATK_Grownth = info.FindPropertyRelative("ATK_Grownth").floatValue ,
                distancenumber = info.FindPropertyRelative("distancenumber").intValue,
                BaseGrade =  grade.intValue
                ,
                position=(Position)info.FindPropertyRelative("position").intValue
                ,
                
                id = CharacterContentMapping.Canonicalize(ID.stringValue),
                name=chrname.stringValue,
                Desc=info.FindPropertyRelative("Desc").stringValue
                ,
                normalattack =  this.c.information.normalattack,
                skills = this.c.information.skills,
                LB =lbcheck ? this.c.information.LB: new Character_action(),
                actionset = this.c.information.actionset
            };
            u.createChracterInformation(c);

        }
        Characterlock = EditorGUILayout.Toggle("캐릭터 잠금 설정", Characterlock);
        if (GUILayout.Button("플레이어블 캐릭터 데이터 생성"))
        {

            PlayerData d = new PlayerData
            {
                Grade = grade.intValue,
                LV = 1
                ,SpecialLV=1,
                skillLV=new int[skill.arraySize]
                ,
                id = CharacterContentMapping.Canonicalize(ID.stringValue),
          
                characterlock = Characterlock
            };
            for (int n = 0; n < d.skillLV.Length; n++)
            {
                d.skillLV[n] = 1;
            }
            string s = PlayerSettings.bundleVersion;
            u.UpdateUserAccountTemplet(d,s);
           

            
    }
      
        // base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
    bool normal_folder;
    bool[] skill_folder;
    bool lbfolder;
   
    public CharacterInformation SetPlayerActInterface(string s)
    {

       CharacterInformation info = u.readCharacterinformation(s);
        if (info != null)
        {
            return info;
        }
        else
        {

            return new CharacterInformation();
        }

    }
   
}

