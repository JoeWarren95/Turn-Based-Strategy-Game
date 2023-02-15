using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    /// <summary>
    /// This script handles our enemies taking turns once the player has ended theirs
    /// </summary>

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        //set the default state to waiting as the game will always start with the player first
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        //this checks to see if it's the player's turn
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        //this sets the state to Busy once an action has started
                        state = State.Busy;
                    }
                    else
                    {
                        //all enemy action points are gone, end the turn
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }

        
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            //if it isn't the player's turn, set the enemy state to taking their turn, and start the timer
            state = State.TakingTurn;
            timer = 2f;
        }


    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        //this function will allow the enemy to try to take a turn if they have action points available
        foreach(Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            //this foreach loop will cycle through all the Enemy Units in the EnemyUnitList
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                //this if statement will will force the enemy units to spend all of their action points if they're available
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                //enemy can't afford whatever action
                continue;
            }

            if(bestEnemyAIAction == null)
            {
                
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }

            
        }

        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }

        #region Old Code that was refactored with the above foreach loop
        /*SpinAction spinAction = enemyUnit.GetSpinAction();

        GridPosition actionGridPosition = enemyUnit.GetGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosition))
        {
            return false;
        }
        if (!enemyUnit.TrySpendActionPointsToTakeAction(spinAction))
        {
            return false;
        }

        Debug.Log("Spin Action!");
        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;*/
        #endregion
    }
}
