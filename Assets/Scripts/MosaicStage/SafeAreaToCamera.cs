using UnityEngine;

public class SafeAreaToCamera : MonoBehaviour {
    private Camera mainCamera;

    private void Reset() {
        TryGetComponent(out mainCamera);
    }

    void Start() {
        TryGetComponent(out mainCamera);
        ApplySafeAreaToCamera(mainCamera);
    }
    void ApplySafeAreaToCamera(Camera cam) {
        Rect safe = Screen.safeArea;

        Rect rect = new Rect(
            safe.x / Screen.width,
            safe.y / Screen.height,
            safe.width / Screen.width,
            safe.height / Screen.height
        );

        cam.rect = rect;
    }
}