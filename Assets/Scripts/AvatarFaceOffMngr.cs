using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class AvatarFaceOffMngr : MonoBehaviour
{
    
    public GameObject gameCanvas;
    public TextMeshProUGUI timer;                      //to show the count down timer
    public TextMeshProUGUI prompt;                     //to show assistant info: timeout, network waiting, invalid player result, etc
    public TextMeshProUGUI RPSDetection;               //to show the captured hand gesture
    public TextMeshProUGUI roundField;                 //to show the round #
    public TextMeshProUGUI gameHistory;                //to show the history result
    public Button confirmGestureBtn;                   //to ask for the player to confirm the result

    public GameObject resultCanvas;
    public TextMeshProUGUI resultField;                 //to show the result of this round
    public TextMeshProUGUI finalResult;                 //to show the final game result--win/lose
    public GameObject nextRoundButton;                  //to navigate to the next round
    public GameObject exitButton;

    private float remainingTime = 10f;
    private const int TOTALROUNDS = 3;
    private int currentRound = 0;
    private int[] historyResults = new int[TOTALROUNDS]; //to store all the round results of the first 3 rounds
    private string gameHistoryText = "";                 //to show history on board
    private int gameCanvasLayer;
    private Color timerColor;
    private bool timeOut = false;                        // to avoid repeated time-out action


    // 1. save global varibale; 2. hide result canvas; 3. show game canvas
    void Start()
    {
        Debug.Log("Arena Scene Starts");

        //save the global variables for future use
        timerColor = timer.faceColor;
        gameCanvasLayer = gameCanvas.GetComponent<Canvas>().sortingOrder;

        showGameResultCanvas(false);
        showGameCanvas(true);

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
            //disable the prompt, gameHistory and final result
            prompt.enabled = false;
            finalResult.enabled = false;
            //update rounds
            updateRoundField();
        }
    }

    //1. shows/hides the gameResult canvas; 2. hides unnecessary components
    private void showGameResultCanvas(bool toShow)
    {
        resultCanvas.SetActive(toShow);
        if (toShow)
        {
            resultCanvas.GetComponent<Canvas>().sortingOrder = gameCanvasLayer + 1;
            exitButton.SetActive(false);
        }
        else
        {
            resultCanvas.GetComponent<Canvas>().sortingOrder = gameCanvasLayer - 1;
        }
    }
    /**************************************************************************/





    /*************************BUTTON EVENT LISTENER****************************/
    //plays game if gesture valid
    public void onClickConfirmGesture()
    {
        //if there's more time and invalid gesture
        if (!timer.text.Equals("0.00") && !isValidGesture())
        {
            prompt.enabled = true;
            prompt.text = "Invalid Gesture, please confirm again!";
            
        } else //it's time's up
        {
            Debug.Log("gesture confirmed");
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
        }//TODO if not valid & timeout = lose
   
    }

    //starts the next round
    public void onClickNextRound()
    {
        Debug.Log("next round");
        showGameResultCanvas(false);
        showGameCanvas(true);
    }

    //switches to the hub world
    public void onClickExitButton()
    {
        Debug.Log("exiting to hub");
        if (SceneManager.GetActiveScene().name == "Arena")
        {
            SceneManager.LoadScene("Hub");
        }
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
        Debug.Log("Game Starts, Round: " + currentRound);

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

                displayFinalResult(roundResultInt);
                exitButton.SetActive(true);
            }
        }
    }

    private string getPlayerChoice()
    {
        return RPSDetection.text;
    }

    //TEST FUNCTION
    private int generateAiResult()
    {
        return -1; // always paper
        int aiResult = 0;
        var rnd = new System.Random();
        aiResult = rnd.Next(1, 3);
        return aiResult;
    }

    //returns REAL round result: win/lose/tie
    private int playGame(int playerInt, int aiInt)
    {
        //decode table: 1 == Rock; 2 == Paper; 3 == Scissors; -1 == INVALID
        bool playerWins = (playerInt == 1 && aiInt == 3)
                          || (playerInt == 2 && aiInt == 1)
                          || (playerInt == 3 && aiInt == 2)
                          || (playerInt > 0 && aiInt == -1);

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
    private void displayFinalResult(int roundResultInt)
    {
        //show final result
        finalResult.text = "You " + decodeToResultText(getFinalResultInt(roundResultInt));
        finalResult.enabled = true;
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
        Debug.Log("Testing for additional round. currentRount:" + currentRound + ".");
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
    // update the time before curRound # is updated 
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
        //timer.text = remainingTime.ToString("F2");
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
    private string decodeToChoice(int aiInt)
    {
        string aiResultString = "";

        if (aiInt == 1)
        {
            aiResultString = "Rock";
        }
        else if (aiInt == 2)
        {
            aiResultString = "Paper";
        }
        else if (aiInt == 3)
        {
            aiResultString = "Scissors";
        }
        else
        {
            aiResultString = "Invalid";
            //Debug.Log("Error, non-recognized ai result: " + aiInt);
        }

        return aiResultString;
    }

    //invalid results -> -1
    private int choiceEncode(string playerText)
    {
        int playerResult = 0;
        if (playerText.Equals("Rock"))
        {
            playerResult = 1;
        }
        else if (playerText.Equals("Paper"))
        {
            playerResult = 2;
        }
        else if (playerText.Equals("Scissors"))
        {
            playerResult = 3;
        }
        else
        {
            playerResult = -1;
            //Debug.Log("Error, non-recognized player choice: " + playerText);
        }

        return playerResult;
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
    private bool isValidGesture()
    {
        if (RPSDetection.text == "Rock" || RPSDetection.text == "Paper" || RPSDetection.text == "Scissors")
        {
            return true;
        }
        return false;
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
        if (remainingTime <= 0)
        {
            if (!timeOut)
            {
                timeOut = true;
                timer.text = "0.00";
                onClickConfirmGesture();
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
        if (Input.GetKeyUp(KeyCode.Return) && confirmGestureBtn.IsActive())
        {
            onClickConfirmGesture();
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

}
