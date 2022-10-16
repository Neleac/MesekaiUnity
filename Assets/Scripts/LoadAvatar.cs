using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class LoadAvatar : MonoBehaviour
    {
        private AvatarLoader avatarLoader;

        void Start()
        {
            avatarLoader = new AvatarLoader();
        }

        public void OnStartClick()
        {
            string avatarURL = "https://api.readyplayer.me/v1/avatars/6230ecd2cc9780a069f9852c.glb";
            avatarLoader.LoadAvatar(avatarURL, OnAvatarImported, OnAvatarLoaded);
        }

        private void OnAvatarImported(GameObject avatar)
        {
            Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        {
            Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");

            
        }
    }
}
