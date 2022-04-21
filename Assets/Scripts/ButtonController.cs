using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ButtonController : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI roundField;
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI resultField;
    public TextMeshProUGUI gameHistory;
    public TextMeshProUGUI finalResult;

    private GameObject rockButton;
    private GameObject paperButton;
    private GameObject scissorsButton;
    private GameObject nextRoundButton;
    private float remainingTime = 10f;
    private const int TOTALROUNDS = 3;
    private int currentRound = 1;
    private int[] historyResults = new int[TOTALROUNDS]; //store all the round results of the first 3 rounds
    private string gameHistoryText = ""; //to show history on board


    //TODO: run-time error
    private void Awake()
    {
        rockButton = GameObject.Find("RockButton");
        paperButton = GameObject.Find("PaperButton");
        scissorsButton = GameObject.Find("ScissorsButton");
        nextRoundButton = GameObject.Find("nextRoundButton");
    }


    void Start() //preset components:
    {
        //timer
        timer.enabled = true;
        timer.text = remainingTime.ToString("F2");
        //prompt
        prompt.enabled = true;
        //round
        roundField.enabled = true;
        roundField.text = "Round " + currentRound;
        //result
        resultField.enabled = false;
        //nextRoundbutton
        nextRoundButton.SetActive(false);
        //historyInfo
        gameHistory.enabled = false;
        //final result
        finalResult.enabled = false;

    }

    void Update() //start countdown
    {
        startCountdown();
    }

    //when the player clicked the choice button made his choice:
    public void getPlayerResult()//TODO: rename: onClickChoiceButton
    {
        //get player and Ai choice
        string playerChoice = getPlayerChoice();
        int aiInt = generateAiResult();
        string aiChoice = decodeToChoice(aiInt);

        //game logic
        int roundResultInt = playGame(choiceEncode(playerChoice), aiInt);
        string roundResultText = decodeToResultText(roundResultInt);

        //set components invisible
        prompt.enabled = false;
        activateChoiceButtons(false);
        timer.enabled = false;

        //update result field
        resultField.enabled = true;
        resultField.text = "You: " + playerChoice + ", AI: " + aiChoice + "\n" + roundResultText;

        //if in round 1 & 2 & 3
        if (currentRound <= TOTALROUNDS)
        {
            //update gameHistory
            if (!gameHistory.enabled)
            {
                gameHistory.enabled = true;
            }
            updateGameHistory(roundResultInt);//store the round result
            updateGameHistoryText(roundResultText);

            //show nextRoundButton
            nextRoundButton.SetActive(true);
        }

        //do the following only in round 3 and above
        if (currentRound >= TOTALROUNDS)
        {
            //debug print
            foreach (var roundResult in historyResults)
            {
                Debug.Log(roundResult);
            }

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
                //TODO: animate or fade out the result

                //show final result
                finalResult.text = "You " + decodeToResultText(getFinalResultInt(roundResultInt));
                finalResult.enabled = true;
            }
        }
    }


    private string getPlayerChoice()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
    }


    private int generateAiResult()
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

        if (playerInt == aiInt)
        {
            return 0; //it's a tie
        } else if (playerWins)
        {
            return 1; //player wins
        }else
        {
            return -1;//player loses
        }
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
        } else
        {
            return roundResultInt;
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

    private void startCountdown()
    {
        remainingTime -= 1 * Time.deltaTime;

        if (remainingTime < 4)
        {
            timer.faceColor = Color.red;

        }

        if (remainingTime <= 0)
        {
            timer.text = "0.00";
            activateChoiceButtons(false);
            prompt.text = "Time Out!";
        }
        else
        {
            timer.text = remainingTime.ToString("F2");
        }
    }

    private void pauseCountdown()
    {
        
    }

    private void activateChoiceButtons(bool activated)
    {
        rockButton.SetActive(activated);
        paperButton.SetActive(activated);
        scissorsButton.SetActive(activated);
    }

    public void onClickNextRound()
    {
        //update round info
        currentRound += 1;
        if (currentRound <= TOTALROUNDS)
        {
            roundField.text = "Round " + currentRound;
        }
        else
        {
            roundField.text = "Final Round";
        }

        //hide this button
        nextRoundButton.SetActive(false);

        //hide result field
        resultField.enabled = false;

        //reset timer
        remainingTime = 10f;
        timer.enabled = true;
        Debug.Log(remainingTime);

        //activate choice buttons & prompt
        activateChoiceButtons(true);
        prompt.enabled = true;


    }

    private void updateGameHistoryText(string roundResultText)
    {
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

    private bool needAdditionalRound(int resultInt)
    {
        Debug.Log("currentRount:" + currentRound);
        if(currentRound == 3)
        {
            if(get3RoundsSum()!= 0)
            {
                return false;//no need additional round if the 3-rounds-result indicates a winner
            }
            return true;
        }else
        {
            if(resultInt == 0)
            {
                return true;//need additional round is the extra round has no winner still
            }
            return false;
        }
        
    }

}
