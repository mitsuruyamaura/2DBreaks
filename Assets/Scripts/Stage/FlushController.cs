using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 被ダメージ時の画面エフェクト（色を変えたり点滅させる）を管理するクラス
/// </summary>
public class FlushController : MonoBehaviour
{
    [Header("点滅させるUIオブジェクト")]
    public Image flushImg;
    [Header("点滅状態のフラグ")]
    public bool isFlush;

    private GameMaster gameMaster;
    private float timer;  //経過時間計測用

    private void Start() {
        gameMaster = GetComponent<GameMaster>();
    }

    /// <summary>
    /// 画面のエフェクト用フラグを立てる
    /// </summary>
    public void StartFlushEffect() {
        //ここでフラグが立つとUpdateの条件を満たすようになる
        isFlush = true;
    }

    void Update() {
        //フラグとゲームの状態に合わせて画面の色を変えたり、点滅させるようにする
        if (isFlush){
            //画面を赤くする
            flushImg.color = new Color(0.5f, 0f, 0f, 0.5f);

            //もしゲーム状態がWARING状態なら画面を点滅させる
            if (gameMaster.gameState == GAME_STATE.WARNING) {
                timer += Time.deltaTime;
                if (timer > 1.0f) {
                    timer = 0;
                    isFlush = false;
                }
            } else {
                //ゲームステートがWARING以外なら一回だけ赤くする
                isFlush = false;
            }
        } else {
            //すぐ上のif分でisFlushがfalseになるので条件後、こちらにも続けて入る
            //赤をだんだんと薄くして透明に戻す
            //Color.Leapメソッドは第一引数と第二引数を第三引数に応じてミックスする
            //ここでは赤と透明に対して時間の経過によって赤から透明に変化させている
            flushImg.color = Color.Lerp(flushImg.color, Color.clear, Time.deltaTime);

            if (gameMaster.gameState == GAME_STATE.WARNING) { 
                //もしゲームが警告状態なら画面を点滅させる
                timer += Time.deltaTime;
                if (timer > 1.5f) {
                    timer = 0;
                    //もう一度isFlushフラグを立て直して、再度Updateに入るようにする
                    StartFlushEffect();
                }
            }
        }
    }

    /// <summary>
    /// 画面の点滅を止める
    /// </summary>
    public void CleanUpFlushEffect() {
        //画面を点滅させるためのフラグを下ろし、Updateで点滅させないようにする
        isFlush = false;

        //画面の色のエフェクトを消して透明に戻す
        flushImg.color = new Color(0f, 0f, 0f, 0f);
    }
}
