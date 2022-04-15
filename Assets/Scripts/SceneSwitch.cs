using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public GameObject guiObject;
    public string StartSc;

    void Start()
        {
        guiObject.SetActive(false);
        }

    // Update is called once per frame
    void OnTriggerEnter(Collider plyr)
        {
        if (plyr.gameObject.tag == "Player")
            {
            guiObject.SetActive(true);
            if (Input.GetKey("e"))
                {
                SceneManager.LoadScene(StartSc);
                }
            }
        }
    void OnTriggerExit(Collider plyr)
        {
        //if (plyr.gameObject.tag == "Player")
            {
            guiObject.SetActive(false);
            }
        }
    }
