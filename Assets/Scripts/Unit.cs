using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private HealthSystem healthSystem;

    //reference to our grid position
    private GridPosition gridPosition;

    //reference to our MoveAction script
    private MoveAction moveAction;

    //reference to our SpinAction script
    private SpinAction spinAction;

    //reference to our ShootAction script
    private ShootAction shootAction;

    //this is a reference to all of the possible actions a button could have?
    private BaseAction[] baseActionArray;

    //number of action points each unit will have on their turn
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        //moveAction = GetComponent<MoveAction>();
        //spinAction = GetComponent<SpinAction>();
        //shootAction = GetComponent<ShootAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        //set our unit's current grid position upon loading into the game
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        //subscribe to the delegate
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            //store a reference to the old position, this fixes a bug where even though we moved, the old grid
            //was still visible in our game 
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            //Unit changed Grid Position, this will change our original grid position to our new grid position
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    //with this generic action, we can replace all the individual action functions
    //the closest comparison to this would be turning our actions into something akin to List in C#
    //with List, all we need to do is use the List keyword and then define the type in <> brackets
    //we are doing the same thing here except now we are extending BaseAction rather than List
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }
    #region Possible Actions (replaced by the generic GetAction<T>() function)
    /*public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public SpinAction GetSpinAction()
    {
        return spinAction;
    }

    public ShootAction GetShootAction()
    {
        return shootAction;
    }*/

    #endregion

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }
    

    public GridPosition GetGridPosition()
    {
        //returns the Unit's position relative to our Grid
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        //returns the Unit's position relative to the world
        return transform.position;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        //this function performs both CanSpendActionPointsToTakeAction() and SpendActionPoints()
        //we expose this one so that we don't have to make SpendActionPoints() public
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        //this function tests if we have enough action points left to perform a given action
        //this could be simplified to: return actionPoints >= baseAction.GetActionPointsCost();
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        //this function handles taking away the correct number of action points based on the action
        //selected, and invokes the event letting our system know the action points have changed
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        //returns the current number of action points the unit has
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        //this function handles resetting the action points of both the player and the enemy units
        if(IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || 
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))

        actionPoints = ACTION_POINTS_MAX;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemy()
    {
        //returns if a unit is an enemy or not
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        //this function handles units taking damage by updating the amount of health remaining
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //this function handles unit death by first removing that unit from the current grid position - 
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        //- destroying the gameobject - 
        Destroy(gameObject);

        //- and invoking the correct Event
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        //this function returns the amount of health we have left but normalized so that we don't return a float value
        return healthSystem.GetHealthNormalized();
    }
}
