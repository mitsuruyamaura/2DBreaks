using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using DG.Tweening;

public class MainGamePresenter : IAsyncStartable, ITickable, IDisposable {  // PackageManager 経由で UniTask を入れないと依存関係が適用されないものがある

    // View クラス
    private MainGameInfoView mainGameInfoView;
    private LifeView lifeView;

    // Model ピュアクラス
    private LifeModel lifeModel;
    private GridCalculator gridCalculator;

    // Model MonoBehaviour クラス
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
        

        // ステージごとの BGM 再生
        SoundManager.instance?.PlayBGM((SoundManager.BGM_TYPE)Enum.Parse(typeof(SoundManager.BGM_TYPE), "Stage_" + SelectStage.stageNo));

        //mainGameManager.CurrentAchievementStageData = new AchievementStageData(SelectStage.stageNo);
        mainGameInfoView.HideGameUpInfo();

        // ステージ情報の取得
        mainGameManager.SetUpStageData();

        //Debug.Log(mainGameManager.GetCurrentStageData());

        // キャラ画像の設定
        mainGameInfoView.SetCharaSprite(mainGameManager.GetCurrentStageData());

        // グリッドの生成
        tileGridBehaviour.CreateTileGrids();

        // 障害物の生成(ここだと UpdateAsObservable が正常に機能しない)
        //obstacleBehaviour.CreateObstacles(mainGameManager.GetCurrentStageData().obstacleCount, mainGameManager.GetCurrentStageData().obstacleSpeeds, mainGameManager, lifeModel, this);

        // ライフアイコンの生成
        lifeModel.SetLifeCount();
        lifeView.CreateLifeIconAsync(lifeModel.LifeCount.Value, token).Forget();


        // ライフの購読
        lifeModel.LifeCount
            .Where(_ => mainGameManager.State.Value == GameState.Play)
            .Subscribe(_ => {
                // SE
                SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Miss);

                // ライフアイコン更新と画面を赤くするエフェクト再生
                lifeView.ReduceLife();

                OnDamage(token);
            })
            .AddTo(token);

        // ステートが準備中のときだけゲーム開始に変更
        if (mainGameManager.State.Value == GameState.Ready) {
            mainGameManager.State.Value = GameState.Play;
        }

        // ゲーム時間の監視
        mainGameManager.GameTime.Subscribe(x => mainGameInfoView.UpdateGameTime(x)).AddTo(disposables);

        // 壊したグリッドの監視
        mainGameManager.TotalErasePoint
            .Zip(mainGameManager.TotalErasePoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => mainGameInfoView.UpdateMosaicCount(x.oldValue, x.newValue)).AddTo(disposables);

        // 初期値セット
        mainGameManager.TotalErasePoint.SetValueAndForceNotify(0);

        // フィーバーゲージの設定
        mainGameInfoView.SetUpSliderValue(mainGameManager.GetTargetFeverPoint());

        // フィーバーポイントの購読(フィーバーしていない時だけ加算される値)
        mainGameManager.FeverPoint
            .Zip(mainGameManager.FeverPoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x =>
            {
                // フィーバーのパーセント表示更新
                mainGameInfoView.UpdateDisplayValue(x.oldValue, x.newValue, mainGameManager.GetTargetFeverPoint(), mainGameManager.GetFeverDuration());

                // フィーバー中の場合(戻り値では評価しない。フィーバー中もポイントが減少しているため、戻り値で IsFeverTime 値を更新してしまうと false になる)
                if (mainGameManager.IsFeverTime.Value) {                    
                    return;
                }

                // フィーバーの確認
                if (!mainGameManager.CheckFeverTime()) {       
                    // フィーバーでなければゲージ増加
                    mainGameInfoView.UpdateFeverSlider(mainGameManager.FeverPoint.Value, 0.25f);
                } else {
                    //Debug.Log(mainGameManager.CheckFeverTime());
                    // フィーバー監視
                    ObserveFeverTimeAsync(token).Forget();
                }               
            })
            .AddTo(disposables);

        // グリッドの数の購読
        tileGridBehaviour.TileGridList            
            .ObserveCountChanged()
            .Where(x => x != 0)
            .Subscribe((int count) => 
            {
                // 残り2個以下ならすべて削除してクリア
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

        // 障害物の生成
        obstacleBehaviour.CreateObstacles(mainGameManager.GetCurrentStageData().obstacleCount, mainGameManager.GetCurrentStageData().obstacleSpeeds, mainGameManager, lifeModel);


        mainGameManager.State.Subscribe(x => 
        {
            if (x == GameState.Ready) {
                OnDamage(token);
            }           
            //Debug.Log("現在の State : " + mainGameManager.State.Value);         
        });
    }

    public void OnDamage(CancellationToken token) {

        // ゲームオーバー判定
        if (lifeModel.IsNotLifeLeft()) {

            mainGameManager.State.Value = GameState.GameUp;

            obstacleBehaviour.StopAllObstacles();
            //Debug.Log("Game Over");

            // 清算
            Result(false, token);
            //Debug.Log("移動停止 : " + mainGameManager.State.Value);
        } else {
            // 障害物に接触した際のグリッドの処理
            gridCalculator.TriggerObstacle();

            // すべての障害物の再移動開始
            obstacleBehaviour.RestartAllObstacles();

            mainGameManager.State.Value = GameState.Play;
            //Debug.Log("移動再開 : " + mainGameManager.State.Value);
        }
    }

    /// <summary>
    /// フィーバータイムの監視
    /// </summary>
    /// <returns></returns>
    public async UniTask ObserveFeverTimeAsync(CancellationToken token) {
        // 障害物の移動速度を低速にし、途中での速度アップもなしにする
        obstacleBehaviour.SlowDownAllObstacles();

        // 100％ のアニメ演出が終わるまで待機
        await UniTask.Delay(500, cancellationToken: token);

        mainGameInfoView.SetFeverTime(mainGameManager.GetTargetFeverPoint(), mainGameManager.GetFeverDuration());

        // ゲージに合わせて、数字も 100% -> 0% にする
        mainGameManager.FeverPoint.Value = 0;

        await UniTask.Delay(mainGameManager.GetFeverDuration(), cancellationToken: token);
        //Debug.Log(mainGameManager.IsFeverTime.Value);
        mainGameManager.IsFeverTime.Value = false;
        //Debug.Log("フィーバータイム終了");

        // 障害物の移動速度を戻す
        obstacleBehaviour.RestartAllObstacles();
    }

    /// <summary>
    /// ゲーム内容の清算
    /// </summary>
    /// <param name="isClear"></param>
    private void Result(bool isClear, CancellationToken token) {
        // 今回の消したブロックのポイントを加算
        UserData.instance.MosaicCount.Value += mainGameManager.TotalErasePoint.Value;

        // 最大値として一旦保持
        mainGameManager.CurrentAchievementStageData.maxMosaicCount = mainGameManager.TotalErasePoint.Value;

        // チャレンジ回数加算
        mainGameManager.CurrentAchievementStageData.challengeCount++;

        // ゲームオーバーの場合
        if (!isClear) {
            // ゲームオーバー回数加算
            mainGameManager.CurrentAchievementStageData.failureCount++;

            // 実績の更新確認
            UserData.instance.CheckUpdateAchievementStageData(mainGameManager.CurrentAchievementStageData);

            GameOverAsync(token).Forget();
            return;
        }

        // クリアしたステージが最終ステージではなくて、初クリアのステージの場合
        if (mainGameManager.GetCurrentStageData().stageNo != 2 && !UserData.instance.clearStageNoList.Contains(mainGameManager.GetCurrentStageData().stageNo + 1)) {
            // 次のステージを追加
            UserData.instance.AddClearStageNoList(mainGameManager.GetCurrentStageData().stageNo + 1);
        }
        // クリア回数加算
        mainGameManager.CurrentAchievementStageData.clearCount++;

        // ノーミスクリアの場合
        if (lifeModel.IsNoMissClear()) {
            mainGameManager.CurrentAchievementStageData.noMissClearCount++;
        }
        // 実績の更新確認
        UserData.instance.CheckUpdateAchievementStageData(mainGameManager.CurrentAchievementStageData);
        GameClearAsync(token).Forget();
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    /// <returns></returns>
    private async UniTask GameOverAsync(CancellationToken token) {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameOver);

        mainGameInfoView.ShowGameOver();

        await UniTask.Delay(1000, cancellationToken: token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.ゲームオーバー);

        // クリック待ち
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // シーン遷移
        TransitionManager.instance.PrepareNextScene(SCENE_STATE.Menu);
    }

    /// <summary>
    /// ゲームクリア演出
    /// </summary>
    /// <returns></returns>
    private async UniTask GameClearAsync(CancellationToken token) {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameClear);

        mainGameInfoView.ShowGameClear();

        await UniTask.Delay(1000, cancellationToken: token);

        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.クリア_1);

        await UniTask.Delay(1000, cancellationToken: token);

        // クリック待ち
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.クリア_2);

        // ノーミスクリア
        if (lifeModel.IsNoMissClear()) {
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Excellent);

            mainGameInfoView.ShowExcellentLogo();
            await UniTask.Delay(1500, cancellationToken: token);

            // クリック待ち
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);

            // エクセレントのロゴを消し、レア画像のアニメ表示
            mainGameInfoView.HideExcellentLogo();

            await UniTask.Delay(1500, cancellationToken: token);

            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.エクセレント);

            // クリック待ち
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
        }
        // ボイスを最後まで流したいため
        await UniTask.Delay(1000, cancellationToken: token);

        // シーン遷移
        TransitionManager.instance.PrepareNextScene(SCENE_STATE.Menu);
    }
}