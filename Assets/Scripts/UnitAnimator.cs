using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    /// <summary>
    /// this script handles all the animations for the units
    /// </summary>

    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    private void Awake()
    {
        //this awake function tries to access the scripts required for the action we're performing
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }


    //these are all the events that require animations
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isMoving", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isMoving", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        //creates the bullet that will get shot from a unit's gun
        Transform bulletProjectileTransform = 
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        //gets the bulletprojectile script, that controls what the bullet does
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        //these few lines make it so that we are shooting at shoulder height for the enemy
        //rather than at the enemy's feet
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        //this moves our shootpoint up so that we're not shooting at the Unit's feet
        targetUnitShootAtPosition.y = shootPointTransform.position.y;
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
}
