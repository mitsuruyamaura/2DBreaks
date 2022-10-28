using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using DG.Tweening;

public class MainGamePresenter : IAsyncStartable, ITickable, IDisposable {  // PackageManager �o�R�� UniTask �����Ȃ��ƈˑ��֌W���K�p����Ȃ����̂�����

    // View �N���X
    private MainGameInfoView mainGameInfoView;
    private LifeView lifeView;

    // Model �s���A�N���X
    private LifeModel lifeModel;
    private GridCalculator gridCalculator;

    // Model MonoBehaviour �N���X
    private MainGameManager mainGameManager;
    private TileGridBehaviour tileGridBehaviour;
    private ObstacleBehaviour obstacleBehaviour;

    private CompositeDisposable disposables;


    public MainGamePresenter(MainGameInfoView mainGameInfoView, LifeView lifeView,   // View
                             LifeModel lifeModel, GridCalculator gridCalculator,     // Pure Model
                             MainGameManager mainGameManager, TileGridBehaviour tileGridBehaviour, ObstacleBehaviour obstacleBehaviour)  // Mono
    {
        this.mainGameInfoView = mainGameInfoView;
        this.lifeView = lifeView;

        this.lifeModel = lifeModel;
        this.gridCalculator = gridCalculator;

        this.mainGameManager = mainGameManager;
        this.tileGridBehaviour = tileGridBehaviour;
        this.obstacleBehaviour = obstacleBehaviour;
        //Debug.Log(this.mainGameInfoView);
        //Debug.Log(this.lifeView);
        //Debug.Log(this.lifeModel);
        //Debug.Log(this.mainGameManager);
        //Debug.Log(this.tileGridBehaviour);
        //Debug.Log(this.gridCalculator);
        //Debug.Log(this.obstacleBehaviour);

        disposables = new();
    }

    void IDisposable.Dispose() => disposables.Dispose();

    public void Tick() {
        ((ITickable)gridCalculator).Tick();
    }

    /// <summary>
    /// EntryPoint
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask StartAsync(CancellationToken token) {
        //mainGameManager = new();
        //gridCalculator.Set(mainGameManager);

        //mainGameManager.State.Value = GameState.Ready;
        

        // �X�e�[�W���Ƃ� BGM �Đ�
        SoundManager.instance?.PlayBGM((SoundManager.BGM_TYPE)Enum.Parse(typeof(SoundManager.BGM_TYPE), "Stage_" + SelectStage.stageNo));

        //mainGameManager.CurrentAchievementStageData = new AchievementStageData(SelectStage.stageNo);
        mainGameInfoView.HideGameUpInfo();

        // �X�e�[�W���̎擾
        mainGameManager.SetUpStageData();

        //Debug.Log(mainGameManager.GetCurrentStageData());

        // �L�����摜�̐ݒ�
        mainGameInfoView.SetCharaSprite(mainGameManager.GetCurrentStageData());

        // �O���b�h�̐���
        tileGridBehaviour.CreateTileGrids();

        // ��Q���̐���(�������� UpdateAsObservable ������ɋ@�\���Ȃ�)
        //obstacleBehaviour.CreateObstacles(mainGameManager.GetCurrentStageData().obstacleCount, mainGameManager.GetCurrentStageData().obstacleSpeeds, mainGameManager, lifeModel, this);

        // ���C�t�A�C�R���̐���
        lifeModel.SetLifeCount();
        lifeView.CreateLifeIconAsync(lifeModel.LifeCount.Value, token).Forget();


        // ���C�t�̍w��
        lifeModel.LifeCount
            .Where(_ => mainGameManager.State.Value == GameState.Play)
            .Subscribe(_ => {
                // SE
                SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Miss);

                // ���C�t�A�C�R���X�V�Ɖ�ʂ�Ԃ�����G�t�F�N�g�Đ�
                lifeView.ReduceLife();

                OnDamage(token);
            })
            .AddTo(token);

        // �X�e�[�g���������̂Ƃ������Q�[���J�n�ɕύX
        if (mainGameManager.State.Value == GameState.Ready) {
            mainGameManager.State.Value = GameState.Play;
        }

        // �Q�[�����Ԃ̊Ď�
        mainGameManager.GameTime.Subscribe(x => mainGameInfoView.UpdateGameTime(x)).AddTo(disposables);

        // �󂵂��O���b�h�̊Ď�
        mainGameManager.TotalErasePoint
            .Zip(mainGameManager.TotalErasePoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => mainGameInfoView.UpdateMosaicCount(x.oldValue, x.newValue)).AddTo(disposables);

        // �����l�Z�b�g
        mainGameManager.TotalErasePoint.SetValueAndForceNotify(0);

        // �t�B�[�o�[�Q�[�W�̐ݒ�
        mainGameInfoView.SetUpSliderValue(mainGameManager.GetTargetFeverPoint());

        // �t�B�[�o�[�|�C���g�̍w��(�t�B�[�o�[���Ă��Ȃ����������Z�����l)
        mainGameManager.FeverPoint
            .Zip(mainGameManager.FeverPoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x =>
            {
                // �t�B�[�o�[�̃p�[�Z���g�\���X�V
                mainGameInfoView.UpdateDisplayValue(x.oldValue, x.newValue, mainGameManager.GetTargetFeverPoint(), mainGameManager.GetFeverDuration());

                // �t�B�[�o�[���̏ꍇ(�߂�l�ł͕]�����Ȃ��B�t�B�[�o�[�����|�C���g���������Ă��邽�߁A�߂�l�� IsFeverTime �l���X�V���Ă��܂��� false �ɂȂ�)
                if (mainGameManager.IsFeverTime.Value) {                    
                    return;
                }

                // �t�B�[�o�[�̊m�F
                if (!mainGameManager.CheckFeverTime()) {       
                    // �t�B�[�o�[�łȂ���΃Q�[�W����
                    mainGameInfoView.UpdateFeverSlider(mainGameManager.FeverPoint.Value, 0.25f);
                } else {
                    //Debug.Log(mainGameManager.CheckFeverTime());
                    // �t�B�[�o�[�Ď�
                    ObserveFeverTimeAsync(token).Forget();
                }               
            })
            .AddTo(disposables);

        // �O���b�h�̐��̍w��
        tileGridBehaviour.TileGridList            
            .ObserveCountChanged()
            .Where(x => x != 0)
            .Subscribe((int count) => 
            {
                // �c��2�ȉ��Ȃ炷�ׂč폜���ăN���A
                if (count <= 2) {
                    //Debug.Log("Game Clear");
                    tileGridBehaviour.AllEraseTileGird();

                    obstacleBehaviour.DestroyAllObstacles();

                    mainGameManager.PrepareGameClear();

                    Result(true, token);
                }
            })
            .AddTo(disposables);

        await UniTask.Yield();

        // ��Q���̐���
        obstacleBehaviour.CreateObstacles(mainGameManager.GetCurrentStageData().obstacleCount, mainGameManager.GetCurrentStageData().obstacleSpeeds, mainGameManager, lifeModel);


        mainGameManager.State.Subscribe(x => 
        {
            if (x == GameState.Ready) {
                OnDamage(token);
            }           
            //Debug.Log("���݂� State : " + mainGameManager.State.Value);         
        });
    }

    public void OnDamage(CancellationToken token) {

        // �Q�[���I�[�o�[����
        if (lifeModel.IsNotLifeLeft()) {

            mainGameManager.State.Value = GameState.GameUp;

            obstacleBehaviour.StopAllObstacles();
            //Debug.Log("Game Over");

            // ���Z
            Result(false, token);
            //Debug.Log("�ړ���~ : " + mainGameManager.State.Value);
        } else {
            // ��Q���ɐڐG�����ۂ̃O���b�h�̏���
            gridCalculator.TriggerObstacle();

            // ���ׂĂ̏�Q���̍Ĉړ��J�n
            obstacleBehaviour.RestartAllObstacles();

            mainGameManager.State.Value = GameState.Play;
            //Debug.Log("�ړ��ĊJ : " + mainGameManager.State.Value);
        }
    }

    /// <summary>
    /// �t�B�[�o�[�^�C���̊Ď�
    /// </summary>
    /// <returns></returns>
    public async UniTask ObserveFeverTimeAsync(CancellationToken token) {
        // ��Q���̈ړ����x��ᑬ�ɂ��A�r���ł̑��x�A�b�v���Ȃ��ɂ���
        obstacleBehaviour.SlowDownAllObstacles();

        // 100�� �̃A�j�����o���I���܂őҋ@
        await UniTask.Delay(500, cancellationToken: token);

        mainGameInfoView.SetFeverTime(mainGameManager.GetTargetFeverPoint(), mainGameManager.GetFeverDuration());

        // �Q�[�W�ɍ��킹�āA������ 100% -> 0% �ɂ���
        mainGameManager.FeverPoint.Value = 0;

        await UniTask.Delay(mainGameManager.GetFeverDuration(), cancellationToken: token);
        //Debug.Log(mainGameManager.IsFeverTime.Value);
        mainGameManager.IsFeverTime.Value = false;
        //Debug.Log("�t�B�[�o�[�^�C���I��");

        // ��Q���̈ړ����x��߂�
        obstacleBehaviour.RestartAllObstacles();
    }

    /// <summary>
    /// �Q�[�����e�̐��Z
    /// </summary>
    /// <param name="isClear"></param>
    private void Result(bool isClear, CancellationToken token) {
        // ����̏������u���b�N�̃|�C���g�����Z
        UserData.instance.MosaicCount.Value += mainGameManager.TotalErasePoint.Value;

        // �ő�l�Ƃ��Ĉ�U�ێ�
        mainGameManager.CurrentAchievementStageData.maxMosaicCount = mainGameManager.TotalErasePoint.Value;

        // �`�������W�񐔉��Z
        mainGameManager.CurrentAchievementStageData.challengeCount++;

        // �Q�[���I�[�o�[�̏ꍇ
        if (!isClear) {
            // �Q�[���I�[�o�[�񐔉��Z
            mainGameManager.CurrentAchievementStageData.failureCount++;

            // ���т̍X�V�m�F
            UserData.instance.CheckUpdateAchievementStageData(mainGameManager.CurrentAchievementStageData);

            GameOverAsync(token).Forget();
            return;
        }

        // �N���A�����X�e�[�W���ŏI�X�e�[�W�ł͂Ȃ��āA���N���A�̃X�e�[�W�̏ꍇ
        if (mainGameManager.GetCurrentStageData().stageNo != 2 && !UserData.instance.clearStageNoList.Contains(mainGameManager.GetCurrentStageData().stageNo + 1)) {
            // ���̃X�e�[�W��ǉ�
            UserData.instance.AddClearStageNoList(mainGameManager.GetCurrentStageData().stageNo + 1);
        }
        // �N���A�񐔉��Z
        mainGameManager.CurrentAchievementStageData.clearCount++;

        // �m�[�~�X�N���A�̏ꍇ
        if (lifeModel.IsNoMissClear()) {
            mainGameManager.CurrentAchievementStageData.noMissClearCount++;
        }
        // ���т̍X�V�m�F
        UserData.instance.CheckUpdateAchievementStageData(mainGameManager.CurrentAchievementStageData);
        GameClearAsync(token).Forget();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    /// <returns></returns>
    private async UniTask GameOverAsync(CancellationToken token) {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameOver);

        mainGameInfoView.ShowGameOver();

        await UniTask.Delay(1000, cancellationToken: token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�Q�[���I�[�o�[);

        // �N���b�N�҂�
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // �V�[���J��
        TransitionManager.instance.PrepareNextScene(SCENE_STATE.Menu);
    }

    /// <summary>
    /// �Q�[���N���A���o
    /// </summary>
    /// <returns></returns>
    private async UniTask GameClearAsync(CancellationToken token) {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameClear);

        mainGameInfoView.ShowGameClear();

        await UniTask.Delay(1000, cancellationToken: token);

        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�N���A_1);

        await UniTask.Delay(1000, cancellationToken: token);

        // �N���b�N�҂�
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�N���A_2);

        // �m�[�~�X�N���A
        if (lifeModel.IsNoMissClear()) {
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Excellent);

            mainGameInfoView.ShowExcellentLogo();
            await UniTask.Delay(1500, cancellationToken: token);

            // �N���b�N�҂�
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);

            // �G�N�Z�����g�̃��S�������A���A�摜�̃A�j���\��
            mainGameInfoView.HideExcellentLogo();

            await UniTask.Delay(1500, cancellationToken: token);

            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�G�N�Z�����g);

            // �N���b�N�҂�
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
        }
        // �{�C�X���Ō�܂ŗ�����������
        await UniTask.Delay(1000, cancellationToken: token);

        // �V�[���J��
        TransitionManager.instance.PrepareNextScene(SCENE_STATE.Menu);
    }
}