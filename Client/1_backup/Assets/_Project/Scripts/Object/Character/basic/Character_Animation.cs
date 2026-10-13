using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Character_Animation
{
    public Character_Animation(GameCharacter gc,RuntimeAnimatorController animator)
    {
        model = gc.transform.GetChild(0);
       ani = model.GetComponent<Animator>();

            ani.runtimeAnimatorController = animator;
    }
    Animator ani;
    Transform model;
    float timer;
    struct animation_buffer{
      
       public string animation_name;
        public float animation_length;
        }
    animation_buffer ani_buffer;
    Vector3 PosBuffer;
    Quaternion rotationBuffer;
  
    void returntransformBuffer()
    {

        model.localPosition = Vector3.zero;
        model.localRotation = Quaternion.identity;
        Debug.Log("Buffer return");

    }

    //필요한거 animator받기
    //핸들러에 에니메이션 실행 기능 추가하기

    public void initialize_animation()
    {
        ani_buffer = new animation_buffer();
        reset_all_parameter();
        //ani.Play("idle", 0, 0);

    }
    public void active_stun_timer_animator(float f)
    {
        ani.SetFloat("stun", f);
        
    }
    public void active_Move_animation()
    {
        ani.SetBool("OnMove", true);
    }
    public void active_Salute()
    {
        ani.Play("Salute");
    }
    public void active_Damaged_animation()
    {
        if (ani.GetCurrentAnimatorClipInfo(0).Length > 0 && ani.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
            ani.Play("Damaged");
    }
   public void active_death_animation()
    {
    
        ani.SetBool("Die", true);
        ani.Play("Die");

       
    }
    //에니메이션 속도 조절
    public void set_animation_speed(float s)
    {
        ani.speed = s;
    }
    //현재 에니메이션을 멈춘다
    public void stop_current_animation()
    {
      
       
        float t = ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
    
        ani_buffer.animation_length = t;
   
        ani.Play("idle",0,0);
    }
    //멈췄던 에니메이션을 다시 재생한다.
    public void resume_animation()
    {
        if (ani_buffer.animation_name == null)
            return;

        ani.Play(ani_buffer.animation_name, 0, ani_buffer.animation_length);
        ani_buffer.animation_name =null;
    }
    //특정 파라미터를 리셋한다
 public   void reset_parameter(string s)
    {

        switch (s)
        {
            case "stun":
                ani.SetFloat("stun", 0);
                break;
            case "Die":
                ani.SetBool("Die", false);
                break;
            case "OnMove":
                ani.SetBool("OnMove", false);
                break;
            //case "NotBattle":
            //    ani.SetBool("NotBattle", false);
            //        break;
            default:
                break;
        }
    }
    void reset_all_parameter()
    {
        ani.SetFloat("stun", 0);
        ani.SetBool("Die", false);

        ani.SetBool("OnMove", false);
    }
    public void active_action_animation(Character_action s)
    {
        try
        {
            ani.Play(s.action_name.ToLower(), 0, 0);
            timer = 1;
        }
        catch
        {
            Debug.Log($"Can't play animation:{s.action_name}");
        }
    }
 public bool resetmodeltransform()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if (timer < 0 && ani.GetCurrentAnimatorClipInfo(0).Length>0 && ani.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
                returntransformBuffer();
        }
        return false;
    }
}
//각도,포지션 무조건 0이 되도록 고치기
