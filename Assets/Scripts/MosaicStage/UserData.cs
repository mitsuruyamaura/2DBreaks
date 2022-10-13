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
    /// セーブ・ロード用のクラス
    /// </summary>
    [System.Serializable]
    public class SaveData {
        public int mozaicPoint;
        public List<int> clearStageNoList = new();
        public List<AchievementStageData> achievementStageDataList = new();
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
        if (clearStageNoList.Count == 0) {
            AddClearStageNoList(0);

            for (int i = 0; i < stageDataSO.stageDataList.Count; i++) {
                achievementStageDataList.Add(new AchievementStageData(stageDataSO.stageDataList[i].stageNo));
            }
            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.挨拶_初回);
            //Debug.Log("初回起動");
        }
    }

    /// <summary>
    /// クリアしたステージの番号をリストに追加
    /// </summary>
    /// <param name="no"></param>
    public void AddClearStageNoList(int no) {
        clearStageNoList.Add(no);
    }

    /// <summary>
    /// ステージの数の取得
    /// </summary>
    /// <returns></returns>
    public int GetStageCount() {
        return stageDataSO.stageDataList.Count;
    }

    /// <summary>
    /// StageData の取得
    /// </summary>
    /// <param name="searchStageNo"></param>
    /// <returns></returns>
    public yamap.StageData GetStageData(int searchStageNo) {
        return stageDataSO.stageDataList.Find(x => x.stageNo == searchStageNo);
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
    public void CheckOpenStageFromPoint() {
        for (int i = 0; i < stageDataSO.stageDataList.Count; i++) {
            // すでにクリア済の場合
            if (clearStageNoList.Contains(stageDataSO.stageDataList[i].stageNo)) {
                continue;
            }

            // ポイントが超えている場合
            if (MosaicCount.Value >= stageDataSO.stageDataList[i].stageOpenPoint) {
                clearStageNoList.Add(stageDataSO.stageDataList[i].stageNo);
            }
        }
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
    /// セーブするタイミングは、ステージクリア時、キャラ契約時
    /// </summary>
    public void SetSaveData() {

        // セーブ用のデータを作成
        SaveData saveData = new() {

            // 各値を SaveData クラスの変数に設定
            mozaicPoint = MosaicCount.Value,
            clearStageNoList = clearStageNoList,
            achievementStageDataList = achievementStageDataList,
        };

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
        clearStageNoList = saveData.clearStageNoList;
        achievementStageDataList = saveData.achievementStageDataList;
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
        clearStageNoList.Clear();
        achievementStageDataList.Clear();

        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(1000, cancellationToken : token);

        Init();
    }
}