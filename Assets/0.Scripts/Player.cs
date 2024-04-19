using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown; //걷기 달리기
    bool jDown; //점프

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();  //GetComponentInChildren : 자식한테 있는 컴포넌트 갖고 오기
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");    //reft shift를 누르면 wDown이 true가 됨
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  //normalized : 어떤 방향이든 이동 속도를 1로 고정시켜줌

        if (isDodge)    //회피를 하고 있다면 방향을 못 바꾸도록 설정
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;   //wDown 걷고 있을 땐 속도가 0.5 달리고 있을 땐 1

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); //플레이어에 이동 방향에 따라 Rotation도 같이 움직임
    }

   void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)   //Jump(Space)키를 눌렀고, isJump(땅에 닿아 있을 때)가 false일 때
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
   }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)   //Jump(Space)키를 눌렀고, isJump(땅에 닿아 있을 때)가 false일 때
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);   //0.5초 뒤에 DodgeOut함수 실행
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
