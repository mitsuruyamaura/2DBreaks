using UnityEngine;

/// <summary>
/// ボールの制御用クラス
/// </summary>
public class BallController : MonoBehaviour 
{
    [Header("ボールの速度")]
    public float speed;
    [Header("ボールの威力。現在はプレイヤーへのダメージに使用")]
    public int power;
  
    [Header("ゲーム管理クラス")]
    public GameMaster gameMaster;

    private Rigidbody2D rb;
    private Vector2 breakDirection;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (gameMaster.gameState == GAME_STATE.READY) {
            ShotBall();
        }
        if (gameMaster.gameState == GAME_STATE.GAME_OVER) {
            StopMoveBall();
        }
    }

    /// <summary>
    /// ボールを打ち出す
    /// </summary>
    public void ShotBall() {
        if (Input.GetMouseButtonDown(0)) {
            // 角度によって速度が変化してしまうのでnormalizedで正規化して同じ速度ベクトルにする
            Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;          

            // ボールを打ち出す(摩擦や空気抵抗、重力を切ってあるので、ずっと同じ速度で動き続ける)
            rb.velocity = -direction * speed * transform.localScale.x;

            gameMaster.gameState = GAME_STATE.PLAY;
        }
    }

    /// <summary>
    /// ボールを止める
    /// </summary>
    public void StopMoveBall() {
        breakDirection = rb.velocity;
        // ボールの速度ベクトルを0にして止める
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// ボールを同じ速度で再度動かす
    /// </summary>
    public void RestartMoveBall() {
        rb.velocity = breakDirection;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Player") {
            // ボールの向きをいれる
            Vector2 dir = transform.position - col.gameObject.transform.position;

            // ボールに速度を加える（Randomな速度で跳ね返す）
            rb.velocity = dir * speed * Random.Range(1.0f, 2.0f) * transform.localScale.x;
        }
    }
}
