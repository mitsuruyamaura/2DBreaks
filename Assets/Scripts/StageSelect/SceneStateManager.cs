using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(SceneType sceneType) {
        SceneManager.LoadScene(sceneType.ToString());
    }
}
