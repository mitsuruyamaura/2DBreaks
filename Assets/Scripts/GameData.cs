using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int charaBallHp;

    [Header("バトル時間の設定値")]
    public int battleTime;

    [Header("Money総数")]
    public int totalMoney;

    // 未

    [HideInInspector]
    public CharaData charaData;   

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
    /// Moneyの総額を増減する
    /// </summary>
    public void ProcMoney(int amount) {
        totalMoney += amount;
    }
}
