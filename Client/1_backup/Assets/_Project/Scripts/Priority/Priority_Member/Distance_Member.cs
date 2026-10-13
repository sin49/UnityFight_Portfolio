using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrioritySystem
{
    public class Distance_Member : Priority_Member
    {
        //vector3.zero로부터의 거리 순으로
        public Distance_Member(GameCharacter c) : base(c)
        {
            _priority_point = (Vector3.zero - c.transform.position).magnitude;
        }
        //v로부터 거리 순으로
        public Distance_Member(GameCharacter c, Vector3 v) : base(c)
        {
            _priority_point = (v - c.transform.position).magnitude;
        }
    }
    public class HP_Member: Priority_Member
    {
        public HP_Member(GameCharacter c) : base(c)
        {
            _priority_point =c.current_hp;
        }
    }
}
