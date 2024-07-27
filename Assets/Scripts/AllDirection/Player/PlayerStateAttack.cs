using System.Collections;
using System.Collections.Generic;
using UnitState;
using UnityEngine;
using Cysharp.Threading.Tasks;


public partial class Player {

    public class StateAttack : UnitStateBase {

        public override void OnEnter(Player owner, UnitStateBase prevState) {
            owner.rb.AddForce(owner.transform.up * owner.BasePower * owner.chargeTimer, ForceMode2D.Impulse);
            DelayChangeStateAsync(owner).Forget();
        }

        /// <summary>
        /// ステート変更
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private async UniTask DelayChangeStateAsync(Player owner) {
            var token = owner.GetCancellationTokenOnDestroy();
            await UniTask.Delay(250, cancellationToken : token);  // 初期値のあるものは変数を記入すると指定したものだけを上書きできる
            owner.ChangeState(stateIdle);
        }
    }
}