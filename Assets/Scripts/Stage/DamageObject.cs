using UnityEngine;

/// <summary>
/// ダメージ発生用クラス
/// </summary>
public class DamageObject : MonoBehaviour
{
    [Header("ライフ管理クラス")]
    public LifeManager lifeManager;
    [Header("被ダメージ用エフェクト管理クラス")]
    public FlushController flushController;

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ball") {
            // Lifeを減らして画面に被ダメージ用エフェクトを表示
            lifeManager.ApplyDamage(col.gameObject.GetComponent<BallController>().power);
            flushController.StartFlushEffect();
        }
    }
}
