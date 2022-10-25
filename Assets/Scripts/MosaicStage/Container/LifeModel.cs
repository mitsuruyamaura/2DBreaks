using UniRx;

public class LifeModel
{
    private int lifeCount = 3;
    public ReactiveProperty<int> LifeCount = new();�@�@//�@�w��


    /// <summary>
    /// ���C�t�̏����ݒ�
    /// </summary>
    public void SetLifeCount() {
        LifeCount.Value = lifeCount;
    }
    
    /// <summary>
    /// �m�[�~�X�N���A����Btrue �Ȃ�m�[�~�X�N���A�B
    /// </summary>
    /// <returns></returns>
    public bool IsNoMissClear() {
        return LifeCount.Value >= lifeCount ? true : false;
    }

    /// <summary>
    /// ���C�t���c���Ă��Ȃ�������Btrue �Ȃ�c���Ă��Ȃ����߁A�Q�[���I�[�o�[�Ɍq����
    /// </summary>
    /// <returns></returns>
    public bool IsNotLifeLeft() {
        return LifeCount.Value <= 0 ? true : false;
    }
}