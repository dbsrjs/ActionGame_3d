using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //�ӵ�
    public GameObject[] weapons;    //����
    public bool[] hasWeapons;       //���� �ִ��� ������
    public GameObject[] grenades;   //����ź

    public int ammo;            //���� ���� �ִ� �Ѿ� ����
    public int coin;            //���� ���� �ִ� ���� ����
    public int health;          //���� ü��
    public int hasGrenades;     //���� ���� �ִ� ����ź ����

    public int maxAmmo;         //max �Ѿ� ����
    public int maxCoin;         //max ���� ����
    public int maxHealth;       //max ü��
    public int maxHasGrenades;  //max ����ź ����

    float hAxis;
    float vAxis;

    bool wDown; //�ȱ� �޸���
    bool jDown; //����
    bool iDown; //eŰ�� ������ ��

    bool sDown1;    //1Ű�� ������ ��
    bool sDown2;    //2Ű�� ������ ��
    bool sDown3;    //3Ű�� ������ ��

    bool isJump;    //���� ���� ��
    bool isDodge;   //ȸ�� ���� ��
    bool isSwap;     //���� ���� ��

    Vector3 moveVec;    //�̵� ��ǥ
    Vector3 dodgeVec;   //ȸ�Ǹ� ������� ���� ��ġ�� ����

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject;  //Player�� �浹���� Weapon
    GameObject equipWeapon; //���� ���� �ִ� Weapon
    int equipWeaponIndex = -1;   //���� �������� Weapon
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
        Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");            //reft shift
        jDown = Input.GetButtonDown("Jump");        //space
        iDown = Input.GetButtonDown("Interation");  //e
        sDown1 = Input.GetButtonDown("Swap1");      //1
        sDown2 = Input.GetButtonDown("Swap2");      //2
        sDown3 = Input.GetButtonDown("Swap3");      //3
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  //normalized : � �����̵� �̵� �ӵ��� 1�� ����������

        if (isDodge)    //ȸ�Ǹ� �ϰ� �ִٸ� ������ �� �ٲٵ��� ����
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)   //Jump(Space)Ű�� ������, isJump(���� ��� ���� ��)�� false�� ��
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
   }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)   //Jump(Space)Ű�� ������, isJump(���� ��� ���� ��)�� false�� ��
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

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))    //1Ű�� ������, hasWeapons(���� ����)�� �����ְų� equipWeaponIndex(���� ��)�� ���� ��
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) //1, 2, 3Ű�� �ϳ��� ������ ��
        {
            if (equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)  //EŰ�� ������, ���⸦ ���� �ְ�, ���� Ȥ�� ȸ�Ǹ� �� ���� ��
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")    //������ ȹ��
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo; 
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
