using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainGameInfoView : MonoBehaviour
{
    [SerializeField]
    private Text[] txtGameTimes;

    [SerializeField]
    private Text txtMosaicCount;

    [SerializeField]
    private Slider sliderFever;

    [SerializeField]
    private Text txtValue;

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private Image imgCharaIcon;

    [SerializeField]
    private Image imgExcellentLogo;

    [SerializeField]
    private Image imgGameInfo;

    [SerializeField]
    private Sprite[] gameInfoLogos;

    [SerializeField]
    private SpriteRenderer normalChara;

    [SerializeField]
    private SpriteRenderer rareChara;


    /// <summary>
    /// �X���C�_�[�̏����ݒ�
    /// </summary>
    /// <param name="targetFeverPoint"></param>
    public void SetUpSliderValue(int targetFeverPoint) {
        // �t�B�[�o�[�Q�[�W�̐ݒ�
        sliderFever.maxValue = targetFeverPoint;
        sliderFever.value = 0;
    }

    /// <summary>
    /// �Q�[�����Ԃ̕\���X�V
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateGameTime(float amount) {
        // �����_�ȉ��͏������\��
        string time = amount.ToString("F2");
        string[] part = time.Split('.');
        txtGameTimes[0].text = part[0] + ".";
        txtGameTimes[1].text = part[1];
    }

    /// <summary>
    /// �󂵂��O���b�h�̐��̕\���X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateMosaicCount(int oldValue, int newValue) {
        txtMosaicCount.DOCounter(oldValue, newValue, 0.5f).SetEase(Ease.Linear);
        // ����̃L�����A�C�R�����A�j��
        imgCharaIcon.transform.DOPunchScale(Vector3.one * 1.25f, 0.25f)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }

    /// <summary>
    /// �t�B�[�o�[�Q�[�W�̕\���X�V
    /// </summary>
    /// <param name="feverPoint"></param>
    /// <param name="duration"></param>
    public void UpdateFeverSlider(int feverPoint, float duration = 0.25f) {
        sliderFever.DOValue(feverPoint, duration).SetEase(Ease.InQuart).SetLink(gameObject);
    }

    /// <summary>
    /// Slider �̏�� % �\���̍X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateDisplayValue(float oldValue, float newValue, int targetFeverPoint, int feverDuraiton) {
        if (newValue >= targetFeverPoint) {
            newValue = targetFeverPoint;
        }
        float before = oldValue / targetFeverPoint * 100;
        float after = newValue / targetFeverPoint * 100;

        //Debug.Log(before);
        //Debug.Log(after);

        int a = (int)before;
        int b = (int)after;

        //Debug.Log(a);
        //Debug.Log(b);
        // �����̃A�j�����Ԃ̐ݒ�B�t�B�[�o�[�����ۂɂ͒����Ȃ�
        float duration = newValue == 0 ? (float)feverDuraiton / 1000 : 0.25f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(txtValue.DOCounter(a, b, duration).SetEase(Ease.Linear)).SetLink(txtValue.gameObject);

        // ���^���ɂȂ�����
        if (newValue == targetFeverPoint) {
            // 100�� �̐�����������A�j�����o��ǉ�
            float scale = txtValue.transform.localScale.x;
            sequence.Append(
            txtValue.transform.DOPunchScale(txtValue.transform.localScale * 1.25f, 0.25f).SetEase(Ease.InQuart).SetLink(txtValue.gameObject)
                .OnComplete(() => txtValue.transform.localScale = Vector3.one * scale));
        }
    }

    /// <summary>
    /// �Q�[���A�b�v���̃C���t�H���\��
    /// </summary>
    public void HideGameUpInfo() => txtInfo.gameObject.SetActive(false);

    /// <summary>
    /// �Q�[���A�b�v���̃C���t�H�\��
    /// </summary>
    public void ShowGameUpInfo() {
        // �N���b�N����
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// �G�N�Z�����g�̕\��
    /// </summary>
    public void ShowExcellentLogo() {
        imgExcellentLogo.DOFade(1.0f, 2.0f).SetEase(Ease.Linear);
        imgExcellentLogo.transform.DOLocalMoveX(0, 1.5f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);
    }

    /// <summary>
    /// �G�N�Z�����g�̔�\��
    /// </summary>
    public void HideExcellentLogo() {
        imgExcellentLogo.DOFade(0f, 0.5f).SetEase(Ease.Linear);
        imgExcellentLogo.transform.DOLocalMoveX(-1250, 1.0f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);

        ShowExcellentBonusChara();
    }

    /// <summary>
    /// �G�N�Z�����g�{�[�i�X�p�̉摜�\��
    /// </summary>
    private void ShowExcellentBonusChara() {
        normalChara.material.DOFloat(-1, "_Flip", 1.5f).SetEase(Ease.Linear).SetLink(normalChara.gameObject);
    }

    /// <summary>
    /// �Q�[���N���A
    /// </summary>
    public void ShowGameClear() {
        imgGameInfo.sprite = gameInfoLogos[1];

        Sequence sequence = DOTween.Sequence();
        sequence.Append(imgGameInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
        sequence.AppendInterval(1.0f);
        sequence.Append(imgGameInfo.DOFade(0, 0.5f).SetEase(Ease.Linear));

        ShowGameUpInfo();
    }

    /// <summary>
    /// �Q�[���I�[�o�[
    /// </summary>
    public void ShowGameOver() {
        imgGameInfo.sprite = gameInfoLogos[0];
        imgGameInfo.DOFade(1.0f, 1.5f).SetEase(Ease.InQuart).SetLink(imgGameInfo.gameObject);

        ShowGameUpInfo();
    }

    /// <summary>
    /// �X�e�[�W���Ƃ̃��C���L����(�w�i)�摜�̐ݒ�
    /// </summary>
    public void SetCharaSprite(yamap.StageData currentStageData) {
        normalChara.sprite = currentStageData.normalCharaSprite;
        rareChara.sprite = currentStageData.rareCharaSprite;

        SetUpCharaIcon(currentStageData.charaIcon);
    }

    /// <summary>
    /// ��ʍ���̃L�����A�C�R�����X�e�[�W�ɍ��킹�Đݒ�
    /// </summary>
    /// <param name="charaIcon"></param>
    public void SetUpCharaIcon(Sprite charaIcon) {
        imgCharaIcon.sprite = charaIcon;

        imgCharaIcon.transform.DOShakeScale(0.75f, 1f, 4)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }

    /// <summary>
    /// �t�B�[�o�[�^�C���J�n
    /// </summary>
    /// <param name="targetFeverPoint"></param>
    /// <param name="feverDuraiton"></param>
    public void SetFeverTime(int targetFeverPoint, int feverDuraiton) {
        sliderFever.value = targetFeverPoint;
        sliderFever.DOValue(0, (float)feverDuraiton / 1000).SetEase(Ease.Linear).SetLink(gameObject);
    }
}