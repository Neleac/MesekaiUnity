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
        GameObject defaultAvatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);
        GameObject playerAvatar = GameObject.Find("Avatar");
        MotionTransfer motionTransfer = GameObject.Find("Template Avatar").GetComponent<MotionTransfer>();
        
        if (playerAvatar == null)
        {
            playerAvatar = defaultAvatar;
        }
        else
        {
            // custom avatar
            Animator animator = playerAvatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = defaultAvatar.GetComponent<Animator>().runtimeAnimatorController;
            animator.enabled = true;
            defaultAvatar.GetComponent<ThirdPersonController>().transferAnimator = animator;

            // hide default avatar
            Component[] meshes = defaultAvatar.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
            foreach (SkinnedMeshRenderer mesh in meshes) mesh.enabled = false;

            // parent to default avatar
            playerAvatar.transform.parent = defaultAvatar.transform;
            playerAvatar.transform.localPosition = Vector3.zero;
            playerAvatar.transform.localRotation = Quaternion.identity;
            playerAvatar.transform.localScale = Vector3.one;
        }

        // set motion transfer targets
        Transform spine = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        motionTransfer.tgtLArm = spine.Find("LeftShoulder/LeftArm");
        motionTransfer.tgtRArm = spine.Find("RightShoulder/RightArm");
        motionTransfer.tgtHead = spine.Find("Neck/Head");
        motionTransfer.tgtFace = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
        motionTransfer.tgtTeeth = playerAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();
    }
}
