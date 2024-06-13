using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Camera")]
    public GameObject menuCam;
    public GameObject gameCam;

    [Header("Script")]
    public Player player;
    public Boss boss;

    public int stage;       
    public float playTime;  //��Ž
    public bool isBattle;   //�ο�� ��?

    [Header("Enemy count")]
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;

    [Header("UI")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;    //�ִ� ����
    public Text scoreTxt;       //�� ����
    public Text stageTxt;       //��������
    public Text playTimeTxt;    //��Ž
    public Text playerHpTxt;    //HP
    public Text playerAmmoTxt;  //�Ѿ�
    public Text playerCoinTxt;  //����
    //
    public Image weapon1Image;
    public Image weapon2Image;
    public Image weapon3Image;
    public Image weaponRImage;
    //
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    //
    public RectTransform bossHpGroup;
    public RectTransform bossHpBar;

    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()   //LateUpdate : �� �����Ӹ��� ���� ������ Update()�� ���� �� ȣ���
    {
        //��� UI
        scoreTxt.text = string.Format("{0:n0}", player.score);  //999,999,999�� ǥ��
        stageTxt.text = $"STAGE {stage}";

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = $"{string.Format("{0:00}", hour)}:{string.Format("{0:00}", min)}:{string.Format("{0:00}", second)}"; //09:30:26���� ǥ��

        //�÷��̾� UI
        playerHpTxt.text = player.hp + " / " + player.maxHp;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null) //���� �ִ� ���� ����
            playerAmmoTxt.text = $"- / {player.ammo}";
        else if (player.equipWeapon.type == Weapon.Type.Melee)//���� ���� ����
            playerAmmoTxt.text = $"- / {player.ammo}";
        else
            playerAmmoTxt.text = $"{player.equipWeapon.curammo} / {player.ammo}";

        //���� UI
        weapon1Image.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Image.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Image.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImage.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //���� ���� UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //���� ü�� UI
        bossHpBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }
}