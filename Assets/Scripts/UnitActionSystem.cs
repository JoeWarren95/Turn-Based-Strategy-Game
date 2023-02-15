using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    
    //we want this field to be read from anywhere, but only set in this class
    public static UnitActionSystem Instance { get; private set; }

    //there can be other delegate types other than EventHandler, this is just the C# standard one
    public event EventHandler OnSelectedUnitChanged;

    public event EventHandler OnSelectedActionChanged;

    public event EventHandler<bool> OnBusyChanged;

    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitsLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        //check to see if there is more than one unit action system in our scene
        if(Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    void Update()
    {
        //this check ensures that we can only perform 1 action at a time
        if (isBusy)
        {
            return;
        }
        //this checks to see if it's the player's turn
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            //check if we are over a button or not
            return;
        }

        if (TryHandleUnitSelection())
        {
            return;
        }

        HandleSelectedAction();
        #region Code that got refactored into HandleSelectedAction() and SetBusy()
        //if (Input.GetMouseButtonDown(0))
        //{
        //if (TryHandleUnitSelection()) return;
        /*GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (selectedUnit.GetMoveAction().IsValidActionGridPosition(mouseGridPosition))
        {
            SetBusy();
            selectedUnit.GetMoveAction().Move(mouseGridPosition, ClearBusy);
        }*/

        //because MouseWorld is a Singleton, we don't need a reference to it
        //there is only ever going to be 1 instance of MouseWorld, we can just call it
        //selectedUnit.GetMoveAction().Move(MouseWorld.GetPosition(mouseGridPosition));
        //}
        /*
        if (Input.GetMouseButtonDown(1))
        {
            SetBusy();
            selectedUnit.GetSpinAction().Spin(ClearBusy);
        }*/
        #endregion
    }

    private void HandleSelectedAction()
    {
        ///This function allows are buttons to work completely through code
        ///This allows the buttons to perform their specified function without us having to manually set them
        ///up inside of Unity
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    //we want to use this code because it is cleaner
                    //we just have to make sure we add the base functions to each action script
                    SetBusy();
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                    #region old switch statement, alternative to ^^^this code
                    /*switch (selectedAction)
                    {
                        case MoveAction moveAction:
                            if (moveAction.IsValidActionGridPosition(mouseGridPosition))
                            {
                                SetBusy();
                                moveAction.Move(mouseGridPosition, ClearBusy);
                            }
                            break;
                        case SpinAction spinAction:
                            SetBusy();
                            spinAction.Spin(ClearBusy);
                            break;
                    }*/
                    #endregion

                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                }
            } 
        }
        #region Alternative Code for these^^^^ if statements
        /*GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
        {
            return;
        }
        if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
        {
            return;
        }
        SetBusy();
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);*/
        #endregion
    }

    private void SetBusy()
    {
        //this function should be called anytime an action is taking place
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        //once the action is done, clear our status
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //this line is casting a ray from the camera's position to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitsLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                    {
                        //Unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        //Unit is an enemy
                        return false;
                    }

                    //made a separate function for this just in case we needed to add any extra 
                    //logic/checks, this way we keep this if statement from getting cluttered
                    SetSelectedUnit(unit);
                    return true;
                }
                #region ALTERNATIVE CODE
                //Another way to write the above if statement
                /*Unit unit = raycastHit.transform.GetComponent<Unit>();
                if(unit != null)
                {

                }*/
                #endregion


            }
        }
        return false;
    }

    //this function sets the selected unit
    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;


        SetSelectedAction(unit.GetAction<MoveAction>());
        //this is the same code as this^^^ but this is using Generics
        //SetSelectedAction(unit.GetMoveAction());

        //this checks if there are subscribers to the event, and if there are, proceed with the event
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

        #region Alternative code null check
        //need to check if there are subscribers to the event before calling it
        /*if(OnSelectedUnitChanged != null)
        {
            //the standard is to pass in 'this' for the first parameter
            OnSelectedUnitChanged(this, EventArgs.Empty);
        }*/
        #endregion
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        //This function sets our unit's base action upon loading into the game
        selectedAction = baseAction;

        //this checks if there are subscribers to the event, and if there are, proceed with the event
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    //because we don't want to make selectedUnit public, we make this class
    //it's only function is to expose the selected unit without changing its accessability
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
