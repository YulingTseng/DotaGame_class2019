using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet移動速度")]
    public float Speed;

    [Header("多久刪除物件")]
    public float DeletTime;

    [Header("怪物被打到後倒退距離")]
    public float Dis;

    // Start is called before the first frame update
    void Start()
    {
        //刪除物件Destroy(要刪除的對象,等待多久把物件刪除)
        Destroy(gameObject, DeletTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Speed * Time.deltaTime, 0, 0, Space.Self);
    }

    void OnTriggerEnter(Collider hit)
    {
        //若碰到標籤為 NPC 或 Boss 把自己毀滅
        if (hit.GetComponent<Collider>().tag == "NPC" || hit.GetComponent<Collider>().tag == "Boss")
        {
            hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z + Dis);
            hit.GetComponent<NPC>().Hurt();
            Destroy(gameObject);
        }

    }
}
