using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenScreen : MonoBehaviour
{
    public void HideShow()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
