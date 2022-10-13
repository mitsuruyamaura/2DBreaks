using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [SerializeField]
    private ExitPopUp exitPopupPrefab;

    private ExitPopUp exitPopUp;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // �[���̃��^�[���{�^���ł��I���ł���悤�ɂ��Ă���
            OnClickOpenExitPopup();
        }
    }

    /// <summary>
    /// �Q�[���I���m�F�p�|�b�v�A�b�v���J��
    /// </summary>
    public void OnClickOpenExitPopup() {
        // ���łɊJ���Ă���ꍇ�ɂ͕���
        if (exitPopUp && exitPopUp.gameObject.activeSelf) {
            exitPopUp.ClosePopup();
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
            return;
        }

        // ExitPopup����
        if (!exitPopUp) {
            exitPopUp = Instantiate(exitPopupPrefab, Camera.main.transform, false);
            exitPopUp.Setup();
        } else {
            exitPopUp.ActivePopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }
}