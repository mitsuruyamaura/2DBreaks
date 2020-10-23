using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Rock : ObstacleBase
{
    public int hp;

    public int money;

    protected override void AfterTriggerEffect(CharaBall charaBall) {
        hp -= charaBall.power;

        Sequence effectSeq = DOTween.Sequence();
        effectSeq.Append(transform.DOShakeScale(1.0f).SetEase(Ease.Linear));

        if (hp <= 0) {
            // Moneyを加算
            battleManager.AddMoney(money);

            // 縮小して破壊
            effectSeq.Join(rectTransform.DOSizeDelta(new Vector2(0, rectTransform.sizeDelta.y), 1.0f).SetEase(Ease.Linear));
            Destroy(gameObject, 1.0f);
        }
    }
}
