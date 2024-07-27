using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnemyController : UnitBase {

    protected override void SetUpUnit(UnitData unitData) {
        base.SetUpUnit(unitData);

        this.OnCollisionEnter2DAsObservable()
            .Where(x => x.gameObject.TryGetComponent(out enterUnit)) 
            .Subscribe(x => {
                //if (CurrentUnitState != UnitState.Guard) {
                    //Debug.Log(enterUnit.GetAttackPower() + " " + enterUnit);
                    CalculateHp(enterUnit.GetAttackPower());
                //}
            })
            .AddTo(gameObject);
    }

    // ˆÚ“®•û–@

}