using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 各ステージの開始時と終了時のトランジション用クラス
/// フェードイン中に画面が見えないようにカメラとUIは非アクティブにしておく
/// </summary>
public class TransitionManager : MonoBehaviour {

    [SerializeField, Header("フェイドイン／フェイドアウト制御用")]
    public Fade fade;
    [SerializeField, Header("マスク用イメージ制御用")]
    public GameObject maskImage;

    private bool isSet;

    public float startWaitTime;

    void Start () {
        //現在のシーンがTitleなら処理をしない
        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }
        TransFadeIn(startWaitTime);       
    }

    /// <summary>
    /// 各ステージの開始時のフェイドイン処理
    /// フェイドインが終わってからゲーム画面関連を表示させる
    /// </summary>
    public void TransFadeIn(float time) {
        fade.FadeIn(0.1f, () =>
        {
            if (!isSet) {
                isSet = true;
                SetUp();
            }
            fade.FadeOut(time);
        });     
    }

    /// <summary>
    /// カメラなどを有効化する処理
    /// </summary>
    private void SetUp() {
        maskImage.SetActive(false);
    }

    /// <summary>
    /// 各ステージ終了時のフェイドアウト処理
    /// </summary>
    /// <param name="time"></param>
	public void TransFadeOut(float time) {
        fade.FadeIn(0.7f, () =>
        {
            fade.FadeOut(time);
        });
    }
}
