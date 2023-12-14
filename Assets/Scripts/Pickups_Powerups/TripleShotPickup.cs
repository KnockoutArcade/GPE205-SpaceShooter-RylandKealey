using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShotPickup : Pickup
{
    public TripleShotPowerup powerup;

    public void OnTriggerEnter(Collider other)
    {
        // Get the PowerupManager object
        PowerupManager powerupManager = other.GetComponent<PowerupManager>();

        // If it has a powerup Manager
        if (powerupManager != null)
        {
            // Remove any existing shot powerups, as we need to refresh our shot powerup time.
            powerupManager.RemoveOfType(powerup.GetType());

            // Add the powerup
            powerupManager.Add(powerup);

            // Destroy this pickup
            Destroy(gameObject);
        }
    }
}
