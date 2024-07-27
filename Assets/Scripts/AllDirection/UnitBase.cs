using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;


//public enum UnitState {
//    Wait,
//    Charge,
//    Attack,
//    Guard
//}

public class UnitBase : MonoBehaviour, IDamageable
{
    protected UnitData unitData;

    [SerializeField]
    protected int hp;
    protected Rigidbody2D rb;
    protected UnitBase enterUnit = null;
    //protected UnitState currentUnitState;
    //public UnitState CurrentUnitState { get => currentUnitState; set => currentUnitState = value; }

    public int id;


    // 購読


    protected virtual void Start() {
        // デバッグ用
        unitData = yamap.DataBaseManager.instance.GetUnitData(id);
        SetUpUnit(unitData);
    }

    protected virtual void SetUpUnit(UnitData unitData) {
        TryGetComponent(out rb);
        this.unitData = unitData;
        hp = unitData.hp;
    }


    protected virtual void CalculateHp(int attackPower) {
        //Debug.Log(attackPower);
        hp = Mathf.Clamp(hp -= attackPower, 0, unitData.hp);
        if (hp <= 0) {
            GameObject effect = Instantiate(yamap.DataBaseManager.instance.GetEffectData(unitData.effectType), transform.position, Quaternion.identity);
            Destroy(effect, 1.5f);

            Destroy(gameObject, 0.5f);  // 要調整。Mass も考慮する 
        }
    }


    public int GetAttackPower() {
        return unitData.attackPower;
    }
}