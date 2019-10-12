using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Ugokimawaru : MonoBehaviour
{
    [SerializeField, Header("キャラの速度")]
    public float speed;

    public int myStageNo;
    public TransitionManager transitionManager;
    public float endWaitTime; // <= TransitionManagerでの内部設定値は0.7f

    [Header ("RigidBodyを入れる")]
    public Rigidbody2D rb; //コンポーネントの格納用

    private Vector2 direction; //ボールの方向

    private bool isTapFlag; //初めはfalse



    void Start()
    {
        MoveChara();
    }

    private void MoveChara()
    {
        direction = new Vector2(Random.Range(-4.5f, 4.5f), 1).normalized;

        rb.velocity = direction * speed * transform.localScale.x;

    }


    public void OnClickChara()
    {
        //! falseかの確認が入る
        //外側でタップの判定を取っている
        if (!isTapFlag)
        {
            isTapFlag = true; //分岐をかけるので連打されても一回だけしかコルーチンが走らない

            //コルーチンを使ってステージ遷移用のメソッドを呼び出す
            //以前ここで処理していたステージ遷移処理もメソッド内に移す
            StartCoroutine(MoveStageScene());

        }
    }


    private IEnumerator MoveStageScene()
    {
        //1. キャラに設定したステージの番号を番号をStaticクラスのstatic変数に渡す　＝次のシーンでもこの値を参照できる
        SelectStage.stageNo = myStageNo;
        //上記の確認
        Debug.Log(SelectStage.stageNo);

        //2. トランジション用のメソッドを呼び出して画面にマスクをかけてフェイドアウトする
        transitionManager.TransFadeOut(endWaitTime);

        //3. トランジションが終わるまで待機する時間　＝ endWaitTimeをつかってフェイドアウト時間と合わせる
        yield return new WaitForSeconds(endWaitTime);

        SceneManager.LoadScene("Stage");

    }


}
