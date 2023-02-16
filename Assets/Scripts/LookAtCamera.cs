using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    /// <summary>
    /// the purpose of this script is to make sure the UI on our Units is always
    /// facing the camera
    /// </summary>

    [SerializeField] private bool invert;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        //With these 2 if statements, we are always ensuring that our UI is facing our camera
        if (invert)
        {
            //this ensures our UI isn't reversed when we're looking at it
            Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position + directionToCamera * -1);
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
    }
}
