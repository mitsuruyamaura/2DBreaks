using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [Header("ライフ管理クラスの取得用")]
    public LifeManager lifeManager;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ball")
        {
            //Lifeを減らす
            //引数はダメージ量
            lifeManager.ApplyDamage(1);
        }
    }


}
