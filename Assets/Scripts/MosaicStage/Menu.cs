using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Text txtMozaicCount;

    [SerializeField]
    private CharaButtonDetail charaButtonPrefab;

    [SerializeField]
    private Transform[] charaButtonSetTrans;

    [SerializeField]
    private Sprite[] charaSprites;

    private List<CharaButtonDetail> charaButtonList = new();
    private int charaCount = 3;
    
    void Start()
    {
        SoundManager.Instance.PlayBGM(SoundManager.BGM_TYPE.Menu);

        // キャラボタンの生成
        CreateCharaButtons();

        // MozaicCount 購読
        UserData.instance.MosaicCount
            .Zip(UserData.instance.MosaicCount.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateDisplayMosaicCount(x.oldValue, x.newValue))
            .AddTo(gameObject);

        // 初期値表示更新
        UserData.instance.MosaicCount.SetValueAndForceNotify(UserData.instance.MosaicCount.Value);
    }

    /// <summary>
    /// MosaicCount の表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayMosaicCount(int oldValue, int newValue) {
        txtMozaicCount.DOCounter(oldValue, newValue, 0.5f).SetEase(Ease.InQuart).SetLink(gameObject);
    }

    /// <summary>
    /// キャラボタンの生成
    /// </summary>
    private void CreateCharaButtons() {
        for (int i = 0; i < charaCount; i++) {
            CharaButtonDetail charaButton = Instantiate(charaButtonPrefab, charaButtonSetTrans[i], false);
            charaButton.SetUpCharaButtonDetail(i, this, charaSprites[i]);
            charaButtonList.Add(charaButton);
        }
    }

    /// <summary>
    /// すべてのキャラボタンを非活性化
    /// </summary>
    public void InactiveAllCharaButtons() {
        for (int i = 0; i < charaButtonList.Count; i++) {
            charaButtonList[i].InactibeCharaButton();
        }
    }
}
