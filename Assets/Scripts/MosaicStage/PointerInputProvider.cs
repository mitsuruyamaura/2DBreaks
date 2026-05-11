using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InputSystem のマウスとタップを VContainer の Tick 対応させたもの
/// Tick ではマウスクリックなどの情報が正しく拾えないので、Update で拾っておいたものを参照させる
/// </summary>
public class PointerInputProvider : MonoBehaviour {
    bool isPressDown;
    bool isPressUp;
    bool isPressing;

    public bool IsPressDown => isPressDown;
    public bool IsPressUp => isPressUp;
    public bool IsPressing => isPressing;


    void Update() {
        bool mouseDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseUp = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        bool mouseHold = Mouse.current != null && Mouse.current.leftButton.isPressed;

        bool touchDown = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        bool touchUp = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        bool touchHold = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

        if (Touchscreen.current != null) {
            // スマホの場合、1本指のみ有効
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed) {
                touchDown = touch.press.wasPressedThisFrame;
                touchUp = touch.press.wasReleasedThisFrame;
                touchHold = true;
            }
        }

        isPressDown = mouseDown || touchDown;
        isPressUp = mouseUp || touchUp;
        isPressing = mouseHold || touchHold;
    }

    /// <summary>
    /// 座標取得
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool TryGetPointerPosition(out Vector2 pos) {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed) {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.isPressed) {
            pos = Mouse.current.position.ReadValue();  // 従来の Input.mousePosition
            return true;
        }

        pos = default;
        return false;
    }
}