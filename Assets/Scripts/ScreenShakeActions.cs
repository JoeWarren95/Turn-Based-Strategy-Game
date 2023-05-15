using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    //THERE IS AN ERROR, CHECK IT

    // Start is called before the first frame update
    void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    /*private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        //ScreenShake.Instance.Shake();
    }*/

    //DOUBLE CHECK THIS
    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
