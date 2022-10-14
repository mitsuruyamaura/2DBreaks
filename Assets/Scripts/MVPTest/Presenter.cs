using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Presenter : MonoBehaviour
{
    void Start()
    {
        // ReactiveProperty �̗��p���₷���󋵂́AMVP �p�^�[���ƌĂ΂��AUI �̕\���X�V�ɂ�����鏈��
        // �l(���_)���w��(�Ď�)���邱�ƂŁA�����I�� UI �̕\���X�V���s���悤�ȃC�x���g���������o�����Ƃ��ł���

        // GameManager �� Presenter �Ƃ�������������
        // Presenter �Ƃ́AModel �ł��� ScoreManager �ƁAView �ł��� UIManager �̑o����m���Ă��āA������Ȃ���ڂ��ɂȂ�

        // ScoreManager �ɗp�ӂ��Ă��� ReactiveProperty(Player �� Enemy �̓��_���Ǘ�����l) �������ōw��(�Ď�)���閽�߂��o��
        // �����ꂩ�̒l���X�V���ꂽ�ꍇ�ASubscribe ���\�b�h���ɂ��鏈���������I�Ɏ��s����
        // ����͒l���X�V����邽�сAUIManager �̃��\�b�h�����s���A�o���̒l�̏����������g���Ē񋟂���

        // �������邱�ƂŁAPresenter ���ł��� GameManager �݂̂��AScoreManager �� UIManager ��m���Ă����(�a����)�ł���Ȃ���
        // �l�̍X�V�ɍ��킹�āA��ʂ̕\���X�V���ꏏ�ɘA�����ď������s�����Ƃ��ł���
        // ����āA���݂̂悤�ɁA�l�̍X�V�ɍ��킹�Ă��̓s�x�A��ʕ\���X�V�̖��߂��s���K�v���Ȃ��Ȃ�


        // ReactiveProperty ���w�ǁ@���̇@(�n�߂Ɋo������@)
        // Model �Ƃ��� ScoreManager �̑���� GameData �𗘗p���Ă��邪�A����́A���� UpdateTxtScore ���\�b�h�̏������������Ă��邽��(���\�b�h�Ɉ������Ȃ�����)
        //GameData.instance.PlayerScore.Subscribe(_ => uiManager.PrepareUpdateTxtScore()).AddTo(this);
        //GameData.instance.EnemyScore.Subscribe(_ => uiManager.PrepareUpdateTxtScore()).AddTo(this);


        // ReactiveProperty ���w�ǁ@���̇A(������A�o���������@�B�@�̏������R�����g�A�E�g����΁A�����悤�ɐ���ɓ����܂�)
        //Observable.CombineLatest(scoreManager.PlayerScore, scoreManager.EnemyScore, (playerScore, enemyScore) => (playerScore, enemyScore))
        //    .Subscribe(scores => uiManager.UpdateDisplayScoreObservable(scores.playerScore, scores.enemyScore))
        //   .AddTo(this);


        // �܂��́AReactiveProperty �𗘗p���� MVP �p�^�[���ɂ��A�l�� UI �\���X�V�̘A������������Ɗo���邱��
        // �������AMVP �p�^�[���� UI �ɂ̂ݎg���悤�ɂ��邱��
        // Subscribe �� AddTo �Ƃ��������\�b�h�̋@�\����������Ɨ������邱��

        // �v���O�����ɂ͐�΂ȏ������͂Ȃ��̂ŁAUniRx �ɂ����Ă��A�����܂ł��A�X�L���̈����o���̕����L������̂ł���ƍl���邱��
        // ���ł������ɂȂ�Ȃ��悤�ɂ���B�_��Ȏv�l��Y��Ȃ�
        // ReactiveProperty ���̂͐F�X�ȏ����ɉ��p�\�����A�Ȃ�ł�����ł����p����A�Ƃ������Ƃł͂Ȃ�
        // �֗��ȋ@�\�ł���A�����������镝���L���邪�A��قǂ������Ă���悤�ɁA���ׂĂɂ����ėL���Ƃ����킯�ł͂Ȃ�(�ǂ݉����Ȃ��l������)

        // ��L�̎�����́A�܂��́A�@�̕��ŐF�X�ȏ����������Ă݂āA�ǂ������������������̂��������āA�X���X���Ə����郌�x���ɂ��邱�Ƃ�ڕW�ɂ���

        // ���̌�A�A�̏����̓��e�𗝉����Ă����悤�ɂ���
        // �l�b�g�Ȃǂɂ�������͂�����̂́A�����̃v���W�F�N�g�ɗ��Ƃ����񂾂��̂͐�΂Ɍ�����Ȃ��̂ŁA�����̓������o���邱��
    }


}
