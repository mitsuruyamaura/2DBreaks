using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// 画面下部のプレイリスト一覧表示部分の管理クラス
/// 実際に並んでいるサムネイルは PlaylistDetailView になる
/// </summary>
public class PlaylistView : MonoBehaviour {
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Transform[] playlistTrans;  // 配置スロット

    public PlaylistDetailView[] playlistDetailViews;     // プレイリストに登録されているムービー。この順番に再生する
    public DropMovieArea[] dropMovieAreas;               // ドロップ先の配列

    public int assignedMovieCount = 0;                   // プレイリストに登録されているムービーの総数

    /// <summary>
    /// 初期設定
    /// </summary>
    public void Setup() {
        // ムービー登録可能な数だけ、空で作成しておく
        playlistDetailViews = new PlaylistDetailView[playlistTrans.Length];

        // ドロップ先の設定
        for (int i = 0; i < dropMovieAreas.Length; i++) {
            int index = i;
            dropMovieAreas[i].SetupDropMovieArea(index);
        }
    }

    /// <summary>
    /// 空いているスロットにムービーを追加
    /// </summary>
    /// <param name="playlistDetailView"></param>
    public void AddMovieThumnailEmptySlot(PlaylistDetailView playlistDetailView) {
        // 空いているスロットを1番目から探して順番に入れる
        for (int i = 0; i < playlistDetailViews.Length; i++) {
            if (playlistDetailViews[i] == null) {
                // 空いているスロットにムービーのサムネイル配置
                playlistDetailViews[i] = playlistDetailView;
                playlistDetailView.transform.SetParent(playlistTrans[i].transform);
                playlistDetailView.transform.localPosition = Vector3.zero;
                playlistDetailView.transform.localScale = Vector3.one * 0.9f;

                // 親のCanvas、配置した番号を設定
                playlistDetailView.DragThumnail.SetupDragThumnail(parentCanvas, dropMovieAreas[i].SlotIndex);

                // ドロップ時のイベント登録(古い Index と新しい Index をもらって配列内のムービーの順番の入れ替えに使う)
                playlistDetailView.DragThumnail.OnDropped.Subscribe(UpdateSlot);

                // スロットに配置したムービー情報を設定
                dropMovieAreas[i].SetChara(playlistDetailView.DragThumnail);
                assignedMovieCount++;
                break;
            }
        }
    }

    /// <summary>
    /// 指定されているスロットにムービー配置
    /// </summary>
    /// <param name="slotNo"></param>
    /// <param name="playlistDetailView"></param>
    public void AssignMovieThumnailToTargetSlot(int slotNo, PlaylistDetailView playlistDetailView) {
        // 指定されているスロットにムービー配置
        playlistDetailViews[slotNo] = playlistDetailView;
        playlistDetailView.transform.SetParent(playlistTrans[slotNo].transform);
        playlistDetailView.transform.localPosition = Vector3.zero;
        playlistDetailView.transform.localScale = Vector3.one * 0.9f;

        // 親のCanvas、配置した番号を設定
        playlistDetailView.DragThumnail.SetupDragThumnail(parentCanvas, dropMovieAreas[slotNo].SlotIndex);

        // ドロップ時のイベント登録(古い Index と新しい Index をもらって配列内のムービーの順番の入れ替えに使う)
        playlistDetailView.DragThumnail.OnDropped.Subscribe(UpdateSlot);

        // スロットに配置したムービー情報を設定
        dropMovieAreas[slotNo].SetChara(playlistDetailView.DragThumnail);
        assignedMovieCount++;
    }

    /// <summary>
    /// プレイリストからムービー削除
    /// </summary>
    /// <param name="slotNo"></param>
    public void LeaveMovieFromPlaylist(int slotNo) {
        // ムービーを削除したスロットを空ける
        playlistDetailViews[slotNo] = null;
        dropMovieAreas[slotNo].ReleaseThumnail();
        assignedMovieCount--;
    }

    /// <summary>
    /// ムービーを追加できるかどうか判定
    /// true なら追加可能
    /// </summary>
    /// <returns></returns>
    public bool CanAddMovie() {
        return assignedMovieCount < playlistDetailViews.Length;
    }

    /// <summary>
    /// playlistDetailViews 配列内のサムネイル入れ替え処理
    /// </summary>
    /// <param name="indexes"></param>
    private void UpdateSlot((int oldIndex, int newIndex) indexes) {
        // サムネイルの入れ替え処理
        (playlistDetailViews[indexes.newIndex], playlistDetailViews[indexes.oldIndex]) = (playlistDetailViews[indexes.oldIndex], playlistDetailViews[indexes.newIndex]);
    }

    /// <summary>
    /// プレイリストから VideoData のみを抽出してリストを作成
    /// 空のプレイリストは削除する
    /// </summary>
    /// <returns></returns>
    public List<VideoData> GetPlayListDataFromDetail() {
        return playlistDetailViews.Where(view => view?.VideoData != null).Select(view => view.VideoData).ToList();
    }
}