using UnityEngine;

[System.Serializable]
public class LocalizeData {
    public string key;
    [TextArea] public string jp;  // インスペクターで複数行のテキストを入力できるようにするため、TextArea 属性を付与
    [TextArea] public string en;
}