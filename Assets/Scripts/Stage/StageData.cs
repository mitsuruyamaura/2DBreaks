
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObject/CreateStageData")]
public class StageData : ScriptableObject
{
    public List<StageDataList> stageDatas = new List<StageDataList>();

    /// <summary>
    /// ステージ毎の設定項目
    /// </summary>
    [System.Serializable]
    public class StageDataList {
        [Header("ステージ番号")]
        public int stageNum;
        [Header("ブロック用画像のファイル名")]
        public string stageSpriteName;

        [Header("ボールの速度")]
        public float ballSpeed;
        [Header("ボールの威力(現在はプレイヤーへのダメージに使用)")]
        public int ballPower;

        [Header("引くラインの最小の長さ")]
        public float minLineLength;
        [Header("ラインの出現時間")]
        public float lineDuration;

        [Header("ライフの最大値")]
        public int initMaxLife;
        [Header("バトル時間")]
        public int initBattleTime;

        [Header("ブロック生成のスタート位置")]
        public Vector3 startPosition;
    }
}
