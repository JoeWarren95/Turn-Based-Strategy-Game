using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    //this script may need to talk with the MoveAction script to access
    //what the player's max move distance is

    [SerializeField] private MeshRenderer meshRenderer;

    public void Show()
    {
        //this function shows the visual prefab
        //if w/in the player's move range
        //can be called if we ever need to activate the prefab
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        //this function hides the visual prefab
        //if not in player's move range
        //can be called whenever we need to deactivate the prefab
        meshRenderer.enabled = false;
    }
}
