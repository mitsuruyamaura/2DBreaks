using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGridDetail : MonoBehaviour
{
    public TileGridType tileGridType;

    public bool IsSelected;

    public int Num;

    public SpriteRenderer spriteTileGrid;

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetUpTileGridDetail(int colorNo) {
        SetTileGridTile(colorNo);
        SetColor(colorNo);
    }

    /// <summary>
    /// 色を取得
    /// </summary>
    /// <param name="colorNo"></param>
    /// <returns></returns>
    private Color GetColor(int colorNo) {
        return colorNo switch {
            0 => Color.black,
            1 => Color.red,
            2 => Color.white,
            3 => Color.blue,
            4 => Color.green,
            _ => Color.gray,
        };
    }

    /// <summary>
    /// 色の種類を設定
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetTileGridTile(int colorNo) {
        tileGridType = (TileGridType)colorNo;
    }

    /// <summary>
    /// 色を設定
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetColor(int colorNo) {
        spriteTileGrid.color = GetColor(colorNo);
    }
}
