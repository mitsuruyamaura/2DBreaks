using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面の色を変えたり点滅させるクラス
/// </summary>
public class FlushController : MonoBehaviour
{
    [Header("点滅させるUIオブジェクト")]
    public Image img; //インスペクターからFlushPanelオブジェクトのImageをアサイン

    [Header("点滅状態のフラグ")]
    public bool isFlush;

    [Header("GameMasterクラス取得")]
    public GameMaster gameMaster;

    private float timer;  //経過時間計測用


    public void StartFlushEffect()
    {
        //ここでフラグが立つとUpdateの条件を満たすようになる
        isFlush = true;
    }

    // Update is called once per frame
    void Update()
    {

        //フラグとゲームの状態に合わせて画面の色を変えたり、点滅させるようにする

        //Bool型の場合、条件式にbool変数のみ書くとTrueかどうかを判定する
        //isFlushがTrueなら
        if (isFlush)
        {
            //画面を赤くする。引数はRGBAの並び順
            img.color = new Color(0.5f, 0f, 0f, 0.5f);

            //もしゲーム状態がWARING状態なら画面を点滅させる
            if (gameMaster.gameState == GAME_STATE.WARNING)

                timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                timer = 0;
                isFlush = false;


                //ゲームステートがWARING以外なら一回だけ赤くする
            }
            else
            {
                isFlush = false;


                //isFlushがFalseなら
            }
        }
        else
        {
            //すぐ上のif分でisFlushがfalseになるので条件後、こちらにも続けて入る
            //赤をだんだんと薄くして透明に戻す
            //Color.Leapメソッドは第一引数と第二引数を第三引数に応じてミックスする
            //ここでは赤と透明に対して時間の経過によって赤から透明に変化させている
            img.color = Color.Lerp(img.color, Color.clear, Time.deltaTime);

            if (gameMaster.gameState == GAME_STATE.WARNING)
            {
                //もしゲームが警告状態なら画面を点滅させる
                timer += Time.deltaTime;
                if (timer > 1.5f)
                {
                    timer = 0;
                    //もう一度isFlushフラグを立て直して、再度Updateに入るようにする
                    StartFlushEffect();
                }
            }

        }

    }

    public void CleanUpFlushEffect()
    {
        //画面を点滅させるためのフラグを下ろし、Updateで点滅させないようにする
        isFlush = false;

        //画面の色のエフェクトを消して透明に戻す
        img.color = new Color(0f, 0f, 0f, 0f);
    }

}
