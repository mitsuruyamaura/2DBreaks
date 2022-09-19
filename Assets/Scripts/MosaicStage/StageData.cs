using UnityEngine;

namespace yamap {

    /// <summary>
    /// �X�e�[�W�̏��
    /// </summary>
    [System.Serializable]
    public class StageData {
        public int stageNo;
        public Sprite normalCharaSprite;   // �ʏ�̃L�����G
        public Sprite rareCharaSprite;     // ���ɉB��Ă���L�����G
        public int obstacleCount;          // ���������Q���̐�
        public float[] obstacleSpeeds;     // ��Q���̈ړ����x�̍ŏ��l�ƍő�l
        public int stageOpenPoint;    �@�@ // �X�e�[�W�J���ɕK�v�ȃ|�C���g
        public Sprite charaIcon;           // �L�����A�C�R��
    }
}