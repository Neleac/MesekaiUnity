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

        private Vector3 defaultPos, defaultRot, defaultScl;

        void Start()
        {
            avatarLoader = new AvatarLoader();
            motionTransfer = templateAvatar.GetComponent<MotionTransfer>();
            
            defaultAvatar = playerAvatar;
            defaultPos = playerAvatar.transform.position;
            defaultRot = playerAvatar.transform.eulerAngles;
            defaultScl = playerAvatar.transform.localScale;
        }

        public void OnLoadClick()
        {
            avatarLoader.LoadAvatar(GetComponent<TMP_InputField>().text, OnAvatarImported, OnAvatarLoaded);
        }

        public void OnResetClick()
        {
            GetComponent<TMP_InputField>().text = "";
            if (playerAvatar != defaultAvatar) OnAvatarLoaded(defaultAvatar, null);
        }

        private void OnAvatarImported(GameObject avatar)
        {
            Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        {
            Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
            
            if (playerAvatar == defaultAvatar) playerAvatar.transform.position = new Vector3(0, -999, 0);
            else Destroy(playerAvatar);
            
            avatar.GetComponent<Animator>().enabled = false;

            // set avatar as motion transfer target
            Transform spine = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
            motionTransfer.tgtLArm = spine.Find("LeftShoulder/LeftArm");
            motionTransfer.tgtRArm = spine.Find("RightShoulder/RightArm");
            motionTransfer.tgtHead = spine.Find("Neck/Head");
            motionTransfer.tgtFace = avatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.tgtTeeth = avatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

            avatar.transform.position = defaultPos;
            avatar.transform.eulerAngles = defaultRot;
            avatar.transform.localScale = defaultScl;
            
            playerAvatar = avatar;
            if (playerAvatar != defaultAvatar) DontDestroyOnLoad(playerAvatar);
        }
    }
}
