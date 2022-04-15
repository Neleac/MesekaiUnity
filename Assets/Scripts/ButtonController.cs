using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public TextMeshProUGUI result;
    public TextMeshProUGUI prompt;

    // Start is called before the first frame update
    void Start()
    {
        prompt.enabled = true;
        result.enabled = false;
    }

    public void getResult()
    {
        string choice = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        result.enabled = true;
        result.text = "You: " + choice + "\n AI: ";
        prompt.enabled = false;
        GameObject.Find("RockButton").SetActive(false);
        GameObject.Find("PaperButton").SetActive(false);
        GameObject.Find("ScissorsButton").SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }


}
