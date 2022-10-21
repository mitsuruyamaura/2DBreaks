using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameInfoView : MonoBehaviour
{
    [SerializeField]
    private Text[] txtGameTimes;

    [SerializeField]
    private Text txtMosaicCount;

    [SerializeField]
    private Slider sliderFever;

    [SerializeField]
    private Text txtValue;

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private Image imgCharaIcon;

    [SerializeField]
    private Image imgExcellentLogo;

    [SerializeField]
    private Image imgGameInfo;

    [SerializeField]
    private Sprite[] gameInfoLogos;


    /// <summary>
    /// ゲーム時間の表示更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateGameTime(float amount) {
        // 小数点以下は小さく表示
        string time = amount.ToString("F2");
        string[] part = time.Split('.');
        txtGameTimes[0].text = part[0] + ".";
        txtGameTimes[1].text = part[1];
    }

    /// <summary>
    /// 壊したグリッドの数の表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void UpdateMosaicCount(int oldValue, int newValue) {
        txtMosaicCount.DOCounter(oldValue, newValue, 0.5f).SetEase(Ease.Linear);
        // 左上のキャラアイコンをアニメ
        imgCharaIcon.transform.DOPunchScale(Vector3.one * 1.25f, 0.25f)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }


    public void UpdateFeverSlider(int feverPoint, float duration = 0.25f) {
        sliderFever.DOValue(feverPoint, duration).SetEase(Ease.InQuart).SetLink(gameObject);
    }


    /// <summary>
    /// Slider の上の % 表示の更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayValue(float oldValue, float newValue, int targetFeverPoint, int feverDuraiton) {
        if (newValue >= targetFeverPoint) {
            newValue = targetFeverPoint;
        }
        float before = oldValue / targetFeverPoint * 100;
        float after = newValue / targetFeverPoint * 100;

        //Debug.Log(before);
        //Debug.Log(after);

        int a = (int)before;
        int b = (int)after;

        //Debug.Log(a);
        //Debug.Log(b);
        // 数字のアニメ時間の設定。フィーバーした際には長くなる
        float duration = newValue == 0 ? (float)feverDuraiton / 1000 : 0.25f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(txtValue.DOCounter(a, b, duration).SetEase(Ease.Linear)).SetLink(txtValue.gameObject);

        // 満タンになったら
        if (newValue == targetFeverPoint) {
            // 100％ の数字を見せるアニメ演出を追加
            float scale = txtValue.transform.localScale.x;
            sequence.Append(
            txtValue.transform.DOPunchScale(txtValue.transform.localScale * 1.25f, 0.25f).SetEase(Ease.InQuart).SetLink(txtValue.gameObject)
                .OnComplete(() => txtValue.transform.localScale = Vector3.one * scale));
        }
    }


    public void HideGameUpInfo() => txtInfo.gameObject.SetActive(false);

    public void ShowGameUpInfo() {
        // クリック導線
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);
    }


    public void SetUpCharaIcon(Sprite charaIcon) {
        imgCharaIcon.sprite = charaIcon;

        imgCharaIcon.transform.DOShakeScale(0.75f, 1f, 4)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }


    public void ShowExcellentLogo() {

        imgExcellentLogo.DOFade(1.0f, 2.0f).SetEase(Ease.Linear);
        imgExcellentLogo.transform.DOLocalMoveX(0, 1.5f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);
    }


    public void HideExcellentLogo() {
        imgExcellentLogo.DOFade(0f, 0.5f).SetEase(Ease.Linear);
        imgExcellentLogo.transform.DOLocalMoveX(-1250, 1.0f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);
    }


    public void ShowGameClear() {
        imgGameInfo.sprite = gameInfoLogos[1];

        Sequence sequence = DOTween.Sequence();
        sequence.Append(imgGameInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
        sequence.AppendInterval(1.0f);
        sequence.Append(imgGameInfo.DOFade(0, 0.5f).SetEase(Ease.Linear));

        ShowGameUpInfo();
    }


    public void ShowGameOver() {
        imgGameInfo.sprite = gameInfoLogos[0];
        imgGameInfo.DOFade(1.0f, 1.5f).SetEase(Ease.InQuart).SetLink(imgGameInfo.gameObject);

        ShowGameUpInfo();
    }
}
