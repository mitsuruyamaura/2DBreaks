using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentTime;

    public int currentHp;

    public UIManager uiManager;

    public int currentPhaseCount;

    public int itemAppearCount;
    public int nextAppearCount;

    private float timer;

    public Transform canvasTran;
    public Transform leftBottomTran;
    public Transform rightTopTran;

    public List<EnemyData> enemyList = new List<EnemyData>();

    public CharaData charaPrefab;
    public EnemyData enemyPrefab;
    public ItemDetail itemPrefab;
    public TreasureData treasurePrefab;

    public GameObject enemyObjPrefab;
    public Transform[] enemyAppearTran;
    public List<GameObject> enemyObjList = new List<GameObject>();

    public int maxPhaseCount;
    public Transform startCharaTran;

    public CharaBall charaBall;


    public enum GameState {
        Wait,
        Ready,
        Play,
        Result
    }

    public GameState gameState = GameState.Wait;

    IEnumerator Start() {
        // 初期化
        yield return StartCoroutine(Initialize());

        // ゲームの準備(Phaseごと)
        yield return StartCoroutine(PreparateNextPhase());


        // 残り時間の表示を更新
        uiManager.UpdateDisplayGameTime(currentTime);
    }

    /// <summary>
    /// ゲーム設定値の初期化
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize() {
        // 初期値設定
        currentHp = GameData.instance.charaData.hp;
        currentTime = GameData.instance.battleTime;

        nextAppearCount = Random.Range(10, 16);

        currentPhaseCount = 0;

        // TODO キャラ生成(今はPublicで入れている)

        // TODO キャラのスタート地点を登録(今はPublicで入れている)

        yield break;
    }

    /// <summary>
    /// 敵を生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateEnemys() {

        int appearEnemyCount = Random.Range(2, 5); 
        for (int i = 0; i < appearEnemyCount; i++) {
            GameObject enemy = Instantiate(enemyObjPrefab, enemyAppearTran[i], false);
            enemy.GetComponent<EnemyBall>().SetUpEnemyBall(this, canvasTran);
            enemyObjList.Add(enemy);
            yield return new WaitForSeconds(0.15f);
        }
        
        
        
    }

    /// <summary>
    /// 障害物を生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateObstructs() {
        yield break;
    }

    /// <summary>
    /// 敵をリストから削除
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemyList(GameObject enemy) {
        enemyObjList.Remove(enemy);
        CheckRemainingEnemies();
    }

    /// <summary>
    /// 敵の残数を確認
    /// </summary>
    public void CheckRemainingEnemies() {
        if (enemyObjList.Count == 0) {
            if (currentPhaseCount >= maxPhaseCount) {
                // ステージクリア
                gameState = GameState.Result;
                uiManager.DisplayStageClear();
            } else {
                // 次のPhaseの準備
                gameState = GameState.Wait;
                StartCoroutine(PreparateNextPhase());
            }
        }
    }

    /// <summary>
    /// Phaseの準備
    /// </summary>
    /// <returns></returns>
    private IEnumerator PreparateNextPhase() {
        // Phase数を加算
        currentPhaseCount++;
        uiManager.UpdateDisplayPhaseCount(currentPhaseCount, maxPhaseCount);

        yield return new WaitForSeconds(0.5f);

        // キャラがスタート地点にいなければ、キャラの位置をスタート地点へ戻す
        if (charaBall.transform.position != startCharaTran.position) {
            yield return StartCoroutine(ResetCharaPosition());
        }

        // Phaseに合わせた敵を生成
        yield return StartCoroutine(GenerateEnemys());

        // TODO Phaseに合わせた障害物を生成
        //yield return StartCoroutine(GenerateObstructs());

        // 画面にPhase数を表示
        yield return StartCoroutine(uiManager.DispayPhaseStart(currentPhaseCount));

        gameState = GameState.Play;
    }

    /// <summary>
    /// キャラの位置をスタート位置へ戻す
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetCharaPosition() {
        float waiTime = 1.0f;
        // キャラの動きを止める
        charaBall.StopMoveBall();
        // スタート位置へ戻す
        charaBall.transform.DOMove(startCharaTran.position, waiTime).SetEase(Ease.Linear);

        yield return new WaitForSeconds(waiTime);
    }


    void Update() {
        if (gameState != GameState.Play) {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= 1) {
            timer = 0;
            currentTime--;
            itemAppearCount++;

            if (itemAppearCount >= nextAppearCount) {
                GenerateItems();
                itemAppearCount = 0;
            }

            if (currentTime <= 0) {
                currentTime = 0;
                // TODO GameUp

            }
        }
        // ゲーム時間の表示を更新
        uiManager.UpdateDisplayGameTime(currentTime);
    }

    /// <summary>
    /// アイテムを生成
    /// </summary>
    private void GenerateItems() {
        // 生成位置を画面内の1点にランダムで設定
        Vector2 generatePos = new Vector2(Random.Range(leftBottomTran.position.x, rightTopTran.position.x), Random.Range(leftBottomTran.position.y, rightTopTran.position.y));

        // TODO アイテム生成 DataBaseManagerから戻り値としてItemDataを貰って、それを使ってインスタンスする
        ItemDetail item = Instantiate(itemPrefab, canvasTran, false);
        item.transform.position = generatePos;

        // 
        item.SetUpItemDetail(DataBaseManager.instance.itemDataList[Random.Range(0, DataBaseManager.instance.itemDataList.Count)], this);

        // TODO アイテム用のリストに追加

        nextAppearCount = Random.Range(10, 16);
    }
}
