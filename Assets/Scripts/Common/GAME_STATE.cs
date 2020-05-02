/// <summary>
/// ゲーム状態の管理用
/// </summary>
public enum GAME_STATE
{
    STOP,　       // ゲームが動いていない状態
    READY,        // ブロックの配置まで終わってボールが打てる状態
    PLAY,         // バトル中の状態
    WARNING,      // ライフの残りが少ない警告状態
    STAGE_CLEAR,  // ステージクリア状態
    GAME_OVER,    // ゲームオーバー状態
    COUNT         // enumをintでキャストした際のLength用
}
