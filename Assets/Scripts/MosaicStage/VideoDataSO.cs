using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoDataSO", menuName = "Create VideoDataSO")]
public class VideoDataSO : ScriptableObject {
    public List<VideoData> videoDataList = new();
}