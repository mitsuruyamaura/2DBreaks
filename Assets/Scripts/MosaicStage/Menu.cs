using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Text txtMozaicCount;

    [SerializeField]
    private Text txtGallery;

    [SerializeField]
    private Button btnGallery;

    [SerializeField]
    private CharaButtonDetail charaButtonPrefab;

    [SerializeField]
    private Transform[] charaButtonSetTrans;

    private List<CharaButtonDetail> charaButtonList = new();
    private BoolReactiveProperty sharedGate = new(true);�@�@�@//�@BindToOnClick �ɂė��p����

    [SerializeField]
    private AchievementPopUp achievementPopUpPrefab;

    private AchievementPopUp achievementPopUp;

    [SerializeField]
    private Button btnAchieve;

    [SerializeField]
    private GalleryPopUp galleryPopUpPrefab;

    private GalleryPopUp galleryPopUp;

    private string masterAudioName = "Master";   // AudioMixer �� AudioGroup �̖��O�Ŏw��ł���BAudioMixer ���̃X�N���v�g�ł̐��䋖�̐ݒ肪�K�v


    void Start()
    {
        // �}�X�^�[���ʂ̏����l�ݒ�
        SoundManager.instance.SetLinearVolumeToMixerGroup(masterAudioName, EntryPoint.instance.masterVolume);
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.Menu);

        // ���U�C�N�J�E���g�ɂ��X�e�[�W�J���̔���
        UserData.instance.CheckOpenStageFromPoint();

        // �L�����{�^���̐���
        CreateCharaButtons();

        // MozaicCount �w��
        UserData.instance.MosaicCount
            .Zip(UserData.instance.MosaicCount.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateDisplayMosaicCount(UserData.instance.beforePoint, x.newValue))   // �O�̒l�Ƃ̍����ŕ\���X�V����
            .AddTo(gameObject);

        // �����l�\���X�V
        UserData.instance.MosaicCount.SetValueAndForceNotify(UserData.instance.MosaicCount.Value);

        // �M�������[���[�h�J���̊m�F�B�Z���]�����āA�ǂ��炩�� true �ł���Ε]�������̂ŁA1�̖ڂ̕]���� true �Ȃ�2�ڂɂ͂����Ȃ��ŏI���
        btnGallery.enabled = UserData.instance.CheckOpenGallaryPoint() || UserData.instance.CheckOpenGalleryAllNoMissClears();

        // �M�������[���[�h���J���̏ꍇ
        if (!btnGallery.enabled) {
            txtGallery.text +=  "�y�S�X�e�[�W�m�[�~�X�N���A��" + "\r\n" + UserData.instance.openGallaryPoint + " �ŊJ���z";   
        }

        // �A�`�[�u�����g�{�^���̍w��
        btnAchieve.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => PrepareAchievementPopUp())
            .AddTo(gameObject);

        // �M�������[�{�^���̍w��
        btnGallery.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => PrepareGalleryPopUp())
            .AddTo(gameObject);
    }

    /// <summary>
    /// MosaicCount �̕\���X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayMosaicCount(int oldValue, int newValue) {
        txtMozaicCount.DOCounter(oldValue, newValue, 2.0f).SetEase(Ease.InQuart).SetLink(gameObject);
    }

    /// <summary>
    /// �L�����{�^���̐���
    /// </summary>
    private void CreateCharaButtons() {
        for (int i = 0; i < UserData.instance.GetStageCount(); i++) {
            yamap.StageData stageData = UserData.instance.GetStageData(i);

            CharaButtonDetail charaButton = Instantiate(charaButtonPrefab, charaButtonSetTrans[i], false);
            charaButton.SetUpCharaButtonDetail(stageData.stageNo, stageData.charaIcon);

            // ���b�N����Ă���X�e�[�W�̏ꍇ
            if (!UserData.instance.clearStageNoList.Contains(stageData.stageNo)) {
                // �L�������V���G�b�g�\�����ă{�^���������Ȃ���Ԃɂ���
                charaButton.LockCharaButton();

                // �X�e�[�W�J���ɕK�v�ȃ|�C���g�\��
                charaButton.DisplayStageOpenPoint(stageData.stageOpenPoint);
            }

            // �����̃{�^���� BoolReactiveProperty ���w�ǂ��āA�P�̃{�^���ɘA�����Đ���ł���
            // ������ AsyncReactiveCommand ���������������BsharedGate �� true �Ȃ̂ŁA���ꂪ false �ɂȂ�ƁA���ׂẴ{�^���� interactable �� false �̏������͂�
            charaButton.GetButton().BindToOnClick(sharedGate, _ =>
                {
                    charaButton.OnClickCharaButton();

                    UserData.instance.beforePoint = UserData.instance.MosaicCount.Value;

                    // 5�b�ԉ����Ȃ��{�^��
                    return Observable.Timer(System.TimeSpan.FromSeconds(5)).AsUnitObservable();
                });

            // ��̏����ŁA�P���̃{�^���𐧌䂵�Ȃ��Ă����ׂẴ{�^���� interactable �ɂł���
            //// �{�^���̍w��
            //charaButton.GetButton().OnClickAsObservable()
            //    .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            //    .Subscribe(_ => 
            //    {
            //        charaButton.OnClickCharaButton();

            //        // ���ׂẴL�����{�^���̔񊈐���
            //        InactiveAllCharaButtons();

            //        UserData.instance.beforePoint = UserData.instance.MosaicCount.Value;
            //    })
            //    .AddTo(gameObject);

            charaButtonList.Add(charaButton);
        }
        // �{�^���� Subscribe ���Ă��Ȃ��̂ŁAReactiveProperty �̕����~�߂�
        sharedGate.AddTo(gameObject);
    }

    /// <summary>
    /// ���ׂẴL�����{�^����񊈐����@���@�s�v
    /// </summary>
    public void InactiveAllCharaButtons() {
        for (int i = 0; i < charaButtonList.Count; i++) {
            charaButtonList[i].InactibeCharaButton();
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�|�b�v�A�b�v�̐����ƃI�[�v��
    /// </summary>
    private void PrepareAchievementPopUp() {
        // �|�b�v�A�b�v����������Ă��Ȃ����
        if (!achievementPopUp) {
            // �������ď����ݒ肵�Ă���J��
            achievementPopUp = Instantiate(achievementPopUpPrefab);
            achievementPopUp.Setup();
        } else {
            // �|�b�v�A�b�v���J��
            achievementPopUp.OpenPopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }

    /// <summary>
    /// �M�������[�|�b�v�A�b�v�̐����ƃI�[�v��
    /// </summary>
    private void PrepareGalleryPopUp() {
        if (!galleryPopUp) {
            galleryPopUp = Instantiate(galleryPopUpPrefab);
            galleryPopUp.SetUp();
        } else {
            galleryPopUp.OpenPopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }
}