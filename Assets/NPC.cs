using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("移動速度")]
    public float Speed;
    float SaveSpeed;
    [Header("扣玩家的血量值")]
    public float HurtPlayer;

    [Header("設定怪物總血量")]
    public float TotalHP;

    [Header("設定怪物傷害量")]
    public float HurtHP;

    [Header("怪物死亡加多少分數")]
    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        SaveSpeed = Speed;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(0,0,Speed*Time.deltaTime);
        //Vector3.forward-(0,0,1)參考世界軸向Z軸方向
        //transform.forward-(0,0,1)物件自己的Z軸方向
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }
    #region 怪物碰到防禦牆
    //OnTriggerEnter:當兩個物件相撞，就觸發一次程式
    //OnTriggerStay :當兩物件持續相撞，就持續觸發程式，直到彼此分開
    //OnTriggerExit :當兩物件相撞且分開，就觸發一次程式
    void OnTriggerEnter(Collider hit)
    {
        //物件碰撞-物件名稱 hit.GameComponent<Collider>().name
        //物件碰撞-物件標籤 hit.GameComponent<Collider>().tag，如要碰撞動態生成出來的物件，皆會使用tag判斷

        //若NPC碰到防禦牆
        if (hit.GetComponent<Collider>().name == "mazu_wall")
        {
            Speed = 0;
            GetComponent<Animator>().SetBool("Att", true);
        }
    }

    void OnTriggerExit(Collider hit)
    {
        //若怪物離開防禦牆
        if (hit.GetComponent<Collider>().name == "mazu_wall")
        {

            Speed = SaveSpeed;      //恢復怪物移動速度
            GetComponent<Animator>().SetBool("Att", false);     //動作切回走路動作
        }
    }
    #endregion
    //#region 怪物離開防禦牆
    public void AttackPlayer()
    {
        //呼叫扣玩家血量
        GameObject.Find("Cube").GetComponent<GameManager>().HurtPlayer(HurtPlayer);
    }

    public void Hurt()
    {
        //扣除怪物總血量
        TotalHP -= HurtHP;

        //如血量<=0
        if (TotalHP <= 0)
        {
            //若死亡的是Boss
            if (gameObject.tag == "Boss")
            {
                StartCoroutine(BossDead());

            }

            //collider關閉
            GetComponent<Collider>().enabled = false;
            //執行死亡動畫
            GetComponent<Animator>().SetTrigger("Die");
            //怪物死亡做加分
            GameObject.Find("Cube").GetComponent<GameManager>().TotalScore(Score);
            //無法移動
            Speed = 0;
        }
    }

    IEnumerator BossDead()
    {
        yield return new WaitForSeconds(1.5f);
        //顯示勝利
        GameObject.Find("Cube").GetComponent<GameManager>().isWin = true;
        //開啟GameOver視窗
        GameObject.Find("Cube").GetComponent<GameManager>().GameOver();
    }

}
