﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyBall : MonoBehaviour
{
    public int hp;

    public int money;

    public int appearance;

    public TreasureBox treasureBoxPrefab;

    public Transform canvasTran;

    public Slider slider;

    private CapsuleCollider2D capsuleCol;

    private int maxHp;

    void Start()
    {
        capsuleCol = GetComponent<CapsuleCollider2D>();

        // 最初のスケールを保持
        Vector3 startScale = transform.localScale;

        // 最小化
        transform.localScale = Vector3.zero;

        maxHp = hp;

        // Sequence初期化
        Sequence sequence = DOTween.Sequence();

        // 敵を回転させながら大きくする
        sequence.Append(transform.DOLocalRotate(new Vector3(0, 720, 0), 1.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        sequence.Join(transform.DOScale(startScale * 1.5f, 1.0f).SetEase(Ease.InCirc));
        sequence.AppendInterval(0.15f);
        sequence.Join(transform.DOScale(startScale, 0.15f).SetEase(Ease.InCirc));
    }

    private void OnCollisionEnter2D(Collision2D col) {

        if (col.gameObject.tag == "Liner") {
            return;
        }


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
                slider.DOValue(Mathf.Clamp((float) hp / maxHp, 0, 1), 0.5f);

                // Sequence初期化
                Sequence sequence = DOTween.Sequence();

                // 敵を回転(Hpゲージは回転させないので、逆回転させて回っていないように見せる)
                sequence.Append(transform.DOLocalRotate(new Vector3(0, 720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
                sequence.Join(slider.transform.DOLocalRotate(new Vector3(0, -720, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

                // Hpが0以下になったら
                if (hp <= 0)
                {
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

                    // 回転させながらスケールを0にする
                    //sequence.Join(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc));

                    // 内側に小さくする ドロップ内容で消える処理を分岐
                    sequence.Join(GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 100), 0.5f).SetEase(Ease.Linear));

                    // スケールが0になるタイミングで破棄
                    Destroy(gameObject, 0.5f);
                }
            }
        }
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
