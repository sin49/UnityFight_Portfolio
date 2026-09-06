using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    GameCharacter target_gc;
    public float speed_weight;

    float z_move;
    public Vector3 CameraPosition = new Vector3(0, 10, 0);
    public Vector3 CameraRotation= new Vector3(60, 0, 0);
    private void Update()
    {
        this.transform.rotation= Quaternion.Euler(CameraRotation);
    }
    private void OnEnable()
    {
        this.transform.position = CameraPosition;
        this.transform.rotation = Quaternion.Euler(CameraRotation);
    }
    private void Start()
    {
        StartCoroutine(CameraMoveOnStage()); 
    }
    public IEnumerator CameraMoveOnStage()
    {
        
        while (true)
        {
            yield return null;
            
            target_gc = PrioritySystem.Character_Priority.instance.get_Character_by_distance_farest(Team.Player, this.transform.position);
            if (target_gc == null)
            {
                Debug.Log("can't find Target");
                break;
            }
            Stage._stage.set_CenterPoint_viewport();
            if (target_gc.transform.position.z - Stage._stage.screenCenter.z > 0/*&&!Stage._stage.WaveCleared*/)
            {

                float y_value = this.transform.position.y;
                z_move = (target_gc.transform.position.z - Stage._stage.screenCenter.z) * speed_weight * Time.deltaTime;
                this.transform.Translate(0, 0, z_move,Space.World);
               //처음에는 좌표계를 회전시켜서 회전축 맞춘후 translate했는데
               //카메라의 회전 값이 바뀌니 계산이 너무 복잡해져서 space.world를 이용하기로 함
            }
        }
    }
   

}
