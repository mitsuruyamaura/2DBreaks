using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Rock : Hole
{
    public int hp;
    Sequence effectSeq;

    protected override void Start() {
        base.Start();
        effectSeq = DOTween.Sequence();
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        base.OnCollisionEnter2D(col);
    }

    protected override void AfterTriggerEffect(CharaBall charaBall) {
        hp -= charaBall.power;
     
        effectSeq.Append(transform.DOShakeScale(1.0f).SetEase(Ease.Linear));

        if (hp <= 0) {
            effectSeq.Append(rectTransform.DOSizeDelta(new Vector2(0, rectTransform.sizeDelta.y), 0.5f).SetEase(Ease.Linear));
            Destroy(gameObject, 0.5f);
        }
    }
}
