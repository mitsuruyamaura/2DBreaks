using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class UserData : MonoBehaviour
{
    public static UserData instance;

    public ReactiveProperty<int> MosaicCount = new(); 

    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
