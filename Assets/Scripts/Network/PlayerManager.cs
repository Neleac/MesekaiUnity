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
        GameObject.Find("Template Avatar").GetComponent<MotionTransfer>().playerAvatar = avatar;
    }
}
