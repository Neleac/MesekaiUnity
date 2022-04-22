using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameMngr : MonoBehaviour
{
    public static GlobalGameMngr Instance;


    public const int TOTALROUNDS = 3;
    public int[] historyResults = new int[TOTALROUNDS]; //store all the round results of the first 3 rounds
    public string gameHistoryText = ""; //to show history on board
    public int currentRound = 0;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
