using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(CanvasGroup))]
public class VideoClipManager : MonoBehaviour {
    public static VideoClipManager instance;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup canvasGroup;
    //[SerializeField] private AspectRatioFitter aspectRatioFitter;

    public VideoClip clip;

    public bool IsVideoPlaying => videoPlayer.isPlaying;

    private float fadeDuration = 1.0f;

    [SerializeField] private Button btnPlayTest;

    [SerializeField] private int movieNo;

    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;
    [SerializeField] float barHeight = 950f;
    [SerializeField] float barDefaultHeight = 1100f;
    [SerializeField] float duration = 0.4f;



    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        Initialize();

        var token = this.GetCancellationTokenOnDestroy();
        btnPlayTest.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => PlayVideoAsync(movieNo, token).Forget())
            .AddTo(this);
    }

    /// <summary>
    /// Video 処理の初期化
    /// </summary>
    private void Initialize() {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        videoPlayer.clip = null;

        if (videoPlayer.targetTexture != null) {
            videoPlayer.targetTexture.Release();
        }
    }

    /// <summary>
    /// VideoClip の準備
    /// </summary>
    public void PrepareVideoClip(int setVideoNo, VideoClip sourceVideoClip = null) {

        if (videoPlayer.clip == null) {
            videoPlayer.clip = sourceVideoClip != null
                ? sourceVideoClip
                : UserData.instance.GetVideoData(setVideoNo);
        }

        //videoPlayer.prepareCompleted += OnCompletePrepare;

        // Aspect Ratio Fitter 使う場合
        //videoPlayer.prepareCompleted += vp =>
        //{
        //    var tex = vp.texture;
        //    aspectRatioFitter.aspectRatio = (float)tex.width / tex.height;
        //    Debug.Log("VideoClip ロード完了");
        //};

        videoPlayer.Prepare();

        Debug.Log("VideoClip ロード開始");

        // 使わない
        void OnCompletePrepare(VideoPlayer vp) {
            videoPlayer.prepareCompleted -= OnCompletePrepare;
            Debug.Log("VideoClip ロード完了");

            //PlayVideoAsync().Forget();
        }
    }

    /// <summary>
    /// VideoClip の再生
    /// </summary>
    private async UniTask PlayVideoAsync() {

        canvasGroup.blocksRaycasts = true;

        // フェードイン(DOTweenは await 非対応なのでそのまま)
        await canvasGroup.DOFade(1.0f, fadeDuration).AsyncWaitForCompletion();

        videoPlayer.Play();
        Debug.Log("VideoClip 再生");

        // 再生終了 or スキップ待ち
        while (videoPlayer.isPlaying) {

            if (Input.GetMouseButtonDown(0)) {
                SkipVideo();
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        StopVideo();
    }


    public async UniTask PlayVideoAsync(int videoNo, CancellationToken token, VideoClip sourceClip = null) {
        PrepareVideoClip(videoNo, sourceClip);

        // 再生開始を待つ(Prepare → Play)
        await UniTask.WaitUntil(
            () => videoPlayer.isPrepared,
            cancellationToken: token
        );

        canvasGroup.blocksRaycasts = true;

        // 必ずメインスレッドに戻す
        await UniTask.SwitchToMainThread(token);

        // シネマスコープ演出スライドイン
        ShowCinematicBarsAsync().Forget();

        // 上記を同時にフェードイン
        await canvasGroup
            .DOFade(1.0f, fadeDuration)
            .AsyncWaitForCompletion();

        videoPlayer.Play();
        Debug.Log("VideoClip 再生");

        try {
            // 再生終了 or スキップ
            await UniTask.WaitUntil(
                () => !videoPlayer.isPlaying,
                cancellationToken: token
            );
        } finally {
            StopVideo();

            DOTween.Kill(topBar);
            DOTween.Kill(bottomBar);
        }

        await HideCinematicBarsAsync();
    }

    /// <summary>
    /// VideoClip のスキップ
    /// </summary>
    public void SkipVideo() {
        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
            Debug.Log("VideoClip スキップ");
        }
    }

    /// <summary>
    /// VideoClip の停止
    /// </summary>
    public void StopVideo() {

        videoPlayer.Stop();

        canvasGroup.DOFade(0, fadeDuration)
            .OnComplete(() => {
                Initialize();
            });

        Debug.Log("VideoClip 停止");
    }

    public async UniTask ShowCinematicBarsAsync() {
        // 念のため初期位置
        //topBar.anchoredPosition = new Vector2(0, barHeight);
        //bottomBar.anchoredPosition = new Vector2(0, -barHeight);

        topBar.DOAnchorPosY(barHeight, duration).SetEase(Ease.OutCubic);
        bottomBar.DOAnchorPosY(-barHeight, duration).SetEase(Ease.OutCubic);

        await UniTask.Delay((int)(duration * 1000));
    }

    public async UniTask HideCinematicBarsAsync() {
        topBar.DOAnchorPosY(barDefaultHeight, duration).SetEase(Ease.InCubic);
        bottomBar.DOAnchorPosY(-barDefaultHeight, duration).SetEase(Ease.InCubic);

        await UniTask.Delay((int)(duration * 1000));
    }
}