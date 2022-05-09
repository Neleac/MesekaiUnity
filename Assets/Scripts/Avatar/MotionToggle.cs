using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotionToggle : MonoBehaviour
{
    private Animator animator;
    private Vector3 position;
    private float idleTime;

    [SerializeField] private float secondsToMotion;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        }
        
        if (idleTime >= secondsToMotion) animator.enabled = false;
    }

    // public void OnMove(InputValue value)
    // {
    //     animator.enabled = true;
    // }
}
