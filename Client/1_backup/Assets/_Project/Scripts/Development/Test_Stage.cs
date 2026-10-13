using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Stage : MonoBehaviour
{
    public List<GameCharacter> Players;
    public List<GameCharacter> enemies;
    private void Awake()
    {
        foreach (var player in Players) {
            player.T = Team.Player;
            player.active_obj();
        }
        foreach (var enemy in enemies)
        {
            enemy.T = Team.Enemy;
            enemy.active_obj();
        }
    }
}
