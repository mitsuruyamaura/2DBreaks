using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GalleryIconDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgGalleryChara;

    [SerializeField]
    private Image imgFrame;

    [SerializeField]
    private Button btnGalleryIcon;

    private bool isZoomIn;
    public bool IsZoomIn { get => isZoomIn; }

    private Vector3 startPos;
    private Vector3 zoomInPos;
    private HoverButton hoverButton;

    
    public void SetUp(Sprite charaSprite, Sprite frameSprite, Vector3 zoomInPos) {
        startPos = transform.localPosition;

        imgGalleryChara.sprite = charaSprite;
        imgFrame.sprite = frameSprite;
        this.zoomInPos = zoomInPos;

        TryGetComponent(out hoverButton);
    }


    public Button GetButton() {
        return btnGalleryIcon;
    }


    public void ZoomInGalleryIcon() {
        isZoomIn = true;
        // �摜�̗D�揇�ʂ��őO�ʂɕύX
        transform.parent.SetAsLastSibling();

        // �{�^���̃z�o�[��؂�
        hoverButton.enabled = false;

        // Sequence �����������ė��p�ł����Ԃɂ���
        Sequence sequence = DOTween.Sequence();

        // �|�b�v�A�b�v���{�^���̈ʒu�����ʂ̒���(Canvas �Q�[���I�u�W�F�N�g�̈ʒu)�Ɉړ�������
        sequence.Append(transform.DOMove(zoomInPos, 0.5f).SetEase(Ease.Linear));

        // �|�b�v�A�b�v�����X�ɑ傫�����Ȃ���\���B�w�肵���T�C�Y�ɂȂ�����A���̃|�b�v�A�b�v�̑傫���ɖ߂�
        sequence.Join(transform.DOScale(Vector2.one * 4.2f, 0.5f).SetEase(Ease.InBack)).OnComplete(() => { transform.DOScale(Vector2.one * 3.8f, 0.2f); }).SetLink(gameObject);
    }


    public void ZoomOutGalleryIcon() {

        // Sequence �����������ė��p�ł����Ԃɂ���
        Sequence sequence = DOTween.Sequence();

        // �|�b�v�A�b�v�̑傫�������X�� 0 �ɂ��Č����Ȃ���Ԃɂ�����
        sequence.Append(transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear));

        // ����ɍ��킹�ă|�b�v�A�b�v���A���o���{�^���̈ʒu�Ɉړ�������B�ړ���Ƀ|�b�v�A�b�v��j��
        // DOLocalMove ���\�b�h�ɂ���ƃ{�^���̈ʒu�ɖ߂�Ȃ����߁ADOMove ���\�b�h���g��
        sequence.Join(transform.DOLocalMove(startPos, 0.5f)
            .SetEase(Ease.Linear))
            .SetLink(gameObject)
            .OnComplete(() => 
            {
                // �Y�[����Ԃ�߂��A�z�o�[�@�\�����Ȃ���
                isZoomIn = false;
                hoverButton.enabled = true;
            });
    }
}