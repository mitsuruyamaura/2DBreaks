using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Linq;

public class UserData : MonoBehaviour, IEntryRun
{
    public static UserData instance;

    [SerializeField]
    private yamap.StageDataSO stageDataSO;

    public ReactiveProperty<int> MosaicCount = new();
    public List<int> clearStageNoList = new();
    public bool isOpenGallary;
    public int openGallaryPoint;
    public int beforePoint;
    public List<AchievementStageData> achievementStageDataList = new();


    /// <summary>
    /// �Z�[�u�E���[�h�p�̃N���X
    /// </summary>
    [System.Serializable]
    public class SaveData {
        public int mozaicPoint;
        public List<int> clearStageNoList = new();
        public List<AchievementStageData> achievementStageDataList = new();
    }

    private const string SAVE_KEY = "SaveData";        // SaveData �N���X�p�� Key

    /// <summary>
    /// �Q�[���N�����̏���
    /// </summary>
    public void EntryRun() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            //Init();�@�@// �{�C�X�Đ��̃^�C�~���O��AEntryPoint �ŏ�����
        } else {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// �����ݒ�
    /// </summary>
    public void Init() {
        // �Z�[�u�f�[�^������ꍇ
        if (PlayerPrefsHelper.ExistsData(SAVE_KEY)) {
            GetSaveData();
            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.���A_2��ڈȍ~);
            return;
        }

        // ����N����
        if (clearStageNoList.Count == 0) {
            AddClearStageNoList(0);

            for (int i = 0; i < stageDataSO.stageDataList.Count; i++) {
                achievementStageDataList.Add(new AchievementStageData(stageDataSO.stageDataList[i].stageNo));
            }
            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.���A_����);
            //Debug.Log("����N��");
        }
    }

    /// <summary>
    /// �N���A�����X�e�[�W�̔ԍ������X�g�ɒǉ�
    /// </summary>
    /// <param name="no"></param>
    public void AddClearStageNoList(int no) {
        clearStageNoList.Add(no);
    }

    /// <summary>
    /// �X�e�[�W�̐��̎擾
    /// </summary>
    /// <returns></returns>
    public int GetStageCount() {
        return stageDataSO.stageDataList.Count;
    }

    /// <summary>
    /// StageData �̎擾
    /// </summary>
    /// <param name="searchStageNo"></param>
    /// <returns></returns>
    public yamap.StageData GetStageData(int searchStageNo) {
        return stageDataSO.stageDataList.Find(x => x.stageNo == searchStageNo);
    }

    /// <summary>
    /// �|�C���g�ɂ��M�������[�̊J������
    /// </summary>
    /// <returns></returns>
    public bool CheckOpenGallaryPoint() {
        return isOpenGallary = MosaicCount.Value >= openGallaryPoint ? true : false;
    }

    /// <summary>
    /// �m�[�~�X�N���A�ɂ��M�������[�̊J������
    /// ���ׂẴX�e�[�W�Ńm�[�~�X�N���A�Ȃ�J��
    /// </summary>
    /// <returns></returns>
    public bool CheckOpenGalleryAllNoMissClears() {
        return achievementStageDataList.Select(x => x.noMissClearCount).All(x => x > 0);
    }

    /// <summary>
    /// ���U�C�N�J�E���g�ɂ��X�e�[�W�J������
    /// </summary>
    public void CheckOpenStageFromPoint() {
        for (int i = 0; i < stageDataSO.stageDataList.Count; i++) {
            // ���łɃN���A�ς̏ꍇ
            if (clearStageNoList.Contains(stageDataSO.stageDataList[i].stageNo)) {
                continue;
            }

            // �|�C���g�������Ă���ꍇ
            if (MosaicCount.Value >= stageDataSO.stageDataList[i].stageOpenPoint) {
                clearStageNoList.Add(stageDataSO.stageDataList[i].stageNo);
            }
        }
    }

    /// <summary>
    /// ���т̍X�V�m�F
    /// </summary>
    /// <param name="currentAchievementStageData"></param>
    public void CheckUpdateAchievementStageData(AchievementStageData currentAchievementStageData) {

        for (int i = 0; i < achievementStageDataList.Count; i++) {
            if (achievementStageDataList[i].stageNo != currentAchievementStageData.stageNo) {
                continue;
            }

            if (currentAchievementStageData.challengeCount != 0) achievementStageDataList[i].challengeCount += currentAchievementStageData.challengeCount;
            if (currentAchievementStageData.clearCount != 0) achievementStageDataList[i].clearCount += currentAchievementStageData.clearCount;
            if (currentAchievementStageData.failureCount != 0) achievementStageDataList[i].failureCount += currentAchievementStageData.failureCount;
            if (currentAchievementStageData.maxFeverCount > achievementStageDataList[i].maxFeverCount) achievementStageDataList[i].maxFeverCount = currentAchievementStageData.maxFeverCount;
            if (currentAchievementStageData.noMissClearCount != 0) achievementStageDataList[i].noMissClearCount += currentAchievementStageData.noMissClearCount;
            if (currentAchievementStageData.maxMosaicCount > achievementStageDataList[i].maxMosaicCount) achievementStageDataList[i].maxMosaicCount = currentAchievementStageData.maxMosaicCount;
            if (currentAchievementStageData.maxLinkCount > achievementStageDataList[i].maxLinkCount) achievementStageDataList[i].maxLinkCount = currentAchievementStageData.maxLinkCount;

            // ����̓N���A���Ԃ��X�V���ď����l�Ƃ���
            if (achievementStageDataList[i].fastestClearTime == 0) achievementStageDataList[i].fastestClearTime = currentAchievementStageData.fastestClearTime;
            // 2��ڈȍ~�͏�����(�N���A������)���̂ݍX�V
            if (currentAchievementStageData.fastestClearTime < achievementStageDataList[i].fastestClearTime) achievementStageDataList[i].fastestClearTime = currentAchievementStageData.fastestClearTime;
            break;         
        }

        // �Z�[�u
        SetSaveData();
    }

    /// <summary>
    /// �Z�[�u����l�� SaveData �ɐݒ肵�ăZ�[�u
    /// �Z�[�u����^�C�~���O�́A�X�e�[�W�N���A���A�L�����_��
    /// </summary>
    public void SetSaveData() {

        // �Z�[�u�p�̃f�[�^���쐬
        SaveData saveData = new() {

            // �e�l�� SaveData �N���X�̕ϐ��ɐݒ�
            mozaicPoint = MosaicCount.Value,
            clearStageNoList = clearStageNoList,
            achievementStageDataList = achievementStageDataList,
        };

        // SaveData �N���X�Ƃ��� SAVE_KEY �̖��O�ŃZ�[�u
        PlayerPrefsHelper.SaveSetObjectData(SAVE_KEY, saveData);
    }

    /// <summary>
    /// SaveData �����[�h���āA�e�l�ɐݒ�
    /// </summary>
    public void GetSaveData() {

        // SaveData �Ƃ��ă��[�h
        SaveData saveData = PlayerPrefsHelper.LoadGetObjectData<SaveData>(SAVE_KEY);

        // �e�l�� SaveData ���̒l��ݒ�
        MosaicCount = new(saveData.mozaicPoint);
        clearStageNoList = saveData.clearStageNoList;
        achievementStageDataList = saveData.achievementStageDataList;
    }

    /// <summary>
    /// �Z�[�u�p�̃L�[�̎擾
    /// </summary>
    /// <returns></returns>
    public string GetSaveDataKey() {
        return SAVE_KEY;
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void PrepareReset() {
        ResetDataAsync().Forget();
    }

    /// <summary>
    /// ������
    /// </summary>
    public async UniTask ResetDataAsync() {
        MosaicCount.Value = 0;
        clearStageNoList.Clear();
        achievementStageDataList.Clear();

        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(1000, cancellationToken : token);

        Init();
    }
}