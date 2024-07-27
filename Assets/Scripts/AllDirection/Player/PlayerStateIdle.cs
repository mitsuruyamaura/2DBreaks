using UnitState;
using UnityEngine;

public partial class Player {

    public class StateIdle : UnitStateBase {

        /// <summary>
        /// �X�e�[�g�؂�ւ��̋N�_�ƂȂ�B�A�N�V�����̐؂�ւ������ł�����
        /// </summary>
        /// <param name="owner"></param>
        public override void OnUpdate(Player owner) {
            // �E�N���b�N�@��]
            if (Input.GetMouseButtonDown(1)) {
                owner.ChangeState(stateRotate);
            }

            // ���N���b�N�@�`���[�W+�U��
            if (Input.GetMouseButtonDown(0)) {
                owner.ChangeState(stateCharge);
            }
        }
    }
}