using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TreasureBox : MonoBehaviour
{
    public Image imgTreasureBox;
    public Sprite soriteOpenBox;

    public BoxCollider2D boxCol;
    public GameObject treasurePrefab;

    private IEnumerator coroutine;

    IEnumerator Start()
    {
        boxCol.enabled = false;
        transform.DOJump(new Vector3(0, 0.15f, 0), 0.5f, 2, 0.5f).SetRelative();

        yield return new WaitForSeconds(1.0f);
        boxCol.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Liner") {
            return;
        }


        // CharaBallに接触したら
        if (col.gameObject.tag == "CharaBall" && coroutine == null) {
            coroutine = Open();



            // スケールが0になるタイミングで破棄

            StartCoroutine(coroutine);    
            


            //sequence.AppendInterval(0.25f);
            

            // 回転させながらスケールを0にする
            //sequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc));

            
        }
    }

    /// <summary>
    /// 宝箱回収のアニメ再生
    /// </summary>
    /// <returns></returns>
    private IEnumerator Open() {
        // Sequence初期化
        Sequence sequence = DOTween.Sequence();

        // 宝箱を開ける　ラムダ式の中ではyield処理不可。AppendIntervalも不可
        sequence.Append(transform.DOShakePosition(0.25f, 80, 2).SetEase(Ease.Linear));
        yield return new WaitForSeconds(0.5f);

        // 宝箱を開ける
        imgTreasureBox.sprite = soriteOpenBox;
        // トレジャー生成
        StartCoroutine(GenerateTreasure());
    }

    /// <summary>
    /// トレジャー生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateTreasure() {
        // Sequence初期化
        Sequence sequence = DOTween.Sequence();

        GameObject treasure = Instantiate(treasurePrefab, transform, false);
        sequence.Append(treasure.transform.DOMoveY(2.0f, 0.5f).SetEase(Ease.Linear));
        sequence.Join(treasure.transform.DOLocalRotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        yield return new WaitForSeconds(0.5f);

        // コルーチン内ではOnCompleteは動かない
        sequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc));

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
