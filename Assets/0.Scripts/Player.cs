using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Component")]
    public Camera followCamera;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;  //Player�� �浹���� Weapon
    Weapon equipWeapon; //���� ���� �ִ� Weapon
    MeshRenderer[] meshs;

    [Header("move")]
    public float speed; //�ӵ�
    float hAxis;
    float vAxis;
    Vector3 moveVec;    //�̵� ��ǥ
    Vector3 dodgeVec;   //ȸ�Ǹ� ������� ���� ��ġ�� ����

    [Header("weappons")]
    public GameObject[] weapons;    //����
    public bool[] hasWeapons;       //���� �ִ��� ������
    int equipWeaponIndex = -1;   //���� �������� Weapon

    [Header("Item")]
    public int coin;            //���� ���� �ִ� ���� ����
    public int maxCoin;         //max ���� ����
    public int health;          //���� ü��
    public int maxHealth;       //max ü��

    [Header("Ammo")]
    public int ammo;            //���� ���� �ִ� �Ѿ� ����
    public int maxAmmo;         //max �Ѿ� ����

    [Header("Grenades")]
    public GameObject[] grenades;   //����ź
    public int maxHasGrenades;      //max ����ź ����
    public int hasGrenades;         //���� ���� �ִ� ����ź ����
    public GameObject grenadeObj;   //���� ����ź

    [Header("MoveButton")]
    bool wDown; //wŰ�� ������ ��(�޸���)
    bool jDown; //�����̽��ٸ� ������ ��(����)
    bool iDown; //eŰ�� ������ ��(ȹ��)

    [Header("MoveButton")]
    bool fDown; //��Ŭ���� ������ ��(����)
    bool gDown; //��Ŭ���� ������ ��(����ź ��ô)
    bool rDown; //rŰ�� ������ ��(������)

    [Header("SwopButton")]
    bool sDown1;    //1Ű�� ������ ��(����)
    bool sDown2;    //2Ű�� ������ ��(����)
    bool sDown3;    //3Ű�� ������ ��(����)

    [Header("condition")]
    bool isJump;    //���� ���� ��
    bool isDodge;   //ȸ�� ���� ��
    bool isSwap;     //���� ���� ��
    bool isReload;  //���� ���� ��
    bool isFireReady = true; //��ġ�� �ֵθ� �� ���� ��
    bool isBorder;  //��輱�� ��ҳ� �� ��ҳ�
    bool isDamage;  //�������� �Ծ��� ��

    float fireDelay;        //������

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();  //GetComponentInChildren : �ڽ����� �ִ� ������Ʈ ���� ����
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
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
        fDown = Input.GetButton("Fire1");           //�Ѿ� �߻�
        gDown = Input.GetButton("Fire2");           //�Ѿ� �߻�
        rDown = Input.GetButtonDown("Reload");      //������
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

        if (isSwap || !isFireReady || isReload) //���⸦ �ٲٰų� ��ġ�� �ֵθ��� �ְų� �����ϰ� ���� ��
            moveVec = Vector3.zero;

        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime;   //wDown �Ȱ� ���� �� �ӵ��� 0.5 �޸��� ���� �� 1

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#Ű���带 �̿��� ȸ��
        transform.LookAt(transform.position + moveVec); //�÷��̾ �̵� ���⿡ ���� Rotation�� ���� ������

        //#���콺�� �̿��� ȸ��
        if(fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);//ScreenPointToRay : ��ũ������ ����� Ray�� ��� �Լ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))//out : returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown && !isReload && !isSwap)   // ����ź�� ���� ��, �������� �ƴ� ��, �������� �ƴ� ��
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);//ScreenPointToRay : ��ũ������ ����� Ray�� ��� �Լ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))//out : returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 12;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;  //���� ���� �ִ� ����ź�� �ϳ� �ٿ���
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)    //���� �ִ� ���Ⱑ ���ٸ�
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

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;   //angularVelocity : ���� ȸ�� �ӵ�
        //"Wall"�̶�� LayerMask�� ���� ��ü�� �浹�ϸ� isBorder�� true�� ��ȯ��
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));  //Raycast : Ray�� ��� ��� ������Ʈ�� �����ϴ� �Լ�
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward *5, Color.green);   //DrawRay : Scene ������ Ray�� �����ִ� �Լ�
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
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

        else if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }

            if (other.gameObject.GetComponent<Rigidbody>() != null)  //�̻��ϰ� �浹
                Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(bool isBossAttack)
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.grey;
        }

        if (isBossAttack)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); //�˹� ����

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAttack)
            rigid.velocity = Vector3.zero;
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
