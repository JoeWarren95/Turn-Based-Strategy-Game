using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //we want this field to be read from anywhere, but only set in this class
    public static Pathfinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        //check to see if there is more than one Pathfinding system in our scene
        if (Instance != null)
        {
            Debug.LogError("There's more than one Pathfinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<PathNode>(10, 10, 2f, 
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        ///with this list we will create the A* algorithm, we do this by following these steps:

        #region #1 Create 2 Lists
        //the openList is the Nodes that are queued up for searching
        List<PathNode> openList = new List<PathNode>();

        //the closedList is the Nodes that have already been searched
        List<PathNode> closedList = new List<PathNode>();
        #endregion

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for(int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        //set up start node
        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                //reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);

            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                int tentativeGCost = 
                    currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                //if we have found a more optimal path
                if(tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //No path found
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        //this function calculates the straight distance between 2 points without taking into account
        //walls or anything else that may be in the way
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);

        int remaining = Mathf.Abs(xDistance - zDistance);
        
        //return distance * MOVE_STRAIGHT_COST;
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {

            //this returns the left node
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

            if(gridPosition.z - 1 >= 0)
            {
                //this returns the left down node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));

            }

            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //this returns the left up node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));

            }

        }

        if(gridPosition.x + 1 < gridSystem.GetWidth())
        {
            //this returns the right node
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

            if (gridPosition.z - 1 >= 0)
            {
                //this returns the right down node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));

            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //this returns the right up node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }

        }

        if (gridPosition.z - 1 >= 0)
        {
            //this returns the down node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }

        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            //this returns the up node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return neighborList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach(PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }
}
