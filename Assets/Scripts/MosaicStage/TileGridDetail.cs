using UnityEngine;

/// <summary>
/// 色覚補助の状態
/// </summary>
public enum ColorAssistanceState {
    Off,
    RedGreen,
    BlueYellow
}


/// <summary>
/// Model と View に分ける必要なし
/// </summary>
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
    private Color GetColor(int colorNo, ColorAssistanceState colorAssistanceState) {
        return colorAssistanceState switch {
            ColorAssistanceState.Off => colorNo switch {
                0 => Color.black,
                1 => Color.red,
                2 => Color.blue,
                3 => Color.green,
                4 => Color.yellow,
                _ => Color.gray,
            },
            ColorAssistanceState.RedGreen => colorNo switch {
                0 => Color.black,
                1 => Color.red,       // 赤
                2 => Color.blue,      // 青
                3 => Color.yellow,    // 緑の代わりに黄
                4 => Color.gray,      // 元の黄は灰に置き換え
                _ => Color.gray,
            },
            ColorAssistanceState.BlueYellow => colorNo switch {
                0 => Color.black,
                1 => Color.red,                     // 赤はそのまま
                2 => new Color(0.667f, 0.4f, 1f),   // 紫寄りの青
                3 => Color.green,                   // 緑そのまま
                4 => new Color(1f, 0.667f, 0.333f), // オレンジ寄りの黄
                _ => Color.gray,
            },
            _ => Color.gray
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
        spriteTileGrid.color = GetColor(colorNo, UserData.instance.CurrentColorAssistanceState.Value);
    }
}