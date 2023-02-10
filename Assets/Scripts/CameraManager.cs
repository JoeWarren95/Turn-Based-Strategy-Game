using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                //get the Unit thats shooting, and the Unit that's getting shot
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                //offset the camera to be at about the height of the character
                //otherwise the camera will be at the Unit's feet
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                //aim at the unit getting shot
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmount = 0.5f;

                //this Vector3 positions the camera at the right shoulder of the Unit
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

                //with this Vector3, we set where we want the action camera to be by: getting the position of the firing unit, 
                //moving it up to about the Unit's shoulder height, then moving it slightly to the right, and then finally moving 
                //it slightly behind the shoulder
                Vector3 actionCameraPosition = 
                    shooterUnit.GetWorldPosition() + 
                    cameraCharacterHeight + 
                    shoulderOffset + 
                    (shootDirection * -1);

                //these 2 lines will position the action camera, and rotate it to where we want it to look
                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }
}
