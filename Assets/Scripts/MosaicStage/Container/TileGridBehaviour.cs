using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// TileGrid の生成・つながったグリッドの削除や、残ったグリッドの管理用クラス
/// </summary>
public class TileGridBehaviour : MonoBehaviour
{
    [SerializeField]
    private TileGridDetail tileGridPrefab;

    [SerializeField]
    private GameObject eraseEffectPrefab;

    [SerializeField]
    private Transform tileGridSetTran;

    [SerializeField, Header("行")]
    private int rowCount;

    [SerializeField, Header("列")]
    private int columnCount;

    [SerializeField]
    private float tileGridSize;

    [SerializeField, Header("生成された Grid のリスト")]
    public List<TileGridDetail> tileGridList = new();

    public ReactiveCollection<TileGridDetail> TileGridList = new();

    private bool isLastColor;


    public Transform GetTileGridSetTran() => tileGridSetTran;
    public GameObject GetEraseEffect() => eraseEffectPrefab;
    public float GetTileGridSize() => tileGridSize;


    /// <summary>
    /// グリッドを生成
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public void CreateTileGrids() {
        for (int i = 0; i < rowCount; i++) {
            for (int j = 0; j < columnCount; j++) {
                // TileGridDetail プレファブを生成
                TileGridDetail tileGrid = Instantiate(tileGridPrefab, tileGridSetTran, true);

                // サイズ変更と位置変更して並べる
                tileGrid.transform.localScale = Vector3.one * tileGridSize;
                tileGrid.transform.localPosition = new(j * tileGridSize, i * tileGridSize, 0);

                // グリッドの初期設定。グリッドの色をランダムに１つ選択
                tileGrid.SetUpTileGridDetail(UnityEngine.Random.Range(0, (int)TileGridType.Count));

                tileGridList.Add(tileGrid);
            }
        }

        TileGridList = new(tileGridList);
    }

    /// <summary>
    /// つながっているグリッド群をまとめて削除
    /// </summary>
    /// <param name="eraseTileGridList"></param>
    public void EraseTileGrids(List<TileGridDetail> eraseTileGridList) {

        for (int i = 0; i < eraseTileGridList.Count; i++) {

            // リストから取り除く
            tileGridList.Remove(eraseTileGridList[i]);
            TileGridList.Remove(eraseTileGridList[i]);

            // エフェクトのプレファブが用意されている場合
            if (eraseEffectPrefab) {
                //Debug.Log("エフェクト生成");
                // エフェクト生成
                GameObject effect = Instantiate(eraseEffectPrefab, eraseTileGridList[i].gameObject.transform);
                effect.transform.SetParent(tileGridSetTran);
                Destroy(effect, 0.6f);
            }

            // グリッドを削除
            Destroy(eraseTileGridList[i].gameObject);
        }
    }

    /// <summary>
    /// 詰まないように残りのグリッドが少なくなったか確認する
    /// </summary>
    public void CheckLastColor() {
        if (!isLastColor && tileGridList.Count < 12) {
            isLastColor = true;

            // 残ったグリッドの色を１色に変える
            ChangeTileGridsColor();
        }
    }

    /// <summary>
    /// 残ったグリッドの色を１色に変える
    /// 残りが指定数以下になったときに利用して、ゲームが詰む状態をなくす
    /// </summary>
    private void ChangeTileGridsColor() {
        // 白以外にする
        int randomColorNo = UnityEngine.Random.Range(0, (int)TileGridType.白);
        for (int i = 0; i < tileGridList.Count; i++) {
            tileGridList[i].SetTileGridTile(randomColorNo);
            tileGridList[i].SetColor(randomColorNo);
        }

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);
    }

    /// <summary>
    /// 残っているグリッドをすべて削除
    /// </summary>
    public void AllEraseTileGird() {
        for (int i = 0; i < tileGridList.Count; i++) {
            Destroy(tileGridList[i].gameObject);
        }
        tileGridList.Clear();
        TileGridList.Clear();
    }
}