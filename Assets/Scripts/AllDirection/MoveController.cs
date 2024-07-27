using UnityEngine;
using DG.Tweening;

public class MoveController : MonoBehaviour
{
    //private Rigidbody2D rb;

    //[SerializeField]
    //private float blowPower;

    private Tween tween;

    [SerializeField]
    private float limitX = 19.5f;�@�@�@//�@StageData ����擾�ł���悤�ɂ��Ƃŕς���

    [SerializeField]
    private float limitY = 12.5f;


    private void Reset() {
        //TryGetComponent(out rb);
    }

    void Start()
    {
        Reset();
    }


    void Update() {

        // �E�N���b�N����
        if (Input.GetMouseButtonDown(1)) {
            // ����������
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.localPosition;
            //Debug.Log(direction);

            // �N���b�N�����n�_�ֈړ�(�ړ������Ȃ��悤�ɂ����B������ς��邾��)
            //MoveToClickPoint(direction.normalized);

            // ������ς��鏈��(tween)�������Ă���ꍇ�ɂ͒�~(�A���N���b�N���̑Ή�)
            tween?.Kill();

            // �}�E�X�̕���������
            tween = transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward, direction), 0.25f).SetEase(Ease.Linear).SetLink(gameObject);
        }

        // ���W����
        Vector3 pos = new(Mathf.Clamp(transform.position.x, -limitX, limitX), Mathf.Clamp(transform.position.y, -limitY, limitY));
        transform.position = pos;

        // �ړ����Ă���Ԃ̓}�E�X�J�[�\���̕����������I�Ɍ����@���@DORotateQuaternion �ɕς���
        //if (rb.velocity != Vector2.zero) {
            //transform.rotation = 
                //Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.localPosition), 0.1f);
        //}
    }

    /// <summary>
    /// �E�N���b�N�����n�_�ֈړ�(���p���Ă��Ȃ�)
    /// </summary>
    /// <param name="direction"></param>
    //private void MoveToClickPoint(Vector2 direction) {
    //    rb.velocity = Vector2.zero;
    //    rb.velocity = direction * blowPower;
    //}

    // �ړ������Ȃ������̂ŁAAttack �̕��ֈڊǂ���
    //void FixedUpdate() {
    //    // �����Ă���Ԃ͏��X�ɒ�~������
    //    if (rb.velocity != Vector2.zero) {
    //        rb.velocity *= 0.995f;
    //    }
    //}
}
