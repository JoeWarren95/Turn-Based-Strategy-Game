using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    /// <summary>
    /// The point of this script is to manage our entire game's level grid
    /// </summary>

    //we want this field to be read from anywhere, but only set in this class
    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;

    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        //check to see if there is more than one LevelGrid in our scene
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //this is setting the dimensions of our grid for now, will be replaced later
        gridSystem = new GridSystem<GridObject>(10, 10, 2f, 
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));

        //this line creates the debug objects which atm is just text saying '1,2'
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    //these four functions Add/Get/Remove UnitAtGridPosition will help our grid recognize when a tile is empty, occupied, or
    //a unit is leaving a tile that it was previously on
    //we're using a list for each position so that units can cross over one another without causing an error
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        //with this function 2 objects can be on the same tile at once
        //this is only meant to prevent an error if one unit crosses over a space that is already occupied by another unit
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        //once a unit moves off a grid position, we want to clear it from the list
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        //this function transitions a Unit from its original grid position to its new grid position
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    //this line will convert our unit's current world position to a Grid Position
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    #region Alternative code to this ^^^ line
    /*public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return gridSystem.GetGridPosition(worldPosition);
    }*/
    #endregion
    
    //this line gets our Unit's current world position
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    //this line checks if we've chosen a valid grid position by talking with our GridSystem script
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    //talks with GridSystem to get the width of our grid
    public int GetWidth() => gridSystem.GetWidth();

    //talks with GridSystem to get the height of our grid
    public int GetHeight() => gridSystem.GetHeight();


    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        //this function determines if there is a Unit on a grid position
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();

    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        //this function determines if there is a Unit on a grid position
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();

    }
}
