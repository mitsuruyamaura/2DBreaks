using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharaButtonDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField]
    private Image imgChara;

    [SerializeField]
    private Text txtStageOpenPoint;

    [SerializeField]   //�@�C���X�y�N�^�[�ł� Debug �p�B�m�F���ς񂾂� SerializeField �����̕t�^���O��
    private int stageNo;


    /// <summary>
    /// �����ݒ�
    /// </summary>
    /// <param name="stageNo"></param>
    /// <param name="menu"></param>
    public void SetUpCharaButtonDetail(int stageNo, Sprite charaSprite) {
        this.stageNo = stageNo;
        imgChara.sprite = charaSprite;
        txtStageOpenPoint.text = "�X�e�[�W " + (stageNo + 1) + "\r\n";
    }

    /// <summary>
    /// �}�E�X���z�o�[�����Ƃ��̏���
    /// </summary>
    private void ResponseHoverButton() {
        if (!btnChara.enabled) {
            return;
        }
        transform.DOShakeScale(0.25f, 0.5f, 4).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => transform.localScale = Vector3.one);
    }

    /// <summary>
    /// �X�e�[�W�J���ɕK�v�ȃ|�C���g�\��
    /// </summary>
    /// <param name="openPoint"></param>
    public void DisplayStageOpenPoint(int openPoint) {
        txtStageOpenPoint.text += openPoint + " �ŊJ��";
    }

    /// <summary>
    /// �L�����{�^���������̏���
    /// </summary>
    public void OnClickCharaButton() {
        SelectStage.stageNo = stageNo;

        btnChara.transform.DOShakeScale(0.3f).SetEase(Ease.InQuart).SetLink(gameObject);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // �V�[���J�ڂƃt�F�C�h�A�E�g����
        StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
    }

    /// <summary>
    /// �{�^���񊈐����@���@�s�v
    /// </summary>
    public void InactibeCharaButton() {
        btnChara.interactable = false;
    }

    /// <summary>
    /// �{�^���̎擾
    /// </summary>
    /// <returns></returns>
    public Button GetButton() {
        return btnChara;
    }

    /// <summary>
    /// �L�����{�^�������b�N
    /// </summary>
    public void LockCharaButton() {
        btnChara.enabled = false;  // ineractable ���� Dsabled Color �ɂȂ邽��
        imgChara.color = new(0, 0, 0, 0.6f);
    }
}