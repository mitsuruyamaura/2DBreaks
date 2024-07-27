using UnitState;

/// <summary>
/// ステート処理の部分
/// </summary>
public partial class Player {

    private static readonly StateIdle stateIdle = new();
    private static readonly StateRotate stateRotate = new();
    private static readonly StateAttack stateAttack = new();
    private static readonly StateCharge stateCharge = new();

    private UnitStateBase currentState = stateIdle;   // 初期設定しないと null になる

    /// <summary>
    /// 最初に一度だけ実行する
    /// </summary>
    private void OnStart() {
        currentState.OnEnter(this, null);       
    }

    /// <summary>
    /// 各ステートでの Update の役割
    /// </summary>
    private void OnUpdate() {
        currentState.OnUpdate(this);
    }

    /// <summary>
    /// ステートの切り替え
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(UnitStateBase nextState) {
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
        //UnityEngine.Debug.Log(currentState);
    }


    private void OnDeath() {
        //ChangeState();
    }


    //=========================================
    // ステートパターンの別の実装方法
    // クラス分けではなくて、メソッドで分岐を作り、デリゲートに登録して利用するタイプ
    //=========================================

    //private string prevStateName;

    //public StateProcessor stateProcessor = new();
    //public UnitStateIdle stateIdle = new();
    //public UnitStateCharge stateCharge = new();
    //public UnitStateAttack stateAttack = new();

    //private void Start() {
    //    // ステートの初期化
    //    stateProcessor.unitState.Value = stateIdle;

    //    // デリゲートでメソッドを登録しておいて、状態に応じて実行するメソッドを変える
    //    stateIdle.executeAction = Idle;
    //    stateCharge.executeAction = Charge;
    //    stateAttack.executeAction = Attack;

    //    // ステートの値を行動し、変更されたら実行処理を行うようにする
    //    stateProcessor.unitState
    //        .Where(_ => stateProcessor.unitState.Value.GetStateName() != prevStateName)
    //        .Subscribe(_ => {
    //            Debug.Log($"Now State: { stateProcessor.unitState.Value.GetStateName()}");
    //            prevStateName = stateProcessor.unitState.Value.GetStateName();
    //            stateProcessor.Execute();
    //        })
    //        .AddTo(this);
    //}

    //public void Idle() {
    //    Debug.Log("State が Idle に遷移しました。");
    //}


    //public void Charge() {
    //    Debug.Log("State が Charge に遷移しました。");
    //}

    //public void Attack() {
    //    Debug.Log("State が Attack に遷移しました。");
    //}

    //protected override void SetUpUnit(UnitData unitData) {
    //    base.SetUpUnit(unitData);
    //    //stateProcessor.unitState.Value = stateAttack;

    //    this.OnCollisionEnter2DAsObservable()
    //        .Where(x => x.gameObject.TryGetComponent(out enterUnit))
    //        .Subscribe(x => {
    //            if (stateProcessor.unitState.Value.GetStateName() != "State : Attack") {
    //                //Debug.Log(enterUnit.attackPower + " " + enterUnit);
    //                CalculateHp(enterUnit.GetAttackPower());
    //            }
    //        })
    //        .AddTo(gameObject);
    //}
}