using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    /// <summary>
    /// This script handles creating and destroying the trail whenever a bullet is fired
    /// </summary>

    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        //set the target position to the position of the Unit getting shot at
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        //set the direction of the bullet
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        //calculate the distance the bullet has to travel
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        float moveSpeed = 200f;

        //this line handles the movement of the bullet
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        //this statement handles the destruction of the bullet trail
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            //set the bullets position to the enemies position
            transform.position = targetPosition;

            //unparent the trail from the bullet prefab
            trailRenderer.transform.parent = null;

            //destroy the bullet
            Destroy(gameObject);

            //spawn the vfx on hit
            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }
    }
}
