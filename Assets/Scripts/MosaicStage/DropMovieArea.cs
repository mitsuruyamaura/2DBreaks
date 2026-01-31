using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMovieArea : MonoBehaviour, IBeginDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Image containerImage;

    private int slotIndex;
    public int SlotIndex => slotIndex;

    private Color normalColor;
    private Color highlightColor = Color.yellow;
    private DragThumnail currentThumnail;                  // 現在このスロットに配置されているサムネイルを保持

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="index"></param>
    public void SetupDropMovieArea(int index) {
        slotIndex = index;

        if (containerImage != null) {
            normalColor = containerImage.color;
        }
    }

    /// <summary>
    /// ドラッグ開始時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData) {
        currentThumnail = null;
    }

    /// <summary>
    /// スロット上にムービーのサムネイルをドロップ時の処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData) {
        containerImage.color = normalColor;

        // ドロップしたオブジェクトの取得
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) {
            return;
        }

        // ドロップされたムービー情報の取得
        if (droppedObject.TryGetComponent(out DragThumnail droppedChara)) {
            droppedChara.SetDropped();

            // 現在のスロットに既にサムネイルがいる場合、配置を入れ替える
            if (currentThumnail != null) {
                // ドロップされてきたサムネイルのいたスロットの DropMovieArea を取得
                DropMovieArea previousSlotArea = droppedChara.CurrentDropMovieArea;

                // 今スロット内にいるサムネイルを、ドロップされてきたサムネイルの位置と交換
                currentThumnail.transform.SetParent(previousSlotArea.transform, false);
                currentThumnail.transform.localPosition = Vector3.zero;

                // 同様に情報も交換
                currentThumnail.SetCurrentDropMovieArea(previousSlotArea);
                previousSlotArea.currentThumnail = currentThumnail;
            }

            // ドロップされたサムネイルをスロットへ配置(空のスロットにドロップした場合はここのみ行う)
            droppedChara.transform.SetParent(transform, false);
            droppedChara.transform.localPosition = Vector3.zero;

            // 新しいインデックスを通知(元の Index を保持しているので、交換も行う)
            droppedChara.NotifyDrop(slotIndex);

            // スロットに配置されたムービー情報の更新
            currentThumnail = droppedChara;

            // 新しく配置したサムネイルのスロット情報を更新
            droppedChara.SetCurrentDropMovieArea(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // マウスが侵入しても、ドラッグ中のオブジェクトがない場合は反応しない
        if (containerImage == null || eventData.pointerDrag == null) {
            return;
        }

        // ドロップ可能な範囲の色を変える
        containerImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        // マウスが出た時も、ドラッグ中のオブジェクトがない場合は反応しない
        if (containerImage == null || eventData.pointerDrag == null) {
            return;
        }

        // ドロップ可能な範囲の色を戻す
        containerImage.color = normalColor;
    }

    /// <summary>
    /// 配置中のサムネイルの情報をセット
    /// 下のスクロールバーからスロットに初期配置されたときや、元のスロット位置に戻った時に実行
    /// </summary>
    /// <param name="dragChara"></param>
    public void SetChara(DragThumnail dragChara) {
        currentThumnail = dragChara;
    }

    /// <summary>
    /// 配置中のサムネイル情報を削除
    /// </summary>
    public void ReleaseThumnail() {
        currentThumnail = null;
    }
}