using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtGameTime;

    [SerializeField]
    private Image imgHp;

    private int initHp;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        // 初期値を保持
        initHp = GameData.instance.charaData.hp;

        // ゲーム時間の表示を更新
        UpdateDisplayGameTime();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO カウントはGameManagerで行う
        timer += Time.deltaTime;
        if (timer >= 1) {
            timer = 0;
            GameData.instance.currentHp--;

            if (GameData.instance.currentHp <= 0) {
                GameData.instance.currentHp = 0;
                // TODO GameUp

            } 
        }
        // ゲーム時間の表示を更新
        UpdateDisplayGameTime();
    }

    /// <summary>
    /// ゲーム時間の表示を更新
    /// </summary>
    public void UpdateDisplayGameTime() {
        float value = (float)GameData.instance.currentHp / initHp;
        imgHp.DOFillAmount(value, 1.0f).SetEase(Ease.Linear);
        txtGameTime.text = GameData.instance.currentHp.ToString();
    }
}
