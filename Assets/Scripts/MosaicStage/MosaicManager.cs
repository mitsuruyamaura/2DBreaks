using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Linq;
using System;

public class MosaicManager : MonoBehaviour
{
    [SerializeField]
    private TileGridDetail tileGridPrefab;

    [SerializeField]
    private GameObject eraseEffectPrefab;

    [SerializeField]
    private ObstacleBall obstacleBallPrefab;

    [SerializeField]
    private Transform tileGridSetTran;

    [SerializeField]
    private Transform[] obstacleBallTrans;

    //[SerializeField]
    //private int obstaclesCount;  // StageData ����擾����̂ŕs�v

    [SerializeField, Header("�s")]
    private int rowCount;

    [SerializeField, Header("��")]
    private int columnCount;

    [SerializeField]
    private float tileGridSize;

    [SerializeField, Header("�������ꂽ Grid �̃��X�g")]
    private List<TileGridDetail> tileGridList = new ();

    private TileGridDetail firstSelectTileGrid;
    private TileGridDetail lastSelectTileGrid;
    private TileGridType? currentTileGridType;

    [SerializeField]
    private List<TileGridDetail> eraseTileGridList = new ();

    [SerializeField]
    private List<ObstacleBall> obstacleList = new ();

    [SerializeField]
    private int linkCount = 0;

    [SerializeField]
    private float tileGridDistance = 1.0f;

    /// <summary>
    /// �Q�[���̐i�s��
    /// </summary>
    public enum GameState {
        Ready,
        Play,
        GameUp
    }

    [Header("���݂̃Q�[���̐i�s��")]
    public GameState gameState = GameState.Ready;

    private bool isLastColor;

    public ReactiveProperty<float> GameTime = new();

    [SerializeField]
    private Text[] txtGameTimes;

    [SerializeField]
    private GameObject lifeIconPrefab;

    [SerializeField]
    private Transform lifeSetTran;

    private int lifeCount = 3;

    [SerializeField]
    private List<GameObject> lifeIconlist = new();

    [SerializeField]
    private Text txtMosaicCount;


    [SerializeField]
    private SpriteRenderer normalChara;

    [SerializeField]
    private SpriteRenderer rareChara;

    [SerializeField]
    private FlushScreen flushScreen;

    private yamap.StageData currentStageData;
    private ReactiveProperty<int> TotalErasePoint = new();

    [SerializeField]
    private Slider sliderFever;

    [SerializeField]
    private Text txtValue;

    private ReactiveProperty<int> FeverPoint = new();

    private int targetFeverPoint = 15;   // 3 =>1  5 => 3  6 => 4  8 => 6
    public ReactiveProperty<bool> IsFeverTime = new(false);
    private int feverDuraiton = 7500;

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private Image imgCharaIcon;

    [SerializeField]
    private Image imgExcellentLogo;

    [SerializeField]
    private Image imgGameInfo;

    [SerializeField]
    private Sprite[] gameInfoLogos;

    [SerializeField]
    private AchievementStageData currentAchievementStageData;


    void Start()
    {
        gameState = GameState.Ready;

        // �X�e�[�W���Ƃ� BGM �Đ�
        SoundManager.instance?.PlayBGM((SoundManager.BGM_TYPE)Enum.Parse(typeof(SoundManager.BGM_TYPE), "Stage_" + SelectStage.stageNo));

        currentAchievementStageData = new AchievementStageData(SelectStage.stageNo);
        txtInfo.gameObject.SetActive(false);

        // �X�e�[�W���̎擾
        currentStageData = UserData.instance.GetStageData(SelectStage.stageNo);

        // �L�����摜�̐ݒ�
        SetCharaSprite();

        // �O���b�h�̐���
        CreateTileGrids();

        // ��Q���̐���
        CreateObstacles(currentStageData.obstacleCount);

        // ���C�t�A�C�R���̐���
        var token = this.GetCancellationTokenOnDestroy();
        CreateLifeIconAsync(token).Forget();

        // �X�e�[�g���������̂Ƃ������Q�[���J�n�ɕύX
        if (gameState == GameState.Ready) {
            gameState = GameState.Play;
        }

        this.UpdateAsObservable()
            .Where(_ => gameState == GameState.Play)
            .Subscribe(_ => 
            {
                // �O���b�h���Ȃ��鏈��
                if (Input.GetMouseButtonDown(0) && firstSelectTileGrid == null) {
                    OnStartDrag();
                } else if (Input.GetMouseButtonUp(0)) {
                    OnEndDrag();
                } else if (firstSelectTileGrid != null) {
                    OnDragging();
                }

                GameTime.Value += Time.deltaTime;
            })
            .AddTo(this);

        // �Q�[�����Ԃ̊Ď�
        GameTime.Subscribe(x =>
        {
            // �����_�ȉ��͏������\��
            string time = x.ToString("F2");
            string[] part = time.Split('.');
            txtGameTimes[0].text = part[0] + ".";
            txtGameTimes[1].text = part[1];// "<size=30>part[1]</size>";  // HTML �̃^�O���ϐ��ɑΉ����Ă��Ȃ��B���e�����̂݁B
        }).AddTo(this);

        // �󂵂��O���b�h�̊Ď�
        TotalErasePoint
            .Zip(TotalErasePoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateMosaicCount(x.oldValue, x.newValue)).AddTo(this);

        // �����l�Z�b�g
        TotalErasePoint.SetValueAndForceNotify(0);

        // �t�B�[�o�[�Q�[�W�̐ݒ�
        sliderFever.maxValue = targetFeverPoint;
        sliderFever.value = 0;

        // �t�B�[�o�[�|�C���g�̍w��
        FeverPoint
            .Zip(FeverPoint.Skip(1), (oldValue, newValue) => (oldValue, newValue))
            .Subscribe(x => UpdateDisplayValue(x.oldValue, x.newValue))
            .AddTo(sliderFever.gameObject);
    }

    /// <summary>
    /// Slider �̏�� % �\���̍X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateDisplayValue(float oldValue, float newValue) {
        if (newValue >= targetFeverPoint) {
            newValue = targetFeverPoint;
        }
        float before = oldValue / targetFeverPoint * 100;
        float after = newValue / targetFeverPoint * 100;

        //Debug.Log(before);
        //Debug.Log(after);

        int a = (int)before;
        int b = (int)after;

        //Debug.Log(a);
        //Debug.Log(b);
        // �����̃A�j�����Ԃ̐ݒ�B�t�B�[�o�[�����ۂɂ͒����Ȃ�
        float duration = newValue == 0 ? (float)feverDuraiton / 1000 : 0.25f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(txtValue.DOCounter(a, b, duration).SetEase(Ease.Linear)).SetLink(txtValue.gameObject);

        // ���^���ɂȂ�����
        if (newValue == targetFeverPoint) {
            // 100�� �̐�����������A�j�����o��ǉ�
            float scale = txtValue.transform.localScale.x;
            sequence.Append( 
            txtValue.transform.DOPunchScale(txtValue.transform.localScale * 1.25f, 0.25f).SetEase(Ease.InQuart).SetLink(txtValue.gameObject)
                .OnComplete(() => txtValue.transform.localScale = Vector3.one * scale));
        }
    }

    /// <summary>
    /// �X�e�[�W���Ƃ̃L�����摜�̐ݒ�
    /// </summary>
    private void SetCharaSprite() {
        normalChara.sprite = currentStageData.normalCharaSprite;
        rareChara.sprite = currentStageData.rareCharaSprite;
        imgCharaIcon.sprite = currentStageData.charaIcon;

        imgCharaIcon.transform.DOShakeScale(0.75f, 1f, 4)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
    }

    /// <summary>
    /// �󂵂��O���b�h�̐��̕\���X�V
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void UpdateMosaicCount(int oldValue, int newValue) {
        txtMosaicCount.DOCounter(oldValue, newValue, 0.5f).SetEase(Ease.Linear);
        // ����̃L�����A�C�R�����A�j��
        imgCharaIcon.transform.DOPunchScale(Vector3.one * 1.25f, 0.25f)
            .SetEase(Ease.InQuart)
            .SetLink(gameObject);
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

                eraseTileGridList = new ();
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
            for (int i = 0; i < eraseTileGridList.Count; i++) {

                // ���X�g�����菜��
                tileGridList.Remove(eraseTileGridList[i]);

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

            // �������O���b�h�̐��̉��Z�B�t�B�[�o�[����3 - 5�{
            TotalErasePoint.Value += IsFeverTime.Value ? eraseTileGridList.Count * (3 + currentStageData.stageNo) : eraseTileGridList.Count;

            // �������O���b�h���̍ő�l�̍X�V�m�F
            if (eraseTileGridList.Count > currentAchievementStageData.maxLinkCount) currentAchievementStageData.maxLinkCount = eraseTileGridList.Count;

            // �t�B�[�o�[���Ă��Ȃ��ꍇ
            if (!IsFeverTime.Value) {
                // �ő�l�𒴂��Ȃ��悤�Ƀt�B�[�o�[�|�C���g���Z�@�������� - 2
                FeverPoint.Value = Mathf.Min(targetFeverPoint, FeverPoint.Value += CalculateFeverPoint(eraseTileGridList.Count));
                //Debug.Log(FeverPoint.Value);

                // �t�B�[�o�[�̊m�F
                if (CheckFeverTime()) {
                    // �t�B�[�o�[�񐔉��Z
                    currentAchievementStageData.maxFeverCount++;

                    // �t�B�[�o�[�^�C���J�n
                    ObserveFeverTimeAsync().Forget();

                    SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�t�B�[�o�[);
                } else {
                    // �t�B�[�o�[�łȂ���΃Q�[�W����
                    sliderFever.DOValue(FeverPoint.Value, 0.25f).SetEase(Ease.InQuart).SetLink(sliderFever.gameObject);

                    // SE
                    SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Erase);
                }
            } else {
                // SE
                SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);
            }
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
        if (!isLastColor && tileGridList.Count < 12) {
            isLastColor = true;
            ChangeTileGridsColor();
        }

        // �c��2�ȉ��Ȃ炷�ׂč폜���ăN���A
        if (tileGridList.Count <= 2) {
            for (int i = 0; i < tileGridList.Count; i++) {
                Destroy(tileGridList[i].gameObject);
            }
            //Debug.Log("Game Clear");

            DestroyAllObstacles();
            tileGridList.Clear();
            gameState = GameState.GameUp;

            // �N���A�^�C���̕ێ�
            currentAchievementStageData.fastestClearTime = GameTime.Value;

            Result(true);
        }
    }

    /// <summary>
    /// �t�B�[�o�[�|�C���g�̌v�Z
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private int CalculateFeverPoint(int point) {
        return point - 2;
    }

    /// <summary>
    /// �t�B�[�o�[�̔���
    /// </summary>
    /// <returns></returns>
    private bool CheckFeverTime() {
        return IsFeverTime.Value = FeverPoint.Value >= targetFeverPoint ? true : false;
    }

    /// <summary>
    /// �t�B�[�o�[�^�C���̊Ď�
    /// </summary>
    /// <returns></returns>
    private async UniTask ObserveFeverTimeAsync() {
        // ��Q���̈ړ����x��ᑬ�ɂ��A�r���ł̑��x�A�b�v���Ȃ��ɂ���
        SlowDownAllObstacles();

        // 100�� �̃A�j�����o���I���܂őҋ@
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(500, cancellationToken: token);

        sliderFever.value = targetFeverPoint;       
        sliderFever.DOValue(0, (float)feverDuraiton / 1000).SetEase(Ease.Linear).SetLink(gameObject);
        
        // �Q�[�W�ɍ��킹�āA������ 100% -> 0% �ɂ���
        FeverPoint.Value = 0;

        await UniTask.Delay(feverDuraiton, false, PlayerLoopTiming.Update, token);
        IsFeverTime.Value = false;
        //Debug.Log("�t�B�[�o�[�^�C���I��");

        // ��Q���̈ړ����x��߂�
        RestartAllObstacles();
    }

    /// <summary>
    /// �O���b�h�𐶐�
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private void CreateTileGrids() {
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
    }

    /// <summary>
    /// �Ȃ������O���b�h���폜���X�g�ɒǉ�
    /// </summary>
    /// <param name="dragTileGrid"></param>
    private void AddEraseTileGridlList(TileGridDetail dragTileGrid) {
        eraseTileGridList.Add(dragTileGrid);
        ChangeTileGridAlpha(dragTileGrid, 0.5f);
    }

    /// <summary>
    /// �O�̃O���b�h�ɖ߂����ۂɍ폜���X�g����폜
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
    /// �O���b�h�̃A���t�@��ύX
    /// </summary>
    /// <param name="dragTileGrid"></param>
    /// <param name="alphaValue"></param>
    private void ChangeTileGridAlpha(TileGridDetail dragTileGrid, float alphaValue) {
        dragTileGrid.spriteTileGrid.color = new (dragTileGrid.spriteTileGrid.color.r, dragTileGrid.spriteTileGrid.color.g, dragTileGrid.spriteTileGrid.color.b, alphaValue);
        dragTileGrid.transform.DOShakeScale(0.15f).SetEase(Ease.InQuart).SetLink(dragTileGrid.gameObject).OnComplete(() => dragTileGrid.transform.localScale = Vector3.one * tileGridSize);
    }

    /// <summary>
    /// �O���b�h�̐F���P�F�ɕς���
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
    /// ��Q���ɐڐG���č폜�Ɏ��s�����ꍇ
    /// </summary>
    public void FailureErase() {
        // �d������h�~(��Q�����ł����䂵�Ă��邪�O�̂���)
        if (gameState != GameState.Play) {
            return;
        }

        gameState = GameState.Ready;

        // SE
        SoundManager.instance?.PlaySE(SoundManager.SE_TYPE.Miss);

        lifeIconlist[0].transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);

        Destroy(lifeIconlist[0].gameObject, 0.5f);
        lifeIconlist.Remove(lifeIconlist[0]);

        // ��ʓ_��
        flushScreen.Flush();

        // �Q�[���I�[�o�[����
        if (lifeIconlist.Count <= 0) {

            gameState = GameState.GameUp;

            StopAllObstacles();
            //Debug.Log("Game Over");

            // ���Z
            Result(false);

            return;
        }

        // �폜���̃O���b�h�̑I��������
        ReleaseTileGrids();

        // �폜�Ώۃ��X�g���N���A
        eraseTileGridList.Clear();

        // �I�������O���b�h��������
        ResetSelectTileGrid();

        // ���ׂĂ̏�Q���̍Ĉړ��J�n
        RestartAllObstacles();

        gameState = GameState.Play;
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
    /// ��Q���̐���
    /// </summary>
    /// <param name="createCount"></param>
    private void CreateObstacles(int createCount) {
        for (int i = 0; i < createCount; i++) {
            ObstacleBall obstacleBall = Instantiate(obstacleBallPrefab, GetObstaclePos(), Quaternion.identity);
            obstacleBall.SetUpObstacleBall(this, currentStageData.obstacleSpeeds);
            obstacleList.Add(obstacleBall);
        }
    }

    /// <summary>
    /// ��Q���̐���������W���擾
    /// </summary>
    /// <returns></returns>
    private Vector3 GetObstaclePos() {
        return new(UnityEngine.Random.Range(obstacleBallTrans[0].position.x, obstacleBallTrans[1].position.x), 
            UnityEngine.Random.Range(obstacleBallTrans[0].position.y, obstacleBallTrans[1].position.y), 0);
    }

    /// <summary>
    /// ���ׂĂ̏�Q�����ēx������
    /// </summary>
    private void RestartAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(false);
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̈ړ����~
    /// </summary>
    private void StopAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].StopMoveBall();
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̈ړ���ᑬ��
    /// </summary>
    private void SlowDownAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            obstacleList[i].ShotBall(true);
        }
    }

    /// <summary>
    /// ���ׂĂ̏�Q���̔j��
    /// </summary>
    private void DestroyAllObstacles() {
        for (int i = 0; i < obstacleList.Count; i++) {
            Destroy(obstacleList[i].gameObject);
        }
        obstacleList.Clear();
    }

    /// <summary>
    /// ���C�t�A�C�R���̐���
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTaskVoid CreateLifeIconAsync(CancellationToken token) {
        for (int i = 0; i < lifeCount; i++) {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, lifeSetTran, false);
            lifeIcon.transform.DOShakeScale(0.5f).SetEase(Ease.Linear).SetLink(gameObject);
            lifeIconlist.Add(lifeIcon);
            await UniTask.Delay(250, false, PlayerLoopTiming.Update, token);
        }
    }

    /// <summary>
    /// �Q�[�����e�̐��Z
    /// </summary>
    /// <param name="isClear"></param>
    private void Result(bool isClear) {
        // ����̏������u���b�N�̃|�C���g�����Z
        UserData.instance.MosaicCount.Value += TotalErasePoint.Value;

        // �ő�l�Ƃ��Ĉ�U�ێ�
        currentAchievementStageData.maxMosaicCount = TotalErasePoint.Value;

        // �`�������W�񐔉��Z
        currentAchievementStageData.challengeCount++;

        // �Q�[���I�[�o�[�̏ꍇ
        if (!isClear) {
            // �Q�[���I�[�o�[�񐔉��Z
            currentAchievementStageData.failureCount++;

            // ���т̍X�V�m�F
            UserData.instance.CheckUpdateAchievementStageData(currentAchievementStageData);

            GameOverAsync().Forget();            
            return;
        }

        // �N���A�����X�e�[�W���ŏI�X�e�[�W�ł͂Ȃ��āA���N���A�̃X�e�[�W�̏ꍇ
        if (currentStageData.stageNo != 2 && !UserData.instance.clearStageNoList.Contains(currentStageData.stageNo + 1)) {
            // ���̃X�e�[�W��ǉ�
            UserData.instance.AddClearStageNoList(currentStageData.stageNo + 1);
        }
        // �N���A�񐔉��Z
        currentAchievementStageData.clearCount++;

        // �m�[�~�X�N���A�̏ꍇ
        if (lifeIconlist.Count >= lifeCount) {
            currentAchievementStageData.noMissClearCount++;
        }
        // ���т̍X�V�m�F
        UserData.instance.CheckUpdateAchievementStageData(currentAchievementStageData);
        GameClearAsync().Forget();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    /// <returns></returns>
    private async UniTask GameOverAsync() {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameOver);

        imgGameInfo.sprite = gameInfoLogos[0];
        imgGameInfo.DOFade(1.0f, 1.5f).SetEase(Ease.InQuart).SetLink(imgGameInfo.gameObject);

        // �N���b�N����
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);

        var token = this.GetCancellationTokenOnDestroy();

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�Q�[���I�[�o�[);

        // �N���b�N�҂�
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);

        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Submit);

        // �V�[���J��
        await StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }

    /// <summary>
    /// �Q�[���N���A���o
    /// </summary>
    /// <returns></returns>
    private async UniTask GameClearAsync() {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE_TYPE.GameClear);

        imgGameInfo.sprite = gameInfoLogos[1];

        Sequence sequence = DOTween.Sequence();
        sequence.Append(imgGameInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
        sequence.AppendInterval(1.0f);
        sequence.Append(imgGameInfo.DOFade(0, 0.5f).SetEase(Ease.Linear));

        // �N���b�N����
        txtInfo.gameObject.SetActive(true);
        txtInfo.DOFade(0, 1.5f).SetEase(Ease.Linear).SetLink(txtInfo.gameObject).SetLoops(-1, LoopType.Yoyo);
        
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�N���A_1);

        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        // �N���b�N�҂�
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);
        SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�N���A_2);

        // �m�[�~�X�N���A
        if (lifeIconlist.Count >= lifeCount) {
            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Excellent);

            imgExcellentLogo.DOFade(1.0f, 2.0f).SetEase(Ease.Linear);
            imgExcellentLogo.transform.DOLocalMoveX(0, 1.5f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);
            await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);

            // �N���b�N�҂�
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);

            SoundManager.instance.PlaySE(SoundManager.SE_TYPE.Fever);

            imgExcellentLogo.DOFade(0f, 0.5f).SetEase(Ease.Linear);
            imgExcellentLogo.transform.DOLocalMoveX(-1250, 1.0f).SetEase(Ease.InOutBack).SetLink(imgExcellentLogo.gameObject);

            // ���A�摜�̃A�j���\��           
            normalChara.material.DOFloat(-1, "_Flip", 1.5f).SetEase(Ease.Linear).SetLink(normalChara.gameObject);
            await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);

            SoundManager.instance.PlayVoice(SoundManager.VOICE_TYPE.�G�N�Z�����g);

            // �N���b�N�҂�
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), PlayerLoopTiming.Update, token);
        }
        // �{�C�X���Ō�܂ŗ�����������
        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);

        // �V�[���J��
        await StartCoroutine(TransitionManager.instance.MoveNextScene(SCENE_STATE.Menu));
    }
}
