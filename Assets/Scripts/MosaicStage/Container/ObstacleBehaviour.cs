using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// 障害物の生成・管理クラス
/// </summary>
public class ObstacleBehaviour : MonoBehaviour//, IStartable
{
    [SerializeField]
    private ObstacleBall obstacleBallPrefab;

    [SerializeField]
    private Transform[] obstacleBallTrans;

    public List<ObstacleBall> obstacleList = new();


    private MainGameManager mainGameManager;
    private LifeModel lifeModel;
    //private MainGamePresenter mainGamePresenter;

    // Injection すると、SerializeField の内容が Null になるので、インスタンスを注入し直す必要がある
    //[Inject]
    //public ObstacleBehaviour(MainGameManager mainGameManager, LifeModel lifeModel, ObstacleBall obstacleBall, Transform[] trans) {//, MainGamePresenter mainGamePresenter 依存関係の循環により、エラーになる
    //    this.mainGameManager = mainGameManager;
    //    this.lifeModel = lifeModel;
    //    //this.mainGamePresenter = mainGamePresenter;

    //    obstacleBallPrefab = obstacleBall;
    //    obstacleBallTrans = trans;

    //    Debug.Log(this.mainGameManager);
    //    Debug.Log(obstacleBallPrefab);
    //    Debug.Log(obstacleBallTrans[0]);
    //}

    //private MainGameManager mainGameManager;
    //private LifeModel lifeModel;

    //[Inject]
    //public ObstacleBehaviour(MainGameManager mainGameManager, LifeModel lifeModel) {
    //    this.mainGameManager = mainGameManager;
    //    this.lifeModel = lifeModel;
    //    Debug.Log(this.mainGameManager);
    //}

    //void IStartable.Start() {
    //    mainGameManager.SetUpStageData();
    //    Debug.Log(this.mainGameManager.GetCurrentStageData());
    //    CreateObstacles(mainGameManager.GetCurrentStageData().obstacleCount, mainGameManager.GetCurrentStageData().obstacleSpeeds);
    //}

    /// <summary>
    /// 障害物の生成
    /// </summary>
    /// <param name="createCount"></param>
    public void CreateObstacles(int createCount, float[] obstacleSpeeds, MainGameManager mainGameManager, LifeModel lifeModel) {
        for (int i = 0; i < createCount; i++) {
            ObstacleBall obstacleBall = Instantiate(obstacleBallPrefab, GetObstaclePos(), Quaternion.identity);
            //obstacleBall.SetUpObstacleBall(this, obstacleSpeeds);
            obstacleBall.SetUpObstacleBall(obstacleSpeeds, mainGameManager, lifeModel);
            obstacleList.Add(obstacleBall);
        }
    }

    /// <summary>
    /// 障害物の生成する座標を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetObstaclePos() {
        //Debug.Log(obstacleBallTrans[0]);
        //Debug.Log(obstacleBallTrans[1]);
        return new(UnityEngine.Random.Range(obstacleBallTrans[0].position.x, obstacleBallTrans[1].position.x),
            UnityEngine.Random.Range(obstacleBallTrans[0].position.y, obstacleBallTrans[1].position.y), 0);
    }

    /// <summary>
    /// すべての障害物を再度動かす
    /// </summary>
    public void RestartAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(false);
        }
    }

    /// <summary>
    /// すべての障害物の移動を停止
    /// </summary>
    public void StopAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].StopMoveBall();
        }
    }

    /// <summary>
    /// すべての障害物の移動を低速に
    /// </summary>
    public void SlowDownAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(true);
        }
    }

    /// <summary>
    /// すべての障害物の破壊
    /// </summary>
    public void DestroyAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            Destroy(obstacleList[i].gameObject);
        }
        obstacleList.Clear();
    }
}
