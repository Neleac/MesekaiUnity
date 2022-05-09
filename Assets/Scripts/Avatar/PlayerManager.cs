using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Mediapipe.Unity.Holistic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject avatar;
    //[SerializeField] private GameObject mediapipeSolution;

    void Start()
    {
        PhotonNetwork.Instantiate(avatar.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);

        // mediapipeSolution.GetComponent<HolisticTrackingSolution>().faceSolver = myAvatar.GetComponent<FaceSolver>();
        // mediapipeSolution.GetComponent<HolisticTrackingSolution>().poseSolver = myAvatar.GetComponent<PoseSolver>();
        // mediapipeSolution.GetComponent<HolisticTrackingSolution>().handSolver = myAvatar.GetComponent<HandSolver>();
    }
}
