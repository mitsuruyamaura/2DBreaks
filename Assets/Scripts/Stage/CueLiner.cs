using UnityEngine;

/// <summary>
/// ボールを打ち返すラインの制御クラス
/// </summary>
public class CueLiner : MonoBehaviour
{
    //[Header("描画するラインの親オブジェクト")]
    //public Transform lineParent;
    [Header("生成するキューラインのプレファブ")]
    public GameObject cuelinePrefab;
    
    [Header("キューラインを生成するために必要なスワイプの最小の長さ")]
    public float mixLineLength;

    //[Header("引くラインの太さ")]
    //public float lineWidth;

    [Header("キューラインの出現時間")]
    public float duration;
    
    //[Header("ゲーム管理クラス")]
    public GameMaster gameMaster;

    private Vector2 touchPos; //マウスのクリック地点

    [SerializeField]
    private BattleManager battleManager;

    private void Update() {
        //if ((gameMaster.gameState == GAME_STATE.PLAY) || (gameMaster.gameState == GAME_STATE.WARNING)) {
        //    //ラインを引く
        //    DrawLine();
        //}

        if (battleManager.gameState == BattleManager.GameState.Play || battleManager.gameState == BattleManager.GameState.Result) {
            // ラインを引く Result中も弾いて遊べるようにする
            DrawLine();
        }
    }

    /// <summary>
    /// ボールを打ち返すラインを生成
    /// </summary>
    private void DrawLine() {
        if (Input.GetMouseButtonDown(0)) {          
            //マウスでクリックした位置を取る
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //使わない情報は0にする
            //touchPos.z = 0;
        }

        if (Input.GetMouseButton(0)) {
            //最初にクリックした位置をスタート地点にする
            Vector2 startPos = touchPos;
            //クリックしている間、その位置を監視し、離した地点を最後の地点にする
            Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //endPos.z = 0;

            if ((endPos - startPos).magnitude > mixLineLength) {
                // マウスの動いた位置のベクトルの長さ(magnitude)が設定した最小値より大きいならラインを生成
                GameObject obj = Instantiate(cuelinePrefab, transform.localPosition, transform.localRotation);  // transform.localPosition, transform.localRotation

                // 生成地点よりtransformの現在地を更新してラインの長さにする
                obj.transform.position = (startPos + endPos) / 2;

                // ベクトルの方向を維持したまま正規化
                //obj.transform.up = (endPos - startPos).normalized;

                //線の太さを設定(長く引くほど、Xの値が大きくなる。lineWidthは１が基準) 
                //obj.transform.localScale = Vector2.one * (endPos - startPos).magnitude * lineWidth;   // new Vector2((endPos - startPos).magnitude, lineWidth);
                //obj.transform.SetParent(this.transform);

                //マウスの位置を更新
                touchPos = endPos;

                // 出現時間が経過したらラインを破壊
                Destroy(obj, duration);
            }
        }
    } 
}
