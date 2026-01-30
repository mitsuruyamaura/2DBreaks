using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギャラリー内のタブに対応したコンテナ
/// </summary>
public class GalleryTabView : MonoBehaviour {
    public int tabIndex;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GalleryIconDetail galleryIconPrefab;
    [SerializeField] private Transform galleryIconTran;
    [SerializeField] private Sprite[] frameSprites;

    public List<GalleryIconDetail> Setup(int tabIndex, List<StageClearData> stageClearDataList) {
        this.tabIndex = tabIndex;
        List<GalleryIconDetail> galleryIconDetailList = CreateGalleryIcons(stageClearDataList);
        return galleryIconDetailList;
    }

    /// <summary>
    /// ギャラリー用キャラアイコンのボタン生成
    /// </summary>
    public List<GalleryIconDetail> CreateGalleryIcons(List<StageClearData> stageClearDataList) {
        List<GalleryIconDetail> galleryIconDetailList = new();

        for (int i = 0; i < stageClearDataList.Count; i++) {
            StageClearData stageClearData = stageClearDataList[i];
            yamap.StageData stageData = UserData.instance.GetStageData(stageClearData.stageType, stageClearData.stageNo);

            // 通常クリア用アイコン
            GalleryIconDetail galleryIcon = Instantiate(galleryIconPrefab, galleryIconTran, false);
            Sprite charaSprite = stageData.normalCharaSprite;
            galleryIcon.SetUp(charaSprite, frameSprites[0]);
            galleryIconDetailList.Add(galleryIcon);

            // ワンミスクリアかエクセレントクリアなら、レア用アイコン
            if (stageClearData.isOneMissClear || stageClearData.isNoMissClear) {
                GalleryIconDetail rareGalleryIcon = Instantiate(galleryIconPrefab, galleryIconTran, false);
                Sprite rareCharaSprite = stageData.rareCharaSprite;
                rareGalleryIcon.SetUp(rareCharaSprite, frameSprites[1]);
                galleryIconDetailList.Add(rareGalleryIcon);
            }
        }

        return galleryIconDetailList;
    }

    public void ShowTabView() {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideTabView() {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}