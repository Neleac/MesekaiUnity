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

        private AvatarLoader avatarLoader;
        private MotionTransfer motionTransfer;
        private TMP_InputField urlInput;

        private Vector3 avatarPos;
        private Vector3 avatarRot;
        private Vector3 avatarScl;

        void Start()
        {
            avatarLoader = new AvatarLoader();
            motionTransfer = templateAvatar.GetComponent<MotionTransfer>();
            
            defaultAvatar = playerAvatar;
            avatarPos = playerAvatar.transform.position;
            avatarRot = playerAvatar.transform.eulerAngles;
            avatarScl = playerAvatar.transform.localScale;
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
            
            if (playerAvatar == defaultAvatar) playerAvatar.transform.position = new Vector3(0, -999, 0);
            else Destroy(playerAvatar); // TODO: avatar caching
            
            avatar.GetComponent<Animator>().enabled = false;
    
            motionTransfer.playerAvatar = avatar;
            Transform spine2Player = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
            motionTransfer.lArmPlayer = spine2Player.Find("LeftShoulder/LeftArm");
            motionTransfer.rArmPlayer = spine2Player.Find("RightShoulder/RightArm");
            motionTransfer.headPlayer = spine2Player.Find("Neck/Head");
            motionTransfer.faceMeshPlayer = avatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.teethMeshPlayer = avatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

            avatar.transform.position = avatarPos;
            avatar.transform.eulerAngles = avatarRot;
            avatar.transform.localScale = avatarScl;

            playerAvatar = avatar;
        }
    }
}
