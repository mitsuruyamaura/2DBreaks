using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData {

    public int itemNo;
    public float effectiveValue;    // 効力
    public float duration;          // 効果時間

    public enum ItemEffectType {
        AddBattleTime,
        GainHp,
    }

    public ItemEffectType itemEffectType;
}
