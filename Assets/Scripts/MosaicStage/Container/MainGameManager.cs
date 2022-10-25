using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Model
/// </summary>
public class MainGameManager  // : IInitializable
{
    private int targetFeverPoint = 15;   // 3 =>1  5 => 3  6 => 4  8 => 6
    private int feverDuraiton = 7500;
    public ReactiveProperty<bool> IsFeverTime = new(false);
    private yamap.StageData currentStageData;

    public ReactiveProperty<GameState> State = new(GameState.Ready);
    public ReactiveProperty<float> GameTime = new(0);
    public ReactiveProperty<int> FeverPoint = new(0);
    public ReactiveProperty<int> TotalErasePoint = new(0);

    public AchievementStageData CurrentAchievementStageData;

    // ゲッター //

    public yamap.StageData GetCurrentStageData() => currentStageData;

    public int GetTargetFeverPoint() => targetFeverPoint;
    public int GetFeverDuration() => feverDuraiton;


    //void IInitializable.Initialize() {
    //    SetUpStageData();
    //}

    public void SetUpStageData() {
        State.Value = GameState.Ready;
        
        CurrentAchievementStageData = new AchievementStageData(SelectStage.stageNo);

        // ステージ情報の取得
        currentStageData = UserData.instance.GetStageData(SelectStage.stageNo);
        //Debug.Log(currentStageData.stageNo);

        //Debug.Log("Stage SetUp");
    }

    /// <summary>
    /// 消したグリッドの数の加算 
    /// </summary>
    /// <param name="eraseTileGridCount"></param>
    public void UpdateTotalErasePoint(int eraseTileGridCount) {
        // 消したグリッドの数の加算。フィーバー中は3 - 5倍
        TotalErasePoint.Value += IsFeverTime.Value ? eraseTileGridCount * (3 + currentStageData.stageNo) : eraseTileGridCount;

        // 消したグリッド数の最大値の更新確認
        if (eraseTileGridCount > CurrentAchievementStageData.maxLinkCount) CurrentAchievementStageData.maxLinkCount = eraseTileGridCount;
    }

    /// <summary>
    /// フィーバーポイントの加算
    /// </summary>
    /// <param name="eraseTileGridCount"></param>
    public void UpdateFeverPoint(int eraseTileGridCount) {
        // フィーバー中の場合
        if (IsFeverTime.Value) {
            // フィーバー中 SE
            SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Fever);
            return;
        }

        // フィーバーしていない場合
        // 最大値を超えないようにフィーバーポイント加算　消した数 - 2
        FeverPoint.Value = Mathf.Min(targetFeverPoint, FeverPoint.Value += CalculateFeverPoint(eraseTileGridCount));
        //Debug.Log(FeverPoint.Value);

        // フィーバーの確認
        if (CheckFeverTime()) {
            //Debug.Log(IsFeverTime.Value);
            // フィーバー回数加算
            CurrentAchievementStageData.maxFeverCount++;
            //Debug.Log("Fever 突入 :" + FeverPoint.Value);

            // フィーバー Voice
            SoundManager.instance?.PlayVoice(SoundManager.VOICE_TYPE.フィーバー);
        } else {
            // 通常の削除SE
            SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Erase);
        }
    }

    /// <summary>
    /// フィーバーポイントの計算
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private int CalculateFeverPoint(int point) => point - 2;

    /// <summary>
    /// フィーバーの判定
    /// </summary>
    /// <returns></returns>
    public bool CheckFeverTime() {
        return IsFeverTime.Value = FeverPoint.Value >= targetFeverPoint ? true : false;
    }

    public void PrepareGameClear() {
        //Debug.Log("Game Clear");
        State.Value = GameState.GameUp;

        // クリアタイムの保持
        CurrentAchievementStageData.fastestClearTime = GameTime.Value;
    }
}