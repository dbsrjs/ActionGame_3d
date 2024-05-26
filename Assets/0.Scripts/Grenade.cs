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
        yield return new WaitForSeconds(3f);     // 3�� ���� ����մϴ�.
        rigid.velocity = Vector3.zero;          // Rigidbody�� �ӵ��� 0���� �����մϴ�.
        rigid.angularVelocity = Vector3.zero; // Rigidbody�� ���ӵ��� 0���� �����մϴ�.
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // �߽� ��ġ���� �ݰ� 15�� ��ü�� ���� Ž���մϴ�.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        // Ž���� �� ������ ����ź�� �¾Ҵٴ� �޼��带 ȣ���մϴ�.
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5); // 5�� �Ŀ� ���� ������Ʈ�� �ı��մϴ�.
    }

}
