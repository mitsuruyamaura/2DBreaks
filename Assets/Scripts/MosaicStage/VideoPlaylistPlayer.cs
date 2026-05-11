using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイリストの再生マネージャー
/// </summary>
public class VideoPlaylistPlayer : MonoBehaviour {
    //private List<VideoData> playList = new();
    //private readonly List<int> playList = new();

    //private const int MaxCount = 12;

    //public void Clear() => playList.Clear();

    //public bool AddVideo(int videoNo) {
    //    if (playList.Count >= MaxCount) return false;
    //    playList.Add(videoNo);
    //    return true;
    //}

    //public bool AddVideoData(VideoData videoData) {
    //    if (playList.Count >= MaxCount) return false;
    //    playList.Add(videoData);
    //    return true;
    //}

    

    /// <summary>
    /// プレイリスト再生
    /// </summary>
    /// <param name="token"></param>
    /// <param name="isWaitTapBetweenVideos"></param>
    /// <returns></returns>
    public async UniTask PlayAsync(List<VideoData> playList, CancellationToken token, bool isWaitTapBetweenVideos) {
        foreach (var videoData in playList) {
            await VideoClipManager.instance.PlayVideoAsync(videoData.videoId, token, videoData.videoClip);

            if (isWaitTapBetweenVideos) {
                // タップするまで待機
                await WaitTapAsync(token);
            } else {
                // 再生終了まで自動で待機
                await UniTask.WaitUntil(
                    () => VideoClipManager.instance.VideoPlayer.frame >= (long)VideoClipManager.instance.VideoPlayer.frameCount - 1,
                    cancellationToken: token
                );
            }
        }
    }

    /// <summary>
    /// 自動再生ではない場合にはクリック待ち
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask WaitTapAsync(CancellationToken token) {
        await UniTask.WaitUntil(
            () => //Input.GetMouseButtonDown(0),
             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame), // インプットシステム対応のタップ・クリック検知
            cancellationToken: token
        );
    }
}