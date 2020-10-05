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
        StartCoroutine(gameManager.ResetCharaPosition(2.0f));
    }
}
