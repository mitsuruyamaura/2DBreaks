using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

/// <summary>
/// ユニットの状態(ステート)
/// </summary>
namespace UnitState {

    /// <summary>
    /// ステートの抽象クラス
    /// </summary>
    public abstract class UnitStateBase {

        public virtual void OnEnter(Player owner, UnitStateBase prevState) { }

        public virtual void OnUpdate(Player owner) { }

        public virtual void OnExit(Player owner, UnitStateBase nextState) { }
    }



    //===========================
    // 今回は使用していないステートパターンの実装
    // StateProcessor の Excute にステートを入れて切り替えていく
    // UniRx でステート用の ReactiveProperty を監視する
    //===========================


    /// <summary>
    ///// ステートの実行を管理するクラス
    ///// </summary>
    //public class StateProcessor {
    //    // ステート本体
    //    public ReactiveProperty<UnitStateBase> unitState = new();

    //    // 実行ブリッジ
    //    public void Execute() => unitState.Value.executeAction();
    //}

    ///// <summary>
    ///// ステートの抽象クラス
    ///// </summary>
    //public abstract class UnitStateBase {

    //    // デリゲート
    //    public Action executeAction { get; set; }  // 各ステート用のメソッドを登録する

    //    // 実行処理
    //    public virtual void Execute() {
    //        executeAction?.Invoke();
    //    }

    //    // ステート名を取得
    //    public abstract string GetStateName();

    //}

    //    //===========================
    //    // 状態クラス
    //    //===========================

    //    /// <summary>
    //    /// 何もしていない状態
    //    /// </summary>
    //    public class UnitStateIdle : UnitStateBase {

    //    public override string GetStateName() {
    //        return "State : Idle";
    //    }
    //}

    ///// <summary>
    ///// チャージしている状態
    ///// </summary>
    //public class UnitStateCharge : UnitStateBase {

    //    public override string GetStateName() {
    //        return "State : Charge";
    //    }
    //}

    ///// <summary>
    ///// 攻撃している状態
    ///// </summary>
    //public class UnitStateAttack : UnitStateBase {

    //    public override string GetStateName() {
    //        return "State : Attack";
    //    }
    //}


    //public class UnitStateFlip : UnitStateBase {
    //    public override string GetStateName() {
    //        return "State : Flip";
    //    }
    //}
}