using UnityEngine;

public class ModelingPin : MonoBehaviour
{
    public Vector3 PinVec3;
   
    void Update()
    {
        transform.position = PinVec3;
    }
}
