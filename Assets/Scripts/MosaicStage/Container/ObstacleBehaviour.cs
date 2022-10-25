using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ��Q���̐����E�Ǘ��N���X
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

    // Injection ����ƁASerializeField �̓��e�� Null �ɂȂ�̂ŁA�C���X�^���X�𒍓��������K�v������
    //[Inject]
    //public ObstacleBehaviour(MainGameManager mainGameManager, LifeModel lifeModel, ObstacleBall obstacleBall, Transform[] trans) {//, MainGamePresenter mainGamePresenter �ˑ��֌W�̏z�ɂ��A�G���[�ɂȂ�
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
    /// ��Q���̐���
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
    /// ��Q���̐���������W���擾
    /// </summary>
    /// <returns></returns>
    private Vector3 GetObstaclePos() {
        //Debug.Log(obstacleBallTrans[0]);
        //Debug.Log(obstacleBallTrans[1]);
        return new(UnityEngine.Random.Range(obstacleBallTrans[0].position.x, obstacleBallTrans[1].position.x),
            UnityEngine.Random.Range(obstacleBallTrans[0].position.y, obstacleBallTrans[1].position.y), 0);
    }

    /// <summary>
    /// ���ׂĂ̏�Q�����ēx������
    /// </summary>
    public void RestartAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(false);
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̈ړ����~
    /// </summary>
    public void StopAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].StopMoveBall();
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̈ړ���ᑬ��
    /// </summary>
    public void SlowDownAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(true);
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̔j��
    /// </summary>
    public void DestroyAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            Destroy(obstacleList[i].gameObject);
        }
        obstacleList.Clear();
    }
}
