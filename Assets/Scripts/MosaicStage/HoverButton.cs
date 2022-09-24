using UnityEngine;
using UnityEngine.EventSystems;  // IPointerEnterHandler �ɕK�v
using UnityEngine.UI;
using DG.Tweening;

public class HoverButton : MonoBehaviour, IPointerEnterHandler
{
    private Button btnHover;
    private bool isSelected;


    void Start() {
        TryGetComponent(out btnHover);

        // UI �͔������Ȃ��̂ŁAOnPointerEnter ���g��
        //this.OnMouseEnterAsObservable()
        //    .Subscribe(_ => ResponseButton())
        //    .AddTo(gameObject);
    }

    // UI �ł̓R���C�_�[���A�^�b�`���Ă����삵�Ȃ�
    //private void OnMouseEnter() {
    //    Debug.Log("Enter");
    //}

    // ����ɂ�������g��
    public void OnPointerEnter(PointerEventData eventData) {
        if (btnHover == null) {
            return;
        }
        ResponseHoverButton();
    }

    /// <summary>
    /// �}�E�X���z�o�[�����Ƃ��̏���
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