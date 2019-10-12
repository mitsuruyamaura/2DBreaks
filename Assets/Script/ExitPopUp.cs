using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPopUp : MonoBehaviour
{
    public GameObject exitpopup;

    public GameMaster gameMaster;


    public void OnClickStageExit()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }

    //リターンをしたらポップアップを閉じる(隠す)処理
    public void OnClickStageReturn()
    {
        // ゲームステートがSTOPだったら、現在ゲームは一時停止と判断しているので
        if (gameMaster.gameState == GAME_STATE.STOP)
        {
            // ステートをPLAYに戻して
            gameMaster.gameState = GAME_STATE.PLAY;
            // ゲーム内の時間の流れを標準に戻す
            Time.timeScale = 1.0f;
        }

        exitpopup.SetActive(false);
    }



    public void OpenPopUp()
    {
        exitpopup.SetActive(true);
        // ゲームステートがPLAYだったら、現在ゲームは動いていると判断しているので
        if (gameMaster.gameState == GAME_STATE.PLAY)
        {
            // ステートをSTOPにして
            gameMaster.gameState = GAME_STATE.STOP;
            // ゲームの流れを止める = このとき各クラスのUpdateメソッドのみ、ゲーム再開できる準備のため動いている。
            Time.timeScale = 0.0f;
        }
    }
}
