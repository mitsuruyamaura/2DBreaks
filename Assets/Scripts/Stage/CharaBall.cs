using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボールの制御用クラス
/// </summary>
public class CharaBall : MonoBehaviour
{
    [Header("手球の速度")]
    public float speed;
    [Header("手球の攻撃力")]
    public int power;

    public int hp;




    private Vector2 procVelocity = Vector2.zero;   // Velocity計算保持用

    private Rigidbody2D rb;

    [SerializeField]
    private CapsuleCollider2D capsuleCol;

    public BattleManager battleManager;    // TODO 後でPrivateにする
    //　未

    //[Header("ゲーム管理クラス")]
    //public GameMaster gameMaster;

    private Vector2 breakDirection;

    [SerializeField]
    private Image imgChara;

    [SerializeField]
    private Button btnChara;



    private const float AnimeTimeSec = 0.25f;
    private const int AnimeRepeatCount = 3;
    private int count;
    private float defaultAlpha;
    private bool isBlinking = false;
    private string blinkLayerName = "BlinkPlayer";
    private string defaultLayerName;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultAlpha = imgChara.color.a;
        defaultLayerName = LayerMask.LayerToName(gameObject.layer);
    }

    /// <summary>
    /// 手球の初期設定。インスタンスした際に呼び出す
    /// </summary>
    /// <param name="battelManager"></param>
    public void SetUpCharaBall(BattleManager battelManager) {
        this.battleManager = battelManager;
        hp = GameData.instance.charaBallHp;
    }

    void Update()
    {
        //if (gameMaster.gameState == GAME_STATE.READY)
       // {
            ShotBall();
        //}
       // if (gameMaster.gameState == GAME_STATE.GAME_OVER)
        //{
           // StopMoveBall();
        //}
    }

    /// <summary>
    /// ボールを打ち出す
    /// </summary>
    public void ShotBall()
    {
        //// マウスクリック時
        //if (Input.GetMouseButtonDown(0) && !isShooted) {

        //    // 画面のクリックした位置からray（見えない光線）を発射
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    // rayの先のオブジェクト取得するための変数を宣言
        //    RaycastHit2D rayCastHit;

        //    // Physics.Raycastメソッドを利用し、光線がコライダーがぶつかるか判定。
        //    // ray変数は発射する始点と方向、true時にoutパラメータ修飾子用の戻り値が入る変数には rayCastHit変数を用意。
        //    Physics2D.Raycast(ray, out rayCastHit);

        //    // rayの当たった位置 - ボール位置間の計算を行い、ベクトルを取得（y座標のみボールの座標を採用）
        //    Vector3 rayHitPosition = new Vector3(rayCastHit.point.x, this.transform.position.y, rayCastHit.point.z);    // y座標のみボールと揃えてます

        //    // クリックした位置の遠近によって力が変化しないようにベクトルを正規化
        //    Vector3 normalDirection = (rayHitPosition - this.transform.position).normalized;

        //    // クリックした位置方向にボールを飛ばす
        //    rb.AddForce(normalDirection * speed * transform.localScale.x);

        //    // 打ち出し済判定をtrueにして2回以上ボールを発射できないようにする
        //    isShooted = true;
        //}


        //if (Input.GetMouseButtonDown(0)) {
        //   // 角度によって速度が変化してしまうのでnormalizedで正規化して同じ速度ベクトルにする
        //   Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;

        //    // ボールを打ち出す(摩擦や空気抵抗、重力を切ってあるので、ずっと同じ速度で動き続ける)
        //    rb.velocity = direction * speed * transform.localScale.x;

        //    //gameMaster.gameState = GAME_STATE.PLAY;
        //}
    }

    /// <summary>
    /// ボールを止める
    /// </summary>
    public void StopMoveBall()
    {
        breakDirection = rb.velocity;
        // ボールの速度ベクトルを0にして止める
        rb.velocity = Vector2.zero;

        // ボールを弾けないようにする
        ChangeActivateCollider(false);
    }

    /// <summary>
    /// ボールのコライダー制御
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ChangeActivateCollider(bool isSwitch) {
        capsuleCol.enabled = isSwitch;
    }

    /// <summary>
    /// ボールを同じ速度で再度動かす
    /// </summary>
    public void RestartMoveBall()
    {
        rb.velocity = breakDirection;
    }

    /// <summary>
    /// 弾いたり、弾かれた際の処理
    /// </summary>
    /// <param name="col"></param>
    private void OnCollisionEnter2D(Collision2D col)
    {                
        // Linerで弾いた場合
        if (col.gameObject.tag == "Liner") 
        {
            // ボールの向きをいれる
            Vector2 dir = transform.position - col.gameObject.transform.position;

            // ボールに速度を加える
            rb.velocity = dir * speed;    //  * transform.localScale.x   // （混乱したらRandomな速度で跳ね返す） * Random.Range(1.0f, 2.0f) 

            // 次の計算用にVelocityの値を保持しておく
            procVelocity = rb.velocity;
        }

        // 的球や壁に接触した場合
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "EnemyBall") 
        {
            // 接触したオブジェクトの接触情報を壁に垂直な単位ベクトルとして取得
            Vector2 normalVector = col.contacts[0].normal;

            // 跳ね返り用のベクトル(反射角度)をReflectメソッドを利用して計算(第1引数でボールの速度、第2引数は壁に垂直な単位ベクトル)
            Vector2 reflectVector = Vector2.Reflect(procVelocity, normalVector);

            // 速度を更新
            rb.velocity = reflectVector;

            // 次の計算用にVelocityの値を保持しておく
            procVelocity = rb.velocity;
        }
    }

    /// <summary>
    /// Hpを増減
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateHp(int amount)
    {
        battleManager.currentHp += amount;

        // UI上にある手球を減らす
        battleManager.uiManager.UpdateDisplayIconRemainingBall(battleManager.currentHp);
       
        // TODO HPゲージ作ったらUIの更新処理追加

        if (amount < 0 && !isBlinking) {
            isBlinking = true;
            // 点滅
            DOTween.Clear();
            Blink();
        }

        if (battleManager.currentHp <= 0) {
            battleManager.currentHp = 0;

            battleManager.gameState = BattleManager.GameState.Result;

            // GameOver
            rb.velocity *= 0.96f;
        }
    }

    /// <summary>
    /// キャラを点滅
    /// </summary>
    private void Blink() {
        gameObject.layer = LayerMask.NameToLayer(blinkLayerName);
        var startColor = imgChara.color;
        startColor.a = 0f;
        imgChara.color = startColor;

        DOTween.ToAlpha(() => imgChara.color, value => imgChara.color = value, defaultAlpha, AnimeTimeSec)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                count += 1;
                if (count < AnimeRepeatCount) {
                    // 一定回数繰り返し.
                    Blink();
                } else {
                    isBlinking = false;
                    gameObject.layer = LayerMask.NameToLayer(defaultLayerName);
                    count = 0;
                }
            });
    }
}
