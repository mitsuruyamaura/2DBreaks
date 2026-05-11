using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class StageSelectIconView : MonoBehaviour {

    [SerializeField] private Button btnChara;
    [SerializeField] private Image imgChara;
    [SerializeField] private Text txtStageNo;
    [SerializeField] private int stageNo;
    [SerializeField] private StageType stageType;
    [SerializeField] private Image imgGlassShade;
    [SerializeField] private Image imgExcellentIcon;
    [SerializeField] private Image imgOneMissClearIcon;

    public void Setup(int stageNo, Sprite charaSprite, StageType stageType, UnityAction<int, StageType> btnAction) {
        this.stageNo = stageNo;
        this.stageType = stageType;
        imgChara.sprite = charaSprite;
        txtStageNo.text = "stage " + stageNo;

        // クリア済のステージの場合、ガラスシェードを外す
        StageClearData stageClearData = UserData.instance.GetStageClearData(stageType, stageNo);
        if (stageClearData != null) {
            imgGlassShade.enabled = false;

            // エクセレントの場合
            if (stageClearData.isNoMissClear) {
                imgExcellentIcon.enabled = true;
            } else if (stageClearData.isOneMissClear) {
                // ワンミスクリアの場合
                imgOneMissClearIcon.enabled = true;
            }
        }

        btnChara.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ => {
                AnimButton();
                btnAction.Invoke(stageNo, stageType);
            });
    }

    /// <summary>
    /// キャラボタン押下時の処理
    /// </summary>
    public void AnimButton() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnChara.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnChara.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }

    /// <summary>
    /// ボタン非活性化　←　不要
    /// </summary>
    public void InactibeCharaButton() {
        btnChara.interactable = false;
    }

    /// <summary>
    /// ボタンの取得
    /// </summary>
    /// <returns></returns>
    public Button GetButton() {
        return btnChara;
    }

    /// <summary>
    /// キャラボタンをロック
    /// </summary>
    public void LockCharaButton() {
        btnChara.enabled = false;  // ineractable だと Dsabled Color になるため
        imgChara.color = new(0, 0, 0, 0.6f);
    }
}