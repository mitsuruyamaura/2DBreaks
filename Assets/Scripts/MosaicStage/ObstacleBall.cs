using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

public class ObstacleBall : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Rigidbody2D rb;
    private float minSpeed;
    private float maxSpeed;
    private float slowSpeed = 1.5f;
    private float timer;
    private float moveInterval = 5.0f;


    /// <summary>
    /// DI �R���e�i���p��
    /// </summary>
    /// <param name="speeds"></param>
    /// <param name="mainGameManager"></param>
    /// <param name="lifeModel"></param>
    public void SetUpObstacleBall(float[] speeds, MainGameManager mainGameManager, LifeModel lifeModel) {
        minSpeed = speeds[0];
        maxSpeed = speeds[1];

        // ���s�̃^�C�~���O�������ƊĎ��̏����� Where �̏������ݒ肳��Ȃ�(�Q�Ƃ� null �ł͂Ȃ����A�Ď��̍ۂɂ��蔲����)

        // �t�B�[�o�[�^�C���ł͂Ȃ��A�v���C���̂Ƃ��ɂ́A��莞�Ԃ��ƂɃ����_���ړ����J��Ԃ�
        this.UpdateAsObservable()
            .Where(_ => !mainGameManager.IsFeverTime.Value)
            .Where(_ => mainGameManager.State.Value == GameState.Play)
            .Subscribe(_ => RandomShot())
            .AddTo(this);

        // �t�B�[�o�[�^�C���ł͂Ȃ��A�v���C���̂Ƃ��ɂ́A�I�𒆂̃O���b�h�ɐڐG�����ꍇ�A�A�E�g�ɂ���
        this.OnTriggerEnter2DAsObservable()
            .Where(_ => !mainGameManager.IsFeverTime.Value)
            .Where(_ => mainGameManager.State.Value == GameState.Play)
            .Subscribe(collision => {
                if (collision.TryGetComponent(out TileGridDetail tileGridDetail)) {
                    if (tileGridDetail.IsSelected) {
                        //Debug.Log(mainGameManager.IsFeverTime.Value);
                        //Debug.Log(mainGameManager.State.Value);

                        lifeModel.LifeCount.Value--;

                        StopMoveBall();

                        mainGameManager.State.Value = GameState.Ready;
                        //Debug.Log("�ڐG");
                    }
                }
            })
            .AddTo(this);

        if (TryGetComponent(out rb)) {
            // �{�[���𔭎�
            ShotBall(false);
        }
    }

    /// <summary>
    /// ��Q���̏����ݒ�(MosaicManager ���p��)
    /// </summary>
    /// <param name="mosaicManager"></param>
    public void SetUpObstacleBall(MosaicManager mosaicManager, float[] speeds) {
        minSpeed = speeds[0];
        maxSpeed = speeds[1];

        // �t�B�[�o�[�^�C���ł͂Ȃ��A�v���C���̂Ƃ��ɂ́A��莞�Ԃ��ƂɃ����_���ړ����J��Ԃ�
        this.UpdateAsObservable()
            .Where(_ => !mosaicManager.IsFeverTime.Value && mosaicManager.gameState == MosaicManager.GameState.Play)
            .Subscribe(_ => RandomShot())
            .AddTo(this);

        // �t�B�[�o�[�^�C���ł͂Ȃ��A�v���C���̂Ƃ��ɂ́A�I�𒆂̃O���b�h�ɐڐG�����ꍇ�A�A�E�g�ɂ���
        this.OnTriggerEnter2DAsObservable()
            .Where(_ => !mosaicManager.IsFeverTime.Value && mosaicManager.gameState == MosaicManager.GameState.Play)
            .Subscribe(collision => {
                //if (collision.gameObject.CompareTag("Untagged")) {
                //    Debug.Log("!!");
                //}

                //Debug.Log(collision.gameObject.name);
                if (collision.TryGetComponent(out TileGridDetail tileGridDetail)) {
                    //Debug.Log(collision.gameObject.name);
                    if (tileGridDetail.IsSelected) {
                        //Debug.Log("�A�E�g");
                        StopMoveBall();

                        mosaicManager.FailureErase();
                    }
                }
            })
            .AddTo(this);

        if (TryGetComponent(out rb)) {
            // �{�[���𔭎�
            ShotBall(false);
        }
    }

    /// <summary>
    /// ��Q���̈ړ����ēx�s���A�ړ���������_����
    /// �ǂ̊Ԃ��s�����藈���肵�Ă��܂����Ƃ��h��
    /// </summary>
    private void RandomShot() {
        timer += Time.deltaTime;

        if (timer >= moveInterval) {
            timer = 0;
            ShotBall(false);
        }
    }

    /// <summary>
    /// �{�[���𔭎�
    /// </summary>
    public void ShotBall(bool isSlowDown) {
        speed = isSlowDown ? slowSpeed : Random.Range(minSpeed, maxSpeed);

        // �p�x�ɂ���đ��x���ω����Ă��܂��̂�normalized�Ő��K�����ē������x�x�N�g���ɂ���
        Vector2 direction = new Vector2(Random.Range(-2.5f, 2.5f), 1).normalized;

        // �{�[����ł��o��(���C���C��R�A�d�͂�؂��Ă���̂ŁA�����Ɠ������x�œ���������)
        rb.velocity = -direction * speed * transform.localScale.x;
        //Debug.Log("�ړ�");
    }

    /// <summary>
    /// �{�[�����~�߂�
    /// </summary>
    public void StopMoveBall() {
        // �{�[���̑��x�x�N�g����0�ɂ��Ď~�߂�
        rb.velocity = Vector2.zero;
    }
}