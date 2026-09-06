using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : GameCharacter
{
    MeleeAttack_normal attack = new MeleeAttack_normal();
    Player1_Skill1 skill1=new Player1_Skill1();
    public Player1()
    {
       
    }
    protected override void OnEnable()
    {
        ATK = 30;
        base.OnEnable();
        
        //Character_action normal = new Character_action("Normal", attack, T, Character_Priority.instance.get_enemy_by_distance, new List<int> { 0 });
        //Character_action Skill1 = new Character_action("Skill1", skill1, T, Character_Priority.instance.get_enemy_by_distance, new List<int> { 0 });
        //action_queue.Enqueue(normal);
        //action_queue.Enqueue(Skill1);
        //action_queue.Enqueue(normal);
    }
    private void OnDisable()
    {
        action_queue.Clear();
    }
}
