using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ステージ情報、ゲームの進捗、UI、他のクラスを管理するためのマネージャー用クラス
/// </summary>
public class GameMaster : MonoBehaviour
{
    [Header("ブロックの下に隠れている画像")]
    public SpriteRenderer underImage;
    [Header("クリア目標ブロック数")]
    public int normaBlockNum;
    
    [Header("現在のゲーム状態")]
    public GAME_STATE gameState;
    [Header("インフォ表示")]
    public TMP_Text infoText;
    [Header("現在のステージ番号")]
    public int stageNum;

    [Header("ブロック生成クラス")]
    public InitBlocks initBlocks;
    [Header("ライン管理クラス")]
    public Liner liner;
    [Header("ボール管理クラス")]
    public BallController ball;  
    [Header("ライフ管理クラス")]
    public LifeManager life;
    
    [Header("ステージ情報。スクリプタブルオブジェクト")]
    public StageData stageData;

    private string stageName = "tsumu";
    private BattleTime battleTime;
    

    void Awake() {
        InitDatas();
    }

    /// <summary>
    /// ステージ情報とクラスを取得して各変数に代入し、ゲームの準備を行う
    /// </summary>
    private void InitDatas() {
        // クラス取得
        battleTime = GetComponent<BattleTime>();

        //StageNumとを照らし合わせ、同じステージ番号を見つける
        foreach (StageData.StageDataList data in stageData.stageDatas) {
            if (data.stageNum == SelectStage.stageNo) {
                // stageDataに登録されている情報を、それぞれのクラスの変数に代入する
                stageNum = data.stageNum;
                initBlocks.fileName = data.stageSpriteName;
                initBlocks.blockPos.transform.position = data.startPosition;
                ball.speed = data.ballSpeed;
                ball.power = data.ballPower;
                liner.lineLength = data.minLineLength;
                liner.duration = data.lineDuration;
                life.initLife = data.initMaxLife;
                battleTime.currentBattleTime = data.initBattleTime;
                // TODO 他にも設定項目が追加されたらここで設定する

            }
        }
        // ブロックの下に隠れる画像を選択されたステージの画像にする
        underImage.sprite = Resources.Load<Sprite>("Textures/" + stageName + stageNum);

        // ブロックを生成し、生成したブロック数をクリアノルマに代入
        normaBlockNum = initBlocks.CreateBlocks();
    }
    
    /// <summary>
    /// クリアノルマを減算
    /// </summary>
    public void ProcNorma() {
        normaBlockNum--;
        if (normaBlockNum <= 0) {
            // すべてのブロックを壊したらクリア
            GameUp(GAME_STATE.STAGE_CLEAR);
        }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <param name="changeNewState"></param>
    public void GameUp(GAME_STATE changeNewState) {
        //ゲーム状態を変更する
        gameState = changeNewState;

        // ゲーム状態に応じてメッセージ表示
        switch (gameState) {
            case GAME_STATE.STAGE_CLEAR :
                // ステージクリア表示
                infoText.text = "CLEAR!";
                break;
            case GAME_STATE.GAME_OVER:
                // ゲームオーバー表示
                infoText.text = "GAME OVER..." + "\n\n" + "Touch Screen To Menu";
                break;
        }      
        // 各クラスへストップ処理を出す
        StopOtherObjects();
    }

    /// <summary>
    /// 管理している他のクラスの処理を止める
    /// </summary>
    private void StopOtherObjects() {
        // 制限時間表示を消す
        battleTime.HideBattleTime();

        //ボールを止める
        ball.StopMoveBall();

        // 画面のエフェクトを止める
        GetComponent<FlushController>().CleanUpFlushEffect();
    }

    /// <summary>
    /// バトルを一時停止する
    /// </summary>
    public void PauseBattle() {       
        gameState = GAME_STATE.STOP;

        // ボールを止める
        ball.StopMoveBall();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CheckState() {
        if ((gameState == GAME_STATE.STOP) || (gameState == GAME_STATE.READY)) {
            return false;
        } else {
            return true;
        }
    }

    void Update() {
        if ((gameState == GAME_STATE.STAGE_CLEAR) || (gameState == GAME_STATE.GAME_OVER)) {
            if (Input.GetMouseButtonDown(0)) {
                // Tapしたらフェイドアウトしながらシーン遷移処理
                StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
            }
        }
    }
}
