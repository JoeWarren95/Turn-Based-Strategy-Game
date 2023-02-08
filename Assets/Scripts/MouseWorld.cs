using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    //this script handles tracking the mouse position on the screen
    private static MouseWorld instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        //make this script into a singleton
        instance = this;
    }

    void Update()
    {
        transform.position = MouseWorld.GetPosition();
    }

    //this function returns the mouse position
    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);

        return raycastHit.point;

    }
}
