using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> iconRemainingBallList = new List<GameObject>();

    [SerializeField]
    private GameObject iconRemainingBallPrefab;

    [SerializeField]
    private Transform remainingBallTran;

    [SerializeField]
    private Image imgTimeGauge;

    [SerializeField]
    private Text txtBattleTime;

    [SerializeField]
    private Text txtMoney;

    [SerializeField]
    private Text txtStageInfo;

    [SerializeField]
    private CanvasGroup canvasGroupStageInfo;

    [SerializeField]
    private Text txtPhaseCount;



    /// <summary>
    /// 手球の残数を画面上に生成
    /// </summary>
    /// <param name="ballCount"></param>
    /// <returns></returns>
    public IEnumerator GenerateIconRemainingBalls(int ballCount) {
        //yield return null;
        for (int i = 0; i < ballCount; i++) {
            yield return new WaitForSeconds(0.15f);
            GameObject icon = Instantiate(iconRemainingBallPrefab, remainingBallTran, false);
            iconRemainingBallList.Add(icon);
        }
    }

    /// <summary>
    /// 手球の残数の表示を更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateDisplayIconRemainingBall(int amount) {

        // 手球の最大値が増える場合
        if (iconRemainingBallList.Count < amount) {
       　　　// 差分だけ手球のアイコンを作成
            int value = amount - iconRemainingBallList.Count;
            for (int i = 0; i < value; i++) {
                GameObject icon = Instantiate(iconRemainingBallPrefab, remainingBallTran, false);
                iconRemainingBallList.Add(icon);
            }
        }

        for (int i = 0; i < iconRemainingBallList.Count; i++) {
            if (i < amount) {
                iconRemainingBallList[i].SetActive(true);
            } else {
                iconRemainingBallList[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// ゲーム時間の表示を更新
    /// </summary>
    public void UpdateDisplayBattleTime(int currentTime) {
        float value = (float)currentTime / GameData.instance.battleTime;
        imgTimeGauge.DOFillAmount(value, 1.0f).SetEase(Ease.Linear);
        txtBattleTime.text = currentTime.ToString();
    }

    /// <summary>
    /// 現在までに獲得しているMoneyの表示を更新
    /// </summary>
    public void UpdateDisplayMoney(int money) {
        txtMoney.text = money.ToString();
    }

    /// <summary>
    /// ステージクリア表示
    /// </summary>
    public void DisplayStageClear() {
        txtStageInfo.transform.localScale = Vector3.one * 5;
        txtStageInfo.text = "Stage Clear!!";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupStageInfo.DOFade(1.0f, 0.5f));

        sequence.Join(txtStageInfo.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        sequence.Append(txtStageInfo.transform.DOScale(Vector3.one * 1.25f, 0.15f));
        sequence.Append(txtStageInfo.transform.DOScale(Vector3.one, 0.15f));
    }

    /// <summary>
    /// ゲームオーバー表示
    /// </summary>
    public void DisplayGameOver() {
        canvasGroupStageInfo.DOFade(1.0f, 0.5f);
        txtStageInfo.DOText("GameOver...", 1.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// Phase数の表示を更新(現在フェーズ数 / 最大フェーズ数)
    /// </summary>
    /// <param name="currentPhaseCount"></param>
    public void UpdateDisplayPhaseCount(int currentPhaseCount, int maxPhaseCount) {
        txtPhaseCount.text = currentPhaseCount.ToString();
        txtPhaseCount.text += "/" + maxPhaseCount;
    }

    /// <summary>
    /// Phaseスタート表示
    /// </summary>
    /// <returns></returns>
    public IEnumerator DispayPhaseStart(int currentPhaseCount) {        
        txtStageInfo.text = "Phase " + currentPhaseCount.ToString() + "\n";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroupStageInfo.DOFade(1.0f, 0.5f));
        
        yield return new WaitForSeconds(1.5f);
        sequence.Append(txtStageInfo.DOText("", 0f).SetEase(Ease.Linear));
        sequence.Append(txtStageInfo.DOText("Ready", 1.0f).SetEase(Ease.Linear));
        sequence.Append(txtStageInfo.DOText("", 0f).SetEase(Ease.Linear));
      
        yield return new WaitForSeconds(1.5f);
        sequence.Append(txtStageInfo.DOText("GO!!", 0f).SetEase(Ease.Linear));
        sequence.Join(txtStageInfo.transform.DOScale(new Vector3(3.0f, 3.0f, 3.0f), 1.0f).SetEase(Ease.Linear));
        sequence.Join(canvasGroupStageInfo.DOFade(0f, 1.0f));

        yield return new WaitForSeconds(1.5f);
        txtStageInfo.transform.localScale = Vector3.one;
    }
}
