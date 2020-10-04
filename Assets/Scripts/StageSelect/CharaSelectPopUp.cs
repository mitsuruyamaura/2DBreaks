using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaSelectPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnClosePopup;

    [SerializeField]
    private Button btnStartBattle;

    public CanvasGroup canvasGroup;

    private StageSelectManager stageSelectManager;

    public void SetUpCharaSelectPopUp(StageSelectManager stageSelectManager) {
        canvasGroup.alpha = 0;
        this.stageSelectManager = stageSelectManager;

        // 各ボタンにメソッド登録
        btnClosePopup.onClick.AddListener(() => stageSelectManager.ActivateCharaSelectPopUp(0.0f, false));
        btnStartBattle.onClick.AddListener(OnClickStartBattle);

        // TODO 初期選択キャラを選択中にする

    }

    /// <summary>
    /// バトル開始
    /// </summary>
    private void OnClickStartBattle() {
        // TODO SE

        // バトルシーンへ遷移開始
        StartCoroutine(stageSelectManager.PraparateBattleScene());
    }

    /// <summary>
    /// タップされたキャラを使用キャラとしてGameDataに登録し、選択中にする
    /// </summary>
    private void OnClickCharaDetail() {
        // TODO GameDataに登録

        // 選択中のフレームを表示
        
        // 選択されなくなったキャラは非表示

        // SE

    }
}
