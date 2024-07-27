using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yamap {

    public class DataBaseManager : MonoBehaviour {

        public static DataBaseManager instance;
        
        public UnitDataSO unitDataSO;
        public EffectDataSO effectDataSO;


        void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }    
        }

        public UnitData GetUnitData(int searchId) {
            return unitDataSO.unitDataList.Find(x => x.id == searchId);
        }


        public GameObject GetEffectData(EffectType searchEffectType) {
            return effectDataSO.effectDataList.Find(x => x.effectType == searchEffectType).effectPrefab;
        }
    }
}