using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI prompt;
    private float remainingTime = 10f;

    void Start()
    {
        timer.text = remainingTime.ToString("F2");
    }


    void Update()
    {

        remainingTime -= 1 * Time.deltaTime;

        if (remainingTime < 4)
        {
            timer.faceColor = Color.red;

        }

        if (remainingTime <= 0)
        {
            timer.text = "0.00";
            GameObject.Find("RockButton").SetActive(false);
            GameObject.Find("PaperButton").SetActive(false);
            GameObject.Find("ScissorsButton").SetActive(false);
            prompt.text = "Time Out!";
        }
        else
        {
            timer.text = remainingTime.ToString("F2");
        }
    }
}