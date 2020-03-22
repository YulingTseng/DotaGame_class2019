using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    void OnTriggerEnter(Collider hit)
    {
        //若大絕招的龍碰撞到NPC或Boss
        if (hit.GetComponent<Collider>().tag == "NPC" || hit.GetComponent<Collider>().tag == "Boss")
        {
            //怪物扣血
            hit.GetComponent<NPC>().Hurt();
            //GameManager腳本數值歸零
            GameObject.Find("Cube").GetComponent<GameManager>().Timer = 0;
            //刪除大魔法物件
            Destroy(transform.parent.gameObject);
        }
        //若大絕招的龍碰撞到地板
        if (hit.GetComponent<Collider>().name == "mazu_floor")
        {
            //GameManager腳本數值歸零
            GameObject.Find("Cube").GetComponent<GameManager>().Timer = 0;
            //刪除大魔法物件
            Destroy(transform.parent.gameObject);
        }
    }
}
