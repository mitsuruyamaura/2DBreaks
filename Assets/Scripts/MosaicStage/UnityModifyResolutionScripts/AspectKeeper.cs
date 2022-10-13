using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ばこさんの記事
// https://3dunity.org/game-create-lesson/clicker-game/mobile-adjustment/
// Free Aspect で確認する

[ExecuteAlways]  // https://docs.unity3d.com/ScriptReference/ExecuteAlways.html  // Prefab 編集モードや、エディター再生が終わったときに実行されるようになる
public class AspectKeeper : MonoBehaviour {

    [SerializeField]
    private Camera targetCamera; //対象とするカメラ

    [SerializeField]
    private Vector2 aspectVec; //目的解像度

    void Update() {
        var screenAspect = Screen.width / (float)Screen.height; //画面のアスペクト比
        var targetAspect = aspectVec.x / aspectVec.y; //目的のアスペクト比

        var magRate = targetAspect / screenAspect; //目的アスペクト比にするための倍率

        var viewportRect = new Rect(0, 0, 1, 1); //Viewport初期値でRectを作成

        if (magRate < 1) {
            viewportRect.width = magRate; //使用する横幅を変更
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;//左端に寄せられてしまうので、0.5 を使い、中央寄せ
        } else {
            viewportRect.height = 1 / magRate; //使用する縦幅を変更(横幅を縮めると、縦幅がはみ出す(1を超える)ため調整)
            viewportRect.y = 0.5f - viewportRect.height * 0.5f;//下に寄せられてしまうので、0.5 を使い、中央寄せ
        }

        targetCamera.rect = viewportRect; //カメラのViewportに適用
    }
}