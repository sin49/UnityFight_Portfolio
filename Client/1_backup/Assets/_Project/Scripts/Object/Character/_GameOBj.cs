using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _GameOBj : MonoBehaviour,GameObj
{
    protected virtual void Awake()
    {
        if ((rgd = this.gameObject.GetComponent<Rigidbody>()) == null)
        {
            rgd = this.gameObject.AddComponent<Rigidbody>();
            rgd.constraints = RigidbodyConstraints.FreezeRotation;
        }

    }
    protected bool checker()
    {
    //�켱����0��: ĳ���� Ȱ��ȭ ����
            if (!object_activasion)
        {
    
            return false;
        }
        //�켱����1��:ĳ���� ��� ����
        if (isDie)
        {
          
            foreach (Action a in _Destroy_handler)
            {
                a.Invoke();
            }
            return false;
        }
        if (rgd.linearVelocity.y < -0.03f&& rgd.linearVelocity.y > 0.03f)
        {
           
            return false;
        }
        //�켱���� 2��:����Ʈ ���ο� ���� ����
        if (check_viewport && !handle_object_by_viewport())
        {
  
            return false;
        }
        if (cant_handle_time >0)//���� �Ұ�
        {
           
foreach(Action<float> act in CantHandle_handler)
            {
                act.Invoke(cant_handle_time);
            }
            return false;
        }
        return true;
    }
  
    protected virtual void OnEnable()
    {
        Initialized();
        initialize_handler();
    }
  
    protected virtual void Initialized()//�ʱ�ȭ
    {

        object_activasion = false;
        velocity_buffer = Vector3.zero;
        deadbody_duration = 0;
        cant_handle_time = 0;
        check_viewport = true;
        isDie = false;
        on_viewport = false;
       
        rgd.linearVelocity = Vector3.zero;
        this.transform.position += Vector3.up * (this.GetComponent<CapsuleCollider>().height* 0.5f);
       
        if(isShader)
            initialize_shader();
    }
    #region Ȱ��ȭ
    public bool object_activasion;
    Vector3 velocity_buffer;
    public virtual void active_obj()
    {
        object_activasion = true;
        if(velocity_buffer != Vector3.zero)
            rgd.linearVelocity = velocity_buffer;
    }

    public virtual void deactive_chr()
    {
        object_activasion = false;
        velocity_buffer = rgd.linearVelocity;
        rgd.linearVelocity = Vector3.zero;
    }
    #endregion

    #region ����
    public float deadbody_duration { get; set; }//���� �ð�
    protected bool is_pulling;
    protected bool isDie;
   protected virtual void _Destroy()
    {
        if ((deadbody_duration -= Time.deltaTime) <= 0){
            if (is_pulling)
                this.gameObject.SetActive(false);
            else
                Destroy(this.gameObject);
        }
    }
    #endregion

    #region �����
    protected Rigidbody rgd;
    protected float speed=1;
  
   
    public virtual void forced(Vector3 vec, float forced_time)
    {
        rgd.linearVelocity = Vector3.zero;
        rgd.AddForce(-1 * vec, ForceMode.Impulse);
        cant_handle_time = forced_time;
    }

    #endregion
    #region ���� �Ұ�
    protected float cant_handle_time;
    public void set_cant_handle_time(float f)
    {
        cant_handle_time = f > cant_handle_time ? f : cant_handle_time;
    }
    #endregion
    #region ����Ʈó��
    protected bool on_viewport;//����Ʈ�� �����ϴ°�?
    bool check_viewport;//����Ʈ�� ������ �޴°�?
    void go_to_viewport()
    {
       
        Vector3 v = Vector3.zero;
            if (viewport_position.x > 0.95f)
            {
                v.x = -10*speed;
            }
            else if (viewport_position.x < 0.05f)
            {
                v.x = 10 * speed;
            }
            if (viewport_position.y > 1.05)
            {
                v.z = -10 * speed;
            }
            else if (viewport_position.y < 0.2)
            {
                v.z = 10 * speed;
            }
            rgd.linearVelocity=v;
       
            if (viewport_position.x <= 0.95f && viewport_position.x >= 0.05f && viewport_position.y >= 0.2 && viewport_position.y <= 1.05)
            {
                //rgd.velocity = Vector3.zero;
                on_viewport = true;
            }
        
    }
    Vector3 viewport_position;

    void snap_to_viewport()
    {
        if (viewport_position.x > 0.95f)
        {
            viewport_position.x = 0.95f;
        }
        else if (viewport_position.x < 0.05f)
        {
            viewport_position.x = 0.05f;
        }
        if (viewport_position.y > 1.05)
        {
            viewport_position.y = 1.05f;
        }
        else if (viewport_position.y < 0.2f)
        {
            viewport_position.y = 0.2f;
        }
        transform.position = Camera.main.ViewportToWorldPoint(viewport_position);
    }
    private bool handle_object_by_viewport()
    {
        viewport_position = Camera.main.WorldToViewportPoint(transform.position);
        //Debug.Log(viewport_position);
         if (on_viewport)
         {
             if (cant_handle_time > 0)
                 snap_to_viewport();
             return true;
         }
         else
         {
             go_to_viewport();
             return false;
         }
    }

    #endregion

    #region shader
    public bool isShader;
    public Material material;
    public Color _Color;
    public Material Transparent_Material;
    
   ColorShaderManager shader = null;
    protected virtual void initialize_shader(ColorShaderManager m=null)
    {
        
        if (shader == null)
        {
            if (m != null)
                shader = m;
            else
                shader = new ColorShaderManager(this.gameObject,true);

            shader.Transparent_Material = Transparent_Material;
            shader.accept_shader_setting(_Color, new Vector4(0, 0, 0, 255),material);
        }
        else
            shader.set_original_shader();
    }
   void Destroy_shader()
    {
        
        if(shader!=null)
        shader.active_Transparent_Shader(deadbody_duration);
    }
    #endregion
    #region �ڵ鷯
    protected List<Action> _Destroy_handler=new List<Action>();
    protected List<Action<float>> CantHandle_handler=new List<Action<float>>();
    protected virtual void initialize_handler()
    {
        _Destroy_handler.Clear();
        //CantHandle_handler.Clear();
        register_Handler();
        //if(isShader)
        //    register_shader_handler();
    }
    protected virtual void register_Handler()
    {
        _Destroy_handler.Add(_Destroy);
        CantHandle_handler.Add(new Action<float>((t) => { cant_handle_time -= Time.deltaTime; }));
    }
    protected virtual void register_shader_handler()
    {
        _Destroy_handler.Add(Destroy_shader);
    }
    protected void execute_handler(List<Action> handler)
    {
        foreach (Action a in handler)
        {
            a.Invoke();
        }
    }
    #endregion
}
