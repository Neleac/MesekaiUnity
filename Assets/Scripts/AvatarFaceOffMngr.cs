using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AvatarFaceOffMngr : MonoBehaviour
{
    
    private GameObject gameCanvas;
    private TextMeshProUGUI timer;
    private TextMeshProUGUI prompt;
    private TextMeshProUGUI RPSDetection;
    private Button confirmGestureBtn;


    private float remainingTime = 10f;

    private void Awake()
    {
        gameCanvas = GameObject.Find("gameCanvas");
        confirmGestureBtn = GameObject.Find("confirmGestureButton").GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Avatar Scene Starts");

        TextMeshProUGUI[] texts = gameCanvas.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length == 3)
        {
            RPSDetection = texts[0];
            timer = texts[1];
            prompt = texts[2];
        } else
        {
            RPSDetection = texts[0];
            timer = texts[2];
            prompt = texts[3];
        }

        timer.enabled = true;
        RPSDetection.enabled = true;
        //confirmGestureBtn.SetActive(true);
        prompt.enabled = false;

        remainingTime = 10f;
        timer.enabled = true;
        timer.text = remainingTime.ToString("F2");

        confirmGestureBtn.gameObject.SetActive(true);
        confirmGestureBtn.onClick.AddListener(onClickConfirmGesture);
    }

    // Update is called once per frame
    void Update()
    {
        startCountdown();
    }


    private void startCountdown()
    {
        remainingTime -= 1 * Time.deltaTime;

        if (remainingTime < 4)
        {
            timer.faceColor = Color.red;
        }
        else
        {
            timer.faceColor = Color.black;
        }

        if (remainingTime <= 0)
        {
            //so far, no punishment
            timer.text = "0.00";
            //update prompt
            prompt.enabled = true;
            prompt.text = "Time Out!";
            prompt.faceColor = Color.red;
            //disable confirm button
            //confirmGestureBtn.SetActive(false);

        }
        else
        {
            timer.text = remainingTime.ToString("F2");
        }
    }

    public void onClickConfirmGesture()
    {
        //confirmGestureBtn.gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().name == "Avatar")
        {
            SceneManager.LoadScene("faceOffPrototype", LoadSceneMode.Single);
        }
        Debug.Log("Loading face off scene");
    }


}
