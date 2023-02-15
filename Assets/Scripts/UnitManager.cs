using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    /// <summary>
    /// this script will manage all the units (both friendly and enemy that are currently active in our scene
    /// it is responsible for finding all active units, adding to the list if new units are 
    /// spawned, and removing units from the list if they are destroyed
    /// </summary>

    public static UnitManager Instance { get; private set; }

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        //check to see if there is more than one unit manager in our scene
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        //this function handles adding to the list when new units are spawned
        Unit unit = sender as Unit;

        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            //this adds to the enemy list
            enemyUnitList.Add(unit);
        }
        else
        {
            //this adds to the friendly list
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        //this function handles removing a unit from the list once it is dead
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList()
    {
        //this function returns the Unit List as a whole
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        //this function returns the friendly unit list
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        //this function returns the enemy unit list
        return enemyUnitList;
    }
}
