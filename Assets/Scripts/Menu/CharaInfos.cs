using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// キャラ情報の登録と移動制御用クラス
/// </summary>
public class CharaInfos : MonoBehaviour
{
    [Header("キャラの速度")]
    public float speed;
    [Header("キャラのステージ")]
    public int myStageNo;
    [Header("タップ時のイベント登録")]
    public EventTrigger eventTrigger;

    private Rigidbody2D rb;
    private Vector2 direction; // ボールの方向
    private bool isTouch;      // 重複タップ防止

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        // イベントトリガーに登録
        eventTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((x) => OnClickChara());
        eventTrigger.triggers.Add(entry);

        // キャラ移動
        MoveChara();
    }

    /// <summary>
    /// キャラをランダムな方向へ移動させる
    /// </summary>
    private void MoveChara() {
        direction = new Vector2(Random.Range(-4.5f, 4.5f), 1).normalized;
        rb.velocity = direction * speed * transform.localScale.x;
    }

    /// <summary>
    /// タップされたキャラのステージ情報を取得し、Stageシーンへ遷移する
    /// </summary>
    public void OnClickChara() {
        if (!isTouch) {
            // 連続タップ防止フラグ
            isTouch = true;

            // ステージ番号取得
            SelectStage.stageNo = myStageNo;

            // シーン遷移とフェイドアウト処理
            StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
        }
    }
}
