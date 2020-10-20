using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyBall : MonoBehaviour
{
    [Header("的球の体力")]
    public int hp;

    private int maxHp;

    public Slider hpSlider;

    private CapsuleCollider2D capsuleCol;

    private BattleManager gameManager;

    private Transform canvasTran;


    // 未
    public int money;

    public int appearance;

    public TreasureBox treasureBoxPrefab;





    void Start()
    {
        capsuleCol = GetComponent<CapsuleCollider2D>();

        // 最初のスケールを保持
        Vector2 startScale = transform.localScale;

        // 最小化
        transform.localScale = Vector2.zero;

        maxHp = hp;

        // Sequence初期化
        Sequence sequence = DOTween.Sequence();

        // 敵を回転させながら大きくする
        sequence.Append(transform.DOLocalRotate(new Vector3(0, 720, 0), 1.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        sequence.Join(transform.DOScale(startScale * 1.5f, 1.0f).SetEase(Ease.InCirc));
        sequence.AppendInterval(0.15f);
        sequence.Join(transform.DOScale(startScale, 0.15f).SetEase(Ease.InCirc));
    }


    public void SetUpEnemyBall(BattleManager gameManager, Transform canvasTran) {
        this.gameManager = gameManager;
        this.canvasTran = canvasTran;
    }

    /// <summary>
    /// 体力ゲージの表示を更新
    /// </summary>
    private void UpdateHpGauge() {
        hpSlider.DOValue((float)hp / maxHp, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D col) {

        //Debug.Log(col.gameObject.tag);

        //if (col.gameObject.tag == "Liner" || col.gameObject.tag == "CueLine") {
        //    return;
        //}

        //Debug.Log(col.gameObject.tag);

        // CharaBallに接触したら
        if (col.gameObject.tag == "CharaBall")
        {
            // CharaBallクラスを取得できるか判定
            if (col.gameObject.TryGetComponent(out CharaBall charaBall))
            {
                // 取得できている表示
                //Debug.Log(charaBall);

                // Hpを減少させる
                hp -= charaBall.power;

                // Hpゲージに反映
                hpSlider.DOValue(Mathf.Clamp((float) hp / maxHp, 0, 1), 0.5f);

                // Sequence初期化
                Sequence sequence = DOTween.Sequence();

                // 敵を回転(Hpゲージは回転させないので、逆回転させて回っていないように見せる)
                sequence.Append(transform.DOLocalRotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
                sequence.Join(hpSlider.transform.DOLocalRotate(new Vector3(0, -720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

                // Hpが0以下になったら
                if (hp <= 0)
                {
                    DestroyEnemy(sequence);
                    //capsuleCol.enabled = false;
                    //// TODO チェイン判定。お金が増える

                    //// 宝箱生成判定
                    //if (JudgeTreasureBox()) {
                    //    Vector3 scale = treasureBoxPrefab.transform.localScale;
                    //    TreasureBox treasureBox = Instantiate(treasureBoxPrefab, transform, false);
                    //    treasureBox.transform.SetParent(canvasTran);
                    //    treasureBox.transform.localScale = scale;
                    //}

                    //// お金を加算
                    //GameData.instance.ProcMoney(money);

                    //// お金の表示を更新
                    //gameManager.uiManager.UpdateDisplayMoney();

                    //// 回転させながらスケールを0にする
                    ////sequence.Join(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc));

                    //// 内側に小さくする ドロップ内容で消える処理を分岐
                    //sequence.Join(GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 100), 0.5f).SetEase(Ease.Linear));

                    //gameManager.RemoveEnemyList(gameObject);

                    //// スケールが0になるタイミングで破棄
                    //Destroy(gameObject, 0.5f);
                }
            }
        }
    }

    /// <summary>
    /// 敵の破棄
    /// </summary>
    /// <param name="sequence"></param>
    public void DestroyEnemy(Sequence sequence) {
        capsuleCol.enabled = false;
        // TODO チェイン判定。お金が増える

        // 宝箱生成判定
        if (JudgeTreasureBox()) {
            Vector3 scale = treasureBoxPrefab.transform.localScale;
            TreasureBox treasureBox = Instantiate(treasureBoxPrefab, transform, false);
            treasureBox.transform.SetParent(canvasTran);
            treasureBox.transform.localScale = scale;
        }

        // お金を加算
        GameData.instance.ProcMoney(money);

        // お金の表示を更新
        //gameManager.uiManager.UpdateDisplayMoney();

        // 回転させながらスケールを0にする
        //sequence.Join(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc));

        // 内側に小さくする ドロップ内容で消える処理を分岐
        sequence.Join(GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 100), 0.5f).SetEase(Ease.Linear));

        gameManager.RemoveEnemyList(this);

        // スケールが0になるタイミングで破棄
        Destroy(gameObject, 0.5f);
    }

    /// <summary>
    /// 宝箱生成判定
    /// </summary>
    /// <returns></returns>
    private bool JudgeTreasureBox() {
        if (Random.Range(0, 100) <= appearance) {
            return true;
        } else {
            return false;
        }    
    }
}
