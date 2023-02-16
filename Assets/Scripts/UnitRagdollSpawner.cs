using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    /// <summary>
    /// this script handles spawning the ragdoll when a Unit dies
    /// </summary>

    [SerializeField] private Transform originalRootBone;

    [SerializeField] private Transform ragdollPrefab;

    private HealthSystem healthSystem;

    private void Awake()
    {
        //access the health system to listen for a unit death
        healthSystem = GetComponent<HealthSystem>();

        //trigger the death event
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //instantiate the ragdoll
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);

        //get the Ragdoll script to know what to do
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();

        //set up the bones for the ragdoll so it doesn't spawn in T-posed
        unitRagdoll.Setup(originalRootBone);
    }
}
