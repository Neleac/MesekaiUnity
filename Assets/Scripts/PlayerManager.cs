using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Mediapipe.Unity.Holistic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject avatarPrefab;
    [SerializeField] private GameObject mediapipeSolution;

    void Start()
    {
        GameObject avatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);
    
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().poseSolver = avatar.GetComponent<PoseSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().handSolver = avatar.GetComponent<HandSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().faceSolver = avatar.GetComponent<FaceSolver>();
    }
}
