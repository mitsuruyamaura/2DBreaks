using UnityEngine;

/// <summary>
/// ギャラリー用ズームビューワー
/// </summary>
public class GalleryZoomViewer : MonoBehaviour {
    public static GalleryZoomViewer instance;

    [SerializeField] private GalleryIconDetail zoomPrefab;
    private Transform overlayRoot;
    private GalleryIconDetail current;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ズームイン用にクローンしたアイコンを表示する場所の設定
    /// </summary>
    /// <param name="zoomTran"></param>
    public void SetOverlayRoot(Transform zoomTran) {
        overlayRoot = zoomTran;
    }

    public void Show(GalleryIconDetail source) {
        if (current != null) return;

        // コピーを生成してズーム表示。フレームは隠す
        current = Instantiate(zoomPrefab, overlayRoot);
        current.SetUp(source.GetCharaSprite(), null);

        current.transform.position = source.transform.position;
        current.SetZoomInPosition(overlayRoot.position);

        // ボタンのホバーを切る
        current.InactivateHoverButton();
        current.ZoomInGalleryIcon();

        // ボタン機能が登録されていないのでアイコンを破棄する処理を登録する
        current.SetZoomOutBtnByClone();
    }

    public void Hide() {
        if (current == null) return;

        current.ZoomOutGalleryIcon();
        Destroy(current.gameObject, 0.3f);
        current = null;
    }
}