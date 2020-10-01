using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public GameManager gameManager;

    public List<CharaData> charaDataList = new List<CharaData>();
    public List<BattleStageData> battleStageDataList = new List<BattleStageData>();
    public List<ItemData> itemDataList = new List<ItemData>();
    public List<TreasureData> treasureDataList = new List<TreasureData>();

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemData(int itemNo) {
        return itemDataList[itemNo];
    }

    
}
