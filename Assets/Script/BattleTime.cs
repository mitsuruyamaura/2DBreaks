using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleTime : MonoBehaviour
{
    [Header("制限時間の設定値")]
    public int battleTime;

    [Header("制限時間の表示")]
    public TMP_Text battleTimeText;

    private float timer; //時間計測用

    private bool isTimerSwitch = true;

	public GameMaster gameMaster;

   
    void Update()
    {
        //isStopが他のメソッドでtrueになった場合、ここで監視しているのでちゃんとフラグの状態に合わせて処理をしてくれる
        if (isTimerSwitch == false)
        {
            return;
            //戻り値がない場合のreturnはここから下の時間を計測する処理が止まる
            //書く場所に注意
        }

		if (gameMaster.gameState == GAME_STATE.GAME_OVER)
		{
			return;
		}

        //timerを利用して経過時間を計測
        timer += Time.deltaTime;
        //1秒ごとにtimerを0に戻し、battletimeを減算する
        if(timer >= 1)
        {
            timer = 0;
            battleTime--;

            if (battleTime < 0)
            {
                StopBattleTime();
				gameMaster.GameUp();
            }
            //時間表示を更新するメソッドを呼び出す
            DisplayBattleTime(battleTime);
        }
    }



    //制限時間を更新して[分:秒]で表示する
    private void DisplayBattleTime(int limitTime)
    {
        //引数で受け取った値を[分:秒]　に変換して表示する
        //ToString("00")でゼロプレースフォルダーして、一桁の時は頭に0をつける
        //割り切れなくてfloat型になってしまった場合、(int)でint型にキャストしている
        //変数 + 文字列 + 変数
        battleTimeText.text = ((int)(limitTime / 60)).ToString("00") + ":" + ((int)limitTime % 60).ToString("00");
    }


    public void StopBattleTime()
    {
        //時間の表示を消す
        battleTimeText.text = "  ";
        isTimerSwitch = false; //時間を止めるフラグが立つ


    }

}
