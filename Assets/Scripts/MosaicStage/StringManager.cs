using UnityEngine;

public class StringManager : MonoBehaviour {
    public static StringManager instance;

    public LocalizeDataSO LocalizeDataSO;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tag に該当する LocalizeData を取得
    /// </summary>
    /// <param name="searchTag"></param>
    /// <returns></returns>
    public LocalizeData GetLocalizeData(string searchTag) {
        //DebugLogger.Log("searchTag : " + searchTag);
        if (searchTag.Trim() == "") {
            return new LocalizeData { key = "", jp = "no base tag", en = "no base tag" };
        }
        return LocalizeDataSO.localizeDataList.Find(data => data.key == searchTag);
    }

    /// <summary>
    /// StringMaster から現在選択中の言語の Text を取得
    /// </summary>
    /// <param name="localizeData"></param>
    /// <returns></returns>
    public string GetCurrentLanguageText(LocalizeData localizeData) {
        return UserData.instance.CurrentLanguage.Value switch {
            Language.jp => localizeData.jp,
            Language.en => localizeData.en,
            _ => localizeData.jp,
        };
    }
}