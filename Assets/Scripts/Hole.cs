using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hole : MonoBehaviour
{
    public int power;


    protected virtual void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startSize = rectTransform.sizeDelta;

        rectTransform.sizeDelta = new Vector2(0, startSize.y);

        // Sequence初期化
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOSizeDelta(startSize, 1.0f).SetEase(Ease.OutCirc));
        sequence.Join(rectTransform.DOShakePosition(1.0f, 3, 20, 180).SetEase(Ease.Linear));
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "Liner")
        {
            return;
        }


        // CharaBallに接触したら
        if (col.gameObject.tag == "CharaBall")
        {
            // CharaBallクラスを取得できるか判定
            if (col.gameObject.TryGetComponent(out CharaBall charaBall))
            {
                // Hpを減少させる
                charaBall.UpdateHp(-power);

                // Sequence初期化
                Sequence sequence = DOTween.Sequence();

                // キャラを回転
                sequence.Append(charaBall.transform.DOLocalRotate(new Vector3(0, 0, 720), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            }
        }
    }
}
