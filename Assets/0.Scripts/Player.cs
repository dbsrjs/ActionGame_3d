using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown; //�ȱ� �޸���
    bool jDown; //����

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();  //GetComponentInChildren : �ڽ����� �ִ� ������Ʈ ���� ����
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
        wDown = Input.GetButton("Walk");    //reft shift�� ������ wDown�� true�� ��
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  //normalized : � �����̵� �̵� �ӵ��� 1�� ����������

        if (isDodge)    //ȸ�Ǹ� �ϰ� �ִٸ� ������ �� �ٲٵ��� ����
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;   //wDown �Ȱ� ���� �� �ӵ��� 0.5 �޸��� ���� �� 1

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); //�÷��̾ �̵� ���⿡ ���� Rotation�� ���� ������
    }

   void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)   //Jump(Space)Ű�� ������, isJump(���� ��� ���� ��)�� false�� ��
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
   }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)   //Jump(Space)Ű�� ������, isJump(���� ��� ���� ��)�� false�� ��
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);   //0.5�� �ڿ� DodgeOut�Լ� ����
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
