using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ムービー一覧用サムネイルボタン
/// </summary>
public class MovieDetailView : MonoBehaviour {
    [SerializeField] private Image imgMovieThumnail;
    [SerializeField] private Button btnMovieDetail;

    private VideoData videoData;
    public VideoData VideoData => videoData;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="videoData"></param>
    /// <param name="btnClickAction"></param>
    public void Setup(VideoData videoData, UnityAction<MovieDetailView, int> btnClickAction) {
        this.videoData = videoData;

        // アイコン画像設定
        if (videoData.thumbnail != null) {
            imgMovieThumnail.sprite = videoData.thumbnail;
        }

        // ボタン購読(プレイリストの空いているスロットにムービー追加)
        btnMovieDetail.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.25f))
            .Subscribe(_ => btnClickAction?.Invoke(this, -1))
            .AddTo(this);
    }
}