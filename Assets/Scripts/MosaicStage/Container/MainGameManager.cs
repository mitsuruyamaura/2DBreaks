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

    // �Q�b�^�[ //

    public yamap.StageData GetCurrentStageData() => currentStageData;

    public int GetTargetFeverPoint() => targetFeverPoint;
    public int GetFeverDuration() => feverDuraiton;


    //void IInitializable.Initialize() {
    //    SetUpStageData();
    //}

    public void SetUpStageData() {
        State.Value = GameState.Ready;
        
        CurrentAchievementStageData = new AchievementStageData(SelectStage.stageNo);

        // �X�e�[�W���̎擾
        currentStageData = UserData.instance.GetStageData(SelectStage.stageNo);
        //Debug.Log(currentStageData.stageNo);

        //Debug.Log("Stage SetUp");
    }

    /// <summary>
    /// �������O���b�h�̐��̉��Z 
    /// </summary>
    /// <param name="eraseTileGridCount"></param>
    public void UpdateTotalErasePoint(int eraseTileGridCount) {
        // �������O���b�h�̐��̉��Z�B�t�B�[�o�[����3 - 5�{
        TotalErasePoint.Value += IsFeverTime.Value ? eraseTileGridCount * (3 + currentStageData.stageNo) : eraseTileGridCount;

        // �������O���b�h���̍ő�l�̍X�V�m�F
        if (eraseTileGridCount > CurrentAchievementStageData.maxLinkCount) CurrentAchievementStageData.maxLinkCount = eraseTileGridCount;
    }

    /// <summary>
    /// �t�B�[�o�[�|�C���g�̉��Z
    /// </summary>
    /// <param name="eraseTileGridCount"></param>
    public void UpdateFeverPoint(int eraseTileGridCount) {
        // �t�B�[�o�[���̏ꍇ
        if (IsFeverTime.Value) {
            // �t�B�[�o�[�� SE
            SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Fever);
            return;
        }

        // �t�B�[�o�[���Ă��Ȃ��ꍇ
        // �ő�l�𒴂��Ȃ��悤�Ƀt�B�[�o�[�|�C���g���Z�@�������� - 2
        FeverPoint.Value = Mathf.Min(targetFeverPoint, FeverPoint.Value += CalculateFeverPoint(eraseTileGridCount));
        //Debug.Log(FeverPoint.Value);

        // �t�B�[�o�[�̊m�F
        if (CheckFeverTime()) {
            //Debug.Log(IsFeverTime.Value);
            // �t�B�[�o�[�񐔉��Z
            CurrentAchievementStageData.maxFeverCount++;
            //Debug.Log("Fever �˓� :" + FeverPoint.Value);

            // �t�B�[�o�[ Voice
            SoundManager.instance?.PlayVoice(SoundManager.VOICE_TYPE.�t�B�[�o�[);
        } else {
            // �ʏ�̍폜SE
            SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Erase);
        }
    }

    /// <summary>
    /// �t�B�[�o�[�|�C���g�̌v�Z
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private int CalculateFeverPoint(int point) => point - 2;

    /// <summary>
    /// �t�B�[�o�[�̔���
    /// </summary>
    /// <returns></returns>
    public bool CheckFeverTime() {
        return IsFeverTime.Value = FeverPoint.Value >= targetFeverPoint ? true : false;
    }

    public void PrepareGameClear() {
        //Debug.Log("Game Clear");
        State.Value = GameState.GameUp;

        // �N���A�^�C���̕ێ�
        CurrentAchievementStageData.fastestClearTime = GameTime.Value;
    }
}