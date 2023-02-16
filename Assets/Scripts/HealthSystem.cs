using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    /// <summary>
    /// This class handles the health bars for all of our units, it will update health
    /// bars when damage has been taken, and trigger an event when they die
    /// </summary>

    [SerializeField] private int health = 100;
    private int healthMax;

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    private void Awake()
    {
        //set healthbars to max upon loading into the game
        healthMax = health;
    }

    public void Damage(int damageAmount)
    {
        //this function handles taking damage and triggering the event when the unit
        //has no more health left
        health -= damageAmount;

        if(health < 0)
        {
            health = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if(health == 0)
        {
            Die();
        }

    }

    private void Die()
    {
        //when health = 0, trigger the Death event
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        //with this we are converting a float value into an int, this is so we only get whole
        //number values for our health bar
        return (float)health / healthMax;
    }
}
