using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    /// This is the base script from which all Actions will derive from

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    //[SerializeField] private Animator unitAnimator;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    //we made GetActionName() and TakeAction() abstract bc we want to force all actions to
    //use these functions
    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        ///This function determines if the player selects a valid grid position or not
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        //we made this virtual bc we just want to define a default cost for all actions
        //we can override it in another function if we want that action to cost more
        //See: SpinAction().GetActionPointsCost()
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        //this function will be called whenever we take and start an action
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        //once an action is complete, this function will reset our system
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        //this function is the "brain" behind our enemy AI

        //this line returns what possible actions are available
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        //this line returns the valid grid positions given the action selected
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        //this loop is what will activate the valid grid positions based on the action the unit decides to take
        foreach(GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        //this if statement will return the enemy action that is most valuable
        if(enemyAIActionList.Count > 0)
        {
            //it does this by sorting the list from greatest actionValue to least -
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);

            //- and returning the top item in the list, this is the action we will take
            return enemyAIActionList[0];
        }
        else
        {
            //no possible enemy AI actions
            return null;
        }
        
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
