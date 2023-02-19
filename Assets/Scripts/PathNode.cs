using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    private GridPosition gridPosition;

    private int gCost;
    private int hCost;
    private int fCost;

    private PathNode cameFromPathNode;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    //this function will tell us through our debugger what is on a tile at a given time
    public override string ToString()
    {

        return gridPosition.ToString();
    }

    public int GetGCost()
    {
        return gCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public int GetFCost()
    {
        return fCost;
    }
}
