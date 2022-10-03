using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class GalleryPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private GalleryIconDetail galleryIconPrefab;

    [SerializeField]
    private Transform[] galleryIconTrans;

    [SerializeField]
    private Sprite[] frameSprites;

    [SerializeField]
    private Transform zoomTran;

    private List<GalleryIconDetail> galleryIconList = new();
    private BoolReactiveProperty sharedGate = new(true);�@�@�@//�@BindToOnClick �ɂė��p����


    public void SetUp() {
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        // �M�������[�p�L�����A�C�R���̃{�^������
        CreateGalleryIcons();

        canvasGroup.alpha = 0;

        // �|�b�v�A�b�v�\��
        OpenPopup();
    }

    /// <summary>
    /// �|�b�v�A�b�v��\������
    /// </summary>
    public void OpenPopup() {
        gameObject.SetActive(true);
        AnimePopup(1.0f);
    }

    /// <summary>
    /// �|�b�v�A�b�v�����
    /// </summary>
    public void ClosePopup() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnClose.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnClose.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
            .OnComplete(() => AnimePopup(0f));

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);        
    }

    /// <summary>
    /// �|�b�v�A�b�v���A�j��������
    /// </summary>
    /// <param name="alpha"></param>
    private void AnimePopup(float alpha) {
        canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;

                if (alpha == 0) {
                    gameObject.SetActive(false);
                }
            }).SetLink(gameObject);
    }

    /// <summary>
    /// �M�������[�p�L�����A�C�R���̃{�^������
    /// </summary>
    private void CreateGalleryIcons() {
        int index = 0;
        for (int i = 0; i < UserData.instance.GetStageCount(); i++) {
            for (int j = 0; j < frameSprites.Length; j++) {
                GalleryIconDetail galleryIcon = Instantiate(galleryIconPrefab, galleryIconTrans[index], false);
                index++;

                Sprite charaSprite = j == 0 ? UserData.instance.GetStageData(i).normalCharaSprite : UserData.instance.GetStageData(i).rareCharaSprite;
                galleryIcon.SetUp(charaSprite, frameSprites[j], zoomTran.position);

                galleryIcon.GetButton().BindToOnClick(sharedGate, _ => {
                    // �Y�[�����Ȃ�
                    if (galleryIcon.IsZoomIn) {
                        // ���Ɉʒu�ɖ߂�
                        galleryIcon.ZoomOutGalleryIcon();
                    } else {
                        // �Y�[��
                        galleryIcon.ZoomInGalleryIcon();
                    }

                    // 1�b�ԉ����Ȃ��{�^��
                    return Observable.Timer(System.TimeSpan.FromSeconds(0.75f)).AsUnitObservable();
                });
                galleryIconList.Add(galleryIcon);
            }
        }
        sharedGate.AddTo(gameObject);
    }
}