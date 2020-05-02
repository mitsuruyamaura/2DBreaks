using UnityEngine;

/// <summary>
/// ブロックを生成するクラス
/// </summary>
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

    [Header("参照する画像ファイル名")]
    public string fileName;

    [Header("デバッグ用スイッチ")]
    public bool isDebugSwitch;
    [Header("デバッグスイッチ true ならブロック数をこの値に変更する")]
    public int debugBlockNum;

    /// <summary>
    /// 空のブロックを生成し、イメージを画像ファイルから参照してアサインし、それらを順番に並べる
    /// </summary>
    /// <returns></returns>
    public int CreateBlocks() {
        // 選択されたステージに必要な全ての画像を読み込む
        Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/" + fileName);

        // 生成したブロックの数のカウント用
        int counter = 0;

        // 行列を指定
        for (int x = 0; x < line; x++) {
            for (int y = 0; y < column; y++) {
                // 画像なしのブロックをBlockPosの持つ位置情報の位置に生成
                GameObject block = Instantiate(planeBlock, planeBlock.transform.position, planeBlock.transform.rotation);
                block.transform.position = new Vector3((blockPos.transform.position.x + (0.735f * y)), (blockPos.transform.position.y + (-1.0f * x)));

                // 取得している画像データの配列sprites[]から、counterと同じ要素数を参照してBlockの画像にする
                block.GetComponent<SpriteRenderer>().sprite = sprites[counter +90];

                // BlockをBlock_Fieldの子オブジェクトにする
                block.transform.parent = blockPos.transform;

                // 生成が終わったので、カウントを一つ増やす
                counter++;
            }
        }

        // 親のサイズ調整
        blockPos.transform.localScale = new Vector3(0.5797f, 0.5751f, 1.0f);

        // デバッグモードでないなら並べたブロックの数をGameMasterに戻し、クリアノルマにする
        if (isDebugSwitch == true) {
            return debugBlockNum;
        }
        else {
            return counter;
        }
    }
}

   
    
    

