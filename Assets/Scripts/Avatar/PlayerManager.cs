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

        mediapipeSolution.GetComponent<HolisticTrackingSolution>().faceSolver = avatar.GetComponent<FaceSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().poseSolver = avatar.GetComponent<PoseSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().handSolver = avatar.GetComponent<HandSolver>();
    }

    private void toggleJointSync(GameObject joint, bool enable)
    {
        // // add components during initial instantiation
        // if (joint.GetComponent<PhotonView>() == null)
        // {
        //     PhotonView pv = joint.AddComponent(typeof(PhotonView)) as PhotonView;
        //     PhotonTransformView ptv = joint.AddComponent(typeof(PhotonTransformView)) as PhotonTransformView;

        //     joint.GetComponent<PhotonView>().ObservedComponents.Add(joint.GetComponent<PhotonTransformView>());
        // }

        // toggle joint sync components
        joint.GetComponent<PhotonView>().enabled = enable;
        joint.GetComponent<PhotonTransformView>().enabled = enable;

        // DFS through children
        foreach (Transform child in joint.transform)
        {
            toggleJointSync(child.gameObject, enable);
        }
    }
}
