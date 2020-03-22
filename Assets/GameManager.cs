using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;   //使用Unity廣告程式庫
public class GameManager : MonoBehaviour
{
    #region 動態生成怪物的變數
    [Header("產生重物的方塊")]
    public Collider SpawnObject;

    //儲存最大邊界
    Vector3 MaxNum;
    //儲存最小邊界
    Vector3 MinNum;

    [Header("NPC怪物")]
    public GameObject NPC;
    [Header("多少時間產生一隻怪物")]
    public float WaitTime;
    public int MaxSpawnNum;

    //計算怪物的數量
    int SpawnNum;
    #endregion

    #region 玩家血量變數
    [Header("玩家血量")]
    public float PlayerHP;
    [Header("玩家血條UI")]
    public Image PlayerHPUI;
    float ScriptPlayerHP;
    #endregion

    [Header("遊戲暫停UI物件")]
    public GameObject PauseUI;

    #region 遊戲結束畫面
    //判斷遊戲是否獲勝
    public bool isWin;
    [Header("遊戲結束UI物件")]
    public GameObject GameOverUI;

    [Header("遊戲勝利失敗Image")]
    public Image GameOverUIImage;

    [Header("遊戲勝利圖")]
    public Sprite WinSprite;

    [Header("遊戲失敗圖")]
    public Sprite LoseSprite;

    [Header("獎勵分數")]
    public int RewardScore;
    int GameOverScore;

    [Header("獎勵分數文字")]
    public Text RewardScoreText;
    [Header("遊戲結束總分數文字")]
    public Text GameOverScoreText;

    [Header("下一戰按鈕")]
    public Button NextButton;

    #endregion

    [Header("玩家物件")]
    public Transform Player;

    //將射線打到的所有3D物件存放在此陣列中
    RaycastHit[] hits;

    RaycastHit hit;
    //滑鼠點擊位置
    Vector3 look_pos;

    [Header("顯示分數的文字")]
    public Text ScoreText;

    [Header("敵妖量的Bar圖")]
    public Image MonsterBar;

    int ScoreNum;   //計算總分
    float DeadNum;    //計算怪物死亡數量

    #region 大絕招
    [Header("大絕招的Bar條等多久會集滿")]
    public float SetTimer;
    //程式中做計時
    public float Timer;
    [Header("大絕招的Bar")]
    public Image MagicBar;
    [Header("大絕招的按鈕")]
    public Button MagicButton;
    //判斷是否可以放大絕招
    bool CanMagic;

    [Header("大絕招的prefab模型")]
    public GameObject MagicObject;
    GameObject MagicObject_prefab;
    #endregion

    [Header("Boss物件")]
    public GameObject Boss;

    //儲存LevelID的數值
    string SaveLevelID = "SaveLevelID";
    //預設關卡數值
    int LevelID = 1;
    [Header("關卡數值文字")]
    public Image[] LevelImage;
    [Header("所有數的圖片0~9")]
    public Sprite[] NumImage;

    [Header("遊戲結束畫面Level圖片")]
    public Image[] GameOverLevelImage;

    //儲存怪物數量的數值
    string SaveNpcNum = "SaveNpcNum";
    [Header("每關增加多少NPC")]
    public int AddNum;

    [Header("輸入Unity Android廣告ID")]
    public string AndroidAdsID;

    void Awake()
    {
        //開通廣告機制初始狀態(平台ID,是否為測試模式)
        Advertisement.Initialize(AndroidAdsID, true);
    }

    void ShowAds()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        //將collider最大邊界數值存放在MaxNum
        MaxNum = SpawnObject.bounds.max;
        //將collider最大邊界數值存放在MinNum
        MinNum = SpawnObject.bounds.min;
        //InvokeRepeating("function名稱",第一次呼叫function 等待的時間,第二次呼叫function等待的時間)
        InvokeRepeating("CreatNPC", 0f, WaitTime);
        //城市中的血量=外部調整玩家血量值
        ScriptPlayerHP = PlayerHP;

        //透過程式修改陣列長度
        //LevelImage = new Image[2];
        //LevelID讀取儲存數值
        LevelID = PlayerPrefs.GetInt(SaveLevelID);

        //MaxSpawnNum讀取儲存數值
        MaxSpawnNum = PlayerPrefs.GetInt(SaveNpcNum);
        NumImage = new Sprite[10];

        //透過for迴圈將Resources資料夾內的數字0~9圖片讀至NumImage陣列中
        for (int i = 0; i < NumImage.Length; i++)
        {
            NumImage[i] = Resources.Load<Sprite>("num_" + i);
        }
        //關卡數值的長度
        if (LevelID.ToString().Length < 2)
        {
            //十位數為0
            LevelImage[0].sprite = NumImage[0];
            //個位數為LevelID數值
            LevelImage[1].sprite = NumImage[LevelID];
        }
        else
        {
            //十位數為0
            LevelImage[0].sprite = NumImage[LevelID / 10];
            //個位數為LevelID數值
            LevelImage[1].sprite = NumImage[LevelID % 10];
        }


    }

    // Update is called once per frame
    void Update()
    {
        //GetMouseButton持續觸發
        //GetMouseButtonDown滑鼠點擊後觸發一次
        //GetMouseButtonUp滑鼠點擊後放開觸發一次
        //Input.GetMouseButton(0)  0 -> 滑鼠左鍵, 1 -> 滑鼠右鍵, 2 ->滑鼠中間滾輪
        //滑鼠左鍵指令=手機螢幕點擊，所以直接輸出在手機上可直接觸發

        //若按下左鍵
        if (!GameOverUI.active)
        {
            if (Input.GetMouseButton(0) && !PauseUI.active)
            {
                //遊戲視窗是2D座標，若要將滑鼠點擊座標轉換成遊戲內的3D座標，需透過ScreenPointToRay(Input.mousePosition)
                //Camera.main會自動快取場景上標籤為MainCamera
                //從主攝影機和滑鼠點擊的座標位置就可以兩點達成一線形成Ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //射線長度為100，將射線打到的所有物件存放在陣列中
                hits = Physics.RaycastAll(ray, 100);

                //透過迴圈看陣列中哪個物件為地板
                for (int i = 0; i < hits.Length; i++)
                {
                    hit = hits[i];
                    if (hit.collider.name == "mazu_floor")
                    {
                        if (MagicObject_prefab == null)
                        {
                            //將滑鼠點擊位置儲存
                            look_pos = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                            //讓玩家看相點擊位置
                            Player.transform.LookAt(look_pos);

                            //讓玩家開始進行攻擊
                            Player.GetComponent<Animator>().SetBool("Att", true);
                        }

                        else
                        {
                            MagicObject_prefab.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                        }

                    }
                }
            }
        }

        //若左鍵放開
        if (Input.GetMouseButtonUp(0))
        {
            if (MagicObject_prefab == null)
            {
                //玩家動畫回到idle
                Player.GetComponent<Animator>().SetBool("Att", false);
            }

            else
            {
                MagicObject_prefab.GetComponentInChildren<Rigidbody>().useGravity = true;
            }

        }

        //時間做累積
        Timer += Time.deltaTime;

        //將時間顯示在magicBar上
        MagicBar.fillAmount = Timer / SetTimer;
        //如果計時器的時間>=我們自己定義的時間
        if (Timer >= SetTimer)
        {
            MagicButton.interactable = true;      //開啟大絕招的按鈕
        }
        else
        {
            MagicButton.interactable = false;
        }
    }

    #region 建立怪物
    void CreatNPC()
    {
        if (SpawnNum < MaxSpawnNum)
        {
            //Instantiate動態生成(要生成的物件，生成出來後的位置，生成出來後的角度)
            Instantiate(NPC, new Vector3(Random.Range(MinNum.x, MaxNum.x), MinNum.y, MinNum.z), transform.rotation);
            SpawnNum++;
        }

        //若NPC死亡數量=自己設定的數量&&場景上沒有任何一隻Boss
        if (DeadNum == MaxSpawnNum && GameObject.FindGameObjectsWithTag("Boss").Length < 1)
        {
            //動態生成一隻Boss
            Instantiate(Boss, transform.position, transform.rotation);
        }
        if (DeadNum == 2)
        {
            Debug.Log(DeadNum);
        }

    }
    #endregion
    #region UI控制
    public void PasueGame()
    {
        Time.timeScale = 0;
        PauseUI.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        PauseUI.SetActive(false);
    }

    public void BackMenu()
    {
        Time.timeScale = 1;
        Application.LoadLevel("Menu");
    }
    #endregion
    #region 玩家扣血
    //怪物攻擊防禦強開始扣除玩家血量
    public void HurtPlayer(float hurt)
    {
        //扣血
        ScriptPlayerHP -= hurt;
        //將玩家血量顯示在UI上
        PlayerHPUI.fillAmount = ScriptPlayerHP / PlayerHP;

        //如果玩家血量=0
        if (PlayerHPUI.fillAmount == 0)
        {
            //遊戲結束
            GameOver();

            //失敗
            isWin = false;
        }
    }
    #endregion
    #region 遊戲結束
    public void GameOver()
    {
        //遊戲結束畫面開啟
        GameOverUI.SetActive(true);

        //關卡數值長度
        if (LevelID.ToString().Length < 2)
        {
            //十位數為0
            GameOverLevelImage[0].sprite = NumImage[0];
            //個位數為LevelID數值
            GameOverLevelImage[1].sprite = NumImage[LevelID];
        }
        else
        {
            //十位數為0
            GameOverLevelImage[0].sprite = NumImage[LevelID / 10];
            //個位數為LevelID數值
            GameOverLevelImage[1].sprite = NumImage[LevelID % 10];
        }

        //若勝利
        if (isWin)
        {
            string str = "win";
            if (ScoreNum >= 2)
            {
                Debug.Log(ScoreNum);
            }

            Debug.Log(str);
            //圖片換成勝利
            GameOverUIImage.sprite = WinSprite;
            RewardScore = 1000;
            //總分加上分數+獎勵分數
            GameOverScore = ScoreNum + RewardScore;
            NextButton.interactable = true;
            ShowAds();
        }

        //失敗
        else
        {
            //圖片換成失敗
            GameOverUIImage.sprite = LoseSprite;
            RewardScore = 0;
            //總分加上分數
            GameOverScore = ScoreNum;
            NextButton.interactable = false;
        }

        //顯示獎勵分數
        RewardScoreText.text = RewardScore + "";
        //顯示總分
        GameOverScoreText.text = GameOverScore + "";

        //整體時間暫停
        Time.timeScale = 0;
    }
    #endregion
    #region 計算分數
    public void TotalScore(int AddScore)
    {
        //怪物死亡就累積分數
        ScoreNum += AddScore;
        //將分數顯示在文字上
        ScoreText.text = ScoreNum + "";
        //增加怪物死亡數量
        DeadNum++;
        //將怪物死亡數量顯示在敵妖量上
        MonsterBar.fillAmount = ((MaxSpawnNum - DeadNum) / MaxSpawnNum);
    }
    #endregion

    public void ReGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void NextGame()
    {
        Time.timeScale = 1;
        //每關要增加多少怪物數量
        MaxSpawnNum += AddNum;
        //儲存怪物數量
        PlayerPrefs.SetInt(SaveNpcNum, MaxSpawnNum);
        //關卡+1
        LevelID++;
        //儲存關卡值
        PlayerPrefs.SetInt(SaveLevelID, LevelID);
        Application.LoadLevel(Application.loadedLevel);
    }

    public void CreateMagic()
    {
        if (MagicButton.interactable && MagicObject_prefab == null)
        {
            //動態生成一個大魔法物件
            MagicObject_prefab = Instantiate(MagicObject);
        }
    }

}
