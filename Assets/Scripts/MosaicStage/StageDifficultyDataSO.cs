using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDifficultyDataSO", menuName = "Create StageDifficultyDataSO")]
public class StageDifficultyDataSO : ScriptableObject {
    public Sprite imgBackGround;
    public List<StageDifficultyData> stageDifficultyDataList = new();
}