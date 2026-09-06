using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrioritySystem
{
    public class Priority_Member : IComparable<Priority_Member>
    {
        protected GameCharacter _character;
        public GameCharacter Character { get { return _character; } }
        protected float _priority_point;
        public float priority_point { get { return _priority_point; } }

        public Priority_Member(GameCharacter c)
        {
            _character = c;

        }
        public int CompareTo(Priority_Member priorityObj)
        {
            if (priorityObj == null)
            {
                return 1;
            }
            int n = this.priority_point.CompareTo(priorityObj.priority_point);
            return n;
        }

    }
}