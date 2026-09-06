using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack_normal : attack_basic
{

    GameObject bullet;
    range_bullet bullet_create;
    Transform bullet_create_transform;
    RangeAttack_normal(GameObject bullet,Transform bullet_create_transform){
        this.bullet = bullet;
        this.bullet_create_transform = bullet_create_transform;
        }

    protected override void attack_by_type(int ATK, List<GameCharacter> obj_list)
    {
        create_bullet(obj_list[0],ATK);
        base.attack_by_type(ATK, obj_list);
    }

  

    void create_bullet(GameCharacter Target,int Damage)
    {
        if (bullet == null)
            return;
        if (bullet_create == null)
        {
           // bullet_create = Instantiate(bullet, this.transform).GetComponent<range_bullet>();
        }
        bullet_create.set_bullet(Target, Damage);
            bullet_create.transform.position = bullet_create_transform.position;
            bullet_create.gameObject.SetActive(true);
    }
}
