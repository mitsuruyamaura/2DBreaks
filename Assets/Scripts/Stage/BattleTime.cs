using UnityEngine;
using TMPro;

/// <summary>
/// バトルの制限時間の管理クラス
/// </summary>
public class BattleTime : MonoBehaviour
{
    [Header("制限時間の設定値")]
    public int currentBattleTime;
    [Header("制限時間の表示")]
    public TMP_Text battleTimeText;

    private float timer;           //時間計測用
    private GameMaster gameMaster;

    private void Start() {
        gameMaster = GetComponent<GameMaster>();
        DisplayBattleTime(currentBattleTime);
    }

    void Update() {
        if ((gameMaster.gameState == GAME_STATE.PLAY) || (gameMaster.gameState == GAME_STATE.WARNING)) {
            timer += Time.deltaTime;
            if (timer >= 1) {
                timer = 0;
                currentBattleTime--;

                if (currentBattleTime < 0) {
                    // ゲーム状態を変更
                    gameMaster.GameUp(GAME_STATE.GAME_OVER);
                }
                // 時間表示を更新する
                DisplayBattleTime(currentBattleTime);
            }
        }
    }

    /// <summary>
    /// 制限時間の表示を現在の時間に更新して[分:秒]で表示する
    /// </summary>
    private void DisplayBattleTime(int limitTime) {
        battleTimeText.text = ((int)(limitTime / 60)).ToString("00") + ":" + ((int)limitTime % 60).ToString("00");
    }

    /// <summary>
    /// 制限時間の表示を消す
    /// </summary>
    public void HideBattleTime() {
        battleTimeText.text = " ";
    }
}
