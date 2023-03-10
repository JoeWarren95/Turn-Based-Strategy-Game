using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    /// <summary>
    /// with this script, we are establishing one of the moves a player can do on their turn
    /// the move action. We simply took the code from the Unit script, and placed it here
    /// </summary>
    private List<Vector3> positionList;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;


    [SerializeField] private int maxMoveDistance = 4;

    private int currentPositionIndex;

    #region Now that we have the isActive bool, we no longer need to set the pos of our unit
    /*protected override void Awake()
    {
        base.Awake();
     
        //set an initial position for our unit
        //targetPosition = transform.position;
    }*/
    #endregion

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];

        //this line sets the direction we want to go
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;

        //this line sets the transforms forward vector to be their move direction
        //so in this way the unit is "facing" where they are going
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            
            float moveSpeed = 4f;
            //this line will move our actual character
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;

            if(currentPositionIndex >= positionList.Count)
            {
                //stop the move animation
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            
        }

    }

    //NOTE this function was originally called 'Move()' but it was refactored in "Generic Take Action"
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        //this function is responsible for moving the unit, it is public so that we can access it from
        //other scripts, without exposing the internal code to everyone
        //this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    #region REFACTORED CODE (function added to BaseAction Script)
    /*public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        ///This function determines if the player selects a valid grid position or not
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }*/
    #endregion

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        ///this function looks through all the possible valid grid positions in order to determine if we can
        ///move to a specified space or not
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();


        //these two for loops cycle through all the valid positions with in our unit's move range
        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //these three if statements check if we are choosing a valid grid position to move to
                //this first checks if the position is inside the grid bounds
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    //this key word checks if we have a valid position, if no, go to next iteration, if
                    //yes, then continue onto the next line in the code
                    continue;
                }
                //test if unit is already on that position or not
                if (unitGridPosition == testGridPosition)
                {
                    continue;
                }
                //test if grid position is already occupied with another unit
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    //this checks makes sure we don't mark grid positions that have no path to 
                    //them as walkable
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;

                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    //path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    //MAY DELETE LATER
    public int GetMoveDistance()
    {
        //expose maxMoveDistance without changing its accessability
        return maxMoveDistance;
    }

    public override string GetActionName()
    {
        //this will specify what we want to call the action button
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        //this function allows our Enemy to determine where to move, by determining how "valuable" a space is to move to
        //with this logic, the enemy will prioritize moving into a spot where it is surrounded by Units

        //this line determines how many targets are around the enemy unit
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        //int targetCountAtGridPosition = unit.GetShootAction().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            //this determines what grid position an enemy will move to, and how valuable it is to move
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
