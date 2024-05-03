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

    public void Use()   //무기를 사용
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
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
}
