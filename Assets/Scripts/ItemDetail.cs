using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class ItemDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgItem;

    private UnityEvent unityEventItemEffect;

    /// <summary>
    /// アイテムの設定
    /// </summary>
    /// <param name="itemEffect"></param>
    public void SetUpItemDetail(int itemNo, UnityAction itemEffect) {
        
        // TODO Image Resource.Load
        // イメージをアイテムに合わせて設定

        // UnityEvent初期化
        unityEventItemEffect = new UnityEvent();

        // アイテムの効果(メソッド)を登録
        unityEventItemEffect.AddListener(itemEffect);

        Debug.Log("SetUp ItemDetail");
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Liner") {
            return;
        }

        if (col.gameObject.tag == "CharaBall") {

            // アイテムの効果発動
            TriggerItemEffect();
            Debug.Log("Trigger ItemEffect");
        }
    }

    /// <summary>
    /// アイテムの効果発動
    /// </summary>
    private void TriggerItemEffect() {

        // アイテムに応じた効果のメソッドを実行
        unityEventItemEffect.Invoke();

        // TODO Dotween 取得エフェクト

        Destroy(gameObject, 0.5f);
    }
}
