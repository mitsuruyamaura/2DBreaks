using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtGameTime;

    [SerializeField]
    private Image imgHp;

    [SerializeField]
    private Text txtPhaseStart;

    [SerializeField]
    private Text txtPhaseCount;

    [SerializeField]
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
        txtPhaseStart.text = "Phase " + currentPhaseCount.ToString() + "\n";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(phaseCountCanvasGroup.DOFade(1.0f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        sequence.Append(txtPhaseStart.DOText("Ready", 0.5f).SetEase(Ease.Linear));
        yield return new WaitForSeconds(0.5f);
        txtPhaseStart.text = "GO!!";

        sequence.Append(phaseCountCanvasGroup.DOFade(0f, 0.5f));
    }
}
