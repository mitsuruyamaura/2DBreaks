using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボールの制御用クラス
/// </summary>
public class CharaBall : MonoBehaviour
{
    [Header("ボールの速度")]
    public float speed;
    [Header("ボールの威力。現在はプレイヤーへのダメージに使用")]
    public int power;

    public int hp;

    //[Header("ゲーム管理クラス")]
    //public GameMaster gameMaster;

    private Rigidbody2D rb;
    private Vector2 breakDirection;

    [SerializeField]
    private Image imgChara;

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


        //if (Input.GetMouseButtonDown(0))
        //{
            // 角度によって速度が変化してしまうのでnormalizedで正規化して同じ速度ベクトルにする
            //Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;

            // ボールを打ち出す(摩擦や空気抵抗、重力を切ってあるので、ずっと同じ速度で動き続ける)
            //rb.velocity = direction * speed * transform.localScale.x;

            //gameMaster.gameState = GAME_STATE.PLAY;
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
    }

    /// <summary>
    /// ボールを同じ速度で再度動かす
    /// </summary>
    public void RestartMoveBall()
    {
        rb.velocity = breakDirection;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Liner")
        {
            // ボールの向きをいれる
            Vector2 dir = transform.position - col.gameObject.transform.position;

            // ボールに速度を加える（Randomな速度で跳ね返す）
            rb.velocity = dir * speed * Random.Range(1.0f, 2.0f) * transform.localScale.x;
        }
    }

    /// <summary>
    /// Hpを増減
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateHp(int amount)
    {
        GameData.instance.currentHp += amount;
        GameData.instance.uiManager.UpdateDisplayGameTime();

        if (amount < 0 && !isBlinking) {
            isBlinking = true;
            // 点滅
            DOTween.Clear();
            Blink();
        }

        if (GameData.instance.currentHp <= 0) {
            GameData.instance.currentHp = 0;

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
