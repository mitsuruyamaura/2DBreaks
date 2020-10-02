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

    public Transform startCharaTran;

    public GameObject charaObj;

    public enum GameState {
        Wait,
        Ready,
        Play,
        Result
    }

    public GameState gameState = GameState.Wait;

    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());

        yield return StartCoroutine(PreparateNextPhase());



        uiManager.UpdateDisplayGameTime(currentTime);

        yield return null;
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

        // キャラ生成


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
            enemy.GetComponent<EnemyBall>().SetUpEnemyBall(this);
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
            StartCoroutine(PreparateNextPhase());
        }
    }


    private IEnumerator PreparateNextPhase() {
        currentPhaseCount++;
        yield return StartCoroutine(ResetCharaPosition());

        yield return StartCoroutine(GenerateEnemys());

        //yield return StartCoroutine(GenerateObstructs());

        yield return StartCoroutine(uiManager.DispayPhaseCount(currentPhaseCount));

        gameState = GameState.Play;
    }

    /// <summary>
    /// キャラの位置をスタート位置へ戻す
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetCharaPosition() {
        // スタート位置へ戻す
        charaObj.transform.DOMove(startCharaTran.position, 1.0f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1.0f);
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
