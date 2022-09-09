using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class CharaButtonDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField]
    private Image imgChara;

    //[SerializeField]
    private int stageNo;
    private Menu menu;

    /// <summary>
    /// �����ݒ�
    /// </summary>
    /// <param name="stageNo"></param>
    /// <param name="menu"></param>
    public void SetUpCharaButtonDetail(int stageNo, Menu menu, Sprite charaSprite) {
        this.stageNo = stageNo;
        this.menu = menu;

        imgChara.sprite = charaSprite;

        btnChara.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => 
            {
                SelectStage.stageNo = stageNo;

                btnChara.transform.DOShakeScale(0.3f).SetEase(Ease.InQuart).SetLink(gameObject);

                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.Submit);

                // ���̃{�^���������Ȃ���Ԃɂ���
                menu.InactiveAllCharaButtons();

                // �V�[���J�ڂƃt�F�C�h�A�E�g����
                StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Stage));
            })
            .AddTo(gameObject);          
    }

    /// <summary>
    /// �{�^���񊈐���
    /// </summary>
    public void InactibeCharaButton() {
        btnChara.interactable = false;
    }
}
