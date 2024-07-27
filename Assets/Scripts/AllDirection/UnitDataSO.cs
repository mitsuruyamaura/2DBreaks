using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "Create UnitDataSO")]
public class UnitDataSO : ScriptableObject
{
    public List<UnitData> unitDataList = new();
}