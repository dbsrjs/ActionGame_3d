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
    public float playTime;  //플탐
    public bool isBattle;   //싸우는 중?

    [Header("Enemy")]
    public Transform[] enemyZones;   //적 스폰 위치
    public GameObject[] enemies;     //적 프리텝
    public List<int> enemyList;      //단계별 적 스폰 개수
    public int enemyCntA;           //현재 남아있는 적A
    public int enemyCntB;           //현재 남아있는 적B
    public int enemyCntC;           //현재 남아있는 적C
    public int enemyCntD;           //현재 남아있는 적D

    [Header("UI")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreTxt;    //최대 점수
    public Text scoreTxt;       //현 점수
    public Text stageTxt;       //스테이지
    public Text playTimeTxt;    //플탐
    public Text playerHpTxt;    //HP
    public Text playerAmmoTxt;  //총알
    public Text playerCoinTxt;  //코인
    public Text curScoreText;  //점수
    public Text bestText;      //최고 점수
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
        if(stage % 5 == 0)  //보스전
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();   //prefab라서 target(player)이 지정이 안 되있음
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else        //일반전
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);   //랜덤 몬스터(A, B, C, D)
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
                int ranZone = Random.Range(0, 4);//랜덤 스폰 위치
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();   //prefab라서 target(player)이 지정이 안 되있음
                //변수 초기화
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

    void LateUpdate()   //LateUpdate : 매 프레임마다 실행 되지만 Update()가 끝난 후 호출됨
    {
        //상단 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);  //999,999,999로 표시
        stageTxt.text = $"STAGE {stage}";

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = $"{string.Format("{0:00}", hour)}:{string.Format("{0:00}", min)}:{string.Format("{0:00}", second)}"; //09:30:26으로 표시

        //플레이어 UI
        playerHpTxt.text = player.hp + " / " + player.maxHp;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null) //갖고 있는 무기 없음
            playerAmmoTxt.text = $"- / {player.ammo}";
        else if (player.equipWeapon.type == Weapon.Type.Melee)//근접 무기 있음
            playerAmmoTxt.text = $"- / {player.ammo}";
        else
            playerAmmoTxt.text = $"{player.equipWeapon.curammo} / {player.ammo}";

        //무기 UI
        weapon1Image.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Image.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Image.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImage.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //몬스터 숫자 UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //보스 체력 UI
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