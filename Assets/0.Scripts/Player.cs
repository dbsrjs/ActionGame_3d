using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component")]
    public Camera followCamera;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;  //Player와 충돌중인 Weapon
    Weapon equipWeapon; //현재 갖고 있는 Weapon

    [Header("move")]
    public float speed; //속도
    float hAxis;
    float vAxis;
    Vector3 moveVec;    //이동 좌표
    Vector3 dodgeVec;   //회피를 사용했을 때의 위치를 저장

    [Header("weappons")]
    public GameObject[] weapons;    //무기
    public bool[] hasWeapons;       //갖고 있는지 없는지
    int equipWeaponIndex = -1;   //현재 장착중인 Weapon

    [Header("Item")]
    public int coin;            //현재 갖고 있는 코인 개수
    public int maxCoin;         //max 코인 개수
    public int health;          //현재 체력
    public int maxHealth;       //max 체력

    [Header("Ammo")]
    public int ammo;            //현재 갖고 있는 총알 개수
    public int maxAmmo;         //max 총알 개수

    [Header("Grenades")]
    public GameObject[] grenades;   //수류탄
    public int maxHasGrenades;      //max 수류탄 개수
    public int hasGrenades;         //현재 갖고 있는 수류탄 개수

    [Header("MoveButton")]
    bool wDown; //w키를 눌렀을 때(달리기)
    bool jDown; //스페이스바를 눌렀을 때(점프)
    bool iDown; //e키를 눌렀을 때(획득)
    bool fDown; //좌클릭을 눌렀을 때(공격)
    bool rDown; //r키를 눌렀을 때(재장전)

    bool sDown1;    //1키를 눌렀을 때(스왑)
    bool sDown2;    //2키를 눌렀을 때(스왑)
    bool sDown3;    //3키를 눌렀을 때(스왑)

    [Header("condition")]
    bool isJump;    //점프 중일 때
    bool isDodge;   //회피 중일 때
    bool isSwap;     //스왑 중일 때
    bool isReload;  //장전 중일 때
    bool isFireReady = true; //망치를 휘두를 수 있을 때

    float fireDelay;        //딜레이

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
        Attack();
        Reload();
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
        fDown = Input.GetButton("Fire1");           //총알 발사
        rDown = Input.GetButtonDown("Reload");      //재장전
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

        if (isSwap || !isFireReady || isReload) //무기를 바꾸거나 망치를 휘두르고 있을 때
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;   //wDown 걷고 있을 땐 속도가 0.5 달리고 있을 땐 1

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#키보드를 이용한 회전
        transform.LookAt(transform.position + moveVec); //플레이어에 이동 방향에 따라 Rotation도 같이 움직임

        //#마우스를 이용한 회전
        if(fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);//ScreenPointToRay : 스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))//out : return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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

    void Attack()
    {
        if (equipWeapon == null)    //갖고 있는 무기가 없다면
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null || equipWeapon.type == Weapon.Type.Melee || ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1.2f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curammo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
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
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

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
