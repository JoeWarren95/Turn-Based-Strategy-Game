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

    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    //this update function will change our debug text whenever there is a unit on top of one of
    //the grid tiles
    private void Update()
    {
        textMeshPro.text = gridObject.ToString();
    }
}
