using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ItemManager : MonoBehaviour
{
    public List<ItemData> itemDataList = new List<ItemData>();

    public ItemDetail itemDetailPrefab;

    private BattleManager battleManager;    

    /// <summary>
    /// ItemManagerの初期設定
    /// </summary>
    /// <param name="battleManager"></param>
    public void SetUpItemManager(BattleManager battleManager) {
        this.battleManager = battleManager;
    }


    /// <summary>
    /// アイテムの生成と効果設定
    /// </summary>
    public void GenerateItem(Transform canvasTran, Vector2 generatePos) {
        // アイテムを生成
        ItemDetail item = Instantiate(itemDetailPrefab, canvasTran, false);
        item.transform.position = generatePos;

        // ランダムな効果を１つ設定
        int itemNo = Random.Range(0, itemDataList.Count);

        // アイテムの設定
        item.SetUpItemDetail(itemNo, GetItemEffect(itemNo));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getItemNo"></param>
    /// <returns></returns>
    private UnityAction GetItemEffect(int getItemNo) {
        switch (getItemNo) {
            case 0:
                return AddBattleTime;
            case 1:
                return GainHp;
            default:
                return null;
        }
    }

    /// <summary>
    /// 残り時間の延長
    /// </summary>
    public void AddBattleTime() {
        battleManager.currentTime += (int)itemDataList.Find(x => x.itemEffectType == ItemData.ItemEffectType.AddBattleTime).effectiveValue;
        Debug.Log("時間延長 : " + (int)itemDataList.Find(x => x.itemEffectType == ItemData.ItemEffectType.AddBattleTime).effectiveValue);
    }

    /// <summary>
    /// 手球を増加
    /// </summary>
    public void GainHp() {
        battleManager.CharaBall.Hp += (int)itemDataList.Find(x => x.itemEffectType == ItemData.ItemEffectType.GainHp).effectiveValue;
        battleManager.uiManager.UpdateDisplayIconRemainingBall(battleManager.CharaBall.Hp);
    }

    public void TempSpeedUp() {
        // TODO Conditionクラスをアタッチ

    }

    public void TempAttackUp() {

    }

    public void TempInvincible() {

    }
}
