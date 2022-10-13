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
            // 端末のリターンボタンでも終了できるようにしておく
            OnClickOpenExitPopup();
        }
    }

    /// <summary>
    /// ゲーム終了確認用ポップアップを開く
    /// </summary>
    public void OnClickOpenExitPopup() {
        // すでに開いている場合には閉じる
        if (exitPopUp && exitPopUp.gameObject.activeSelf) {
            exitPopUp.ClosePopup();
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
            return;
        }

        // ExitPopup生成
        if (!exitPopUp) {
            exitPopUp = Instantiate(exitPopupPrefab, Camera.main.transform, false);
            exitPopUp.Setup();
        } else {
            exitPopUp.ActivePopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }
}