using UnityEngine;

/// <summary>
/// ボールを打ち返すラインの制御クラス
/// </summary>
public class Liner : MonoBehaviour
{
    [Header("描画するラインの親オブジェクト")]
    public GameObject lineParent;
    [Header("描画するラインのプレファブ")]
    public GameObject linePrefab;
    [Header("引くラインの最小の長さ")]
    public float lineLength;

    [Header("引くラインの太さ")]
    public float lineWidth;
    [Header("ラインの出現時間")]
    public float duration;
    //[Header("ゲーム管理クラス")]
    //public GameMaster gameMaster;

    private Vector3 touchPos; //マウスのクリック地点
    
    private void Update() {
        //if ((gameMaster.gameState == GAME_STATE.PLAY) || (gameMaster.gameState == GAME_STATE.WARNING)) {
            // ラインを引く
            DrawLine();
        //}
    }

    /// <summary>
    /// ボールを打ち返すラインを生成
    /// </summary>
    private void DrawLine() {
        if (Input.GetMouseButtonDown(0)) {          
            //マウスでクリックした位置を取る
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //使わない情報は0にする
            touchPos.z = 0;
        }

        if (Input.GetMouseButton(0)) {
            //最初にクリックした位置をスタート地点にする
            Vector3 startPos = touchPos;
            //クリックしている間、その位置を監視し、離した地点を最後の地点にする
            Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPos.z = 0;

            if ((endPos - startPos).magnitude > lineLength) {
                // マウスの動いた位置のベクトルの長さ(magnitude)が設定した最小値より大きいならラインを生成
                GameObject obj = Instantiate(linePrefab, transform.position, transform.rotation) as GameObject;

                // 生成地点よりtransformの現在地を更新してラインの長さにする
                obj.transform.position = (startPos + endPos) / 2;

                // ベクトルの方向を維持したまま正規化
                obj.transform.up = (endPos - startPos).normalized;

                //線の太さを設定
                obj.transform.localScale = new Vector3((endPos - startPos).magnitude, lineWidth, lineWidth);
                obj.transform.SetParent(lineParent.transform);

                //マウスの位置を更新
                touchPos = endPos;

                // 出現時間が経過したらラインを破壊
                Destroy(obj, duration);
            }
        }
    } 
}
