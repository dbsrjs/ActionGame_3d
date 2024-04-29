using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; //속도
    public GameObject[] weapons;    //무기
    public bool[] hasWeapons;       //갖고 있는지 없는지
    public GameObject[] grenades;   //수류탄

    public int ammo;            //현재 갖고 있는 총알 개수
    public int coin;            //현재 갖고 있는 코인 개수
    public int health;          //현재 체력
    public int hasGrenades;     //현재 갖고 있는 수류탄 개수

    public int maxAmmo;         //max 총알 개수
    public int maxCoin;         //max 코인 개수
    public int maxHealth;       //max 체력
    public int maxHasGrenades;  //max 수류탄 개수

    float hAxis;
    float vAxis;

    bool wDown; //걷기 달리기
    bool jDown; //점프
    bool iDown; //e키를 눌렀을 때

    bool sDown1;    //1키를 눌렀을 때
    bool sDown2;    //2키를 눌렀을 때
    bool sDown3;    //3키를 눌렀을 때

    bool isJump;    //점프 중일 때
    bool isDodge;   //회피 중일 때
    bool isSwap;     //스왑 중일 때

    Vector3 moveVec;    //이동 좌표
    Vector3 dodgeVec;   //회피를 사용했을 때의 위치를 저장

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject;  //Player와 충돌중인 Weapon
    GameObject equipWeapon; //현재 갖고 있는 Weapon
    int equipWeaponIndex = -1;   //현재 장착중인 Weapon
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
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  //normalized : 어떤 방향이든 이동 속도를 1로 고정시켜줌

        if (isDodge)    //회피를 하고 있다면 방향을 못 바꾸도록 설정
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)   //Jump(Space)키를 눌렀고, isJump(땅에 닿아 있을 때)가 false일 때
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
   }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)   //Jump(Space)키를 눌렀고, isJump(땅에 닿아 있을 때)가 false일 때
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

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))    //1키를 눌렀고, hasWeapons(갖고 있음)이 꺼져있거나 equipWeaponIndex(장착 중)이 같을 때
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) //1, 2, 3키중 하나라도 눌렀을 때
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
        if (iDown && nearObject != null && !isJump && !isDodge)  //E키를 눌렀고, 무기를 갖고 있고, 점프 혹은 회피를 안 했을 때
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
        if (other.tag == "Item")    //아이템 획득
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
