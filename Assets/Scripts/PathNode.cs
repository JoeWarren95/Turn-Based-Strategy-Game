using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    /// <summary>
    /// This script for our grid object that represents a single pathfinding node
    /// </summary>

    private GridPosition gridPosition;

    //how long it takes to get from point A to point B
    //I see it as the "General Cost"
    private int gCost;

    //heuristic cost, an estimation of the distance it takes to get from point A to point
    //B, it is basically an educated guess
    private int hCost;

    //the gCost + the hCost
    private int fCost;

    //the reference to the path node we came from to reach the node we are currently on
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

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }

    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        //this function manually resets the node we came from in the event that we've 
        //found a more optimal path
        cameFromPathNode = pathNode;
    }

    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
}
