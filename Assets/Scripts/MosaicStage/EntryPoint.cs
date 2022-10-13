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

    // �N�����Ɏ��s����e�N���X�̐ݒ�
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
        
        // �w�肵�����ԂɊe�N���X�̏����ݒ���s�� 
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
            await UniTask.Delay(1000, cancellationToken: token);  // ����܂�x���ƃQ�[���I�u�W�F�N�g���j������āA���̏����ɍs���Ȃ��Ȃ�

            // �Z�[�u�f�[�^�L���̊m�F�Ǝ擾�A�{�C�X�Đ�
            UserData.instance.Init();
        }
    }
}