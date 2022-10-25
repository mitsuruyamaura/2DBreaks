#pragma warning disable 0649
#pragma warning disable 0414

using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer.Unity;
using System;
using DG.Tweening;

/// <summary>
/// �O���b�h���Ȃ�����A�Ȃ����Ă���O���b�h���v�Z���Đ��䂵�āATileGridBehaivour �ɒ񋟂���N���X
/// </summary>
public class GridCalculator : ITickable, IDisposable {

    private MainGameManager mainGameManager;
    private TileGridBehaviour tileGridBehaviour;

    private TileGridDetail firstSelectTileGrid;
    private TileGridDetail lastSelectTileGrid;
    private TileGridType? currentTileGridType;

    [SerializeField]
    private List<TileGridDetail> eraseTileGridList = new();

    private int linkCount = 0;
    private float tileGridDistance = 1000.0f;

    private CompositeDisposable disposables;


    public GridCalculator(MainGameManager mainGameManager, TileGridBehaviour tileGridBehaviour) {
        this.mainGameManager = mainGameManager;
        this.tileGridBehaviour = tileGridBehaviour;
        //Debug.Log(this.mainGameManager);

        disposables = new();
    }

    void IDisposable.Dispose() => disposables.Dispose();


    /// <summary>
    /// Presenter ���� Entry ������
    /// </summary>
    void ITickable.Tick() {
        if (mainGameManager == null) {
            return;
        }

        mainGameManager.State
            .Where(state => state == GameState.Play)
            .Subscribe(_ => {
                // �O���b�h���Ȃ��鏈��
                if (Input.GetMouseButtonDown(0) && firstSelectTileGrid == null) {
                    OnStartDrag();
                } else if (Input.GetMouseButtonUp(0)) {
                    OnEndDrag();
                } else if (firstSelectTileGrid != null) {
                    OnDragging();
                }
                mainGameManager.GameTime.Value += Time.deltaTime;
            })
            .AddTo(disposables);
    }

    /// <summary>
    /// �O���b�h���ŏ��Ƀh���b�O�����ۂ̏���
    /// </summary>
    private void OnStartDrag() {
        //Debug.Log("�h���b�O�J�n");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); //  Camera.main.ScreenToWorldPoint

        // �O���b�h���Ȃ����Ă��鐔��������
        linkCount = 0;

        if (hit.collider != null) {
            if (hit.collider.gameObject.TryGetComponent(out TileGridDetail startTileGrid)) {
                firstSelectTileGrid = startTileGrid;
                lastSelectTileGrid = startTileGrid;
                currentTileGridType = startTileGrid.tileGridType;

                startTileGrid.IsSelected = true;
                startTileGrid.Num = linkCount;

                eraseTileGridList = new();
                AddEraseTileGridlList(startTileGrid);

                SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Block_Choose);
            }
        }
    }

    /// <summary>
    /// �O���b�h�̃h���b�O���i�X���C�v�j����
    /// </summary>
    private void OnDragging() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out TileGridDetail dragTileGrid)) {

            // TileGrid �ȊO�̏ꏊ�̏ꍇ�ɂ͉������Ȃ�
            if (currentTileGridType == null) {
                return;
            }

            // �h���b�O������̃O���b�h�����ݑI�����Ă���O���b�h�̃^�C�v���A�I���ςłȂ��ꍇ
            if (dragTileGrid.tileGridType == currentTileGridType && lastSelectTileGrid != dragTileGrid && !dragTileGrid.IsSelected) {
                float distance = Vector2.Distance(dragTileGrid.transform.position, lastSelectTileGrid.transform.position);

                // �O���b�h�ƃO���b�h�̋������Ȃ���͈͓��Ȃ�
                if (distance < tileGridDistance) {
                    dragTileGrid.IsSelected = true;

                    lastSelectTileGrid = dragTileGrid;

                    linkCount++;
                    dragTileGrid.Num = linkCount;
                    AddEraseTileGridlList(dragTileGrid);

                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Block_Choose);
                }
            }
            //Debug.Log(dragTileGrid.tileGridType);

            if (eraseTileGridList.Count > 1) {
                //Debug.Log(dragTileGrid.Num);

                // �폜���X�g�̃O���b�h���Ō�ɑI�����Ă���O���b�h�ł͂Ȃ��A�폜�Ώۂ̔ԍ��Ɠ����őI���ς̏ꍇ(�P��O�ɖ߂����ꍇ)
                if (eraseTileGridList[linkCount - 1] != lastSelectTileGrid && eraseTileGridList[linkCount - 1].Num == dragTileGrid.Num && dragTileGrid.IsSelected) {

                    // �I�𒆂̃O���b�h����菜�� 
                    RemoveEraseTileGridList(lastSelectTileGrid);

                    // ���I���ɖ߂�
                    lastSelectTileGrid.GetComponent<TileGridDetail>().IsSelected = false;

                    // �Ō�̃O���b�h�̏����A�O�̃O���b�h�̏��ɖ߂�
                    lastSelectTileGrid = dragTileGrid;
                    linkCount--;

                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Cancel);
                }
            }
        }
    }

    /// <summary>
    /// �O���b�h�̃h���b�O����߂��i�w����ʂ��痣�����j�ۂ̏���
    /// </summary>
    private void OnEndDrag() {
        // 3�ȏ�O���b�h���I������Ă���ꍇ
        if (eraseTileGridList.Count >= 3) {

            // �폜�ΏۂƂ��đI������Ă���(���X�g�ɓo�^����Ă���)�O���b�h������
            tileGridBehaviour.EraseTileGrids(eraseTileGridList);

            // �������O���b�h�̐��̉��Z
            mainGameManager.UpdateTotalErasePoint(eraseTileGridList.Count);

            // �t�B�[�o�[�|�C���g�̉��Z
            mainGameManager.UpdateFeverPoint(eraseTileGridList.Count);


        } else {
            // �폜���̃O���b�h�̑I��������
            ReleaseTileGrids();

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
        }
        // �폜�Ώۃ��X�g���N���A
        eraseTileGridList.Clear();

        // ������
        ResetSelectTileGrid();

        // �c��̃O���b�h�����m�F�B�F��ς��Ă��炸�A12�ȉ��̏ꍇ�ɂ́A�����_����1�F�ɂ���
        tileGridBehaviour.CheckLastColor();     
    }

    /// <summary>
    /// �Ȃ������O���b�h���폜�\��̃��X�g�ɒǉ�
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void AddEraseTileGridlList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Add(dragTileGrid);
        ChangeTileGridAlpha(dragTileGrid, 0.5f);
    }

    /// <summary>
    /// �O�̃O���b�h�ɖ߂����ۂɍ폜�\��̃��X�g����폜
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void RemoveEraseTileGridList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Remove(dragTileGrid);

        // �F���߂�
        ChangeTileGridAlpha(dragTileGrid, 1.0f);

        // ���I���̏�Ԃɖ߂�
        if (dragTileGrid.IsSelected) {
            dragTileGrid.IsSelected = false;
        }
    }

    /// <summary>
    /// �폜�\��̃O���b�h�̃A���t�@��ύX
    /// �I�𒆂̂��͔̂������B���I���ɂȂ������̂͌��̃A���t�@�ɖ߂�
    /// </summary>
    /// <param name="dragTileGrid"></param>
    /// <param name="alphaValue"></param>
    private void ChangeTileGridAlpha(TileGridDetail dragTileGrid, float alphaValue) {
        dragTileGrid.spriteTileGrid.color = new(dragTileGrid.spriteTileGrid.color.r, dragTileGrid.spriteTileGrid.color.g, dragTileGrid.spriteTileGrid.color.b, alphaValue);
        dragTileGrid.transform.DOShakeScale(0.15f)
            .SetEase(Ease.InQuart)
            .SetLink(dragTileGrid.gameObject)
            .OnComplete(() => dragTileGrid.transform.localScale = Vector3.one * tileGridBehaviour.GetTileGridSize());
    }

    /// <summary>
    /// �I�𒆂̃O���b�h�̑I������
    /// </summary>
    private void ReleaseTileGrids() {
        for (int i = 0; i < eraseTileGridList.Count; i++) {
            // �I�񂾐���2�ȉ��̏ꍇ�@�e�O���b�h�̑I����Ԃ���������
            eraseTileGridList[i].IsSelected = false;
            // �F���߂�
            ChangeTileGridAlpha(eraseTileGridList[i], 1.0f);
        }
    }

    /// <summary>
    /// �I�������O���b�h��������
    /// </summary>
    private void ResetSelectTileGrid() {
        // ������
        firstSelectTileGrid = null;
        lastSelectTileGrid = null;
        currentTileGridType = null;
    }

    /// <summary>
    /// ��Q���ɐڐG�����ۂ̃O���b�h�̏���
    /// </summary>
    public void TriggerObstacle() {
        // �폜���̃O���b�h�̑I��������
        ReleaseTileGrids();

        // �폜�Ώۃ��X�g���N���A
        eraseTileGridList.Clear();
        //Debug.Log(eraseTileGridList.Count);

        // �I�������O���b�h��������
        ResetSelectTileGrid();
        //Debug.Log(firstSelectTileGrid);

        //Debug.Log("�I������");
    }
}