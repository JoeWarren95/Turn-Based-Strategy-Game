using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    /// <summary>
    /// The purpose of this script is to expose valid grid positions using a prefab we have set up
    /// All it does is cycle through the possible positions, and instantiate our prefab at valid ones
    /// </summary>
    public static GridSystemVisual Instance { get; private set; }


    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        //check to see if there is more than one Grid System Visual in our scene
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //Get the width/height of our grid
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];

        //this is where the instantiation of our visual prefab takes place
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = 
                    Instantiate(gridSystemVisualSinglePrefab, 
                    LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = 
                    gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        //listener for whenever an action has changed
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
    }

    public void HideAllGridPosition()
    {
        //cycles through both axis and hides all the visual prefabs
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        //this function allows us to see the valid move positions based on which unit we've selected
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
        }
    }

    private void UpdateGridVisual()
    {
        //This function allows us to update our visual by hiding all the possible grid positions-
        HideAllGridPosition();

        #region REFACTORED CODE bc we implemented more base functions, this code is no longer needed
        //Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        //ShowGridPositionList(selectedUnit.GetMoveAction().GetValidActionGridPositionList());
        #endregion
        
        //-determining which unit we currently have selected - 
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        //-show the possible grid positions available for that unit
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList());
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        //with this function, we won't be updating the grid visual every frame, so it's a lot more efficient
        UpdateGridVisual();
    }
}
