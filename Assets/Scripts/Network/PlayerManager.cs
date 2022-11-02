using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject avatarPrefab;

    void Awake()
    {
        GameObject playerAvatar = GameObject.Find("Custom Avatar");    // if custom avatar is available
        
        GameObject defaultAvatar;
        if (playerAvatar == null)
        {
            defaultAvatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);
        }
        else
        {
            // instantiate custom avatar with RPM url for other clients to load the avatar
            object[] instData = {(object)playerAvatar.GetComponent<AvatarURL>().url};
            defaultAvatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity, 0, instData);
        }

        // setup motion transfer
        MotionTransfer motionTransfer = GameObject.Find("Template Avatar").GetComponent<MotionTransfer>();
        motionTransfer.networkPlayer = defaultAvatar.GetComponent<NetworkPlayer>();

        if (playerAvatar == null)
        {
            playerAvatar = defaultAvatar;
        }
        else
        {
            // transfer animations
            Animator animator = playerAvatar.GetComponent<Animator>();
            AnimationTransfer animTransfer = defaultAvatar.GetComponent<AnimationTransfer>();
            animTransfer.tgt = animator;
            animTransfer.SetController();
            animator.enabled = true;
            animTransfer.enabled = true;

            // hide default avatar
            Component[] meshes = defaultAvatar.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
            foreach (SkinnedMeshRenderer mesh in meshes) mesh.enabled = false;

            // parent to default avatar
            playerAvatar.transform.parent = defaultAvatar.transform;
            playerAvatar.transform.localPosition = Vector3.zero;
            playerAvatar.transform.localRotation = Quaternion.identity;
            playerAvatar.transform.localScale = Vector3.one;
        }

        Transform spine = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        motionTransfer.tgtLArm = spine.Find("LeftShoulder/LeftArm");
        motionTransfer.tgtRArm = spine.Find("RightShoulder/RightArm");
        motionTransfer.tgtHead = spine.Find("Neck/Head");
        motionTransfer.tgtFace = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
        motionTransfer.tgtTeeth = playerAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();
    }
}
