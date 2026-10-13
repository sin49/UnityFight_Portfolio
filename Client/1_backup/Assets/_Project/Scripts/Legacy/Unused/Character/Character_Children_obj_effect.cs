using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Children_obj_effect : MonoBehaviour, Character_observer
{
    ColorShaderManager shader;
    Rigidbody rgd;
    public bool Object_flying;
    Vector3 localPosition;
    Quaternion localrotation;
    Transform parent;
    Transform t;
    Vector3 localscale;
    public void update_information(int a, GameCharacter character)
    { 
        if (a <= 0)
        {
           /* if (shader.Transparent_Material == null)
                shader.Transparent_Material = character.Transparent_Material;*/
            if (Object_flying) {
                rgd.useGravity = true;
                rgd.constraints = RigidbodyConstraints.None;
                rgd.transform.SetParent(this.transform.parent.parent);
                rgd.AddForce((-1*this.transform.parent.forward+Vector3.up)*0.000035f);
            }
           
                shader.active_Transparent_Shader(character.deadbody_duration);
      
        }
        
    }
    private void Awake()
    {
        
        
        GameCharacter chr = this.transform.parent.parent.parent.GetComponent<GameCharacter>();
        chr.AddObserver(this);
        if (Object_flying) {
            rgd = this.GetComponent<Rigidbody>();
            localrotation = this.transform.localRotation;
            localPosition = this.transform.localPosition;
            localscale = this.transform.localScale;
            parent = this.transform.parent;
            rgd.useGravity = false;
            
        }
        }
    private void Update()
    {
        if (Object_flying&& !parent.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        //initialize_children();
        
    }
    private void OnEnable()
    {
        initialize_children();
    }
    public void initialize_children()
    {
        if(shader==null)
            shader = new ColorShaderManager(this.gameObject);
        shader.set_original_shader();
        if (Object_flying)
        {
            this.transform.SetParent(parent);
            this.transform.localRotation = localrotation;
            this.transform.localPosition = localPosition;
            this.transform.localScale = localscale;
            rgd.constraints = RigidbodyConstraints.FreezeAll;
            rgd.useGravity = false;

        }
    }
}
