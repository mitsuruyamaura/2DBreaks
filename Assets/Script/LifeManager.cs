using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public int initLife; //HP最大値
    private int currentLife; //現在のHP
    private int missCount; //失敗(壁にぶつかった)した回数

    public GameObject lifePrefab; //Prefabにした絵
    public GameObject maskPrefab; //並べたときにイメージの場所がずれないようにするための空のオブジェクト

    private GameObject[] lifeObjects; //LifePrefabを入れておく配列
    private GameObject[] maskObjects; //MaskPrefabを入れておく配列

    public int threshold; //スレッショルド。閾ーしきいー値。

    public GameMaster gameMaster;



    // Start is called before the first frame update
    void Start()
    {
        //画面にライフ用のイメージをライフの初期値分だけ表示するメソッド
        StartCoroutine(SetUpLifeObjects());
    }


    /// <summary>
    /// ライフ用のイメージをプレファブからライフの初期値分だけ生成する
    /// </summary>


    private IEnumerator SetUpLifeObjects()
    {
        //配列の初期化。ライフの初期値分だけ配列の要素数を用意する
        lifeObjects = new GameObject[initLife];
        maskObjects = new GameObject[initLife];

        //初期値分のライフイメージを生成
        for (int i = 0; i < initLife; i++)
        {

            //ライフのイメージをPrefabから生成
            lifeObjects[i] = Instantiate(lifePrefab, transform, true);

            //生成されたイメージをこのクラスがコンポーネント化されているオブジェクトの子にする
            lifeObjects[i].transform.SetParent(gameObject.transform);

            //TODOアニメさせる

            //徐々にライフを生成するために少し待機時間を作る
            //StartCoroutine(SetUpLifeObjects());とセット
            yield return new WaitForSeconds(0.3f);

            //ライフが減少した時のマスクイメージをプレファブから生成
            maskObjects[i] = Instantiate(maskPrefab, transform, true);
            //マスクを生成するけどボールの絵が隠れないように(非表示に)する
            maskObjects[i].SetActive(false);

            //こちらも子にする
            maskObjects[i].transform.SetParent(gameObject.transform);
        }

        //ライフの現在値を最大値に設定する
        currentLife = initLife;

        gameMaster.gameState = GAME_STATE.READY;

    }


    //壁のオブジェクトが呼ぶメソッド
    /// <summary>
    /// ライフの減算処理
    /// </summary>
    /// <param name="attackPower"></param>
    public  void ApplyDamage(int attackPower)
    {
        //ライフの現在地が0以上残っているなら
        if (currentLife > 0)
        {
            //ダメージが現在地よりも大きくなる（マイナスになる）なら
            if (attackPower > currentLife)
            {
                currentLife = 0;
            }
            else
            {
                //ライフの現在地を減算する
                currentLife -= attackPower;
                //ボールの残り数の生成に使う
                missCount += attackPower;
            }

            if (currentLife <= threshold)
            {
                //ライフの現在値が閾値よりも低下したら警告状態にする
                gameMaster.gameState = GAME_STATE.WARNING;
            }

            //ライフが残り0以下なら
            if (currentLife <= 0)
            {
                //GameMasterのステートをゲームオーバーに変更するメソッドを呼び出す
                gameMaster.GameUp();
            }
        }

        //LifeImageをcurrentLifeの値に更新する
        UpDateDisplayLife();

    }

    public void UpDateDisplayLife()
    {
        //ライフイメージの更新
        //残っているcurrentLife分だけライフのイメージを表示させる
        //減少した分はマスクイメージを表示させる＝ライフの位置が変わらないようにする
        for (int i = 0; i < initLife; i++)
        {
            if (missCount <= i)
            {
                lifeObjects[i].SetActive(true);
                maskObjects[i].SetActive(false);
            }
            else
            {
                lifeObjects[i].SetActive(false);
                maskObjects[i].SetActive(true);
            }
        }
    }
    
}
