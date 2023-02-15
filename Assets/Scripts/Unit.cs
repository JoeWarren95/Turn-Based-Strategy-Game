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
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
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
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if(IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || 
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))

        actionPoints = ACTION_POINTS_MAX;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
