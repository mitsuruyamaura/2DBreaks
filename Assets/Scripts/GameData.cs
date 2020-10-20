using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int charaBallHp;

    [HideInInspector]
    public CharaData charaData;

    [HideInInspector]
    public int money;

    [HideInInspector]
    public int battleTime;

    [HideInInspector]
    public int chooseStageNo;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// お金を増減する
    /// </summary>
    public void ProcMoney(int amount) {
        money += amount;
    }
}
