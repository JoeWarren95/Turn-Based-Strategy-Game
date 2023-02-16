using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    /// <summary>
    /// this class handles our turns in our game, it does this by tracking what turn we're on, and invoking
    /// an event to reset certain fields whenever a turn changes
    /// </summary>

    //we want this field to be read from anywhere, but only set in this class
    public static TurnSystem Instance { get; private set; }

    private int turnNumber = 1;
    private bool isPlayerTurn = true;

    public event EventHandler OnTurnChanged;

    private void Awake()
    {
        //check to see if there is more than one unit action system in our scene
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NextTurn()
    {
        //this function handles keeping track of the turn # - 
        turnNumber++;

        //- checking if it's the player's turn -
        isPlayerTurn = !isPlayerTurn;

        //- and triggering the event to allow our system to know that a turn has changed
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        //This function allows our UI to keep track of the turn # without accessing the variable directly
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        //this function allows us to check if it's the player's turn without giving direct access to our bool
        return isPlayerTurn;
    }
}
