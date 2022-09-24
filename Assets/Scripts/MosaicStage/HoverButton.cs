using UnityEngine;
using UnityEngine.EventSystems;  // IPointerEnterHandler に必要
using UnityEngine.UI;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler
{
    private Button btnHover;
    private bool isSelected;


    void Start() {
        TryGetComponent(out btnHover);

        // UI は反応しないので、OnPointerEnter を使う
        //this.OnMouseEnterAsObservable()
        //    .Subscribe(_ => ResponseButton())
        //    .AddTo(gameObject);
    }

    // UI ではコライダーをアタッチしても動作しない
    //private void OnMouseEnter() {
    //    Debug.Log("Enter");
    //}

    // 代わりにこちらを使う
    public void OnPointerEnter(PointerEventData eventData) {
        if (btnHover == null) {
            return;
        }
        ResponseHoverButton();
    }

    /// <summary>
    /// マウスをホバーしたときの処理
    /// </summary>
    private void ResponseHoverButton() {
        if (!btnHover.enabled || isSelected) {
            return;
        }
        isSelected = true;
        
        transform.DOShakeScale(0.25f, 0.5f, 4)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                isSelected = false;            
            });
    }
}