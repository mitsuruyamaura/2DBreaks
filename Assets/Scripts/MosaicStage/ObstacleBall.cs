using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ObstacleBall : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Rigidbody2D rb;
    private MosaicManager mosaicManager;

    /// <summary>
    /// ��Q���̏����ݒ�
    /// </summary>
    /// <param name="mosaicManager"></param>
    public void SetUpObstacleBall(MosaicManager mosaicManager) {

        this.mosaicManager = mosaicManager;

        // �I�𒆂̃O���b�h�ɐڐG�����ꍇ�A�A�E�g�ɂ���
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                //if (collision.gameObject.CompareTag("Untagged")) {
                //    Debug.Log("!!");
                //}

                //Debug.Log(collision.gameObject.name);
                if (collision.TryGetComponent(out TileGridDetail tileGridDetail)) {
                    //Debug.Log(collision.gameObject.name);
                    if (tileGridDetail.IsSelected) {
                        Debug.Log("�A�E�g");
                        StopMoveBall();

                        mosaicManager.FailureErase();
                    }
                }
            })
            .AddTo(this);

        if (TryGetComponent(out rb)) {
            // �{�[���𔭎�
            ShotBall();
        }
    }

    /// <summary>
    /// �{�[���𔭎�
    /// </summary>
    public void ShotBall() {
        // �p�x�ɂ���đ��x���ω����Ă��܂��̂�normalized�Ő��K�����ē������x�x�N�g���ɂ���
        Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;

        // �{�[����ł��o��(���C���C��R�A�d�͂�؂��Ă���̂ŁA�����Ɠ������x�œ���������)
        rb.velocity = -direction * speed * transform.localScale.x;
    }

    /// <summary>
    /// �{�[�����~�߂�
    /// </summary>
    public void StopMoveBall() {
        // �{�[���̑��x�x�N�g����0�ɂ��Ď~�߂�
        rb.velocity = Vector2.zero;
    }
}
