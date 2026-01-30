using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPopup : MonoBehaviour {
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private StageSelectIconView stageSelectIconViewPrefab;

    [SerializeField]
    private Transform iconViewTran;

    [SerializeField]
    private Canvas canvas;

    private List<StageSelectIconView> iconViewList = new();
    private BoolReactiveProperty sharedGate = new(true);　　　//　BindToOnClick にて利用する


    public void Setup(List<yamap.StageData> stageDataList) {
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 3;

        // ボタンの購読
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        // ステージアイコンビューの生成と設定
        for (int i = 0; i < stageDataList.Count; i++) {
            StageSelectIconView stageSelectIconView = Instantiate(stageSelectIconViewPrefab, iconViewTran, false);            
            yamap.StageData stageData = stageDataList[i];
            stageSelectIconView.Setup(stageData.stageNo, stageData.normalCharaSprite, stageData.stageType, ChooseCharaAndStartStage);

            // 複数のボタンを BoolReactiveProperty を購読して、１つのボタンに連動して制御できる
            // 内部で AsyncReactiveCommand が自動生成される。sharedGate が true なので、それが false になると、すべてのボタンの interactable に false の処理が届く
            stageSelectIconView.GetButton()
                .BindToOnClick(sharedGate, _ => {
                    // 5秒間押せないボタン
                    return Observable.Timer(System.TimeSpan.FromSeconds(2)).AsUnitObservable();
                });

            iconViewList.Add(stageSelectIconView);
        }

        // ボタンは Subscribe していないので、ReactiveProperty の方を止める
        sharedGate.AddTo(gameObject);

        // ポップアップ表示
        OpenPopup();
    }

    /// <summary>
    /// ポップアップを表示する
    /// </summary>
    public void OpenPopup() {
        gameObject.SetActive(true);
        AnimePopup(1.0f);
    }

    /// <summary>
    /// ポップアップをアニメさせる
    /// </summary>
    /// <param name="alpha"></param>
    private void AnimePopup(float alpha) {
        canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;

                if (alpha == 0) {
                    gameObject.SetActive(false);
                }
            }).SetLink(gameObject);
    }

    /// <summary>
    /// キャラボタン押下時の処理
    /// </summary>
    public void ChooseCharaAndStartStage(int stageNo, StageType stageType) {
        SelectStage.stageNo = stageNo;
        SelectStage.stageType = stageType;

        // シーン遷移とフェイドアウト処理
        StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
        Destroy(gameObject);
    }
}