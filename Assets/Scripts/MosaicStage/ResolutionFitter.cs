using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionFitter : MonoBehaviour
{
    [System.Serializable]
    sealed class Resolution {
        public int width;
        public int height;

        public void Set(int width, int height) {
            this.width = width;
            this.height = height;
        }
    }

    [SerializeField]
    List<Resolution> resolutions = new();


    //void Start()
    //{
    //    MeasureScreenSize();
    //}


    public void MeasureScreenSize() {
        resolutions.Clear();

        int currentIndex = 0;
        List<string> options = new();
        Resolution resolution = new();
        for (int i = 0; i < Screen.resolutions.Length; i++) {
            if (Screen.resolutions[i].width == Screen.width && Screen.resolutions[i].height == Screen.height) {
                currentIndex = i;
            }
            resolution = new();
            resolution.width = Screen.resolutions[i].width;
            resolution.height = Screen.resolutions[i].height;
            resolutions.Add(resolution);
            options.Add(Screen.resolutions[i].width.ToString() + "x" + Screen.resolutions[i].height.ToString());
        }

        resolution.Set(1440, 2880);
        SetResolution(resolution);
    }

    void SetResolution(Resolution resolution) {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log($"‰ð‘œ“x•ÏX : { resolution.width } : { resolution.height }");
    }
}