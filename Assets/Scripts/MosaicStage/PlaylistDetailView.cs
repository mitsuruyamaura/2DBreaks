using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// プレイリスト内に並ぶサムネイル用ビュー
/// </summary>
public class PlaylistDetailView : MonoBehaviour {
    [SerializeField] private Image imgThumnail;
    [SerializeField] private Button btnPlaylistDetail;
    [SerializeField] private CanvasGroup canvasGroup;

    private DragThumnail dragThumnail;
    public DragThumnail DragThumnail => dragThumnail;

    private VideoData videoData;
    public VideoData VideoData => videoData;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="btnClickAction">RemoveFromCurrnetTeam メソッド</param>
    public void SetupPlaylistDetailView(VideoData videoData, UnityAction<PlaylistDetailView> btnClickAction) {
        this.videoData = videoData;

        TryGetComponent(out dragThumnail);
        canvasGroup.blocksRaycasts = true;

        // アイコン画像設定
        imgThumnail.sprite = videoData.thumbnail;

        // ボタン購読(プレイリストからこのムービーを削除)
        if (btnClickAction != null) {
            btnPlaylistDetail.OnClickAsObservable().ThrottleFirst(System.TimeSpan.FromSeconds(0.5f)).Subscribe(_ => btnClickAction?.Invoke(this));
        }
    }

    public void ActivateView() {
        dragThumnail.enabled = false;
        btnPlaylistDetail.enabled = true;
    }

    public void InactivateView() {
        if(TryGetComponent(out dragThumnail)) {
            dragThumnail.enabled = false;
        }

        btnPlaylistDetail.enabled = false;
        canvasGroup.blocksRaycasts = false;
    }
}