using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlushScreen : MonoBehaviour
{
    [SerializeField]
    private Image imgFlushEffectPanel;

    [SerializeField]
    private float duration;

    [SerializeField]
    private int flushCount;


    public void Flush() {

        imgFlushEffectPanel.DOColor(new(0.5f, 0, 0, 0.5f), duration)
            .SetEase(Ease.Linear)
            .SetLoops(flushCount, LoopType.Yoyo)
            .OnComplete(() => imgFlushEffectPanel.DOColor(new(0, 0, 0, 0), 0.2f).SetEase(Ease.Linear).SetLink(gameObject))
            .SetLink(gameObject);
    }
}