using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public CharaData charaData;

    public int money;

    public int battleTime;

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
