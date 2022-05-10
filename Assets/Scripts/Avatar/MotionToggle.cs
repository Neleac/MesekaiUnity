using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class MotionToggle : MonoBehaviourPun
{
    private Animator animator;
    private PhotonAnimatorView PAV;

    private Vector3 prevPos;
    private float idleTime;

    [SerializeField] private float secondsToMotion;
    [SerializeField] private Transform playerSpine;
    private GameObject templateAvatar;
    private Transform templateSpine;

    void Start()
    {
        animator = GetComponent<Animator>();
        PAV = GetComponent<PhotonAnimatorView>();

        prevPos = transform.position;
        idleTime = 0;

        templateAvatar = GameObject.Find("Caelen");
        templateSpine = templateAvatar.transform.Find("Hips/Spine/Spine1/Spine2");
    }

    void Update()
    {
        Vector3 currPos = transform.position;

        if (prevPos == currPos)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            prevPos = currPos;
            idleTime = 0;

            animator.enabled = true;
            PAV.enabled = true;
        }
        
        if (idleTime >= secondsToMotion)
        {
            animator.enabled = false;
            PAV.enabled = false;

            if (photonView.IsMine) mapJointRotation(playerSpine, templateSpine);
        }
    }

    private void mapJointRotation(Transform playerJoint, Transform templateJoint)
    {       
        playerJoint.localRotation = templateJoint.localRotation;

        foreach (Transform playerChild in playerJoint)
        {
            Transform templateChild = templateJoint.Find(playerChild.name);
            mapJointRotation(playerChild, templateChild);
        }
    }
}
