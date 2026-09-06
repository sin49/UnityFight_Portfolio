using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_point_Player : spawn_point
{
    
    public Transform[] SpawnedPlayer=new Transform[5];
    public override void spawn(GameCharacter c)
    {
        if (c.gameObject.activeSelf)
            c.gameObject.SetActive(false);
        c.transform.SetParent(parenttransform);
        c.transform.position = SpawnedPlayer[index].position;
            c.transform.rotation = this.transform.rotation;
        index++;
    }
}
