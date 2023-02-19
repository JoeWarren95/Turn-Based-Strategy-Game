using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//removed MonoBehaviour bc we won't extend it in this class
//we want to be able to use the constructor to create our grid system
//if we extend MonoBehaviour, then we won't be able to use the constructor
public class GridSystem<TGridObject>
{
    //this script is responsible for creating/handling our grid

    private int width;
    private int height;
    private float cellSize;

    //by adding a ',' in the brackets, we are specifying that this is a 2D array
    //this variable is used to store the reference for our grid objects so that they aren't destroyed after being created
    private TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        //this line is what creates our grid
        gridObjectArray = new TGridObject[width, height];
        
        //this nested for loop populates our grid, and tracks what objects are on a grid position at any given time
        //again first loop creates the X axis, second loop creates the Z axis
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                //the below 2 lines of code are the same, the top one just uses delegates
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
                //gridObjectArray[x, z] = new TGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        //this function translates our world position into a position on our grid
        //return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
        return new Vector3(gridPosition.x, 0.02f, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        //this function makes it so that we can only move in whole grid positions and not percentages of grid positions
        //it is converting our world position into a grid position
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
            );
    }

    //this function is simply for debug purposes
    public void CreateDebugObjects(Transform debugPrefab)
    {
        //these 2 loops create our grid, first loop creates the X axis of the grid, second loop creates the Z axis of the
        //grid
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                //these lines are what create the rest of the grid
                //first we instantiate the transforms at their given grid positions from a world position,
                //then the next 2 lines set the grid debug objects at their given grid positions
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));

            }
        }
    }

    //get a grid object from a grid position
    //this function allows us to see what tile our unit is currently occupying
    //it takes our world position, and converts it into a grid position that we are able to see through our debugger
    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        //only returns positive if the position selected is a valid x,z coordinate
        return gridPosition.x >= 0 && 
            gridPosition.z >= 0 && 
            gridPosition.x < width && 
            gridPosition.z < height;
    }

    public int GetWidth()
    {
        //exposes the width of our grid
        return width;
    }

    public int GetHeight()
    {
        //exposes the height of our grid
        return height;
    }
}
