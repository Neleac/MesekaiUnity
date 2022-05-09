using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class AvatarFaceOffMngr : MonoBehaviour
{
    public GameObject cameraRotator;
    public GameObject surroundingCam;

    [Range(0, 1)]
    public int UIgameSelector;                                     //to switch from games for test from UI easily

    //avatar position and rotation
    public GameObject avatar;
    private Vector3 playerPosition = new Vector3(0, -0.15f, 11);   //avatar position remain the same 
    private Vector3 faceOffPlayerRot = new Vector3(0, 180, 0);     //avatar rotate in different games
    private Vector3 hswPlayerRot = new Vector3(0, -20, 0);

    // camera position and rotation
    public Camera mainCam;
    private Vector3 CameraRotation = new Vector3(0, 180, 0);       //camera rotation remains the same 
    private Vector3 faceoffCamPos = new Vector3(-0.5f, 1, 11.65f); //camera pose in different places in each game
    private Vector3 hswCamPos = new Vector3(-0.3f, 1.55f, 11.6f);

    //gameCanvas Components
    public GameObject gameCanvas;
    private TextMeshProUGUI detection;                             // to show the captured gesture/facial
    public Button confirmBtn;                                      // to ask for the player to confirm the result
    public TextMeshProUGUI timer;                                  // to show the count down timer
    public TextMeshProUGUI prompt;                                 // to show assistant info: timeout, network waiting, invalid player result, etc
    public TextMeshProUGUI roundField;                             // to show the round #
    public TextMeshProUGUI gameHistory;                            // to show the history result                

    //resultCanvas components
    public GameObject resultCanvas;
    public TextMeshProUGUI resultField;                            // to show the result of this round
    public TextMeshProUGUI finalResult;                            // to show the final game result--win/lose
    public GameObject nextRoundButton;                             // to navigate to the next round
    public GameObject exitButton;
    public TextMeshProUGUI gameRecap;                              // to show the final recap when game ends

    //canvas info fields
    private float remainingTime = 10f;
    private const int TOTALROUNDS = 3;
    private int currentRound = 0;
    private int[] historyResults = new int[TOTALROUNDS];           // to store all the round results of the first 3 rounds
    private string gameHistoryText = "";                           // to show history on board
    private bool isFinalRound = false;                             // to indicate if it's the final round for final display

    private int gameCanvasLayer;
    private Color timerColor;
    private bool timeOut = false;                                  // to avoid repeated time-out action
    public enum MiniGame { FaceOff, HSW }
    private enum FaceOffChoices { Rock, Paper, Scissors, Invalid }
    private enum HSWChoices { Happy, Wow, Sad, Invalid}
    private MiniGame miniGame;                                     // to store the game name for choosing game accordingly TODO: need decision logic
    const int INVALID = 3;
    const int GAMENUM = 2;                                         // 2 minigames so far, not used
    const int GAMECHOICE = 4;                                      // 4 player choices: 3 valid + 1 invalid
    private string[] userChoice = new string[GAMECHOICE];          // to store the valid player choices string in each game type 


    //decide which game is playing
    private void Awake()
    {
        miniGame = (MiniGame)UIgameSelector;// may need a global variable to pass in
        setGame(miniGame);
    }

    // 1. save global varibale; 2. hide result canvas; 3. show game canvas
    void Start()

    {
        //save the global variables for future use
        timerColor = timer.faceColor;
        gameCanvasLayer = gameCanvas.GetComponent<Canvas>().sortingOrder;

        StartCoroutine(waitForCamRotate());

        //Debug.Log(miniGame + " Scene Starts");

        ////save the global variables for future use
        //timerColor = timer.faceColor;
        //gameCanvasLayer = gameCanvas.GetComponent<Canvas>().sortingOrder;

        //showGameResultCanvas(false);
        //showGameCanvas(true);

    }

    // Updates: 1. count-down; 2. keyboard input
    void Update()
    {
        startCountdown();
        //isOpponentReady(); //call this func when network is done
        //USAGE: "enter" triggers "confirm"; "space" triggers "exit" & "next round"
        enableKeyboardBtnTrigger();

    }




    /***********************CANVAS DISPLAY FUNCTIONS***************************/
    //1. shows/hides the game canvas; 2. updates timer & round#; 3 hides unnecessary components
    private void showGameCanvas(bool toShow)
    {
        gameCanvas.SetActive(toShow);

        if (toShow)
        {
            setTimer(currentRound);
            
            //update rounds
            updateRoundField();

            //disable the prompt, and gameHistory conditionally
            prompt.enabled = false;
            gameHistory.enabled = (currentRound != 1);
        }
    }

    //1. shows/hides the gameResult canvas; 2. hides unnecessary components
    private void showGameResultCanvas(bool toShow)
    {
        resultCanvas.SetActive(toShow);
        if (toShow)
        {
            resultCanvas.GetComponent<Canvas>().sortingOrder = gameCanvasLayer + 1;


            if (isFinalRound)
            {
                finalResult.enabled = true;
                displayGameRecap();
                exitButton.SetActive(true);
            }
            else
            {
                exitButton.SetActive(false);
                finalResult.enabled = false;
                gameRecap.enabled = false;
            }
        }
        else
        {
            resultCanvas.GetComponent<Canvas>().sortingOrder = gameCanvasLayer - 1;
        }
    }
    /**************************************************************************/





    /*************************BUTTON EVENT LISTENER****************************/
    //plays game if gesture valid
    public void onClickConfirm()
    {
        Debug.Log("Game Starts, Round: " + currentRound);
        //if there's more time to get a valid choice
        if (!timer.text.Equals("0.00") && !isValidChoice())
        {
            prompt.enabled = true;
            prompt.text = "Invalid Choice, please confirm again!";
            
        } else //it's time's up, pass in whatever choice it is 
        {
            Debug.Log("choice confirmed: " +detection.text);
            if (isOpponentReady())
            {
                showGameCanvas(false);

                updateResultCanvasInfo();

                showGameResultCanvas(true);
            }
            else
            {
                prompt.enabled = true;
                prompt.text = "waiting for another player...";
            }
        }
   
    }

    //starts the next round
    public void onClickNextRound()
    {
        Debug.Log("Current round: " + currentRound + " ends. Next round starts. ");
        showGameResultCanvas(false);
        showGameCanvas(true);
    }

    //switches to the hub world
    public void onClickExitButton()
    {
        Debug.Log("exiting to hub");
        SceneManager.LoadScene("Hub");

    }
    /**************************************************************************/





    /*************************GAME LOGIC FUNCTIONS*****************************/
    //core game playing process:
    //1. get player choice
    //2. compute round results and uodate
    //3. store and update history info
    //4. decide if final round is needed
    //5. get and update final game result
    public void updateResultCanvasInfo()
    {
        //Debug.Log("Game Starts, Round: " + currentRound);

        //get player and Ai choice
        string playerChoice = getPlayerChoice();
        int playerInt = choiceEncode(playerChoice);
        int aiInt = generateAiResult();

        //game logic
        int roundResultInt = playGame(playerInt, aiInt);

        //display current round result
        displayRoundResult(playerInt, aiInt, roundResultInt);

        //if in round 1 & 2 & 3
        if (currentRound <= TOTALROUNDS)
        {
            updateAndDisplayHistoryInfo(roundResultInt);

            //show nextRoundButton
            nextRoundButton.SetActive(true);
        }

        //do the following only in round 3 and above
        if (currentRound >= TOTALROUNDS)
        {

            //test if final round is needed
            if (needAdditionalRound(roundResultInt))
            {
                nextRoundButton.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Final Round";
                nextRoundButton.SetActive(true);
            }
            else
            {
                //disable the button
                nextRoundButton.SetActive(false);
                getFinalResult(roundResultInt);
                isFinalRound = true;
            }
        }
    }

    private string getPlayerChoice()
    {
        return detection.text;
    }

    //TEST FUNCTION
    private int generateAiResult()
    {
        return 1; // always paper
        int aiResult = 0;
        var rnd = new System.Random();
        aiResult = rnd.Next(1, 3);
        return aiResult;
    }

    //returns REAL round result: win/lose/tie
    private int playGame(int playerInt, int aiInt)
    {
        //decode table: 0 == Rock/Happy; 1 == Paper/Sad; 2 == Scissors/Wow; 3 == INVALID
        bool playerWins = (playerInt == 0 && aiInt == 2)
                          || (playerInt == 1 && aiInt == 0)
                          || (playerInt == 2 && aiInt == 1)
                          || (playerInt != INVALID && aiInt == INVALID);

        if (aiInt == playerInt) 
        {
            return 0; //it's a tie
        }
        else if (playerWins)
        {
            return 1; //player wins
        }
        else
        {
            return -1; //player loses
        }
    }

    //displays the round result in result field
    private void displayRoundResult(int playerInt, int aiInt, int roundResultInt)
    {
        string playerChoice = decodeToChoice(playerInt);
        string aiChoice = decodeToChoice(aiInt);
        string roundResultText = decodeToResultText(roundResultInt);

        resultField.enabled = true;
        resultField.text = "You: " + playerChoice + ", AI: " + aiChoice + "\n" + roundResultText;
    }

    //computes the game's final score when no more additional round needed
    private int getFinalResultInt(int roundResultInt)
    {
        if (currentRound == 3)
        {
            int score = get3RoundsSum();
            int finalResultInt = 0;
            if (score == 0)
            {
                finalResultInt = 0;
            }
            else if (score > 0)
            {
                finalResultInt = 1;
            }
            else
            {
                finalResultInt = -1;
            }
            return finalResultInt;
        }
        else
        {
            return roundResultInt;
        }


    }

    //displays the final result in finalResult field
    private void getFinalResult(int roundResultInt)
    {
        finalResult.text = "You " + decodeToResultText(getFinalResultInt(roundResultInt));
        Debug.Log(finalResult.text);
    }

    //displays the game history in gameRecap field
    private void displayGameRecap()
    {
        gameRecap.enabled = true;
        gameRecap.text = gameHistoryText;
    }
    /**************************************************************************/





    /*************************COLLECT HISTORY INFO*****************************/
    //1. updates gameHistory int & text; 2. displays gameHistory
    private void updateAndDisplayHistoryInfo(int roundResultInt)
    {
        
        if (!gameHistory.enabled)
        {
            gameHistory.enabled = true;
        }
        updateGameHistory(roundResultInt);//store the round result
        updateGameHistoryText(roundResultInt);
    }

    //stores the history results in int
    private void updateGameHistory(int resultInt)
    {
        historyResults[currentRound - 1] = resultInt;
    }

    //stores the history results in text
    private void updateGameHistoryText(int roundResultInt)
    {
        string roundResultText = decodeToResultText(roundResultInt);
        if (currentRound == 1)
        {
            gameHistoryText = "Round 1: " + roundResultText;
        }
        else
        {
            gameHistoryText += "\nRound " + currentRound + ": " + roundResultText;
        }
        gameHistory.text = gameHistoryText;
    }
    /**************************************************************************/





    /**********************ADDITIONAL ROUND FUNCTIONS**************************/
    private bool needAdditionalRound(int resultInt)
    {
        if (currentRound == 3)
        {
            if (get3RoundsSum() != 0)
            {
                return false;//no need additional round if the 3-rounds-result indicates a winner
            }
            return true;
        }
        else
        {
            if (resultInt == 0)
            {
                return true;//need additional round is the extra round has no winner still
            }
            return false;
        }

    }

    private int get3RoundsSum()
    {
        if (currentRound == 3)
        {
            int sum = 0;
            foreach (var item in historyResults)
            {
                sum += item;
            }
            return sum;
        }

        Debug.Log("Error: access 3-rounds-result at round: " + currentRound);
        return Int32.MinValue;
    }
    /**************************************************************************/





    /*********************GAME CANVAS DEFAULT FUNCTIONS************************/
    // update the time before curRound# is updated
    // timer is set to be half of the previous round
    private void setTimer(int preUpdateRound)
    {
        timeOut = false;
        if (preUpdateRound <= 3)
        {
            int denominator = (int)Math.Pow(2, preUpdateRound);
            remainingTime = (float)Math.Round(10f / denominator);
        }
        else
        {
            remainingTime = 1f;
        }

        timer.enabled = true;
    }

    //increments the currentRound and controls roundField Display
    private void updateRoundField()
    {
        currentRound += 1;
        if (currentRound <= TOTALROUNDS)
        {
            roundField.text = "Round " + currentRound;
        }
        else
        {
            roundField.text = "Final Round";
        }
    }
    /**************************************************************************/





    /**********************DECODING/ENCODING FUNCTIONS*************************/
    private string decodeToChoice(int playerInt)
    {
        return userChoice[playerInt];
    }

    //invalid results -> 3
    private int choiceEncode(string playerText)
    {
        
        for(int i = 0; i < GAMECHOICE - 1; i++)
        {
            if (userChoice[i].Equals(playerText))
            {
                return i;
            }
        }

        Debug.Log("Error! un-recognized player choice: " + playerText);
        return INVALID;

    }

    private string decodeToResultText(int resultInt)
    {
        string roundResultText = "";

        if (resultInt == 0)
        {
            roundResultText = "Tie";
        }
        else if (resultInt == 1)
        {
            roundResultText = "Win";
        }
        else if (resultInt == -1)
        {
            roundResultText = "Lose";
        }
        else
        {
            Debug.Log("Error, non-recognized round result: " + resultInt);
        }

        return roundResultText;
    }
    /**************************************************************************/





    /**************************VALIDATION FUNCTIONS****************************/
    private bool isValidChoice()
    {
        if (choiceEncode(detection.text) == INVALID)
        {
            return false;
        }
        return true;

        //if (RPSDetection.text == "Rock" || RPSDetection.text == "Paper" || RPSDetection.text == "Scissors")
        //{
        //    return true;
        //}
        //return false;
    }

    private bool isOpponentReady()
    {
        return true;
    }
    /**************************************************************************/





    /***************************UPDATE FUNCTIONS*******************************/
    //timer control block: 1. display content; 2. display color; 3. time-out action
    private void startCountdown()
    {
        remainingTime -= 1 * Time.deltaTime;

        //display color
        if (remainingTime < 4)
        {
            timer.faceColor = Color.red;
        }
        else
        {
            timer.faceColor = timerColor;
        }

        //display content
        if (remainingTime <= 0 && gameCanvas.activeInHierarchy)
        {
            if (!timeOut)
            {
                timeOut = true;
                timer.text = "0.00";
                confirmBtn.onClick.Invoke();
            }
            //do nothing, timer would be reset every time game canvas is shown, aka game is playing
        }
        else
        {
            timer.text = remainingTime.ToString("F2");
        }
    }

    //to allow mouse-less interation with keyboard input
    private void enableKeyboardBtnTrigger()
    {
        //CAUTION: in the play-focused mode, mouse click to focus the screen is needed before first-pressed key
        //while in the play-maximize mode, keyboard inputs works fine

        //USAGE: "enter" triggers "confirm"; "space" triggers "exit" & "next round"
        if (Input.GetKeyUp(KeyCode.Return) && confirmBtn.IsActive())
        {
            confirmBtn.onClick.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (nextRoundButton.GetComponent<Button>().IsActive())
            {
                onClickNextRound();
            }
            else if (exitButton.GetComponent<Button>().IsActive())
            {
                onClickExitButton();
            }
        }
    }
    /**************************************************************************/





    /************************GAME SETTING FUNCTIONS****************************/
    private void setGame(MiniGame gameName)
    {
        if(gameName == MiniGame.FaceOff)
        {
            setAvatarPosition(MiniGame.FaceOff);
            setCamPosition(MiniGame.FaceOff);
            avatar.GetComponent<HandSolver>().enabled = true;
            avatar.GetComponent<FaceSolver>().enabled = false;

            detection = GameObject.Find("gestureDetection").GetComponent<TextMeshProUGUI>();
            GameObject.Find("emotionDetection").SetActive(false);
            userChoice = Enum.GetNames(typeof(FaceOffChoices));
        }
        else if (gameName == MiniGame.HSW)
        {
            setAvatarPosition(MiniGame.HSW);
            setCamPosition(MiniGame.HSW);
            avatar.GetComponent<HandSolver>().enabled = false;
            avatar.GetComponent<FaceSolver>().enabled = true;

            detection = GameObject.Find("emotionDetection").GetComponent<TextMeshProUGUI>();
            GameObject.Find("gestureDetection").SetActive(false);
            userChoice = Enum.GetNames(typeof(HSWChoices));
        }
        else
        {
            //TODO: consider throw an error
            Debug.Log("Error! Setting game view for un-recognized mini game" + gameName);
        }
        
    }

    private void setAvatarPosition(MiniGame gameName)
    {
        avatar.transform.position = playerPosition;
        if (gameName == MiniGame.FaceOff)
        {
            avatar.transform.eulerAngles = faceOffPlayerRot;
        }
        else if (gameName == MiniGame.HSW)
        {
            avatar.transform.eulerAngles = hswPlayerRot;
        }
        else
        {
            Debug.Log("Error! Setting player pos for un-recognized mini game" + gameName);
        }
    }

    private void setCamPosition(MiniGame gameName)
    {
        mainCam.transform.eulerAngles = CameraRotation;
        if (gameName == MiniGame.FaceOff)
        {
            mainCam.transform.position = faceoffCamPos;
        }
        else if (gameName == MiniGame.HSW)
        {
            mainCam.transform.position = hswCamPos;
        }
        else
        {
            Debug.Log("Error! Setting camera pos for un-recognized mini game" + gameName);
        }
    }
    /**************************************************************************/

    private void turnOnRotatingCam(bool toShow)
    {
        mainCam.gameObject.SetActive(!toShow);

        surroundingCam.SetActive(toShow);
        cameraRotator.SetActive(toShow);
        cameraRotator.GetComponent<CameraRotator>().enabled = toShow;
    }


    IEnumerator waitForCamRotate()
    {

        gameCanvas.SetActive(false);
        resultCanvas.SetActive(false);

        turnOnRotatingCam(true);
        yield return new WaitForSeconds(8);
        turnOnRotatingCam(false);

        Debug.Log(miniGame + " Scene Starts");

        showGameResultCanvas(false);
        showGameCanvas(true);
    }
}
