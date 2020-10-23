using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public UIManager uiManager;

    [SerializeField]
    private CharaBall charaBallPrefab;

    [SerializeField]
    private Transform startCharaTran;

    private CharaBall charaBall;

    [SerializeField]
    private EnemyBall enemyBallPrefab;

    [SerializeField]
    private List<EnemyBall> enemyBallList = new List<EnemyBall>();

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Transform leftBottomTran;

    [SerializeField]
    private Transform rightTopTran;

    [SerializeField]
    private Transform enemyPlace;

    [Header("バトルの残り時間")]
    public int currentTime;

    private float timer;           // 計測用

    private int money;

    public enum GameState {
        Wait,
        Ready,
        Play,
        Result
    }

    public GameState gameState = GameState.Wait;


    [SerializeField]
    private ResultPopUp resultPopUpPrefab;

    // 未

    public int currentHp;

    public int currentPhaseCount;

    public int itemAppearCount;
    public int nextAppearCount;


    public List<EnemyData> enemyList = new List<EnemyData>();

    public CharaData charaPrefab;
    public EnemyData enemyPrefab;
    public ItemDetail itemPrefab;
    public TreasureData treasurePrefab;
    public GameObject obstacleObjPrefab;

    public GameObject enemyObjPrefab;
    public Transform[] enemyAppearTran;
    public List<GameObject> enemyObjList = new List<GameObject>();
    public List<GameObject> obstacleObjList = new List<GameObject>();

    public int maxPhaseCount;
    

    public Transform[] obstaclesTran;

    //public CharaBall charaBall;

    public GameObject obstacleRockPrefab;
    public GameObject enemyKumaPrefab;



    public IEnumerator Start() {
        // 初期化
        yield return StartCoroutine(Initialize());

        // ゲームの準備(Phaseごと)
        //yield return StartCoroutine(PreparateNextPhase());

        

        // 残り時間の表示を更新
        uiManager.UpdateDisplayBattleTime(currentTime);

        // Moneyの表示を更新
        uiManager.UpdateDisplayMoney(money);

    }

    /// <summary>
    /// ゲーム設定値の初期化
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize() {
        gameState = GameState.Wait;
        Debug.Log(gameState);

        // 初期値設定
        currentHp = GameData.instance.charaData.hp;
        currentTime = GameData.instance.battleTime;

        // Moneyの初期値設定
        money = 0;

        nextAppearCount = Random.Range(10, 16);

        currentPhaseCount = 0;

        // TODO キャラ生成(今はPublicで入れている)

        // TODO キャラのスタート地点を登録(今はPublicで入れている)

        // 手球を体力の数だけ生成する
        yield return StartCoroutine(uiManager.GenerateIconRemainingBalls(currentHp));

        // GenerateCharaBallメソッドで手球の生成処理し、戻り値で変数に代入
        charaBall = GenerateCharaBall();

        // 敵を生成
        yield return StartCoroutine(GenerateEnemys());

        gameState = GameState.Play;

        Debug.Log(gameState);
        //yield break;
    }

    /// <summary>
    /// 手球の生成
    /// </summary>
    /// <returns></returns>
    private CharaBall GenerateCharaBall() {
        CharaBall chara = Instantiate(charaBallPrefab, startCharaTran, false);
        chara.SetUpCharaBall(this);
        return chara;
    }

    /// <summary>
    /// 敵を生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateEnemys() {

        //int appearEnemyCount = Random.Range(2, 5); 
        //for (int i = 0; i < appearEnemyCount; i++) {
        //    GameObject enemy = Instantiate(enemyObjPrefab, enemyAppearTran[i], false);
        //    enemy.GetComponent<EnemyBall>().SetUpEnemyBall(this, canvasTran);
        //    enemyObjList.Add(enemy);
        //    yield return new WaitForSeconds(0.15f);
        //}
        //GameObject kuma = Instantiate(enemyKumaPrefab, enemyAppearTran[4], false);
        //kuma.GetComponent<EnemyBall>().SetUpEnemyBall(this, canvasTran);
        //enemyObjList.Add(kuma);

        // 生成する目標数をランダムで設定
        int appearEnemyCount = Random.Range(2, 5);

        // 生成した数をカウント用
        int count = 0;

        // 生成した数が目標数になるまでループする。目標数になったら生成終了し、ループを抜ける
        while (count < appearEnemyCount) {

            // Transform情報を代入
            Transform enemyTran = canvasTran;

            // 位置を画面内に収まる範囲でランダムに設定
            enemyTran.position = new Vector2(Random.Range(leftBottomTran.localPosition.x, rightTopTran.localPosition.x), Random.Range(leftBottomTran.localPosition.y, rightTopTran.localPosition.y));

            // 設定した位置に対してRayを発射
            RaycastHit2D hit = Physics2D.Raycast(enemyTran.position, Vector3.forward);

            // Rayに当たったオブジェクトを表示。何もないときは null
            Debug.Log(hit.collider);
            
            // もしも何もRayに当たらない場合(Rayに何か当たった場合にはその位置には生成しないので、ループの最初からやり直す)
            if (hit.collider == null) {

                // 敵を生成
                EnemyBall enemyBall = Instantiate(enemyBallPrefab, canvasTran, false);

                // 親子関係を設定
                enemyBall.transform.SetParent(enemyPlace);

                // 敵の位置をランダムで設定した位置に設定
                enemyBall.transform.localPosition = enemyTran.position;

                // 敵の初期設定
                enemyBall.SetUpEnemyBall(this, canvasTran);

                // 敵の生成カウントを加算
                count++;

                // 敵の管理リストに追加
                enemyBallList.Add(enemyBall);

                // 少し待機して、ループを最初から繰り返す
                yield return new WaitForSeconds(0.15f);
            }         
        }
    }

    /// <summary>
    /// 障害物を生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateObstructs() {

        for (int i = 0; i < 1; i++) {
            GameObject obstacle = Instantiate(obstacleObjPrefab, obstaclesTran[i], false);
            obstacle.GetComponent<ObstacleBase>().SetUpObstacle(this);
            obstacleObjList.Add(obstacle);
        }
        GameObject rock = Instantiate(obstacleRockPrefab, obstaclesTran[1], false);
        rock.GetComponent<ObstacleBase>().SetUpObstacle(this);
        obstacleObjList.Add(rock);

        yield break;
    }

    /// <summary>
    /// 敵をリストから削除
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemyList(EnemyBall enemy) {
        enemyBallList.Remove(enemy);
        CheckRemainingEnemies();
    }


    /// <summary>
    /// 敵の残数を確認
    /// </summary>
    public void CheckRemainingEnemies() {
        if (enemyBallList.Count == 0) {
            // ObstacleListをクリア
            ClearObstacleList();

            // 今回のゲーム内で獲得したMoneyをMoney総数に加算
            //GameData.instance.ProcMoney(money);

            gameState = GameState.Result;
            Debug.Log(gameState);

            // スタート地点へ戻す
            RestartCharaPosition(1.0f);

            // 障害物を消す
            ClearObstacleList();

            // リザルト表示生成
            StartCoroutine(GenerateResultPopUp());


            //if (currentPhaseCount >= maxPhaseCount) {
            //    // ステージクリア
            //    gameState = GameState.Result;
            //    uiManager.DisplayStageClear();

            //    // 今回のゲーム内で獲得したMoneyをMoney総数に加算
            //    GameData.instance.ProcMoney(money);
            //} else {
            //    // 次のPhaseの準備
            //    gameState = GameState.Wait;
            //    StartCoroutine(PreparateNextPhase());
            //}
        }
    }

    /// <summary>
    /// ObstacleListをクリア
    /// </summary>
    private void ClearObstacleList() {
        if (obstacleObjList.Count > 0) {
            foreach (GameObject obj in obstacleObjList) {
                Destroy(obj);
            }
            obstacleObjList.Clear();
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
            yield return StartCoroutine(RestartCharaPosition());
        }

        // Phaseに合わせた敵を生成
        yield return StartCoroutine(GenerateEnemys());

        // Phaseに合わせた障害物を生成
        yield return StartCoroutine(GenerateObstructs());

        // 画面にPhase数を表示
        yield return StartCoroutine(uiManager.DispayPhaseStart(currentPhaseCount));

        gameState = GameState.Play;
    }

    /// <summary>
    /// キャラを停止させてスタート位置へ戻す
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestartCharaPosition(float waiTime = 1.0f) {
        // キャラの動きを止める
        charaBall.StopMoveBall();

        // スタート位置へ戻す
        charaBall.transform.DOMove(startCharaTran.position, waiTime).SetEase(Ease.Linear);

        yield return new WaitForSeconds(waiTime);

        // 手球を弾けるようにする
        charaBall.ChangeActivateCollider(true);
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
        uiManager.UpdateDisplayBattleTime(currentTime);
    }

    /// <summary>
    /// Moneyを加算
    /// </summary>
    public void AddMoney(int amount) {
        // moneyを加算
        money += amount;
        Debug.Log(money);

        // Moneyの表示を更新
        uiManager.UpdateDisplayMoney(money);
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

    private IEnumerator GenerateResultPopUp() {
        yield return new WaitForSeconds(1.0f);

        // リザルト表示ポップアップを生成
        ResultPopUp resultPopUp = Instantiate(resultPopUpPrefab, canvasTran, false);

        // ポップアップを設定
        resultPopUp.SetUpResultPopUp(this, money, currentTime, charaBall.GetCharaBallHp());
    }
}
