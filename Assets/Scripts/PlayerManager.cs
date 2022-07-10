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
        GameObject avatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);
        avatar.GetComponent<FaceSolver>().enabled = false;
        avatar.GetComponent<PoseSolver>().enabled = false;
        avatar.GetComponent<HandSolver>().enabled = false;

        GameObject.Find("Template Avatar").GetComponent<MotionTransfer>().playerAvatar = avatar;
    }
}
