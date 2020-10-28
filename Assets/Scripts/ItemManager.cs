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
        //int itemNo = Random.Range(0, itemDataList.Count);
        ItemData.ItemEffectType itemEffectType = (ItemData.ItemEffectType)Random.Range(0, itemDataList.Count);

        // アイテムの設定
        item.SetUpItemDetail((int)itemEffectType, GetItemEffect(itemEffectType));
    }

    /// <summary>
    /// アイテムの効果用のメソッドを判定して戻す
    /// </summary>
    /// <param name="itemEffectType"></param>
    /// <returns></returns>
    private UnityAction GetItemEffect(ItemData.ItemEffectType itemEffectType) {
        switch (itemEffectType) {
            case ItemData.ItemEffectType.AddBattleTime:
                return AddBattleTime;
            case ItemData.ItemEffectType.GainHp:
                return GainHp;
            case ItemData.ItemEffectType.TempSpeedUp:
                return TempSpeedUp;
            case ItemData.ItemEffectType.TempAttackUp:
                return TempAttackUp;
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

    /// <summary>
    /// 速度アップ
    /// </summary>
    public void TempSpeedUp() {
        // TODO Conditionクラスをアタッチ 重複チェックは不要
        battleManager.CharaBall.gameObject.AddComponent<ConditionSpeedUp>();

        Debug.Log("速度アップ 付与");
    }

    /// <summary>
    /// 攻撃力アップ
    /// </summary>
    public void TempAttackUp() {
        battleManager.CharaBall.gameObject.AddComponent<ConditionAttackPowerUp>();

        Debug.Log("攻撃力アップ 付与");
    }

    public void TempInvincible() {

    }
}
