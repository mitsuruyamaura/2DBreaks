using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

/// <summary>
/// ���j�b�g�̏��(�X�e�[�g)
/// </summary>
namespace UnitState {

    /// <summary>
    /// �X�e�[�g�̒��ۃN���X
    /// </summary>
    public abstract class UnitStateBase {

        public virtual void OnEnter(Player owner, UnitStateBase prevState) { }

        public virtual void OnUpdate(Player owner) { }

        public virtual void OnExit(Player owner, UnitStateBase nextState) { }
    }



    //===========================
    // ����͎g�p���Ă��Ȃ��X�e�[�g�p�^�[���̎���
    // StateProcessor �� Excute �ɃX�e�[�g�����Đ؂�ւ��Ă���
    // UniRx �ŃX�e�[�g�p�� ReactiveProperty ���Ď�����
    //===========================


    /// <summary>
    ///// �X�e�[�g�̎��s���Ǘ�����N���X
    ///// </summary>
    //public class StateProcessor {
    //    // �X�e�[�g�{��
    //    public ReactiveProperty<UnitStateBase> unitState = new();

    //    // ���s�u���b�W
    //    public void Execute() => unitState.Value.executeAction();
    //}

    ///// <summary>
    ///// �X�e�[�g�̒��ۃN���X
    ///// </summary>
    //public abstract class UnitStateBase {

    //    // �f���Q�[�g
    //    public Action executeAction { get; set; }  // �e�X�e�[�g�p�̃��\�b�h��o�^����

    //    // ���s����
    //    public virtual void Execute() {
    //        executeAction?.Invoke();
    //    }

    //    // �X�e�[�g�����擾
    //    public abstract string GetStateName();

    //}

    //    //===========================
    //    // ��ԃN���X
    //    //===========================

    //    /// <summary>
    //    /// �������Ă��Ȃ����
    //    /// </summary>
    //    public class UnitStateIdle : UnitStateBase {

    //    public override string GetStateName() {
    //        return "State : Idle";
    //    }
    //}

    ///// <summary>
    ///// �`���[�W���Ă�����
    ///// </summary>
    //public class UnitStateCharge : UnitStateBase {

    //    public override string GetStateName() {
    //        return "State : Charge";
    //    }
    //}

    ///// <summary>
    ///// �U�����Ă�����
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