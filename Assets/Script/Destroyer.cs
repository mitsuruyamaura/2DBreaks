using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ball")
        {
            //GameMasterクラスをゲーム内で探してクリア目標数をデクリメント(-1)する
           GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>().normaBlockNum--;

            Destroy(gameObject);
        }
    }

}
