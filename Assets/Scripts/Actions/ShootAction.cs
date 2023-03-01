using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    //an enum for all the states of our ShootAction
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private LayerMask obstaclesLayerMask;

    private State state;
    private int maxShootDistance = 7;
    private float stateTimer;

    private Unit targetUnit;
    private bool canShootBullet;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Aim();
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        //this function handles the transistions of our Shoot action
        //it does this by stating what to do when Aiming, Shooting, and during Cooloff
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }

    }

    private void Aim()
    {
        //this function handles our enemy unit Aiming at the Player

        #region my attempt at rotating towards the player
        //rotate towards the enemy
        //Vector3 shootDirection = (targetUnit.transform.position - transform.position).normalized;
        #endregion

        //rotate unit towards the player
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        //this function invokes the Shoot event and determines how much damage one shot will do
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        targetUnit.Damage(40);
    }

    public override string GetActionName()
    {
        //this function returns the specific name of the action
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        //this function returns the valid grid positions around/on top of our unit, given the action that is selected
        GridPosition unitGridPosition = unit.GetGridPosition();

        return GetValidActionGridPositionList(unitGridPosition);
    }


    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        ///this function looks through all the possible valid grid positions in order to determine if we can
        ///move to a specified space or not
        List<GridPosition> validGridPositionList = new List<GridPosition>();



        //these two for loops cycle through all the valid positions with in our unit's move range
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
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

                //this check will give us a diamond shaped range, rather than a square
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > maxShootDistance)
                {
                    continue;
                }

                //test if grid position is not occupied by another unit
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                //bc enemies share the same logic as the player, we want to make sure enemies can't target
                //and shoot at other enemies
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //both units on the grid position are on the same 'team'
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if(Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask))
                {
                    //blocked by an obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //this function allows our enemy to take its ShootAction

        //first it gets a valid unit to target
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        //sets its state to Aiming, and starts the timer for how long aiming should take
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        //once done it sets itself to complete
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        //this function returns a unit to target
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        //this function exposes our maxShootDistance variable without giving away it's accessability
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        //this function determines how valuable it is to perform a ShootAction
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            //here we set the actionValue by determining which Unit has taken the most damage
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        //This function returns how many valid targets (could be player units, or could be obstacles/items) are around our enemy Unit
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
