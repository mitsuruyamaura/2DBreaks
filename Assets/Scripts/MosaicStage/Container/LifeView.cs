using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LifeView : MonoBehaviour
{
    [SerializeField]
    private GameObject lifeIconPrefab;

    [SerializeField]
    private Transform lifeSetTran;

    [SerializeField]
    private List<GameObject> lifeIconlist = new();

    [SerializeField]
    private FlushScreen flushScreen;


    /// <summary>
    /// ���C�t�A�C�R���̐���
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTaskVoid CreateLifeIconAsync(int lifeCount, CancellationToken token) {
        for (int i = 0; i < lifeCount; i++) {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeSetTran, false);
            lifeIcon.transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);
            lifeIconlist.Add(lifeIcon);
            await UniTask.Delay(250, cancellationToken: token);
        }
    }

    /// <summary>
    /// ���C�t�����B�A�C�R���̐��X�V
    /// </summary>
    public void ReduceLife() {
        if (lifeIconlist.Count > 0) {

            lifeIconlist[0].transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);

            Destroy(lifeIconlist[0].gameObject, 0.5f);
            lifeIconlist.Remove(lifeIconlist[0]);

            // ��ʓ_��
            flushScreen.Flush();
        }
    }
}