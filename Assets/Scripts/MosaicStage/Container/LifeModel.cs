using UniRx;

public class LifeModel
{
    private readonly int lifeCount = 3;
    private readonly int oneMissCount = 2;
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
    /// ワンミスクリア判定。true ならワンミスクリア。
    /// </summary>
    /// <returns></returns>
    public bool IsOnMissClear() {
        return LifeCount.Value >= oneMissCount ? true : false;
    }

    /// <summary>
    /// ライフが残っていないか判定。true なら残っていないため、ゲームオーバーに繋げる
    /// </summary>
    /// <returns></returns>
    public bool IsNotLifeLeft() {
        return LifeCount.Value <= 0 ? true : false;
    }
}