using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
    {
    public GameObject guiObject;
    public string StartSc;

    private GameObject _player;

    void Start()
        {
        guiObject.SetActive(false);
        }

    // Update is called once per frame
    void OnTriggerEnter(Collider plyr)
        {
        if (plyr.gameObject.tag == "Player")
            {
            _player = plyr.gameObject;
            guiObject.SetActive(true);
            }
        }

    void Update()
        {
        if (_player)
            {
            if (Input.GetButtonDown("Use"))
                {
                Debug.Log("Test!!!");
                SceneManager.LoadScene(StartSc);
                }
            }
        }

    void OnTriggerExit(Collider plyr)
        {
        //if (plyr.gameObject.tag == "Player")
            {
            _player = null;
            guiObject.SetActive(false);
            }
        }

    private void OnDrawGizmos()
        {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }