using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class Menu : MonoBehaviour, IEntryRun
{
    [SerializeField]
    private Text txtMozaicCount;

    [SerializeField]
    private Text txtGallery;

    [SerializeField]
    private Button btnGallery;

    [SerializeField]
    private CharaButtonDetail charaButtonPrefab;

    [SerializeField]
    private Transform[] charaButtonSetTrans;

    private List<CharaButtonDetail> charaButtonList = new();
    private BoolReactiveProperty sharedGate = new(true);　　　//　BindToOnClick にて利用する

    [SerializeField]
    private AchievementPopUp achievementPopUpPrefab;

    private AchievementPopUp achievementPopUp;

    [SerializeField]
    private Button btnAchieve;

    [SerializeField]
    private GalleryPopUp galleryPopUpPrefab;

    private GalleryPopUp galleryPopUp;


    /// <summary>
    /// ゲーム起動時の処理
    /// </summary>
    public void EntryRun() {
        // マスター音量の初期値設定
        SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, SoundManager.instance.masterVolume);
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.Menu);

        // モザイクカウントによるステージ開放の判定
        UserData.instance.CheckOpenStageFromPoint();

        // キャラボタンの生成
        CreateCharaButtons();

        // MozaicCount 購読
        UserData.instance.MosaicCount
            .Zip(UserData.instance.MosaicCount.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateDisplayMosaicCount(UserData.instance.beforePoint, x.newValue))   // 前の値との差分で表示更新する
            .AddTo(gameObject);

        // 初期値表示更新
        UserData.instance.MosaicCount.SetValueAndForceNotify(UserData.instance.MosaicCount.Value);

        // ギャラリーモード開放の確認。短絡評価して、どちらかが true であれば評価されるので、1つの目の評価が true なら2つ目にはいかないで終わる
        btnGallery.enabled = UserData.instance.CheckOpenGallaryPoint() || UserData.instance.CheckOpenGalleryAllNoMissClears();

        // ギャラリーモード未開放の場合
        if (!btnGallery.enabled) {
            txtGallery.text += "【全ステージ" + "\r\n" + "ノーミスクリアか" + "\r\n" + UserData.instance.openGallaryPoint + " で開放】";
        }

        // アチーブメントボタンの購読
        btnAchieve.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => PrepareAchievementPopUp())
            .AddTo(gameObject);

        // ギャラリーボタンの購読
        btnGallery.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => PrepareGalleryPopUp())
            .AddTo(gameObject);
    }

    //void Start()
    //{
    //    // マスター音量の初期値設定
    //    SoundManager.instance.SetLinearVolumeToMixerGroup(ConstData.MASTER_AUDIO_NAME, SoundManager.instance.masterVolume);
    //    SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.Menu);

    //    // モザイクカウントによるステージ開放の判定
    //    UserData.instance.CheckOpenStageFromPoint();

    //    // キャラボタンの生成
    //    CreateCharaButtons();

    //    // MozaicCount 購読
    //    UserData.instance.MosaicCount
    //        .Zip(UserData.instance.MosaicCount.Skip(1), (oldValue, newValue) => (oldValue, newValue))
    //        .Subscribe(x => UpdateDisplayMosaicCount(UserData.instance.beforePoint, x.newValue))   // 前の値との差分で表示更新する
    //        .AddTo(gameObject);

    //    // 初期値表示更新
    //    UserData.instance.MosaicCount.SetValueAndForceNotify(UserData.instance.MosaicCount.Value);

    //    // ギャラリーモード開放の確認。短絡評価して、どちらかが true であれば評価されるので、1つの目の評価が true なら2つ目にはいかないで終わる
    //    btnGallery.enabled = UserData.instance.CheckOpenGallaryPoint() || UserData.instance.CheckOpenGalleryAllNoMissClears();

    //    // ギャラリーモード未開放の場合
    //    if (!btnGallery.enabled) {
    //        txtGallery.text +=  "【全ステージノーミスクリアか" + "\r\n" + UserData.instance.openGallaryPoint + " で開放】";   
    //    }

    //    // アチーブメントボタンの購読
    //    btnAchieve.OnClickAsObservable()
    //        .ThrottleFirst(System.TimeSpan.FromSeconds(1))
    //        .Subscribe(_ => PrepareAchievementPopUp())
    //        .AddTo(gameObject);

    //    // ギャラリーボタンの購読
    //    btnGallery.OnClickAsObservable()
    //        .ThrottleFirst(System.TimeSpan.FromSeconds(2))
    //        .Subscribe(_ => PrepareGalleryPopUp())
    //        .AddTo(gameObject);
    //}

    /// <summary>
    /// MosaicCount の表示更新
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayMosaicCount(int oldValue, int newValue) {
        txtMozaicCount.DOCounter(oldValue, newValue, 2.0f).SetEase(Ease.InQuart).SetLink(gameObject);
    }

    /// <summary>
    /// キャラボタンの生成
    /// </summary>
    private void CreateCharaButtons() {
        for (int i = 0; i < UserData.instance.GetStageCount(); i++) {
            yamap.StageData stageData = UserData.instance.GetStageData(i);

            CharaButtonDetail charaButton = Instantiate(charaButtonPrefab, charaButtonSetTrans[i], false);
            charaButton.SetUpCharaButtonDetail(stageData.stageNo, stageData.charaIcon);

            // ロックされているステージの場合
            if (!UserData.instance.clearStageNoList.Contains(stageData.stageNo)) {
                // キャラをシルエット表示してボタンを押せない状態にする
                charaButton.LockCharaButton();

                // ステージ開放に必要なポイント表示
                charaButton.DisplayStageOpenPoint(stageData.stageOpenPoint);
            }

            // 複数のボタンを BoolReactiveProperty を購読して、１つのボタンに連動して制御できる
            // 内部で AsyncReactiveCommand が自動生成される。sharedGate が true なので、それが false になると、すべてのボタンの interactable に false の処理が届く
            charaButton.GetButton()
                .BindToOnClick(sharedGate, _ =>
                {
                    charaButton.OnClickCharaButton();

                    UserData.instance.beforePoint = UserData.instance.MosaicCount.Value;

                    // 5秒間押せないボタン
                    return Observable.Timer(System.TimeSpan.FromSeconds(5)).AsUnitObservable();
                });

            // 上の処理で、１つずつのボタンを制御しなくてもすべてのボタンを interactable にできる
            //// ボタンの購読
            //charaButton.GetButton().OnClickAsObservable()
            //    .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
            //    .Subscribe(_ => 
            //    {
            //        charaButton.OnClickCharaButton();

            //        // すべてのキャラボタンの非活性化
            //        InactiveAllCharaButtons();

            //        UserData.instance.beforePoint = UserData.instance.MosaicCount.Value;
            //    })
            //    .AddTo(gameObject);

            charaButtonList.Add(charaButton);
        }
        // ボタンは Subscribe していないので、ReactiveProperty の方を止める
        sharedGate.AddTo(gameObject);
    }

    /// <summary>
    /// すべてのキャラボタンを非活性化　←　不要
    /// </summary>
    public void InactiveAllCharaButtons() {
        for (int i = 0; i < charaButtonList.Count; i++) {
            charaButtonList[i].InactibeCharaButton();
        }
    }

    /// <summary>
    /// アチーブメントポップアップの生成とオープン
    /// </summary>
    private void PrepareAchievementPopUp() {
        // ポップアップが生成されていなければ
        if (!achievementPopUp) {
            // 生成して初期設定してから開く
            achievementPopUp = Instantiate(achievementPopUpPrefab);
            achievementPopUp.Setup();
        } else {
            // ポップアップを開く
            achievementPopUp.OpenPopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }

    /// <summary>
    /// ギャラリーポップアップの生成とオープン
    /// </summary>
    private void PrepareGalleryPopUp() {
        if (!galleryPopUp) {
            galleryPopUp = Instantiate(galleryPopUpPrefab);
            galleryPopUp.SetUp();
        } else {
            galleryPopUp.OpenPopup();
        }
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);
    }
}