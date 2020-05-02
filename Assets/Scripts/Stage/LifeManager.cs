using System.Collections;
using UnityEngine;

/// <summary>
/// ライフ管理クラス
/// </summary>
public class LifeManager : MonoBehaviour
{
    [Header("ライフの初期値")]
    public int initLife;
    [Header("警告用の閾値")]
    public int threshold;

    [Header("ライフ用イメージ")]
    public GameObject lifePrefab;
    [Header("マスク用イメージ")]
    public GameObject maskPrefab;

    [Header("ゲーム管理クラス")]
    public GameMaster gameMaster;

    private int currentLife; //現在のHP
    private int missCount;   //失敗(壁にぶつかった)した回数

    private GameObject[] lifeObjects; // ライフ用イメージ格納用
    private GameObject[] maskObjects; // マスク用イメージ格納用

    /// <summary>
    /// currentLife更新用プロパティ
    /// </summary>
    public int LifePoint {
        set {
            currentLife = value;

            // ライフイメージの更新。残っているcurrentLife分だけライフのイメージを表示
            // 減少した分はマスクイメージを表示させ、ライフの全体幅が変わらないようにする
            for (int i = 0; i < initLife; i++) {
                if (missCount <= i) {
                    lifeObjects[i].SetActive(true);
                    maskObjects[i].SetActive(false);
                } else {
                    lifeObjects[i].SetActive(false);
                    maskObjects[i].SetActive(true);
                }
            }
        }

        get {
            return currentLife;
        }
    }

    void Start() {
        StartCoroutine(SetUpLifeObjects());
    }

    /// <summary>
    /// ライフ用イメージとマスク用イメージをライフの初期値分だけ生成する
    /// </summary>
    private IEnumerator SetUpLifeObjects() {
        // 配列を初期化し、ライフの初期値分だけ配列の要素数を用意する
        lifeObjects = new GameObject[initLife];
        maskObjects = new GameObject[initLife];

        // 初期値分のライフイメージを生成
        for (int i = 0; i < initLife; i++) {
            // ライフのイメージをPrefabから生成し、子にする
            lifeObjects[i] = Instantiate(lifePrefab, transform, true);
            lifeObjects[i].transform.SetParent(gameObject.transform);

            // 徐々にライフを生成する
            yield return new WaitForSeconds(0.3f);

            // ライフが減少した時のマスクイメージをプレファブから生成
            maskObjects[i] = Instantiate(maskPrefab, transform, true);
            // マスクを非表示にして子にする
            maskObjects[i].SetActive(false);
            maskObjects[i].transform.SetParent(gameObject.transform);
        }
        // ライフの現在値を最大値に設定する
        LifePoint = initLife;

        // ゲームの進行状況を変更し、ボールが発射できる状態に切り替える
        gameMaster.gameState = GAME_STATE.READY;
    }

    /// <summary>
    /// ライフの減算処理
    /// </summary>
    /// <param name="attackPower"></param>
    public  void ApplyDamage(int attackPower) {
        // ライフが1以上残っているなら
        if (currentLife > 0) {
            // ダメージが現在値よりも大きくなる（マイナスになる）なら
            if (attackPower > currentLife) {
                currentLife = 0;
            } else {
                // ライフを減算
                currentLife -= attackPower;
                missCount += attackPower;
            }

            if (currentLife <= threshold) {
                // ライフの現在値が閾値よりも低下したら警告状態にする
                gameMaster.gameState = GAME_STATE.WARNING;
            }

            if (currentLife <= 0) {
                // ライフが残り0以下ならゲームオーバー処理
                gameMaster.GameUp(GAME_STATE.GAME_OVER);
            }
        }
        // LifeImageをcurrentLifeの値に更新して再表示
        LifePoint = currentLife;
    }
}
