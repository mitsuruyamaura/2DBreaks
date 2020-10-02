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
    /// Phase数表示
    /// </summary>
    /// <returns></returns>
    public IEnumerator DispayPhaseCount(int currentPhaseCount) {
        txtPhaseCount.text = "Phase " + currentPhaseCount.ToString() + "\n";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(phaseCountCanvasGroup.DOFade(1.0f, 0.5f));
        
        txtPhaseCount.text += sequence.Append(txtPhaseCount.DOText("Ready", 0.5f).SetEase(Ease.Linear));
        yield return new WaitForSeconds(0.5f);
        txtPhaseCount.text = "GO!!";
    }
}
