using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ReadyPlayerMe;

public class RPMAvatarLoader : MonoBehaviour
{
    public AvatarLoader avatarLoader;
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        avatarLoader = new AvatarLoader();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Avatar") Destroy(this.gameObject);
    }
}
