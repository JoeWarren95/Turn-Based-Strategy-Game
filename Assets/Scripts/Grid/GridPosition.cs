


using System;

/// <summary>
/// this script is a struct bc we are passing in a copy of the values of the Unit's grid position
/// and not a reference to their original position
/// </summary>
public struct GridPosition : IEquatable<GridPosition>
{
    /// <summary>
    /// the point of this script is to give our unit a grid position 
    /// we aren't using the built in Vector2Int struct bc we are using the X,Z axis, and Vector2Int uses
    /// the X,Y axis
    /// </summary>

    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    //this function will give us our current grid position
    public override bool Equals(object obj)
    {
        if (!(obj is GridPosition))
        {
            return false;
        }

        var position = (GridPosition)obj;
        return x == position.x &&
               z == position.z;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public override string ToString()
    {
        return "x: " + x + "; z: " + z;
        //could also put: return $"x: {x}; z: {z}";
    }


    //bc we don't derive from Monobehaviour in some of our scripts, these functions below allow
    //us to handle some basic functions that would be taken care of by Monobehaviour
    public static bool operator == (GridPosition a, GridPosition b)
    {
        //Equality Operator
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator != (GridPosition a, GridPosition b)
    {
        //Not equal to
        return !(a == b); 
    }

    public static GridPosition operator + (GridPosition a, GridPosition b)
    {
        //Addition
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        //Subtraction
        return new GridPosition(a.x - b.x, a.z - b.z);
    }
}