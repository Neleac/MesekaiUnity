using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class MotionToggle : MonoBehaviour
{
    private Animator animator;
    private PhotonAnimatorView PAV;

    private Vector3 position;
    private float idleTime;

    [SerializeField] private float secondsToMotion;

    void Start()
    {
        animator = GetComponent<Animator>();
        PAV = GetComponent<PhotonAnimatorView>();

        position = transform.position;
        idleTime = 0;
    }

    void Update()
    {
        Vector3 prevPos = position;
        Vector3 currPos = transform.position;

        if (prevPos == currPos)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            position = currPos;
            idleTime = 0;

            animator.enabled = true;
            PAV.enabled = true;
        }
        
        if (idleTime >= secondsToMotion)
        {
            animator.enabled = false;
            PAV.enabled = false;
        }
    }
}
