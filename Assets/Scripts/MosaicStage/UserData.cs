using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// このデータが存在する時点で、そのステージはクリア済であることが保証される
/// </summary>
[System.Serializable]
public class StageClearData {
    public StageType stageType;
    public int stageNo;
    public bool isOneMissClear;   // ワンミス以内達成
    public bool isNoMissClear;    // ノーミス達成

    public StageClearData(StageType stageType, int stageNo, bool isOneMissClear, bool isNoMissClear) {
        this.stageType = stageType;
        this.stageNo = stageNo;
        this.isOneMissClear = isOneMissClear;
        this.isNoMissClear = isNoMissClear;
    }

    /// <summary>
    /// クリア結果を反映する(達成済みフラグは上書きしない)
    /// </summary>
    public void ApplyClearResult(bool oneMissClear, bool noMissClear) {
        if (oneMissClear) {
            isOneMissClear = true;
        }

        if (noMissClear) {
            isNoMissClear = true;
        }
    }
}

public class UserData : MonoBehaviour, IEntryRun
{
    public static UserData instance;

    [SerializeField] private yamap.StageDataSO stageDataSO;
    [SerializeField] private StageDifficultyDataSO stageDifficultyDataSO;
    [SerializeField] private VideoDataSO videoDataSO;

    public ReactiveProperty<int> MosaicCount = new();
    public bool isOpenGallary;
    public int openGallaryPoint;
    public int beforePoint;
    public List<AchievementStageData> achievementStageDataList = new();

    public ReactiveProperty<ColorAssistanceState> CurrentColorAssistanceState = new(ColorAssistanceState.Off);
    public ReactiveProperty<Language> CurrentLanguage = new(Language.jp);
    public List<StageClearData> stageClearDataList = new();

    public bool isDebugClearBtns;
    private float defaultMasterVolume = 0.3f;

    /// <summary>
    /// セーブ・ロード用のクラス
    /// </summary>
    [System.Serializable]
    public class SaveData {
        public int mozaicPoint;
        //public List<int> clearStageNoList = new();
        public List<AchievementStageData> achievementStageDataList = new();
        public Language language;
        public ColorAssistanceState colorAssistanceState;
        public float masterVolume;
        public List<StageClearData> stageClearDataList = new();
    }

    private const string SAVE_KEY = "SaveData";        // SaveData クラス用の Key

    /// <summary>
    /// ゲーム起動時の処理
    /// </summary>
    public void EntryRun() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            //Init();　　// ボイス再生のタイミング上、EntryPoint で初期化
        } else {
            Destroy(this.gameObject);
        }
        //Debug.Log("UserData Entry 終了");
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public void Init() {
        // セーブデータがある場合
        if (PlayerPrefsHelper.ExistsData(SAVE_KEY)) {
            GetSaveData();
            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.挨拶_2回目以降);
            return;
        }

        // 初回起動時
        if (stageClearDataList.Count == 0) {
            //AddClearStageNoList(0);

            // 以前のもの。ステージ番号での実績データを初期化して追加
            //for (int i = 0; i < stageDataSO.stageDataList.Count; i++) {
            //    achievementStageDataList.Add(new AchievementStageData(stageDataSO.stageDataList[i].stageNo));     
            //}

            // ステージタイプを基に実績データを初期化して追加
            foreach (StageType stageType in Enum.GetValues(typeof(StageType))) {
                achievementStageDataList.Add(new AchievementStageData((int)stageType));
            }

            SoundManager.instance.SetMasterVolume(defaultMasterVolume);
            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.挨拶_初回);
            //Debug.Log("初回起動");
        }
    }

    /// <summary>
    /// クリアしたステージの番号をリストに追加
    /// </summary>
    /// <param name="no"></param>
    //public void AddClearStageNoList(int no) {
    //    clearStageNoList.Add(no);
    //}

    public void AddClearStageDataList(StageType stageType, int stageNo, bool isOneMissClear, bool isNoMissClear) {
        StageClearData stageClearData = new(stageType, stageNo, isOneMissClear, isNoMissClear);
        stageClearDataList.Add(stageClearData);

        // 昇順ソート(難易度 → ステージ番号)
        stageClearDataList = stageClearDataList.OrderBy(data => data.stageType).ThenBy(data => data.stageNo).ToList();
    }

    /// <summary>
    /// ステージの数の取得
    /// </summary>
    /// <returns></returns>
    public int GetStageCount() {
        return stageDataSO.stageDataList.Count;
    }

    public int GetStageTypeCount => Enum.GetValues(typeof(StageType)).Length;

    /// <summary>
    /// StageData の取得
    /// </summary>
    /// <param name="searchStageType"></param>
    /// <param name="searchStageNo"></param>
    /// <returns></returns>
    public yamap.StageData GetStageData(StageType searchStageType, int searchStageNo) {
        return stageDataSO.stageDataList.Find(x => x.stageType == searchStageType && x.stageNo == searchStageNo);
    }

    public yamap.StageData GetStageDataByStageNo(int searchStageNo) {
        return stageDataSO.stageDataList[searchStageNo];
    }

    public List<yamap.StageData> GetStageDataListByStageType(StageType searchStageType) {
        return stageDataSO.stageDataList.Where(data => data.stageType == searchStageType).ToList();
    }

    /// <summary>
    /// ポイントによるギャラリーの開放判定
    /// </summary>
    /// <returns></returns>
    public bool CheckOpenGallaryPoint() {
        return isOpenGallary = MosaicCount.Value >= openGallaryPoint ? true : false;
    }

    /// <summary>
    /// ノーミスクリアによるギャラリーの開放判定
    /// すべてのステージでノーミスクリアなら開放
    /// </summary>
    /// <returns></returns>
    public bool CheckOpenGalleryAllNoMissClears() {
        return achievementStageDataList.Select(x => x.noMissClearCount).All(x => x > 0);
    }

    /// <summary>
    /// モザイクカウントによるステージ開放判定
    /// </summary>
    public (bool[], int[]) CheckOpenStageDifficultyFromPoint() {
        bool[] isOpenStages = new bool[] { false, false, false };
        int[] openPoints = new int[] { 0, 0, 0 };

        for (int i = 0; i < stageDifficultyDataSO.stageDifficultyDataList.Count; i++) {
            // ポイントが超えている場合
            int stageOpenPoint = stageDifficultyDataSO.stageDifficultyDataList[i].stageOpenPoint;
            if (MosaicCount.Value >= stageOpenPoint) {
                isOpenStages[i] = true;
            }
            openPoints[i] = stageOpenPoint;
        }

        return (isOpenStages, openPoints);
    }

    /// <summary>
    /// 実績の更新確認
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

            // 初回はクリア時間を更新して初期値とする
            if (achievementStageDataList[i].fastestClearTime == 0) achievementStageDataList[i].fastestClearTime = currentAchievementStageData.fastestClearTime;
            // 2回目以降は小さい(クリアが早い)時のみ更新
            if (currentAchievementStageData.fastestClearTime < achievementStageDataList[i].fastestClearTime) achievementStageDataList[i].fastestClearTime = currentAchievementStageData.fastestClearTime;
            break;         
        }

        // セーブ
        SetSaveData();
    }

    /// <summary>
    /// セーブする値を SaveData に設定してセーブ
    /// セーブするタイミングは、ステージクリア時、ゲームオーバー時、設定変更時
    /// </summary>
    public void SetSaveData() {
        // セーブ用のデータを作成
        SaveData saveData = new() {
            // 各値を SaveData クラスの変数に設定
            mozaicPoint = MosaicCount.Value,
            achievementStageDataList = achievementStageDataList,
            language = CurrentLanguage.Value,
            colorAssistanceState = CurrentColorAssistanceState.Value,
            masterVolume = SoundManager.instance.masterVolume,
            stageClearDataList = stageClearDataList,
        };

        //Debug.Log($"SaveData masterVolume : {SoundManager.instance.masterVolume}");

        // SaveData クラスとして SAVE_KEY の名前でセーブ
        PlayerPrefsHelper.SaveSetObjectData(SAVE_KEY, saveData);
    }

    /// <summary>
    /// SaveData をロードして、各値に設定
    /// </summary>
    public void GetSaveData() {
        // SaveData としてロード
        SaveData saveData = PlayerPrefsHelper.LoadGetObjectData<SaveData>(SAVE_KEY);

        // 各値に SaveData 内の値を設定
        MosaicCount = new(saveData.mozaicPoint);
        achievementStageDataList = saveData.achievementStageDataList;
        CurrentLanguage = new(saveData.language);
        CurrentColorAssistanceState = new(saveData.colorAssistanceState);
        stageClearDataList = saveData.stageClearDataList;

        SoundManager.instance.masterVolume = saveData.masterVolume;
        SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, saveData.masterVolume);
    }

    /// <summary>
    /// セーブ用のキーの取得
    /// </summary>
    /// <returns></returns>
    public string GetSaveDataKey() {
        return SAVE_KEY;
    }

    /// <summary>
    /// 初期化準備
    /// </summary>
    public void PrepareReset() {
        ResetDataAsync().Forget();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public async UniTask ResetDataAsync() {
        MosaicCount.Value = 0;
        achievementStageDataList.Clear();
        stageClearDataList.Clear();

        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(1000, cancellationToken : token);

        Init();
    }

    /// <summary>
    /// 言語変更
    /// </summary>
    /// <param name="newLanguage"></param>
    public void ChangeLanguage(Language newLanguage) {
        CurrentLanguage.Value = newLanguage;
    }

    /// <summary>
    /// 色覚サポート状態の変更
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeColorAssistanceState(ColorAssistanceState newState) {
        CurrentColorAssistanceState.Value = newState;
    }

    /// <summary>
    /// ステージクリアデータの取得
    /// </summary>
    /// <param name="searchStageType"></param>
    /// <param name="searchStageNo"></param>
    /// <returns></returns>
    public StageClearData GetStageClearData(StageType searchStageType, int searchStageNo) {
        return stageClearDataList.FirstOrDefault(data => data.stageType == searchStageType && data.stageNo == searchStageNo);
    }

    /// <summary>
    /// 指定した難易度のステージクリアのリストを取得
    /// </summary>
    /// <param name="searchStageType"></param>
    /// <returns></returns>
    public List<StageClearData> GetStageClearDataListByStageType(StageType searchStageType) {
        return stageClearDataList.Where(data => data.stageType == searchStageType).ToList();
    }

    /// <summary>
    /// ボタン用キャラ画像の取得
    /// </summary>
    /// <param name="searchStageType"></param>
    /// <returns></returns>
    public Sprite GetBtnChara(StageType searchStageType) {
        return stageDifficultyDataSO.stageDifficultyDataList.FirstOrDefault(data => data.stageType == searchStageType).btnCharaSprite;
    }

    /// <summary>
    /// VideoClip 取得
    /// </summary>
    /// <param name="searchVideoId"></param>
    /// <returns></returns>
    public VideoClip GetVideoClip(int searchVideoId) {
        return videoDataSO.videoDataList.FirstOrDefault(data => data.videoId == searchVideoId).videoClip;
    }

    /// <summary>
    /// VideoData 取得
    /// </summary>
    /// <param name="searchVideoId"></param>
    /// <returns></returns>
    public VideoData GetVideoData(int searchVideoId) {
        return videoDataSO.videoDataList.FirstOrDefault(data => data.videoId == searchVideoId);
    }

    /// <summary>
    /// プレイリストの順番に VideoData を取得
    /// 未使用
    /// </summary>
    /// <param name="playlist"></param>
    /// <returns></returns>
    public List<VideoClip> GetVideoPlayListData(List<int> playlist) {
        //return playlist.Select(id => videoDataSO.videoDataList.FirstOrDefault(data => data.videoId == id).videoClip)
        //    .Where(data => data != null)  // null のものは省く
        //    .ToList();

        // 上記でも動くが、こちらの方が高速で数が増えたときにも対応できる
        var dict = videoDataSO.videoDataList.ToDictionary(d => d.videoId);

        // playlist を起点とすることで再生順を保証する
        return playlist
            .Where(id => dict.ContainsKey(id))
            .Select(id => dict[id].videoClip)
            .ToList();
    }
}