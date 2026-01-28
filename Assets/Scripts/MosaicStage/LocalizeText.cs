using UniRx;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocalizeText : MonoBehaviour {
    [SerializeField] private Text txtLocalize;

#pragma warning disable 0108
    [SerializeField] private string tag;
#pragma warning restore 0108

    public string Tag => tag;

    // マスターデータをキャッシュ
    [SerializeField] private LocalizeData localizeData;  // デバッグ終わったら SerializeField 外す
    public LocalizeData LocalizeData => localizeData;

    UnityAction changeLanguageAction;
    IDisposable disposable;


    void Reset() {
        TryGetComponent(out txtLocalize);
    }

    void Start() {
        // tag が登録されていない場合には処理しない
        if (string.IsNullOrEmpty(tag)) {
            return;
        }

        LocalizeData tempLocalizeData = StringManager.instance.GetLocalizeData(tag);

        if (tempLocalizeData != null) {
            // Tag からマスターデータ取得してキャッシュ(tag 内の全言語分キャッシュ済み)
            localizeData = StringManager.instance.GetLocalizeData(tag);
        } else {
            Debug.Log($"ローカライズデータが取得できませんでした。: {tag}");
            localizeData = null;
        }

        // 初期の言語監視時のデリゲートを登録
        if (changeLanguageAction == null) {
            SetChangeLanguageAciton(UpdateLocalizeText);
        }

        // 言語監視
        disposable = UserData.instance.CurrentLanguage.Subscribe(language => changeLanguageAction?.Invoke()).AddTo(this);
    }

    /// <summary>
    /// 言語切り替え時のデリゲート登録用
    /// 初期は UpdateLocalizeText メソッドが登録される
    /// </summary>
    /// <param name="languageAction"></param>
    public void SetChangeLanguageAciton(UnityAction languageAction) {
        changeLanguageAction = languageAction;
    }

    /// <summary>
    /// 指定した言語による表示の切替
    /// </summary>
    /// <param name="newLanguage"></param>
    private void UpdateLocalizeText() {
        if (localizeData == null) {
            Debug.Log($"ローカライズデータが存在しません。: {tag}");
            txtLocalize.text = "no key";
            return;
        }
        string newLangaugeText = StringManager.instance.GetCurrentLanguageText(localizeData);
        txtLocalize.text = newLangaugeText;
    }

    /// <summary>
    /// 文字設定
    /// 直接表示を切り替える時に利用
    /// </summary>
    /// <param name="newMessage"></param>
    public void SetMessageText(string newMessage) {
        txtLocalize.text = newMessage;
    }

    /// <summary>
    /// 外部からのローカライズデータのセット用
    /// インスペクターで Tag が未登録の場合(ゲーム内で動的に Tag を設定する場合)に利用する
    /// </summary>
    /// <param name="newLocalizeData"></param>
    public void SetLocalizeData(LocalizeData newLocalizeData) {
        if (newLocalizeData == null) {
            // ローカライズデータが存在しない
            localizeData = new LocalizeData { key = "", jp = "no string data", en = "no string data" };
        } else {
            localizeData = newLocalizeData;
        }
        UpdateLocalizeText();
    }


    public void SetTextColor(Color newColor) {
        txtLocalize.color = newColor;
    }

    /// <summary>
    /// 現在表示中のメッセージを取得
    /// </summary>
    /// <returns></returns>
    public string GetMessage() {
        return txtLocalize.text;
    }

    private void OnDestroy() {
        disposable?.Dispose();
    }
}