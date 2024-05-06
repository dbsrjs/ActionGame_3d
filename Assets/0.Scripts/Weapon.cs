using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,  //���� ����
        Range   //���Ÿ� ����
    }

    public Type type;
    public int damage;  //������
    public float rate;  //���� �ӵ�
    public BoxCollider mellArea;    //���� ���� ����
    public TrailRenderer trailEffect;
    [Header("Gun")]
    public Transform bulletPos; //�Ѿ� ���� ��ġ
    public GameObject bullet;    //�Ѿ�
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public int maxAmmo; //�ִ� �Ѿ�
    public int curammo; //���� �Ѿ�

    public void Use()   //���⸦ ���
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

    private IEnumerator Swing() //IEnumerator�� yield�� ������ ��ȯ �������(Co Routine)
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
        //�Ѿ� �߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        //ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);             //AddForce
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);    //AddTorque : ȸ�� �Լ�
        yield return null;
    }
}
