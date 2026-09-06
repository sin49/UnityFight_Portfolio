using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class range_bullet : MonoBehaviour
{
    public GameCharacter target_chr;
    float speed;
    [HideInInspector]
    public int damage;
    Vector3 target_velocity;
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public float time;
    Rigidbody rgd;
    float magni;
    Vector3 vector;
    /*Vector3 calculate_vector_by_time()//�ð��� ����
    {
        vector = target_chr.position - transform.position;
        magni = vector.magnitude;
        target_velocity = vector.normalized;
        target_velocity = target_velocity * (magni / (time - timer));

        return target_velocity;
    }*/
    Vector3 calculate_vector()
    {
        vector = target_chr.transform.position - transform.position;
        magni = vector.magnitude;
        target_velocity = vector.normalized;
        speed = 5;
        return target_velocity * speed;
    }
    private void Start()
    {
        rgd = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (target_chr != null)
            rgd.linearVelocity = calculate_vector();
        else
            gameObject.SetActive(false);
        //timer += Time.deltaTime;
        if (magni <= 0.5f&&magni>0)
        {
            get_damage();
        }
    }
    void get_damage()
    {
        target_chr.execute_Hit_handler(damage);
        
        magni = 0;
        target_chr = null;
        gameObject.SetActive(false);
    }
    public void set_bullet(GameCharacter Target,int DMG)
    {
        target_chr = Target;
        damage = DMG;
    }
}
