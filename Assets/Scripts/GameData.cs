using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public CharaData charaData;

    public int currentHp;

    public UIManager uiManager;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        Initialize();
    }

    public void Initialize() {
        currentHp = charaData.hp;
    }
}
