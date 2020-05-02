using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Titleシーンの管理クラス
/// </summary>
public class TitleManeger : MonoBehaviour {

    [Header("スタートボタン")]
    public Button startBtn;

    private void Start() {
        startBtn.onClick.AddListener(OnClickStartGame);
    }

    /// <summary>
    /// ゲームスタート処理。Menuシーンへ遷移する
    /// </summary>
    public void OnClickStartGame() {
            startBtn.interactable = false;
            // シーン遷移とフェイドアウト処理
            StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }
}
