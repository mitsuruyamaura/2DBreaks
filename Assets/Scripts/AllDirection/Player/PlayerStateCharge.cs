using UnitState;
using UnityEngine;

public partial class Player {

    public class StateCharge : UnitStateBase {

        public override void OnUpdate(Player owner) {
            if (Input.GetMouseButton(0)) {

                if (owner.chargeTimer > owner.maxChargeCount) {
                    owner.chargeTimer = owner.maxChargeCount;
                    return;
                }

                // �`���[�W + �`���[�W���x�̒���
                owner.chargeTimer += Time.deltaTime * owner.countUpRate;

                // �G�t�F�N�g����

            } else if (Input.GetMouseButtonUp(0)) {
                // �`���[�W�U���@�`���[�W�������ԕ��������x�A�b�v
                owner.ChangeState(stateAttack);

                owner.chargeTimer = 0;

                // �G�t�F�N�g������ꍇ�ɂ͔j��

            }
        }
    }
}