using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class GameInitPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnCancel;

    [SerializeField]
    private Button btnInit;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Text txtVolumeValue;


    /// <summary>
    /// �����ݒ�
    /// </summary>
    public void SetUp() {
        btnCancel.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => 
            {
                // ���݂� Slider �� Value �l��ێ�
                SoundManager.instance.SetMasterVolume(volumeSlider.value);
                ClosePopup();
            })
            .AddTo(gameObject);

        btnInit.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => InitGame())
            .AddTo(gameObject);

        volumeSlider.OnValueChangedAsObservable()
            .Subscribe(x =>
            {
                SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, x);
                UpdateVolumeValue(x); 
            })
            .AddTo(gameObject);

        // �X���C�_�[�� Value �Ɍ��݂̃{�����[���ݒ�
        volumeSlider.value = SoundManager.instance.masterVolume;

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
        sequence.Append(btnCancel.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnCancel.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
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
    /// �Q�[���̏�����
    /// </summary>
    private void InitGame() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnInit.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnInit.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // �w�肵���L�[�̃Z�[�u�f�[�^�폜
        PlayerPrefsHelper.RemoveObjectData(UserData.instance.GetSaveDataKey());

        // ��񃊃Z�b�g
        UserData.instance.PrepareReset();

        // �V�[���ēǂݍ���
        StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }

    /// <summary>
    /// �{�����[�� �� �\���̍X�V
    /// </summary>
    /// <param name="value"></param>
    private void UpdateVolumeValue(float value) {
        txtVolumeValue.text = (value * 100).ToString("F0");
    }
}