using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnitState;
using UnityEngine;

public partial class Player {

    public class StateRotate : UnitStateBase {

        private Tween tween;

        public override void OnEnter(Player owner, UnitStateBase prevState) {

            // ����������
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.localPosition;

            // ������ς��鏈��(tween)�������Ă���ꍇ�ɂ͒�~(�A���N���b�N���̑Ή�)
            tween?.Kill();

            // �}�E�X�̕���������
            tween = owner.transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward, direction), 0.25f)
                .SetEase(Ease.Linear)
                .SetLink(owner.gameObject)
                .OnComplete(() => owner.ChangeState(stateIdle));
        }
    }
}