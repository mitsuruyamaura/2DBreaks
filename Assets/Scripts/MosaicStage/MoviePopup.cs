using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MoviePopup : MonoBehaviour {
    [SerializeField] private MovieDetailView movieDetailViewPrefab;        // ムービー一覧サムネイル用のプレハブ

    [SerializeField] private PlaylistView playlistView;                    // プレイリスト
    [SerializeField] private PlaylistDetailView playlistDetailViewPrefab;  // プレイリスト内のサムネイル用のプレハブ
    [SerializeField] private VideoPlaylistPlayer playlistPlayer;

    [SerializeField] private MovieTabView[] movieTabViews;

    [SerializeField] private Toggle[] toggleTabViews;
    [SerializeField] private Button btnAutoPlay;
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnSurvey;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup canvasGroupMoviePanel;        // ムービー再生中の画面を隠す背景用

    [SerializeField] private string surveyUrl;                         // Google フォームの URL

    [SerializeField] private PlaylistDetailView specialMovieView;      // おまけムービー。全ステージクリアで開放
    [SerializeField] private Image specialMovieViewGlassShade;         // おまけムービー用グラスシェード
    [SerializeField] private int specialMovieId = 30;                  // おまけムービーの ID

    private List<MovieDetailView> movieThumnailList = new();           // ムービー一覧に並んでいるサムネイルのリスト。全タブのサムネイルが入っている


    public void Setup(bool isMovieOnly) {
        var token = this.GetCancellationTokenOnDestroy();

        // 各ボタン設定
        if (isMovieOnly) {
            btnCancel.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(2))
                .Subscribe(_ => ExitPoint.instance.OnClickOpenExitPopup())
                .AddTo(gameObject);
        } else {
            btnCancel.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(2))
                .Subscribe(_ => ClosePopup())
                .AddTo(gameObject);
        }

        // タップ待ちの再生は一旦使わない
        //btnPlay.OnClickAsObservable()
        //    .ThrottleFirst(System.TimeSpan.FromSeconds(2))
        //    .Subscribe(_ => ProcPlaylist(token, isWaitTapBetweenVideos: true))
        //    .AddTo(gameObject);

        btnAutoPlay.OnClickAsObservable()
            .Where(_ => playlistView.playlistDetailViews.Length > 0)
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => ProcPlayMovies(token, isWaitTapBetweenVideos: false))
            .AddTo(gameObject);

        btnSurvey.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => OpenGoogleForm())
            .AddTo(gameObject);

        //if (!isMovieOnly) {
            // ムービー一覧サムネイル作成
            CreateMovieThumnails();
        //}

        // 動画オンリーのムービー一覧サムネイル作成
        CreateMovieOnlyThumnails();

        // プレイリスト用のサムネイル置き場の設定
        playlistView.Setup();

        // タブ用トグルの設定
        for (int i = 0; i < toggleTabViews.Length; i++) {
            int index = i;

            Toggle toggle = toggleTabViews[index];
            MovieTabView movieTabView = movieTabViews[index];

            toggle.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe(isOn => {
                    if (isOn) {
                        movieTabView.ShowTabView();
                    } else {
                        movieTabView.HideTabView();
                    }
                })
                .AddTo(gameObject);
        }

        //if (!isMovieOnly) {
            // おまけムービーの開放チェック
            CheckUnlockedSpecialMovie();
        //}

        canvasGroup.alpha = 0;

        // ポップアップ表示
        OpenPopup();
    }

    /// <summary>
    /// プレイリスト準備
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isWaitTapBetweenVideos"></param>
    private void ProcPlayMovies(CancellationToken token, bool isWaitTapBetweenVideos) {
        List<VideoData> playList = playlistView.GetPlayListDataFromDetail();
        if (playList.Count == 0) {
            return;
        }
        PlayMoviesAsync(playList, token, isWaitTapBetweenVideos).Forget();
    }

    /// <summary>
    /// プレイリスト再生
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isWaitTapBetweenVideos"></param>
    /// <returns></returns>
    private async UniTask PlayMoviesAsync(List<VideoData> playList, CancellationToken token, bool isWaitTapBetweenVideos) {
        // 背景を白くする。 BGM のボリュームを 0 にする(Stop すると再生しなくなるので、0 ボリュームで対応する)
        canvasGroupMoviePanel.DOFade(1.0f, 0.25f).SetEase(Ease.Linear).SetLink(gameObject);
        canvasGroupMoviePanel.blocksRaycasts = true;

        float volume = SoundManager.instance.masterVolume;
        SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, 0);

        // プレイリストに登録したムービーを順番に再生
        await playlistPlayer.PlayAsync(playList, token, isWaitTapBetweenVideos);

        // 背景と BGM のボリュームを元に戻す
        canvasGroupMoviePanel.DOFade(0f, 0.25f).SetEase(Ease.Linear).SetLink(gameObject);
        canvasGroupMoviePanel.blocksRaycasts = false;
        
        SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, volume);
    }

    /// <summary>
    /// 各タブ内ごとに一覧を作成
    /// </summary>
    public void CreateMovieThumnails() {
        // MovieTabView の設定
        for (int i = 0; i < movieTabViews.Length; i++) {
            int index = i;

            // クリアしているステージデータのリスト作成
            if (i != 3) {
                // ムービー選択用のサムネイル作成
                List<StageClearData> stageClearDataList = UserData.instance.GetStageClearDataListByStageType((StageType)index);
                List<MovieDetailView> movieDetailList = movieTabViews[index].Setup(index, stageClearDataList, movieDetailViewPrefab, AddPlaylist);

                // 現状特に利用していないが、一応管理下に置く
                movieThumnailList.AddRange(movieDetailList);
            } else {
                // スペシャルの場合
                // インターフェースで分岐なしを検討

            }
        }
    }


    public void CreateMovieOnlyThumnails() {
        for (int i = 0; i < UserData.instance.movieOnlyVideoDataSOList.Count; i++) {
            // ムービー選択用のサムネイル作成
            List<MovieDetailView> movieDetailList = movieTabViews[i].CreateMovieOnlyThumnails(UserData.instance.movieOnlyVideoDataSOList[i], movieDetailViewPrefab, AddPlaylist);

            // 現状特に利用していないが、一応管理下に置く
            movieThumnailList.AddRange(movieDetailList);
        }
    }

    /// <summary>
    /// ムービー一覧に並んでいるサムネイル(MovieDetailView)をタップした際の処理
    /// プレイリストに追加
    /// </summary>
    /// <param name="charaId"></param>
    /// <param name="movieDetailView"></param>
    /// <param name="isOwner"></param>
    public virtual void AddPlaylist(MovieDetailView movieDetailView, int slotIndex = -1) {
        // プレイリストに追加できるかチェック
        bool canAssignChara = playlistView.CanAddMovie();

        // 一杯なら処理しない
        if (!canAssignChara) {
            return;
        }

        // プレイリスト一覧内にムービーのサムネイル生成
        PlaylistDetailView playlistDetailView = Instantiate(playlistDetailViewPrefab, transform, false);
        playlistDetailView.SetupPlaylistDetailView(movieDetailView.VideoData, RemoveFromPlaylist);

        // スロット番号の指定がない場合
        if (slotIndex == -1) {
            // プレイリストの空いているスロットに追加
            playlistView.AddMovieThumnailEmptySlot(playlistDetailView);
        } else {
            // プレイリストの指定されたスロットに追加
            playlistView.AssignMovieThumnailToTargetSlot(slotIndex, playlistDetailView);
        }

        // プレイリストにデータ追加(これだと詰められてしまうので一旦使わない。PlaylistView の配列を使う)
        //playlistPlayer.AddVideoData(movieDetailView.VideoData);    
    }

    /// <summary>
    /// 画面上部の編成中のサムネイル(PlaylistDetailView)をタップした際の処理
    /// プレイリストから選択されたサムネイルのムービーを削除
    /// </summary>
    /// <param name="playlistDetailView">画面上部の編成内のキャラアイコン</param>
    public virtual void RemoveFromPlaylist(PlaylistDetailView playlistDetailView) {
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);

        // プレイリストから選択されたサムネイルのムービーを削除
        playlistView.LeaveMovieFromPlaylist(playlistDetailView.DragThumnail.SlotIndex);

        Destroy(playlistDetailView.gameObject);
    }

    private void OpenGoogleForm() {
        Application.OpenURL(surveyUrl);
    }


    private void CheckUnlockedSpecialMovie() {
        bool isAllStageCleared = UserData.instance.CheckAllClearStage();

        // 全ステージクリア済ならおまけムービーを見れる状態にする
        if (isAllStageCleared && !UserData.instance.isDebugClearBtns) {
            VideoData videoData = UserData.instance.GetVideoData(specialMovieId);
            specialMovieView.SetupPlaylistDetailView(videoData, ProcPlaySpecialMovie);
            specialMovieView.ActivateView();
            specialMovieViewGlassShade.gameObject.SetActive(false);
        } else {
            // ホバー、ボタン無効化、グラスシェードかける、アンロック文字も出す
            specialMovieView.InactivateView();
            specialMovieViewGlassShade.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// おまけムービー再生準備
    /// </summary>
    /// <param name="playlistDetailView"></param>
    private void ProcPlaySpecialMovie(PlaylistDetailView playlistDetailView) {
        List<VideoData> playList = new() {
            playlistDetailView.VideoData
        };

        var token = this.GetCancellationTokenOnDestroy();

        PlayMoviesAsync(playList, token, isWaitTapBetweenVideos: false).Forget();
    }

    /// <summary>
    /// ポップアップを表示する
    /// </summary>
    public void OpenPopup() {
        gameObject.SetActive(true);
        AnimePopup(1.0f);
    }

    /// <summary>
    /// ポップアップをアニメさせる
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
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        UserData.instance.SetSaveData();  // 設定を保存

        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnCancel.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnCancel.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
            .OnComplete(() => Destroy(gameObject));

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
    }
}