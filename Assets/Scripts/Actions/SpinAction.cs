using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    //TODO: RESEARCH DELEGATES

    private float totalSpinAmount;


    void Update()
    {
        if (!isActive)
        {
            return;
        }

        //need to calculate when it's made 1 revolution and stop after that
        float spinAddAmount = 360f * Time.deltaTime;
        //transform.eulerAngles.y / 360;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;
        if(totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    //NOTE this function was originally called 'Spin()' but it was refactored in "Generic Take Action"
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //this is the function that handles spinning the unit
        //The gridPosition does nothing here, but we need it to match the original signature
        totalSpinAmount = 0f;

        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        //this will specify what we want to call the action button
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        ///this function looks through all the possible valid grid positions in order to determine if we can
        ///move to a specified space or not
        //List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        //refactored version of original List<GridPosition> declaration ^^
        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override int GetActionPointsCost()
    {
        //we want the spin action to cost 2 points to use
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

}
