using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liner : MonoBehaviour
{
    [Header("描画する線の親オブジェクト")]
    public GameObject lineParent;

    [Header("描画する線のプレファブ")]
    public GameObject linePrefab;

    [Header("引く線の最小の長さ")]
    public float lineLength;

    [Header("引く線の太さ")]
    public float lineWidth;

    [Header("線の出現時間")]
    public float duration;

    private Vector3 touchPos; //マウスのクリック地点

    public BallController ballController;

    public GameMaster gameMaster;

    private void Update()
    {

        if (gameMaster.gameState == GAME_STATE.STOP)
        {
            return;
        }

        if (gameMaster.gameState == GAME_STATE.GAME_OVER)
        {
            return;
        }

        //ボールが打ち出されたら線を引く処理を実行する
        if (ballController.isStart == true)
        {
            //いつでも線が引ける状態にしておく
            DrawLine();
        }

    
    }



    private void DrawLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            //マウスでクリックした位置を取る
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //使わない情報は0にする
            touchPos.z = 0;
        }



        if (Input.GetMouseButton(0))
        {
            //最初にクリックした位置をスタート地点にする
            Vector3 startPos = touchPos;

            //クリックしている間、その位置を監視し、離した地点を最後の地点にする
            Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //使わない情報は0にする
            endPos.z = 0;


            if ((endPos - startPos).magnitude > lineLength) //点と点を結んで.magnitudeで線にしてくれる
            {
                //マウスの動いた位置のベクトルの長さ(magnitude/float型)より、最小値より大きいなら線を生成
                GameObject obj = Instantiate(linePrefab, transform.position, transform.rotation) as GameObject;

                //生成地点より、transformの現在地を更新して線にする
            obj.transform.position = (startPos + endPos) / 2;

                //横方向に対して線を引く
                //ベクトルの方向を維持したまま、長さが1(正規化)のベクトルにする
                obj.transform.up = (endPos - startPos).normalized;

                //線の太さを設定
                obj.transform.localScale = new Vector3((endPos - startPos).magnitude, lineWidth, lineWidth);

                //線オブジェクトをこのクラスがアタッチされているオブジェクトの子にする
                //.parentは親の指定を命令する、Linerが親
                obj.transform.parent = lineParent.transform;

                //マウスの位置を更新
                //GetMouseButtonが続いている間、いつ指を離してもいいように現在地を更新している
                touchPos = endPos;

                //1...線を消す
                //2...生存時間
                Destroy(obj, duration);
        }

        }
    }
  
}
