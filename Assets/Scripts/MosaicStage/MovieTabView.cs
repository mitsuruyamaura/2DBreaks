using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ムービーポップアップ内のタブに対応したコンテナ
/// </summary>
public class MovieTabView : MonoBehaviour {
    public int tabIndex;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform movieThumnailTran;

    public List<MovieDetailView> Setup(int tabIndex, List<StageClearData> stageClearDataList, MovieDetailView movieDetailViewPrefab, UnityAction<MovieDetailView, int> thumnailAction) {
        this.tabIndex = tabIndex;
        List<MovieDetailView> movieThumnailList = CreateMovieThumnails(stageClearDataList, movieDetailViewPrefab, thumnailAction);
        return movieThumnailList;
    }

    /// <summary>
    /// ムービー用サムネイルボタン生成
    /// </summary>
    public List<MovieDetailView> CreateMovieThumnails(List<StageClearData> stageClearDataList, MovieDetailView movieDetailViewPrefab, UnityAction<MovieDetailView, int> thumnailAction) {
        List<MovieDetailView> movieThumnailList = new();

        for (int i = 0; i < stageClearDataList.Count; i++) {
            StageClearData stageClearData = stageClearDataList[i];

            // ノーミスクリアしていない場合にはムービーは一覧に出さない
            if (!stageClearData.isNoMissClear) {
                continue;
            }

            yamap.StageData stageData = UserData.instance.GetStageData(stageClearData.stageType, stageClearData.stageNo);

            // サムネイル
            MovieDetailView movieThumnail = Instantiate(movieDetailViewPrefab, movieThumnailTran, false);
            VideoData videoData = UserData.instance.GetVideoData(stageData.videoId);
            movieThumnail.Setup(videoData, thumnailAction);
            movieThumnailList.Add(movieThumnail);
        }

        return movieThumnailList;
    }

    /// <summary>
    /// 動画オンリーモード用サムネイルボタン生成
    /// </summary>
    /// <param name="videoDataList"></param>
    /// <param name="movieDetailViewPrefab"></param>
    /// <param name="thumnailAction"></param>
    /// <returns></returns>
    public List<MovieDetailView> CreateMovieOnlyThumnails(VideoDataSO videoDataList, MovieDetailView movieDetailViewPrefab, UnityAction<MovieDetailView, int> thumnailAction) {
        List<MovieDetailView> movieThumnailList = new();

        for (int i = 0; i < videoDataList.videoDataList.Count; i++) {
            // サムネイル
            MovieDetailView movieThumnail = Instantiate(movieDetailViewPrefab, movieThumnailTran, false);
            Debug.Log($"videoId: {videoDataList.videoDataList[i].videoId}");
            VideoData videoData = UserData.instance.GetMovieOnlyVideoData(videoDataList.videoDataList[i].videoId);
            movieThumnail.Setup(videoData, thumnailAction);
            movieThumnailList.Add(movieThumnail);
        }

        return movieThumnailList;
    }

    public void ShowTabView() {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideTabView() {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}