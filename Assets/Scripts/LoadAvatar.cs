using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class LoadAvatar : MonoBehaviour
    {
        [SerializeField] private GameObject playerAvatar;
        [SerializeField] private GameObject templateAvatar;

        private AvatarLoader avatarLoader;
        private MotionTransfer motionTransfer;
        private string avatarURL;

        void Start()
        {
            avatarLoader = new AvatarLoader();
            motionTransfer = templateAvatar.GetComponent<MotionTransfer>();
            avatarURL = null;
        }

        public void OnStartClick()
        {
            string url = "https://api.readyplayer.me/v1/avatars/6230ecd2cc9780a069f9852c.glb";
            if (url != avatarURL)
            {
                avatarLoader.LoadAvatar(url, OnAvatarImported, OnAvatarLoaded);
                avatarURL = url;
            }
        }

        private void OnAvatarImported(GameObject avatar)
        {
            Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        {
            Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");

            Vector3 pos = playerAvatar.transform.position;
            Vector3 rot = playerAvatar.transform.eulerAngles;
            Vector3 scale = playerAvatar.transform.localScale;
            Destroy(playerAvatar);
            playerAvatar = avatar;

            playerAvatar.GetComponent<Animator>().enabled = false;
            playerAvatar.transform.position = pos;
            playerAvatar.transform.eulerAngles = rot;
            playerAvatar.transform.localScale = scale;

            motionTransfer.playerAvatar = playerAvatar;
            Transform spine2Player = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
            motionTransfer.lArmPlayer = spine2Player.Find("LeftShoulder/LeftArm");
            motionTransfer.rArmPlayer = spine2Player.Find("RightShoulder/RightArm");
            motionTransfer.headPlayer = spine2Player.Find("Neck/Head");
            motionTransfer.faceMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.teethMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();
        }
    }
}
