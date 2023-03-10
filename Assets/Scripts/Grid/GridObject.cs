using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    /// <summary>
    /// this script will contain a list of all the units that are on a certain
    /// grid position
    /// </summary>
    
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        //this is the constructor that will set all the units on a grid position
        //it is public so that it can be accessed from outside this script
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    //this function will tell us through our debugger what is on a tile at a given time
    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += unit + "\n";
        }

        return gridPosition.ToString() + "\n" + unitString;
    }

    public void AddUnit(Unit unit)
    {
        //this function temporarily adds a unit to our list
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        //this function removes a unit from our list
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        //this function returns the unitList from a given grid position
        return unitList;
    }

    public bool HasAnyUnit()
    {
        //this function returns how many units are on a grid position if it's greater than 0
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        //this function returns the object/unit that is on that specific grid position
        //this way we make sure we are doing damage to the correct unit
        if (HasAnyUnit())
        {
            //return the first item in the list from that grid object
            return unitList[0];
        }
        else
        {
            return null;
        }
    }
}
