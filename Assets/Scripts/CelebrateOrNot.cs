using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelebrateOrNot : MonoBehaviour
    {
    private Animator _animator;

    void Start()
        {
            _animator = GetComponent<Animator>();
        //Debug.Log("Celebrate");
        }

    void Update()
        {
        if (Input.GetKeyDown(KeyCode.W))
            {
            _animator.SetBool("IsMatchOver", true);
            _animator.SetBool("IsWinner", true);
            //Debug.Log("1");
            }

        if (Input.GetKeyDown(KeyCode.L))
            {
            _animator.SetBool("IsMatchOver", true);
            _animator.SetBool("IsWinner", false);
            //Debug.Log("2");
            }
        }
    }

