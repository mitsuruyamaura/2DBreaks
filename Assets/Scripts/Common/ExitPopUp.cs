using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// バトル終了確認用のポップアップ制御用クラス
/// </summary>
public class ExitPopUp : MonoBehaviour
{
    [Header("キャンバスグループ")]
    public CanvasGroup canvasGroup;

    public RectTransform rectTran;

    [Header("ポップアップを閉じるボタン")]
    public Button closeBtn;
    [Header("バトルを終了するボタン")]
    public Button exitBtn;

    [Header("生成時に自動アサイン")]
    public GameMaster gameMaster;
    public GAME_STATE currentSceneState;

    private bool isSelectBtn;   // 重複でボタンを押せないように制御

    /// <summary>
    /// 各ボタンにシーンに応じたメソッドを登録
    /// </summary>
    public void Setup() {
        if (TransitionManager.instance.sceneState == SCENE_STATE.Stage) {
            closeBtn.onClick.AddListener(ClosePopup);
            exitBtn.onClick.AddListener(ExitStage);
        } else {
            closeBtn.onClick.AddListener(ClosePopup);
            exitBtn.onClick.AddListener(ExitGame);
        }
        ActivePopup();
    }

    /// <summary>
    /// ポップアップを表示する
    /// </summary>
    public void ActivePopup() {
        AnimePopup(1.0f);
    }

    /// <summary>
    /// ポップアップをアニメさせる
    /// </summary>
    /// <param name="alpha"></param>
    private void AnimePopup(float alpha) {
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTran.DOScale(new Vector3(5f, 5f), 0.15f).SetEase(Ease.Linear));
        seq.Append(rectTran.DOScale(new Vector3(3f, 3f), 0.35f).SetEase(Ease.Linear));
        seq.Join(canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;
                isSelectBtn = false;
            });
    }

    /// <summary>
    /// バトルを終了し、StageシーンからMenuシーンへ遷移
    /// </summary>
    public void ExitStage() {
        if (!isSelectBtn) {
            isSelectBtn = true;
            StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
        }
    }

    /// <summary>
    /// バトルに戻る
    /// </summary>
    public void ReturnBattle() {
        if (gameMaster.normaBlockNum <= 0) {
            // クリア状態で開いた場合にはクリア状態にする
            gameMaster.gameState = GAME_STATE.STAGE_CLEAR;
        } else if (gameMaster.life.LifePoint <= 0) {
            // ライフが残っていない状態で開いた場合にはゲームオーバー状態にする
            gameMaster.gameState = GAME_STATE.GAME_OVER;
        } else { 
            // ゲームの状態をポップアップが開く前の状態に変更し、ボールの移動を再開
            gameMaster.gameState = currentSceneState;
            gameMaster.ball.RestartMoveBall();
        }
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void ExitGame() {
        if (!isSelectBtn) {
            isSelectBtn = true;
            //  ゲームの実行状況に合わせて処理を行う
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_WEBGL
        Application.OpenURL("https://www.yahoo.co.jp/");

#else //UNITY_STANDALONE
        Application.Quit();

#endif
        }
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        if (!isSelectBtn) {
            isSelectBtn = true;
            AnimePopup(0f);
            //Destroy(gameObject, 0.5f);
            //TransitionManager.instance.openBtn.interactable = true;

            // バトル中ならバトルを再開
            //if (TransitionManager.instance.sceneState == SCENE_STATE.Stage) {
            //    ReturnBattle();
            //}
        }
    }
}
