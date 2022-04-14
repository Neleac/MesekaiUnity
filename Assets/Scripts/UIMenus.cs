using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class UIMenus : MonoBehaviour
{
    private GameObject escMenu;
    private GameObject settingsMenu;
    private GameObject currMenu;

    void Start()
    {
        escMenu = transform.Find("ESC Menu").gameObject;
        settingsMenu = transform.Find("Settings Menu").gameObject;
        currMenu = null;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && currMenu == null) 
        {
            escMenu.SetActive(true);
            currMenu = escMenu;
        }
    }

    public void Resume()
    {
        escMenu.SetActive(false);
        settingsMenu.SetActive(false);
        currMenu = null;
    }

    public void SwitchMenu(GameObject menu)
    {
        currMenu.SetActive(false);
        menu.SetActive(true);
        currMenu = menu;
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
