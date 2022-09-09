using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class CharaButtonDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField]
    private Image imgChara;

    //[SerializeField]
    private int stageNo;
    private Menu menu;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="stageNo"></param>
    /// <param name="menu"></param>
    public void SetUpCharaButtonDetail(int stageNo, Menu menu, Sprite charaSprite) {
        this.stageNo = stageNo;
        this.menu = menu;

        imgChara.sprite = charaSprite;

        btnChara.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => 
            {
                SelectStage.stageNo = stageNo;

                btnChara.transform.DOShakeScale(0.3f).SetEase(Ease.InQuart).SetLink(gameObject);

                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.Submit);

                // 他のボタンも押せない状態にする
                menu.InactiveAllCharaButtons();

                // シーン遷移とフェイドアウト処理
                StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
            })
            .AddTo(gameObject);          
    }

    /// <summary>
    /// ボタン非活性化
    /// </summary>
    public void InactibeCharaButton() {
        btnChara.interactable = false;
    }
}
