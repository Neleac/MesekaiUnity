using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepThisObject : MonoBehaviour
{
    public static keepThisObject Instance;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(this.gameObject);
            Debug.Log("destroyed unnecessary canvas");
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("called dont destroy");
        }

       
    }
}
