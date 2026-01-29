using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDifficultyDataSO", menuName = "Create StageDifficultyDataSO")]
public class StageDifficultyDataSO : ScriptableObject {
    public List<StageDifficultyData> stageDifficultyDataList = new();
}