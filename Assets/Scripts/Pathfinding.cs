using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //we want this field to be read from anywhere, but only set in this class
    public static Pathfinding Instance { get; private set; }

    //these two values (move_straight, and move_diagonal) are supposed to be 1 & 1.4
    //but they have been multiplied by 10 so that an int can be used for them
    private const int MOVE_STRAIGHT_COST = 10;

    //the reason it's 14 is bc instead of moving 1 left and 1 up, we just move diagonally
    //which is the sq root of (1^2 + 1^2)
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
        //This function we will calculate the path between the start position and the end postition and return it
        ///with this list we will create the A* algorithm, we do this by following these steps:

        #region #1 Create 2 Lists
        //the openList is the Nodes that are queued up for searching
        List<PathNode> openList = new List<PathNode>();

        //the closedList is the Nodes that have already been searched
        List<PathNode> closedList = new List<PathNode>();
        #endregion

        //set the start node and we want to add it - 
        PathNode startNode = gridSystem.GetGridObject(startGridPosition);

        //set the end node on the final point we want to reach
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);

        //- to the open list here
        openList.Add(startNode);

        //cycle through all the nodes and reset their state and then initialize them when
        //a path needs to be found
        for(int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                //we want to reset the gcost to the max value possible
                pathNode.SetGCost(int.MaxValue);

                //this is for the heuristic cost to get from one point to another
                //bc we won't have an end node upon initialization, it's best to set this to 0
                pathNode.SetHCost(0);

                pathNode.CalculateFCost();

                //this reset our calculation so that we don't carry the results from our previous calculation
                pathNode.ResetCameFromPathNode();
            }
        }

        //set up start node
        startNode.SetGCost(0);

        //this is the basic guess how long it'll take to get from point A to point B
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        //we run the algorithm only while there are still elements in the open list
        while (openList.Count > 0)
        {
            //search for the lowest cost path
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                //reached final node
                return CalculatePath(endNode);
            }

            //remove the current node and -
            openList.Remove(currentNode);

            //- add it to the closed list indicating we've already searched through it
            closedList.Add(currentNode);

            //this foreach loop gets the list of all of the neighbors to the node we're on
            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                //first we check if we have already gone through a possible node by checking if it's on the
                //closed list
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                //get a possible gCost for a particular move
                int tentativeGCost = 
                    currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                //if we have found a more optimal path
                if(tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    //reset the gcost
                    neighborNode.SetGCost(tentativeGCost);
                    //update the hcost
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    //recalculate the fcost
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

        //calculate the x and z costs separately so that we can determine the diagonal cost more accurately
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);

        int remaining = Mathf.Abs(xDistance - zDistance);

        //this line was re-written bc straight cost movement had a chance to be overwritten easily
        //return distance * MOVE_STRAIGHT_COST;
        //with this we are calculating a heuristic for the distance while also taking into account straight
        //and diagonal movement
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        //this function searches through the 8 possible move options and returns the one with the
        //lowest cost
        PathNode lowestFCostPathNode = pathNodeList[0];

        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                //if we find a path with a lower fCost than our current lowest fCost option, replace it
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        //this function returns the node for a given x and z
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        ///by using this algorithm, we've essentially turned all of our options into a binary search tree
        ///now we need to make sure we cover all our branches using this function

        List<PathNode> neighborList = new List<PathNode>();


        GridPosition gridPosition = currentNode.GetGridPosition();

        //these if checks make sure that we don't go outside of the grid bounds when looking through the possible nodes
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

        //return all of the neighbors for the node we are currently on
        return neighborList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        //this function calculates the path between our starting position and where we want
        //to go to

        //initialize a new list
        List<PathNode> pathNodeList = new List<PathNode>();

        //add the final destination to our list
        pathNodeList.Add(endNode);
        //with this we are going to start at the endNode and walk backwards through our list
        PathNode currentNode = endNode;

        //while the pathnode is not null, it means there is something connected to it
        while(currentNode.GetCameFromPathNode() != null)
        {
            //add the linked node to the list
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            //make the linked node the current node
            currentNode = currentNode.GetCameFromPathNode();
        }

        //this is a built in function that will go through our list in reverse order
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach(PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }
}
