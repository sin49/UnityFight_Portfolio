using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PrioritySystem
{
    public class Distance_priority : Character_Priority
    {
        public Distance_priority(List<GameCharacter> Gc)
        {
            foreach (GameCharacter chr in Gc)
            {
                gc.Add(new Distance_Member(chr));
            }
            gc.Sort();

        }
        public Distance_priority(List<GameCharacter> Gc, Vector3 V)
        {
            foreach (GameCharacter chr in Gc)
            {
                gc.Add(new Distance_Member(chr, V));
            }
            gc.Sort();

        }
    }
}
