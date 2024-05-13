using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material; //Material은 MeshRenderer 컴포넌트에서 접근 가능
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")  //근접
        { 
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;    //현재 위치에 피격 위치를 빼서 반작용 방향 구하기
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
             Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;    //현재 위치에 피격 위치를 빼서 반작용 방향 구하기
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
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

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse);   //함수로 넉백 구현

            Destroy(gameObject, 4);
        }
    }
}