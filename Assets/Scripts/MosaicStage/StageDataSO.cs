using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yamap {

    [CreateAssetMenu(fileName = "StageDataSO", menuName = "Create StageDataSO")]
    public class StageDataSO : ScriptableObject {
        public List<StageData> stageDataList = new();
    }
}