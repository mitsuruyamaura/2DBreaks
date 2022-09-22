using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ObstacleBall : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Rigidbody2D rb;
    private float minSpeed;
    private float maxSpeed;
    private float slowSpeed = 1.5f;
    private float timer;
    private float moveInterval = 5.0f;

    /// <summary>
    /// 障害物の初期設定
    /// </summary>
    /// <param name="mosaicManager"></param>
    public void SetUpObstacleBall(MosaicManager mosaicManager, float[] speeds) {
        minSpeed = speeds[0];
        maxSpeed = speeds[1];

        // フィーバータイムではなく、プレイ中のときには、一定時間ごとにランダム移動を繰り返す
        this.UpdateAsObservable()
            .Where(_ => !mosaicManager.IsFeverTime.Value && mosaicManager.gameState == MosaicManager.GameState.Play)
            .Subscribe(_ => RandomShot())
            .AddTo(this);

        // フィーバータイムではなく、プレイ中のときには、選択中のグリッドに接触した場合、アウトにする
        this.OnTriggerEnter2DAsObservable()
            .Where(_ => !mosaicManager.IsFeverTime.Value && mosaicManager.gameState == MosaicManager.GameState.Play)
            .Subscribe(collision =>
            {
                //if (collision.gameObject.CompareTag("Untagged")) {
                //    Debug.Log("!!");
                //}

                //Debug.Log(collision.gameObject.name);
                if (collision.TryGetComponent(out TileGridDetail tileGridDetail)) {
                    //Debug.Log(collision.gameObject.name);
                    if (tileGridDetail.IsSelected) {
                        //Debug.Log("アウト");
                        StopMoveBall();

                        mosaicManager.FailureErase();
                    }
                }
            })
            .AddTo(this);

        if (TryGetComponent(out rb)) {
            // ボールを発射
            ShotBall(false);
        }
    }

    /// <summary>
    /// 障害物の移動を再度行い、移動先をランダム化
    /// 壁の間を行ったり来たりしてしまうことも防ぐ
    /// </summary>
    private void RandomShot() {
        timer += Time.deltaTime;

        if (timer >= moveInterval) {
            timer = 0;
            ShotBall(false);
        }
    }

    /// <summary>
    /// ボールを発射
    /// </summary>
    public void ShotBall(bool isSlowDown) {
        speed = isSlowDown ? slowSpeed : Random.Range(minSpeed, maxSpeed);

        // 角度によって速度が変化してしまうのでnormalizedで正規化して同じ速度ベクトルにする
        Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;

        // ボールを打ち出す(摩擦や空気抵抗、重力を切ってあるので、ずっと同じ速度で動き続ける)
        rb.velocity = -direction * speed * transform.localScale.x;
    }

    /// <summary>
    /// ボールを止める
    /// </summary>
    public void StopMoveBall() {
        // ボールの速度ベクトルを0にして止める
        rb.velocity = Vector2.zero;
    }
}