using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameramoveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //z_move = (target_gc.transform.position.z - Stage._stage.screenCenter.z) * speed_weight * Time.deltaTime;
        //y축을 30도 회전
        transform.Translate(Vector3.forward * Time.deltaTime, Space.World);
        //this.transform.position = new Vector3(this.transform.position.x, 10, this.transform.position.z);

    }
}
