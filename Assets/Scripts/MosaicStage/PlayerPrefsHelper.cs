using UnityEngine;

/// <summary>
/// 指定したクラスを string 型の Json 形式で PlayerPrefs クラスにセーブ・ロードするためのヘルパークラス
/// </summary>
public static class PlayerPrefsHelper
{
    /// <summary>
    /// 指定したキーのデータが存在しているか確認
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistsData(string key) {

        // 指定したキーのデータが存在しているか確認して、存在している場合は true 、存在していない場合には false を戻す
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをセーブ
    /// </summary>
    /// <typeparam name="T">セーブする型</typeparam>
    /// <param name="key">データを識別するためのキー</param>
    /// <param name="obj">セーブする情報</param>
    public static void SaveSetObjectData<T>(string key, T obj) {

        // オブジェクトのデータを Json 形式に変換
        string json = JsonUtility.ToJson(obj);

        // セット
        PlayerPrefs.SetString(key, json);

        // セットした Key と json をセーブ
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをロード
    /// </summary>
    /// <typeparam name="T">ロードする型</typeparam>
    /// <param name="key">データを識別するためのキー</param>
    /// <returns></returns>
    public static T LoadGetObjectData<T>(string key) {

        // セーブされているデータをロード
        string json = PlayerPrefs.GetString(key);

        // 読み込む型を指定して変換して取得
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 指定されたキーのデータを削除
    /// </summary>
    /// <param name="key"></param>
    public static void RemoveObjectData(string key) {

        // 指定されたキーのデータを削除
        PlayerPrefs.DeleteKey(key);

        Debug.Log("セーブデータを削除　実行 : " + key);
    }

    /// <summary>
    /// すべてのセーブデータを削除
    /// </summary>
    public static void AllClearSaveData() {

        // すべてのセーブデータを削除
        PlayerPrefs.DeleteAll();

        Debug.Log("全セーブデータを削除　実行");
    }
}
