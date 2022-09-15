using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class AchievementPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private Button btnAchieve;

    [SerializeField]
    private Button btnReset;


    // �e�A�`�[�u�����g�f�[�^�̕\���p
    [SerializeField]
    private Text[] txtChallengeCounts;

    [SerializeField]
    private Text[] txtClearCounts;

    [SerializeField]
    private Text[] txtFailureCounts;

    [SerializeField]
    private Text[] txtMaxFeverCounts;

    [SerializeField]
    private Text[] txtNoMissClearCounts;

    [SerializeField]
    private Text[] txtMaxMosaicCounts;

    [SerializeField]
    private Text[] txtMaxLinkCounts;

    [SerializeField]
    private Text[] txtFastestClearTimes;

    // GameInitPopUp
    [SerializeField]
    private GameInitPopUp gameInitPopUpPrefab;

    private GameInitPopUp gameInitPopUp;


    /// <summary>
    /// �����ݒ�
    /// </summary>
    public void Setup() {
        // �{�^���̍w��
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        btnAchieve.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        btnReset.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => PrepareGameInitPopUp())
            .AddTo(gameObject);

        // �X�e�[�W���̃A�`�[�u�����g�f�[�^��\��
        SetAchievementStageDatas();

        canvasGroup.alpha = 0;

        // �|�b�v�A�b�v�\��
        OpenPopup();
    }

    /// <summary>
    /// �X�e�[�W���̃A�`�[�u�����g�f�[�^��\��
    /// </summary>
    private void SetAchievementStageDatas() {
        for (int i = 0; i < UserData.instance.achievementStageDataList.Count; i++) {
            txtChallengeCounts[i].text = UserData.instance.achievementStageDataList[i].challengeCount.ToString();
            txtClearCounts[i].text = UserData.instance.achievementStageDataList[i].clearCount.ToString();
            txtFailureCounts[i].text = UserData.instance.achievementStageDataList[i].failureCount.ToString();
            txtMaxFeverCounts[i].text = UserData.instance.achievementStageDataList[i].maxFeverCount.ToString();
            txtNoMissClearCounts[i].text = UserData.instance.achievementStageDataList[i].noMissClearCount.ToString();
            txtMaxMosaicCounts[i].text = UserData.instance.achievementStageDataList[i].maxMosaicCount.ToString();
            txtMaxLinkCounts[i].text = UserData.instance.achievementStageDataList[i].maxLinkCount.ToString();
            txtFastestClearTimes[i].text = UserData.instance.achievementStageDataList[i].fastestClearTime.ToString("F2");
        }
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
        AnimePopup(0f);
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
    /// GameInitPopUp �̐����ƃI�[�v��
    /// </summary>
    private void PrepareGameInitPopUp() {
        // �|�b�v�A�b�v����������Ă��Ȃ����
        if (!gameInitPopUp) {
            // �������ď����ݒ肵�Ă���J��
            gameInitPopUp = Instantiate(gameInitPopUpPrefab);
            gameInitPopUp.SetUp();
        } else {
            // �|�b�v�A�b�v���J��
            gameInitPopUp.OpenPopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }
}