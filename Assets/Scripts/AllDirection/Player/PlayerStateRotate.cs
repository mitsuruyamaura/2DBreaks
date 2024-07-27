using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnitState;
using UnityEngine;

public partial class Player {

    public class StateRotate : UnitStateBase {

        private Tween tween;

        public override void OnEnter(Player owner, UnitStateBase prevState) {

            // 方向を決定
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.localPosition;

            // 向きを変える処理(tween)が動いている場合には停止(連続クリック時の対応)
            tween?.Kill();

            // マウスの方向を向く
            tween = owner.transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward, direction), 0.25f)
                .SetEase(Ease.Linear)
                .SetLink(owner.gameObject)
                .OnComplete(() => owner.ChangeState(stateIdle));
        }
    }
}