using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStageData 
{
    public int stageNo;

    public int phaseCount;

    public int clearMoney;

    public List<PhaseData> phaseDataList = new List<PhaseData>();

    [System.Serializable]
    public class PhaseData {
        public int phaseNo;

        public int[] enemyNos;
        public Vector2[] enemyPositios;

        public int[] obstructNos;
        public Vector2[] obstructPositions;

    }
}
