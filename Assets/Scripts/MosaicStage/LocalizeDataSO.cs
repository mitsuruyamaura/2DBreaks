using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeDataSO", menuName = "Create LocalizeDataSO")]
public class LocalizeDataSO : ScriptableObject {
    public List<LocalizeData> localizeDataList = new();
}