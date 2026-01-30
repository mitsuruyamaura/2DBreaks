using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharaButtonDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField]
    private Image imgChara;

    [SerializeField]
    private Text txtStageOpenPoint;

    [SerializeField]   //　インスペクターでの Debug 用。確認が済んだら SerializeField 属性の付与を外す
    private int stageNo;

    [SerializeField]
    private StageType stageType;

    [SerializeField]
    private HoverButton hoverButton;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="stageNo"></param>
    /// <param name="menu"></param>
    public void SetUpCharaButtonDetail(int stageNo, Sprite charaSprite, StageType stageType, UnityAction<StageType> btnAction, bool isOpenStage, int openPoint) {
        this.stageNo = stageNo;
        this.stageType = stageType;
        imgChara.sprite = charaSprite;
        
        if (isOpenStage) {
            txtStageOpenPoint.text = string.Empty;
            btnChara.OnClickAsObservable()
                        .ThrottleFirst(System.TimeSpan.FromSeconds(1.5f))
                        .Subscribe(_ => {
                            btnAction.Invoke(stageType);
                        });
        } else {
            DisplayStageOpenPoint(openPoint);
            hoverButton.enabled = false;
        }
    }

    /// <summary>
    /// マウスをホバーしたときの処理
    /// </summary>
    private void ResponseHoverButton() {
        if (!btnChara.enabled) {
            return;
        }
        transform.DOShakeScale(0.25f, 0.5f, 4).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => transform.localScale = Vector3.one);
    }

    /// <summary>
    /// ステージ開放に必要なポイント表示
    /// </summary>
    /// <param name="openPoint"></param>
    public void DisplayStageOpenPoint(int openPoint) {
        txtStageOpenPoint.text = $"Unlocked at {openPoint} pt.";
    }

    /// <summary>
    /// キャラボタン押下時の処理
    /// 現在未使用
    /// </summary>
    public void OnClickCharaButton() {
        SelectStage.stageNo = stageNo;
        SelectStage.stageType = stageType;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnChara.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnChara.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject);
        //btnChara.transform.DOShakeScale(0.3f).SetEase(Ease.InQuart).SetLink(gameObject);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // シーン遷移とフェイドアウト処理
        StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
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