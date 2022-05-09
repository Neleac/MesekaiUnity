using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRotator : MonoBehaviour
    {
    public float speed;
    public CinemachineVirtualCamera virtualCamera;
    public Transform follow;

    private float _timer = 0.0f;

    ///Update is called once per frame
    void Update()
    {
        _timer = _timer + 1 * Time.deltaTime;
        // Debug.Log(_timer);

        if (_timer >= 30.0f)
        {
            virtualCamera.m_Follow = follow;
        }
        else
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }

    }


}
