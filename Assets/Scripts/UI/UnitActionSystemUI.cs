using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    /// <summary>
    /// This script handles the creation of the UI for all of our Action Buttons based on whichever unit
    /// is selected. It does this by creating a list of actions, and creating a button for each item in
    /// that list
    /// </summary>

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText; 

    //to keep track of all our buttons
    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        //access the list for the number of actions this unit has
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        //add listeners to each of our appropriate events
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }


    private void CreateUnitActionButtons()
    {
        //this function creates all the buttons needed based upon the unit that is selected
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            //must include the '.gameObject' in this destroy, otherwise there will be
            //an error bc every transform MUST be connected to a gameObject
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        //this loop creates all the buttons for the unit we've selected, and grabs the appropriate script for those buttons
        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();

    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        //this event updates our visuals (unit selected circle/UI) whenever we select a different unit
        UpdateSelectedVisual();

    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        //once we perform an action, we want our actionPoints to go down, this function updates them
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {

        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        //this is the function that actually handles the resetting of the actionPoints UI
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        //this function resets the number of action points we or the enemies have once a turn has changed
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        //this function ensures that we won't have any visual bugs when updating our action points
        //due to both action points (both for the unit and the UI) getting updated on start
        //we could have a chance where one gets updated before the other, this would make it so 
        //that our UI says we have 0 action points, but in reality, we have 2 or 3 or however many
        UpdateActionPoints();
    }
}
