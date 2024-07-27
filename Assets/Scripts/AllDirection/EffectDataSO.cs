using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDataSO", menuName = "Create EffectDataSO")]
public class EffectDataSO : ScriptableObject
{
    public List<EffectData> effectDataList = new();
}