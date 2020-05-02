using UnityEngine;

/// <summary>
/// ブロック破壊管理クラス
/// </summary>
public class Destroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ball") {
            // クリア目標数をデクリメント
            GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>().ProcNorma();            
            Destroy(gameObject);
        }
    }
}
