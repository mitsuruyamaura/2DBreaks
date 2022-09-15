using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementStageData
{
    public int stageNo;
    public int challengeCount;
    public int clearCount;
    public int failureCount;
    public int maxFeverCount;
    public int noMissClearCount;
    public int maxMosaicCount;      // �P�Q�[��������̍ō��l���|�C���g
    public int maxLinkCount;        // �P�Q�[��������ŁA��x�ɂ܂Ƃ߂ď������u���b�N�̍ō���
    public float fastestClearTime;    // �P�Q�[���ɂ�����A�ł������N���A�^�C��


    public AchievementStageData(int stageNo) {
        this.stageNo = stageNo;
    }
}
