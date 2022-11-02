using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadyPlayerMe;

public class RPMAvatarLoader : MonoBehaviour
{
    public AvatarLoader avatarLoader;
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
        avatarLoader = new AvatarLoader();
    }
}
