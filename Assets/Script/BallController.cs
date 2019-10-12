using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BallController : MonoBehaviour
{
    [SerializeField, Header("ボールの速度")]
    public float speed;

    public Rigidbody2D rb; //コンポーネントの格納用

    private Vector2 direction; //ボールの方向

    public bool isStart; //スタートフラグ(中身はfalse)

    public GameMaster gameMaster;

    void Update()
    {

        if (gameMaster.gameState == GAME_STATE.READY)
        {

            //ボールをタップして一度だけ打ち出す
            if (!isStart)
            {
                ShotBall();
            }
        }
       
    }

    public void ShotBall()
    {
        if (Input.GetMouseButtonDown(0))
        {
            direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;
            //角度によって速度が変化してしまうのでnormalizedで正規化して同じ速度にする

            //ボールを打ち出す(摩擦や空気抵抗、重力を切ってあるので、ずっと同じ速度で動き続ける)
            rb.velocity = -direction * speed * transform.localScale.x;
            //scaleXをかけてオブジェクトに見合った力とスピードにする

            isStart = true;
            gameMaster.gameState = GAME_STATE.PLAY;

        }
    }

    public void StopMoveBall()
    {
        //ボールの速度ベクトルを0にしてボールを止める
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Playerタグのオブジェクトに接触したら
        if (col.gameObject.tag == "Player")
        {
            //dirに弾の向きをいれる。向きは弾の現在位置 - プレイヤーの位置より算出。
            Vector2 dir = transform.position - col.gameObject.transform.position;

            //リジッドボディを使って弾に力を加える（Randomな値で跳ね返すようにする）
            rb.velocity = dir * speed * Random.Range(1.0f, 2.0f) * transform.localScale.x;
        }
    }

}
