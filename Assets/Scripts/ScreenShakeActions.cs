using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }

    /*private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgsEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }*/
}
