using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultPopUp : MonoBehaviour
{
    private BattleManager battleManager;

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
    private CanvasGroup canvasGroup;

    [SerializeField]
    private int magnifyingTimePoint;

    [SerializeField]
    private int magnifyingBallPoint;


    private bool isSelectBtn;   // 重複でボタンを押せないように制御




    public void SetUpResultPopUp(BattleManager battleManager, int money, int battleTime, int remainingballCount) {
        canvasGroup.DOFade(1.0f, 1.0f);

        this.battleManager = battleManager;

        btnClosePopUp.onClick.AddListener(OnClickClosePopUp);
        btnExitGame.onClick.AddListener(ExitGame);

        btnClosePopUp.interactable = false;
        btnExitGame.interactable = false;

        DispayResult(money, battleTime, remainingballCount);
    }

    private void OnClickClosePopUp() {
        if (!isSelectBtn) {
            isSelectBtn = true;

            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame() {
        canvasGroup.DOFade(0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private void OnClickExitGame() {
        if (!isSelectBtn) {
            isSelectBtn = true;
            ExitGame();
        }
    }

    /// <summary>
    /// ゲームを終了する
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

    private void DispayResult(int money, int battleTime, int remainingballCount) {

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
                    battleTime * magnifyingTimePoint,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));

        // ④シーケンス処理を0.5秒だけ待機
        sequence.AppendInterval(0.5f);

        // ⑤残っている手球の数 × 倍率の数字をアニメして表示
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtBallBonus.text = num.ToString();
                    },
                    remainingballCount * magnifyingBallPoint,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));


        int totalPoint = money + (battleTime * magnifyingTimePoint) + (remainingballCount * magnifyingBallPoint);

        // ⑥シーケンス処理を1.0秒だけ待機
        sequence.AppendInterval(1.0f);

        // ⑦すべての合計値を表示して、GameDataのtotalMoneyに加算
        sequence.Append(DOTween.To(() => initValue,
                    (num) => {
                        initValue = num;
                        txtTimeBonus.text = num.ToString();
                    },
                    GameData.instance.totalMoney + totalPoint,
                    1.0f).OnComplete(() => { initValue = 0; }).SetEase(Ease.InCirc));

        // 各ボタンを押せるようにする
        btnClosePopUp.interactable = true;
        btnExitGame.interactable = true;
    }
}
