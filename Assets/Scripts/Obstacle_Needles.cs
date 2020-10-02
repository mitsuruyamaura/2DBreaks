using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle_Needles : Hole
{
    [Header("移動時間。大きいほどゆっくり動く")]
    public float moveTime;

    [Header("回転させる場合はスイッチいれる")]
    public bool isRotate;

    protected override void Start()
    {
        base.Start();

        transform.DOMoveX(2, moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        if (isRotate) {
            transform.DORotate(new Vector3(0, 0, 360), 1.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }
    }
}
