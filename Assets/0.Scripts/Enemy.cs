using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material; //Material은 MeshRenderer 컴포넌트에서 접근 가능
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if(isChase)
            nav.SetDestination(target.position);    //SetDestination : 도착할 위치 지정 함수
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;   //angularVelocity : 물리 회전 속도
            rigid.angularVelocity = Vector3.zero;//angularVelocity : 회전력
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")  //근접
        { 
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;    //현재 위치에 피격 위치를 빼서 반작용 방향 구하기
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
             Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;    //현재 위치에 피격 위치를 빼서 반작용 방향 구하기
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 expolsoinPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - expolsoinPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;  //히트 됐을 때 색을 red로 변경
        yield return new WaitForSeconds(0.1f);  //0.1초 뒤

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if(isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);   //함수로 넉백 구현
            }
            

            Destroy(gameObject, 4);
        }
    }
}