using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,  //근접 공격
        Range   //원거리 공격
    }

    public Type type;
    public int damage;  //데미지
    public float rate;  //공격 속도
    public BoxCollider mellArea;    //근접 공격 범위
    public TrailRenderer trailEffect;
    [Header("Gun")]
    public Transform bulletPos; //총알 생성 위치
    public GameObject bullet;    //총알
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public int maxAmmo; //최대 총알
    public int curammo; //현재 총알

    public void Use()   //무기를 사용
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if(type == Type.Range && curammo > 0)
        {
            curammo--;
            StartCoroutine("Shot");
        }
    }

    private IEnumerator Swing() //IEnumerator는 yield를 무조건 반환 해줘야함(Co Routine)
    {
        yield return new WaitForSeconds(0.1f);
        mellArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        mellArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    private IEnumerator Shot()
    {
        //총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        //탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);             //AddForce
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);    //AddTorque : 회전 함수
        yield return null;
    }
}
