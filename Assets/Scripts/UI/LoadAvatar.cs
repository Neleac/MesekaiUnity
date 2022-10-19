using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ReadyPlayerMe
{
    public class LoadAvatar : MonoBehaviour
    {
        [SerializeField] private GameObject playerAvatar;
        [SerializeField] private GameObject templateAvatar;
        private GameObject defaultAvatar;

        [SerializeField] private FullBodyView fullBodyView;
        private AvatarLoader avatarLoader;
        private MotionTransfer motionTransfer;
        private TMP_InputField urlInput;

        void Start()
        {
            avatarLoader = new AvatarLoader();
            motionTransfer = templateAvatar.GetComponent<MotionTransfer>();
            defaultAvatar = playerAvatar;
        }

        public void OnLoadClick()
        {
            avatarLoader.LoadAvatar(GetComponent<TMP_InputField>().text, OnAvatarImported, OnAvatarLoaded);
        }

        public void OnResetClick()
        {
            GetComponent<TMP_InputField>().text = "";
            if (playerAvatar.name != defaultAvatar.name) OnAvatarLoaded(defaultAvatar, null);
        }

        private void OnAvatarImported(GameObject avatar)
        {
            Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        {
            Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");

            avatar.transform.position = playerAvatar.transform.position;
            avatar.transform.eulerAngles = playerAvatar.transform.eulerAngles;
            avatar.transform.localScale = playerAvatar.transform.localScale;
            
            if (playerAvatar == defaultAvatar) playerAvatar.transform.position = new Vector3(0, -999, 0);
            else Destroy(playerAvatar); // TODO: avatar caching
            
            avatar.GetComponent<Animator>().enabled = false;

            // half/full body button modify loaded avatar
            fullBodyView.avatar = avatar;
    
            // map template avatar to loaded avatar
            motionTransfer.playerAvatar = avatar;
                        
            motionTransfer.spinePlayer = avatar.transform.Find("Armature/Hips/Spine");
            motionTransfer.spine1Player = motionTransfer.spinePlayer.Find("Spine1");
            motionTransfer.spine2Player = motionTransfer.spine1Player.Find("Spine2");

            motionTransfer.lArmPlayer = motionTransfer.spine2Player.Find("LeftShoulder/LeftArm");
            motionTransfer.rArmPlayer = motionTransfer.spine2Player.Find("RightShoulder/RightArm");
            motionTransfer.headPlayer = motionTransfer.spine2Player.Find("Neck/Head");
            motionTransfer.faceMeshPlayer = avatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.teethMeshPlayer = avatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

            playerAvatar = avatar;
        }
    }
}
