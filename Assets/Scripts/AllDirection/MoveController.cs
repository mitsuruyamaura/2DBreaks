using UnityEngine;
using DG.Tweening;

public class MoveController : MonoBehaviour
{
    //private Rigidbody2D rb;

    //[SerializeField]
    //private float blowPower;

    private Tween tween;

    [SerializeField]
    private float limitX = 19.5f;　　　//　StageData から取得できるようにあとで変える

    [SerializeField]
    private float limitY = 12.5f;


    private void Reset() {
        //TryGetComponent(out rb);
    }

    void Start()
    {
        Reset();
    }


    void Update() {

        // 右クリック判定
        if (Input.GetMouseButtonDown(1)) {
            // 方向を決定
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.localPosition;
            //Debug.Log(direction);

            // クリックした地点へ移動(移動させないようにした。向きを変えるだけ)
            //MoveToClickPoint(direction.normalized);

            // 向きを変える処理(tween)が動いている場合には停止(連続クリック時の対応)
            tween?.Kill();

            // マウスの方向を向く
            tween = transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward, direction), 0.25f).SetEase(Ease.Linear).SetLink(gameObject);
        }

        // 座標制限
        Vector3 pos = new(Mathf.Clamp(transform.position.x, -limitX, limitX), Mathf.Clamp(transform.position.y, -limitY, limitY));
        transform.position = pos;

        // 移動している間はマウスカーソルの方向を自動的に向く　→　DORotateQuaternion に変えた
        //if (rb.velocity != Vector2.zero) {
            //transform.rotation = 
                //Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.localPosition), 0.1f);
        //}
    }

    /// <summary>
    /// 右クリックした地点へ移動(利用していない)
    /// </summary>
    /// <param name="direction"></param>
    //private void MoveToClickPoint(Vector2 direction) {
    //    rb.velocity = Vector2.zero;
    //    rb.velocity = direction * blowPower;
    //}

    // 移動をしなくしたので、Attack の方へ移管する
    //void FixedUpdate() {
    //    // 動いている間は徐々に停止させる
    //    if (rb.velocity != Vector2.zero) {
    //        rb.velocity *= 0.995f;
    //    }
    //}
}
