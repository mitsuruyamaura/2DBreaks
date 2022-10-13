using UnityEngine;
using System.Collections.Generic;
using TNRD;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class EntryPoint : MonoBehaviour
{
    //public static EntryPoint instance;

    //public SoundManager soundManager;
    //public UserData userData;

    [SerializeField]
    private ResolutionFitter resolutionFitter;

    private bool isSetResolution;

    // 起動時に実行する各クラスの設定
    public List<SerializableInterface<IEntryRun>> entryList = new();

    //public float masterVolume;
    //private float defaultMasterVolume = 0.7f;


    void Awake() {
        //if (instance == null) {
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //    DOTween.Init();
        //} else {
        //    Destroy(gameObject);
        //}
        
        // 指定した順番に各クラスの初期設定を行う 
        foreach (var entry in entryList) {
            entry.Value?.EntryRun();
        }

        //if (SoundManager.instance == null) {
        //    SoundManager.instance = soundManager;
        //    DontDestroyOnLoad(soundManager.gameObject);
        //    SoundManager.instance.Init(defaultMasterVolume);
        //    masterVolume = defaultMasterVolume;
        //} else {
        //    Destroy(soundManager.gameObject);
        //}

        //if (UserData.instance == null) {
        //    UserData.instance = userData;
        //    DontDestroyOnLoad(userData.gameObject);
        //    UserData.instance.Init();
        //} else {
        //    Destroy(userData.gameObject);
        //}

        if (!isSetResolution && resolutionFitter) {
            resolutionFitter?.MeasureScreenSize();

            isSetResolution = true;
        }
    }

    private async UniTaskVoid Start() {
        if (SceneManager.GetActiveScene().name == SCENE_STATE.Start.ToString()) {
            StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));

            var token = this.GetCancellationTokenOnDestroy();
            //await UniTask.WaitUntil(() => SoundManager.instance.isSetUp, cancellationToken: token);
            await UniTask.Delay(1000, cancellationToken: token);  // あんまり遅いとゲームオブジェクトが破棄されて、下の処理に行かなくなる

            // セーブデータ有無の確認と取得、ボイス再生
            UserData.instance.Init();
        }
    }
}