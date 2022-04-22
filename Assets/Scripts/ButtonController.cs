using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

    public TextMeshProUGUI roundField;
    public TextMeshProUGUI resultField;
    public TextMeshProUGUI gameHistory;
    public TextMeshProUGUI finalResult;

    private GameObject nextRoundButton;

    //get components from previous scene
    private GameObject previousCanvas;
    private TextMeshProUGUI gesturePrevScene;
    private TextMeshProUGUI timer;
    private TextMeshProUGUI prompt;
    private Button confirmPrevBtn;

    //find and link button objects
    private void Awake()
    {
        nextRoundButton = GameObject.Find("nextRoundButton");

        previousCanvas = GameObject.Find("gameCanvas");
        //gesturePrevScene = previousCanvas.GetComponentsInChildren<TextMeshProUGUI>()[0];
        //confirmPrevBtn = previousCanvas.GetComponentInChildren<Button>(); 
        //timer = previousCanvas.GetComponentsInChildren<TextMeshProUGUI>()[2];
        //prompt = previousCanvas.GetComponentsInChildren<TextMeshProUGUI>()[3];


        TextMeshProUGUI[] texts = previousCanvas.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length == 3)
        {
            gesturePrevScene = texts[0];
            timer = texts[1];
            prompt = texts[2];
        }
        else
        {
            gesturePrevScene = texts[0];
            timer = texts[2];
            prompt = texts[3];
        }

    }


    void Start() //preset components:
    {
        //disable previous components
        activePrevCanvasComponents(false);
        //round
        roundField.enabled = true;
        roundField.text = "Round " + GlobalGameMngr.Instance.currentRound;
        //result
        resultField.enabled = false;
        //nextRoundbutton
        nextRoundButton.SetActive(false);
        //historyInfo
        gameHistory.enabled = false;
        //final result
        finalResult.enabled = false;

        if(GlobalGameMngr.Instance != null)
        {
            startGame();
        }
        
    }

    void Update() 
    {
    }

    public void startGame()
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


        //update round info
        updateRoundField();

        //if in round 1 & 2 & 3
        if (GlobalGameMngr.Instance.currentRound <= GlobalGameMngr.TOTALROUNDS)
        {
            updateAndDisplayHistoryInfo(roundResultInt);

            //show nextRoundButton
            nextRoundButton.SetActive(true);
        }

        //do the following only in round 3 and above
        if (GlobalGameMngr.Instance.currentRound >= GlobalGameMngr.TOTALROUNDS)
        {
            //debug print
            //foreach (var roundResult in historyResults)
            //{
            //    Debug.Log(roundResult);
            //}

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
                //TODO: animate or fade out the round result

                displayFinalResult(roundResultInt);
            }
        }
    }

    public void onClickNextRound()
    {
        //TODO: go to the previous scene & dontdestroyonload

        backToAvatarScene();


        ////update round info
        //updateRoundField();

        ////hide this button itself
        //nextRoundButton.SetActive(false);

        ////hide round result field
        //resultField.enabled = false;

        ////reset timer
        ////remainingTime = 10f;
        ////timer.enabled = true;
        //// Debug.Log(remainingTime);

        ////activate choice buttons & prompt
        ////activateChoiceButtons(true);
        //prompt.enabled = true;
    }

    private string getPlayerChoice()
    {
        //return EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        return gesturePrevScene.text;
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

        if(aiInt == playerInt)
        {
            return 0; //it's a tie
        }else if (playerWins)
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
        if (GlobalGameMngr.Instance.currentRound == 3)
        {
            int sum = 0;
            foreach (var item in GlobalGameMngr.Instance.historyResults)
            {
                sum += item;
            }
            return sum;
        }

        Debug.Log("Error: access 3-rounds-result at round: " + GlobalGameMngr.Instance.currentRound);
        return Int32.MinValue;
    }

    private int getFinalResultInt(int roundResultInt)
    {
        if (GlobalGameMngr.Instance.currentRound == 3)
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
        } else
        {
            return roundResultInt;
        }
        

    }

    private bool needAdditionalRound(int resultInt)
    {
        Debug.Log("Testing for additional round. currentRount:" + GlobalGameMngr.Instance.currentRound + ".");
        if (GlobalGameMngr.Instance.currentRound == 3)
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


    private void pauseCountdown()
    {
        
    }


    private void updateRoundField()
    {
        GlobalGameMngr.Instance.currentRound += 1;
        if (GlobalGameMngr.Instance.currentRound <= GlobalGameMngr.TOTALROUNDS)
        {
            roundField.text = "Round " + GlobalGameMngr.Instance.currentRound;
        }
        else
        {
            roundField.text = "Final Round";
        }
    }

    private void updateGameHistoryText(int roundResultInt)
    {
        string roundResultText = decodeToResultText(roundResultInt);
        if (GlobalGameMngr.Instance.currentRound == 1)
        {
            GlobalGameMngr.Instance.gameHistoryText = "Round 1: " + roundResultText;
        }
        else
        {
            GlobalGameMngr.Instance.gameHistoryText += "\nRound " + GlobalGameMngr.Instance.currentRound + ": " + roundResultText;
        }
        gameHistory.text = GlobalGameMngr.Instance.gameHistoryText;
    }

    private void updateGameHistory(int resultInt)
    {
        GlobalGameMngr.Instance.historyResults[GlobalGameMngr.Instance.currentRound - 1] = resultInt;
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


    private void backToAvatarScene()
    {
        if (SceneManager.GetActiveScene().name == "faceOffPrototype")
        {
            SceneManager.LoadScene("Avatar", LoadSceneMode.Single);
        }
        Debug.Log("Loading Avatar scene");
    }


    private void activePrevCanvasComponents(bool isActive)
    {
        timer.enabled = isActive;
        gesturePrevScene.enabled = isActive;
        prompt.enabled = isActive;
        //confirmPrevBtn.enabled = isActive;
            //enabled = isActive;
    }

}
