using UnityEngine;

public enum EffectType {
    Damage,
    Destroy_s,
    Destroy_l
}

[System.Serializable]
public class EffectData
{
    public int id;
    public EffectType effectType;
    public GameObject effectPrefab;
}