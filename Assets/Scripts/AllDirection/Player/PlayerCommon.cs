using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ステート処理以外の部分
/// このクラスがインスペクターにシリアライズされるクラス
/// </summary>
public partial class Player : UnitBase
{
    // インスペクターに変数をしたい場合にはこのクラスに書く
    private float chargeTimer;

    [SerializeField]
    private float maxChargeCount = 2.0f;

    [SerializeField]
    private float countUpRate = 1.0f;   // 補正

    [SerializeField]
    private float BasePower = 50.0f;

    [SerializeField]
    private float limitX = 19.5f;   //　StageData から取得できるようにあとで変える

    [SerializeField]
    private float limitY = 12.5f;


    protected override void Start() {
        base.Start(); // UnitBase の Start を参照

        OnStart();
    }

    protected override void SetUpUnit(UnitData unitData) {
        base.SetUpUnit(unitData);

        this.OnCollisionEnter2DAsObservable()
            .Where(x => x.gameObject.TryGetComponent(out enterUnit))
            .Subscribe(x => {
                if (currentState != stateAttack) {
                    CalculateHp(enterUnit.GetAttackPower());
                }
            })
            .AddTo(gameObject);
    }


    private void Reset() {
        TryGetComponent(out rb);
    }

    void Update()
    {
        // 座標制限
        Vector3 pos = new(Mathf.Clamp(transform.position.x, -limitX, limitX), Mathf.Clamp(transform.position.y, -limitY, limitY));
        transform.position = pos;

        // 各ステートで Update が必要な場合には override して利用するので、それを実行するための処理
        OnUpdate();
    }

    void FixedUpdate() {

        // 動いている間は徐々に停止させる
        if (rb.velocity != Vector2.zero) {
            rb.velocity *= 0.9f;

            // 双方の浮動小数点の値が 0 に近いなら
            if (Mathf.Approximately(0, rb.velocity.x) && Mathf.Approximately(0, rb.velocity.y)) {
                rb.velocity = Vector2.zero;
            }
        }
    }
}