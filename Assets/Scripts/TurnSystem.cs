using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
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
        turnNumber++;

        isPlayerTurn = !isPlayerTurn;

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
