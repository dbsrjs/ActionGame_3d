using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3); //3초 뒤에 삭제
        }
        
        if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); //3초 뒤에 삭제
        }
    }
}
