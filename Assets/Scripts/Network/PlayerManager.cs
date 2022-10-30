using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject avatarPrefab;

    void Awake()
    {
        GameObject defaultAvatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);

        MotionTransfer motionTransfer = GameObject.Find("Template Avatar").GetComponent<MotionTransfer>();
        
        GameObject customAvatar = GameObject.Find("Avatar");
        if (customAvatar == null)
        {
            // default avatar

            // set motion transfer targets
            motionTransfer.tgtJoints = new Transform[] {defaultAvatar.transform.Find("Armature/Hips/Spine")};
            motionTransfer.tgtFace = defaultAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.tgtTeeth = defaultAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();
        }
        else
        {
            // custom avatar

            // set motion transfer targets
            motionTransfer.tgtJoints = new Transform[] {customAvatar.transform.Find("Armature/Hips/Spine")};
            motionTransfer.tgtFace = customAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
            motionTransfer.tgtTeeth = customAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

            // hide default avatar
            Component[] meshes = defaultAvatar.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
            foreach (SkinnedMeshRenderer mesh in meshes) mesh.enabled = false;

            // parent to default avatar
            customAvatar.transform.parent = defaultAvatar.transform;
            customAvatar.transform.localPosition = Vector3.zero;
            customAvatar.transform.localRotation = Quaternion.identity;
            customAvatar.transform.localScale = Vector3.one;
        }
    }
}
