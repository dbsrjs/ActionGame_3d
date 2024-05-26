using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine("Explosion");
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);     // 3초 동안 대기합니다.
        rigid.velocity = Vector3.zero;          // Rigidbody의 속도를 0으로 설정합니다.
        rigid.angularVelocity = Vector3.zero; // Rigidbody의 각속도를 0으로 설정합니다.
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // 중심 위치에서 반경 15의 구체로 적을 탐지합니다.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        // 탐지된 각 적에게 수류탄에 맞았다는 메서드를 호출합니다.
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5); // 5초 후에 게임 오브젝트를 파괴합니다.
    }

}
