using UnityEngine;

[System.Serializable]
public class UnitData
{
    public int id;
    public int hp;
    public int attackPower;
    public float moveSpeed;
    public float moveInterval;
    public float mass;
    public float scale;

    public Sprite spriteUnit;
    public EffectType effectType;
}