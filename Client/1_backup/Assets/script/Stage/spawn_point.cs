using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_point : MonoBehaviour
{

  public  Transform parenttransform;
    float spawn_distance_meter_x=3;
    //float spawn_distance_meter_y=0.5f;
    public int index;
  
   
    public virtual void spawn(GameCharacter c)
    {
        if(c.gameObject.activeSelf)
            c.gameObject.SetActive(false);
        c.transform.SetParent(parenttransform);
        //distance에 따라 배치
        /*  switch (c.this_pos)
          {
              case Position.front:
                  c.transform.position = this.transform.position + Vector3.forward * spawn_distance_meter_y;
                  break;
              case Position.mid:
                  c.transform.position = this.transform.position;
                  break;
              case Position.back:
                  c.transform.position = this.transform.position - Vector3.forward * spawn_distance_meter_y;
                  break;
          }*/

        c.transform.position = this.transform.position+Vector3.up*0.1f + Vector3.right * ((spawn_distance_meter_x * ((index + 1) / 2)) * (-1 * index % 2));
        c.transform.rotation = this.transform.rotation;
        c.gameObject.SetActive(true);
       
        index++;
        
    }
    public void reset_spawn_index()
    {
        index = 0;
    }
   
}
