using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public CharaSelectPopUp charaSelectPopUpPrefab;
    private CharaSelectPopUp charaSelectPopUp;

    public Button[] btnStages;

    public Transform canvasTran;

    void Awake() {
        // 各ステージのアイコンボタンにメソッド登録
        for (int i = 0; i < btnStages.Length; i++) {
            btnStages[i].onClick.AddListener(() => OnClickStage(i));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageNo"></param>
    private void OnClickStage(int stageNo) {
        // 選択したステージ番号を登録
        GameData.instance.chooseStageNo = stageNo;

        // 一度もステージ選択をしていない場合
        if (charaSelectPopUp == null) {
            // キャラ選択ポップアップを生成
            charaSelectPopUp = Instantiate(charaSelectPopUpPrefab, canvasTran, false);
            charaSelectPopUp.SetUpCharaSelectPopUp(this);
        }

        // 生成されている場合にはキャラ選択ポップアップを表示
        ActivateCharaSelectPopUp(1.0f, true);
    }

    /// <summary>
    /// キャラ選択ポップアップの表示/非表示切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void ActivateCharaSelectPopUp(float alpha, bool isSwitch) {
        // 画面のタップを制御
        charaSelectPopUp.canvasGroup.blocksRaycasts = isSwitch;
        charaSelectPopUp.canvasGroup.DOFade(alpha, 1.0f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// バトルシーンへの遷移
    /// </summary>
    /// <returns></returns>
    public IEnumerator PraparateBattleScene() {
        // TODO トランジション処理追加

        SceneStateManager.instance.ChangeScene(SceneType.Battle);
        yield break;
    }
}
