using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public enum PlayerState {
    Charge,
    Normal
}

public class AttackController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float chargeTimer;

    [SerializeField]
    private float maxChargeCount;

    [SerializeField]
    private float countUpRate;   // �␳

    [SerializeField]
    private float BasePower;


    void Start() {
        TryGetComponent(out rb);
    }

    void Update()
    {
        // �P�V���b�g
        //if (Input.GetMouseButtonDown(0)) {
        //    // �m�[�}���U��
        //    Attack(); 
        //}


        if (Input.GetMouseButton(0)) {
            // �`���[�W
            if (chargeTimer > maxChargeCount) {
                chargeTimer = maxChargeCount;
                return;
            }

            // �`���[�W + �`���[�W���x�̒���
            chargeTimer += Time.deltaTime * countUpRate;

            // �G�t�F�N�g����


        } else if (Input.GetMouseButtonUp(0)){
            // �`���[�W�U���@�`���[�W�������ԕ��������x�A�b�v
            Attack(chargeTimer);

            chargeTimer = 0;

            // �G�t�F�N�g������ꍇ�ɂ͔j��


        }
    }

    private void Attack(float chargeTime) {
        Debug.Log(chargeTime);
        //rb.velocity = Vector2.zero;  // �Ȃ��Ă����Ȃ�

        // transform.up ��2�c���E�ł͐��ʂ�������B3�c�� transform.forword �ƃC���[�W�͓����Bright �ɂ���ƉE�ɍs��
        rb.AddForce(transform.up * BasePower * chargeTime, ForceMode2D.Impulse);
    }


    void FixedUpdate() {

        // �����Ă���Ԃ͏��X�ɒ�~������
        if (rb.velocity != Vector2.zero) {
            rb.velocity *= 0.9f;

            // �o���̕��������_�̒l�� 0 �ɋ߂��Ȃ�
            if (Mathf.Approximately(0, rb.velocity.x) && Mathf.Approximately(0, rb.velocity.y)) {
                rb.velocity = Vector2.zero;
            }
        }
    }
}