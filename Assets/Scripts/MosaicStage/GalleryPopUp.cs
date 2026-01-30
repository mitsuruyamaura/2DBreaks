using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GalleryPopUp : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button btnClose;
    [SerializeField] private Transform zoomTran;
    [SerializeField] private GalleryTabView[] galleryTabViews;
    [SerializeField] private Toggle[] toggleTabViews;

    private List<GalleryIconDetail> galleryIconList = new();
    private BoolReactiveProperty sharedGate = new(true);　　　//　BindToOnClick にて利用する


    public void SetUp() {
        GalleryZoomViewer.instance.SetOverlayRoot(zoomTran);

        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        // ギャラリー用キャラアイコンのボタン生成
        CreateGalleryIcons();

        // タブ用トグルの設定
        for (int i = 0; i < toggleTabViews.Length; i++) {
            int index = i;

            Toggle toggle = toggleTabViews[index];
            GalleryTabView galleryTabView = galleryTabViews[index];

            toggle.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe(isOn => {
                    if (isOn) {
                        galleryTabView.ShowTabView();
                    } else {
                        galleryTabView.HideTabView();
                    }
                })
                .AddTo(gameObject);
        }

        canvasGroup.alpha = 0;

        // ポップアップ表示
        OpenPopup();
    }

    /// <summary>
    /// ギャラリー用キャラアイコンのボタン生成
    /// クリアしたステージの分だけ並べる
    /// </summary>
    private void CreateGalleryIcons() {
        int index = 0;

        // GalleryTabView の設定
        for (int i = 0; i < galleryTabViews.Length; i++) {
            // 難易度ごとのステージデータのリスト作成
            //List<yamap.StageData> stageDataList = UserData.instance.GetStageDataListByStageType((StageType)index);

            // クリアしているステージデータのリスト作成
            List<StageClearData> stageClearDataList = UserData.instance.GetStageClearDataListByStageType((StageType)index);

            List<GalleryIconDetail> galleryIconDetailList = galleryTabViews[i].Setup(index, stageClearDataList);
            galleryIconList.AddRange(galleryIconDetailList);
            index++;
        }

        // すべてのアイコンに同じ処理を施す
        for (int i = 0; i < galleryIconList.Count; i++) {
            GalleryIconDetail galleryIcon = galleryIconList[i];
            galleryIcon.SetZoomInPosition(zoomTran.position);

            galleryIcon.GetButton().BindToOnClick(sharedGate, _ => {
                // ズーム中ではないなら
                if (!galleryIcon.IsZoomIn) {
                    // ズームイン(クローン作成)
                    GalleryZoomViewer.instance.Show(galleryIcon);
                }
                // ズームアウトはクローン時に登録した処理で行う(GalleryIconDetail の SetZoomOutBtnByClone)

                // 1秒間押せないボタン
                return Observable.Timer(System.TimeSpan.FromSeconds(1.5f)).AsUnitObservable();
            });
        }

        // 購読削除設定
        sharedGate.AddTo(gameObject);
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
        canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear)  // .SetLoops(-1, LoopType.Yoyo)
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;

                if (alpha == 0) {
                    gameObject.SetActive(false);
                }
            }).SetLink(gameObject);
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);

        sequence.Append(btnClose.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart));
        sequence.Append(btnClose.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).OnComplete(() => AnimePopup(0f));

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
    }
}