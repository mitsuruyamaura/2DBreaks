using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ギャラリー用のアイコンビュー
/// </summary>
public class GalleryIconDetail : MonoBehaviour {

    [SerializeField] private Image imgGalleryChara;
    [SerializeField] private Image imgFrame;
    [SerializeField] private Button btnGalleryIcon;

    private bool isZoomIn;
    public bool IsZoomIn { get => isZoomIn; }

    private Vector3 startPos;
    private Vector3 zoomInPos;
    private HoverButton hoverButton;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="charaSprite"></param>
    /// <param name="frameSprite">null で来るのはズーム用のクローン想定</param>
    public void SetUp(Sprite charaSprite, Sprite frameSprite) {
        startPos = ((RectTransform)transform).anchoredPosition;

        imgGalleryChara.sprite = charaSprite;

        if (frameSprite != null) {
            imgFrame.sprite = frameSprite;
        } else {
            // null で来た場合にはフレーム出さない
            imgFrame.enabled = false;
        }
        
        TryGetComponent(out hoverButton);
    }

    /// <summary>
    /// ズーム用の座標設定
    /// </summary>
    /// <param name="zoomInPos"></param>
    public void SetZoomInPosition(Vector3 zoomInPos) {
        this.zoomInPos = zoomInPos;
    }

    public Button GetButton() {
        return btnGalleryIcon;
    }

    public Sprite GetCharaSprite() => imgGalleryChara.sprite;
    public Sprite GetFrameSprite() => imgFrame.sprite;

    /// <summary>
    /// ホバー無効
    /// </summary>
    public void InactivateHoverButton() => hoverButton.enabled = false;

    /// <summary>
    /// クローン用のアイコンを破棄する処理を登録
    /// </summary>
    public void SetZoomOutBtnByClone() {
        btnGalleryIcon.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => GalleryZoomViewer.instance.Hide())
            .AddTo(gameObject);
    }

    /// <summary>
    /// アイコンズームイン(正確にはクローンしたアイコンで使っている)
    /// </summary>
    public void ZoomInGalleryIcon() {
        isZoomIn = true;
        // 画像の優先順位を最前面に変更
        transform.parent.SetAsLastSibling();

        // ボタンのホバーを切る
        hoverButton.enabled = false;

        // Sequence を初期化して利用できる状態にする
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);

        // アイコンをボタンの位置から画面の中央(Canvas ゲームオブジェクトの位置)に移動させつつ
        sequence.Append(transform.DOMove(zoomInPos, 0.5f).SetEase(Ease.Linear));

        // アイコンを徐々に大きくしながら表示。指定したサイズになったら、元のアイコンの大きさに戻す
        sequence.Join(transform.DOScale(Vector2.one * 5.0f, 0.5f).SetEase(Ease.InBack)).OnComplete(() => { transform.DOScale(Vector2.one * 4.8f, 0.2f); });
    }

    /// <summary>
    /// アイコンズームアウト。こちらもクローンしたアイコンで使っている
    /// </summary>
    public void ZoomOutGalleryIcon() {
        // Sequence を初期化して利用できる状態にする
        Sequence sequence = DOTween.Sequence();

        sequence.SetLink(gameObject);

        // アイコンの大きさを徐々に 0 にして見えない状態にさせつつ
        sequence.Append(transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear));

        // それに合わせてアイコンをアルバムボタンの位置に移動させる。移動後にポップアップを破棄
        // DOLocalMove メソッドにするとボタンの位置に戻らないため、DOMove メソッドを使う
        sequence.Join(transform.DOLocalMove(startPos, 0.5f)
            .SetEase(Ease.Linear))
            .OnComplete(() => 
            {
                // ズーム状態を戻し、ホバー機能を入れなおす
                isZoomIn = false;
                hoverButton.enabled = true;
            });
    }
}