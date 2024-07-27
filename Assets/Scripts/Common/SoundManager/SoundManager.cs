#pragma warning disable 0414    // CS0414 警告の回避。変数をｐ利用していても利用していないという警告が出るため

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

/// <summary>
/// 音管理クラス
/// </summary>
public class SoundManager : MonoBehaviour, IEntryRun {

    public static SoundManager instance;
	
	// 音楽管理
	public enum ENUM_BGM : int{
        MATCHING,
        BATTLE,
        RESULT,
        MAIN,
	}

    // 効果音管理
    public enum ENUM_SE : int{
        RELEASE_OK,
        RELEASE_NG,
        DISCOVERY_TRAP,
    }

    // ボイス
    public enum ENUM_VOICE : int{
    }


    public enum BGM_TYPE {
        Menu,
        Stage_0,
        Stage_1,
        Stage_2,
    }


    public enum SE_TYPE {
        Erase,          // ブロック削除
        Submit,         // 決定
        Cancel,         // ブロック選択取り消し、キャンセル、戻る
        Block_Choose,   // ブロック選択時
        Miss,           // 障害物との接触時
        Fever,          // フィーバー時
        GameClear,
        GameOver,
        Excellent,
    }

    public enum VOICE_TYPE {
        挨拶_初回,
        挨拶_2回目以降,
        クリア_1,
        クリア_2,
        エクセレント,
        ゲームオーバー,
        フィーバー,
    }

    // クロスフェード時間
    public const float XFADE_TIME = 1.4f;

	// 音量
	public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    // BGM
    [SerializeField]
	private AudioSource[] BGMsources = new AudioSource[2];
	// SE
	private AudioSource[] SEsources = new AudioSource[24];
	// 音声
	private AudioSource[] VoiceSources = new AudioSource[8];
	
	// === AudioClip ===
	// BGM
	public BGMDatas[] BGM;
	// SE
	public AudioClip[] SE;
	// 音声
	public AudioClip[] Voice;

    // SE用AudioMixer
    [SerializeField]
    private AudioMixerGroup[] audioMixerGroups;

    bool isXFading = false;

    int currentBgmIndex = 999;

    public float masterVolume;


    [System.Serializable]
    public class BGMDatas {
        public AudioClip clip;
        public float loopTime;
        public float endTime;
    }


    /// <summary>
    /// ゲーム起動時の処理
    /// </summary>
    public void EntryRun() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init(ConstData.DEFAULT_MASTER_VOLUME);
        } else {
            Destroy(this.gameObject);
        }
        //Debug.Log("SoundManager Entry 終了");
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="newMasterVolume"></param>
    public void Init (float newMasterVolume) {
        //// BGM AudioSource  ->  SerializeField属性にてインスペクターで登録済
        //BGMsources[0] = gameObject.AddComponent<AudioSource>();
        //BGMsources[1] = gameObject.AddComponent<AudioSource>();

        // SE AudioSource
        for (int i = 0 ; i < SEsources.Length ; i++ ){
			SEsources[i] = gameObject.AddComponent<AudioSource>();
            SEsources[i].outputAudioMixerGroup = audioMixerGroups[0];

        }
		
		// 音声 AudioSource
		for(int i = 0 ; i < VoiceSources.Length ; i++ ){
			VoiceSources[i] = gameObject.AddComponent<AudioSource>();
            VoiceSources[i].outputAudioMixerGroup = audioMixerGroups[1];
        }
        // 初期音量設定
        SetMasterVolume(newMasterVolume);

        DOTween.Init();
        //Debug.Log("SoundManager Init");
    }

    void Update () {
        // ミュート設定
        //BGMsources[0].mute = volume.Mute;
        //BGMsources[1].mute = volume.Mute;
  //      foreach(AudioSource source in SEsources ){
		//	source.mute = volume.Mute;
		//}
		//foreach(AudioSource source in VoiceSources ){
		//	source.mute = volume.Mute;
		//}

        // ボリューム設定
  //      if(!isXFading) {
  //          //BGMsources[0].volume = volume.BGM;
  //          //BGMsources[1].volume = volume.BGM;
  //      }
  //      foreach(AudioSource source in SEsources ){
		//	source.volume = volume.SE;
		//}
		//foreach(AudioSource source in VoiceSources ){
		//	source.volume = volume.Voice;
		//}

        // Loop処理
        //if(currentBgmIndex != 999) {
            //if(BGM[currentBgmIndex].loopTime > 0f) {
            //    if(!BGMsources[0].mute && BGMsources[0].isPlaying && BGMsources[0].clip != null) {
            //        if(BGMsources[0].time >= BGM[currentBgmIndex].endTime) {
            //            BGMsources[0].time = BGM[currentBgmIndex].loopTime;
            //        }
            //    }
            //    if(!BGMsources[1].mute && BGMsources[1].isPlaying && BGMsources[1].clip != null) {
            //        if(BGMsources[1].time >= BGM[currentBgmIndex].endTime) {
            //            BGMsources[1].time = BGM[currentBgmIndex].loopTime;
            //        }
            //    }
            //}
        //}
    }

    //***** BGM再生 *****
    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmNo"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(ENUM_BGM bgmNo, bool loopFlg = true){
        int index = (int)bgmNo;
        currentBgmIndex = index;
        //if(PlayerPrefs.GetInt(Constant.BGM_FLG_NAME,1) == 1){
        if( 0 > index || BGM.Length <= index ){
				return;
			}
        // 同じBGMの場合は何もしない
        if(BGMsources[0].clip != null && BGMsources[0].clip == BGM[index].clip) {
            return;
        } else if(BGMsources[1].clip != null && BGMsources[1].clip == BGM[index].clip) {
            return;
        }
        // フェードでBGM開始
        if(BGMsources[0].clip == null && BGMsources[1].clip == null) {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].Play();
            //BGMsources[0].DOFade(gameData.volumeBgm, XFADE_TIME);
        } else {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// モザ消し用
    /// </summary>
    /// <param name="bgmNo"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BGM_TYPE bgmNo, bool loopFlg = true) {
        int index = (int)bgmNo;
        currentBgmIndex = index;
        //if(PlayerPrefs.GetInt(Constant.BGM_FLG_NAME,1) == 1){
        if (0 > index || BGM.Length <= index) {
            return;
        }
        // 同じBGMの場合は何もしない
        if (BGMsources[0].clip != null && BGMsources[0].clip == BGM[index].clip) {
            return;
        } else if (BGMsources[1].clip != null && BGMsources[1].clip == BGM[index].clip) {
            return;
        }
        // フェードでBGM開始
        if (BGMsources[0].clip == null && BGMsources[1].clip == null) {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].Play();
            //BGMsources[0].DOFade(gameData.volumeBgm, XFADE_TIME);
        } else {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// クロスフェード再生
    /// </summary>
    /// <param name="index"></param>
    /// <param name="loopFlg"></param>
    /// <returns></returns>
    private IEnumerator CrossfadeChangeBMG(int index, bool loopFlg) {
        isXFading = true;
        if(BGMsources[0].clip != null) {
            // 0がなっていて、1を新しい曲としてPlay
            BGMsources[1].volume = 0;
            BGMsources[1].clip = BGM[index].clip;
            BGMsources[1].loop = loopFlg;
            BGMsources[1].Play();
            BGMsources[0].DOFade(0, XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            BGMsources[1].DOFade(1, XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[0].Stop();
            BGMsources[0].clip = null;
        } else {
            // 1がなっていて、0を新しい曲としてPlay
            BGMsources[0].volume = 0;
            BGMsources[0].clip = BGM[index].clip;
            BGMsources[0].loop = loopFlg;
            BGMsources[0].Play();
            BGMsources[1].DOFade(0, XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            BGMsources[0].DOFade(1, XFADE_TIME).SetEase(Ease.Linear).SetLink(gameObject);
            yield return new WaitForSeconds(XFADE_TIME);
            BGMsources[1].Stop();
            BGMsources[1].clip = null;
        }
        isXFading = false;
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM(){
        BGMsources[0].Stop();
        BGMsources[1].Stop();
        //BGMsources[0].clip = null;
        //BGMsources[1].clip = null;
    }

    // ***** SE再生 *****
    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seNo"></param>
    public void PlaySE(ENUM_SE seNo){
		int index = (int)seNo;
		//if(PlayerPrefs.GetInt(Constant.SE_FLG_NAME,1) == 1){
			if( 0 > index || SE.Length <= index ){
				return;
			}
			
			// 再生中で無いAudioSouceで鳴らす
			foreach(AudioSource source in SEsources){
				if( false == source.isPlaying ){
					source.clip = SE[index];
					//volume.SE = gameData.volumeSe;
					source.Play();
					return;
				}
			}
        //}
    }

    /// <summary>
    /// モザ消し用
    /// </summary>
    /// <param name="seNo"></param>
    public void PlaySE(SE_TYPE seNo) {
        int index = (int)seNo;
        //if(PlayerPrefs.GetInt(Constant.SE_FLG_NAME,1) == 1){
        if (0 > index || SE.Length <= index) {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources) {
            if (false == source.isPlaying) {
                source.clip = SE[index];
                //volume.SE = gameData.volumeSe;
                source.Play();
                return;
            }
        }
        //}
    }

    /// <summary>
    /// SE停止
    /// </summary>
    public void StopSE(){
		// 全てのSE用のAudioSouceを停止する
		foreach(AudioSource source in SEsources){
			source.Stop();
			source.clip = null;
		}  
	}

    /// <summary>
    /// 指定した AudioGroup の音量変更
    /// Slider の値(0 - 1.0f)を AudioMixer のデジベルに変換して適用
    /// </summary>
    /// <param name="mixerGroupName"></param>
    /// <param name="linearVolume"></param>
    public void SetLinearVolumeToMixerGroup(string mixerGroupName, float linerVolume) {
        float decibel = 20.0f * Mathf.Log10(linerVolume);

        if (float.IsNegativeInfinity(decibel)) {
            decibel = -96f;  // 無音は -80f ではなくて -96f にする
        }

        audioMixerGroups[0].audioMixer.SetFloat(mixerGroupName, decibel);
    }

    /// <summary>
    /// 指定した AudioGroup の音量を float で取得
    /// </summary>
    /// <param name="mixerGroupName"></param>
    /// <returns></returns>
    public float GetLinearVolumeFromMixerGroup(string mixerGroupName) {
        float decibel;

        // Master
        audioMixerGroups[2].audioMixer.GetFloat(mixerGroupName, out decibel);

        return Mathf.Pow(10f, decibel / 20f);
    }


    // 使えなくはないが、上の方がよい
    //public float ConvertVolumeToDb(float sliderValue) {
    //    return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(sliderValue, 0f, 1f)) * 20f, -80f, 0f);
    //}

    //public void SetMasterVolume(float sliderValue) {
    //    audioMixerGroups[0].audioMixer.SetFloat("Master", ConvertVolumeToDb(sliderValue));
    //}

    ///// <summary>
    ///// AudioMixerのボリューム設定
    ///// </summary>
    ///// <param name="vol"></param>
    //public void SetAudioMixerVolume(float vol) {
    //    if(vol == 0) {
    //        audioMixerGroups.audioMixer.SetFloat("volumeSE", -80);
    //    } else {
    //        audioMixerGroups.audioMixer.SetFloat("volumeSE", 0);
    //    }
    //}

    /// <summary>
    /// モザ消し用
    /// </summary>
    /// <param name="voiceNo"></param>
    public void PlayVoice(VOICE_TYPE voiceNo) {
        int index = (int)voiceNo;

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in VoiceSources) {
            if (false == source.isPlaying) {
                source.clip = Voice[index];
                volume.Voice = 1.0f;
                source.Play();
                return;
            }
        }
    }

    /// <summary>
    /// マスターボリュームの設定値更新
    /// </summary>
    /// <param name="newVolume"></param>
    public void SetMasterVolume(float newVolume) {
        masterVolume = newVolume;
    }

    // ***** 音声再生 *****
    // 音声再生
    //public void PlayVoice(ENUM_VOICE voiceNo){
    //	int index = (int)voiceNo;
    //	if(PlayerPrefs.GetInt(Constant.VOICE_FLG_NAME,1) == 1){
    //		if( 0 > index || Voice.Length <= index ){
    //			return;
    //		}
    //		// 再生中で無いAudioSouceで鳴らす
    //		foreach(AudioSource source in VoiceSources){
    //			if( false == source.isPlaying ){
    //				source.clip = Voice[index];
    //				volume.Voice = gameData.volumeVoice;
    //				source.Play();
    //				return;
    //			}
    //		}
    //       }
    //   }

    //   // 音声停止
    //   public void StopVoice(){
    //	// 全ての音声用のAudioSouceを停止する
    //	foreach(AudioSource source in VoiceSources){
    //		source.Stop();
    //		source.clip = null;
    //	}  
    //}
}