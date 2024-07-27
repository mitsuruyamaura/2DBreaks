using UnitState;
using UnityEngine;

public partial class Player {

    public class StateIdle : UnitStateBase {

        /// <summary>
        /// ステート切り替えの起点となる。アクションの切り替え部分でもある
        /// </summary>
        /// <param name="owner"></param>
        public override void OnUpdate(Player owner) {
            // 右クリック　回転
            if (Input.GetMouseButtonDown(1)) {
                owner.ChangeState(stateRotate);
            }

            // 左クリック　チャージ+攻撃
            if (Input.GetMouseButtonDown(0)) {
                owner.ChangeState(stateCharge);
            }
        }
    }
}