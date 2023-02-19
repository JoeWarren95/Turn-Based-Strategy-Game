using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    /// <summary>
    /// this script allows us to test if our grid is being properly updated or not
    /// </summary>
    [SerializeField] private TextMeshPro textMeshPro;

    //private GridObject gridObject;
    private object gridObject;

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    //this update function will change our debug text whenever there is a unit on top of one of
    //the grid tiles
    protected virtual void Update()
    {
        textMeshPro.text = gridObject.ToString();
    }
}
