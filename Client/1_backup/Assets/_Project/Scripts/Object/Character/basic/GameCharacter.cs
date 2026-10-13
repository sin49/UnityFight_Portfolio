
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class GameCharacter : _GameOBj, Character {


    protected override void Awake()
    {
        base.Awake();
        s = Stage._stage;

    }
    // Update is called once per frame
    protected void FixedUpdate()
    {

        if (!checker())
            return;
  

        execute_frame_handler();

        //if (is_LB_can_use_anywhere&&Auto_LB)
        //    use_LB();


            set_target();
        if (chasing_target != null)
        {
            if (target_distance > information.distancenumber)
            {
                execute_Handler(Move_handler);

            }

            else
            {
                is_target_on_distance = true;
                //if (!is_LB_can_use_anywhere && Auto_LB)
                //    use_LB();
                Character_Act();
            }
        }
        #region backup
        //set_target();//Ÿ�� ����
        //if (target_distance != 9999 && target_distance > information.distancenumber)
        //{

        //    is_target_on_distance = false;

        //    chase_target();

        //} else if (target_distance == 9999)
        //{

        //}
        //else
        //{

        //    is_target_on_distance = true;
        //    if (!is_LB_can_use_anywhere&&Auto_LB)
        //        use_LB();
        //    Character_Act();
        //}
        #endregion
    }
    #region ĳ����

    public string ID;
    public string chr_name;
    public Sprite chr_sprite;
    [SerializeField]
    int LV;
    [SerializeField]
    int grade;

    CharacterData data;
    #endregion
    #region �ɷ�ġ
    public List<StatusEffect> StautusEffectList;
    public CharacterInformation information { get { return _information; }set { _information = value; } }
    [SerializeField]
     CharacterInformation _information;
    public Team T { get; set; }
    public int MAX_hp;

    
    [SerializeField]
    protected int _ATK;
    public int ATK { get { return _ATK; } set { _ATK = value; } }
    public int current_atk;

    public int current_hp;

    public float HitteDamageNumber=1;

    #region ������ �ҷ�����
    public void set_Character(CharacterInformation c, CharacterData d)
    {
        CharacterContentMapping.NormalizeData(c);
        CharacterContentMapping.NormalizeData(d);
        chr_name = c.name;
        LV = d.LV;
        ID = CharacterContentMapping.Canonicalize(d.id);
     
        chr_sprite = d.charactersprite;
        _information = c;
        MAX_hp = information.BaseHP + Mathf.RoundToInt(LV * information.HP_Grownth * grade);
        ATK = information.BaseATK + Mathf.RoundToInt(LV * information.ATK_Grownth * grade);

        if (grade == 0)
            grade = 1;


        //Initialized();
    }
    //�켱�� Hit(����)�� Die(���� �� ����Ʈ�� Ȱ��)
    //(�������� ����Ʈ�� �ʿ��ϴٰ� ������ �� ����)
    public void GetEffect(ParticleSystem Hitparticle, ParticleSystem Dieparticle)
    {
        HitEffect = Hitparticle;
        DieEffect = Dieparticle;
    }


    #endregion




    #endregion
    #region �ʱ�ȭ

    protected override void Initialized()
    {
        
        ID = CharacterContentMapping.Canonicalize(ID);
        CharacterContentMapping.NormalizeData(_information);
        base.Initialized();
       
        MAX_hp = information.BaseHP + Mathf.RoundToInt(LV * information.HP_Grownth* grade);
        ATK = information.BaseATK + Mathf.RoundToInt(LV * information.ATK_Grownth*grade);
        LB_gauge_gain = 10;
        current_hp = MAX_hp;
        current_atk = ATK;
        var a = this.GetComponent<CapsuleCollider>();
        if (ID == "e00")
        {
            if (transform.childCount > 0)
            {
          
                Transform model = transform.GetChild(0);
                Vector3 vec3 = new Vector3(0, -1, 0);
                model.localPosition = vec3;
                model.localRotation = Quaternion.identity;
            }
        }
        HitteDamageNumber = 1;
        deadbody_duration = 0.5f;
        rgd.freezeRotation = true;
        initialize_target();
        initialize_action();
        LB_gauge = 0;
        if (is_animation)
            initialize_animation();
        //InitializeAudio();
        particle_System = this.GetComponent<Character_Particle_System>();
        StautusEffectList = new List<StatusEffect>();
    }

    #endregion
    #region Ȱ��ȭ
    public override void active_obj()
    {
        base.active_obj();
        if (is_animation)
        {
            C_ani.set_animation_speed(1);
        }
    }
    public override void deactive_chr()
    {
        base.deactive_chr();
        execute_handler(CancelAction_handler);
        if (is_animation)
        {
            C_ani.set_animation_speed(0);
        }
    }

    #endregion
    #region �ǰ�&ȸ�� ó��
    public void set_pulling()
    {
        is_pulling = true;
    }
    void Die()
    {
 
        isDie = true;
        execute_handler(CancelAction_handler);
        if (is_pulling)
            Stage._stage.AddPullingChrList(this);
        

        rgd.linearVelocity = Vector3.zero;
       
    }
    protected override void _Destroy()
    {
        base._Destroy();
        if (deadbody_duration <= 0)
            particle_System.CreateDieEffect();
    }
    void hitted(int a)
    {
        if (a == 0)
            return;
        
        current_hp -=Mathf.RoundToInt( a*HitteDamageNumber);

        create_damage_font(a);
        notifyObserver();

        if (current_hp <= 0)
        {

            current_hp = 0;
            Die();

            return;
        }
        
    }
    void create_damage_font(int a)
    {
        Damage_font font = s.pulling_damage_font(T);
        font.set_font(a, this.transform);
    }
    public void heal(int a)
    {
        if ((current_hp += a) > MAX_hp)
            current_hp = MAX_hp;
        //ȸ�� ��Ʈ?
        notifyObserver();
    }
    #endregion
    #region ���� ���� ó��
    float forceTimer;
  public  bool focused;
    public override void forced(Vector3 vec, float forced_time)
    {
        execute_handler(CancelAction_handler);
        forceTimer = forced_time;
        frame_handelr.Add(() =>
        {
            forceTimer -= Time.deltaTime;
            set_cant_handle_time(forceTimer);
            if (forceTimer <= 0)
                return true;
            else
                return false;
        });
        base.forced(vec, forced_time);
    }
   
    #endregion
    #region ������
    Stage s;
    List<Character_observer> MyObserver = new List<Character_observer>();
    public void AddObserver(Character_observer s)
    {
        MyObserver.Add(s);
        s.update_information(current_hp, this);
    }

    public void DeleteObserver(Character_observer s)
    {
        MyObserver.Remove(s);
    }
  
    public void notifyObserver()
    {
        foreach (Character_observer s in MyObserver)
        {
            s.update_information(current_hp, this);
        }
    }
    #endregion
    #region �ൿ
    float action_delay = 1f;
    float delay_time;

    Character_action _current_action=null;

    Coroutine act_corutine;
    bool on_action;
    public bool Auto_LB;
    float init_timer;
    public void GetStatus(StatusEffect b)
    {

        StautusEffectList.Add(b);
        frame_handelr.Add(b.invoke);
    }
   
    void initialize_action()
    {
        delay_time = 0;
        _current_action = null;
        on_action = false;
        init_timer = 0;
        LimitBurst = information.LB;
        action_queue.Clear();
        foreach (string s in information.actionset)
        {
            if (s.Contains("skill"))
            {
                int number = int.Parse(s.Substring(5));
                action_queue.Enqueue(information.skills[number - 1]);
            }
            else
            {
                action_queue.Enqueue(information.normalattack);
            }
        }
    }
    protected Queue<Character_action> action_queue = new Queue<Character_action>();
    Character_action action_pop()
    {
        Character_action _action = action_queue.Dequeue();
        action_queue.Enqueue(_action);
        return _action;
    }
    void Character_Act()
    {
        
        if (on_action)
        {
            init_timer += Time.deltaTime;
        } else if (delay_time > 0)
        {
            delay_time -= Time.deltaTime;
        }
        else
        {
           
            //���� �ൿ�� ���ٸ� ����
            if (action_queue.Count == 0)
                return;

            transform.LookAt(new Vector3(chasing_target.transform.position.x, this.transform.position.y, chasing_target.transform.position.z));
            if (_current_action==null)
            {
                _current_action = action_pop();
            }
            foreach(Action<Character_action> a in Action_handler)
            {
                a.Invoke(_current_action);
            }
        }
    }

 
  
    IEnumerator Act(Character_action a)
    {
        on_action = true;
        yield return new WaitUntil(() => init_timer >= a.init_time);
        init_timer = 0;
        a.Invoke(current_atk, this);
        if (AudioPlayer != null)
        {
    
            AudioPlayer.PlayHitClip(a);
        }
        else
        {
            Debug.Log("AudioPlayier is null,");
        }
        particle_System.CreateHitEffect(
            this.transform.position+(this.transform.position-
            
            chasing_target.transform.position)/2 *-1 );
        delay_time = action_delay;
        gain_LBgauge();
        on_action = false;
        _current_action = null;
        notifyObserver();
    }
    #endregion
    #region �ʻ��
    public bool is_LB_can_use_anywhere;
    public float LB_gauge { get; set; }
    
    public readonly float LB_gauge_max = 100;
    float LB_gauge_gain;
    bool on_LB;
    bool start_lb;
    Character_action LimitBurst;

   void gain_LBgauge()
    {
        if (LB_gauge < LB_gauge_max)
            LB_gauge += LB_gauge_gain;
        else
            return;
        if (LB_gauge > LB_gauge_max)
            LB_gauge = LB_gauge_max;
    }
    public void gain_LBgauge(float gain)
    {
        if (LB_gauge < LB_gauge_max)
            LB_gauge += gain;
        else
            return;
        if (LB_gauge > LB_gauge_max)
            LB_gauge = LB_gauge_max;
        notifyObserver();
    }
    public void use_LB()
    {
        Debug.Log("LB��� ��ư Ŭ��");
        if (LB_gauge >= LB_gauge_max)
        {
            
            Debug.Log("LB���");
           
            execute_handler(CancelAction_handler);
            execute_LB_handler();
        }
    }
    IEnumerator Act_LB()
    {
        if (LimitBurst.action==null)
            yield break; 
     on_action = true;
        yield return new WaitForSeconds(LimitBurst.init_time);
       LimitBurst.Invoke(current_atk, this);
        LB_gauge = 0;
        
        on_action = false;
        notifyObserver();
        delay_time = action_delay;

    }
    #endregion
    #region �ڵ鷯
    List<Action> Move_handler = new List<Action>();
    //�ǰ� �ڵ鷯
    List<Action<int>> Hitted_Handler = new List<Action<int>>();
    //ȸ�� �ڵ鷯
    List<Action<int>> Healed_Handler = new List<Action<int>>();
    //�� ������ ���� �ߵ��ϴ� �ڵ鷯
    List<Func<bool>> frame_handelr = new List<Func<bool>>();
    //�ൿ�� ����ϴ� �ڵ鷯
    List<Action> CancelAction_handler = new List<Action>();
    //�ൿ�� �����ϴ� �ڵ鷯
    List<Action<Character_action>> Action_handler = new List<Action<Character_action>>();
    //�ʻ�� �ڵ鷯
    List<Action> LB_handler = new List<Action>();
    public void register_frame_handler(Func<bool> a)
    {
        frame_handelr.Add(a);
    }
    void execute_Handler(List<Action> handler)
    {
        foreach (Action action in handler)
        {
            action.Invoke();
        }
    }
    void execute_Handler(List<Action<int>> handler,int n)
    {
        foreach (Action<int> action in handler)
        {
            action.Invoke(n);
        }
    }
    void execute_LB_handler()
    {
        
            foreach (Action action in LB_handler)
            {
                action.Invoke();
            }
        
    }
    
    void execute_frame_handler()
    {
        for (int n = 0; n < frame_handelr.Count; n++)
        {
            if (frame_handelr[n].Invoke())
            {
                frame_handelr.RemoveAt(n);
                n--;
            }
        }
    }
   
    protected override void register_shader_handler()
    {
        
        base.register_shader_handler();
        Hitted_Handler.Add(chr_shader.change_hitted_emission);
        Hitted_Handler.Add(new Action<int>((num1) => {
            register_frame_handler(chr_shader.return_hitted_emission); 
        }));
        Healed_Handler.Add(chr_shader.change_healed_emission);
        Healed_Handler.Add(new Action<int>((num1) => {
            register_frame_handler(chr_shader.return_healed_emission);
        }));

    }
    protected override void initialize_handler()
    {
        Hitted_Handler.Clear();
        Healed_Handler.Clear();
        frame_handelr.Clear();
        CancelAction_handler.Clear();
        Move_handler.Clear();
        Action_handler.Clear();
        base.initialize_handler();
        if (is_animation)
            register_animation_handler();
    }
    protected void register_animation_handler()
    {
    
        _Destroy_handler.Add(C_ani.active_death_animation);
        CantHandle_handler.Add(C_ani.active_stun_timer_animator);
        CancelAction_handler.Add(C_ani.stop_current_animation);
        Action_handler.Add(C_ani.active_action_animation);
        Move_handler.Add(C_ani.active_Move_animation);
        Hitted_Handler.Add((a)=> { C_ani.active_Damaged_animation(); });
        frame_handelr.Add(C_ani.resetmodeltransform);
    }
    protected override void register_Handler()
    {
        base.register_Handler();
        CantHandle_handler.Add((t) => chasing_target = null );
        Hitted_Handler.Add(hitted);
        Healed_Handler.Add(heal);
        Move_handler.Add(move);
        //ȸ�� ��Ʈ �߰�
        Action_handler.Add((a) => act_corutine = StartCoroutine(Act(a)));
        CancelAction_handler.Add(() => { if (act_corutine != null) { StopCoroutine(act_corutine); } });
        LB_handler.Add(() => StartCoroutine(Act_LB()));
        LB_handler.Add(() => { Debug.Log("LB�ʱ�ȭ"); this.LB_gauge = 0; }) ;
    }
    public void execute_Heal_handler(int n)
    {
        foreach (Action<int> a in Healed_Handler)
        {
            a.Invoke(n);
        }
    }
    public void execute_Hit_handler(int n)
    {
        foreach (Action<int> a in Hitted_Handler)
        {
            a.Invoke(n);
        }
    }
    #endregion
    #region Ÿ����&�̵�
    GameCharacter chasing_target;



    float target_distance { get {return 
                chasing_target == null ? 9999 : (chasing_target.transform.position - transform.position).magnitude; } }
    public bool is_target_on_distance;
    void initialize_target()
    {
        chasing_target = null;
    }

   void move()
    {
        if (target_distance > information.distancenumber)
        {
            chase_target();
        }
        else
        {
            move_forward();
        }
    }
   
    void set_target()
    {
        if (chasing_target != null && (!chasing_target.object_activasion || chasing_target.current_hp <= 0
            ))
            chasing_target = null;
        if (chasing_target == null )
        {
        
            is_target_on_distance = false;
            if ( (chasing_target = PrioritySystem.Character_Priority.instance.get_Character_by_distance_nearest(T==Team.Player? Team.Enemy:Team.Player,this.transform.position)))
            {
         
                Func<bool> func = new Func<bool>(() =>
                {
                    if (target_distance <= information.distancenumber)
                    {

                        rgd.linearVelocity = Vector3.zero;
                        notifyObserver();
                        return true;
                    }
                    else
                        return false;

                });
                    frame_handelr.Add(func);
                //���ϸ����Ͱ� ���� ��
                if (is_animation)
                {
                    Func<bool> func2 = new Func<bool>(() =>
                    {
                     
                        if (is_target_on_distance)
                        {
                            C_ani.reset_parameter("OnMove");
                            return true;
                        }else
                            return false;
                    });
                    frame_handelr.Add(func2);
                }
            }


        }
       

       
    }


    void chase_target()
    {
      
        if (chasing_target.on_viewport)
        {
   
            transform.LookAt(new Vector3(chasing_target.transform.position.x, this.transform.position.y, chasing_target.transform.position.z));
  
            rgd.linearVelocity = transform.forward * 10;
        }
        else move_forward();
    }
    void move_forward()
    {
  
        if (T == Team.Player)
            transform.rotation = Quaternion.identity;
        else
            transform.rotation = new Quaternion(0, 180, 0,0);

       

        rgd.linearVelocity = transform.forward * 10 * speed;

    }
    #endregion
    #region ���̴�
    protected CharacterShader chr_shader;
    protected override void initialize_shader(ColorShaderManager m = null)
    {
        if (chr_shader == null)
            chr_shader = new CharacterShader(this.gameObject,true);       
        base.initialize_shader(chr_shader); 
        chr_shader.set_Emission_by_team(T);
    }
    #endregion
    #region ���ϸ��̼�
    Character_Animation C_ani;
    public bool is_animation=true;
    public RuntimeAnimatorController this_ani_controller;
    void initialize_animation()
    {
        if(C_ani==null)
            C_ani = new Character_Animation(this, this_ani_controller);
        else
            C_ani.initialize_animation();
    }
    public void setAnimation(RuntimeAnimatorController animator)
    {
        if (animator != null)
        {
            is_animation = true;
            this_ani_controller = animator;
            //Debug.Log("���ϸ����� �ڵ鷯 ����");
            //C_ani = new Character_Animation(this,animator);
        }
        else
        {
            is_animation = false;
        }
    }


    #endregion
    #region ����
    Character_Audio_Player AudioPlayer;
    
    //bool InitializeAudio()
    //{
    //    if (TryGetComponent<Character_Audio_Player>(out AudioPlayer))
    //    {
    //        //Hitted_Handler.Add((a) => { AudioPlayer.PlaySFX("Hit"); });
    //        //Action_handler.Add(AudioPlayer.PlaySfxByActionHandler);
    //        return true;
           
    //    }
    //    else
    //        return false;
    //}

    #endregion
    #region ����Ʈ
    Character_Particle_System particle_System;

    public ParticleSystem HitEffect;
    public ParticleSystem DieEffect;
    public void CreateHitEffect(Vector3 Position)
    {
        Instantiate(HitEffect, Position, HitEffect.transform.rotation);
    }
    public void CreateDieEffect()
    {
        Instantiate(DieEffect, this.transform.position, DieEffect.transform.rotation);
    }
    #endregion
}

