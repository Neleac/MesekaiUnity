using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject escapeMenu;
    [SerializeField] GameObject lobbyMenu;
    [SerializeField] GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKey(KeyCode.Escape))
       {
            ShowMenu(escapeMenu);
       }
    }

    public void ShowMenu(GameObject menu)
    {
        escapeMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        settingsMenu.SetActive(false);
        menu?.SetActive(true);
    }

    public void HideMenu()
    {
        escapeMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
