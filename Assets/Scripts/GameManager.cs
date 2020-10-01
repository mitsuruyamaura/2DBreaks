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
    public int nextApperCount;

    private float timer;

    public Transform canvasTran;
    public Transform leftBottomTran;
    public Transform rightTopTran;

    public List<EnemyData> enemyList = new List<EnemyData>();

    public CharaData charaPrefab;
    public EnemyData enemyPrefab;
    public ItemDetail itemPrefab;
    public TreasureData treasurePrefab;


    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());

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

        nextApperCount = Random.Range(10, 16);

        yield break;
    }




    void Update() {
        timer += Time.deltaTime;
        if (timer >= 1) {
            timer = 0;
            currentTime--;
            itemAppearCount++;

            if (itemAppearCount >= nextApperCount) {
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
    /// アイテム生成
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

        nextApperCount = Random.Range(10, 16);
    }
}
