using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMaster : MonoBehaviour
{
    [Header("クリア目標ブロック数")]
    public int normaBlockNum;

    [Header("クラスを取得する")]
    public InitBlocks initBlocks;

    public GAME_STATE gameState;

    public TMP_Text infoText;

    public BattleTime battleTime;

    public BallController ball;

    public int stageNum;

    public LifeManager life;

    //StageDataのスクリプタブルオブジェクトを参照する
    public StageData stageData;

    public SpriteRenderer underImage;
    private string stageName = "tsumu";


    void Awake()
    {
     SetUp();
    }

    private void SetUp()
    {
        //スクリプタブルプロジェクトとして登録してあるデータにステージの番号(stageNum)があり
        //その値とSelectStageno
        //StageNumとを照らし合わせ、同じステージ番号を見つける

            foreach (StageData.StageDataList data in stageData.stageDatas)
        {
            //dataの値には」 stageData.stageDatas(登録されているステージの情報)が一つずつ順番に入る
            //dataのstageNumとSelectStage.stageNoが同じになったら、それは選択されたステージであるので
            if (data.stageNum == SelectStage.stageNo)
            {
                
                //登録されている情報を、それぞれのクラスの変数に代入する
                //ボールの速度やブロックのイメージファイルの名前を取得
                stageNum = data.stageNum;
                Debug.Log(stageNum);

                initBlocks.fileName = data.stageSpriteName;
                initBlocks.blockPos.transform.position = data.startPosition;
                ball.speed = data.ballSpeed;
                life.initLife = data.maxLife;
            }
        }

        underImage.sprite = Resources.Load<Sprite>("Textures/" + stageName + stageNum);

    }

    void Start()
    {
       
        //CreateBlocks()(戻り値ありのメソッド)から戻ってきたint型の値をnormaBlockNum(用意しておいた箱)へ代入する
        normaBlockNum = initBlocks.CreateBlocks();

    }


    public void GameUp()
    {
        //ゲームの進行状況をゲームオーバーの状態にする
        gameState = GAME_STATE.GAME_OVER;

        //ボールを止める処理を呼び出す（次の手順でメソッドを追加）
        ball.StopMoveBall();

        //ゲーム終了の文字を表示
       infoText.text = "GAME OVER..." + "\n\n" + "Touch Screen To Menu";
    }

    void Update()
    {
        if (normaBlockNum <= 0)
        {
            //クリアおめでとうメッセージを出す
            infoText.text = "CLEAR!";

            //ゲームの残り時間を止める
             battleTime.StopBattleTime();

            //クリアのフラグを立てる
            gameState = GAME_STATE.STAGE_CLEAR;

        }

        //フラグが立ったら下の処理

        if (gameState == GAME_STATE.STAGE_CLEAR)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //メニューシーンに戻る
                SceneManager.LoadScene("Menu");
            }
        }

        if (gameState == GAME_STATE.GAME_OVER)
        {
            //ゲームオーバーの状態になったら
            if (Input.GetMouseButtonDown(0))
            {
                //タップでMenuにもどる
                SceneManager.LoadScene("Menu");
            }
        }

    }
}
