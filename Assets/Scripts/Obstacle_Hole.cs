using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle_Hole : ObstacleBase
{
    protected override void BeforeTriggerEffect(CharaBall charaBall) {
        // 手球を１つ減らす
        charaBall.UpdateHp(-power);

        // スタート位置へ戻す
        StartCoroutine(battleManager.RestartCharaPosition(2.0f));
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        base.OnCollisionEnter2D(col);

        if (col.gameObject.tag == "EnemyBall") {
            if (col.gameObject.TryGetComponent(out EnemyBall enemy)) {
                // Sequence初期化
                Sequence sequence = DOTween.Sequence();

                // 敵を回転
                sequence.Append(enemy.transform.DOLocalRotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

                // 敵を破壊
                enemy.DestroyEnemy(sequence);
            }
        }

        if (col.gameObject.tag == "EnemyBall") {
            if (col.gameObject.TryGetComponent(out Enemy enemy)) {
                // Sequence初期化
                Sequence sequence = DOTween.Sequence();

                // 敵を回転
                sequence.Append(enemy.transform.DOLocalRotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

                // 敵を破壊
                enemy.DestroyEnemy(sequence);
            }
        }
    }
}
