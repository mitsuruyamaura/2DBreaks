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
    /// �����ݒ�
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetUpTileGridDetail(int colorNo) {
        SetTileGridTile(colorNo);
        SetColor(colorNo);
    }

    /// <summary>
    /// �F���擾
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
    /// �F�̎�ނ�ݒ�
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetTileGridTile(int colorNo) {
        tileGridType = (TileGridType)colorNo;
    }

    /// <summary>
    /// �F��ݒ�
    /// </summary>
    /// <param name="colorNo"></param>
    public void SetColor(int colorNo) {
        spriteTileGrid.color = GetColor(colorNo);
    }
}
