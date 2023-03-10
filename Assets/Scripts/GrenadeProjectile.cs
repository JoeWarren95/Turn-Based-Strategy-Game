using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private Vector3 targetPosition;

    private Action onGrenadeBehaviorComplete;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float moveSpeed = 15f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float reachedTargetDistance = .2f;
        if(Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach(Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
            }

            Destroy(gameObject);

            onGrenadeBehaviorComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete)
    {
        this.onGrenadeBehaviorComplete = onGrenadeBehaviorComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }
}
