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
    public TextMeshProUGUI timer;                     //to show the count down timer
    public TextMeshProUGUI prompt;                    //to show time out if necessary 
    public TextMeshProUGUI RPSDetection;              //to show the captured hand gesture
    public TextMeshProUGUI roundField;                 //to show the round #
    public TextMeshProUGUI gameHistory;                //to show the history result
    public Button confirmGestureBtn;                  //to ask for the player to confirm the result


    public GameObject resultCanvas;
    public TextMeshProUGUI resultField;                 //to show the result of this round
    public TextMeshProUGUI finalResult;                 //to show the final game result--win/lose
    public GameObject nextRoundButton;                  //to navigate to the next round

    public GameObject exitButton;

    private float remainingTime = 10f;
    //private const int DEFAULTGAMELAYER = 0;
    private const int TOTALROUNDS = 3;
    private int currentRound = 0;
    private int[] historyResults = new int[TOTALROUNDS]; //store all the round results of the first 3 rounds
    private string gameHistoryText = "";                 //to show history on board
    private int gameCanvasLayer;
    private Color timerColor;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Arena Scene Starts");

        //save the pre-set timer color
        timerColor = timer.faceColor;
        //save the canvas layer number
        gameCanvasLayer = gameCanvas.GetComponent<Canvas>().sortingOrder;

        //hide gameResult canvas
        resultCanvas.SetActive(false);

        //show game canvas
        showGameCanvas(true);

    }

    // Update is called once per frame
    void Update()
    {
        startCountdown();
        //isOpponentReady(); //call this func when network is done
    }


    public void onClickConfirmGesture()
    {
        if (isValidGesture())
        {
            if (isOpponentReady())
            {
                exitButton.SetActive(false);
                showGameCanvas(false);

                updateResultCanvasInfo();

                showGameResultCanvas(true);
            }
            else
            {
                prompt.enabled = true;
                prompt.text = "waiting for the other player...";
            }
            
        } else
        {
            prompt.enabled = true;
            prompt.text = "Invalid Gesture, please confirm again!";
        }


        
    }


    private void startCountdown()
    {
        remainingTime -= 1 * Time.deltaTime;

        //Debug.Log("timerColor: " + timerColor);

        if (remainingTime < 4)
        {
            timer.faceColor = Color.red;
        }
        else
        {
            timer.faceColor = timerColor;
        }

        if (remainingTime <= 0)
        {
            //so far, no punishment
            timer.text = "0.00";
            //update prompt
            prompt.enabled = true;
            prompt.text = "Time Out!";
            prompt.faceColor = Color.red;

        }
        else
        {
            timer.text = remainingTime.ToString("F2");
        }
    }


    //display the gameResult canvas and make sure it's visible
    private void showGameResultCanvas(bool toShow)
    {
        resultCanvas.SetActive(toShow);
        resultCanvas.GetComponent<Canvas>().sortingOrder = gameCanvasLayer + 1;
    }

    //display the game canvas and set correspondent settings
    private void showGameCanvas(bool toShow)
    {
        gameCanvas.SetActive(toShow);

        if (toShow)
        {
            //refresh the timer
            remainingTime = 10f;
            timer.enabled = true;
            timer.text = remainingTime.ToString("F2");
            //diable the prompt, gameHistory and final result
            prompt.enabled = false;
            finalResult.enabled = false;
            //update rounds
            updateRoundField();
        }

    }


    public void updateResultCanvasInfo()
    {
        Debug.Log("Game Starts");

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

    public void onClickNextRound()
    {
        showGameResultCanvas(false);
        showGameCanvas(true);
    }

    private string getPlayerChoice()
    {
            return RPSDetection.text;  
    }

    private int generateAiResult()
    //not so random so far
    {
        int aiResult = 0;
        var rnd = new System.Random();
        aiResult = rnd.Next(1, 3);
        return aiResult;
    }

    private int playGame(int playerInt, int aiInt)
    {
        //decode table: 1 == Rock; 2 == Paper; 3 == Scissors
        bool playerWins = (playerInt == 1 && aiInt == 3)
                          || (playerInt == 2 && aiInt == 1)
                          || (playerInt == 3 && aiInt == 2);

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
            Debug.Log("Error, non-recognized ai result: " + aiInt);
        }

        return aiResultString;
    }

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
            Debug.Log("Error, non-recognized player choice: " + playerText);
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

    private void updateGameHistory(int resultInt)
    {
        historyResults[currentRound - 1] = resultInt;
    }

    private void updateAndDisplayHistoryInfo(int roundResultInt)
    {
        //display gameHistory
        if (!gameHistory.enabled)
        {
            gameHistory.enabled = true;
        }
        updateGameHistory(roundResultInt);//store the round result
        updateGameHistoryText(roundResultInt);
    }

    private void displayRoundResult(int playerInt, int aiInt, int roundResultInt)
    {
        string playerChoice = decodeToChoice(playerInt);
        string aiChoice = decodeToChoice(aiInt);
        string roundResultText = decodeToResultText(roundResultInt);

        resultField.enabled = true;
        resultField.text = "You: " + playerChoice + ", AI: " + aiChoice + "\n" + roundResultText;
    }

    private void displayFinalResult(int roundResultInt)
    {
        //show final result
        finalResult.text = "You " + decodeToResultText(getFinalResultInt(roundResultInt));
        finalResult.enabled = true;
    }



    //PS: immatural function, hub world has error msg.
    public void onClickExitButton()
    {
        if(SceneManager.GetActiveScene().name == "Arena")
        {
            SceneManager.LoadScene("Hub");
        }
    }

    private bool isValidGesture() {
        if(RPSDetection.text == "Rock" || RPSDetection.text == "Paper" || RPSDetection.text == "Scissors")
        {
            return true;
        }
        return false;
    }

    private bool isOpponentReady()
    {
        return true;
    }

}
