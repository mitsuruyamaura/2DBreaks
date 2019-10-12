using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManeger : MonoBehaviour
{
    public TransitionManager transition;
    [Header("シーン遷移が終わるまでの時間")]
    public float fadeInTime;
    private bool isTouch;

  


    private void Update()
    {
        if (!isTouch && Input.GetMouseButtonDown(0))
        {
            isTouch = true; //一回だけタッチできる
            Debug.Log("タッチ");
            SceneManager.LoadScene("Menu");
            //タイトル画面でタップされたらコルーチンのメソッドを呼び出す
            //StartCoroutine(MoveMenuScene());

        }
    }



    /// <summary>
    /// フェイドアウトしながら次のシーンへ移る
    /// </summary>
    /// <returns></returns>

    //private IEnumerator MoveMenuScene()
    //{
        //TransitionManagerのフェイドアウト用のメソッドを呼び出す。引数はフェイドアウトの時間
        //transition.TransFadeOut(fadeInTime);

        //処理を一時停止してfadeInTimeだけ待機する
        //yield return new WaitForSeconds(fadeInTime);

        //次のMenuシーンへ遷移する
        //SceneManager.LoadScene("Menu");

    }
//}
