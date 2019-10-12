using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBlocks : MonoBehaviour
{
    [Header("ブロックのスタート地点")]
    public GameObject blockPos;
    [Header("画像なしのブロックのプレファブ")]
    public GameObject planeBlock;

    [Header("行")]
    public int line;
    [Header("列")]
    public int column;


    //画像ファイルの取得用設定値
    public string fileName;
    public string spriteName;

    public bool isDebugSwitch;
    public int debugBlockNum;

    //voidの位置に型を指定すると呼ばれた方に一つだけ変数を返せる
    public int CreateBlocks()
    {
        //設定用の全てのスプライト（画像）を読み込んでおく、Texturesのフォルダを参照する
        Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/" + fileName);

        //生成したブロックの数のカウント用
        int counter = 0;

        //行列を指定する
        for (int x = 0; x < line; x++)
        {
            for (int y = 0; y < column; y++)
            {
                //画像なしのブロックをBlockPosの持つ位置情報の位置に生成する
                GameObject block = Instantiate(planeBlock, planeBlock.transform.position, planeBlock.transform.rotation);

                block.transform.position = new Vector3((blockPos.transform.position.x + (0.735f * y)), (blockPos.transform.position.y + (-1.0f * x)));

                //取得している画像データの配列sprites[]から、counterと同じ要素数を参照してBlockの画像にする
                block.GetComponent<SpriteRenderer>().sprite = sprites[counter +90];

                //BlockをBlock_Fieldの子オブジェクトにする
                block.transform.parent = blockPos.transform;

                //生成が終わったので、カウントを一つ増やす
                counter++;

            }
        }

        //親に子を並べた後のサイズ調整(子の)を任せるイメージ
        blockPos.transform.localScale = new Vector3(0.5797f, 0.5751f, 1.0f);

        //並べたブロックの数をGameMasterにフィードバックしてクリア目標数(normaBlockNum)にする(戻り値を活用)
        if (isDebugSwitch == true)
        {
            return debugBlockNum;

        }
        else
        {
            return counter;
        }
    }
}

   
    
    

