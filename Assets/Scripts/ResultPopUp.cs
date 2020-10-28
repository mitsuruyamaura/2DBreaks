using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultPopUp : MonoBehaviour
{
    // UI関連  同名のゲームオブジェクトをアサインする
    [SerializeField]
    private Text txtMoney;

    [SerializeField]
    private Text txtTimeBonus;

    [SerializeField]
    private Text txtBallBonus;

    [SerializeField]
    private Text txtTotalMoney;

    [SerializeField]
    private Text txtDiscriptionTimeBonus;

    [SerializeField]
    private Text txtDiscriptionBallBonus;

    [SerializeField]
    private Button btnClosePopUp;

    [SerializeField]
    private Button btnExitGame;

    [SerializeField]
    private CanvasGroup canvasGroup;    // ResultPopUp ゲームオブジェクトのCanvasGroupをアサインする


    [SerializeField, Header("残り時間ボーナス倍率")]
    private int magnifyingTimePoint;

    [SerializeField, Header("手球残数ボーナス倍率")]
    private int magnifyingBallPoint;


    private bool isSelectBtn;   // 重複してボタンを押せないように制御

    private BattleManager battleManager;

    /// <summary>
    /// リザルトポップアップの初期設定
    /// </summary>
    /// <param name="battleManager"></param>
    /// <param name="money"></param>
    /// <param name="battleTime"></param>
    /// <param name="remainingballCount"></param>
    /// <param name="isClear">true = クリア, false = ゲームオーバー</param>
    public void SetUpResultPopUp(BattleManager battleManager, int money, int battleTime, int remainingballCount, bool isClear) {
        Debug.Log(money);
        Debug.Log(battleTime);
        Debug.Log(remainingballCount);

        // 一度リザルトポップアップを見えなくする
        canvasGroup.alpha = 0;

        // 徐々にリザルトポップアップを表示
        canvasGroup.DOFade(1.0f, 1.0f);

        this.battleManager = battleManager;

        // 各ボタンにメソッドを登録
        btnClosePopUp.onClick.AddListener(OnClickClosePopUp);
        btnExitGame.onClick.AddListener(OnClickExitGame);

        // Money表示処理中はボタンを押せない状態にしておく
        btnClosePopUp.interactable = false;
        btnExitGame.interactable = false;

        // 各ボーナス倍率を表示
        txtDiscriptionTimeBonus.text = "( battleTime × " + magnifyingTimePoint + " )";

        txtDiscriptionBallBonus.text = "(RemainingBall × " + magnifyingBallPoint + " )";

        // リザルトの内容を表示
        DispayResult(money, battleTime, remainingballCount, isClear);
    }

    /// <summary>
    /// リザルトポップアップを閉じる
    /// </summary>
    private void OnClickClosePopUp() {

        // まだボタンが押されていなければ
        if (!isSelectBtn) {

            // ボタンの重複・連続タップ防止
            isSelectBtn = true;

            // ゲームを再度スタートする
            StartCoroutine(RestartGame());
        }
    }

    /// <summary>
    /// ゲームを再度スタート
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartGame() {

        // 徐々にリザルトポップアップを透明にする
        canvasGroup.DOFade(0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        // 現在のシーン名を取得
        string sceneName = SceneManager.GetActiveScene().name;

        // シーンの読み込み・遷移
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    private void OnClickExitGame() {

        // いずれかのボタンが押されているなら
        if (isSelectBtn) {
            // このメソッドは処理しない
            return;
        }

        // ボタンの重複・連続タップ防止
        isSelectBtn = true;
       
        // ゲーム終了処理
        ExitGame();   　
    }

    /// <summary>
    /// ゲーム終了の実処理
    /// </summary>
    public void ExitGame() {

        //  ゲームのPlatform実行状況に合わせて処理を行う
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_WEBGL
        Application.OpenURL("https://www.yahoo.co.jp/");

#else //UNITY_STANDALONE
        Application.Quit();

#endif
    }

    /// <summary>
    /// リザルトポップアップに今回のゲームのリザルト内容を表示
    /// </summary>
    /// <param name="money"></param>
    /// <param name="battleTime"></param>
    /// <param name="remainingballCount"></param>
    /// <param name="isClear">true = クリア, false = ゲームオーバー</param>
    private void DispayResult(int money, int battleTime, int remainingballCount, bool isClear) {

        // 獲得したMoneyを表示
        // 計算用の初期値を設定
        int initValue = 0;

        // DoTweenのSequence(シーケンス)機能を初期化して使用できるようにする
        Sequence sequence = DOTween.Sequence();

        // シーケンスを利用して、DoTweenの処理を制御したい順番で記述する。まずは①獲得したMoneyをアニメして表示
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtMoney.text = num.ToString();
                    },
                    money,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));   // 次の処理のためにOnCompleteを使ってinitValueを初期化

        // ②シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(0.5f);

        // ③残っている時間 × 倍率の数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtTimeBonus.text = num.ToString();
                    },
                    isClear ? battleTime * magnifyingTimePoint : 0,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));

        // ④シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(0.5f);

        // ⑤残っている手球の数 × 倍率の数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtBallBonus.text = num.ToString();
                    },
                    isClear ? remainingballCount * magnifyingBallPoint : 0,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));


        int totalPoint = money + (battleTime * magnifyingTimePoint) + (remainingballCount * magnifyingBallPoint);

        // ⑥シーケンス処理を1.0秒だけ待機
        sequence.AppendInterval(1.0f);

        // ⑦すべての合計値を表示して、GameDataのtotalMoneyに加算
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtTotalMoney.text = num.ToString();
                    },
                    totalPoint,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));


        sequence.AppendInterval(1.0f);

        // 今回のゲーム内で獲得したMoneyをMoney総数に加算
        GameData.instance.ProcMoney(totalPoint);

        // リザルトの表示が終了したので、各ボタンを押せるようにする
        btnClosePopUp.interactable = true;
        btnExitGame.interactable = true;
    }
}
