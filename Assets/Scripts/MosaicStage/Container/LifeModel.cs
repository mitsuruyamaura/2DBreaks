using UniRx;

public class LifeModel
{
    private int lifeCount = 3;
    public ReactiveProperty<int> LifeCount = new();　　//　購読


    /// <summary>
    /// ライフの初期設定
    /// </summary>
    public void SetLifeCount() {
        LifeCount.Value = lifeCount;
    }
    
    /// <summary>
    /// ノーミスクリア判定。true ならノーミスクリア。
    /// </summary>
    /// <returns></returns>
    public bool IsNoMissClear() {
        return LifeCount.Value >= lifeCount ? true : false;
    }

    /// <summary>
    /// ライフが残っていないか判定。true なら残っていないため、ゲームオーバーに繋げる
    /// </summary>
    /// <returns></returns>
    public bool IsNotLifeLeft() {
        return LifeCount.Value <= 0 ? true : false;
    }
}