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


    // 未

    [SerializeField, HideInInspector]
    private Text txtGameTime;

    [SerializeField, HideInInspector]
    private Text txtMoney;

    [SerializeField, HideInInspector]
    private Image imgHp;

    [SerializeField, HideInInspector]
    private Text txtStageInfo;

    [SerializeField, HideInInspector]
    private Text txtPhaseCount;

    [SerializeField, HideInInspector]
    private CanvasGroup phaseCountCanvasGroup;


    /// <summary>
    /// ゲーム時間の表示を更新
    /// </summary>
    public void UpdateDisplayGameTime(int currentTime) {
        float value = (float)currentTime / GameData.instance.battleTime;
        imgHp.DOFillAmount(value, 1.0f).SetEase(Ease.Linear);
        txtGameTime.text = currentTime.ToString();
    }

    /// <summary>
    /// Moneyの表示を更新
    /// </summary>
    public void UpdateDisplayMoney() {
        txtMoney.text = GameData.instance.money.ToString();
    }

    /// <summary>
    /// Phase数の表示を更新
    /// </summary>
    /// <param name="currentPhaseCount"></param>
    public void UpdateDisplayPhaseCount(int currentPhaseCount, int maxPhaseCount) {
        txtPhaseCount.text = currentPhaseCount.ToString();
        txtPhaseCount.text += " / " + maxPhaseCount.ToString();
    }

    /// <summary>
    /// Phaseスタート表示
    /// </summary>
    /// <returns></returns>
    public IEnumerator DispayPhaseStart(int currentPhaseCount) {        
        txtStageInfo.text = "Phase " + currentPhaseCount.ToString() + "\n";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(phaseCountCanvasGroup.DOFade(1.0f, 0.5f));
        
        yield return new WaitForSeconds(1.5f);
        sequence.Append(txtStageInfo.DOText("", 0f).SetEase(Ease.Linear));
        sequence.Append(txtStageInfo.DOText("Ready", 1.0f).SetEase(Ease.Linear));
        sequence.Append(txtStageInfo.DOText("", 0f).SetEase(Ease.Linear));
      
        yield return new WaitForSeconds(1.5f);
        sequence.Append(txtStageInfo.DOText("GO!!", 0f).SetEase(Ease.Linear));
        sequence.Join(txtStageInfo.transform.DOScale(new Vector3(3.0f, 3.0f, 3.0f), 1.0f).SetEase(Ease.Linear));
        sequence.Join(phaseCountCanvasGroup.DOFade(0f, 1.0f));

        yield return new WaitForSeconds(1.5f);
        txtStageInfo.transform.localScale = Vector3.one;
    }

    public void DisplayGameOver() {
        phaseCountCanvasGroup.DOFade(1.0f, 0.5f);
        txtStageInfo.DOText("GameOver...", 1.5f).SetEase(Ease.Linear);
    }

    public void DisplayStageClear() {
        phaseCountCanvasGroup.DOFade(1.0f, 0.5f);
        txtStageInfo.DOText("Stage Clear!!", 1.5f).SetEase(Ease.Linear);
    }

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
        for (int i = 0; i < iconRemainingBallList.Count; i++) {
            if (i < amount) {
                iconRemainingBallList[i].SetActive(true);
            } else {
                iconRemainingBallList[i].SetActive(false);
            }
        }
    }
}
