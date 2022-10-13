using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �΂�����̋L��
// https://3dunity.org/game-create-lesson/clicker-game/mobile-adjustment/
// Free Aspect �Ŋm�F����

[ExecuteAlways]  // https://docs.unity3d.com/ScriptReference/ExecuteAlways.html  // Prefab �ҏW���[�h��A�G�f�B�^�[�Đ����I������Ƃ��Ɏ��s�����悤�ɂȂ�
public class AspectKeeper : MonoBehaviour {

    [SerializeField]
    private Camera targetCamera; //�ΏۂƂ���J����

    [SerializeField]
    private Vector2 aspectVec; //�ړI�𑜓x

    void Update() {
        var screenAspect = Screen.width / (float)Screen.height; //��ʂ̃A�X�y�N�g��
        var targetAspect = aspectVec.x / aspectVec.y; //�ړI�̃A�X�y�N�g��

        var magRate = targetAspect / screenAspect; //�ړI�A�X�y�N�g��ɂ��邽�߂̔{��

        var viewportRect = new Rect(0, 0, 1, 1); //Viewport�����l��Rect���쐬

        if (magRate < 1) {
            viewportRect.width = magRate; //�g�p���鉡����ύX
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;//���[�Ɋ񂹂��Ă��܂��̂ŁA0.5 ���g���A������
        } else {
            viewportRect.height = 1 / magRate; //�g�p����c����ύX(�������k�߂�ƁA�c�����͂ݏo��(1�𒴂���)���ߒ���)
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;//���Ɋ񂹂��Ă��܂��̂ŁA0.5 ���g���A������
        }

        targetCamera.rect = viewportRect; //�J������Viewport�ɓK�p
    }
}