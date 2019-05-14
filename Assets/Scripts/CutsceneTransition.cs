using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadSceneAfterVideoEnded : MonoBehaviour {
    
    public VideoPlayer vp;
    
    void Start() {
        vp.loopPointReached += LoadScene;
    }

    void LoadScene(VideoPlayer vp) {
        SceneManager.LoadScene(2);
    }
}