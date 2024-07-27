using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Linq;
using System;

// DI コンテナ採用により現在は使用しない
public class MosaicManager : MonoBehaviour
{
    [SerializeField]
    private TileGridDetail tileGridPrefab;

    [SerializeField]
    private GameObject eraseEffectPrefab;

    [SerializeField]
    private ObstacleBall obstacleBallPrefab;

    [SerializeField]
    private Transform tileGridSetTran;

    [SerializeField]
    private Transform[] obstacleBallTrans;

    //[SerializeField]
    //private int obstaclesCount;  // StageData から取得するので不要

    [SerializeField, Header("行")]
    private int rowCount;

    [SerializeField, Header("列")]
    private int columnCount;

    [SerializeField]
    private float tileGridSize;

    [SerializeField, Header("生成された Grid のリスト")]
    private List<TileGridDetail> tileGridList = new ();

    private TileGridDetail firstSelectTileGrid;
    private TileGridDetail lastSelectTileGrid;
    private TileGridType? currentTileGridType;

    [SerializeField]
    private List<TileGridDetail> eraseTileGridList = new ();

    [SerializeField]
    private List<ObstacleBall> obstacleList = new ();

    [SerializeField]
    private int linkCount = 0;

    [SerializeField]
    private float tileGridDistance = 1.0f;

    /// <summary>
    /// ゲームの進行状況
    /// </summary>
    public enum GameState {
        Ready,
        Play,
        GameUp
    }

    [Header("現在のゲームの進行状況")]
    public GameState gameState = GameState.Ready;

    private bool isLastColor;

    public ReactiveProperty<float> GameTime = new();

    [SerializeField]
    private Text[] txtGameTimes;

    [SerializeField]
    private GameObject lifeIconPrefab;

    [SerializeField]
    private Transform lifeSetTran;

    private int lifeCount = 3;

    [SerializeField]
    private List<GameObject> lifeIconlist = new();

    [SerializeField]
    private Text txtMosaicCount;


    [SerializeField]
    private SpriteRenderer normalChara;

    [SerializeField]
    private SpriteRenderer rareChara;

    [SerializeField]
    private FlushScreen flushScreen;

    private yamap.StageData currentStageData;
    private ReactiveProperty<int> TotalErasePoint = new();

    [SerializeField]
    private Slider sliderFever;

    [SerializeField]
    private Text txtValue;

    private ReactiveProperty<int> FeverPoint = new();

    private int targetFeverPoint = 15;   // 3 =>1  5 => 3  6 => 4  8 => 6
    public ReactiveProperty<bool> IsFeverTime = new(false);
    private int feverDuraiton = 7500;

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
    private AchievementStageData currentAchievementStageData;


    void Start()
    {
        gameState = GameState.Ready;

        // ステージごとの BGM 再生
        SoundManager.instance?.PlayBGM((SoundManager.BGM_TYPE)Enum.Parse(typeof(SoundManager.BGM_TYPE), "Stage_" + SelectStage.stageNo));

        currentAchievementStageData = new AchievementStageData(SelectStage.stageNo);
        txtInfo.gameObject.SetActive(false);

        // ステージ情報の取得
        currentStageData = UserData.instance.GetStageData(SelectStage.stageNo);

        // キャラ画像の設定
        SetCharaSprite();

        // グリッドの生成
        CreateTileGrids();

        // 障害物の生成
        CreateObstacles(currentStageData.obstacleCount);

        // ライフアイコンの生成
        var token = this.GetCancellationTokenOnDestroy();
        CreateLifeIconAsync(token).Forget();

        // ステートが準備中のときだけゲーム開始に変更
        if (gameState == GameState.Ready) {
            gameState = GameState.Play;
        }

        this.UpdateAsObservable()
            .Where(_ => gameState == GameState.Play)
            .Subscribe(_ => 
            {
                // グリッドをつなげる処理
                if (Input.GetMouseButtonDown(0) && firstSelectTileGrid == null) {
                    OnStartDrag();
                } else if (Input.GetMouseButtonUp(0)) {
                    OnEndDrag();
                } else if (firstSelectTileGrid != null) {
                    OnDragging();
                }

                GameTime.Value += Time.deltaTime;
            })
            .AddTo(this);

        // ゲーム時間の監視
        GameTime.Subscribe(x => UpdateGameTime(x)).AddTo(this);

        // 壊したグリッドの監視
        TotalErasePoint
            .Zip(TotalErasePoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateMosaicCount(x.oldValue, x.newValue)).AddTo(this);

        // 初期値セット
        TotalErasePoint.SetValueAndForceNotify(0);

        // フィーバーゲージの設定
        sliderFever.maxValue = targetFeverPoint;
        sliderFever.value = 0;

        // フィーバーポイントの購読
        FeverPoint
            .Zip(FeverPoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateDisplayValue(x.oldValue, x.newValue))
            .AddTo(sliderFever.gameObject);
    }

    /// <summary>
    /// ゲーム時間の表示更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateGameTime(float amount) {
        // 小数点以下は小さく表示
        string time = amount.ToString("F2");
        string[] part = time.Split('.');
        txtGameTimes[0].text = part[0] + ".";
        txtGameTimes[1].text = part[1];
    }

    /// <summary>
    /// Slider の上の % 表示の更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayValue(float oldValue, float newValue) {
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
        // 数字のアニメ時間の設定。フィーバーした際には長くなる
        float duration = newValue == 0 ? (float)feverDuraiton / 1000 : 0.25f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(txtValue.DOCounter(a, b, duration).SetEase(Ease.Linear)).SetLink(txtValue.gameObject);

        // 満タンになったら
        if (newValue == targetFeverPoint) {
            // 100％ の数字を見せるアニメ演出を追加
            float scale = txtValue.transform.localScale.x;
            sequence.Append( 
            txtValue.transform.DOPunchScale(txtValue.transform.localScale * 1.25f, 0.25f).SetEase(Ease.InQuart).SetLink(txtValue.gameObject)
                .OnComplete(() => txtValue.transform.localScale = Vector3.one * scale));
        }
    }

    /// <summary>
    /// ステージごとのキャラ画像の設定
    /// </summary>
    private void SetCharaSprite() {
        normalChara.sprite = currentStageData.normalCharaSprite;
        rareChara.sprite = currentStageData.rareCharaSprite;
        imgCharaIcon.sprite = currentStageData.charaIcon;

        imgCharaIcon.transform.DOShakeScale(0.75f, 1f, 4)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 壊したグリッドの数の表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateMosaicCount(int oldValue, int newValue) {
        txtMosaicCount.DOCounter(oldValue, newValue, 0.5f).SetEase(Ease.Linear);
        // 左上のキャラアイコンをアニメ
        imgCharaIcon.transform.DOPunchScale(Vector3.one * 1.25f, 0.25f)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }

    /// <summary>
    /// グリッドを最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag() {
        //Debug.Log("ドラッグ開始");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); //  Camera.main.ScreenToWorldPoint

        // グリッドがつながっている数を初期化
        linkCount = 0;

        if (hit.collider != null) {
            if (hit.collider.gameObject.TryGetComponent(out TileGridDetail startTileGrid)) {
                firstSelectTileGrid = startTileGrid;
                lastSelectTileGrid = startTileGrid;
                currentTileGridType = startTileGrid.tileGridType;

                startTileGrid.IsSelected = true;
                startTileGrid.Num = linkCount;

                eraseTileGridList = new ();
                AddEraseTileGridlList(startTileGrid);

                SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Block_Choose);
            }
        }
    }

    /// <summary>
    /// グリッドのドラッグ中（スワイプ）処理
    /// </summary>
    private void OnDragging() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out TileGridDetail dragTileGrid)) {

            // TileGrid 以外の場所の場合には何もしない
            if (currentTileGridType == null) {
                return;
            }

            // ドラッグした先のグリッドが現在選択しているグリッドのタイプかつ、選択済でない場合
            if (dragTileGrid.tileGridType == currentTileGridType && lastSelectTileGrid != dragTileGrid && !dragTileGrid.IsSelected) {
                float distance = Vector2.Distance(dragTileGrid.transform.position, lastSelectTileGrid.transform.position);

                // グリッドとグリッドの距離がつながる範囲内なら
                if (distance < tileGridDistance) {
                    dragTileGrid.IsSelected = true;

                    lastSelectTileGrid = dragTileGrid;

                    linkCount++;
                    dragTileGrid.Num = linkCount;
                    AddEraseTileGridlList(dragTileGrid);

                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Block_Choose);
                }
            }
            //Debug.Log(dragTileGrid.tileGridType);

            if (eraseTileGridList.Count > 1) {
                //Debug.Log(dragTileGrid.Num);

                // 削除リストのグリッドが最後に選択しているグリッドではなく、削除対象の番号と同じで選択済の場合(１つ手前に戻した場合)
                if (eraseTileGridList[linkCount - 1] != lastSelectTileGrid && eraseTileGridList[linkCount - 1].Num == dragTileGrid.Num && dragTileGrid.IsSelected) {

                    // 選択中のグリッドを取り除く 
                    RemoveEraseTileGridList(lastSelectTileGrid);

                    // 未選択に戻す
                    lastSelectTileGrid.GetComponent<TileGridDetail>().IsSelected = false;

                    // 最後のグリッドの情報を、前のグリッドの情報に戻す
                    lastSelectTileGrid = dragTileGrid;
                    linkCount--;

                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Cancel);
                }
            }
        }
    }

    /// <summary>
    /// グリッドのドラッグをやめた（指を画面から離した）際の処理
    /// </summary>
    private void OnEndDrag() {
        // 3つ以上グリッドが選択されている場合
        if (eraseTileGridList.Count >= 3) {
            // 削除対象として選択されている(リストに登録されている)グリッドを消す
            for (int i = 0; i < eraseTileGridList.Count; i++) {

                // リストから取り除く
                tileGridList.Remove(eraseTileGridList[i]);

                // エフェクトのプレファブが用意されている場合
                if (eraseEffectPrefab) {
                    //Debug.Log("エフェクト生成");
                    // エフェクト生成
                    GameObject effect = Instantiate(eraseEffectPrefab, eraseTileGridList[i].gameObject.transform);
                    effect.transform.SetParent(tileGridSetTran);
                    Destroy(effect, 0.6f);
                }

                // グリッドを削除
                Destroy(eraseTileGridList[i].gameObject);
            }

            // 消したグリッドの数の加算。フィーバー中は3 - 5倍
            TotalErasePoint.Value += IsFeverTime.Value ? eraseTileGridList.Count * (3 + currentStageData.stageNo) : eraseTileGridList.Count;

            // 消したグリッド数の最大値の更新確認
            if (eraseTileGridList.Count > currentAchievementStageData.maxLinkCount) currentAchievementStageData.maxLinkCount = eraseTileGridList.Count;

            // フィーバーしていない場合
            if (!IsFeverTime.Value) {
                // 最大値を超えないようにフィーバーポイント加算　消した数 - 2
                FeverPoint.Value = Mathf.Min(targetFeverPoint, FeverPoint.Value += CalculateFeverPoint(eraseTileGridList.Count));
                //Debug.Log(FeverPoint.Value);

                // フィーバーの確認
                if (CheckFeverTime()) {
                    // フィーバー回数加算
                    currentAchievementStageData.maxFeverCount++;

                    // フィーバータイム開始
                    ObserveFeverTimeAsync().Forget();

                    SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.フィーバー);
                } else {
                    // フィーバーでなければゲージ増加
                    sliderFever.DOValue(FeverPoint.Value, 0.25f).SetEase(Ease.InQuart).SetLink(sliderFever.gameObject);

                    // SE
                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Erase);
                }
            } else {
                // SE
                SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);
            }
        } else {
            // 削除候補のグリッドの選択を解除
            ReleaseTileGrids();

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
        }
        // 削除対象リストをクリア
        eraseTileGridList.Clear();

        // 初期化
        ResetSelectTileGrid();

        // 残りのグリッド数を確認。色を変えておらず、12個以下の場合には、ランダムな1色にする
        if (!isLastColor && tileGridList.Count < 12) {
            isLastColor = true;
            ChangeTileGridsColor();
        }

        // 残り2個以下ならすべて削除してクリア
        if (tileGridList.Count <= 2) {
            for (int i = 0; i < tileGridList.Count; i++) {
                Destroy(tileGridList[i].gameObject);
            }
            //Debug.Log("Game Clear");

            DestroyAllObstacles();
            tileGridList.Clear();
            gameState = GameState.GameUp;

            // クリアタイムの保持
            currentAchievementStageData.fastestClearTime = GameTime.Value;

            Result(true);
        }
    }

    /// <summary>
    /// フィーバーポイントの計算
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private int CalculateFeverPoint(int point) {
        return point - 2;
    }

    /// <summary>
    /// フィーバーの判定
    /// </summary>
    /// <returns></returns>
    private bool CheckFeverTime() {
        return IsFeverTime.Value = FeverPoint.Value >= targetFeverPoint ? true : false;
    }

    /// <summary>
    /// フィーバータイムの監視
    /// </summary>
    /// <returns></returns>
    private async UniTask ObserveFeverTimeAsync() {
        // 障害物の移動速度を低速にし、途中での速度アップもなしにする
        SlowDownAllObstacles();

        // 100％ のアニメ演出が終わるまで待機
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(500, cancellationToken: token);

        sliderFever.value = targetFeverPoint;       
        sliderFever.DOValue(0, (float)feverDuraiton / 1000).SetEase(Ease.Linear).SetLink(gameObject);
        
        // ゲージに合わせて、数字も 100% -> 0% にする
        FeverPoint.Value = 0;

        await UniTask.Delay(feverDuraiton, false, PlayerLoopTiming.Update, token);
        IsFeverTime.Value = false;
        //Debug.Log("フィーバータイム終了");

        // 障害物の移動速度を戻す
        RestartAllObstacles();
    }

    /// <summary>
    /// グリッドを生成
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private void CreateTileGrids() {
        for (int i = 0; i < rowCount; i++) {
            for (int j = 0; j < columnCount; j++) {
                // TileGridDetail プレファブを生成
                TileGridDetail tileGrid = Instantiate(tileGridPrefab, tileGridSetTran, true);

                // サイズ変更と位置変更して並べる
                tileGrid.transform.localScale = Vector3.one * tileGridSize;
                tileGrid.transform.localPosition = new(j * tileGridSize, i * tileGridSize, 0);

                // グリッドの初期設定。グリッドの色をランダムに１つ選択
                tileGrid.SetUpTileGridDetail(UnityEngine.Random.Range(0, (int)TileGridType.Count));

                tileGridList.Add(tileGrid);
            }
        }
    }

    /// <summary>
    /// つながったグリッドを削除リストに追加
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void AddEraseTileGridlList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Add(dragTileGrid);
        ChangeTileGridAlpha(dragTileGrid, 0.5f);
    }

    /// <summary>
    /// 前のグリッドに戻った際に削除リストから削除
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void RemoveEraseTileGridList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Remove(dragTileGrid);

        // 色も戻す
        ChangeTileGridAlpha(dragTileGrid, 1.0f);

        // 未選択の状態に戻す
        if (dragTileGrid.IsSelected) {
            dragTileGrid.IsSelected = false;
        }
    }

    /// <summary>
    /// グリッドのアルファを変更
    /// </summary>
    /// <param name="dragTileGrid"></param>
    /// <param name="alphaValue"></param>
    private void ChangeTileGridAlpha(TileGridDetail dragTileGrid, float alphaValue) {
        dragTileGrid.spriteTileGrid.color = new (dragTileGrid.spriteTileGrid.color.r, dragTileGrid.spriteTileGrid.color.g, dragTileGrid.spriteTileGrid.color.b, alphaValue);
        dragTileGrid.transform.DOShakeScale(0.15f).SetEase(Ease.InQuart).SetLink(dragTileGrid.gameObject).OnComplete(() => dragTileGrid.transform.localScale = Vector3.one * tileGridSize);
    }

    /// <summary>
    /// グリッドの色を１色に変える
    /// 残りが指定数以下になったときに利用して、ゲームが詰む状態をなくす
    /// </summary>
    private void ChangeTileGridsColor() {
        // 白以外にする
        int randomColorNo = UnityEngine.Random.Range(0, (int)TileGridType.白);
        for (int i = 0; i < tileGridList.Count; i++) {
            tileGridList[i].SetTileGridTile(randomColorNo);
            tileGridList[i].SetColor(randomColorNo);
        }

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);
    }

    /// <summary>
    /// 障害物に接触して削除に失敗した場合
    /// </summary>
    public void FailureErase() {
        // 重複判定防止(障害物側でも制御しているが念のため)
        if (gameState != GameState.Play) {
            return;
        }

        gameState = GameState.Ready;

        // SE
        SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Miss);

        lifeIconlist[0].transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);

        Destroy(lifeIconlist[0].gameObject, 0.5f);
        lifeIconlist.Remove(lifeIconlist[0]);

        // 画面点滅
        flushScreen.Flush();

        // ゲームオーバー判定
        if (lifeIconlist.Count <= 0) {

            gameState = GameState.GameUp;

            StopAllObstacles();
            //Debug.Log("Game Over");

            // 清算
            Result(false);

            return;
        }

        // 削除候補のグリッドの選択を解除
        ReleaseTileGrids();

        // 削除対象リストをクリア
        eraseTileGridList.Clear();

        // 選択したグリッドを初期化
        ResetSelectTileGrid();

        // すべての障害物の再移動開始
        RestartAllObstacles();

        gameState = GameState.Play;
    }

    /// <summary>
    /// 選択中のグリッドの選択解除
    /// </summary>
    private void ReleaseTileGrids() {
        for (int i = 0; i < eraseTileGridList.Count; i++) {
            // 選んだ数か2個以下の場合　各グリッドの選択状態を解除する
            eraseTileGridList[i].IsSelected = false;
            // 色も戻す
            ChangeTileGridAlpha(eraseTileGridList[i], 1.0f);
        }
    }

    /// <summary>
    /// 選択したグリッドを初期化
    /// </summary>
    private void ResetSelectTileGrid() {
        // 初期化
        firstSelectTileGrid = null;
        lastSelectTileGrid = null;
        currentTileGridType = null;
    }

    /// <summary>
    /// 障害物の生成
    /// </summary>
    /// <param name="createCount"></param>
    private void CreateObstacles(int createCount) {
        for (int i = 0; i < createCount; i++) {
            ObstacleBall obstacleBall = Instantiate(obstacleBallPrefab, GetObstaclePos(), Quaternion.identity);
            obstacleBall.SetUpObstacleBall(this, currentStageData.obstacleSpeeds);
            obstacleList.Add(obstacleBall);
        }
    }

    /// <summary>
    /// 障害物の生成する座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetObstaclePos() {
        return new(UnityEngine.Random.Range(obstacleBallTrans[0].position.x, obstacleBallTrans[1].position.x), 
            UnityEngine.Random.Range(obstacleBallTrans[0].position.y, obstacleBallTrans[1].position.y), 0);
    }

    /// <summary>
    /// すべての障害物を再度動かす
    /// </summary>
    private void RestartAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(false);
        }
    }

    /// <summary>
    /// すべての障害物の移動を停止
    /// </summary>
    private void StopAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].StopMoveBall();
        }
    }

    /// <summary>
    /// すべての障害物の移動を低速に
    /// </summary>
    private void SlowDownAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(true);
        }
    }

    /// <summary>
    /// すべての障害物の破壊
    /// </summary>
    private void DestroyAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            Destroy(obstacleList[i].gameObject);
        }
        obstacleList.Clear();
    }

    /// <summary>
    /// ライフアイコンの生成
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTaskVoid CreateLifeIconAsync(CancellationToken token) {
        for (int i = 0; i < lifeCount; i++) {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeSetTran, false);
            lifeIcon.transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);
            lifeIconlist.Add(lifeIcon);
            await UniTask.Delay(250, false, PlayerLoopTiming.Update, token);
        }
    }

    /// <summary>
    /// ゲーム内容の清算
    /// </summary>
    /// <param name="isClear"></param>
    private void Result(bool isClear) {
        // 今回の消したブロックのポイントを加算
        UserData.instance.MosaicCount.Value += TotalErasePoint.Value;

        // 最大値として一旦保持
        currentAchievementStageData.maxMosaicCount = TotalErasePoint.Value;

        // チャレンジ回数加算
        currentAchievementStageData.challengeCount++;

        // ゲームオーバーの場合
        if (!isClear) {
            // ゲームオーバー回数加算
            currentAchievementStageData.failureCount++;

            // 実績の更新確認
            UserData.instance.CheckUpdateAchievementStageData(currentAchievementStageData);

            GameOverAsync().Forget();            
            return;
        }

        // クリアしたステージが最終ステージではなくて、初クリアのステージの場合
        if (currentStageData.stageNo != 2 && !UserData.instance.clearStageNoList.Contains(currentStageData.stageNo + 1)) {
            // 次のステージを追加
            UserData.instance.AddClearStageNoList(currentStageData.stageNo + 1);
        }
        // クリア回数加算
        currentAchievementStageData.clearCount++;

        // ノーミスクリアの場合
        if (lifeIconlist.Count >= lifeCount) {
            currentAchievementStageData.noMissClearCount++;
        }
        // 実績の更新確認
        UserData.instance.CheckUpdateAchievementStageData(currentAchievementStageData);
        GameClearAsync().Forget();
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    /// <returns></returns>
    private async UniTask GameOverAsync() {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameOver);

        imgGameInfo.sprite = gameInfoLogos[0];
        imgGameInfo.DOFade(1.0f, 1.5f).SetEase(Ease.InQuart).SetLink(imgGameInfo.gameObject);

        // クリック導線
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);

        var token = this.GetCancellationTokenOnDestroy();

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.ゲームオーバー);

        // クリック待ち
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // シーン遷移
        await StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }

    /// <summary>
    /// ゲームクリア演出
    /// </summary>
    /// <returns></returns>
    private async UniTask GameClearAsync() {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameClear);

        imgGameInfo.sprite = gameInfoLogos[1];

        Sequence sequence = DOTween.Sequence();
        sequence.Append(imgGameInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
        sequence.AppendInterval(1.0f);
        sequence.Append(imgGameInfo.DOFade(0, 0.5f).SetEase(Ease.Linear));

        // クリック導線
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);

        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.クリア_1);

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        // クリック待ち
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.クリア_2);

        // ノーミスクリア
        if (lifeIconlist.Count >= lifeCount) {
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Excellent);

            imgExcellentLogo.DOFade(1.0f, 2.0f).SetEase(Ease.Linear);
            imgExcellentLogo.transform.DOLocalMoveX(0, 1.5f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);
            await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);

            // クリック待ち
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);

            imgExcellentLogo.DOFade(0f, 0.5f).SetEase(Ease.Linear);
            imgExcellentLogo.transform.DOLocalMoveX(-1250, 1.0f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);

            // レア画像のアニメ表示           
            normalChara.material.DOFloat(-1, "_Flip", 1.5f).SetEase(Ease.Linear).SetLink(normalChara.gameObject);
            await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);

            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.エクセレント);

            // クリック待ち
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);
        }
        // ボイスを最後まで流したいため
        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        // シーン遷移
        await StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }
}
