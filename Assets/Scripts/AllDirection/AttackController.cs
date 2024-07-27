using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public enum PlayerState {
    Charge,
    Normal
}

public class AttackController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float chargeTimer;

    [SerializeField]
    private float maxChargeCount;

    [SerializeField]
    private float countUpRate;   // 補正

    [SerializeField]
    private float BasePower;


    void Start() {
        TryGetComponent(out rb);
    }

    void Update()
    {
        // １ショット
        //if (Input.GetMouseButtonDown(0)) {
        //    // ノーマル攻撃
        //    Attack(); 
        //}


        if (Input.GetMouseButton(0)) {
            // チャージ
            if (chargeTimer > maxChargeCount) {
                chargeTimer = maxChargeCount;
                return;
            }

            // チャージ + チャージ速度の調整
            chargeTimer += Time.deltaTime * countUpRate;

            // エフェクト生成


        } else if (Input.GetMouseButtonUp(0)){
            // チャージ攻撃　チャージした時間分だけ速度アップ
            Attack(chargeTimer);

            chargeTimer = 0;

            // エフェクトがある場合には破棄


        }
    }

    private void Attack(float chargeTime) {
        Debug.Log(chargeTime);
        //rb.velocity = Vector2.zero;  // なくても問題なし

        // transform.up で2Ｄ世界では正面を示せる。3Ｄの transform.forword とイメージは同じ。right にすると右に行く
        rb.AddForce(transform.up * BasePower * chargeTime, ForceMode2D.Impulse);
    }


    void FixedUpdate() {

        // 動いている間は徐々に停止させる
        if (rb.velocity != Vector2.zero) {
            rb.velocity *= 0.9f;

            // 双方の浮動小数点の値が 0 に近いなら
            if (Mathf.Approximately(0, rb.velocity.x) && Mathf.Approximately(0, rb.velocity.y)) {
                rb.velocity = Vector2.zero;
            }
        }
    }
}