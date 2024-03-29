﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle_Needles : ObstacleBase
{
    [Header("移動時間。大きいほどゆっくり動く")]
    public float moveTime;

    [Header("X軸の移動距離")]
    public float posX;

    [Header("Y軸の移動距離")]
    public float posY;

    [Header("回転させる場合はスイッチいれる")]
    public bool isRotate;

    Sequence loopSeq;

    protected override void Start()
    {
        base.Start();

        LoopMove();
    }

    private void LoopMove() {
        loopSeq = DOTween.Sequence();

        // 移動。XとY両方に数値が入ると斜めに動く
        loopSeq.Append(transform.DOMove(new Vector2(posX, posY), moveTime)).SetRelative().SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        loopSeq.Join(transform.DOShakeScale(moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental));
        // 回転
        if (isRotate) {
            transform.rotation = Quaternion.identity;
            loopSeq.Join(transform.DORotate(new Vector3(0, 0, 360), 1.0f, RotateMode.FastBeyond360)).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }
    }

    protected override void BeforeTriggerEffect(CharaBall charaBall) {
        // TODO 時間を減少させる
        //charaBall.UpdateHp(-power);

        // TODO キャラ点滅
        

    }

    protected override void AfterTriggerEffect(CharaBall charaBall) {
        LoopMove();
    }
}
