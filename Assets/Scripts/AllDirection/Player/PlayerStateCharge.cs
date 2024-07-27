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

                // チャージ + チャージ速度の調整
                owner.chargeTimer += Time.deltaTime * owner.countUpRate;

                // エフェクト生成

            } else if (Input.GetMouseButtonUp(0)) {
                // チャージ攻撃　チャージした時間分だけ速度アップ
                owner.ChangeState(stateAttack);

                owner.chargeTimer = 0;

                // エフェクトがある場合には破棄

            }
        }
    }
}