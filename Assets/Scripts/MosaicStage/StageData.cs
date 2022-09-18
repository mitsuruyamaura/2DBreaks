using UnityEngine;

namespace yamap {

    /// <summary>
    /// ステージの情報
    /// </summary>
    [System.Serializable]
    public class StageData {
        public int stageNo;
        public Sprite normalCharaSprite;   // 通常のキャラ絵
        public Sprite rareCharaSprite;     // 裏に隠れているキャラ絵
        public int obstacleCount;          // 生成する障害物の数
        public float[] obstacleSpeeds;     // 障害物の移動速度の最小値と最大値
        public int stageOpenPoint;    　　 // ステージ開放に必要なポイント
        public Sprite charaIcon;           // キャラアイコン
    }
}