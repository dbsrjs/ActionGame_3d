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

    public void Use()   //���⸦ ���
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
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
}
