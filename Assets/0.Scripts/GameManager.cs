using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Object")]
    public GameObject menuCam;
    public GameObject gameCam;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    [Header("Script")]
    public Player player;
    public Boss boss;

    public int stage;       
    public float playTime;  //��Ž
    public bool isBattle;   //�ο�� ��?

    [Header("Enemy")]
    public Transform[] enemyZones;   //�� ���� ��ġ
    public GameObject[] enemies;     //�� ������
    public List<int> enemyList;      //�ܰ躰 �� ���� ����
    public int enemyCntA;           //���� �����ִ� ��A
    public int enemyCntB;           //���� �����ִ� ��B
    public int enemyCntC;           //���� �����ִ� ��C
    public int enemyCntD;           //���� �����ִ� ��D

    [Header("UI")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreTxt;    //�ִ� ����
    public Text scoreTxt;       //�� ����
    public Text stageTxt;       //��������
    public Text playTimeTxt;    //��Ž
    public Text playerHpTxt;    //HP
    public Text playerAmmoTxt;  //�Ѿ�
    public Text playerCoinTxt;  //����
    public Text curScoreText;  //����
    public Text bestText;      //�ְ� ����
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
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if(stage % 5 == 0)  //������
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();   //prefab�� target(player)�� ������ �� ������
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else        //�Ϲ���
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);   //���� ����(A, B, C, D)
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);//���� ���� ��ġ
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();   //prefab�� target(player)�� ������ �� ������
                //���� �ʱ�ȭ
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);

                yield return new WaitForSeconds(2.5f);
            }
        }

        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        boss = null;
        StageEnd();
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
        if(boss != null)
        {
            bossHpGroup.anchoredPosition = new Vector3(0, -23, 0);
            bossHpBar.localScale = new Vector3((float)boss.curHp / boss.maxHp, 1, 1);
        }
        else
        {
            bossHpGroup.anchoredPosition = new Vector3(0, 100, 0);
        }
    }
}