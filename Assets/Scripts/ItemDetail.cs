using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class ItemDetail : MonoBehaviour
{
    public ItemData itemData;   // TODO 多分もっていないくていい

    public Image imgItem;

    private UnityEvent unityEvent;

    private GameManager gameManager;


    public void SetUpItemDetail(ItemData itemData, GameManager gameManager) {
        this.itemData = itemData;
        this.gameManager = gameManager;

        // TODO Image Resource.Load

        unityEvent = new UnityEvent();

        unityEvent.AddListener(GetItemEffect(this.itemData.itemNo));

        Debug.Log("SetUp End Item");
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Liner") {
            return;
        }

        if (col.gameObject.tag == "CharaBall") {
            // TODO CharaBall取得して、引数で渡す(HPなどを参照できるように。GameDataで管理するなら不要)

            TriggerItemEffect();
            Debug.Log("Trigger");
        }
    }

    private void TriggerItemEffect() {
        unityEvent.Invoke();

        // TODO Dotween 取得エフェクト

        Destroy(gameObject, 0.5f);
    }

    public UnityAction GetItemEffect(int getItemNo) {
        switch (getItemNo) {
            case 0:
                return AddBattleTime;
            default:
                return null;
        }
    }

    public void GainHp() {

    }

    public void AddBattleTime() {
        gameManager.currentTime += (int)itemData.effectiveValue;
        Debug.Log("時間延長 : " + (int)itemData.effectiveValue);
    }

    public void TempSpeedUp() {
        // TODO Conditionクラスをアタッチ

    }

    public void TempAttackUp() {

    }

    public void TempInvincible() {

    }
}
