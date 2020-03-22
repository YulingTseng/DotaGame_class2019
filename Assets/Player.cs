using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("法術物件")]
    public GameObject Arrow;

    [Header("法術要產生的位置")]
    public GameObject SpawnPosition;
    //判斷攻擊動畫的Farm觸發此function
    public void SpawnArrow()
    {
        //動態生成法術物件
        Instantiate(Arrow, SpawnPosition.transform.position, SpawnPosition.transform.rotation);
    }
}
