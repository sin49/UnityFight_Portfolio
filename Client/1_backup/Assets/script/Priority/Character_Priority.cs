using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrioritySystem
{
    public class Character_Priority
    {
        static Character_Priority _instance;
        public static Character_Priority instance { get { if (_instance == null) _instance = new Character_Priority(); return _instance; } }
        public List<GameCharacter> P = new List<GameCharacter>();
        public List<GameCharacter> E = new List<GameCharacter>();
        public List<GameCharacter> Target = new List<GameCharacter>();
        protected List<Priority_Member> gc = new List<Priority_Member>();

       
        public List<GameCharacter> get_Characters(Priority p, PrioritySort m, int MemberNumber, GameCharacter obj, Team t,AttackType type,float attacksize)
        {
            if (p == Priority.self)
                return new List<GameCharacter>() { obj };
            var a = t;
            if (obj.T == Team.Enemy)
            {
                if (t == Team.Enemy)
                    t = Team.Player;
                else
                    t = Team.Enemy;
            }
            Debug.Log($"{obj.name}이 우선순위 탐색 알고리즘을 실행\n 팀은 {obj.T} 이기에 {t}를 탐색\n  입력갑은 {a}");
            initialize_List(t);
            List<GameCharacter> targets;
            switch (p)
            {
                case Priority.Distance:
                    targets= get_Character_by_distance(t, obj.transform.position, m, MemberNumber);
                    break;
                case Priority.HP:
                    targets = get_Character_by_hp(t, m, MemberNumber);
                    break;
                default:
                    targets= null;
                    break;
            }
            targets = getCharacterByRange(targets, type, attacksize);
            return targets;
        }
        public GameCharacter get_Character_by_distance_nearest(Team t, Vector3 pos)
        {

            initialize_List(t);
            if (Target.Count == 0)
            {
                return null;
            }
             
            for (int i = 0; i < Target.Count; i++)
            {
                Distance_Member memeber = new Distance_Member(Target[i], pos);
                gc.Add(memeber);
            }
            if (gc.Count == 0)
                return null;
            gc.Sort();
           

            return gc[0].Character;

        }
        public GameCharacter get_Character_by_distance_farest(Team t, Vector3 pos)
        {

            initialize_List(t);
            if (Target.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < Target.Count; i++)
            {
                Distance_Member memeber = new Distance_Member(Target[i], pos);
                gc.Add(memeber);
            }
            if (gc.Count == 0)
                return null;
            gc.Sort();
            gc.Reverse();

            return gc[0].Character;

        }
        List<GameCharacter> getCharacterByRange(List<GameCharacter> tmp,AttackType type, float attacksize)
        {
            if (type == AttackType.single)
                return tmp;



            List<GameCharacter> value=new List<GameCharacter>();

            switch (type)
            {
                case AttackType.single:
                    break;
                case AttackType.Circlerange:
                    for (int i = 0; i < tmp.Count; i++)
                    {
                       for(int n = 0; n < Target.Count; n++)
                        {
                            if(tmp[i]==Target[n])
                                continue;
                            if ((tmp[i].transform.position - Target[n].transform.position).magnitude <= attacksize)
                            {
                                value.Add(Target[n]);
                            }
                            else
                                break;
                        }
                        value.Add(tmp[i]);
                    }
                    
                    break;
                
            }
            return value;
        }
        List<GameCharacter> get_Character_by_hp(Team t,  PrioritySort m, int membernumber)
        {
            initialize_List(t);
            if (Target.Count == 0)
                return null;
            for (int i = 0; i < Target.Count; i++)
            {
                HP_Member memeber = new HP_Member(Target[i]);
                gc.Add(memeber);
            }
            gc.Sort();
            if (gc.Count == 0)
                return null;

            return GetTargetMemberList(gc, m, membernumber);
        }
        List<GameCharacter> get_Character_by_distance(Team t, Vector3 pos, PrioritySort m, int membernumber)
        {

            initialize_List(t);
            if (Target.Count == 0)
                return null;
            for (int i = 0; i < Target.Count; i++)
            {
                Distance_Member memeber = new Distance_Member(Target[i], pos);
                gc.Add(memeber);
            }
            gc.Sort();
            if (gc.Count == 0)
                return null;

            return GetTargetMemberList(gc, m, membernumber);

        }
        List<GameCharacter> GetTargetMemberList(List<Priority_Member> List, PrioritySort m, int membernumber)
        {
            bool focusedchecker=false;
            List<GameCharacter> tmp = new List<GameCharacter>();
            List<Priority_Member> tmp2 = new List<Priority_Member>();
            foreach (Priority_Member chr in List)
            {
                if(chr.Character.focused)
                {
                    focusedchecker |= true;
                    tmp2.Add(chr);
                }    
            }
            if (!focusedchecker)
                tmp2 = List;
           
            switch (m)
            {
                case PrioritySort.highest:
                    for (int i = 0; i < membernumber; i++)
                    {
                        tmp.Add(tmp2[i].Character);
                    }
                    break;
                case PrioritySort.Lowest:
                    for (int i = 0; i < membernumber; i++)
                    {
                        tmp.Add(tmp2[i].Character);
                    }
                    break;
                case PrioritySort.All:
                    for (int i = 0; i < List.Count; i++)
                    {
                        tmp.Add(tmp2[i].Character);
                    }
                    break;
                case PrioritySort.RandomAndNotMultiple:
                    for (int i = 0; i < membernumber; i++)
                    {
                        int random = Random.Range(0, tmp2.Count);
                        tmp.Add(tmp2[random].Character);
                        tmp2.RemoveAt(random);
                    }
                    break;
                case PrioritySort.RandomAndMultiple:
                    for (int i = 0; i < membernumber; i++)
                    {
                        int random = Random.Range(0, tmp2.Count);
                        tmp.Add(tmp2[random].Character);
                    }
                    break;
                case PrioritySort.secondHighest:
                    for (int i = 0; i < membernumber; i++)
                    {
                        if (i + 1 > tmp2.Count)
                        {
                            tmp.Add(tmp2[i].Character);
                            break;
                        }
                        else
                            tmp.Add(tmp2[i + 1].Character);
                    }
                    break;
                case PrioritySort.SecondLowest:
                    List.Reverse();
                    for (int i = 0; i < membernumber; i++)
                    {
                        if (i + 1 > tmp2.Count)
                        {
                            tmp.Add(tmp2[i].Character);
                            break;
                        }
                        else
                            tmp.Add(tmp2[i + 1].Character);
                    }
                    break;
            }

            return tmp;
        }

        void initialize_List(Team t)
        {
            gc.Clear();
            if (t == Team.Player)
                Target = Stage._stage.PlayerCreated;
            else
                Target = Stage._stage.EnemyCreated;
            
        }

    }
}
