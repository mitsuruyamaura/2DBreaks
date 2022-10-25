using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// TileGrid �̐����E�Ȃ������O���b�h�̍폜��A�c�����O���b�h�̊Ǘ��p�N���X
/// </summary>
public class TileGridBehaviour : MonoBehaviour
{
    [SerializeField]
    private TileGridDetail tileGridPrefab;

    [SerializeField]
    private GameObject eraseEffectPrefab;

    [SerializeField]
    private Transform tileGridSetTran;

    [SerializeField, Header("�s")]
    private int rowCount;

    [SerializeField, Header("��")]
    private int columnCount;

    [SerializeField]
    private float tileGridSize;

    [SerializeField, Header("�������ꂽ Grid �̃��X�g")]
    public List<TileGridDetail> tileGridList = new();

    public ReactiveCollection<TileGridDetail> TileGridList = new();

    private bool isLastColor;


    public Transform GetTileGridSetTran() => tileGridSetTran;
    public GameObject GetEraseEffect() => eraseEffectPrefab;
    public float GetTileGridSize() => tileGridSize;


    /// <summary>
    /// �O���b�h�𐶐�
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public void CreateTileGrids() {
        for (int i = 0; i < rowCount; i++) {
            for (int j = 0; j < columnCount; j++) {
                // TileGridDetail �v���t�@�u�𐶐�
                TileGridDetail tileGrid = Instantiate(tileGridPrefab, tileGridSetTran, true);

                // �T�C�Y�ύX�ƈʒu�ύX���ĕ��ׂ�
                tileGrid.transform.localScale = Vector3.one * tileGridSize;
                tileGrid.transform.localPosition = new(j * tileGridSize, i * tileGridSize, 0);

                // �O���b�h�̏����ݒ�B�O���b�h�̐F�������_���ɂP�I��
                tileGrid.SetUpTileGridDetail(UnityEngine.Random.Range(0, (int)TileGridType.Count));

                tileGridList.Add(tileGrid);
            }
        }

        TileGridList = new(tileGridList);
    }

    /// <summary>
    /// �Ȃ����Ă���O���b�h�Q���܂Ƃ߂č폜
    /// </summary>
    /// <param name="eraseTileGridList"></param>
    public void EraseTileGrids(List<TileGridDetail> eraseTileGridList) {

        for (int i = 0; i < eraseTileGridList.Count; i++) {

            // ���X�g�����菜��
            tileGridList.Remove(eraseTileGridList[i]);
            TileGridList.Remove(eraseTileGridList[i]);

            // �G�t�F�N�g�̃v���t�@�u���p�ӂ���Ă���ꍇ
            if (eraseEffectPrefab) {
                //Debug.Log("�G�t�F�N�g����");
                // �G�t�F�N�g����
                GameObject effect = Instantiate(eraseEffectPrefab, eraseTileGridList[i].gameObject.transform);
                effect.transform.SetParent(tileGridSetTran);
                Destroy(effect, 0.6f);
            }

            // �O���b�h���폜
            Destroy(eraseTileGridList[i].gameObject);
        }
    }

    /// <summary>
    /// �l�܂Ȃ��悤�Ɏc��̃O���b�h�����Ȃ��Ȃ������m�F����
    /// </summary>
    public void CheckLastColor() {
        if (!isLastColor && tileGridList.Count < 12) {
            isLastColor = true;

            // �c�����O���b�h�̐F���P�F�ɕς���
            ChangeTileGridsColor();
        }
    }

    /// <summary>
    /// �c�����O���b�h�̐F���P�F�ɕς���
    /// �c�肪�w�萔�ȉ��ɂȂ����Ƃ��ɗ��p���āA�Q�[�����l�ޏ�Ԃ��Ȃ���
    /// </summary>
    private void ChangeTileGridsColor() {
        // ���ȊO�ɂ���
        int randomColorNo = UnityEngine.Random.Range(0, (int)TileGridType.��);
        for (int i = 0; i < tileGridList.Count; i++) {
            tileGridList[i].SetTileGridTile(randomColorNo);
            tileGridList[i].SetColor(randomColorNo);
        }

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);
    }

    /// <summary>
    /// �c���Ă���O���b�h�����ׂč폜
    /// </summary>
    public void AllEraseTileGird() {
        for (int i = 0; i < tileGridList.Count; i++) {
            Destroy(tileGridList[i].gameObject);
        }
        tileGridList.Clear();
        TileGridList.Clear();
    }
}