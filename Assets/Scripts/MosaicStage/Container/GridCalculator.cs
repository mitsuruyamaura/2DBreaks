#pragma warning disable 0649
#pragma warning disable 0414

using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer.Unity;
using System;
using DG.Tweening;

/// <summary>
/// グリッドをつなげたり、つながっているグリッドを計算して制御して、TileGridBehaivour に提供するクラス
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
    /// Presenter から Entry させる
    /// </summary>
    void ITickable.Tick() {
        // MainGameManager をこの中で使わなくなったので不要 
        //if (mainGameManager == null) {
        //    return;
        //}

        // Tick に Subscribe だと、Update 内で Subscribe しているのと同じになるので、普通に Update 内に必要な処理だけを書く
        // ゲームの状態については、Presenter 側で見ているので、ここでのチェックは不要
        // グリッドをつなげる処理
        if (Input.GetMouseButtonDown(0) && firstSelectTileGrid == null) {
            OnStartDrag();
        } else if (Input.GetMouseButtonUp(0)) {
            OnEndDrag();
        } else if (firstSelectTileGrid != null) {
            OnDragging();
        }

        //mainGameManager.State// <- Update で呼びまくっているので不具合が起こる
        //    .Where(state => state == GameState.Play)
        //    .Subscribe(_ => {
        //        // グリッドをつなげる処理
        //        if (Input.GetMouseButtonDown(0) && firstSelectTileGrid == null) {
        //            OnStartDrag();
        //        } else if (Input.GetMouseButtonUp(0)) {
        //            OnEndDrag();
        //        } else if (firstSelectTileGrid != null) {
        //            OnDragging();
        //        }
        //        mainGameManager.GameTime.Value += Time.deltaTime;
        //    })
        //    .AddTo(disposables);
    }

    /// <summary>
    /// グリッドを最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag() {
        //Debug.Log("ドラッグ開始");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); //  Camera.main.ScreenToWorldPoint

        // グリッドがつながっている数を初期化
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
    /// グリッドのドラッグ中（スワイプ）処理
    /// </summary>
    private void OnDragging() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out TileGridDetail dragTileGrid)) {

            // TileGrid 以外の場所の場合には何もしない
            if (currentTileGridType == null) {
                return;
            }

            // ドラッグした先のグリッドが現在選択しているグリッドのタイプかつ、選択済でない場合
            if (dragTileGrid.tileGridType == currentTileGridType && lastSelectTileGrid != dragTileGrid && !dragTileGrid.IsSelected) {
                float distance = Vector2.Distance(dragTileGrid.transform.position, lastSelectTileGrid.transform.position);

                // グリッドとグリッドの距離がつながる範囲内なら
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

                // 削除リストのグリッドが最後に選択しているグリッドではなく、削除対象の番号と同じで選択済の場合(１つ手前に戻した場合)
                if (eraseTileGridList[linkCount - 1] != lastSelectTileGrid && eraseTileGridList[linkCount - 1].Num == dragTileGrid.Num && dragTileGrid.IsSelected) {

                    // 選択中のグリッドを取り除く 
                    RemoveEraseTileGridList(lastSelectTileGrid);

                    // 未選択に戻す
                    lastSelectTileGrid.GetComponent<TileGridDetail>().IsSelected = false;

                    // 最後のグリッドの情報を、前のグリッドの情報に戻す
                    lastSelectTileGrid = dragTileGrid;
                    linkCount--;

                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Cancel);
                }
            }
        }
    }

    /// <summary>
    /// グリッドのドラッグをやめた（指を画面から離した）際の処理
    /// </summary>
    private void OnEndDrag() {
        // 3つ以上グリッドが選択されている場合
        if (eraseTileGridList.Count >= 3) {

            // 削除対象として選択されている(リストに登録されている)グリッドを消す
            tileGridBehaviour.EraseTileGrids(eraseTileGridList);

            // 消したグリッドの数の加算
            mainGameManager.UpdateTotalErasePoint(eraseTileGridList.Count);

            // フィーバーポイントの加算
            mainGameManager.UpdateFeverPoint(eraseTileGridList.Count);


        } else {
            // 削除候補のグリッドの選択を解除
            ReleaseTileGrids();

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Cancel);
        }
        // 削除対象リストをクリア
        eraseTileGridList.Clear();

        // 初期化
        ResetSelectTileGrid();

        // 残りのグリッド数を確認。色を変えておらず、12個以下の場合には、ランダムな1色にする
        tileGridBehaviour.CheckLastColor();     
    }

    /// <summary>
    /// つながったグリッドを削除予定のリストに追加
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void AddEraseTileGridlList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Add(dragTileGrid);
        ChangeTileGridAlpha(dragTileGrid, 0.5f);
    }

    /// <summary>
    /// 前のグリッドに戻った際に削除予定のリストから削除
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void RemoveEraseTileGridList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Remove(dragTileGrid);

        // 色も戻す
        ChangeTileGridAlpha(dragTileGrid, 1.0f);

        // 未選択の状態に戻す
        if (dragTileGrid.IsSelected) {
            dragTileGrid.IsSelected = false;
        }
    }

    /// <summary>
    /// 削除予定のグリッドのアルファを変更
    /// 選択中のものは半透明。未選択になったものは元のアルファに戻す
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
    /// 選択中のグリッドの選択解除
    /// </summary>
    private void ReleaseTileGrids() {
        for (int i = 0; i < eraseTileGridList.Count; i++) {
            // 選んだ数か2個以下の場合　各グリッドの選択状態を解除する
            eraseTileGridList[i].IsSelected = false;
            // 色も戻す
            ChangeTileGridAlpha(eraseTileGridList[i], 1.0f);
        }
    }

    /// <summary>
    /// 選択したグリッドを初期化
    /// </summary>
    private void ResetSelectTileGrid() {
        // 初期化
        firstSelectTileGrid = null;
        lastSelectTileGrid = null;
        currentTileGridType = null;
    }

    /// <summary>
    /// 障害物に接触した際のグリッドの処理
    /// </summary>
    public void TriggerObstacle() {
        // 削除候補のグリッドの選択を解除
        ReleaseTileGrids();

        // 削除対象リストをクリア
        eraseTileGridList.Clear();
        //Debug.Log(eraseTileGridList.Count);

        // 選択したグリッドを初期化
        ResetSelectTileGrid();
        //Debug.Log(firstSelectTileGrid);

        //Debug.Log("選択解除");
    }
}